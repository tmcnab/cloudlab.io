namespace Cloudlab.Logic
{
    using System;

    [Serializable]
    public struct VMResponse
    {
        public string Text { get; set; }

        public bool IsError { get; set; }

        public bool IsJS { get; set; }
    }
}