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

namespace Zombie
{
    /// <summary>
    /// Every application should implement IStatusListener in at least 1 class
    /// in order to receive trace and error messages. If you don't provide an
    /// implementation, messages will be lost.
    /// </summary>
    public interface IStatusListener
    {
        void StatusMessage(string Summary, string Details);
        void WarningMessage(string Summary, string Details);
        void ErrorMessage(string Summary, string Details);
    }

    /// <summary>
    /// The status entry class represents one status event with multiple
    /// lines of information.
    /// </summary>
    public class StatusEntry
    {
        public enum EntryType
        {
            Trace,
            Status,
            Warning,
            Error
        }

        public StatusEntry()
        {
            Details = string.Empty;
        }

        public EntryType TypeOfEntry { get; set; }
        public string Summary { get; set; }
        public string Details { get; set; }

        public void AddLine(string line)
        {
            if(Details != string.Empty)
            {
                Details += Environment.NewLine;
            }

            Details += line;
        }
    }       

    /// <summary>
    /// The StatusMgr class coordinates all tracing and error messages
    /// </summary>
    public static class StatusMgr
    {
        private struct ListenerNode
        {
            public IStatusListener listener;
            public bool wantsTrace;
        }

        private static List<ListenerNode> sm_Listeners;

        /// <summary>
        /// If true, an error has occurred
        /// </summary>
        public static bool ErrorState { get; private set; }

        public static void AddListener(IStatusListener listener, bool wantsTrace)
        {
            if (sm_Listeners == null)
            {
                sm_Listeners = new List<ListenerNode>();
            }

            var node = new ListenerNode()
            {
                listener = listener,
                wantsTrace = wantsTrace
            };

            sm_Listeners.Add(node);
        }

        public static void RemoveListener(IStatusListener listenerToRemove)
        {
            if (sm_Listeners == null) return;

            ListenerNode nodeToRemove = new ListenerNode() { listener = null, wantsTrace = false };

            foreach (var lNode in sm_Listeners)
            {
                if (lNode.listener == listenerToRemove)
                {
                    nodeToRemove = lNode;
                    break;
                }
            }

            if (nodeToRemove.listener != null)
            {
                sm_Listeners.Remove(nodeToRemove);
            }
        }

        public static void HandleException(string context, Exception ex)
        {
            var entry = new StatusEntry()
            {
                TypeOfEntry = StatusEntry.EntryType.Error,
                Summary = context
            };

            for (var currentException = ex;
                    currentException != null;
                    currentException = currentException.InnerException)
            {
                entry.AddLine(currentException.Message);
                entry.AddLine(currentException.StackTrace);
            }

            NotifyListeners(entry);
        }

        public static void LogEntry(StatusEntry entry)
        {
            NotifyListeners(entry);
        }

        public static void LogError(string error)
        {
            var entry = new StatusEntry()
            {
                TypeOfEntry = StatusEntry.EntryType.Error,
                Summary = error
            };

            NotifyListeners(entry);
        }

        public static void FormatError(string msg, object obj1)
        {
            LogError(FormatSingleParameter(msg, obj1));
        }

        private static string FormatSingleParameter(string msg, object obj1)
        {
            try
            {
                return string.Format(msg, obj1);
            }
            catch(FormatException)
            {
                return string.Format("Single parameter {0} for message {1}", obj1, msg);
            }
        }

        public static void FormatError(string msg, object obj1, object obj2)
        {
            try
            {
                LogError(string.Format(msg, obj1, obj2));
            }
            catch (FormatException)
            {
                FormatError("Format exception for double parameters: {0} and {1}", obj1, obj2);

                LogError(msg);
            }
        }

        public static void FormatError(string msg, object[] objs)
        {
            try
            {
                LogError(string.Format(msg, objs));
            }
            catch (FormatException)
            {
                LogError("Format exception for array parameter");

                foreach (object obj in objs)
                {
                    FormatError("Parameter:{0}", obj);
                }

                LogError(msg);
            }
        }


        public static void LogQBError(string context, string statusMessage, int statusCode)
        {
            var entry = new StatusEntry()
            {
                TypeOfEntry = StatusEntry.EntryType.Error,
                Summary = "QuickBooks Error:" + context,
                Details = statusMessage
            };

            entry.AddLine("Status Code:" + statusCode);

            NotifyListeners(entry);
        }

        public static void LogWarning(string warning)
        {
            var entry = new StatusEntry()
            {
                TypeOfEntry = StatusEntry.EntryType.Warning,
                Summary = warning
            };

            NotifyListeners(entry);
        }

        public static void FormatWarning(string msg, object obj1)
        {
            LogWarning(FormatSingleParameter(msg, obj1));
        }

        public static void FormatWarning(string msg, object obj1, object obj2)
        {
            LogWarning(string.Format(msg, obj1, obj2));
        }

        public static void LogStatus(string msg)
        {
            var entry = new StatusEntry()
            {
                TypeOfEntry = StatusEntry.EntryType.Status,
                Summary = msg
            };

            NotifyListeners(entry);
        }

        public static void FormatStatus(string msg, object obj1)
        {
            LogStatus(string.Format(msg, obj1));
        }

        public static void FormatStatus(string msg, object obj1, object obj2)
        {
            LogStatus(string.Format(msg, obj1, obj2));
        }

        public static void FormatStatus(string msg, object obj1, object obj2, object obj3)
        {
            LogStatus(string.Format(msg, obj1,obj2, obj3));
        }

        public static void FormatStatus(string msg, object[] args)
        {
            LogStatus(string.Format(msg, args));
        }

        public static void Trace(string message)
        {
            var entry = new StatusEntry()
            {
                TypeOfEntry = StatusEntry.EntryType.Trace,
                Summary = message
            };

            NotifyListeners(entry);
        }        

        private static void NotifyListeners(StatusEntry entry)
        {
            if (sm_Listeners == null) return;

            lock (sm_Listeners)
            {
                foreach (var node in sm_Listeners)
                {
                    if (entry.TypeOfEntry == StatusEntry.EntryType.Trace && node.wantsTrace == false)
                    {
                        continue;
                    }

                    var listener = node.listener;

                    switch (entry.TypeOfEntry)
                    {
                        case StatusEntry.EntryType.Error:
                            listener.ErrorMessage(entry.Summary, entry.Details);
                            break;

                        case StatusEntry.EntryType.Warning:
                            listener.WarningMessage(entry.Summary, entry.Details);
                            break;

                        default:
                            listener.StatusMessage(entry.Summary, entry.Details);
                            break;
                    }
                }
            }
        }
    }
}
