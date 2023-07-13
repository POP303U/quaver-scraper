using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        Console.Clear();
        Console.WriteLine("#-----------------------------------------------------------#");
        Console.WriteLine("|               quaver-scraper [version 1.3]                |");
        Console.WriteLine("#-----------------------------------------------------------#");

        string scrapeType;
        string[] scrapeExtensions;
        while (true)
        {
            Console.Write("What type of files do you want to scrape: (Images, Music): ");
            scrapeType = Console.ReadLine();

            if (scrapeType == "Images" || scrapeType == "Music")
            {
                if (scrapeType == "Images")
                {
                    scrapeExtensions = new string[] { ".png", ".jpg", ".jpeg" };
                }
                else if (scrapeType == "Music")
                {
                    scrapeExtensions = new string[] { ".mp3" };
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

        string rootDirectory = Path.Combine("C:", "Program Files (x86)", "Steam", "steamapps", "common", "Quaver", "Songs");

        if (!Directory.Exists(destinationDirectory))
        {
            Directory.CreateDirectory(destinationDirectory);
        }
        
        foreach (string folderName in Directory.GetDirectories(rootDirectory)) {
            string folderPath = Path.Combine(rootDirectory, folderName);

            foreach (string fileName in Directory.GetFiles(folderPath)) {
                string filePath = Path.Combine(folderPath, fileName);
                Console.WriteLine(filePath);
            }
        }
    }
}
/*
        // Loop through each folder in the root directory
        foreach (string folderName in Directory.GetDirectories(rootDirectory))
        {
            string folderPath = Path.Combine(rootDirectory, folderName);

            // Loop through files in the current folder
            foreach (string fileName in Directory.GetFiles(folderPath))
            {
                string filePath = Path.Combine(folderPath, fileName);

                // Check if the current file is an image file
                if (Array.Exists(scrapeExtensions, ext => fileName.ToLower().EndsWith(ext)))
                {
                    // Sanitize the file name
                    string sanitizedFileName = SanitizeFilename(Path.GetFileName(fileName));

                    // Copy the image file to the destination directory with the sanitized file name
                    string imageFilePath = Path.Combine(destinationDirectory, sanitizedFileName);
                    File.Copy(filePath, imageFilePath);
                }

                // Check if the current file is a .qua file
                if (fileName.ToLower().EndsWith(".qua"))
                {
                    string quaFilePath = filePath;

                    // Process the .qua file if an image file and .qua file are found in the same directory
                    if (File.Exists(imageFilePath) && File.Exists(quaFilePath))
                    {
                        using (StreamReader quaFile = new StreamReader(quaFilePath))
                        {
                            string[] lines = quaFile.ReadToEnd().Split('\n');
                            if (lines.Length >= 7)
                            {
                                string titleLine = lines[6].Trim();
                                if (titleLine.StartsWith("Title: "))
                                {
                                    string songName = titleLine.Substring(7);
                                    // Sanitize the song name
                                    string sanitizedSongName = SanitizeFilename(songName);

                                    // Rename the copied image file with the sanitized song name
                                    string newImageFileName = sanitizedSongName + Path.GetExtension(imageFilePath);
                                    string newImageFilePath = Path.Combine(destinationDirectory, newImageFileName);
                                    File.Move(imageFilePath, newImageFilePath);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    static string GetSteamInstallationDirectory()
    {
        // Implement the logic to get the Steam installation directory
        // Example:
        return "C:\\Steam";
    }

    static string SanitizeFilename(string fileName)
    {
        // Implement the logic to sanitize the file name
        // Example:
        return fileName.Replace(" ", "_");
    }
}
*/