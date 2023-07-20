import java.nio.file.*
import java.io.*

val steamInstallationDirectory: String?
    get() = when {
        System.getProperty("os.name").contains("win", ignoreCase = true) -> {
            val defaultDirectory = Paths.get(System.getenv("ProgramFiles(x86)"), "Steam").toString()
            if (Files.exists(Paths.get(defaultDirectory))) defaultDirectory
            else {
                val driveLetters = ('A'..'Z').map { "$it:" }.toTypedArray()
                driveLetters.map {
                    Paths.get(it, "Program Files (x86)", "Steam").toString()
                }.firstOrNull { Files.exists(Paths.get(it)) }
            }
        }
        System.getProperty("os.name").contains("mac", ignoreCase = true) -> Paths.get(System.getProperty("user.home"), "Library", "Application Support", "Steam").toString()
        else -> Paths.get(System.getProperty("user.home"), ".steam", "steam").toString()
    }

fun sanitizeFilename(filename: String?): String = filename!!.replace("[<>:\"/\\\\|?*]".toRegex(), "")
fun getQuaTitle(quaFilePath: String): String = File(quaFilePath).useLines { it.drop(6).firstOrNull()?.substring(7)?.trim() ?: "" }

fun main() {
    print("\u001b[H\u001b[2J")
    println("#-----------------------------------------------------------#")
    println("|               quaver-scraper [version 1.3]                |")
    println("#-----------------------------------------------------------#")
    print("What type of files do you want to scrape: (Images, Music): ")

    val allowedExtension = when (readlnOrNull()?.trim()) {
        "Images" -> "\\.(jpg|png|jpeg)$"
        "Music" -> "\\.(mp3)$"
        else -> {
            println("Invalid file type, please choose the valid file types listed above.")
            return
        }
    }

    print("Enter output directory, example: (C:Extractor): ")
    val destinationDirectory = BufferedReader(FileReader(FileDescriptor.`in`)).readLine().trim { it <= ' ' }
    val rootDirectory = steamInstallationDirectory?.let { Paths.get(it, "steamapps", "common", "Quaver", "Songs").toString() }
    var quaTitleName = ""

    Files.createDirectories(Paths.get(destinationDirectory))
    Files.walk(rootDirectory?.let { Paths.get(it) })
        .filter { Files.isRegularFile(it) }
        .forEach { path ->
            val fileName = path.fileName.toString()
            val extension = fileName.substring(fileName.lastIndexOf(".")).lowercase()
            when {
                extension == ".qua" -> quaTitleName = sanitizeFilename(getQuaTitle(path.toString()))
                extension.matches(allowedExtension.toRegex()) -> {
                    val destinationPath = Paths.get(destinationDirectory, quaTitleName + extension)
                    Files.copy(path, destinationPath, StandardCopyOption.REPLACE_EXISTING)
                }
            }
        }
}