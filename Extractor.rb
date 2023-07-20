require 'fileutils'

def get_steam_installation_directory
  case RbConfig::CONFIG['host_os']
  when /mswin|mingw/
    default_directory = File.join(ENV['ProgramFiles(x86)'], 'Steam')
    return default_directory if Dir.exist?(default_directory)
    Dir.glob('A:/*/Program Files (x86)/Steam').first || Dir.glob('C:/*/Program Files (x86)/Steam').first
  when /darwin/
    File.join(ENV['HOME'], 'Library', 'Application Support', 'Steam')
  when /linux/
    File.join(ENV['HOME'], '.steam', 'steam')
  end
end

def get_qua_title(qua_file_path)
  File.open(qua_file_path, 'r') do |qua_file|
    lines = qua_file.readlines
    song_name = lines[6].strip[7..-1]
    song_name
  end
end

def sanitize_filename(filename)
  invalid_chars = /[<>:"\/\\|?*]/
  safe_char = ''
  filename.gsub(invalid_chars, safe_char)
end

puts '#-----------------------------------------------------------#'
puts '|               quaver-scraper [version 1.3]                |'
puts '#-----------------------------------------------------------#'

input = nil
allowed_extension = /\.(jpg|png|jpeg)$/

loop do
  print "What type of files do you want to scrape: (Images, Music): "
  input = gets.chomp

  if input == "Images"
    break
  elsif input == "Music"
    allowed_extension = /\.(mp3)$/
    break
  else
    puts "Invalid file type, please choose the valid file types listed above."
  end
end

print "Enter output directory, example: (C:Extractor): "
destination_directory = gets.chomp

root_directory = File.join(get_steam_installation_directory, 'steamapps', 'common', 'Quaver', 'Songs')
qua_title_name = ''

FileUtils.mkdir_p(destination_directory) unless Dir.exist?(destination_directory)

Dir.glob(File.join(root_directory, '**/*')).each do |path|
  if File.file?(path)
    file_name = File.basename(path)
    extension = File.extname(path).downcase

    if extension == '.qua'
      qua_title_name = sanitize_filename(get_qua_title(path))
    end

    if extension =~ allowed_extension
      destination_path = File.join(destination_directory, "#{qua_title_name}#{extension}")
      FileUtils.cp(path, destination_path, preserve: false)
    end
  end
end 
