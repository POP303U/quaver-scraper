import java.io.*;
import java.nio.file.*;
import java.util.regex.*;

class Extractor {
    static String getSteamInstallationDirectory() {
        String system = System.getProperty("os.name");

        if (system.contains("Windows")) {
            String defaultDirectory = Paths.get(System.getenv("ProgramFiles(x86)"), "Steam").toString();
            if (Files.exists(Paths.get(defaultDirectory))) {
                return defaultDirectory;
            }

            for (File drive : File.listRoots()) {
                String possibleDirectory = Paths.get(drive.getAbsolutePath(), "Program Files (x86)", "Steam").toString();
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
        while (true) {
            System.out.printf("What type of files do you want to scrape: (Images, Music): ");
            input = System.console().readLine();
            if (input.equalsIgnoreCase("Images")) {
                break;
            } else if (input.equalsIgnoreCase("Music")) {
                allowedExtension = "(\\.mp3)$";
                break;
            } else {
                System.out.println("Invalid file type, please choose the valid file types listed above.\n");
                continue;
            }
        }
        System.out.printf("Enter output directory, example: (C:Extractor): ");
        String destinationDirectory = System.console().readLine();
        String rootDirectory = Paths.get(getSteamInstallationDirectory(), "steamapps", "common", "Quaver", "Songs").toString();
        String quaTitleName = "";

        if (!Files.exists(Paths.get(destinationDirectory))) {
            Files.createDirectories(Paths.get(destinationDirectory));
        }

        try (DirectoryStream<Path> directoryStream = Files.newDirectoryStream(Paths.get(rootDirectory))) {
            for (Path folderPath : directoryStream) {
                try (DirectoryStream<Path> fileStream = Files.newDirectoryStream(folderPath)) {
                    for (Path filePath : fileStream) {
                        String fileName = filePath.getFileName().toString();
                        String extension = filePath.toString().substring(filePath.toString().lastIndexOf('.')).toLowerCase();
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
