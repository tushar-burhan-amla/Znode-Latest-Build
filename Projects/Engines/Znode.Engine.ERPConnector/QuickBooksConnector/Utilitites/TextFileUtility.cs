using System;
using System.IO;

namespace Znode.Engine.ERPConnector
{
    public class TextFileUtility : IDisposable
    {
        #region Private Variables

        private bool isDisposed;

        private readonly string _lastSORefNumberFileName = "LastImportedSalesOrderId.txt";

        #endregion Private Variables

        #region Constructor

        ~TextFileUtility()
        {
            if (!isDisposed)
                Dispose();
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Creates text file with name "LastImportedSalesOrderId.txt" and write data passed to it
        /// </summary>
        /// <param name="lastWriteData">Reference id that needs to be saved in text file </param>
        public void CreateTextFile(string lastWriteData)
        {
            StreamWriter streamWriter = File.CreateText(_lastSORefNumberFileName);
            streamWriter.Write(lastWriteData);
            streamWriter.Close();
        }

        /// <summary>
        /// Creates text file with name "LastImportedSalesOrderId.txt" and write data passed to it
        /// </summary>
        /// <param name="lastWriteData">Reference id that needs to be saved in text file </param>
        public void WriteRefNumberInTextFile(string lastWriteData)
        {
            StreamWriter streamWriter = File.CreateText(_lastSORefNumberFileName);
            streamWriter.Write(lastWriteData);
            streamWriter.Close();
        }

        /// <summary>
        /// Read reference number of sales order saved in text file.
        /// </summary>
        /// <returns>Reference number of lastly inserted sales order in QuickBooks</returns>
        public string GetLastRefNumberFromTextFile()
        {
            string refNumber = string.Empty;
            if (File.Exists(_lastSORefNumberFileName))
            {
                StreamReader streamReader = File.OpenText(_lastSORefNumberFileName);
                refNumber = streamReader.ReadToEnd();
                streamReader.Close();
            }
            else
                refNumber = "0";
            return refNumber;
        }

        /// <summary>
        /// Implementation of IDisposable interface
        /// </summary>
        public void Dispose() => isDisposed = true;

        #endregion Public Methods
    }
}