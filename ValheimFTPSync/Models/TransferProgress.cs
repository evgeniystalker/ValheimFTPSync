using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Models
{
    internal class TransferProgress
    {
        public int Percent { get; }
        public long BytesTransferred { get; }
        public long TotalBytes { get; }
        public Operation Operation { get; } // "Upload" или "Download"
        public double Speed { get; }
        int CompletedFiles { get; }
        int TotalFiles { get; }
        public float FileProgress => TotalBytes == 0 ? 0 : (float)BytesTransferred / TotalBytes;

        public float OverallProgress => TotalFiles == 0 ? 0 : (CompletedFiles + FileProgress) / TotalFiles;


        public TransferProgress(int percent, long bytesTransferred, long totalBytes, Operation operation)
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
