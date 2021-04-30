using System;
using System.IO;

namespace Shared.Utilities
{
    public static class FileHelper
    {
        public static string GetFilePath(string relativePath)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
        }

        public static FileInfo GetFile(string relativePath)
        {
            return new FileInfo(GetFilePath(relativePath));
        }

        public static void CreateFolderIfNotExistsByRelative(string relativePath) => CreateFolderIfNotExists(GetFilePath(relativePath));

        public static void CreateFolderIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
