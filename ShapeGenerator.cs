using AForge;
using AForge.Math.Geometry;
using DelaunatorSharp;
using SkiaSharp;

namespace RealmStudio
{
    internal class ShapeGenerator
    {
        public static List<SKPoint> GenerateRandomPointSet(int x, int y, int width, int height, int gridSize)
        {
            List<SKPoint> pointSet = GetRandomPoints(x, y, width, height, gridSize);
            return pointSet;
        }

        public static SKPath GetConvexHullPath(List<IPoint> points)
        {
            SKPath convexHullPath = new SKPath();
            List<IntPoint> intPoints = [];

            foreach (IPoint point in points)
            {
                intPoints.Add(new IntPoint((int)point.X, (int)point.Y));
            }

            IConvexHullAlgorithm hullFinder = new GrahamConvexHull();
            List<IntPoint> hull = hullFinder.FindHull(intPoints);

            convexHullPath.MoveTo(new SKPoint(hull[0].X, hull[0].Y));

            for (int i = 1; i < hull.Count; i++)
            {
                convexHullPath.LineTo(new SKPoint(hull[i].X, hull[i].Y));
            }

            convexHullPath.Close();

            return convexHullPath;
        }

        private static List<SKPoint> GetRandomPoints(int x, int y, int width, int height, int gridSize)
        {
            List<SKPoint> mapPoints = [];

            for (int pointx = x; pointx < x + width; pointx += gridSize)
            {
                for (int pointy = y; pointy < y + height; pointy += gridSize)
                {
                    SKPoint p = new((float)(pointx + gridSize * (Random.Shared.NextDouble() - Random.Shared.NextDouble())), (float)(pointy + gridSize * (Random.Shared.NextDouble() - Random.Shared.NextDouble())));
                    mapPoints.Add(p);
                }
            }

            return mapPoints;
        }
    }
}
