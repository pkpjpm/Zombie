/*
 *               ZOMBIE utility library for QBFC
 * 
 *             created by Paul Keister (pk@pjpm.biz)
 *                copyright (c) 2003 - 2012 PJPM
 *  
 *  Licensed under the Eclipse Public License 1.0 (EPL-1.0)
 *  full license available at http://opensource.org/licenses/EPL-1.0
 */

using System;
using System.IO;
using System.Text;

namespace Zombie
{
    /// <summary>
    /// The status log file provides an easy way to capture error and trace information
    /// in a log file.
    /// </summary>
    public class StatusLogFile : IStatusListener
    {
        private string m_LogFilePath;

        public string LogFilePath
        {
            get
            {
                return m_LogFilePath;
            }
        }

        public StatusLogFile(string logFilePath)
        {
            m_LogFilePath = logFilePath;
        }

        private void WriteTextToFile(string Contents)
        {
            using (var fs = new FileStream(m_LogFilePath, FileMode.Append))
            {
                using (var w = new StreamWriter(fs, Encoding.UTF8))
                {
                    w.WriteLine(Contents);

                    w.Flush();
                }
            }   
        }

        string FormatLine(string Heading, string Summary, string Details)
        {
            string Message = DateTime.Now.ToString("MM/dd/yyyy hh:mm") + "\t" + Heading + "\t" + Summary;

            if (Details != string.Empty)
            {
                Details = Details.Replace(Environment.NewLine, Environment.NewLine + "\t");

                Message += Environment.NewLine + "\tDetails -----------------------" 
                    + Environment.NewLine + "\t" + Details;
            }

            return Message;
        }

        #region IStatusListener Members

        void IStatusListener.StatusMessage(string Summary, string Details)
        {
            WriteTextToFile(FormatLine("STATUS", Summary, Details));
        }

        void IStatusListener.ErrorMessage(string Summary, string Details)
        {
            WriteTextToFile(FormatLine("ERROR", Summary, Details));
        }

        void IStatusListener.WarningMessage(string Summary, string Details)
        {
            WriteTextToFile(FormatLine("WARNING", Summary, Details));
        }

        #endregion
    }
}
