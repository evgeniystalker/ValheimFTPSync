using System;
using System.Collections.Generic;
using System.Text;

namespace ValheimFTPSync.Models
{
    internal class OperationProgress : TransferProgress
    {
        public string CurrentFile { get; }
        public Operation Operation { get; } // "Upload" или "Download"
        public int CompletedFiles { get; }
        public int TotalFiles { get; }
        public float OverallProgress => TotalFiles == 0 ? 0 : (CompletedFiles + base.Percent * 0.01f) / TotalFiles;
    
        public OperationProgress(string currentFilename, TransferProgress progressFile, int totalFile, int completedFiles, Operation operation) : base(progressFile.TransferredBytes, progressFile.TotalBytes)
        {
            CurrentFile = currentFilename;
            CompletedFiles = completedFiles;
            TotalFiles = totalFile;
            Operation = operation;
        }
    }
    internal enum Operation
    {
        Upload,
        Download,
    }
}
