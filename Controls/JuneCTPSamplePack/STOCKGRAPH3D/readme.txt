Readme for StockGraph3D 

Purpose:


1. databinding in a 3D layout
2. Animations among a large number of 3D mesh objects
3. Performance with a large number of 3D mesh objects
4. Use as a general graph for any purpose, in this case for stocks in demos.

General:

1. This app requires an internet connection as it uses yahoo stock services to get its data

2. The code is general demo\prototype code to show functionality. It is in no way architected or tested very much. It can be expanded to provide functionality for quick prototyping.

3. The stock service call in not async. The focus is on the graphing control not the data.



Use:



1. Click on the "Graph Stocks" button to generate a graph with the current parameters.

2. To add a stock, enter the stock symbol in the "Stock" text box, choose the brush color in the drop down list, and click "add stock".

3. After adding stocks to your list, type in your From/To dates. The "i" in "y/m/d/i" is the interval. For example, 1 would be an interval of 1 day. 30 would be an interval of 30 days. If you have a from\to span of 360 days, you may want a graph point every 15 days rather than 1 every day. Animations will start to slow down as you add more and more mesh objects. I can have about 360 mesh objects and still animate at 44 frames per second.

4. Once you have bulit a graph via the "Graph Stocks" button, you may want to animate between graphs. After you have built a graph, change the from/to dates, and click on "Animate Stocks". ONce the 2nd graph is built, you can use "animate to" and "animate from" buttons to animate between the graphs.

5. You can also do 3D hit testing on the graph objects. Click on on the of the 3D graph objects, and the information on that graph point will show up in the "Hit Test" area.

6. The information below the animate buttons are the Framework elements that were generated for the labels. These framework elements are also databound.

7. I have a Trackball in all my samples. Here is how to use it.

	1. Rotate - Right click on a 3D object and hold down, then move the mouse
	2. Scale - mouse wheel up/down
	3. Translate - make sure the viewport has focus (tab to it to see the dotted lines), hold down the space bar, click and hold down on the right mouse button, and move the mouse.

	The trackball is hooked up in code, and it uses the TransformGroup of the highest level model3dgroup transform. It also expects this transform to be setup as scale, rotate, translate.


Details:


1. THe data model for binding is in StocksDataModel.cs. This is where all the data is processed.

2. THe Graph3D control located in the 3DControls folder is a general layout control for 3D items. In this case, I use the Relayout() method to lay out the 3D pieces. The List3DItems are pulled in from Model.xaml. The grid with labels is in Grid3DMOdel.xaml. SO you can change the type of model by dropping in a different mesh.

3. Myapp.xaml has the styles for the labels, the data model instantiation. In the 3DControls\ControlLibrary\Themes\generic.xaml objects that theme the graph are defined here - I was experimenting with this type of re-usable resource dictionary apporach to show the possibility for better workflow.

4. Graph3D control has a number of properties to let you change angles and camera positions.


How to use List3D Class:


List3D is a Class that I use when I want to layout items in a single viewport3D that requires data binding to data models. It's not well architected, but it gets the job done. Below is a description on how to use it for your own desires.


The List3DItem.cs loads the Model.xaml and provides methods to animate and etc. For starters, in Model.xaml, set the material for each mesh to a solidcolorbrush.

If you use resources for your meshes, just define them in MyApp.xaml. There is a section called <!-- 3D Resources -->, and I define a PlaneMesh there which I use as the mesh in the CDModel.xaml. You can of course embed the mesh into the model if you want.

Next you need to look at Window1.xaml and look at <graphList3D:List3D../> This is where the control gets instantiated. Also look in the <!-- 3D Resources for List 3D --> section to see resources I define for the control.

The key to getting something to showup is to have something to databind too. Since List3D is an ItemsControl, you can add things to it manually by adding a List3DItem as its children, but you will want to get databinding working anyway, and I already have a datamodel you can bind to called StocksData.

The StocksData datamodel is defined in StockDataModel.cs, and it is instantiated in MyApp.xaml. 

You can see how to add items into the datamodel by going to StockDataModel.cs AddGraph3DItem(). At this point, you will care about filling in the ID which should be unique to the product (this allows you to use methods later that I have when 3D hit testing occurs). Next, just add the List3DItem to the appropriate collection code...Data.List3DItesm.Add (item3D) .

At this point, the List3D item will layout the objects based on the items (generally scaling stuff to fit into a box) , and presto magico you have your 3D objects.

Also, in the Window1.xaml where graphList3D:List3D is was declared, there is a ItemSelected and ItemUnselected events which are in window1.xaml.cs. If you click on a 3D item, you will get the ItemSelected event fired off with the List3DItem. I have a region called Events which is where the event method is located. 

Once you get this far, all that is left is to layout the 3D items as you desire. To do that go to List3D.cs in the 3DControls\List3D folder. Find the Relayout() method and you will see me lay things out. This code was used for a number of different projects, and there are remnants of that code floating around...probably best to step through the code.

Once you get the layout you want, go to the SelectItem() and UnselectItem() methods to alter the selection\unselection states to what you want. Here you will see something like select.AnimatePosition(select.FrontPosition). During the relayout method I setup the different positions I will be animating to, and then I only need to use the List3DItem AnimatePosition method to move from one spot to the other. I don't have multiple keyframes, but you could implement it because I have eventhanders for when animations end from one position to another.

Once you get this working you will need to modify List3DItem to expose a property for the brushes you will be using. I already have property in List3DItem.cs called MainImage. You will want the MAIN_IMAGE_INDEX to point to the mesh location in Model.xaml.

You may find white papers on this floating about as folks try to use this approach.

Good luck. 

