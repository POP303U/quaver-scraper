import java.nio.file.*
import java.io.*

val steamInstallationDirectory: String?
    get() = when {
        System.getProperty("os.name").contains("win", ignoreCase = true) -> {
            val defaultDirectory = Paths.get(System.getenv("ProgramFiles(x86)"), "Steam").toString()
            if (Files.exists(Paths.get(defaultDirectory))) defaultDirectory
            else ('A'..'Z').firstNotNullOfOrNull {
                val path = Paths.get("$it:", "Program Files (x86)", "Steam")
                if (Files.exists(path)) path.toString() else null
            }
        }
        System.getProperty("os.name").contains("mac", ignoreCase = true) -> Paths.get(System.getProperty("user.home"), "Library", "Application Support", "Steam").toString()
        else -> Paths.get(System.getProperty("user.home"), ".steam", "steam").toString()
    }

fun String.sanitizeFilename() = replace(Regex("[<>:\"/\\\\|?*]"), "")
fun getQuaTitle(quaFilePath: String) = File(quaFilePath).useLines { it.drop(6).firstOrNull()?.substring(7)?.trim() ?: "" }

fun main() {
    print("\u001b[H\u001b[2J")
    println("#-----------------------------------------------------------#")
    println("|               quaver-scraper [version 1.3]                |")
    println("#-----------------------------------------------------------#")
    print("What type of files do you want to scrape: (Images, Music): ")

    val allowedExtension = when (readlnOrNull()?.trim()?.lowercase()) {
        "images" -> "\\.(jpg|png|jpeg)$"
        "music" -> "\\.(mp3)$"
        else -> {
            println("Invalid file type, please choose the valid file types listed above.")
            return
        }
    }

    print("Enter output directory, example: (C:Extractor): ")
    val destinationDirectory = readlnOrNull()?.trim { it <= ' ' } ?: "C:Extractor"
    val rootDirectory = steamInstallationDirectory?.let { Paths.get(it, "steamapps", "common", "Quaver", "Songs").toString() }
    var quaTitleName = ""

    Files.createDirectories(Paths.get(destinationDirectory))
    Files.walk(rootDirectory?.let { Paths.get(it) })
        .filter { Files.isRegularFile(it) }
        .forEach { path ->
            val fileName = path.fileName.toString()
            val extension = fileName.substring(fileName.lastIndexOf(".")).lowercase()
            when {
                extension == ".qua" -> quaTitleName = getQuaTitle(path.toString()).sanitizeFilename()
                extension.matches(Regex(allowedExtension)) -> {
                    val destinationPath = Paths.get(destinationDirectory, quaTitleName + extension)
                    Files.copy(path, destinationPath, StandardCopyOption.REPLACE_EXISTING)
                }
            }
        }
}