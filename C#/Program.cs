using System;
using System.IO;
using System.Text.RegularExpressions;

class Program {
    static string GetQuaTitle(string quaFilePath) {
        using (StreamReader quaFile = new StreamReader(quaFilePath)) {
            string[] lines = quaFile.ReadToEnd().Split('\n');
            string songName = lines[6].Trim().Substring(7);
            return songName;
        }
    }
    static string SanitizeFilename(string filename) {
        string invalidChars = @"[<>:""/\\|?*]";
        string safeChar = string.Empty;
        return Regex.Replace(filename, invalidChars, safeChar);
    }
    static void Main(string[] args) {
        string destinationDirectory = Path.Combine("C:", "Helloman");
        string rootDirectory = Path.Combine("C:", "Program Files (x86)", "Steam", "steamapps", "common", "Quaver", "Songs");
        string quaTitleName = string.Empty;
        
        if (!Path.Exists(destinationDirectory)) {
            Directory.CreateDirectory(destinationDirectory);
        }
        foreach (string folderPath in Directory.GetDirectories(rootDirectory)) {
            foreach (string filePath in Directory.GetFiles(folderPath)) {
                string fileName = Path.GetFileName(filePath);
                if (filePath.ToLower().EndsWith(".qua")) {
                    quaTitleName = SanitizeFilename(GetQuaTitle(filePath));
                }

                if (filePath.ToLower().EndsWith(".jpg") || filePath.ToLower().EndsWith(".png") || filePath.ToLower().EndsWith(".jpeg")) {
                    File.Copy(filePath, Path.Combine(destinationDirectory, fileName), true);
                    File.Move(Path.Combine(destinationDirectory, fileName), Path.Combine(destinationDirectory, quaTitleName + ".png"), true);
                }
            }
        }
    }
}