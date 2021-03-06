# GGSTCollisionEditor
![image](https://user-images.githubusercontent.com/9942055/161677765-33c4024e-c22a-4923-97ad-9555831fdf65.png)

A hitbox editor for Guilty Gear -Strive-. Made with Windows App SDK and WinUI 3, written in C#. A lot of the backend is borrowed from https://github.com/Labreezy/bb-collision-editor/blob/master/BBCollisionEditor/.

Currently, you can edit, add, and remove boxes from existing sprites. You cannot currently add new sprites, rename sprites, or reassign sprite animations. There is currently no error handling, so if you try to edit a PAC from Xrd, DBFZ, or GBVS, the program will just crash.

This was originally just made as an experimental app to test new Windows features. As a result, I can only guarantee functionality on Windows 10 Fall 2018 or above. While it shouldn't be too hard to port to a normal Windows executable, I don't really have the energy or time right now. If you're *really* impatient, use my archived original version here: https://github.com/WistfulHopes/ggst-collision-editor

There are two installation options: Unpackaged, and MSIX Packaged. MSIX Packaged uses the MSIX format to install the program to your computer like a Microsoft Store app, while Unpackaged works like a normal downloaded executable.
