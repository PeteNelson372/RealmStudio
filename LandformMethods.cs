using SkiaSharp;

namespace RealmStudio
{
    internal class LandformMethods
    {
        public static int LandformBrushSize { get; set; } = 64;

        public static SKPath LandformErasePath { get; set; } = new SKPath();

        public static Color LandformOutlineColor { get; set; } = ColorTranslator.FromHtml("#3D3728");
        public static int LandformOutlineWidth { get; set; } = 2;
        public static GradientDirectionEnum ShorelineStyle { get; set; } = GradientDirectionEnum.None;
        public static Color CoastlineColor { get; set; } = ColorTranslator.FromHtml("#BB9CC3B7");
        public static int CoastlineEffectDistance { get; set; } = 16;
        public static string CoastlineStyleName { get; set; } = "Dash Pattern";
        public static string? CoastlineHatchPattern { get; set; } = string.Empty;
        public static int CoastlineHatchOpacity { get; set; } = 0;
        public static int CoastlineHatchScale { get; set; } = 0;
        public static string? CoastlineHatchBlendMode { get; set; } = string.Empty;

        internal static void CreateInnerAndOuterPaths(RealmStudioMap map, Landform landform)
        {
            if (map == null || landform == null) return;

            landform.ContourPath = DrawingMethods.GetContourPathFromPath(landform.DrawPath,
                map.MapWidth, map.MapHeight, out List<SKPoint> contourPoints);

            landform.ContourPoints = contourPoints;

            int pathDistance = CoastlineEffectDistance / 8;

            landform.InnerPath1 = DrawingMethods.GetInnerOrOuterPath(contourPoints, pathDistance, ParallelEnum.Below);
            landform.InnerPath2 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 2 * pathDistance, ParallelEnum.Below);
            landform.InnerPath3 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 3 * pathDistance, ParallelEnum.Below);
            landform.InnerPath4 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 4 * pathDistance, ParallelEnum.Below);
            landform.InnerPath5 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 5 * pathDistance, ParallelEnum.Below);
            landform.InnerPath6 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 6 * pathDistance, ParallelEnum.Below);
            landform.InnerPath7 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 7 * pathDistance, ParallelEnum.Below);
            landform.InnerPath8 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 8 * pathDistance, ParallelEnum.Below);

            landform.OuterPath1 = DrawingMethods.GetInnerOrOuterPath(contourPoints, pathDistance, ParallelEnum.Above);
            landform.OuterPath2 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 2 * pathDistance, ParallelEnum.Above);
            landform.OuterPath3 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 3 * pathDistance, ParallelEnum.Above);
            landform.OuterPath4 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 4 * pathDistance, ParallelEnum.Above);
            landform.OuterPath5 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 5 * pathDistance, ParallelEnum.Above);
            landform.OuterPath6 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 6 * pathDistance, ParallelEnum.Above);
            landform.OuterPath7 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 7 * pathDistance, ParallelEnum.Above);
            landform.OuterPath8 = DrawingMethods.GetInnerOrOuterPath(contourPoints, 8 * pathDistance, ParallelEnum.Above);
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

                        Task.Run(() => CreateInnerAndOuterPaths(map, l));
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
                            CreateInnerAndOuterPaths(map, landform_i);

                            landformLayer.MapLayerComponents[i] = landform_i;

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

        internal static bool MergeLandformPaths(SKPath landformPath2, ref SKPath landformPath1)
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
