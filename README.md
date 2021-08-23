
# MicrosopyGUI
This program is used to give the user a digital interface to process and work with images captured by a uEye camera in regards to microscopy. 

It is separated into 3 distinct workspaces: The "Control Panel", the "Image Viewer" and the "Image Gallery".  

<p>
  <img width="48%" src="https://user-images.githubusercontent.com/40501092/130403036-fa2a9b13-b433-463d-a705-089dcf6b9244.PNG"/>
  <img align="right" width="48%" src="https://user-images.githubusercontent.com/40501092/130403066-7660827a-d4ba-49c2-820d-91a18865c508.PNG"/>
</p>

## Control Panel:
This is where the user can configure the camera settings or add post processing effects to the image. The post processing can either be done on the live camera feed or on a frozen frame of the live feed, which can be toggled at the top of the screen. 

Once the user is satisfied with the quality of the image, they can save the image by clicking the "Save Frame" button or by saving it through the menu bar under File -> Save Current Frame. Additionally, all of the slider values will be saved in the [metadata](https://user-images.githubusercontent.com/74535078/129571256-ea9d4cba-2f76-4c79-bc6d-88285dfc61f5.PNG) of the image and the user can add more metadata by using the enter key or delete redundant metadata by using backspace.

At the bottom of the Control Panel is an histogram, which displays the amount of red, green and blue pixels of the current image on the y-axis and their corresponding intensity in the range of 0-255 on the x-axis.

##  Image Viewer:

This is the most important segment of this software since this is where the live camera feed/the frozen frame, on which the processing occurs, is displayed.
The image can be dragged around while holding down the middle mouse button and it returns to its initial position with the press of the R key.

With the scroll wheel, the image can be digitally zoomed in/out, however, it is preferred to adjust the settings on the microscope itself to achieve that.

The right mouse button click will open up a [popup](https://user-images.githubusercontent.com/74535078/129575532-65981763-877c-4159-aa08-f3193cd1fef2.png) in which the user can select their preferred measuring tool (either a rectangle, a line, or a size calibration with the help of a predetermined size factor).

With the left mouse button click the user now can measure distances with the before selected tool and with a single left mouse button click, the measurement can be removed.  

<p>
  <img width="48%" src="https://user-images.githubusercontent.com/74535078/129682106-c1f2eb5a-d7f6-4e0c-8504-02604f80ffd6.PNG"/>
  <img align="right" width="48%" src="https://user-images.githubusercontent.com/74535078/129682008-417787d3-305b-4370-b795-a9907c9a158f.PNG"/>
</p>


## Image Gallery

The Image Gallery contains all of the images which are in the folder that is currently chosen.
The path of the current folder can be viewed in the setting tab of the menu bar and changed with the "Change Image Gallery Folder" tab.

By double-clicking any of the shown images, the image itself will be displayed in the Image Viewer for further postprocessing or analysis.

By hovering long enough over one image, its name is going to show up as a tooltip.

To delete one of the images, [right-click](https://user-images.githubusercontent.com/74535078/129578381-9e50dc02-70d1-46fb-be63-cfe3eeb3319e.png) on the image and press the delete button. With this, the user can also edit the metadata of any given image.

## Keybinds
<pre>
RMB:                   Select measuring tool
Shift + LMB:           Alternative measuring mode
Middle mouse button:   Move image
Mousewheel:            Zoom in on image
RMB on Image Gallery:  Edit metadata or delete image 

R:                     Reset image position
F1:                    Open settings
F5:                    Reload Camera
CTRL + S:              Save image
CTRL + SHIFT + S:      Save config
CTRL + O:              Load config
CTRL + L:              Activate live feed
CTRL + F:              Freeze camera
ESC:                   Close any window
</pre>
