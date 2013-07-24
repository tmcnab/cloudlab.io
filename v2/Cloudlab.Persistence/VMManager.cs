namespace Cloudlab.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Text;

    public static class VMManager
    {
        #region Members

        private static ConcurrentDictionary<string, DateTime> LastActivity;

        #endregion

        public static void Initialize()
        {
            LastActivity = new ConcurrentDictionary<string, DateTime>();
        }

        #region VM Activity

        /// <summary>
        /// Updates the last activity of the user in the VM
        /// </summary>
        private static void ActivityUpdate(string username)
        {
            LastActivity.TryUpdate(username, DateTime.UtcNow, DateTime.UtcNow);
        }

        #endregion
    }
}
