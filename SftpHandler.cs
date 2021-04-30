
using Renci.SshNet;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Shared.Utilities
{
    public class SftpHandler : IFtpHandler
    {
        #region Fields

        private readonly FtpConfiguration ftpConfiguration;

        private SftpClient sftpClient;

        #endregion

        #region ctor / Dispose

        public SftpHandler(FtpConfiguration ftpConfiguration)
        {
            this.ftpConfiguration = ftpConfiguration;
        }

        public void Dispose()
        {
            if (sftpClient != null)
            {
                sftpClient.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Base Sftp Functionality

        private SftpClient GetSftpClient(FtpConfiguration ftpConfiguration)
        {
            return new SftpClient(GetSftpConnectionInfo(ftpConfiguration));
        }

        private  ConnectionInfo GetSftpConnectionInfo(FtpConfiguration ftpConfiguration)
        {
            return new ConnectionInfo(ftpConfiguration.Host,
                                      ftpConfiguration.Username,
                                      new PasswordAuthenticationMethod(ftpConfiguration.Username, ftpConfiguration.Password));
        }

        private void EnsureConnected()
        {
            if (sftpClient == null)
            {
                sftpClient = GetSftpClient(ftpConfiguration);
            }

            if (!sftpClient.IsConnected)
            {
                sftpClient.Connect();
            }
        }

        #endregion

        #region Delete File(s)

        public void DeleteFile(string relativeFilePath)
        {
            EnsureConnected();
            sftpClient.DeleteFile(relativeFilePath);
        }

        #endregion

        #region Get File Names from Directory

        public List<string> GetRelativeFilePathsInDirectory(string folderPath)
        {
            EnsureConnected();
            return sftpClient.ListDirectory(folderPath).Where(x => !x.IsDirectory).Select(x => x.Name).ToList();
        }

        #endregion

        #region Download File(s)

        public void DownloadFile(string ftpRelativeSourceFilePath, string localDestinationFilePath)
        {
            EnsureConnected();
            using Stream fileStream = File.Create(localDestinationFilePath);
            sftpClient.DownloadFile(ftpRelativeSourceFilePath, fileStream);
        }

        #endregion

        #region Upload File(s)

        public void UploadFile(string ftpFilePath, byte[] data)
        {
            EnsureConnected();
            sftpClient.UploadFile(new MemoryStream(data), ftpFilePath, null);
        }

        #endregion

        #region Does File Exist

        public bool DoesFileExist(string relativeFilePath)
        {
            EnsureConnected();
            return sftpClient.Exists(relativeFilePath);
        }

        #endregion
    }
}
