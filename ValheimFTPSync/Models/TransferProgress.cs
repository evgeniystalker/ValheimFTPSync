namespace ValheimFTPSync.Models
{
    internal class TransferProgress
    {
        public TransferProgress(long bytesTransferred, long totalBytes)
        {
            TransferredBytes = bytesTransferred;
            TotalBytes = totalBytes;
        }

        public int Percent => TotalBytes > 0 ? (int)(TransferredBytes / TotalBytes * 100)  : 0;
        public long TransferredBytes { get; }
        public long TotalBytes { get; }
    }
}