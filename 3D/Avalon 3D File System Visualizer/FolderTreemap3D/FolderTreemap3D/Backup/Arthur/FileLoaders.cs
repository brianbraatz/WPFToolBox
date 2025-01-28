using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Media3D;
using System.IO;

namespace Arthur
{
	class ObjFileLoader
	{
		public static MeshGeometry3D Load(string filespec)
		{
			MeshGeometry3D meshGeometry3D = new MeshGeometry3D();

			using (StreamReader tr = File.OpenText(filespec))
			{
				while (tr.Peek() != -1)
				{
					string textLine;
					textLine = tr.ReadLine();
					string[] parms = textLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

					switch (parms[0])				// See what the first parameter on the line is.
					{
						case "#":					// a comment.
							break;
						case "v":
							MeshGenerator.AddPoint(
								meshGeometry3D,
								double.Parse(parms[1]),
								double.Parse(parms[2]),
								double.Parse(parms[3]));
							break;
						case "vn":
							break;
						case "vt":
							break;
						case "g":					// group.
							break;
						case "f":					// face.
							// Each set of indices is of the form v, v/vt, v/vt/vn or v//vn.
							string[] index1 = parms[1].Split(new char[] { '/' });
							string[] index2 = parms[2].Split(new char[] { '/' });
							string[] index3 = parms[3].Split(new char[] { '/' });
							if (parms.Length == 5)
							{
								string[] index4 = parms[4].Split(new char[] { '/' });
								MeshGenerator.AddQuad1Based(
									meshGeometry3D,
									Int32.Parse(index1[0]),
									Int32.Parse(index2[0]),
									Int32.Parse(index3[0]),
									Int32.Parse(index4[0]));
							}
							else
							{
								MeshGenerator.AddTriangle1Based(
									meshGeometry3D,
									Int32.Parse(index1[0]),
									Int32.Parse(index2[0]),
									Int32.Parse(index3[0]));
							}
							break;
						case "usemtl":					// 'usemtl' - use a material.
							break;
						case "mtllib":					// 'mtllib' - material library.
							break;
						default:
							break;
					}
				}
			}
			return meshGeometry3D;
		}
	}
}
