Sample for "Avalon" WPF (Windows Presentation Foundation) Beta 2 RC Sept 2005 CTP release

Purpose:

	Demonstrates Visual Brushes on a 3D surface. The Visual Brushes are resources passed into a custom carousel control, but they
	could be framework elements like a canvas also. Shows hittesting on mousemove. The custom carousel uses storyboarding to managed the
	animation of all the elements - reuse of animations shows a possible work flow for static animations for quicker development and 
	designer control.

Use:
	Press next\previous to move the carousel, and hover the mouse over an item to get a popup to show the hit test.

Notes:
	I noticed minor animation and hit test bug, but this sample should give an idea of the technique. It's not meant to be pretty ;)


Trackball:
	I have a Trackball in all my samples. Here is how to use it.

	1. Rotate - Right click on a 3D object and hold down, then move the mouse
	2. Scale - mouse wheel up/down
	3. Translate - make sure the viewport has focus (tab to it to see the dotted lines), hold down the space bar, click and hold down on the right mouse button, and move the mouse.

	The trackball is hooked up in code, and it uses the TransformGroup of the highest level model3dgroup transform. It also expects this transform to be setup as scale, rotate, translate.
Building:

	MSBuild.exe from command line or Visual Studio Beta 1

Running:

	Bin\release\fulltrust.exe

