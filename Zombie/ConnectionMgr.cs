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
using SDK = Interop.QBFC13;

namespace Zombie
{
  /// <summary>
  /// The ConnectionMgr class is used to configure the application identity and
  /// as a source of SDK connections, which require the application identity
  /// </summary>
  public static class ConnectionMgr
  {
    private static List<SDKConnection> _connections = new List<SDKConnection>();
    private static ApplicationIdentity _thisApplication;

    #region configuration options and structures

    public class ApplicationIdentity
    {
      public string Name { get; set; }
      public string IntuitAppID { get; set; }
      public QBEdition TargetEdition { get; set; }
    }

    public enum FCConnectionType
    {
      Desktop,
      DesktopLaunchUI,
      Online
    }

    public enum QBEdition
    {
      US,
      Canada,
      UK,
      Australia
    }

    public class ConnectionConfig
    {
      public bool RequireSingleUser { get; set; }
      public FCConnectionType ConnectionType { get; set; }
      public bool UseCurrentFile { get; set; }
      public string FilePath { get; set; }

      public ConnectionConfig()
      {
        //default connection configuration values
        RequireSingleUser = false;
        ConnectionType = FCConnectionType.Desktop;
        UseCurrentFile = true;
      }
    }

    /// <summary>
    /// The connection configuration that is used if no configuration is specified
    /// </summary>
    public static ConnectionConfig DefaultConnectionConfig { get; set; }

    public static void SetApplicationIdentity(ApplicationIdentity id)
    {
      if (_thisApplication != null)
      {
        throw new Exception("Application identity cannot be set twice");
      }

      _thisApplication = id;
    }

    #endregion configuration options and structures

    /// <summary>
    /// A simple method to initialize an application that will connect to the
    /// current QuickBooks desktop edition file.
    /// </summary>
    /// <param name="applicationName">The name that identifies the application to QuickBooks</param>
    public static void InitDesktop(string applicationName)
    {
      SetApplicationIdentity(new ApplicationIdentity
      {
        Name = applicationName,
        TargetEdition = QBEdition.US
      });

      DefaultConnectionConfig = new ConnectionConfig
      {
        ConnectionType = FCConnectionType.Desktop,
        UseCurrentFile = true,
        RequireSingleUser = false
      };
    }

    /// <summary>
    /// Creates an SDK connection using the current default connection configuration
    /// </summary>
    /// <returns>A usable connection or null if an error occurred</returns>
    public static SDKConnection GetConnection()
    {
      return GetConnection(DefaultConnectionConfig);
    }

    /// <summary>
    /// Creates and SDK connection using the specified connection configuration
    /// </summary>
    /// <param name="config">The connection configuration to use</param>
    /// <returns>a usable connection or null if an error occurred</returns>
    public static SDKConnection GetConnection(ConnectionConfig config)
    {
      if (_thisApplication == null)
      {
        throw new Exception("Application ID is not set");
      }

      try
      {
        var cn = new SDKConnection(_thisApplication, config);

        if (config.ConnectionType != FCConnectionType.Online)
        {
          cn.QueryDesktopVersion();
        }

        lock (_connections)
        {
          _connections.Add(cn);
        }

        return cn;
      }
      catch (SDKException sdkex)
      {
        var entry = new StatusEntry()
        {
          TypeOfEntry = StatusEntry.EntryType.Error,
          Summary = sdkex.Message,
          Details = sdkex.ProblemDetail
        };

        StatusMgr.LogEntry(entry);

        return null;
      }
    }

    internal static void ConnectionClosed(SDKConnection cn)
    {
      lock (_connections)
      {
        if (false == _connections.Remove(cn))
        {
          StatusMgr.LogError("Connection closed: connection not found");
        }
      }
    }

    /// <summary>
    /// Closes all connections and ends the session.  If you wrap all of your connections
    /// with the using statement, you won't need this. But if you're getting creative with
    /// connection management, this method should clean things up if it is called
    /// during application shutdown.
    /// </summary>
    public static void ShutDown()
    {
      lock (_connections)
      {
        while (_connections.Count != 0)
        {
          _connections[0].Close();
        }
      }
    }

    public static SDKConnection GetTestConnection(SDK.IQBSessionManager sessionMgr)
    {
      return new SDKConnection(sessionMgr);
    }
  }
}