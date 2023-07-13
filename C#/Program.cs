using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class Program
{
    static string GetSteamInstallationDirectory()
    {
        string system = Environment.OSVersion.Platform.ToString();

        if (system == "Win32NT")
        {
            // Check if the Steam installation directory exists in the default location
            string defaultDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam");
            if (Directory.Exists(defaultDirectory))
            {
                return defaultDirectory;
            }

            // Search for the Steam installation directory on all available drives
            var drives = DriveInfo.GetDrives()
                .Where(d => d.DriveType == DriveType.Fixed && d.IsReady)
                .Select(d => d.Name);
            foreach (var drive in drives)
            {
                string possibleDirectory = Path.Combine(drive, "Program Files (x86)", "Steam");
                if (Directory.Exists(possibleDirectory))
                {
                    return possibleDirectory;
                }
            }
        }
        else if (system == "Darwin")
        {
            string steamDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", "Steam");
            if (Directory.Exists(steamDirectory))
            {
                return steamDirectory;
            }
        }
        else if (system == "Unix")
        {
            string steamDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".steam", "steam");
            if (Directory.Exists(steamDirectory))
            {
                return steamDirectory;
            }
        }

        return null;
    }

    static string SanitizeFilename(string filename)
    {
        string invalidChars = @"[<>:""/\\|?*]";
        string safeChar = " ";
        return Regex.Replace(filename, invalidChars, safeChar);
    }

    static string SanitizeDirectoryPath(string directoryPath)
    {
        char[] invalidChars = Path.GetInvalidPathChars();
        return string.Concat(directoryPath.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
    }

    static void Main()
    {
        Console.Clear();
        Console.WriteLine("#-----------------------------------------------------------#");
        Console.WriteLine("|               quaver-scraper [version 1.3]                |");
        Console.WriteLine("#-----------------------------------------------------------#");

        string scrapeType;
        while (true)
        {
            Console.Write("What type of files do you want to scrape: (Images, Music): ");
            scrapeType = Console.ReadLine();
            if (scrapeType == "Images" || scrapeType == "Music")
            {
                if (scrapeType == "Images")
                {
                    scrapeType = ".png,.jpg,.jpeg";
                }
                if (scrapeType == "Music")
                {
                    scrapeType = ".mp3";
                }
                break;
            }
            else
            {
                Console.WriteLine("Invalid file type, please choose the valid file types listed above.");
            }
        }

        Console.Write("Enter output directory, example: (C:\\Extractor): ");
        string destinationDirectory = Console.ReadLine();
        string rootDirectory = Path.Combine(GetSteamInstallationDirectory(), "steamapps", "common", "Quaver", "Songs");

        if (!Directory.Exists(destinationDirectory))
        {
            Directory.CreateDirectory(destinationDirectory);
        }

        // Loop through each folder in the root directory
        foreach (string folderName in Directory.GetDirectories(rootDirectory))
        {
            string folderPath = Path.Combine(rootDirectory, folderName);

            // Check if the current item is a directory
            if (Directory.Exists(folderPath))
            {
                string imageFilePath = null;
                string quaFilePath = null;

                // Loop through files in the current folder
                foreach (string fileName in Directory.GetFiles(folderPath))
                {
                    string filePath = Path.Combine(folderPath, fileName);

                    // Check if the current file is an image file
                    if (scrapeType.Split(',').Any(extension => fileName.ToLower().EndsWith(extension)))
                    {
                        // Sanitize the file name
                        string sanitizedFileName = SanitizeFilename(fileName);

                        // Copy the image file to the destination directory with the sanitized file name
                        imageFilePath = Path.Combine(destinationDirectory, sanitizedFileName);
                        File.Copy(filePath, imageFilePath);
                    }

                    // Check if the current file is a .qua file
                    if (fileName.ToLower().EndsWith(".qua"))
                    {
                        quaFilePath = filePath;
                    }
                }

                // Process the .qua file if an image file and .qua file are found in the same directory
                if (!string.IsNullOrEmpty(imageFilePath) && !string.IsNullOrEmpty(quaFilePath))
                {
                    string[] lines = File.ReadAllLines(quaFilePath);
                    if (lines.Length >= 7)
                    {
                        string titleLine = lines[6].Trim();
                        if (titleLine.StartsWith("Title: "))
                        {
                            string songName = titleLine.Substring(7);
                            // Sanitize the song name
                            string sanitizedSongName = SanitizeFilename(songName);

                            // Rename the copied image file with the sanitized song name
                            string newImageFileName = $"{sanitizedSongName}{Path.GetExtension(imageFilePath)}";
                            string newImageFilePath = Path.Combine(destinationDirectory, newImageFileName);
                            if (File.Exists(newImageFilePath))
                            {
                                // Generate a new file name by appending a random number
                                string newFileNameWithRandom = $"{sanitizedSongName}_{Guid.NewGuid()}{Path.GetExtension(imageFilePath)}";
                                newImageFilePath = Path.Combine(destinationDirectory, newFileNameWithRandom);
                            }
                            // Ensure the destination directory exists
                            Directory.CreateDirectory(destinationDirectory);
                            File.Move(imageFilePath, newImageFilePath);
                        }
                    }
                }
            }
        }
    }
}
