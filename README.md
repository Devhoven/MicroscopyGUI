# MicrosopyGUI
This program is used to give the user a digital interface to process and work with images captured by a uEye camera in regards to microscopy. 

It is separated into 3 distinct workspaces: The "Controlpanel", the "Image Viewer" and the "Image Gallery".  

<p>
  <img width="48%" src="https://user-images.githubusercontent.com/74535078/129679506-115af088-4316-46bd-9fcc-c677359742ce.PNG"/>
  <img align="right" width="48%" src="https://user-images.githubusercontent.com/74535078/129680899-ea09e08f-7245-457f-9d13-1142ca62d761.PNG"/>
</p>

## Controlpanel:
This is where the user can pre-/postprocess the image with the help of various settings. The processing can either be done on the live camera feed or on a freezeframe of the current image, which can be toggled at the top of the screen. 

Once the user is satisfied with the quality of the image, they can save the image by clicking the "Save Frame" button or saving it through the menu bar under File -> Save Current Frame. Additionally, all of the slider values will be saved in the [metadata](https://user-images.githubusercontent.com/74535078/129571256-ea9d4cba-2f76-4c79-bc6d-88285dfc61f5.PNG) of the image and the user can add more metadata by using the enter key or delete redundant metadata by using backspace.

At the bottom of the Controlpanel is the histogram located, which displays the amount of red, green and blue pixels of the current image on the y-axis and their corresponding intensity in the range of 0-255 on the x-axis.

##  Image Viewer:

This is the most important segment of this software since this is where the live camera feed/the frozen frame, on which the processing occurs, is displayed.
The image can be dragged around while holding down the middle mouse button and it returns to its initial position with the press of the R key.

With the scroll wheel, the image can be digitally zoomed in/out, however, it is preferred to adjust the settings on the microscope itself to achieve that.

The right mouse button click will open up a [popup](https://user-images.githubusercontent.com/74535078/129575532-65981763-877c-4159-aa08-f3193cd1fef2.png) in which the user can select their preferred measuring tool (either a rectangle, a line, or a size calibration with the help of a predetermined size factor).

With the left mouse button click the user now can measure distances with the before selected tool and with a single left mouse button click, the measurement can be removed.  

![Measuring](https://user-images.githubusercontent.com/74535078/129679637-603d19df-318e-4d77-b099-4a04d914b72e.PNG)

## Image Gallery

The Image Gallery contains all of the images which are in the folder that is currently chosen.
The path of the current folder can be viewed in the setting tab of the menu bar and changed with the "Change Image Gallery Folder" tab.

By double-clicking any of the shown images, the image itself will be displayed in the Image Viewer for further postprocessing or analysis.

By hovering long enough over one image, its name is going to show up as a tooltip.

To delete one of the images, [right-click](https://user-images.githubusercontent.com/74535078/129578381-9e50dc02-70d1-46fb-be63-cfe3eeb3319e.png) on the image and press the delete button. With this, the user can also edit the metadata of any given image.

## Keybinds
<pre>
* R:                     Resets image to original position
* RMB on Image Viewer:   Select measuring tool
* Middle mouse button:   Moves image
* Mousewheel:            Zooming
* LMB on Image Viewer:   Remove measuring
* Shift + LMB:           Measure in a straight line
* RMB on Image Gallery:  Edit metadata or delete image 
</pre>
