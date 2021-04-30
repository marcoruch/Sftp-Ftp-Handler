using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Shared.Utilities
{
    public class FtpHandler : IFtpHandler
    {
        #region Fields

        private readonly FtpConfiguration ftpConfiguration;

        #endregion

        #region ctor / Dispose

        public FtpHandler(FtpConfiguration ftpConfiguration)
        {
            this.ftpConfiguration = ftpConfiguration;
        }

        public void Dispose()
        {
            // Not used, used in other IFtpHandler's
        }

        #endregion

        #region Base Ftp Functionality

        private  FtpWebRequest GetWebRequestBase(string folderOrFilePath, string methodType)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(folderOrFilePath);
            request.Credentials = new NetworkCredential(ftpConfiguration.Username, ftpConfiguration.Password);
            request.KeepAlive = false;
            request.UseBinary = true;
            request.UsePassive = true;
            request.Method = methodType;
            return request;
        }

        #endregion

        #region Delete File(s)

        public  void DeleteFile(string relativeFilePath)
        {
            var requestPath = $"{ftpConfiguration.Host}/{relativeFilePath}";
            var request = GetWebRequestBase(requestPath, WebRequestMethods.Ftp.DeleteFile);
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            response.Close();
        }

        #endregion

        #region Get File Names from Directory

        public  List<string> GetRelativeFilePathsInDirectory(string folderPath)
        {
            var request = GetWebRequestBase($"{ftpConfiguration.Host}/{folderPath}",
                                            WebRequestMethods.Ftp.ListDirectory);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            List<string> fileNameList = new List<string>();

            while (!reader.EndOfStream)
            {
                string file = reader.ReadLine();

                //Make sure you only get the filename and not the whole path.
                file = file.Substring(file.LastIndexOf('/') + 1);

                //The root folder will also be added, this can of course be ignored.
                if (!file.StartsWith(".") && file.Contains("."))
                {
                    fileNameList.Add(file);
                }
            }

            reader.Close();
            response.Close();

            return fileNameList;
        }

        #endregion

        #region Download File(s)

        public void DownloadFile(string ftpRelativeSourceFilePath, string localDestinationFilePath)
        {
            var request = GetWebRequestBase($"{ftpConfiguration.Host}/{ftpRelativeSourceFilePath}", WebRequestMethods.Ftp.DownloadFile);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();

            using FileStream fs = File.Create(localDestinationFilePath);
            responseStream.CopyTo(fs);
        }

        #endregion

        #region Upload File(s)

        public void UploadFile(string ftpFilePath, byte[] data)
        {
            var request = GetWebRequestBase($"{ftpConfiguration.Host}/{ftpFilePath}",
                                            WebRequestMethods.Ftp.UploadFile);

            using Stream ftpStream = request.GetRequestStream();
            ftpStream.Write(data, 0, data.Length);
        }

        #endregion

        #region Does File Exist

        public bool DoesFileExist(string relativeFilePath)
        {
            var request = GetWebRequestBase($"{ftpConfiguration.Host}/{relativeFilePath}",
                                            WebRequestMethods.Ftp.GetFileSize);
            try
            {
                request.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                return response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable;
            }
        }

        #endregion
    }
}
