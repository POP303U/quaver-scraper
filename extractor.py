import os
import shutil
import platform
import string

def get_steam_installation_directory():
    system = platform.system()
    
    if system == "Windows":
        # Check if the Steam installation directory exists in the default location
        default_directory = os.path.expanduser("~\\Program Files (x86)\\Steam")
        if os.path.exists(default_directory):
            return default_directory
    
        # Search for the Steam installation directory on all available drives
        drives = ["{}:".format(d) for d in string.ascii_uppercase if os.path.exists("{}:\\".format(d))]
        for drive in drives:
            possible_directory = os.path.join(drive, "Program Files (x86)\\Steam")
            if os.path.exists(possible_directory):
                return possible_directory

    elif system == "Darwin":
        steam_directory = os.path.expanduser("~/Library/Application Support/Steam")
        if os.path.exists(steam_directory):
            return steam_directory
        else:
            return None
    
    elif system == "Linux":
        steam_directory = os.path.expanduser("~/.steam/steam")
        if os.path.exists(steam_directory):
            return steam_directory
        else:
            return None
    
    return None

def extract_images(song_directory, output_directory, scrape_type):
    print("Scraping...")
    if not os.path.exists(output_directory):
        os.makedirs(output_directory);

    # Rectify different image formats to a tuple (why do so many exist bruh)
    if scrape_type == ".png":
        scrape_type = ('.png', '.jpeg', '.jpg');

    # Iterate through the files in the song directory
    for root, _, files in os.walk(song_directory):
        for file in files:
            if file.lower().endswith((scrape_type)):
                # Create the full path for the image file
                image_path = os.path.join(root, file);
                
                # Copy the image file to the output directory
                shutil.copy(image_path, output_directory);

    print("Scraping complete!");

def take_input():
    print("\x1B[2J\x1B[1;1H");
    print("#-----------------------------------------------------------#");
    print("|               quaver-scraper [version 1.2]                |");
    print("#-----------------------------------------------------------#");
    scrape_type = input("What type of files do you want to scrape: (.png, .mp3, .qua): ");
    while True:
        if scrape_type == ".png" or scrape_type == ".mp3" or scrape_type == ".qua":
            break;
        else:
            print("Invalid file type, please choose the valid file types listed above.");
    output_dir = input("Enter output directory, example: (C:\Extractor): ");

    steam_directory = get_steam_installation_directory() + "\steamapps\common\Quaver\Songs"
    extract_images(steam_directory, output_dir, scrape_type);
    if input("Exit? (y/n): ") != "y":
        take_input();

take_input();
