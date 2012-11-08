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
using System.Diagnostics;

namespace Zombie
{
    public class DebugListener : IStatusListener
    {
        #region IStatusListener Members

        public void WriteToDebug(string Category, string Summary, string Details)
        {
            Debug.WriteLine(Summary, Category);

            if (!string.IsNullOrEmpty(Details))
            {
                Debug.WriteLine(Details, Category);
            }
        }

        public void ErrorMessage(string Summary, string Details)
        {
            WriteToDebug("Error", Summary, Details);
        }

        public void StatusMessage(string Summary, string Details)
        {
            WriteToDebug("Warning", Summary, Details);
        }

        public void WarningMessage(string Summary, string Details)
        {
            WriteToDebug("Status", Summary, Details);
        }

        #endregion
    }
}
