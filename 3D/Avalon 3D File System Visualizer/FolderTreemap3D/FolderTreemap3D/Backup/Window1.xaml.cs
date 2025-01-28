using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Globalization;
using System.IO;

namespace Arthur
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>

	public partial class Window1 : Window
	{
		private string _folderName = @"C:\Windows\Microsoft.NET\Windows";
		private Datatree _folderTree;
		private TreemapNode _treemapRoot;
		private BackToFrontTreemap _btfTreemap;

		private PerspectiveCamera _pc = new PerspectiveCamera();
		private OrthographicCamera _oc = new OrthographicCamera();

		private Color _fileColor = Color.FromArgb(0x80, 0x7F, 0xFF, 0x7F);
		private Color _textColor = Color.FromArgb(0x6F, 0xA0, 0xA0, 0xA0);

		private double _layerDepth = 0.5f;

		public Window1()
			: base()
		{
			InitializeComponent();
		}

		private void btnFilePath_Click(object sender, RoutedEventArgs e)
		{
			if (txtFilePath.Text != string.Empty && Directory.Exists(txtFilePath.Text))
			{
				_folderName = txtFilePath.Text;
                BuildTreemap();
            }
			else
				txtFilePath.Text = string.Empty;
		}

		private void chkOrthographic_Click(object sender, RoutedEventArgs e)
		{
			if (chkOrthographic.IsChecked == true)
			{
				VP0.Camera = _oc;
			}
			else
			{
				VP0.Camera = _pc;
			}
			VP0.SetCameraPosition();
		}

        private void WindowLoaded(object sender, EventArgs e)
        {
            BuildTreemap();
        }

		private void BuildTreemap()
		{
			_pc.LookDirection = new Vector3D(0, 0, -1);
            _pc.UpDirection = new Vector3D(0, 1, 0);
			_pc.Position = new Point3D(0, 0, 6);
			_pc.FieldOfView = 50;

            _oc.LookDirection = new Vector3D(0, 0, -1);
            _oc.UpDirection = new Vector3D(0, 1, 0);
			_oc.Position = new Point3D(0, 0, 10);
			_oc.Width = 3;

			VP0.Camera = _pc;

			txtFilePath.Text = _folderName;

            Model3D light = ((VP0.Children[0] as ModelVisual3D).Content as Model3DGroup).Children[0];
            (VP0.Children[0] as ModelVisual3D).Content = new Model3DGroup();
            ((VP0.Children[0] as ModelVisual3D).Content as Model3DGroup).Children.Add(light);

			VP0.SetCameraPosition();

			// Add 3-D objects to the collection and add the collection to the Viewport3D.
            Model3DGroup models = (VP0.Children[0] as ModelVisual3D).Content as Model3DGroup;

			if (!Directory.Exists(_folderName))
				return;

			_folderTree = new Datatree();

			InitializeFolderTree(new DirectoryInfo(_folderName), _folderTree);
			_folderTree.Sort();

			InitializeTreemap();

			_btfTreemap = new BackToFrontTreemap(_treemapRoot);

			// Add 3-D objects to the collection and add the collection to the Viewport3D.
			//Build3DTreemap(_treemapRoot, models.Children, 0);
			Build3DTreemapBackToFront(_btfTreemap, models.Children);

			(VP0.Children[0] as ModelVisual3D).Content = models;
		}

		private void InitializeTreemap()
		{
			double width = 9.0f / 12;
			double height = 1;
			_treemapRoot = new TreemapNode(_folderTree.Magnitude, _folderTree.Name, new Rect(-width / 2, -height / 2, width, height));

			RemainingRectangle.Process(_treemapRoot, _folderTree, 0);
		}

		private void Build3DTreemapBackToFront(BackToFrontTreemap btfTreemap, Model3DCollection modelCollection)
		{
			Int32 layerIX = btfTreemap.Layers.Count - 1;

			VP0.CameraDistance = _layerDepth * layerIX * 3.0f;
			VP0.SetCameraPosition();

			byte colorDelta = (byte)((_fileColor.A - 0x1F) / layerIX);
			byte textDelta = (byte)((0xD0 - _textColor.A) / layerIX);
			double z = 0;

			for (; layerIX >= 0; --layerIX)
			{
				foreach (TreemapNode treemapNode in btfTreemap.Layers[layerIX].Nodes)
				{
					TextBlock tb = new TextBlock();
					tb.Text = treemapNode.Name;
					tb.TextAlignment = TextAlignment.Center;
					tb.VerticalAlignment = VerticalAlignment.Center;
					//tb.Background = Brushes.Transparent;
					tb.Foreground = new SolidColorBrush(_textColor);

					Viewbox viewb = new Viewbox();
					viewb.Stretch = Stretch.Uniform;
					viewb.Child = tb;

					Border bdr = new Border();
					bdr.Width = treemapNode.Rect.Width * 800;
					bdr.Height = treemapNode.Rect.Height * 600;
					bdr.Background = treemapNode.Nodes.Count != 0 ? Brushes.Transparent : new SolidColorBrush(_fileColor);
					bdr.BorderBrush = Brushes.Gray;
					bdr.BorderThickness = new Thickness(0.5f);
					bdr.HorizontalAlignment = HorizontalAlignment.Center;
					bdr.VerticalAlignment = VerticalAlignment.Center;
					bdr.Child = viewb;

					//theHiddenCanvas.Children.Add(bdr);

					MeshGeometry3D geom = MeshGenerator.Grid(treemapNode.Rect.Left, -treemapNode.Rect.Top, treemapNode.Rect.Width, treemapNode.Rect.Height, z);
					VisualBrush vb = new VisualBrush(bdr);
					DiffuseMaterial material = new DiffuseMaterial(vb);
					Model3D m = new GeometryModel3D(geom, material);
					modelCollection.Add(m);
				}
				z += _layerDepth;
				_fileColor = Color.FromArgb((byte)(_fileColor.A - colorDelta), _fileColor.R, _fileColor.G, _fileColor.B);
				_textColor = Color.FromArgb((byte)(_textColor.A + textDelta), _textColor.R, _textColor.G, _textColor.B);
			}
		}

		private void Build3DTreemap(TreemapNode treemapNode, Model3DCollection modelCollection, double z)
		{
			TextBlock tb = new TextBlock();
			tb.Text = treemapNode.Name;
			tb.TextAlignment = TextAlignment.Center;
			tb.VerticalAlignment = VerticalAlignment.Center;
			tb.Foreground = new SolidColorBrush(Color.FromArgb(0x7F, 0, 0, 0));

			Viewbox viewb = new Viewbox();
			viewb.Stretch = Stretch.Uniform;
			viewb.Child = tb;

			Border bdr = new Border();
			bdr.Width = treemapNode.Rect.Width * 800;
			bdr.Height = treemapNode.Rect.Height * 600;
			bdr.Background = treemapNode.Nodes.Count != 0 ? Brushes.Transparent : Brushes.MistyRose;
			bdr.BorderBrush = Brushes.Gray;
			bdr.BorderThickness = new Thickness(0.5f);
			bdr.HorizontalAlignment = HorizontalAlignment.Center;
			bdr.VerticalAlignment = VerticalAlignment.Center;
			bdr.Child = viewb;

			theHiddenCanvas.Children.Add(bdr);

			MeshGeometry3D geom = MeshGenerator.Grid(treemapNode.Rect.Left, -treemapNode.Rect.Top, treemapNode.Rect.Width, treemapNode.Rect.Height, z);
			VisualBrush vb = new VisualBrush(bdr);
			DiffuseMaterial material = new DiffuseMaterial(vb);
			Model3D m = new GeometryModel3D(geom, material);
			modelCollection.Add(m);

			foreach (TreemapNode tmn in treemapNode.Nodes)
			{
				Build3DTreemap(tmn, modelCollection, z - 0.5f);
			}
		}

		private static Int64 FileSystemInfoLength(FileSystemInfo fsi)
		{
			if (fsi is FileInfo)
			{
				return (fsi as FileInfo).Length;
			}
			else
			{
				Int64 size = 0;
				DirectoryInfo di = fsi as DirectoryInfo;
				foreach (FileSystemInfo fsis in di.GetFileSystemInfos())
				{
					size += FileSystemInfoLength(fsis);
				}
				return size;
			}
		}

		private void InitializeFolderTree(FileSystemInfo fsi, DatatreeNode datatreeNode)
		{
			Int64 magnitude = FileSystemInfoLength(fsi);
			datatreeNode.Name = fsi.Name;
			datatreeNode.Magnitude = magnitude;
			if (fsi is DirectoryInfo)
			{
				DirectoryInfo di = fsi as DirectoryInfo;
				FileSystemInfo[] fsis = di.GetFileSystemInfos();
				foreach (FileSystemInfo fsi2 in fsis)
				{
					if (FileSystemInfoLength(fsi2) > 1024) // ignore anything too small to be relevant.
					{
						DatatreeNode dtn = datatreeNode.AddChild(magnitude);
						InitializeFolderTree(fsi2, dtn);
					}
				}
			}
		}
	}
}
