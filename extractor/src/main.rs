use std::env;
use std::fs;
use std::io::{BufRead, BufReader};
use std::path::{Path, PathBuf};

fn get_steam_installation_directory() -> Option<String> {
    let system = env::consts::OS;

    match system {
        "windows" => {
            let default_directory = Path::new(
                &env::var("ProgramFiles(x86)").unwrap_or("C:/Program Files (x86)".to_string()),
            )
            .join("Steam")
            .to_string_lossy()
            .to_string();

            if Path::new(&default_directory).exists() {
                Some(default_directory)
            } else {
                for drive in fs::read_dir("/").unwrap() {
                    if let Ok(drive) = drive {
                        let drive_name = drive.file_name();
                        let possible_directory = Path::new(&drive_name)
                            .join("Program Files (x86)")
                            .join("Steam")
                            .to_string_lossy()
                            .to_string();

                        if Path::new(&possible_directory).exists() {
                            return Some(possible_directory);
                        }
                    }
                }
                None
            }
        }
        "macos" => {
            let steam_directory = Path::new(&env::var("HOME").unwrap_or("".to_string()))
                .join("Library")
                .join("Application Support")
                .join("Steam")
                .to_string_lossy()
                .to_string();

            if Path::new(&steam_directory).exists() {
                Some(steam_directory)
            } else {
                None
            }
        }
        "linux" => {
            let steam_directory = Path::new(&env::var("HOME").unwrap_or("".to_string()))
                .join(".steam")
                .join("steam")
                .to_string_lossy()
                .to_string();

            if Path::new(&steam_directory).exists() {
                Some(steam_directory)
            } else {
                None
            }
        }
        _ => None,
    }
}

fn get_qua_title(qua_file_path: &str) -> String {
    let file = fs::File::open(qua_file_path).expect("Failed to open the file");
    let reader = BufReader::new(file);
    let lines: Vec<String> = reader.lines().map(|line| line.unwrap()).collect();
    let song_name = lines[6].trim()[7..].to_string();
    song_name
}

fn sanitize_filename(filename: &str) -> String {
    let invalid_chars = r#"[<>:"/\\|?*]"#;
    let safe_char = "";
    regex::Regex::new(invalid_chars)
        .unwrap()
        .replace_all(filename, safe_char)
        .to_string()
}

fn main() {
    print!("\u{001b}[H\u{001b}[2J");
    println!("#-----------------------------------------------------------#");
    println!("|               quaver-scraper [version 1.3]                |");
    println!("#-----------------------------------------------------------#");

    let mut allowed_extension = r#"(\.jpg|\.png|\.jpeg)$"#.to_string();

    loop {
        print!("What type of files do you want to scrape: (Images, Music): ");
        let mut input = String::new();
        std::io::stdin()
            .read_line(&mut input)
            .expect("Failed to read input");
        let input = input.trim().to_lowercase();

        match input.as_str() {
            "images" => break,
            "music" => {
                allowed_extension = r#"(\.mp3)$"#.to_string();
                break;
            }
            _ => {
                println!("Invalid file type, please choose the valid file types listed above.");
                continue;
            }
        }
    }

    print!("Enter output directory, example: (C:Extractor): ");
    let mut destination_directory = String::new();
    std::io::stdin()
        .read_line(&mut destination_directory)
        .expect("Failed to read input");
    let destination_directory = destination_directory.trim().to_string();

    if !Path::new(&destination_directory).exists() {
        fs::create_dir_all(&destination_directory).expect("Failed to create the output directory");
    }

    if let Some(root_directory) = get_steam_installation_directory() {
        let root_directory = Path::new(&root_directory)
            .join("steamapps")
            .join("common")
            .join("Quaver")
            .join("Songs")
            .to_string_lossy()
            .to_string();
        let mut qua_title_name = String::new();

        for entry in fs::read_dir(&root_directory).expect("Failed to read directory") {
            if let Ok(entry) = entry {
                if entry.file_type().unwrap().is_file() {
                    let file_name = entry.file_name();
                    let file_path = entry.path().to_string_lossy().to_string();
                    let extension = Path::new(&file_path)
                        .extension()
                        .unwrap_or_default()
                        .to_string_lossy()
                        .to_string()
                        .to_lowercase();

                    if extension.ends_with(".qua") {
                        qua_title_name = sanitize_filename(&get_qua_title(&file_path));
                    }

                    if regex::Regex::new(&allowed_extension)
                        .unwrap()
                        .is_match(&extension)
                    {
                        let destination_path = Path::new(&destination_directory)
                            .join(format!("{}{}", qua_title_name, extension))
                            .to_string_lossy()
                            .to_string();

                        fs::copy(&file_path, &destination_path).expect("Failed to copy file");
                    }
                }
            }
        }
    }
}
