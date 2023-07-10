import os
import string
import shutil

def get_steam_installation_directory():
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
    
    return None

def extract_images(song_directory, output_directory):
    # Check if the output directory exists, create it if not
    if not os.path.exists(output_directory):
        os.makedirs(output_directory)
    
    # Iterate through the files in the song directory
    for root, _, files in os.walk(song_directory):
        for file in files:
            # Check if the file is a .qua file
            if file.lower().endswith('.qua'):
                qua_file_path = os.path.join(root, file)
                song_name = get_song_name_from_qua(qua_file_path)
                if song_name:
                    # Extract images using the new function
                    extract_image(root, output_directory, song_name)
    
    print("Image extraction complete!")

def get_song_name_from_qua(qua_file_path):
    # Parse the .qua file to retrieve the song name
    with open(qua_file_path, 'r', encoding='utf-8', errors='ignore') as qua_file:
        for line in qua_file:
            if line.startswith("TITLE:"):
                song_name = line[7:].strip()
                return song_name
    
    return None

def extract_image(directory, output_directory, song_name):
    print("Scraping...")
    # Iterate through the files in the song directory
    for root, _, files in os.walk(directory):
        for file in files:
            # Check if the file is an image file
            if file.lower().endswith(('.png', '.jpeg', '.jpg')):
                # Create the full path for the image file
                file_path = os.path.join(root, file)
                
                # Copy and rename the image file to the output directory
                new_filename = song_name + "_" + file
                new_file_path = os.path.join(output_directory, new_filename)
                shutil.copy(file_path, new_file_path)

# Call the function to get the Steam installation directory
steam_directory = get_steam_installation_directory()

if steam_directory:
    song_directory = os.path.join(steam_directory, "steamapps", "common", "Quaver", "Songs")
    output_directory = "C:TESTTESTTESTTEST"  # Provide the desired output directory path here

    extract_image(song_directory, output_directory, "sus")
else:
    print("Steam installation directory not found.")