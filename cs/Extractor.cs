using System.Text.RegularExpressions;

class Extractor {
    static string? GetSteamInstallationDirectory() {
        string system = Environment.OSVersion.Platform.ToString();

        if (system == "Win32NT") {
            string defaultDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam");
            if (Directory.Exists(defaultDirectory)) {
                return defaultDirectory;
            }

            foreach (DriveInfo drive in DriveInfo.GetDrives()) {
                string possibleDirectory = Path.Combine(drive.Name, "Program Files (x86)", "Steam");
                if (Directory.Exists(possibleDirectory)) {
                    return possibleDirectory;
                }
            }
        }
        else if (system == "MacOSX") {
            string steamDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", "Steam");
            if (Directory.Exists(steamDirectory)) {
                return steamDirectory;
            } else {
                return null;
            }
        }
        else if (system == "Unix") {
            string steamDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".steam", "steam");
            if (Directory.Exists(steamDirectory)) {
                return steamDirectory;
            } else {
                return null;
            }
        }
        return null;
    }
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
    Console.Clear();
    Console.WriteLine("#-----------------------------------------------------------#");
    Console.WriteLine("|               quaver-scraper [version 1.3]                |");
    Console.WriteLine("#-----------------------------------------------------------#");
    string? input;
    string allowedExtension= @"(\.jpg|\.png|\.jpeg)$";
    while (true) {
        Console.Write("What type of files do you want to scrape: (Images, Music): ");
        input = Console.ReadLine();
        if (input == "Images") {
            break;
        } else if (input == "Music") {
            allowedExtension = @"(\.mp3)$";
            break;
        } else {
            Console.WriteLine("Invalid file type, please choose the valid file types listed above.");
            continue;
        } 
    }
        Console.Write("Enter output directory, example: (C:Extractor): ");
        string? destinationDirectory = Console.ReadLine();
        string rootDirectory = GetSteamInstallationDirectory() + Path.Combine("\\", "steamapps", "common", "Quaver", "Songs");
        string quaTitleName = string.Empty;
        
        if (!Path.Exists(destinationDirectory)) {
            Directory.CreateDirectory(destinationDirectory);
        }

        foreach (string folderPath in Directory.GetDirectories(rootDirectory)) {

            foreach (string filePath in Directory.GetFiles(folderPath)) {
                string fileName = Path.GetFileName(filePath);
                string extension = Path.GetExtension(filePath).ToLower();
                if (filePath.ToLower().EndsWith((".qua"))) {
                    quaTitleName = SanitizeFilename(GetQuaTitle(filePath));
                }

                if (Regex.IsMatch(extension, allowedExtension)) {
                    File.Copy(filePath, Path.Combine(destinationDirectory, fileName), true);
                    File.Move(Path.Combine(destinationDirectory, fileName), Path.Combine(destinationDirectory, quaTitleName + extension), true);
                }
            }
        }
    }
} 