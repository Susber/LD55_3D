from PIL import Image
import numpy as np

images_to_process = [
    "chickkii.png",
    "testsheep.png"
]

def extract_bounds_from_meta_file(image_path):
    meta_path = image_path + ".meta"
    xs = []
    ys = []
    widths = []
    heights = []

    with open(meta_path, "r") as f:
        for line in f:
            if (line.strip()[:2] == "x:"):
                xs.append()
                continue
            if (line.strip()[:2] == "y:"):
                continue
            if (line.strip()[:6] == "width:"):
                continue
            if (line.strip()[:7] == "height:"):
                continue

if __name__ == "__main__":
    extract_bounds_from_meta_file(images_to_process[0])