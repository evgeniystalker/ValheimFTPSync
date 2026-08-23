using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Models
{
    internal readonly struct OperationProgress
    {
        public string CurrentFile { get; }
        public Operation Operation { get; } // "Upload" или "Download"
        public int CompletedFiles { get; }
        public int TotalFiles { get; }
        public TransferProgress TransferProgress { get; }
        public float OverallProgress => TotalFiles == 0 ? 0 : (CompletedFiles * 100 + TransferProgress.Percent) / TotalFiles;

        public OperationProgress(string currentFilename, TransferProgress progressFile, int completedFiles, int totalFile, Operation operation)
        {
            CurrentFile = currentFilename;
            CompletedFiles = completedFiles;
            TotalFiles = totalFile;
            Operation = operation;
            TransferProgress = progressFile;
        }
    }
    internal enum Operation
    {
        Upload,
        Download,
        Append,
    }
}
