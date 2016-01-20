using UnityEngine;
using System.Collections;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using ICSharpCode.SharpZipLib.Checksums;

/// <summary>
/// Zip压缩与解压缩 
/// </summary>
public class ZipHelper
{
    public delegate void OnFinished();
    /// <summary>
    /// 压缩单个文件
    /// </summary>
    /// <param name="fileToZip">要压缩的文件</param>
    /// <param name="zipedFile">压缩后的文件</param>
    /// <param name="compressionLevel">压缩等级</param>
    /// <param name="blockSize">每次写入大小</param>
    /// <param name="onFinished">压缩完成后的委托</param>
    public static void ZipFile(string fileToZip, string zipedFile, int compressionLevel, int blockSize, string password, OnFinished onFinished)
    {
        //如果文件没有找到，则报错
        if (File.Exists(fileToZip))
            throw new FileNotFoundException("指定要压缩的文件: " + fileToZip + " 不存在!");
        using (FileStream zipFile = File.Create(zipedFile))
        {
            using (ZipOutputStream zipStream = new ZipOutputStream(zipFile))
            {
                using (FileStream streamToZip = new FileStream(fileToZip, FileMode.Open, FileAccess.Read))
                {
                    string fileName = fileToZip.Substring(fileToZip.LastIndexOf("/") + 1);
                    ZipEntry zipEntry = new ZipEntry(fileName);
                    zipStream.PutNextEntry(zipEntry);
                    zipStream.SetLevel(compressionLevel);
                    byte[] buffer = new byte[blockSize];
                    int sizeRead = 0;
                    try
                    {
                        do
                        {
                            sizeRead = streamToZip.Read(buffer, 0, buffer.Length);
                            zipStream.Write(buffer, 0, sizeRead);
                        }
                        while (sizeRead > 0);
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }
                    streamToZip.Close();
                }
                zipStream.Finish();
                zipStream.Close();
            }
            zipFile.Close();
            if (null != onFinished) onFinished();
        }
    }


    /// <summary>
    /// 压缩单个文件
    /// </summary>
    /// <param name="fileToZip">要进行压缩的文件名</param>
    /// <param name="zipedFile">压缩后生成的压缩文件名</param>
    /// <param name="level">压缩等级</param>
    /// <param name="password">密码</param>
    /// <param name="onFinished">压缩完成后的代理</param>
    public static void ZipFile(string fileToZip, string zipedFile, string password = "", int level = 5, OnFinished onFinished = null)
    {
        //如果文件没有找到，则报错
        if (!File.Exists(fileToZip))
            throw new FileNotFoundException("指定要压缩的文件: " + fileToZip + " 不存在!");
        using (FileStream fs = File.OpenRead(fileToZip))
        {
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            using (FileStream ZipFile = File.Create(zipedFile))
            {
                using (ZipOutputStream ZipStream = new ZipOutputStream(ZipFile))
                {
                    string fileName = fileToZip.Substring(fileToZip.LastIndexOf("/") + 1);
                    ZipEntry ZipEntry = new ZipEntry(fileName);
                    ZipStream.PutNextEntry(ZipEntry);
                    ZipStream.SetLevel(level);
                    ZipStream.Password = password;
                    ZipStream.Write(buffer, 0, buffer.Length);
                    ZipStream.Finish();
                    ZipStream.Close();
                    if (null != onFinished) onFinished();
                }
            }
        }
    }

    /// <summary>
    /// 压缩多层目录
    /// </summary>
    /// <param name="directory">文件夹目录</param>
    /// <param name="saveFolder">压缩后的文件</param>
    public static void ZipFileDirectory(string directory, string zipedFile, IList filters = null, OnFinished onFinished = null)
    {
        using (FileStream ZipFile = File.Create(zipedFile))
        {
            using (ZipOutputStream zipStream = new ZipOutputStream(ZipFile))
            {
                ZipSetp(directory, zipStream, "", filters);
            }
        }
    }
    /// <summary>
    /// 压缩文件夹
    /// </summary>
    /// <param name="compressFolder"></param>
    /// <param name="saveFolder"></param>
    /// <param name="password"></param>
    /// <param name="onFinished"></param>
    public static void CompressFolderToZip(string compressFolder, string saveFolder, IList filters = null)
    {
        string fileName = compressFolder.Split('/')[compressFolder.Split('/').Length - 1];
        saveFolder = saveFolder == "" ? compressFolder.Substring(0, compressFolder.IndexOf(fileName)) : saveFolder;
        fileName = saveFolder + fileName + ".zip";
        ZipFileDirectory(compressFolder, fileName, filters);
    }

    public static void CompressFolder(string folder, string saveFolder, string saveName, IList filters = null)
    {
        ZipFileDirectory(folder, saveFolder + saveName, filters);
    }

    /// <summary>
    /// 递归遍历目录
    /// </summary>
    /// <param name="directory">文件夹目录</param>
    /// <param name="zipStream">The ZipOutputStream Object.</param>
    /// <param name="parentPath">The parent path.</param>
    private static void ZipSetp(string directory, ZipOutputStream zipStream, string parentPath, IList filters = null)
    {
        if (directory[directory.Length - 1] != Path.DirectorySeparatorChar)
            directory += Path.DirectorySeparatorChar;
        Crc32 crc = new Crc32();

        string[] filenames = Directory.GetFileSystemEntries(directory);

        foreach (string file in filenames)
        {
            if (Directory.Exists(file))
            {
                string pPath = parentPath;
                pPath += file.Substring(file.LastIndexOf("/") + 1);
                pPath += "/";
                ZipSetp(file, zipStream, pPath, filters);
            }
            else
            {
                string s = file.Substring(file.LastIndexOf('.'));
                if (filters != null && filters.Contains(s))
                    continue;
                using (FileStream fs = File.OpenRead(file))
                {
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    string fileName = parentPath + file.Substring(file.LastIndexOf("/") + 1);
                    ZipEntry entry = new ZipEntry(fileName);
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    zipStream.PutNextEntry(entry);
                    zipStream.Write(buffer, 0, buffer.Length);
                }
            }
        }
    }

    /// <summary>
    /// 解压缩一个 zip 文件。
    /// </summary>
    /// <param name="zipedFile">The ziped file.</param>
    /// <param name="directory">The STR directory.</param>
    /// <param name="password">zip 文件的密码。</param>
    /// <param name="overWrite">是否覆盖已存在的文件。</param>
    /// <param name="onFinished">解压完成后的代理</param>
    public static void UnZip(Stream stream, string directory, bool overWrite = true)
    {
        if (directory == "")
            directory = Directory.GetCurrentDirectory();
        if (!directory.EndsWith("/"))
            directory = directory + "/";
        using (ZipInputStream zipStream = new ZipInputStream(stream))
        {
            ZipEntry theEntry;

            while ((theEntry = zipStream.GetNextEntry()) != null)
            {
                string directoryName = "";
                string pathToZip = "";
                pathToZip = theEntry.Name;
                if (pathToZip != "")
                    directoryName = Path.GetDirectoryName(pathToZip) + "/";
                string fileName = Path.GetFileName(pathToZip);
                Directory.CreateDirectory(directory + directoryName);
                if (fileName != "")
                {
                    if ((File.Exists(directory + directoryName + fileName) && overWrite) || (!File.Exists(directory + directoryName + fileName)))
                    {
                        using (FileStream streamWriter = File.Create(directory + directoryName + fileName))
                        {
                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = zipStream.Read(data, 0, data.Length);

                                if (size > 0)
                                    streamWriter.Write(data, 0, size);
                                else
                                    break;
                            }
                            streamWriter.Close();
                        }
                    }
                }
            }
            zipStream.Close();
        }
    }

    public static void UnZip(string file, string directory, bool overWrite = true)
    {
        UnZip(File.OpenRead(file), directory, overWrite);
    }
    public static void UnZip(byte[] bytes, string directory, string password = "", bool overWrite = true, OnFinished onFinished = null)
    {
        UnZip(new MemoryStream(bytes), directory, overWrite);
    }


}
