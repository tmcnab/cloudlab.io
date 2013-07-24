namespace Cloudlab.Logic
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Threading;

    public static class VMManager
    {
        static ConcurrentDictionary<string,VMRecord> Session;
        static Timer Reaper;
        
        public static void Initialize()
        {
            Session = new ConcurrentDictionary<string, VMRecord>();
            Reaper = new Timer(new TimerCallback(Reap), null, new TimeSpan(0, 0, 30), new TimeSpan(0,0,30));
        }

        public static void Reap(object stateInfo)
        {
            Debug.WriteLine("VMManager::Reap()");
            foreach (var item in Session)
            {
                if (item.Value.LastActivity.AddMinutes(20) < DateTime.UtcNow)
                {
                    Debug.WriteLine("VMManager::Reap("+ item.Key +")");
                    Evict(item.Key);
                }
            }
        }

        public static void PostMessage(string username, string content)
        {
            if (Session.ContainsKey(username))
            {
                Session[username].MessageAdd(content);
            }
        }

        public static bool FetchMessage(string username, out string content)
        {
            if (Session.ContainsKey(username))
            {
                return Session[username].MessageGet(out content);
            }
            else
            {
                content = null;
                return false;
            }
        }

        public static void AddOrReset(string username)
        {
            if (!Session.ContainsKey(username))
            {
                Session.AddOrUpdate(username, new VMRecord(username), (oldVM, newVM) => newVM);
            }
            else
            {
                VMRecord oldRecord;
                if (Session.TryRemove(username, out oldRecord))
                {
                    Session.AddOrUpdate(username, new VMRecord(username), (oldVM, newVM) => newVM);
                    oldRecord.Dispose();
                    GC.Collect();
                }
            }
        }

        public static void Evict(string username)
        {
            try
            {
                VMRecord oldRecord;
                if (Session.TryRemove(username, out oldRecord))
                {
                    oldRecord.Dispose();
                    GC.Collect();
                }
            }
            catch { }
        }

        public static VMResponse Execute(string username, string javascript)
        {
            if (!Session.ContainsKey(username))
            {
                AddOrReset(username);
            }
            var record = Session[username];
            var result = record.Execute(javascript);
            Session[username] = record;
            return result;
        }
    }
}