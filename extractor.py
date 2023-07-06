import os
import shutil

def extract_images(song_directory, output_directory, scrape_type):
    print("Scraping...")
    # Check if the output directory exists, create it if not
    if not os.path.exists(output_directory):
        os.makedirs(output_directory);
    
    # Iterate through the files in the song directory
    for root, _, files in os.walk(song_directory):
        for file in files:
            # Check if the file is an image file
            if file.lower().endswith((scrape_type)):
                # Create the full path for the image file
                image_path = os.path.join(root, file);
                
                # Copy the image file to the output directory
                shutil.copy(image_path, output_directory);

    print("Scraping complete!");

print("\x1B[2J\x1B[1;1H");
print("#-----------------------------------------------------------#");
print("|               quaver-scraper [version 1.0]                |");
print("#-----------------------------------------------------------#");
scrape_type = input("What type of files do you want to scrape: (.png, .mp3, .qua): ");
output_dir = input("Enter output directory, example: (C:\Extractor): ");
input_dir = input("Enter input directory, leave blank if Quaver is installed through Steam: ");
if len(input_dir) == 0:
    input_dir = "C:\Program Files (x86)\Steam\steamapps\common\Quaver\Songs"
extract_images(input_dir, output_dir, scrape_type);
