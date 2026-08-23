namespace ValheimFTPSync.Models
{
    internal readonly struct TransferProgress
    {
        public TransferProgress(long bytesTransferred, long totalBytes)
        {
            TransferredBytes = bytesTransferred;
            TotalBytes = totalBytes;
        }

        public int Percent => TotalBytes > 0 ? (int)(TransferredBytes * 100 / TotalBytes ) : 0;
        public long TransferredBytes { get; }
        public long TotalBytes { get; }
    }
}