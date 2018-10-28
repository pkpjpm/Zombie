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
  /// Everything that happens in the Zombie library happens through the SDKBatch class.
  /// This class provides access to a QBFC message set and a way to specify follow-up
  /// actions for SDK queries.
  /// </summary>
  public class SDKBatch
  {
    private SDK.IMsgSetRequest _msg;
    private SDK.IQBSessionManager _mgr;
    private Dictionary<int, ClosureSpec> _clousures;
    private IteratorSetup _iterator;

    private class ClosureSpec
    {
      public QuerySuccess success { get; set; }
      public QueryFailure failure { get; set; }
      public bool allowEmptyReturn { get; set; }
    }

    public delegate void QuerySuccess(SDK.IQBBase qbObj);

    public delegate void QueryFailure(string statusMsg, int statusCode);

    public delegate bool IteratorSetup(string iteratorId, int remaining);

    private class IteratorCall
    {
      public IteratorSetup iterator { get; set; }
      public string iteratorId { get; set; }
      public int remaining { get; set; }
    }

    public SDK.IMsgSetRequest MsgSet { get { return _msg; } }

    internal SDKBatch(SDK.IQBSessionManager mgr, short majorVer, short minorVer, string Country)
    {
      _clousures = new Dictionary<int, ClosureSpec>();
      _mgr = mgr;
      _msg = _mgr.CreateMsgSetRequest(Country, majorVer, minorVer);
      _iterator = null;

      if (_msg == null) throw new Exception("Can't create message set");
    }

    public void SetClosures(SDK.IQBBase qry, QuerySuccess success, QueryFailure failure = null, bool allowEmpty = false)
    {
      var rqs = _msg.RequestList;

      for (int idx = 0; idx < rqs.Count; idx++)
      {
        var rq = rqs.GetAt(idx);
        if (rq.Detail == qry)
        {
          _clousures.Add(rq.RequestID, new ClosureSpec()
          {
            success = success,
            failure = failure,
            allowEmptyReturn = allowEmpty
          });
          return;
        }
      }

      throw new Exception("Query not found in this batch");
    }

    public void SetIteratorSetup(IteratorSetup setup)
    {
      _iterator = setup;
    }

    public bool RunIfNotEmpty()
    {
      if (_msg.RequestList.Count == 0)
      {
        return true;
      }
      else
      {
        return Run();
      }
    }

    public bool Run()
    {
      switch (_msg.RequestList.Count)
      {
        case 0:
          StatusMgr.LogError("No items in batch to process");
          return false;

        case 1:
          break;

        default:
          if (_iterator != null)
          {
            StatusMgr.LogError("Iterators are only allowed in single request batches");
            return false;
          }
          _msg.Attributes.OnError = SDK.ENRqOnError.roeContinue;
          break;
      }

      try
      {
        bool retVal = false;

        IteratorCall iterCall = null;

        do
        {
          retVal = RunOneBatch(out iterCall);

          if (iterCall != null)
          {
            if (!iterCall.iterator(iterCall.iteratorId, iterCall.remaining))
            {
              return false; //user cancelled
            }
          }
        }
        while (retVal == true && iterCall != null);

        return retVal;
      }
      catch (System.Runtime.InteropServices.COMException cex)
      {
        var entry = new StatusEntry()
        {
          Summary = "Request Error",
          TypeOfEntry = StatusEntry.EntryType.Error
        };
        entry.AddLine(cex.Message);

        try
        {
          //This may not be the source of the error
          //entry.AddLine(_msg.ToXMLString());

          //this causes further COM errors
          //entry.AddLine(_msg.Attributes.ResponseData.ToString());
          entry.AddLine("Enable SDK logging for further diagnostics");
        }
        catch (Exception)
        {
        }

        StatusMgr.LogEntry(entry);

        return false;
      }
    }

    private bool RunOneBatch(out IteratorCall _iterCall)
    {
      const string ERROR_CONTEXT = "QuickBooks query follow-up";

      _iterCall = null;

      var resp = _mgr.DoRequests(_msg);

      if (resp == null || resp.ResponseList == null)
      {
        throw new Exception("null response returned from batch");
      }

      if (resp.ResponseList.Count == 0)
      {
        throw new Exception("no response items returned from batch");
      }

      bool errors = false;

      var lstResponse = resp.ResponseList;

      for (int idx = 0; idx < lstResponse.Count; idx++)
      {
        var itm = lstResponse.GetAt(idx);

        if (itm == null)
        {
          var entry = new StatusEntry()
          {
            TypeOfEntry = StatusEntry.EntryType.Error,
            Summary = "null reponse item encountered",
            Details = resp.ToXMLString()
          };

          StatusMgr.LogEntry(entry);

          return false;
        }

        ClosureSpec cls = null;
        int rqID = int.Parse(itm.RequestID);

        if (_clousures.ContainsKey(rqID))
        {
          cls = _clousures[rqID];
        }

        if (itm.StatusCode == 0 || itm.StatusCode == 500)
        {
          if (cls != null)
          {
            try
            {
              cls.success(itm.Detail);
            }
            catch (Exception ex)
            {
              StatusMgr.HandleException(ERROR_CONTEXT, ex);
              return false;
            }
          }

          if (_iterator != null)
          {
            int iteratorRemaining = 0;

            try
            {
              //iterator remaining count can be toxic, it
              //iterator is not set up correctly
              iteratorRemaining = itm.iteratorRemainingCount;
            }
            catch (System.Runtime.InteropServices.COMException)
            {
              StatusMgr.LogError("Iterator not properly initialized");
            }

            if (iteratorRemaining > 0)
            {
              _iterCall = new IteratorCall()
              {
                iterator = _iterator,
                iteratorId = itm.iteratorID,
                remaining = iteratorRemaining
              };
            }
          }
        }
        else if (cls != null && cls.allowEmptyReturn
                    && itm.StatusCode == 1)
        {
          //process empty result
          try
          {
            cls.success(null);
          }
          catch (Exception ex)
          {
            StatusMgr.HandleException(ERROR_CONTEXT, ex);
            return false;
          }
        }
        else
        {
          switch (itm.StatusSeverity.ToLower())
          {
            case "info":
              StatusMgr.Trace(itm.StatusMessage);
              if (cls != null)
              {
                try
                {
                  cls.success(itm.Detail);
                }
                catch (Exception ex)
                {
                  StatusMgr.HandleException(ERROR_CONTEXT, ex);
                  return false;
                }
              }
              break;

            case "warning":
            case "warn":
              StatusMessage(idx, "Warning", itm.StatusMessage, itm.StatusCode, false);
              if (cls != null)
              {
                try
                {
                  cls.success(itm.Detail);
                }
                catch (Exception ex)
                {
                  StatusMgr.HandleException(ERROR_CONTEXT, ex);
                  return false;
                }
              }
              break;

            case "error":
            default:
              if (cls != null && cls.failure != null)
              {
                try
                {
                  cls.failure(itm.StatusMessage, itm.StatusCode);
                }
                catch (Exception ex)
                {
                  StatusMgr.HandleException(ERROR_CONTEXT, ex);
                  return false;
                }
              }
              else
              {
                StatusMessage(idx, "Error", itm.StatusMessage, itm.StatusCode, true);
              }
              errors = true;
              break;
          }
        }
        //end of response loop
      }
      return errors == false;
    }

    private void StatusMessage(int idx, string Heading, string StatusMessage, int statusCode, bool IsError)
    {
      StatusEntry.EntryType t = StatusEntry.EntryType.Status;
      if (IsError) t = StatusEntry.EntryType.Error;

      var entry = new StatusEntry()
      {
        TypeOfEntry = t,
        Summary = Heading + "detected for index " + idx,
        Details = StatusMessage
      };

      entry.AddLine("Status Code:" + statusCode);

      StatusMgr.LogEntry(entry);
    }
  }
}