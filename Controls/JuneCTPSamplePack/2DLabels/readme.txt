Sample for "Avalon" WPF (Windows Presentation Foundation) Beta 2 RC Sept 2005 CTP release

Purpose:

	Demostrates conversion from a 3D point to 2D screen coordinates. Used for 2D labels pointing to 3D objects.

Use:
	Click on the "set line" button to set the 2D line to point to the upper left vertex point of red 3D rectangle.
	Click on the other buttons to transform the 3D rectangle while the 2D stays attached to the 3D vertex point.

Notes:
	This uses a MatrixCamera to do the conversion. Other cameras in WPF don't expose view\projection matrix. 
	Therefore, using the MatrixCamera is the only way to do the conversion. In Version 2, this should change.
	The negative of using the MatrixCamera is that the app is now responsible for the camera which is more work for the app.
	When doing the conversion, the vertex point must be transformed all the way to world coordinates - meaning all parents with transforms must be applied to the point.

Trackball:
	I have a Trackball in all my samples. Here is how to use it.

	1. Rotate - Right click on a 3D object and hold down, then move the mouse
	2. Scale - mouse wheel up/down
	3. Translate - make sure the viewport has focus (tab to it to see the dotted lines), hold down the space bar, click and hold down on the right mouse button, and move the mouse.

	The trackball is hooked up in code, and it uses the TransformGroup of the highest level model3dgroup transform. It also expects this transform to be setup as scale, rotate, translate.

Building:

	MSBuild.exe from command line or Visual Studio Beta 1

Running:

	Bin\Debug\MaterialGroups.exe

