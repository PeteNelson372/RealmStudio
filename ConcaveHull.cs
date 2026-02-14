/**************************************************************************************************************************
* Copyright 2024, Peter R. Nelson
*
* This file is part of the RealmStudio application. The RealmStudio application is intended
* for creating fantasy maps for gaming and world building.
*
* RealmStudio is free software: you can redistribute it and/or modify it under the terms
* of the GNU General Public License as published by the Free Software Foundation,
* either version 3 of the License, or (at your option) any later version.
*
* This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
* without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
* See the GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License along with this program.
* The text of the GNU General Public License (GPL) is found in the LICENSE.txt file.
* If the LICENSE.txt file is not present or the text of the GNU GPL is not present in the LICENSE.txt file,
* see https://www.gnu.org/licenses/.
*
* For questions about the RealmStudio application or about licensing, please email
* support@brookmonte.com
*
***************************************************************************************************************************/
using SkiaSharp;

namespace RealmStudioX
{
    // <summary>
    // Represents the concave hull of a set of points in a 2D plane.
    // The concave hull is a non-convex polygon that encloses all the points.
    // </summary>
    public class ConcaveHull(List<SKPoint> inputPoints)
    {
        private readonly List<SKPoint> points = inputPoints;

        // <summary>
        // Computes the concave hull of a set of points using the k-nearest neighbors algorithm.
        //
        // Parameters:
        // - points: The set of points for which to compute the concave hull.
        // - k: The number of nearest neighbors to consider for each point.
        //
        // Returns:
        // - A list of points representing the concave hull.
        // </summary>
        public static List<SKPoint> ComputeConcaveHull(List<SKPoint> points)
        {
            // Check if the number of points is less than 3.
            if (points.Count < 3)
            {
                throw new ArgumentException("At least 3 points are required to compute the concave hull.");
            }

            // Sort the points based on their x-coordinate.
            points.Sort((p1, p2) => p1.X.CompareTo(p2.X));

            // Initialize the concave hull with the first two points.
            List<SKPoint> concaveHull = [points[0], points[1]];

            // Compute the remaining points of the concave hull.
            for (int i = 2; i < points.Count; i++)
            {
                concaveHull.Add(points[i]);

                // Remove any points that are not part of the concave hull.
                while (concaveHull.Count >= 3 && !IsConcaveHull(concaveHull))
                {
                    concaveHull.RemoveAt(concaveHull.Count - 2);
                }
            }

            return concaveHull;
        }

        // <summary>
        // Checks if a point is part of the concave hull formed by a set of points.
        //
        // Parameters:
        // - concaveHull: The set of points representing the concave hull.
        //
        // Returns:
        // - True if the point is part of the concave hull, false otherwise.
        // </summary>
        private static bool IsConcaveHull(List<SKPoint> concaveHull)
        {
            int n = concaveHull.Count;

            // Compute the cross product of the last three points.
            double crossProduct = CrossProduct(concaveHull[n - 3], concaveHull[n - 2], concaveHull[n - 1]);

            // If the cross product is positive, the point is part of the concave hull.
            return crossProduct > 0;
        }

        // <summary>
        // Computes the cross product of three points.
        //
        // Parameters:
        // - p1: The first point.
        // - p2: The second point.
        // - p3: The third point.
        //
        // Returns:
        // - The cross product of the three points.
        // </summary>
        private static double CrossProduct(SKPoint p1, SKPoint p2, SKPoint p3)
        {
            return (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
        }
    }
}

// Example usage of the ConcaveHull class.
/*

public class Program
{
    public static void Main()
    {
        // Create a list of points.
        List<Point> points = new List<Point>
        {
            new Point(1, 1),
            new Point(2, 3),
            new Point(3, 2),
            new Point(4, 4),
            new Point(5, 1),
            new Point(6, 3),
            new Point(7, 2),
            new Point(8, 4),
            new Point(9, 1),
            new Point(10, 3)
        };

        // Compute the concave hull with k = 3.
        List<Point> concaveHull = ConcaveHull.ComputeConcaveHull(points, 3);

        // Print the concave hull points.
        Console.WriteLine("Concave Hull Points:");
        foreach (Point point in concaveHull)
        {
            Console.WriteLine($"({point.X}, {point.Y})");
        }
    }
}

*/
