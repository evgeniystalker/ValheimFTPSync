using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Models
{
    internal class ProgressEventArgs
    {
        public double ProgressPercent { get; }
        public long BytesTransferred { get; }
        public long TotalBytes { get; }
        public string Status { get; }
        public ProgressEventArgs(double progressPercent, long bytesTransferred, long totalBytes, string status)
        {
            ProgressPercent = progressPercent;
            BytesTransferred = bytesTransferred;
            TotalBytes = totalBytes;
            Status = status;
        }
    }
}
