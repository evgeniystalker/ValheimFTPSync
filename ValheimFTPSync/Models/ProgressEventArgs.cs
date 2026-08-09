using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Models
{
    internal class ProgressEventArgs
    {
        public int Percent { get; }
        public long BytesTransferred { get; }
        public long TotalBytes { get; }
        public Operation Operation { get; } // "Upload" или "Download"
        public double Speed { get; set; }

        public ProgressEventArgs(int percent, long bytesTransferred, long totalBytes, Operation operation)
        {
            Percent = percent;
            BytesTransferred = bytesTransferred;
            TotalBytes = totalBytes;
            Operation = operation;
        }
    }
    internal enum Operation
    {
        Upload,
        Download,
    }
}
