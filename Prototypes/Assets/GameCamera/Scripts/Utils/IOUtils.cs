// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RG_GameCamera.Utils
{
    public static class IO
    {
        /// <summary>
        /// copy all content of source directory to target directory
        /// </summary>
        /// <param name="sourceDirectory">name of source directory</param>
        /// <param name="targetDirectory">name of target directory</param>
        public static void CopyDirs(string sourceDirectory, string targetDirectory)
        {

#if !UNITY_WEBPLAYER
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
#endif
        }

        /// <summary>
        /// copy directory helper
        /// </summary>
        private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
#if !UNITY_WEBPLAYER
            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
#endif
        }

        /// <summary>
        /// return name of the file from absolut path
        /// </summary>
        /// <param name="absolutPath">path</param>
        /// <returns>file name</returns>
        public static string GetFileName(string absolutPath)
        {
#if !UNITY_WEBPLAYER || UNITY_EDITOR
            return Path.GetFileName(absolutPath);
#else
            return null;
#endif
        }

        public static string GetFileNameWithoutExtension(string absolutPath)
        {
#if !UNITY_WEBPLAYER || UNITY_EDITOR
            return Path.GetFileNameWithoutExtension(absolutPath);
#else
            return null;
#endif
        }

        /// <summary>
        /// return name of the file extension from absolut path
        /// </summary>
        /// <param name="absolutPath">path</param>
        /// <returns>extension with dot character ".bmp"</returns>
        public static string GetExtension(string absolutPath)
        {
#if !UNITY_WEBPLAYER || UNITY_EDITOR
            return Path.GetExtension(absolutPath).ToLower();
#else
            return null;
#endif
        }

        /// <summary>
        /// return content of text file
        /// </summary>
        /// <param name="absolutPath">path to text file</param>
        /// <returns>file content</returns>
        public static string ReadTextFile(string absolutPath)
        {
#if !UNITY_WEBPLAYER || UNITY_EDITOR

            if (File.Exists(absolutPath))
            {
                var reader = new StreamReader(absolutPath);
                var text = reader.ReadToEnd();
                reader.Close();
                return text;
            }

            return null;
#else
            return null;
#endif
        }

        /// <summary>
        /// create new text file with content string
        /// </summary>
        /// <param name="absolutPath">path to text file</param>
        /// <param name="content">content of the file</param>
        public static void WriteTextFile(string absolutPath, string content)
        {
#if !UNITY_WEBPLAYER || UNITY_EDITOR

            var writeText = new StreamWriter(absolutPath);
            writeText.Write(content);
            writeText.Close();
#endif
        }

        /// <summary>
        /// delete file
        /// </summary>
        /// <param name="absolutPath">absolut path to file</param>
        public static bool DeleteFile(string absolutPath)
        {
#if !UNITY_WEBPLAYER || UNITY_EDITOR

            if (File.Exists(absolutPath))
            {
                File.Delete(absolutPath);
                return true;
            }

            return false;
#else
            return false;
#endif
        }

        /// <summary>
        /// delete directory and all its content
        /// </summary>
        /// <param name="absolutPath">absolut path to directory</param>
        public static void DeleteDir(string absolutPath)
        {
#if !UNITY_WEBPLAYER

            if (DeleteFile(absolutPath))
            {
                return;
            }

            var dir = new DirectoryInfo(absolutPath);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                DeleteDir(di.FullName);

                if (Directory.Exists(di.FullName))
                {
                    di.Delete();
                }
            }

            Directory.Delete(absolutPath);
#endif
        }

        /// <summary>
        /// rename file
        /// </summary>
        /// <param name="absolutPath">absolut path to file</param>
        /// <param name="newName">new file name (not including path, example: "picture.bmp")</param>
        public static void RenameFile(string absolutPath, string newName)
        {
#if !UNITY_WEBPLAYER
            var newPath = Path.GetDirectoryName(absolutPath) + "/" + newName;

//            if (File.Exists(newPath))
//            {
//                File.Delete(newPath);
//            }

            File.Move(absolutPath, newPath);
#endif
        }

        /// <summary>
        /// copy file from source path do destination path
        /// </summary>
        public static bool CopyFile(string src, string dst, bool overwrite)
        {
#if !UNITY_WEBPLAYER || UNITY_EDITOR

            if (File.Exists(src) && (!File.Exists(dst) || overwrite))
            {
                File.Copy(src, dst, overwrite);
                return true;
            }

            return false;
#else
            return false;
#endif
        }

        public static void WriteBinaryFile(string path, byte[] bytes)
        {
#if !UNITY_WEBPLAYER || UNITY_EDITOR

            var file = File.Open(path, FileMode.Create);
            var binary = new BinaryWriter(file);
            binary.Write(bytes);
            file.Close();
#endif
        }

        /// <summary>
        /// convert windows file separator "\\" to "/"
        /// </summary>
        /// <param name="path">path</param>
        /// <returns>converted path</returns>
        public static string ConvertFileSeparators(string path)
        {
            return path.Replace("\\", "/");
        }

        /// <summary>
        /// returns true if file exists
        /// </summary>
        public static bool FileExists(string path)
        {
#if !UNITY_WEBPLAYER || UNITY_EDITOR
            return File.Exists(path);
#else
            return false;
#endif
        }
    }
}
