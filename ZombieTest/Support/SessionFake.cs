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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interop.QBFC11;

namespace ZombieTest.Support
{
    class SessionFake : IQBSessionManager
    {
        private IMsgSetRequest _msgSet;

        public SessionFake(IMsgSetRequest msgSet)
        {
            _msgSet = msgSet;
        }

        public void BeginSession(string qbFile, ENOpenMode openMode)
        {
            throw new NotImplementedException();
        }

        public void ClearErrorRecovery()
        {
            throw new NotImplementedException();
        }

        public void CloseConnection()
        {
        }

        public void CommunicateOutOfProcess(bool useOutOfProc)
        {
            throw new NotImplementedException();
        }

        public void CommunicateOutOfProcessEx(bool useOutOfProc, string outOfProcCLSID)
        {
            throw new NotImplementedException();
        }

        public ENConnectionType ConnectionType
        {
            get { throw new NotImplementedException(); }
        }

        public IMsgSetRequest CreateMsgSetRequest(string Country, short qbXMLMajorVersion, short qbXMLMinorVersion)
        {
            return _msgSet;
        }

        public ISubscriptionMsgSetRequest CreateSubscriptionMsgSetRequest(short qbXMLMajorVersion, short qbXMLMinorVersion)
        {
            throw new NotImplementedException();
        }

        public IMsgSetResponse DoRequests(IMsgSetRequest request)
        {
            throw new NotImplementedException();
        }

        public IMsgSetResponse DoRequestsFromXMLString(string qbXMLRequest)
        {
            throw new NotImplementedException();
        }

        public ISubscriptionMsgSetResponse DoSubscriptionRequests(ISubscriptionMsgSetRequest request)
        {
            throw new NotImplementedException();
        }

        public ISubscriptionMsgSetResponse DoSubscriptionRequestsFromXMLString(string qbXMLSubscriptionRequest)
        {
            throw new NotImplementedException();
        }

        public bool EnableErrorRecovery
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void EndSession()
        {
        }

        public IQBGUIDType ErrorRecoveryID
        {
            get { throw new NotImplementedException(); }
        }

        public string GetCurrentCompanyFileName()
        {
            throw new NotImplementedException();
        }

        public IMsgSetResponse GetErrorRecoveryStatus()
        {
            throw new NotImplementedException();
        }

        public IMsgSetRequest GetSavedMsgSetRequest()
        {
            throw new NotImplementedException();
        }

        public void GetVersion(out short MajorVersion, out short MinorVersion, out ENReleaseLevel releaseLevel, out short releaseNumber)
        {
            throw new NotImplementedException();
        }

        public bool IsErrorRecoveryInfo()
        {
            throw new NotImplementedException();
        }

        public void OpenConnection(string AppID, string AppName)
        {
            throw new NotImplementedException();
        }

        public void OpenConnection2(string AppID, string AppName, ENConnectionType connType)
        {
            throw new NotImplementedException();
        }

        public IQBAuthPreferences QBAuthPreferences
        {
            get { throw new NotImplementedException(); }
        }

        public string[] QBXMLVersionsForSession
        {
            get { throw new NotImplementedException(); }
        }

        public string[] QBXMLVersionsForSubscription
        {
            get { throw new NotImplementedException(); }
        }

        public bool SaveAllMsgSetRequestInfo
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IEventsMsgSet ToEventsMsgSet(string qbXMLEventsResponse, short qbXMLMajorVersion, short qbXMLMinorVersion)
        {
            throw new NotImplementedException();
        }

        public IMsgSetRequest ToMsgSetRequest(string qbXMLRequest)
        {
            throw new NotImplementedException();
        }

        public IMsgSetResponse ToMsgSetResponse(string qbXMLResponse, string Country, short qbXMLMajorVersion, short qbXMLMinorVersion)
        {
            throw new NotImplementedException();
        }

        public ISubscriptionMsgSetResponse ToSubscriptionMsgSetResponse(string qbXMLSubscriptionResponse, short qbXMLMajorVersion, short qbXMLMinorVersion)
        {
            throw new NotImplementedException();
        }
    }
}
