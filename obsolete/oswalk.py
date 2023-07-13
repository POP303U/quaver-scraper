import os
import shutil
import re

root_directory = os.path.join('C:', 'Program Files (x86)', 'Steam', 'steamapps', 'common', 'Quaver', 'Songs')
destination_directory = os.path.join('C:', 'oswalk')

# Function to sanitize the file name by removing or replacing invalid characters
def sanitize_filename(filename):
    invalid_chars = r'[<>:"/\\|?*]'
    safe_char = '_'
    return re.sub(invalid_chars, safe_char, filename)

if not os.path.exists(destination_directory):
    os.makedirs(destination_directory)

# Traverse the root directory and its subdirectories
for dirpath, dirnames, filenames in os.walk(root_directory):
    # Process image files
    for filename in filenames:
        if filename.lower().endswith(('.jpg', '.jpeg', '.png', '.gif')):
            # Sanitize the file name
            sanitized_file_name = sanitize_filename(filename)

            # Copy the image file to the destination directory with the sanitized file name
            source_path = os.path.join(dirpath, filename)
            destination_path = os.path.join(destination_directory, sanitized_file_name)
            shutil.copy(source_path, destination_path)

            # Extract song name from .qua file
            qua_file_path = os.path.join(dirpath, filename.rsplit('.', 1)[0] + '.qua')
            with open(qua_file_path, 'r', encoding='utf8') as qua_file:
                lines = qua_file.readlines()
                if len(lines) >= 7:
                    title_line = lines[6].strip()
                    if title_line.startswith('Title: '):
                        song_name = title_line[7:]
                        # Sanitize the song name
                        sanitized_song_name = sanitize_filename(song_name)

                        # Rename the copied image file with the sanitized song name
                        new_image_file_name = f'{sanitized_song_name}{os.path.splitext(sanitized_file_name)[1]}'
                        new_image_file_path = os.path.join(destination_directory, new_image_file_name)
                        os.rename(destination_path, new_image_file_path)
