import java.io.*;
import java.nio.file.*;
import java.util.regex.*;

public class Extractor {
    static String getSteamInstallationDirectory() {
        String system = System.getProperty("os.name");

        if (system.contains("Win")) {
            String defaultDirectory = Paths.get(System.getenv("ProgramFiles(x86)"), "Steam").toString();
            if (Files.exists(Paths.get(defaultDirectory))) {
                return defaultDirectory;
            }

            File[] drives = File.listRoots();
            for (File drive : drives) {
                String possibleDirectory = Paths.get(drive.getPath(), "Program Files (x86)", "Steam").toString();
                if (Files.exists(Paths.get(possibleDirectory))) {
                    return possibleDirectory;
                }
            }
        } else if (system.contains("Mac")) {
            String steamDirectory = Paths.get(System.getProperty("user.home"), "Library", "Application Support", "Steam").toString();
            if (Files.exists(Paths.get(steamDirectory))) {
                return steamDirectory;
            } else {
                return null;
            }
        } else if (system.contains("nix") || system.contains("nux") || system.contains("mac")) {
            String steamDirectory = Paths.get(System.getProperty("user.home"), ".steam", "steam").toString();
            if (Files.exists(Paths.get(steamDirectory))) {
                return steamDirectory;
            } else {
                return null;
            }
        }
        return null;
    }

    static String getQuaTitle(String quaFilePath) throws IOException {
        try (BufferedReader quaFile = new BufferedReader(new FileReader(quaFilePath))) {
            String[] lines = quaFile.lines().toArray(String[]::new);
            String songName = lines[6].trim().substring(7);
            return songName;
        }
    }

    static String sanitizeFilename(String filename) {
        String invalidChars = "[<>:\"/\\\\|?*]";
        String safeChar = "";
        return filename.replaceAll(invalidChars, safeChar);
    }

    public static void main(String[] args) throws IOException {
        System.out.println("#-----------------------------------------------------------#");
        System.out.println("|               quaver-scraper [version 1.3]                |");
        System.out.println("#-----------------------------------------------------------#");

        String input;
        String allowedExtension = "(\\.jpg|\\.png|\\.jpeg)$";
        BufferedReader reader = new BufferedReader(new InputStreamReader(System.in));
        while (true) {
            System.out.print("What type of files do you want to scrape: (Images, Music): ");
            input = reader.readLine();
            if (input.equalsIgnoreCase("Images")) {
                break;
            } else if (input.equalsIgnoreCase("Music")) {
                allowedExtension = "(\\.mp3)$";
                break;
            } else {
                System.out.println("Invalid file type, please choose one of the valid file types listed above.");
            }
        }

        System.out.print("Enter output directory, example: (C:Extractor): ");
        String destinationDirectory = reader.readLine();
        String rootDirectory = Paths.get(getSteamInstallationDirectory(), "steamapps", "common", "Quaver", "Songs").toString();
        String quaTitleName = "";

        if (!Files.exists(Paths.get(destinationDirectory))) {
            Files.createDirectories(Paths.get(destinationDirectory));
        }

        try (DirectoryStream<Path> folderPaths = Files.newDirectoryStream(Paths.get(rootDirectory))) {
            for (Path folderPath : folderPaths) {
                if (Files.isDirectory(folderPath)) {
                    try (DirectoryStream<Path> filePaths = Files.newDirectoryStream(folderPath)) {
                        for (Path filePath : filePaths) {
                            String fileName = filePath.getFileName().toString();
                            String extension = filePath.getFileName().toString().toLowerCase();
                            if (filePath.toString().toLowerCase().endsWith(".qua")) {
                                quaTitleName = sanitizeFilename(getQuaTitle(filePath.toString()));
                            }

                            if (Pattern.matches(allowedExtension, extension)) {
                                Files.copy(filePath, Paths.get(destinationDirectory, fileName), StandardCopyOption.REPLACE_EXISTING);
                                Files.move(Paths.get(destinationDirectory, fileName), Paths.get(destinationDirectory, quaTitleName + extension), StandardCopyOption.REPLACE_EXISTING);
                            }
                        }
                    }
                }
            }
        }
    }
}
 