from PIL import Image
import numpy as np
from collections import deque
import os

images_to_process = [
    "chickkii.png",
    "testsheep.png",
    "fox.png",
    #"dog1.png",
    #"dog2.png",
    #"Minion.png",
    #"flora.png",
    "smallObjects.png"
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


def index_valid(i, j, shape):
    return i >= 0 and j >= 0 and i < shape[0] and j < shape[1]

def get_neighbouring_indices(i, j, shape):
    res = []
    for d_i in [-1, 0, 1]:
        for d_j in [-1,0,1]:
            if d_i == 0 and d_j == 0:
                continue
            # if d_i != 0 and d_j != 0:
            #     continue
            nb = (i + d_i, j + d_j)
            if index_valid(*nb, shape):
                res.append(nb)
    return res
            

def create_uv_remap_and_boundary_texture(image_path):
    imfile = open(image_path, "rb")
    im = np.array(Image.open(imfile))
    imfile.close()
    # flip image for easy processing
    im = np.flip(im, axis=0)
    xs, ys, widths, heights = extract_bounds_from_meta_file(image_path)
    texture_im = np.zeros(shape=im.shape, dtype=im.dtype)

    if len(xs) == 0:
        # single sprites 
        xs.append(0)
        ys.append(0)
        widths.append(im.shape[1])
        heights.append(im.shape[0])

    for sprite_id in range(len(xs)):
        # Write u values in r channel
        for i_u in range(widths[sprite_id]):
            value = int(255 * min(1, widths[sprite_id] / heights[sprite_id]) * (i_u / (widths[sprite_id] - 1)))
            texture_im[ys[sprite_id]:ys[sprite_id] + heights[sprite_id], xs[sprite_id] + i_u, 0] = value

        # Write v values in b channel
        for i_v in range(heights[sprite_id]):
            value = int(255 * min(1, heights[sprite_id] / widths[sprite_id]) * (i_v / (heights[sprite_id] - 1)))
            texture_im[ys[sprite_id] + i_v, xs[sprite_id] : (xs[sprite_id] + widths[sprite_id]), 1] = value

        # compute distance to boundary map
        distance_map = np.zeros(shape=(heights[sprite_id] + 4, widths[sprite_id] + 4))
        for i in range(heights[sprite_id] + 4):
            for j in range(widths[sprite_id] + 4):
                _i = ys[sprite_id] - 2 + i
                _j = xs[sprite_id] - 2 + j
                if index_valid(_i, _j, im.shape) and im[_i, _j, 3] != 0:
                    continue
                nbs = get_neighbouring_indices(_i, _j, im.shape)
                for nb in nbs:
                    if im[nb[0], nb[1], 3] != 0:
                        distance_map[i, j] = -1
                        break
        # make sure the outer edge is zero everywhere
        distance_map[0,:] = 0
        distance_map[-1,:] = 0
        distance_map[:, 0] = 0
        distance_map[:, -1] = 0

        # mark inner pixels
        outer_map = np.zeros(shape=distance_map.shape)
        search_start = (0,0) # guaranteed to be outside
        search_queue = deque()
        search_queue.append(search_start)
        while search_queue:
            curr_pos = search_queue.pop()
            if outer_map[curr_pos] == 1:
                continue
            outer_map[curr_pos] = 1
            # add neighbours that are not boundary
            nbs = get_neighbouring_indices(*curr_pos, distance_map.shape)
            for nb in nbs:
                if distance_map[nb] != -1 and outer_map[nb] != 1:
                    search_queue.append(nb)
        inner_map = np.ones(shape=distance_map.shape) + distance_map - outer_map

        # fill in the distance map
        filled_distances = np.zeros(shape=distance_map.shape)
        for i in range(distance_map.shape[0]):
            for j in range(distance_map.shape[1]):
                if distance_map[i, j] == -1:
                    filled_distances[i, j] = 1

        iter = 1
        while True:
            new_distances = np.copy(distance_map)
            new_filled_info = np.copy(filled_distances)
            update_made = False
            for i in range(heights[sprite_id] + 4):
                for j in range(widths[sprite_id] + 4):
                    if inner_map[i, j] == 0:
                        continue
                    if filled_distances[i, j] == 1:
                        continue

                    nb_vals = []
                    for nb in get_neighbouring_indices(i, j, distance_map.shape):
                        if filled_distances[nb] == 1:
                            nb_vals.append(distance_map[nb])
                    if len(nb_vals) > 0:
                        new_distances[i, j] = np.min(nb_vals) + 1
                        new_filled_info[i, j] = 1
                        update_made = True

            distance_map = np.copy(new_distances)
            filled_distances = np.copy(new_filled_info)
            iter += 1
            if not update_made:
                break
        
        #transfer data to texture
        max_val = np.max(distance_map)
        for i in range(heights[sprite_id] + 4):
            for j in range(widths[sprite_id] + 4):
                _i = ys[sprite_id] - 2 + i
                _j = xs[sprite_id] - 2 + j
                # if distance_map[i, j] == -1:
                #     texture_im[_i, _j, 2] = 255
                if not index_valid(i, j, im.shape) or distance_map[i, j] <= 0:
                    continue
                texture_im[_i, _j, 2] = int(255 * distance_map[i, j] / max_val)

    texture_im[:,:,3] = 255

    print(texture_im.shape)
    texture_im_save = Image.fromarray(np.flip(texture_im, axis=0), mode="RGBA")
    texture_im_save.save(image_path[:-4] + "_tear_text.png")

def try_to_copy_metadata(image_path):
    orig_meta_path = image_path + ".meta"
    if not os.path.isfile(orig_meta_path):
        print("No metadata to copy found")
        return
    new_meta_path = image_path[:-4] + "_tear_text.png.meta"
    if not os.path.isfile(new_meta_path):
        print("No target meta data file found")
        return
    orig_meta_file = open(orig_meta_path, "rt")
    new_meta_file = open(new_meta_path, "rt")

    orig_data = orig_meta_file.readlines()
    new_meta_data = new_meta_file.readlines()

    fused_new_meta_data = new_meta_data[:2] + orig_data[2:]

    orig_meta_file.close()
    new_meta_file.close()

    with open(new_meta_path, "w") as new_meta_fh:
        new_meta_fh.writelines(fused_new_meta_data)
    

if __name__ == "__main__":
    for im_path in images_to_process:
        create_uv_remap_and_boundary_texture(im_path)
        try_to_copy_metadata(im_path)
        print(im_path, "processed")

        