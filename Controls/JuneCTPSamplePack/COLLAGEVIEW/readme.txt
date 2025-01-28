Sample for "Avalon" WPF (Windows Presentation Foundation) Beta 2 RC Sept 2005 CTP release

Purpose:

	Demostrates MaterialGroups animations in xaml. This is useful for background effects. I used this technique in the Nimbus and MediaMania demos.

Use:
	Click on the Image 1 animate button to start the animation.

Notes:
	I use Emissive materials to create additive blending which creates hotter glow effects.
	Use the Trackball to zoom, rotate and translate to a point of interest. Resize the window to resize the viewport. 
	Using different textures with differnt animations creates different points of interest within the texture. So, zooming into an isolated spot can look really cool.
	Remember that with this control you can tilt the mesh in 3D with the trackball. So you can get lots of different 3D angles and views.
	Of course, you can use other 3D models like spheres, cones, whatever to create different effects too.
	Using different textures over the top of each other are interesting also.
	The possibilites are endless.

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

