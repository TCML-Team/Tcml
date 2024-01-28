﻿namespace Tkmm.Core.Helpers.Operations;

public static class DirectoryOperations
{
    public static void CopyDirectory(string src, string dst, bool overwrite = false)
    {
        Directory.CreateDirectory(dst);

        foreach (var file in Directory.EnumerateFiles(src)) {
            File.Copy(file, Path.Combine(dst, Path.GetFileName(file)), overwrite);
        }

        foreach (var folder in Directory.EnumerateDirectories(src)) {
            string folderName = Path.GetFileName(folder);
            string dstFolder = Path.Combine(dst, folderName);
            CopyDirectory(folder, dstFolder, overwrite);
        }
    }

    public static void CopyDirectory(string src, string dst, List<string> excludeFiles, List<string> excludeFolders, bool overwrite = false)
    {
        Directory.CreateDirectory(dst);

        foreach (var file in Directory.EnumerateFiles(src)) {
            if (!excludeFiles.Contains(Path.GetExtension(file))) {
                File.Copy(file, Path.Combine(dst, Path.GetFileName(file)), overwrite);
            }
        }

        foreach (var folder in Directory.EnumerateDirectories(src)) {
            string folderName = Path.GetFileName(folder);
            if (!excludeFolders.Contains(folderName)) {
                string dstFolder = Path.Combine(dst, folderName);
                CopyDirectory(folder, dstFolder, overwrite);
            }
        }
    }
}
