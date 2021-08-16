# MicrosopyGUI
This programm is used to give the user a digital interface to process and work with images captured by a uEye camera in regards to microscopy. 

It is seperated into 3 destinct workspaces: The "Controlpanel", the "Imageviewer" and the "Image Gallery"

## Controlpanel:
This is where the user is able to pre-/postprocess the image with the help of various settings. The processing can either be done on the live camera feed or on a freezeframe of the current image, which can be toggled at the top of the screen. 

Once the user is satisfied with the quality of the image, they can save the image by clicking the "Save Frame" button or saving it through the menu bar under File -> Save Current Frame. Additionally all of the slidervalues will be saved in the [metadata](https://user-images.githubusercontent.com/74535078/129571256-ea9d4cba-2f76-4c79-bc6d-88285dfc61f5.PNG) of the image and the user is able to add more metadata by using the enter key or delete redundant metadata by using backspace.

At the bottom of the Controlpanel is the histrogram located, which displays the amount of red, green and blue pixels of the current image on the y-axis and their corresponding intensity in the range of 0-255 on the x-axis.

##  Imageviewer:

This is the most important segment of this software since this is where the live camera feed/the frozen frame, on which the processing occurs, is displayed.
The image can be dragged around while holding down the middle mouse button and it returns to its initial postion with the press of the R key.

With the scrollwheel the image can be digitally zoomed in/out, however it is preferred to adjust the settings on the microscope itself to achieve that.

The right mouse button click will open up a [popup](https://user-images.githubusercontent.com/74535078/129575532-65981763-877c-4159-aa08-f3193cd1fef2.png) in which the user can select their preferred measuring tool (either a rectangle, a line, or a size calibration with the help of a predetermined sizefactor).

With the left mouse button click the user now is able to measure distances with the before selected tool and with a single left mouse button klick, the measurement can be removed.

## Image Gallery

The Image Gallery contains all of the images which are in the folder that is currently chosen.
The path of the current folder can be viewed in the setting tab of the menu bar and changed with the "Change Image Gallery Folder" tab.

By doubleclicking any of the shown images, the image itself will be displayed in the Imageviever for further postprocessing or analysis.

By hovering long enough over one image, its name is going to show up as a tooltip.

To delete one of the images, [right-click](https://user-images.githubusercontent.com/74535078/129578381-9e50dc02-70d1-46fb-be63-cfe3eeb3319e.png) on the image and press the delete button. With this the user can also edit the metadata of any given image.

## Keybinds

* R: Resets Image to original position
* Right click on Imageviewer: select measuring tool
* Middle mouse button: moves image
* Left click on Imageviewer: remove measuring
* Right click on Image Gallery: edit metadata or delete image 
