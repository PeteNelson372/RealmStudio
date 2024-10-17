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

namespace RealmStudio
{
    internal class LandformMethods
    {
        public static SKPath LandformErasePath { get; set; } = new SKPath();

        public static int LandformBrushSize { get; set; } = 64;
        public static int LandformEraserSize { get; set; } = 64;
        public static int LandformColorBrushSize { get; set; } = 20;
        public static int LandformColorEraserBrushSize { get; set; } = 20;

        internal static void CreateAllPathsFromDrawnPath(RealmStudioMap map, Landform landform)
        {
            if (map == null || landform == null) return;

            landform.ContourPath = DrawingMethods.GetContourPathFromPath(landform.DrawPath,
                map.MapWidth, map.MapHeight, out List<SKPoint> contourPoints);

            landform.ContourPoints = contourPoints;

            int pathDistance = landform.CoastlineEffectDistance / 8;

            Task cpt1 = Task.Run(() => landform.InnerPath1 = DrawingMethods.GetInnerOrOuterPath(contourPoints, pathDistance, ParallelEnum.Below));
            Task cpt2 = Task.Run(() => landform.InnerPath2 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 2 * pathDistance, ParallelEnum.Below));
            Task cpt3 = Task.Run(() => landform.InnerPath3 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 3 * pathDistance, ParallelEnum.Below));
            Task cpt4 = Task.Run(() => landform.InnerPath4 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 4 * pathDistance, ParallelEnum.Below));
            Task cpt5 = Task.Run(() => landform.InnerPath5 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 5 * pathDistance, ParallelEnum.Below));
            Task cpt6 = Task.Run(() => landform.InnerPath6 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 6 * pathDistance, ParallelEnum.Below));
            Task cpt7 = Task.Run(() => landform.InnerPath7 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 7 * pathDistance, ParallelEnum.Below));
            Task cpt8 = Task.Run(() => landform.InnerPath8 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 8 * pathDistance, ParallelEnum.Below));

            Task cpt9 = Task.Run(() => landform.OuterPath1 = DrawingMethods.GetInnerOrOuterPath(contourPoints, pathDistance, ParallelEnum.Above));
            Task cpt10 = Task.Run(() => landform.OuterPath2 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 2 * pathDistance, ParallelEnum.Above));
            Task cpt11 = Task.Run(() => landform.OuterPath3 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 3 * pathDistance, ParallelEnum.Above));
            Task cpt12 = Task.Run(() => landform.OuterPath4 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 4 * pathDistance, ParallelEnum.Above));
            Task cpt13 = Task.Run(() => landform.OuterPath5 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 5 * pathDistance, ParallelEnum.Above));
            Task cpt14 = Task.Run(() => landform.OuterPath6 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 6 * pathDistance, ParallelEnum.Above));
            Task cpt15 = Task.Run(() => landform.OuterPath7 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 7 * pathDistance, ParallelEnum.Above));
            Task cpt16 = Task.Run(() => landform.OuterPath8 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 8 * pathDistance, ParallelEnum.Above));

            Task.WaitAll(cpt1, cpt2, cpt3, cpt4, cpt4, cpt6, cpt7, cpt8, cpt9, cpt10, cpt11, cpt12, cpt13, cpt14, cpt15, cpt16);

            landform.IsModified = true;
        }

        internal static void CreateInnerAndOuterPathsFromContourPoints(RealmStudioMap map, Landform landform)
        {
            if (map == null || landform == null) return;

            if (landform.ContourPoints.Count == 0)
            {
                landform.ContourPoints = [.. landform.ContourPath.Points];
            }

            int pathDistance = landform.CoastlineEffectDistance / 8;

            Task cpt1 = Task.Run(() => landform.InnerPath1 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, pathDistance, ParallelEnum.Below));
            Task cpt2 = Task.Run(() => landform.InnerPath2 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 2 * pathDistance, ParallelEnum.Below));
            Task cpt3 = Task.Run(() => landform.InnerPath3 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 3 * pathDistance, ParallelEnum.Below));
            Task cpt4 = Task.Run(() => landform.InnerPath4 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 4 * pathDistance, ParallelEnum.Below));
            Task cpt5 = Task.Run(() => landform.InnerPath5 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 5 * pathDistance, ParallelEnum.Below));
            Task cpt6 = Task.Run(() => landform.InnerPath6 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 6 * pathDistance, ParallelEnum.Below));
            Task cpt7 = Task.Run(() => landform.InnerPath7 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 7 * pathDistance, ParallelEnum.Below));
            Task cpt8 = Task.Run(() => landform.InnerPath8 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 8 * pathDistance, ParallelEnum.Below));

            Task cpt9 = Task.Run(() => landform.OuterPath1 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, pathDistance, ParallelEnum.Above));
            Task cpt10 = Task.Run(() => landform.OuterPath2 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 2 * pathDistance, ParallelEnum.Above));
            Task cpt11 = Task.Run(() => landform.OuterPath3 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 3 * pathDistance, ParallelEnum.Above));
            Task cpt12 = Task.Run(() => landform.OuterPath4 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 4 * pathDistance, ParallelEnum.Above));
            Task cpt13 = Task.Run(() => landform.OuterPath5 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 5 * pathDistance, ParallelEnum.Above));
            Task cpt14 = Task.Run(() => landform.OuterPath6 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 6 * pathDistance, ParallelEnum.Above));
            Task cpt15 = Task.Run(() => landform.OuterPath7 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 7 * pathDistance, ParallelEnum.Above));
            Task cpt16 = Task.Run(() => landform.OuterPath8 = DrawingMethods.GetInnerOrOuterPath(landform.ContourPoints, 8 * pathDistance, ParallelEnum.Above));

            Task.WaitAll(cpt1, cpt2, cpt3, cpt4, cpt4, cpt6, cpt7, cpt8, cpt9, cpt10, cpt11, cpt12, cpt13, cpt14, cpt15, cpt16);
        }

        internal static void EraseLandForm(RealmStudioMap map)
        {
            if (LandformErasePath.PointCount > 0)
            {
                MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER);

                foreach (Landform l in landformLayer.MapLayerComponents.Cast<Landform>())
                {
                    using SKPath diffPath = l.DrawPath.Op(LandformErasePath, SKPathOp.Difference);

                    if (diffPath != null)
                    {
                        l.DrawPath = new(diffPath);
                        l.IsModified = true;
                        Task.Run(() => CreateAllPathsFromDrawnPath(map, l));
                    }
                }

                LandformErasePath.Reset();
            }
        }

        internal static void MergeLandforms(RealmStudioMap map)
        {
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER);

            List<Guid> mergedLandformGuids = [];

            // merge overlapping landforms
            for (int i = 0; i < landformLayer.MapLayerComponents.Count; i++)
            {
                for (int j = 0; j < landformLayer.MapLayerComponents.Count; j++)
                {
                    if (i != j
                        && !mergedLandformGuids.Contains(((Landform)landformLayer.MapLayerComponents[i]).LandformGuid)
                        && !mergedLandformGuids.Contains(((Landform)landformLayer.MapLayerComponents[j]).LandformGuid))
                    {
                        Landform landform_i = (Landform)landformLayer.MapLayerComponents[i];
                        Landform landform_j = (Landform)landformLayer.MapLayerComponents[j];

                        SKPath landformPath1 = landform_i.ContourPath;
                        SKPath landformPath2 = landform_j.ContourPath;

                        bool pathsMerged = MergeLandformPaths(landformPath2, ref landformPath1);

                        if (pathsMerged)
                        {
                            landform_i.DrawPath = new(landformPath1);
                            CreateAllPathsFromDrawnPath(map, landform_i);

                            landformLayer.MapLayerComponents[i] = landform_i;
                            landform_i.IsModified = true;

                            mergedLandformGuids.Add(landform_j.LandformGuid);
                        }
                    }
                }
            }

            for (int k = landformLayer.MapLayerComponents.Count - 1; k >= 0; k--)
            {
                if (mergedLandformGuids.Contains(((Landform)landformLayer.MapLayerComponents[k]).LandformGuid))
                {
                    landformLayer.MapLayerComponents.RemoveAt(k);
                }
            }
        }

        private static bool MergeLandformPaths(SKPath landformPath2, ref SKPath landformPath1)
        {
            // merge paths from two landforms; if the paths overlap, then landformPath1
            // is modified to include landformPath2 (landformPath1 becomes the union
            // of the two original paths)
            bool pathsMerged = false;

            if (landformPath2.PointCount > 0 && landformPath1.PointCount > 0)
            {
                // get the intersection between the paths
                SKPath intersectionPath = landformPath1.Op(landformPath2, SKPathOp.Intersect);

                // if the intersection path isn't null or empty, then merge the paths
                if (intersectionPath != null && intersectionPath.PointCount > 0)
                {
                    // calculate the union between the paths
                    SKPath unionPath = landformPath1.Op(landformPath2, SKPathOp.Union);

                    if (unionPath != null && unionPath.PointCount > 0)
                    {
                        pathsMerged = true;
                        landformPath1.Dispose();
                        landformPath1 = new SKPath(unionPath)
                        {
                            FillType = SKPathFillType.Winding
                        };

                        unionPath.Dispose();
                    }
                }
            }

            return pathsMerged;
        }
    }
}
