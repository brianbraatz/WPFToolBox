//--------------------------------------------
// FractalCube.cs (c) 2007 by Charles Petzold
//--------------------------------------------
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Petzold.FractalCube
{
    public partial class FractalCube : Page
    {
        // Shared GeometryModel3D for the unit cube.
        GeometryModel3D modelCube;

        // Parent ModelVisual3D to which all cubes get added.
        ModelVisual3D vis3dParent;

        // Vectors pointing in all six cube sides.
        Vector3D[] vectSides = { new Vector3D(1, 0, 0), new Vector3D(-1, 0, 0),
                                 new Vector3D(0, 1, 0), new Vector3D(0, -1, 0),
                                 new Vector3D(0, 0, 1), new Vector3D(0, 0, -1) };

        // Variable and collections to keep generations going.
        int generation = 1;

        // Collection of the position where the base of each new cube is located.
        List<Point3D> listBase = new List<Point3D>();

        // Collection of vectors indicating the animated growth direction of each new cube.
        List<Vector3D> listVect;

        // Constructor.
        public FractalCube()
        {
            InitializeComponent();

            // Display message at bottom.
            txtblk.Text = (RenderCapability.Tier < 0x20000 ? "You really should be" : "Thank your for") +
                " running this program with a graphics board capable of Tier 2 rendering!";

            // Create camera and set to Viewport3D.
            viewport3d.Camera = 
                new PerspectiveCamera(new Point3D(-2, 2, 4),
                                      new Vector3D(2, -1.875, -4),
                                      new Vector3D(0, 1, 0), 45);

            // Create ModelVisual3D with light sources & add to Viewport3D.
            ModelVisual3D vis3d = new ModelVisual3D();
            Model3DGroup modgrp3d = new Model3DGroup();
            modgrp3d.Children.Add(new AmbientLight(Color.FromRgb(64, 64, 64)));
            modgrp3d.Children.Add(
                new DirectionalLight(Color.FromRgb(92, 92, 92),
                                     new Vector3D(2, -3, -1)));
            vis3d.Content = modgrp3d;
            viewport3d.Children.Add(vis3d);

            // Create a ModelVisual3D that's parent to all cubes.
            vis3dParent = new ModelVisual3D();
            viewport3d.Children.Add(vis3dParent);

            // Define a rotation transform for this parent.
            AxisAngleRotation3D rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            RotateTransform3D xformRotate = new RotateTransform3D(rotation);
            vis3dParent.Transform = xformRotate;

            // Animate the parent ModelVisual3D to keep it spinning throughout program.
            DoubleAnimation anima = new DoubleAnimation();
            anima.From = 0;
            anima.To = 360;
            anima.Duration = new Duration(TimeSpan.FromMinutes(1));
            anima.RepeatBehavior = RepeatBehavior.Forever;
            rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, anima);

            // Create a MeshGeometry3D for the unit cube.
            MeshGeometry3D meshCube = new MeshGeometry3D();

            // Unit cube vertices: front, back, left, right, top, bottom.
            meshCube.Positions = new Point3DCollection(new Point3D[] 
            { 
                new Point3D(-0.5,  0.5,  0.5), new Point3D( 0.5,  0.5,  0.5),
                new Point3D(-0.5, -0.5,  0.5), new Point3D( 0.5, -0.5,  0.5),
                new Point3D( 0.5,  0.5, -0.5), new Point3D(-0.5,  0.5, -0.5),
                new Point3D( 0.5, -0.5, -0.5), new Point3D(-0.5, -0.5, -0.5),
                new Point3D(-0.5,  0.5, -0.5), new Point3D(-0.5,  0.5,  0.5),
                new Point3D(-0.5, -0.5, -0.5), new Point3D(-0.5, -0.5,  0.5),
                new Point3D( 0.5,  0.5,  0.5), new Point3D( 0.5,  0.5, -0.5),
                new Point3D( 0.5, -0.5,  0.5), new Point3D( 0.5, -0.5, -0.5),
                new Point3D(-0.5,  0.5, -0.5), new Point3D( 0.5,  0.5, -0.5),
                new Point3D(-0.5,  0.5,  0.5), new Point3D( 0.5,  0.5,  0.5),
                new Point3D( 0.5, -0.5, -0.5), new Point3D(-0.5, -0.5, -0.5),
                new Point3D( 0.5, -0.5,  0.5), new Point3D(-0.5, -0.5,  0.5)
            });

            // Unit cube TriangleIndices.
            meshCube.TriangleIndices = new Int32Collection(new int[] 
            {  
                 0,  2,  1,  1,  2,  3,
                 4,  6,  5,  5,  6,  7,
                 8, 10,  9,  9, 10, 11,
                12, 14, 13, 13, 14, 15,
                16, 18, 17, 17, 18, 19,
                20, 22, 21, 21, 22, 23 
            });

            // Create a shareable GeometryModel3D for the cube.
            modelCube = new GeometryModel3D(meshCube, new DiffuseMaterial(Brushes.Cyan));
            modelCube.Freeze();

            // Create a new ModelVisual3D for the first cube.
            ModelVisual3D vis3dCube = new ModelVisual3D();
            vis3dCube.Content = modelCube;
            vis3dParent.Children.Add(vis3dCube);

            // Prepare the collections for the first generation.
            listVect = new List<Vector3D>(vectSides);

            foreach (Vector3D vect in vectSides)
                listBase.Add((Point3D)(0.5 * vect));

            // Kick off the process.
            AnimationOnCompleted(null, EventArgs.Empty);
        }

        void AnimationOnCompleted(object sender, EventArgs args)
        {
            // Display content in button and enable it.
            btn.Content = String.Format("Generation {0}: Create {1:N0} new cubes", 
                                        generation, listBase.Count);
            btn.IsEnabled = true;
        }

        void ButtonOnClick(object sender, EventArgs args)
        {
            // Disable button.
            btn.IsEnabled = false;

            // Create collections for next iteration.
            List<Point3D> listBaseNew = new List<Point3D>(5 * listBase.Count);
            List<Vector3D> listVectNew = new List<Vector3D>(5 * listBase.Count);

            // Create a single shared (and animated) ScaleTransform3D 
            //  with bindings on properties.
            ScaleTransform3D xformScale = new ScaleTransform3D(0, 0, 0);

            Binding bind = new Binding();
            bind.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
            bind.Path = new PropertyPath(ScaleTransform3D.ScaleXProperty);
            BindingOperations.SetBinding(xformScale,
                                ScaleTransform3D.ScaleYProperty, bind);

            bind = new Binding();
            bind.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
            bind.Path = new PropertyPath(ScaleTransform3D.ScaleXProperty);
            BindingOperations.SetBinding(xformScale,
                                ScaleTransform3D.ScaleZProperty, bind);

            // Loop through all the new cubes to build.
            for (int i = 0; i < listBase.Count; i++)
            {
                // Get the cube parameters.
                Point3D pointBase = listBase[i];
                Vector3D vectTrans = listVect[i];

                // Create a new ModelVisual3D for the cube.
                ModelVisual3D vis3d = new ModelVisual3D();
                vis3d.Content = modelCube;

                // Create a group for the appropriate transforms.
                Transform3DGroup xformgrp = new Transform3DGroup();

                xformgrp.Children.Add(
                    new TranslateTransform3D(0.5 * vectTrans));

                // This makes the cubes smaller in each generation.
                double scaleOverall = 1 / Math.Pow(2, generation);
                xformgrp.Children.Add(
                    new ScaleTransform3D(
                            scaleOverall, scaleOverall, scaleOverall));

                // This is the shared transform created outside the loop.
                xformgrp.Children.Add(xformScale);

                // This moves the cube to the base.
                xformgrp.Children.Add(
                    new TranslateTransform3D(pointBase.X, pointBase.Y, pointBase.Z));

                // Set the transform group to the ModelVisual3D.
                vis3d.Transform = xformgrp;

                // Add the ModelVisual3D to the parent ModelVisual3D.
                vis3dParent.Children.Add(vis3d);

                // Find the center of the cube.        
                Point3D pointCenter = pointBase + scaleOverall * 0.5 * vectTrans;

                // Generate information for 5 new cubes for next generation.
                foreach (Vector3D vect in vectSides)
                {
                    if (generation == 0 || vect != -vectTrans)
                    {
                        listVectNew.Add(vect);
                        listBaseNew.Add(pointCenter + scaleOverall * 0.5 * vect);
                    }
                }
            }

            // Create the animation for the ScaleTransform3D field.
            DoubleAnimation anima = new DoubleAnimation();
            anima.From = 0;
            anima.To = 1;
            anima.Duration = new Duration(TimeSpan.FromMinutes(1));
            anima.Completed += AnimationOnCompleted;
            xformScale.BeginAnimation(ScaleTransform3D.ScaleXProperty, anima);

            // Up the generation for the next call.
            generation++;
            listBase = listBaseNew;
            listVect = listVectNew;
        }
    }
}
