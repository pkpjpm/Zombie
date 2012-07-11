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

namespace Zombie
{
    /// <summary>
    /// The StatusConsole provides and easy way to view trace and error output in a
    /// console application. It should not be used in other types of applications.
    /// </summary>
    public class StatusConsole : IStatusListener
    {
        #region IStatusListener Members

        private void WriteToConsole(string prefix, string Summary, string Details)
        {
            Console.WriteLine("{0}:{1}", prefix, Summary);

            if (!string.IsNullOrEmpty(Details))
            {
                Console.WriteLine("Details: {0}", Details);
            }
        }

        public void StatusMessage(string Summary, string Details)
        {
            WriteToConsole("Trace:", Summary, Details);
        }

        public void WarningMessage(string Summary, string Details)
        {
            WriteToConsole("Warning:", Summary, Details);
        }

        public void ErrorMessage(string Summary, string Details)
        {
            WriteToConsole("Error:", Summary, Details);
        }

        #endregion
    }
}
