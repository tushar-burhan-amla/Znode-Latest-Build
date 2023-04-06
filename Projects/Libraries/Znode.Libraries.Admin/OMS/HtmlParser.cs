using System;
using System.Text.RegularExpressions;

namespace Znode.Libraries.Admin
{
    // Various functions for parsing HTML blocks.
    public class HtmlParser
    {
        #region Private Variables
        string _html = "";
        int _curIndex = 0;
        int _blockStart = 0;
        int _blockEnd = 0;
        int _contentStart = 0;
        int _contentEnd = 0;
        #endregion

        #region Public Property       
        public string HtmlTxt
        {
            get { return _html; }
            set
            {
                _html = value;
            }
        }

        public int CurIndex
        {
            get { return _curIndex; }
            set { _curIndex = value; }
        }

        public string HtmlBlock
        {
            get
            {
                string ret = "";
                if (_blockEnd > _blockStart)
                {
                    ret = _html.Substring(_blockStart, _blockEnd - _blockStart);
                }

                return ret;
            }
        }

        public string HtmlBlockContents
        {
            get
            {
                string ret = "";
                if (_contentEnd > _blockStart)
                {
                    ret = _html.Substring(_contentStart, _contentEnd - _contentStart);
                }

                return ret;
            }
        }

        public string BeginTag
        {
            get
            {
                string ret = "";
                if (_contentEnd > _blockStart)
                {
                    ret = _html.Substring(_blockStart, _contentStart - _blockStart);
                }

                return ret;
            }
        }

        public string EndTag
        {
            get
            {
                string ret = "";
                if (_blockEnd > _contentEnd)
                {
                    ret = _html.Substring(_contentEnd + 1, _blockEnd - _contentEnd - 1);
                }

                return ret;
            }
        }
        #endregion

        #region Constructor
        public HtmlParser(string html)
        {
            HtmlTxt = html;
        }
        #endregion

        #region Public Methods
        /// <summary>
        // Given an index into an HTML string, this function finds the surrounding
        /// HTML block (that part including and between "<tag>" and "</tag>".
        /// </summary>
        /// <param name="html">The HTML you want to parse.</param>
        /// <param name="index">The index into the HTML that you want to start at.</param>
        /// <returns>The HTML block surrounding the the index.</returns>
        public void ParseHtml()
        {
            Regex regexStart;
            Regex regexEnd;
            string substr;

            // As an example, assume our HTML block looks like:
            // <table><tr bgcolor='white'><td>abcd</td></tr>
            //            ^
            // Assume our _curIndex is 11 which points to the "b" in bgcolor.

            // Back up to the beginning of our section and find its _curIndex.
            // Should point to the beginning of "<tr" in our example.
            if (_curIndex > 0)
            {
                substr = _html.Substring(0, _curIndex);
                _blockStart = substr.LastIndexOf('<');
            }
            else
            {
                return;
            }

            // Calculate where the beginning  of the content is (<td>abcd</td>).
            _contentStart = _html.IndexOf('>', _blockStart) + 1;

            // Get the tag name of this section ("tr" in our example).
            regexStart = new Regex(@"\w*");
            Match startTag = regexStart.Match(_html, _blockStart + 1);

            // Now build the end tag ("</tr>" in our example) and find its _curIndex.
            // Make sure we pick up the last index and not just a nested tag.
            regexEnd = new Regex("</" + startTag + ">", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            Match endTag;
            int tempStartInd = _blockStart;
            {
                // Find the end tag.
                endTag = regexEnd.Match(_html, tempStartInd);

                // Now check to see if there is a start tag in there. If so then we just
                // found the mate to a nested tag.
                tempStartInd = _html.IndexOf("<" + startTag, tempStartInd + 1, _blockEnd, StringComparison.OrdinalIgnoreCase);

            } while (tempStartInd > 0 && tempStartInd < endTag.Index);

            _blockEnd = endTag.Index;

            if (_blockEnd <= _blockStart)
            {
                Exception ex = new Exception("Couldn't find matching \"" + endTag.Value + "\" to \"" + startTag.Value + "\"");
                throw ex;
            }

            // Adjust our pointers so that they are correct.
            _contentEnd = _blockEnd - 1;
            _blockEnd += endTag.Length;
        }
        #endregion
    }
}
