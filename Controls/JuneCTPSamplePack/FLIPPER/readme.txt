Sample for "Avalon" WPF (Windows Presentation Foundation) Beta 2 RC Sept 2005 CTP release

Purpose:

	Demostrates transitional effect with 3D. The UV's on the planes are setup to only show top/bottom half of an image.
	Shows 3D animations using xaml.

Use:
	SingleStep - flip once
	Toggle AutoRUn - starts a timer to flip automatically

Notes:
	The directional lighting works well here to show a nice seam. There are 4 planes used to create the effect.

Trackball:
	I have a Trackball in all my samples. Here is how to use it.

	1. Rotate - Right click on a 3D object and hold down, then move the mouse
	2. Scale - mouse wheel up/down
	3. Translate - make sure the viewport has focus (tab to it to see the dotted lines), hold down the space bar, click and hold down on the right mouse button, and move the mouse.

	The trackball is hooked up in code, and it uses the TransformGroup of the highest level model3dgroup transform. It also expects this transform to be setup as scale, rotate, translate.

Building:

	MSBuild.exe from command line or Visual Studio

Running:

	Bin\Debug\Flipper.exe

