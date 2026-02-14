/**************************************************************************************************************************
* Copyright 2025, Peter R. Nelson
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
using AForge.Imaging.Filters;
using SkiaSharp;
using System.Diagnostics;
using RealmStudioShapeRenderingLib;

namespace RealmStudioX
{
    internal sealed class RealmGenerationMethods
    {
        internal static void GenerateRandomLandform(RealmStudioMap map, SKRect selectedArea, GeneratedLandformType selectedLandformType)
        {
            SKPoint location = new(map.MapWidth / 2, map.MapHeight / 2);
            SKSize size = new(map.MapWidth / 2, map.MapHeight / 2);

            if (!selectedArea.IsEmpty)
            {
                location = new SKPoint(selectedArea.MidX, selectedArea.MidY);
                size = selectedArea.Size;
            }

            switch (selectedLandformType)
            {
                case GeneratedLandformType.Archipelago:
                    {
                        if (selectedArea.IsEmpty)
                        {
                            size = new SKSize(map.MapWidth * 0.8F, map.MapHeight * 0.8F);
                        }

                        CreateLandformFromGeneratedPaths(map, location, size, selectedLandformType);
                    }
                    break;
                case GeneratedLandformType.Atoll:
                    {
                        CreateLandformFromGeneratedPaths(map, location, size, selectedLandformType);
                    }
                    break;
                case GeneratedLandformType.Continent:
                    {
                        if (selectedArea.IsEmpty)
                        {
                            size = new SKSize(map.MapWidth * 0.8F, map.MapHeight * 0.8F);
                        }

                        CreateLandformFromGeneratedPaths(map, location, size, GeneratedLandformType.Island);

                        if (selectedArea.IsEmpty)
                        {
                            size = new SKSize(map.MapWidth - 4, map.MapHeight - 4);
                        }

                        CreateLandformFromGeneratedPaths(map, location, size, GeneratedLandformType.Archipelago);
                    }
                    break;
                case GeneratedLandformType.Island:
                    {
                        CreateLandformFromGeneratedPaths(map, location, size, selectedLandformType);
                    }
                    break;
                case GeneratedLandformType.Region:
                    {
                        if (selectedArea.IsEmpty)
                        {
                            size = new SKSize(map.MapWidth - 4, map.MapHeight - 4);
                        }

                        CreateLandformFromGeneratedPaths(map, location, size, selectedLandformType);
                    }
                    break;
                case GeneratedLandformType.World:
                    {
                        GenerateWorldLandforms(map, new SKSizeI(map.MapWidth, map.MapHeight));
                    }
                    break;
            }

            //LandformManager.MergeLandforms(map);
        }

        internal static void GenerateWorldLandforms(RealmStudioMap map, SKSizeI size)
        {
            // TODO: this is a hack to avoid passing the main form around
            // and avoid major refactoring of the landform generation methods
            // for now
            RealmStudioMainForm? mainForm = UtilityMethods.GetMainForm();

            if (mainForm == null) return;

            LoadingStatusForm progressForm = new();
            progressForm.Location = new Point(((mainForm.Location.X + mainForm.Width) / 2) - (progressForm.Width / 2), ((mainForm.Location.Y + mainForm.Height) / 2) - (progressForm.Height / 2));

            progressForm.Show();
            progressForm.Hide();

            progressForm.SetStatusText("Generating world landforms...");
            int progressPercentage = 0;

            progressForm.Show();

            bool hasNorthernIcecap = Random.Shared.Next(0, 2) == 1;
            bool hasSouthernIcecap = Random.Shared.Next(0, 2) == 1;

            SKSizeI northernIcecapSize = SKSizeI.Empty;
            SKSizeI southernIcecapSize = SKSizeI.Empty;

            // if the realm has a northern ice cap, there is a 75% chance it has a southern ice cap also
            if (hasNorthernIcecap && !hasSouthernIcecap)
            {
                hasSouthernIcecap = Random.Shared.NextDouble() > 0.25;
            }

            // if the realm has a southern ice cap, there is a 75% chance it has a northern ice cap also
            if (hasSouthernIcecap && !hasNorthernIcecap)
            {
                hasNorthernIcecap = Random.Shared.NextDouble() > 0.25;
            }

            if (hasNorthernIcecap)
            {
                progressPercentage += 2;
                progressForm.SetStatusPercentage(progressPercentage);
                progressForm.SetStatusText("Generating northern icecap");

                northernIcecapSize.Width = Random.Shared.Next((int)(map.MapWidth / 4.0), map.MapWidth - 8);
                northernIcecapSize.Height = Random.Shared.Next((int)(map.MapHeight / 8.0), (int)(map.MapWidth / 4.0));

                SKPoint northernIcecapLocation = new(map.MapWidth / 2, 4);

                CreateLandformFromGeneratedPaths(map, northernIcecapLocation, northernIcecapSize, GeneratedLandformType.Icecap, false);

                progressPercentage += 3;
                progressForm.SetStatusPercentage(progressPercentage);
                progressForm.SetStatusText("Generated northern icecap");

                mainForm.SetStatusText("Generated northern icecap");
            }

            if (hasSouthernIcecap)
            {
                progressPercentage += 2;
                progressForm.SetStatusPercentage(progressPercentage);
                progressForm.SetStatusText("Generating southern icecap");

                southernIcecapSize.Width = Random.Shared.Next((int)(map.MapWidth / 4.0), map.MapWidth - 8);
                southernIcecapSize.Height = Random.Shared.Next((int)(map.MapHeight / 8.0), (int)(map.MapWidth / 4.0));

                SKPoint southernIcecapLocation = new(map.MapWidth / 2, map.MapHeight - southernIcecapSize.Height - 4);

                CreateLandformFromGeneratedPaths(map, southernIcecapLocation, southernIcecapSize, GeneratedLandformType.Icecap, true);

                progressPercentage += 3;
                progressForm.SetStatusPercentage(progressPercentage);
                progressForm.SetStatusText("Generated southern icecap");

                mainForm.SetStatusText("Generated southern icecap");
            }

            int numDivisions = Random.Shared.Next(1, 8);  // max 7 subdivisions of the remaining map area for now

            progressPercentage += 1;
            progressForm.SetStatusPercentage(progressPercentage);
            progressForm.SetStatusText("Creating " + numDivisions + " landforms");

            mainForm.SetStatusText("Creating " + numDivisions + " landforms");

            SKRectI remainingMapRect = new(0, 0, size.Width, size.Height);

            if (hasNorthernIcecap)
            {
                remainingMapRect.Top = northernIcecapSize.Height;
            }

            if (hasSouthernIcecap)
            {
                remainingMapRect.Bottom = map.MapHeight - southernIcecapSize.Height;
            }

            List<SKRectI> continentRects = [];

            if (numDivisions == 1)
            {
                continentRects.Add(remainingMapRect);
            }
            else
            {
                Debug.Assert(remainingMapRect.Top < remainingMapRect.Bottom);

                int minRectSize = 300;

                if (remainingMapRect.Width < remainingMapRect.Height)
                {
                    minRectSize = (remainingMapRect.Width / numDivisions) - Random.Shared.Next(10, 50);
                }
                else if (remainingMapRect.Height < remainingMapRect.Width)
                {
                    minRectSize = (remainingMapRect.Height / numDivisions) - Random.Shared.Next(10, 50);
                }

                List<SKRectI> rects = DrawingMethods.SubdivideRect2(remainingMapRect, numDivisions, minRectSize);

                int tries = 0;
                while (rects.Count < numDivisions && tries < 10)
                {
                    tries++;
                    minRectSize -= 20;

                    minRectSize = Math.Max(50, minRectSize);

                    rects = DrawingMethods.SubdivideRect2(remainingMapRect, numDivisions, minRectSize);
                }

                if (rects.Count == 0)
                {
                    mainForm.SetStatusText("Map rectangle subdivision failed");
                    progressForm.Hide();
                    progressForm.Dispose();

                    return;
                }

                continentRects.AddRange(rects);

                progressPercentage += 1;
                progressForm.SetStatusPercentage(progressPercentage);
                progressForm.SetStatusText("Landform rectangles calculated");

                mainForm.SetStatusText("Landform rectangles calculated");
            }

            int landformCount;

            int progressStep = (100 - progressPercentage) / continentRects.Count;

            foreach (SKRectI rect in continentRects)
            {
                Debug.Assert(rect.Width > 0 && rect.Height > 0 && rect.Width < int.MaxValue && rect.Height < int.MaxValue);

                bool success = false;
                int tries = 0;

                while (!success && tries < 10)
                {
                    tries++;

                    double landformTypeRandom = Random.Shared.NextDouble();
                    GeneratedLandformType generateLandformType = GeneratedLandformType.Continent;

                    if (landformTypeRandom >= 0.0 && landformTypeRandom < 0.5)
                    {
                        // 50% chance of generating continent
                        generateLandformType = GeneratedLandformType.Continent;
                    }
                    else if (landformTypeRandom >= 0.5 && landformTypeRandom < 0.75)
                    {
                        // 25% chance of generating island
                        generateLandformType = GeneratedLandformType.Island;
                    }
                    else if (landformTypeRandom >= 0.75 && landformTypeRandom < 0.90)
                    {
                        // 15% chance of generating archipelago
                        generateLandformType = GeneratedLandformType.Archipelago;
                    }
                    else
                    {
                        // 10% chance of generating atoll
                        generateLandformType = GeneratedLandformType.Atoll;
                    }

                    progressForm.SetStatusText("Generating " + generateLandformType.ToString());

                    mainForm.SetStatusText("Generating landform " + generateLandformType.ToString());

                    CreateLandformFromGeneratedPaths(map, new SKPoint(rect.MidX, rect.MidY), new SKSize(rect.Width, rect.Height), generateLandformType, false);

                    //landformCount = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER).MapLayerComponents.Count;
                    //progressForm.SetStatusPercentage(landformCount * progressStep);
                }
            }

            progressForm.Hide();
            progressForm.Dispose();

        }

        internal static void CreateLandformFromGeneratedPaths(RealmStudioMap map, SKPoint location, SKSize size, GeneratedLandformType selectedLandformType, bool flipVertical = false)
        {
            //ArgumentNullException.ThrowIfNull(LandformManager.LandformMediator);

            const int minimumGeneratedLandformContourPathPoints = 10;


            List<SKPath> generatedLandformPaths = GenerateRandomLandformPaths(location, size, selectedLandformType, flipVertical);

            /*
            MapLayer landformLayer = MapBuilder.GetMapLayerByIndex(map, MapBuilder.LANDFORMLAYER);

            foreach (SKPath path in generatedLandformPaths)
            {
                if (!path.IsEmpty)
                {
                    Landform? newLandform = (Landform?)LandformManager.Create();

                    if (newLandform != null)
                    {
                        newLandform.DrawPath = new(path);
                        LandformManager.CreateAllPathsFromDrawnPath(map, newLandform);

                        if (newLandform.ContourPath != null && newLandform.ContourPath.PointCount > minimumGeneratedLandformContourPathPoints)
                        {
                            newLandform.ContourPath.GetBounds(out SKRect boundsRect);
                            newLandform.X = (int)location.X;
                            newLandform.Y = (int)location.Y;
                            newLandform.Width = (int)boundsRect.Width;
                            newLandform.Height = (int)boundsRect.Height;

                            landformLayer.MapLayerComponents.Add(newLandform);
                        }
                    }
                }
            }
            */
        }

        internal static List<SKPath> GenerateRandomLandformPaths(SKPoint location, SKSize size, GeneratedLandformType selectedLandformType, bool flipVertical = false)
        {
            List<SKPath> generatedLandformPaths = [];

            Bitmap? landformBitmap = ShapeGenerator.GetNoiseGeneratedLandformShape((int)size.Width, (int)size.Height, selectedLandformType, flipVertical);

            if (landformBitmap == null) { return generatedLandformPaths; }

            // fill any holes in the bitmap
            Bitmap filledBitmap = DrawingMethods.FillHoles(landformBitmap);

            // extract all blobs over set size - arbitrarily set to 1/16 the input size
            List<Bitmap> generatedBitmaps = DrawingMethods.ExtractBlobs(filledBitmap, new Size((int)(size.Width / 16), (int)(size.Height / 16)));

            for (int i = 0; i < generatedBitmaps.Count; i++)
            {
                Bitmap landBitmap = generatedBitmaps[i];

                // the mean filter smooths out pixels that differ a lot in color from their neighbors;
                // when applied to a monochrome image like the generated landform bitmaps, the filter
                // will average out black or white pixels that are surrounded (or mostly surrounded)
                // by pixels of the opposite color. When the bitmap colors are then flattened back to
                // monochrome (black and white), this has the effect of removing "spikes" from around the
                // edges of the landform (and other anomalies) that cause the Moore neighborhood algorithm
                // to fail, causing the landform boundaries not to be computed correctly
                Mean meanFilter = new();
                meanFilter.ApplyInPlace(landBitmap);
                meanFilter.ApplyInPlace(landBitmap);

                DrawingMethods.FlattenBitmapColors(ref landBitmap);

                // fill any holes in the bitmap
                Bitmap filledLandBitmap = DrawingMethods.FillHoles(landBitmap);

                using Graphics g = Graphics.FromImage(filledLandBitmap);
                using Pen p = new(Color.White, 3);

                g.DrawLine(p, new Point(2, 2), new Point(filledLandBitmap.Width - 2, 2));
                g.DrawLine(p, new Point(2, filledLandBitmap.Height - 2), new Point(filledLandBitmap.Width - 2, filledLandBitmap.Height - 2));
                g.DrawLine(p, new Point(2, 2), new Point(2, filledLandBitmap.Height - 2));
                g.DrawLine(p, new Point(filledLandBitmap.Width - 2, 2), new Point(filledLandBitmap.Width - 2, filledLandBitmap.Height - 2));

                // run Moore meighborhood algorithm to get the perimeter path
                List<SKPoint> contourPoints = DrawingMethods.GetBitmapContourPoints(filledLandBitmap);

                SKPath landformPath = new();
                if (contourPoints.Count > 4)   // require at least 4 points in the contour to be added as a landform
                {
                    // the Moore-Neighbor algorithm sets the first (0th) pixel in the list of contour points to
                    // an empty pixel, so remove it before constructing the path from the contour points
                    contourPoints.RemoveAt(0);

                    landformPath.MoveTo(contourPoints[0]);

                    for (int j = 1; j < contourPoints.Count; j++)
                    {
                        landformPath.LineTo(contourPoints[j]);
                    }

                    if (landformPath[0] != landformPath[contourPoints.Count - 1])
                    {
                        landformPath.Close();
                    }

                    if (selectedLandformType != GeneratedLandformType.Icecap)
                    {
                        landformPath.Transform(SKMatrix.CreateTranslation(location.X - (size.Width / 2.0F), location.Y - (size.Height / 2.0F)));
                    }
                    else
                    {
                        landformPath.Transform(SKMatrix.CreateTranslation(location.X - (size.Width / 2.0F), location.Y));
                    }

                    generatedLandformPaths.Add(new(landformPath));
                }
            }

            return generatedLandformPaths;
        }
    }
}
