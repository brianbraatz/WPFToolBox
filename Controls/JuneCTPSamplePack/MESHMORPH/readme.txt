Sample for Avalon Beta 1 RC May CTP release

Purpose:

	Demostrates a technique do software mesh morphing through software interpolation. The 3 models in the sample were created using Zam3D.
	Also shows the use of CompositionTarget.Rendering which is used as the drawing loop which allows for indpendent frame rate tied to the application to stay in sync with other animations.

Use:
	Click on "Stage One to Two" to see the mesh morph animation from model 1 to model 2.
	Click on "Stage Two to One" to see the mesh morph animation from model 2 to model 1.
	Click on "Stage One to Three" to see the mesh morph animation from model 1 to model 3.
	Click on "Stage Three to One" to see the mesh morph animation from model 3 to model 1.
	Animage Stages Toggle will cycle through 1 to 2 to 1 to 3 and etc...

Notes:
	Avalon3D does not yet support vertex tweening\blending at the hardware level. I found that changing the actual vertex positions ran into serious perf issues - something the Avalon 3D is aware of.
	The alternative approach is to recreate a model for each "key frame" in software. This is fairly straight forward, and the performance is very good (much to my surprise)
	I created a class that allows you to pass in 2 models, and an event handler returns when it's done animating. So, this should be easy for novices to reuse.

	If you create a model in Studio Max 3D or Zam 3D, make sure your model stages have the same number of vertex points. This is easy using Zam3D's advanced tab to edit the mesh, and then export the xaml into a stage file.

Trackball:
	I have a Trackball in all my samples. Here is how to use it.

	1. Rotate - Right click on a 3D object and hold down, then move the mouse
	2. Scale - mouse wheel up/down
	3. Translate - make sure the viewport has focus (tab to it to see the dotted lines), hold down the space bar, click and hold down on the right mouse button, and move the mouse.

	The trackball is hooked up in code, and it uses the TransformGroup of the highest level model3dgroup transform. It also expects this transform to be setup as scale, rotate, translate.

	Note: Animating in xaml for Beta makes a copy of the Viewport3D. Any alterations to the Model during animation will be lost. You will see this when you trackball during an animation, and when an animation ends the model is reset to when the animation started. This is a known issues which is being addressed.
	
Building:

	MSBuild.exe from command line or Visual Studio Beta 1

Running:

	Bin\Debug\MorphMesh.exe

