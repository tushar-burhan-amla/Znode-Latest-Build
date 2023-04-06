using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Admin
{
    public class ZnodeReceiptHelper
    {
        #region Private Variables
        protected String _output = string.Empty;
        #endregion

        #region Constructor
        public ZnodeReceiptHelper(string receiptTemplate)
        {
            _output = receiptTemplate;
        }
        #endregion

        #region Public Property
        public string Output
        {
            get { return _output; }
        }
        #endregion

        #region Public Method        
        //to substitute the data into template variables from datarow
        public virtual void Parse(DataTableReader rdrData)
        {
            if (rdrData.Read())
            {
                Replace(ref _output, rdrData);
            }
        }

        //to replaces a single string value without any formatting
        public virtual void Parse(string fieldName, string fieldValue)
        {
            string tempFieldName = $"#{ fieldName }#";
            _output = _output.Replace(tempFieldName, fieldValue);
        }

        //To replaces a single string value without any formatting for form builder.
        public virtual void ParseForFormBuilder(DataTableReader rdrData)
        {
            if (rdrData.Read())
            {
                ReplaceForFormBuilder(ref _output, rdrData);
            }
        }

        //to replaces template variables with a repeating block of data.
        public virtual void Parse(string RepeatSection, DataTableReader rdrData)
        {
            string origHtmlBlock;
            string repeatBlock;
            HtmlParser origHtml = new HtmlParser(_output);

            RepeatSection = $"#{ RepeatSection }#";

            // Find where we are to repeat.
            int currentIndex = _output.IndexOf(RepeatSection);
            origHtml.CurIndex = currentIndex;
            origHtml.ParseHtml();

            // Save off our original block of code.
            origHtmlBlock = origHtml.HtmlBlock;

            // Get the block of code that will be repeated.
            repeatBlock = origHtmlBlock.Replace(RepeatSection, "");

            // Start off a buffer for out output.
            StringBuilder newOutput = new StringBuilder(_output.Length);

            // Add the repeat blocks and substitute the data.
            string tempRepeat = "";
            while (rdrData.Read())
            {
                tempRepeat = repeatBlock;
                Replace(ref tempRepeat, rdrData);
                newOutput.Append(tempRepeat);
            }

            newOutput.Append(origHtml.EndTag);

            // Write our new block back to our output string.
            if (origHtmlBlock.Length > 0 && newOutput.Length > 0)
                _output = _output.Replace(origHtmlBlock, newOutput.ToString());
        }

        //to replaces template variables with a repeating block of data.
        public virtual void RemoveHTML(string removeSection)
        {
            string origHtmlBlock;
            string repeatBlock;
            HtmlParser origHtml = new HtmlParser(_output);

            removeSection = $"#{ removeSection }#";

            // Find where we are to repeat.
            int curIndx = _output.IndexOf(removeSection);
            origHtml.CurIndex = curIndx;
            origHtml.ParseHtml();

            // Save off our original block of code.
            origHtmlBlock = origHtml.HtmlBlock;

            // Get the block of code that will be repeated.
            repeatBlock = origHtmlBlock.Replace(removeSection, "");

            // Start off a buffer for out output.
            StringBuilder newOutput = new StringBuilder(_output.Length);            

            newOutput.Append(origHtml.EndTag);

            // Write our new block back to our output string.
            if (origHtmlBlock.Length > 0 && newOutput.Length > 0)
                _output = _output.Replace(origHtmlBlock, newOutput.ToString());
        }

        //Replace the token for form  builder.
        protected virtual void ReplaceForFormBuilder(ref string output, DataTableReader rdrData)
        {
            for (int counter = 0; counter < rdrData.FieldCount; counter++)
            {
                string fieldName = rdrData.GetName(counter);
                string baseFieldName = fieldName;

                // Build up a regular expression to replace the field name.
                // Build up a regular expression to replace the field name.
                fieldName = @"\#" + fieldName + @"\#";
                Regex regex = new Regex(fieldName, RegexOptions.IgnoreCase);
                MatchCollection matches = regex.Matches(output);
                output = ReplaceValues(output, rdrData, counter, matches);
            }
        }

        //Replace the value for the receipt.
        protected virtual string ReplaceValues(string output, DataTableReader rdrData, int counter, MatchCollection matches)
        {
            foreach (Match match in matches)
            {
                // Get the formatting.
                string type = "";
                char[] sep = { '.' };
                string[] field = match.ToString().Split(sep);
                if (field.Length > 1)
                {
                    type = field[field.Length - 1];

                    // Remove the trailing "}".
                    type = type.Substring(0, type.Length - 1);
                }

                // Try and format the value based on the template. This may fail if the 
                // template writer has specified an inappropriate format type 
                // (i.e. formatting a string to currency).                    
                string newVal = $" *** { match.ToString() } *** ";
                try
                {
                    if (type.Length > 0 && DBNull.Value != rdrData[counter])
                        newVal = $"#0:{ rdrData[counter]}#";
                    else
                        newVal = rdrData[counter].ToString();
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                }
                output = output.Replace(match.ToString(), newVal);
            }

            return output;
        }

        public void ParseWithGroup(string RepeatSection, List<DataTable> dataTable)
        {
            string origHtmlBlock;
            string repeatBlock;
            HtmlParser origHtml = new HtmlParser(_output);

            RepeatSection = $"#{ RepeatSection }#";

            // Find where we are to repeat.
            int currentIndex = _output.IndexOf(RepeatSection);
            origHtml.CurIndex = currentIndex;
            origHtml.ParseHtml();

            // Save off our original block of code.
            origHtmlBlock = origHtml.HtmlBlock;

            // Get the block of code that will be repeated.
            repeatBlock = origHtmlBlock.Replace(RepeatSection, "");

            // Start off a buffer for out output.
            StringBuilder newOutput = new StringBuilder(_output.Length);

            // Add the repeat blocks and substitute the data.
            foreach (DataTable group in dataTable)
            {
                int count = group.Rows.Count;
                foreach (DataRow row in group.Rows)
                {
                    row["GroupingRowspan"] = count;
                    row["GroupingDisplay"] = "display: none !important;";
                }

                group.Rows[0].SetField("GroupingDisplay", " ");

                string tempRepeat = "";
                DataTableReader rdrData = group.CreateDataReader();
                while (rdrData.Read())
                {
                    tempRepeat = repeatBlock;
                    Replace(ref tempRepeat, rdrData);
                    newOutput.Append(tempRepeat);
                }
            }

            newOutput.Append(origHtml.EndTag);

            // Write our new block back to our output string.
            if (origHtmlBlock.Length > 0 && newOutput.Length > 0)
                _output = _output.Replace(origHtmlBlock, newOutput.ToString());
        }
        #endregion

        #region Private Method 
        // Replaces template variables in the string   
        protected virtual void Replace(ref string output, DataTableReader rdrData)
        {
            for (int counter = 0; counter < rdrData.FieldCount; counter++)
            {
                string fieldName = rdrData.GetName(counter);
                string baseFieldName = fieldName;

                // Build up a regular expression to replace the field name.
                fieldName = @"\#" + fieldName + @".*?\#";
                Regex regex = new Regex(fieldName, RegexOptions.IgnoreCase);

                MatchCollection matches = regex.Matches(output);

                output = ReplaceValues(output, rdrData, counter, matches);
            }
        }

        protected virtual string GetTemplateRepeatSection(string template, string repeatSectionKey, string tag)
        {
            string endtag = "</tr>";
            switch (tag)
            {
                case "tr":
                    tag = "<tr>";
                    endtag = "</tr>";
                    break;
                default:
                    break;
            }

            int startIndex = template.IndexOf(repeatSectionKey);
            string lineItemTemplate = template.Substring(startIndex, template.Length - startIndex);
            int endIndex = lineItemTemplate.IndexOf("</tr>");
            string result = template.Substring(startIndex, endIndex);
            return string.Concat(tag, result, endtag);
        }
        #endregion        
    }
}
