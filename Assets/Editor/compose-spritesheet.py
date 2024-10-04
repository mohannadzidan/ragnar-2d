from PIL import Image
import os
import math;

MAX_PER_ROW = 10;

def create_spritesheet(frames, output_path, animation_name, direction_name):
    # Load the first image to determine the size
    first_image = Image.open(frames[0])
    width, height = first_image.size

    total_height = math.ceil(len(frames) / MAX_PER_ROW ) * height;
   
    # Calculate the total width of the spritesheet
    total_width =  min(MAX_PER_ROW * width, width * len(frames) )

    # Create a new blank image for the spritesheet
    spritesheet = Image.new('RGBA', (total_width, total_height))
    filename = f"{animation_name}_{width}x{height}_{len(frames)}_{direction_name}.png"

    # Paste each image into the spritesheet
    for i, image_path in enumerate(frames):
        img = Image.open(image_path)
        y = (i // MAX_PER_ROW)  * height;
        x = (i % MAX_PER_ROW) * width;
        spritesheet.paste(img, (x, y));

    filepath = os.path.join(output_path, filename); 
    # Save the spritesheet
    spritesheet.save(filepath)
    print(f"Spritesheet created and saved to {output_path}")


def compose():
    cwd = os.path.dirname(os.path.realpath(__file__))
    anim_folders_path = os.path.join(cwd, "player", "character");
    childPath = os.path.relpath(anim_folders_path, cwd);


    for animation_name in os.listdir(anim_folders_path):
        directions_directory = os.path.join(anim_folders_path, animation_name);
        for direction_name in os.listdir(directions_directory):
            direction_directory = os.path.join(directions_directory, direction_name); 
            frames_filenames =  os.listdir(os.path.join(directions_directory, direction_name));
            frames =  [os.path.join(direction_directory, f) for f in sorted(frames_filenames) if f.endswith('.png')]
            output_path = os.path.join(cwd, 'output', childPath); 
            if not os.path.exists(output_path):
                os.makedirs(output_path)
            create_spritesheet(frames, output_path, animation_name, direction_name)


if __name__ == '__main__':
    compose()