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
                xs.append(int(line.split(": ")[1]))
                continue
            if (line.strip()[:2] == "y:"):
                ys.append(int(line.split(": ")[1]))
                continue
            if (line.strip()[:6] == "width:"):
                widths.append(int(line.split(": ")[1]))
                continue
            if (line.strip()[:7] == "height:"):
                heights.append(int(line.split(": ")[1]))
                continue
    return xs, ys, widths, heights

def create_uv_remap_and_boundary_texture(image_path):
    im = np.array(Image.open(image_path))
    # flip image for easy processing
    im = np.flip(im, axis=0)
    xs, ys, widths, heights = extract_bounds_from_meta_file(image_path)
    print(im.shape)
    print(im.dtype)

    texture_im = np.zeros(shape=im.shape, dtype=im.dtype)
    print(texture_im.shape)


    for i in range(len(xs)):
        cropped_sprite = im[ys[i]:ys[i] + heights[i], xs[i]:xs[i] + widths[i]]
        test_im = Image.fromarray(np.flip(cropped_sprite, axis=0))
        test_im.save("testtest_" + str(i) + ".png")

        print(widths[i] / heights[i])
        # Write u values in r channel
        for i_u in range(widths[i]):
            value = int(255 * min(1, widths[i] / heights[i]) * (i_u / (widths[i] - 1)))
            texture_im[ys[i]:ys[i] + heights[i], xs[i] + i_u, 0:3] = value

        # Write v values in b channel
        for i_v in range(heights[i]):
            value = int(255 * min(1, heights[i] / widths[i]) * (i_v / (heights[i] - 1)))
            texture_im[ys[i] + i_v, xs[i] : (xs[i] + widths[i]), 1] = value

        texture_im[:,:,3] = 255

    print(texture_im.shape)
    texture_im_save = Image.fromarray(np.flip(texture_im, axis=0))
    texture_im_save.save("test_text_.png")



if __name__ == "__main__":
    create_uv_remap_and_boundary_texture(images_to_process[0])