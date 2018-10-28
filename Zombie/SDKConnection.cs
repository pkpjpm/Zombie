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
using System.Globalization;
using SDK = Interop.QBFC13;

namespace Zombie
{
  /// <summary>
  /// Represents a connection to the SDK. Supports both desktop and
  /// online connections. In order to create a connection, use the
  /// ConnectionMgr class
  /// </summary>
  public class SDKConnection : IDisposable
  {
    private const short MAX_DESKTOP_QBXML_VERSION = 12;

    //This is the QBXML version that will be used to find the QBXML version.
    private const short HOST_QUERY_QBXML_VERSION = 1;

    private const short ONLINE_QBXML_VERSION = 6;

    private short _SDKMajorVersion;
    private short _SDKMinorVersion;

    /// <summary>
    /// Gets the QBXML version that is being used for messages sets on this connection
    /// </summary>
    public decimal SDKVersion
    {
      get
      {
        return _SDKMajorVersion + ((decimal)_SDKMinorVersion / 10);
      }
    }

    private string _country;

    private SDK.IQBSessionManager _sessionMgr;

    internal SDKConnection(SDK.IQBSessionManager sessionMgrMock)
    {
      _sessionMgr = sessionMgrMock;
    }

    internal SDKConnection(ConnectionMgr.ApplicationIdentity appID,
        ConnectionMgr.ConnectionConfig config)
    {
      _SDKMajorVersion = HOST_QUERY_QBXML_VERSION;
      _SDKMinorVersion = 0;

      SDK.ENConnectionType cnType;

      switch (config.ConnectionType)
      {
        case ConnectionMgr.FCConnectionType.Desktop:
          cnType = SDK.ENConnectionType.ctLocalQBD;
          break;

        case ConnectionMgr.FCConnectionType.DesktopLaunchUI:
          cnType = SDK.ENConnectionType.ctLocalQBDLaunchUI;
          break;

        case ConnectionMgr.FCConnectionType.Online:
          cnType = SDK.ENConnectionType.ctRemoteQBOE;
          _SDKMajorVersion = ONLINE_QBXML_VERSION;
          break;

        default:
          throw new Exception("Unsupported connection type:" + config.ConnectionType.ToString());
      }

      try
      {
        //ToDo: use online session manager for online version
        var sessionMgr = new SDK.QBSessionManager();

        sessionMgr.OpenConnection2(appID.IntuitAppID, appID.Name, cnType);

        _sessionMgr = sessionMgr;
      }
      catch (System.Runtime.InteropServices.COMException cex)
      {
        throw new SDKSessionException(cex);
      }

      _country = string.Empty;

      switch (appID.TargetEdition)
      {
        case ConnectionMgr.QBEdition.Australia:
        case ConnectionMgr.QBEdition.US:
          _country = "US";
          break;

        case ConnectionMgr.QBEdition.Canada:
          _country = "CA";
          break;

        case ConnectionMgr.QBEdition.UK:
          _country = "UK";
          break;
      }

      SDK.ENOpenMode cnMode;

      if (config.RequireSingleUser)
      {
        cnMode = SDK.ENOpenMode.omSingleUser;
      }
      else
      {
        cnMode = SDK.ENOpenMode.omDontCare;
      }

      string filePath = config.FilePath;

      if (config.UseCurrentFile) filePath = string.Empty;

      try
      {
        _sessionMgr.BeginSession(filePath, cnMode);
      }
      catch (System.Runtime.InteropServices.COMException cex)
      {
        throw new SDKSessionException(cex);
      }
    }

    private void HostQuerySuccess(SDK.IQBBase baseObj)
    {
      var respHost = baseObj as SDK.IHostRet;

      if (respHost == null)
      {
        string typeName = "null type";

        if (baseObj != null)
        {
          typeName = baseObj.Type.GetAsString();
        }
        throw new Exception("1.0 host query failure: unexpected detail type:" + typeName);
      }

      var lstVersion = respHost.SupportedQBXMLVersionList;
      double candidateVersion = 0.0;
      string versionList = string.Empty;

      for (int idx = 0; idx < lstVersion.Count; idx++)
      {
        string svers = lstVersion.GetAt(idx);

        if (versionList != string.Empty)
        {
          versionList += ", ";
        }

        versionList += svers;

        double dver = 0.0;

        if (!double.TryParse(svers, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out dver))
        {
          StatusMgr.LogStatus("Unexpected SDK version:" + svers);

          continue;
        }

        if (dver > candidateVersion && dver <= MAX_DESKTOP_QBXML_VERSION)
        {
          candidateVersion = dver;
        }
      }

      if (candidateVersion == 0.0)
      {
        StatusMgr.FormatError("No compatible SDK version found, using {0}", MAX_DESKTOP_QBXML_VERSION);
        candidateVersion = MAX_DESKTOP_QBXML_VERSION;
      }

      _SDKMajorVersion = (short)candidateVersion;

      string minor = (candidateVersion - _SDKMajorVersion).ToString();

      if (minor.Length > 1)
      {
        minor = minor.Substring(2, minor.Length - 1);
        _SDKMinorVersion = short.Parse(minor);
      }
      else
      {
        _SDKMinorVersion = 0;
      }

      var successEntry = new StatusEntry()
      {
        Summary = "QuickBooks Connection Established",
        TypeOfEntry = StatusEntry.EntryType.Status
      };

      successEntry.AddLine(string.Format("{0} version {1}.{2}", Safe.Value(respHost.ProductName),
              Safe.Value(respHost.MajorVersion), Safe.Value(respHost.MinorVersion)));

      successEntry.AddLine(string.Format("Supported qbXML versions:{0}", versionList));
      successEntry.AddLine(string.Format("Using version {0}.{1}", _SDKMajorVersion, _SDKMinorVersion));

      StatusMgr.LogEntry(successEntry);
    }

    private void HostQueryFail(string statusMsg, int statusCode)
    {
      var entry = new StatusEntry()
      {
        TypeOfEntry = StatusEntry.EntryType.Error,
        Summary = "Host Query Failed",
        Details = statusMsg
      };

      entry.AddLine("Status Code:" + statusCode);

      StatusMgr.LogEntry(entry);
    }

    internal void QueryDesktopVersion()
    {
      var batch = NewBatch();

      var qry = batch.MsgSet.AppendHostQueryRq();

      batch.SetClosures(qry,
                      new SDKBatch.QuerySuccess(HostQuerySuccess),
                      new SDKBatch.QueryFailure(HostQueryFail),
                      false);

      batch.Run();
    }

    /// <summary>
    /// Closes this connection
    /// </summary>
    public void Close()
    {
      var sessionMgr = _sessionMgr;
      _sessionMgr = null;

      ConnectionMgr.ConnectionClosed(this);

      try
      {
        sessionMgr.EndSession();
        sessionMgr.CloseConnection();
      }
      catch (System.Runtime.InteropServices.COMException cex)
      {
        throw new SDKCloseException(cex);
      }
    }

    public SDKBatch NewBatch()
    {
      if (_sessionMgr == null)
      {
        throw new Exception("Query attempt after QuickBooks connection is closed");
      }

      return new SDKBatch(_sessionMgr, _SDKMajorVersion, _SDKMinorVersion, _country);
    }

    #region IDisposable Members

    /// <summary>
    /// Dispose closes the SDK session if it is still open
    /// </summary>
    public void Dispose()
    {
      if (_sessionMgr != null)
      {
        Close();
      }
    }

    #endregion IDisposable Members
  }
}