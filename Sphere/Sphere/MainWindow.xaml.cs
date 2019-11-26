using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Sphere
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double Epsilon = 5e-3;
        private const double Scale = 0.5;

        public MainWindow()
        {
            InitializeComponent();
            Go();
        }

        private void Go()
        {
            var r = 0.99;
            var R = 1.0;
            var N = 10000;

            DrawPoints(GetPoints(r, R).Take(N));
        }

        private void DrawPoints(IEnumerable<(double x, double y, double z)> pts)
        {
            var points3D = GetPoint3D(pts);

            var content = (this.Space.Children[0] as ModelVisual3D)?.Content as Model3DGroup;

            foreach (var pt in points3D)
                content.Children.Add(GetGeomModel(pt));
        }

        private static GeometryModel3D GetGeomModel(Point3D pt)
        {
            var res = new GeometryModel3D();
            var geom = new MeshGeometry3D();
            geom.Positions.Add(new Point3D(pt.X + Epsilon, pt.Y, pt.Z));
            geom.Positions.Add(new Point3D(pt.X, pt.Y + Epsilon, pt.Z));
            geom.Positions.Add(new Point3D(pt.X, pt.Y, pt.Z + Epsilon));
            geom.Positions.Add(new Point3D(pt.X, pt.Y, pt.Z));

            for (var i = 0; i < geom.Positions.Count; i++)
            for (var j = 0; j < geom.Positions.Count; j++)
            for (var k = 0; k < geom.Positions.Count; k++)
            {
                geom.TriangleIndices.Add(i);
                geom.TriangleIndices.Add(j);
                geom.TriangleIndices.Add(k);
            }

            res.Geometry = geom;

            var mat = new DiffuseMaterial(Brushes.AliceBlue);
            res.Material = mat;

            return res;
        }

        private static Point3DCollection GetPoint3D(IEnumerable<(double x, double y, double z)> pts)
        {
            var res = new Point3DCollection();
            foreach (var (x, y, z) in pts)
                res.Add(new Point3D(x * Scale, y * Scale, z * Scale));

            return res;
        }

        private static IEnumerable<(double x, double y, double z)> GetPoints(double r, double R)
        {
            if(R - r < 1e-5)
                throw new Exception();

            var rd = new Random(DateTime.Now.Millisecond);

            while (true)
            {
                var phi = 2 * Math.PI * rd.NextDouble();

                var cosTheta = 1 - 2 * rd.NextDouble();
                if (cosTheta > 1)
                    cosTheta = 1;
                if (cosTheta < -1)
                    cosTheta = -1;
                var theta = Math.Acos(cosTheta);

                var ro = Math.Pow((R * R * R - r * r * r) * rd.NextDouble() + r * r * r, 1.0 / 3);

                var x = ro * Math.Sin(theta) * Math.Cos(phi);
                var y = ro * Math.Sin(theta) * Math.Sin(phi);
                var z = ro * cosTheta;
                
                yield return (x, y, z);
            }
        }
    }
}
