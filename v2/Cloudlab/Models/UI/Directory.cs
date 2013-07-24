namespace Cloudlab.Models.UI
{
    using System;
    using System.Collections.Generic;

    public class Directory
    {
        public Directory()
        {
            this.Entries = new List<FileEntry>();
        }

        public string Path { get; set; }

        public List<FileEntry> Entries { get; set; }

        public class FileEntry
        {
            public string Name { get; set; }

            public bool IsFile { get; set; }

            public long Size { get; set; }

            public DateTime Modified { get; set; }
        }
    }
}