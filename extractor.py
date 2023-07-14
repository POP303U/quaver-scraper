import os
import re
import platform
import string
import shutil

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

def sanitize_filename(filename):
    invalid_chars = r'[<>:"/\\|?*]'
    safe_char = ' '
    return re.sub(invalid_chars, safe_char, filename)

print("\x1B[2J\x1B[1;1H")
print("#-----------------------------------------------------------#")
print("|               quaver-scraper [version 1.3]                |")
print("#-----------------------------------------------------------#")
while True:
    scrape_type = input("What type of files do you want to scrape: (Images, Music): ")
    if scrape_type == "Images" or scrape_type == "Music":
        if scrape_type == "Images":
            scrape_type = ('.png', '.jpg', 'jpeg')
        if scrape_type == "Music":
            scrape_type = ('.mp3')
        break
    else:
        print("Invalid file type, please choose the valid file types listed above.")
destination_directory = input("Enter output directory, example: (C:Extractor): ")
root_directory = get_steam_installation_directory() + os.path.join('\\', 'steamapps', 'common', 'Quaver', 'Songs')

if not os.path.exists(destination_directory):
    os.makedirs(destination_directory)

# Loop through each folder in the root directory
for folder_name in os.listdir(root_directory):
    folder_path = os.path.join(root_directory, folder_name)

    # Check if the current item is a directory
    if os.path.isdir(folder_path):
        image_file_path = None
        qua_file_path = None

        # Loop through files in the current folder
        for file_name in os.listdir(folder_path):
            file_path = os.path.join(folder_path, file_name)

            # Check if the current file is an image file
            if file_name.lower().endswith(scrape_type):
                # Sanitize the file name
                sanitized_file_name = sanitize_filename(file_name)

                # Copy the image file to the destination directory with the sanitized file name
                image_file_path = shutil.copy(file_path, os.path.join(destination_directory, sanitized_file_name))

            # Check if the current file is a .qua file
            if file_name.lower().endswith('.qua'):
                qua_file_path = file_path

        # Process the .qua file if an image file and .qua file are found in the same directory
        if image_file_path and qua_file_path:
            with open(qua_file_path, 'r', encoding='utf8') as qua_file:
                lines = qua_file.readlines()
                if len(lines) >= 7:
                    title_line = lines[6].strip()
                    if title_line.startswith('Title: '):
                        song_name = title_line[7:]
                        # Sanitize the song name
                        sanitized_song_name = sanitize_filename(song_name)

                        # Rename the copied image file with the sanitized song name
                        new_image_file_name = f'{sanitized_song_name}{os.path.splitext(image_file_path)[1]}'
                        new_image_file_path = os.path.join(destination_directory, new_image_file_name)
                        shutil.move(image_file_path, new_image_file_path)