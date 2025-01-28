using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Media3D;

using System.Diagnostics;

namespace Arthur
{
	public class MeshGenerator
	{
		public static MeshGeometry3D Tetrahedron()
		{
			return Tetrahedron(-1, false);
		}

		public static MeshGeometry3D Tetrahedron(Int32 granularity, bool genTextCoords)
		{
			MeshGeometry3D meshGeometry3D = new MeshGeometry3D();

			// Geometry.
			AddPointInSphericalCoordinates(meshGeometry3D, 0.5, 0, 0);
			AddPointInSphericalCoordinates(meshGeometry3D, 0.5, 0, 2 * Math.PI / 3);
			AddPointInSphericalCoordinates(meshGeometry3D, 0.5, 2 * Math.PI / 3, 2 * Math.PI / 3);
			AddPointInSphericalCoordinates(meshGeometry3D, 0.5, 4 * Math.PI / 3, 2 * Math.PI / 3);

			Int32[] triangleIndices = { 0, 2, 1, 0, 3, 2, 0, 1, 3, 1, 2, 3 };

			AddTriangleArray0Based(meshGeometry3D, triangleIndices);

			return meshGeometry3D;
		}

		public static MeshGeometry3D Grid(Int32 granularity, double z, bool genTextCoords)
		{
			Debug.Assert(granularity > 0);

			MeshGeometry3D meshGeometry3D = new MeshGeometry3D();

			double delta = 1.0 / granularity;

			// Geometry.
			long ySteps = granularity;
			for (double y = -0.5; ySteps-- != 0; y += delta)
			{
				long xSteps = granularity;
				for (double x = -0.5; xSteps-- != 0; x += delta)
				{
					double x2 = (xSteps != 0) ? x + delta : 0.5;
					double y2 = (ySteps != 0) ? y + delta : 0.5;
					Int32 ixP0 = AddPoint(meshGeometry3D, x, y, z);
					Int32 ixP1 = AddPoint(meshGeometry3D, x, y2, z);
					Int32 ixP2 = AddPoint(meshGeometry3D, x2, y2, z);
					Int32 ixP3 = AddPoint(meshGeometry3D, x2, y, z);

					if (genTextCoords)
					{
						double u = 0.5 + x;
						double v = 0.5 - y;
						double u2 = (xSteps != 0) ? 0.5 + x + delta : 1;
						double v2 = (ySteps != 0) ? 0.5 - y - delta : 0;
						AddTexCoord(meshGeometry3D, u, v);
						AddTexCoord(meshGeometry3D, u, v2);
						AddTexCoord(meshGeometry3D, u2, v2);
						AddTexCoord(meshGeometry3D, u2, v);
					}
					AddQuad0Based(meshGeometry3D, ixP0, ixP3, ixP2, ixP1);
				}
			}
			return meshGeometry3D;
		}

		public static MeshGeometry3D Grid(double left, double top, double width, double height, double z)
		{
			MeshGeometry3D meshGeometry3D = new MeshGeometry3D();

			AddPoint(meshGeometry3D, left, top - height, z);
			AddPoint(meshGeometry3D, left, top, z);
			AddPoint(meshGeometry3D, left + width, top, z);
			AddPoint(meshGeometry3D, left + width, top - height, z);
			AddTexCoord(meshGeometry3D, 0, 1);
			AddTexCoord(meshGeometry3D, 0, 0);
			AddTexCoord(meshGeometry3D, 1, 0);
			AddTexCoord(meshGeometry3D, 1, 1);
			AddQuad0Based(meshGeometry3D, 0, 3, 2, 1);

			return meshGeometry3D;
		}

		public static Model3DGroup Cube(DiffuseMaterial DiffuseMaterial)
		{
			return Cube(DiffuseMaterial, 1);
		}
		public static Model3DGroup Cube(DiffuseMaterial DiffuseMaterial, Int32 granularity)
		{
			return Cube(DiffuseMaterial, granularity, false);
		}
		public static Model3DGroup Cube(DiffuseMaterial DiffuseMaterial, Int32 granularity, bool genTextCoords)
		{
			return Cube(DiffuseMaterial, granularity, genTextCoords, true, true);
		}
		public static Model3DGroup Cube(DiffuseMaterial DiffuseMaterial, Int32 granularity, bool genTextCoords, bool bottom, bool top)
		{
			Debug.Assert(granularity > 0);

			Model3DGroup group = new Model3DGroup();

			Model3D model = new GeometryModel3D(Grid(granularity, 0.5, genTextCoords), DiffuseMaterial);
			group.Children.Add(model);

			model = new GeometryModel3D(Grid(granularity, 0.5, genTextCoords), DiffuseMaterial);
			model.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 90));
			group.Children.Add(model);

			model = new GeometryModel3D(Grid(granularity, 0.5, genTextCoords), DiffuseMaterial);
			model.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 180));
			group.Children.Add(model);

			model = new GeometryModel3D(Grid(granularity, 0.5, genTextCoords), DiffuseMaterial);
			model.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 270));
			group.Children.Add(model);

			if (top)
			{
				model = new GeometryModel3D(Grid(granularity, 0.5, genTextCoords), DiffuseMaterial);
				model.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 270));
				group.Children.Add(model);
			}

			if (bottom)
			{
				model = new GeometryModel3D(Grid(granularity, 0.5, genTextCoords), DiffuseMaterial);
                model.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90));
				group.Children.Add(model);
			}

			return group;
		}

		public static MeshGeometry3D Tube()
		{
			return Tube(10, 10, false);
		}

		public static MeshGeometry3D Tube(Int32 stacks, Int32 slices, bool genTextCoords)
		{
			Debug.Assert(stacks > 1 && slices > 3);

			MeshGeometry3D meshGeometry3D = new MeshGeometry3D();

			if (slices % 2 != 0) ++slices;
			double phi, phiDelta = Math.PI / stacks;
			double theta, thetaDelta = 2 * Math.PI / slices;
			double u, uDelta = 1 / (float)slices;
			double v, vDelta = 1 / (float)stacks;

			// Geometry.
			Int32 phiSteps = stacks;
			for (v = 0, phi = 0; phiSteps-- != 0; phi += phiDelta, v += vDelta)
			{
				Int32 thetaSteps = slices;
				for (u = 0, theta = 0; thetaSteps-- != 0; theta += thetaDelta, u += uDelta)
				{
					Int32 ixP0 = AddPointInSphericalCoordinates(meshGeometry3D, 0.5, theta, phi);
					Int32 ixP1 = AddPointInSphericalCoordinates(meshGeometry3D, 0.5, theta, (phiSteps == 0) ? Math.PI : phi + phiDelta);
					Int32 ixP2 = AddPointInSphericalCoordinates(meshGeometry3D, 0.5, (thetaSteps == 0) ? 0 : theta + thetaDelta, (phiSteps == 0) ? Math.PI : phi + phiDelta);
					Int32 ixP3 = AddPointInSphericalCoordinates(meshGeometry3D, 0.5, (thetaSteps == 0) ? 0 : theta + thetaDelta, phi);

					if (genTextCoords)
					{
						AddTexCoord(meshGeometry3D, u, v);
						AddTexCoord(meshGeometry3D, u, (phiSteps == 0) ? 1 : v + vDelta);
						AddTexCoord(meshGeometry3D, (thetaSteps == 0) ? 1 : u + uDelta, (phiSteps == 0) ? 1 : v + vDelta);
						AddTexCoord(meshGeometry3D, (thetaSteps == 0) ? 1 : u + uDelta, v);
					}

					AddQuad0Based(meshGeometry3D, ixP0, ixP1, ixP2, ixP3);
				}
			}

			return meshGeometry3D;
		}

		public static MeshGeometry3D Sphere()
		{
			return Sphere(10, 10, false);
		}

		public static MeshGeometry3D Sphere(Int32 stacks, Int32 slices, bool genTextCoords)
		{
			Debug.Assert(stacks > 1 && slices > 3);

			MeshGeometry3D meshGeometry3D = new MeshGeometry3D();

			if (slices % 2 != 0) ++slices;
			double phi, phiDelta = Math.PI / stacks;
			double theta, thetaDelta = 2 * Math.PI / slices;
			double u, uDelta = 1 / (float)slices;
			double v, vDelta = 1 / (float)stacks;

			// Geometry.
			Int32 phiSteps = stacks;
			for (v = 0, phi = 0; phiSteps-- != 0; phi += phiDelta, v += vDelta)
			{
				Int32 thetaSteps = slices;
				for (u = 0, theta = 0; thetaSteps-- != 0; theta += thetaDelta, u += uDelta)
				{
					Int32 ixP0 = AddPointInSphericalCoordinates(meshGeometry3D, 0.5, theta, phi);
					Int32 ixP1 = AddPointInSphericalCoordinates(meshGeometry3D, 0.5, theta, (phiSteps == 0) ? Math.PI : phi + phiDelta);
					Int32 ixP2 = AddPointInSphericalCoordinates(meshGeometry3D, 0.5, (thetaSteps == 0) ? 0 : theta + thetaDelta, (phiSteps == 0) ? Math.PI : phi + phiDelta);
					Int32 ixP3 = AddPointInSphericalCoordinates(meshGeometry3D, 0.5, (thetaSteps == 0) ? 0 : theta + thetaDelta, phi);

					if (genTextCoords)
					{
						AddTexCoord(meshGeometry3D, u, v);
						AddTexCoord(meshGeometry3D, u, (phiSteps == 0) ? 1 : v + vDelta);
						AddTexCoord(meshGeometry3D, (thetaSteps == 0) ? 1 : u + uDelta, (phiSteps == 0) ? 1 : v + vDelta);
						AddTexCoord(meshGeometry3D, (thetaSteps == 0) ? 1 : u + uDelta, v);
					}

					AddQuad0Based(meshGeometry3D, ixP0, ixP1, ixP2, ixP3);
				}
			}

			return meshGeometry3D;
		}

		public static MeshGeometry3D SmoothSphere()
		{
			return SmoothSphere(10, 10, false);
		}

		public static MeshGeometry3D SmoothSphere(Int32 stacks, Int32 slices, bool genTextCoords)
		{
			Debug.Assert(stacks > 1 && slices > 3);

			MeshGeometry3D meshGeometry3D = new MeshGeometry3D();

			if (slices % 2 != 0) ++slices;
			double phi, phiDelta = Math.PI / stacks;
			double theta, thetaDelta = 2 * Math.PI / slices;
			double u, uDelta = 1 / (float)slices;
			double v, vDelta = 1 / (float)stacks;

			// Geometry.
			Int32 phiSteps = stacks;
			AddPointInSphericalCoordinates(meshGeometry3D, 0.5, 0, 0);
			if (genTextCoords)
			{
				AddTexCoord(meshGeometry3D, 0, 1);
			}
			for (v = 0, phi = 0; phiSteps-- != 0; phi += phiDelta, v += vDelta)
			{
				Int32 thetaSteps = slices;
				for (u = 0, theta = 0; thetaSteps-- != 0; theta += thetaDelta, u += uDelta)
				{
					if (theta == 0)
						AddPointInSphericalCoordinates(meshGeometry3D, 0.5, theta, (phiSteps == 0) ? Math.PI : phi + phiDelta);
					if (theta == 0 && genTextCoords)
						AddTexCoord(meshGeometry3D, u, (phiSteps == 0) ? 1 : v + vDelta);

					AddPointInSphericalCoordinates(meshGeometry3D, 0.5, (thetaSteps == 0) ? 0 : theta + thetaDelta, (phiSteps == 0) ? Math.PI : phi + phiDelta);

					if (phi == 0)
						AddPointInSphericalCoordinates(meshGeometry3D, 0.5, (thetaSteps == 0) ? 0 : theta + thetaDelta, phi);

					if (genTextCoords)
						AddTexCoord(meshGeometry3D, (thetaSteps == 0) ? 1 : u + uDelta, (phiSteps == 0) ? 1 : v + vDelta);
					if (phi == 0 && genTextCoords)
						AddTexCoord(meshGeometry3D, (thetaSteps == 0) ? 1 : u + uDelta, v);

					AddQuad0Based(meshGeometry3D,
						slices * (stacks - phiSteps) + (slices - thetaSteps),
						slices * (stacks - phiSteps + 1) + (slices - thetaSteps),
						slices * (stacks - phiSteps + 1) + (slices - thetaSteps + 1),
						slices * (stacks - phiSteps) + (slices - thetaSteps + 1)
						);
				}
			}

			return meshGeometry3D;
		}

		public static void AddPointArray(MeshGeometry3D meshGeometry3D, double[] pointCoords)
		{
			AddPointArray(meshGeometry3D, pointCoords, 3);
		}

		public static void AddPointArray(MeshGeometry3D meshGeometry3D, double[] pointCoords, byte numCoords)
		{
			Debug.Assert(pointCoords.Length % numCoords == 0);

			for (Int32 ix = 0; ix < pointCoords.Length; ix += numCoords)
			{
				meshGeometry3D.Positions.Add(new Point3D(pointCoords[ix], pointCoords[ix + 1], pointCoords[ix + 2]));
			}
		}

		public static Int32 AddPointInSphericalCoordinates(MeshGeometry3D meshGeometry3D, double rho, double theta, double phi)
		{
			double x = rho * -Math.Sin(theta) * Math.Sin(phi);
			double y = rho * Math.Cos(phi);
			double z = rho * -Math.Cos(theta) * Math.Sin(phi);

			return AddPoint(meshGeometry3D, x, y, z);
		}

		public static Int32 AddPoint(MeshGeometry3D meshGeometry3D, double x, double y, double z)
		{
			meshGeometry3D.Positions.Add(new Point3D(x, y, z));
			return meshGeometry3D.Positions.Count - 1;
		}

		public static void AddTriangleArray0Based(MeshGeometry3D meshGeometry3D, Int32[] triangleIndices)
		{
			Debug.Assert(triangleIndices.Length % 3 == 0);

			for (Int32 ix = 0; ix < triangleIndices.Length; ix += 3)
			{
				AddTriangle0Based(meshGeometry3D, triangleIndices[ix], triangleIndices[ix + 1], triangleIndices[ix + 2]);
			}
		}

		public static void AddTriangle0Based(
			MeshGeometry3D meshGeometry3D,
			Int32 ixP0, Int32 ixP1, Int32 ixP2)
		{
			meshGeometry3D.TriangleIndices.Add(ixP0);
			meshGeometry3D.TriangleIndices.Add(ixP1);
			meshGeometry3D.TriangleIndices.Add(ixP2);
		}

		public static void AddTriangle1Based(
			MeshGeometry3D meshGeometry3D,
			Int32 ixP0, Int32 ixP1, Int32 ixP2)
		{
			meshGeometry3D.TriangleIndices.Add(ixP0-1);
			meshGeometry3D.TriangleIndices.Add(ixP1-1);
			meshGeometry3D.TriangleIndices.Add(ixP2-1);
		}

		public static void AddQuadArray0Based(MeshGeometry3D meshGeometry3D, Int32[] quadIndices)
		{
			Debug.Assert(quadIndices.Length % 4 == 0);

			for (Int32 ix = 0; ix < quadIndices.Length; ix += 4)
			{
				AddQuad0Based(meshGeometry3D, quadIndices[ix], quadIndices[ix+1], quadIndices[ix+2], quadIndices[ix+3]);
			}
		}

		public static void AddQuadArray1Based(MeshGeometry3D meshGeometry3D, Int32[] quadIndices)
		{
			Debug.Assert(quadIndices.Length % 4 == 0);

			for (Int32 ix = 0; ix < quadIndices.Length; ix += 4)
			{
				AddQuad1Based(meshGeometry3D, quadIndices[ix], quadIndices[ix+1], quadIndices[ix+2], quadIndices[ix+3]);
			}
		}

		public static void AddQuad0Based(
			MeshGeometry3D meshGeometry3D,
			Int32 ixP0, Int32 ixP1, Int32 ixP2, Int32 ixP3)
		{
			meshGeometry3D.TriangleIndices.Add(ixP0);
			meshGeometry3D.TriangleIndices.Add(ixP1);
			meshGeometry3D.TriangleIndices.Add(ixP2);
			meshGeometry3D.TriangleIndices.Add(ixP0);
			meshGeometry3D.TriangleIndices.Add(ixP2);
			meshGeometry3D.TriangleIndices.Add(ixP3);
		}

		public static void AddQuad1Based(
			MeshGeometry3D meshGeometry3D,
			Int32 ixP0, Int32 ixP1, Int32 ixP2, Int32 ixP3)
		{
			meshGeometry3D.TriangleIndices.Add(ixP0 - 1);
			meshGeometry3D.TriangleIndices.Add(ixP1 - 1);
			meshGeometry3D.TriangleIndices.Add(ixP2 - 1);
			meshGeometry3D.TriangleIndices.Add(ixP0 - 1);
			meshGeometry3D.TriangleIndices.Add(ixP2 - 1);
			meshGeometry3D.TriangleIndices.Add(ixP3 - 1);
		}

		public static void AddTexCoordArray(MeshGeometry3D meshGeometry3D, double[] texCoords)
		{
			Debug.Assert(texCoords.Length % 2 == 0);

			for (Int32 ix = 0; ix < texCoords.Length; ix += 2)
			{
				AddTexCoord(meshGeometry3D, texCoords[ix], texCoords[ix + 1]);
			}
		}

		public static Int32 AddTexCoord(MeshGeometry3D meshGeometry3D, double u, double v)
		{
			meshGeometry3D.TextureCoordinates.Add(new System.Windows.Point(u, v));
			return meshGeometry3D.TextureCoordinates.Count - 1;
		}
	}
}
