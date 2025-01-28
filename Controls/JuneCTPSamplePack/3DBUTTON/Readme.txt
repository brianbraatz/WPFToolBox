Sample for "Avalon" WPF (Windows Presentation Foundation) Beta 2 RC 

Purpose:

	Demonstrates the benefit of a 3D button. 
	Uses specular for material reflection types and morph meshing when clicked on to simulate pressing into the button.

Use:
	Hover and click on the buttons

Notes:
	On the morph mesh, I have a bug because I mixed a storyboard into the timing for the text drop. This was a quick way to see the effect and it works 90% of the time which is good enough for the sample.


Building:

	MSBuild.exe from command line or Visual Studio Beta 1

Running:

	Bin\release\fulltrust.exe

