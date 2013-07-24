namespace Cloudlab.Logic
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Diagnostics.PerformanceData;

    public class VMRecord : IDisposable
    {
        #region Properties

        public DateTime LastActivity { get; private set; }

        public JSVM VM { get; private set; }

        public ConcurrentQueue<string> MessageBox { get; private set; }

        #endregion

        public VMRecord(string username)
        {
            this.LastActivity = DateTime.Now;
            this.MessageBox = new ConcurrentQueue<string>();
            this.VM = new JSVM(username);
        }

        public void MessageAdd(string content)
        {
            this.LastActivity = DateTime.UtcNow;
            this.MessageBox.Enqueue(content);
        }

        public bool MessageGet(out string content)
        {
            this.LastActivity = DateTime.UtcNow;
            content = null;
            if (!this.MessageBox.IsEmpty)
                return this.MessageBox.TryDequeue(out content);
            return false;
        }

        public VMResponse Execute(string javascript)
        {
            this.LastActivity = DateTime.UtcNow;
            return this.VM.Interpret(javascript);
        }

        public void Dispose()
        {
            try { this.VM.Dispose(); }
            catch { }
        }
    }
}