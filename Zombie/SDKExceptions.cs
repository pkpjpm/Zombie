/*
 *               ZOMBIE utility library for QBFC
 * 
 *             created by Paul Keister (pk@pjpm.biz)
 *                copyright (c) 2003 - 2012 PJPM
 *  
 *  Licensed under the Eclipse Public License 1.0 (EPL-1.0)
 *  full license available at http://opensource.org/licenses/EPL-1.0
 *  
 *  Library-specific exceptions and special error handling
 */

using System;
using System.Runtime.InteropServices;

namespace Zombie
{
    public class SDKException : Exception
    {
        public string ProblemDetail { get; protected set; }

        public SDKException(COMException cex)
            :base("QuickBooks Connection Error", cex)
        {}
    }

    internal class SDKSessionException : SDKException
    {
        public SDKSessionException(COMException cex)
            : base(cex)
        {
            switch ((uint)cex.ErrorCode)
            { 
                    //Possible enhancements: Handle connection failure for online edition
                    //also we need to support the case where a company file has been specified
                    //perhaps a special callback for customizing this message?
                case 0x80040408: //old school: could not start QuickBooks
                case 0x80040416: //specify co file if not running
                case 0x80040417: //specify co file if no file open
                    ProblemDetail = "Can't connect to QuickBooks.  Please make sure:"
                        + Environment.NewLine + "1. QuickBooks is running"
                        + Environment.NewLine + "2. If QuickBooks is running, check for an application authorization dialog"
                        + Environment.NewLine + "3. If using Vista or Windows 7, make sure UAC is enabled";
                    break;

                default:
                    //it's only necessary to override the default message if we have something to say
                    ProblemDetail = String.Format("{0}, Code: {1:X}", cex.Message, cex.ErrorCode);
                    break;
            }
        }
    }

    internal class SDKConnectionException : SDKException
    {
        public SDKConnectionException(COMException cex)
            :base(cex)
        {
            switch ((uint)cex.ErrorCode)
            {
                default:
                    ProblemDetail = String.Format("{0}, Code: {1:X}", cex.Message, cex.ErrorCode);
                    break;
            }
        }
    }

    public class SDKCloseException : SDKException
    {
        public SDKCloseException(COMException cex)
            :base(cex)
        {
            ProblemDetail = String.Format("Error on closing QuickBooks:{0}, Code: {1:X}", cex.Message, cex.ErrorCode);
        }
    }
}
