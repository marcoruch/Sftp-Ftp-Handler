using System;
using System.Collections.Generic;
using System.IO;

namespace Shared.Utilities
{
    public interface IFtpHandler : IDisposable
    {
        void DeleteFile(string relativeFilePath);
        bool DoesFileExist(string relativeFilePath);
        void DownloadFile(string ftpRelativeSourceFilePath, string localDestinationFilePath);
        List<string> GetRelativeFilePathsInDirectory(string folderPath);
        void UploadFile(string ftpFilePath, byte[] data);
    }
}
