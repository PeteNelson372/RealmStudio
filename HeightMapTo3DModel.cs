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
using SkiaSharp;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace RealmStudio
{
    internal sealed class HeightMapTo3DModel
    {
        public static void WriteObjModelToFile(List<string> objModelString)
        {
            if (objModelString.Count == 0)
            {
                MessageBox.Show("No 3D model data to write to file.", "No Model Created", MessageBoxButtons.OK);
                return;
            }

            SaveFileDialog sfd = new()
            {
                Filter = "Wavefront OBJ files (*.obj)|*.obj",
                Title = "Save 3D Model as OBJ File",
                InitialDirectory = AssetManager.DefaultModelsDirectory,
                RestoreDirectory = true,
                AddExtension = true,
                CheckPathExists = true,
                ShowHiddenFiles = false,
                ValidateNames = true,
            };

            DialogResult result = sfd.ShowDialog();

            if (result == DialogResult.OK)
            {
                string filename = sfd.FileName;
                File.WriteAllLines(filename, objModelString);
                MessageBox.Show("Height map model written to " + System.IO.Path.GetFileName(filename), "Model File Saved", MessageBoxButtons.OK);
            }
        }


        internal static List<string> GenerateOBJ(SKBitmap grayscaleHeightMap, float heightScale)
        {
            int width = grayscaleHeightMap.Width;
            int height = grayscaleHeightMap.Height;

            List<string> objModelList = [];
            List<Vector3> vertices = [];
            List<Vector3> faces = [];
            Vector3[,] normals = new Vector3[width, height];

            // generate vertices
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    SKColor pixel = grayscaleHeightMap.GetPixel(x, y);
                    float heightValue = (float)pixel.Red / byte.MaxValue * heightScale;

                    Vector3 vertex = new(x, heightValue, y);
                    vertices.Add(vertex);
                }
            }

            // generate normals
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    float hl = (float)grayscaleHeightMap.GetPixel(x - 1, y).Red / byte.MaxValue * heightScale;
                    float hr = (float)grayscaleHeightMap.GetPixel(x + 1, y).Red / byte.MaxValue * heightScale;
                    float hu = (float)grayscaleHeightMap.GetPixel(x, y - 1).Red / byte.MaxValue * heightScale;
                    float hd = (float)grayscaleHeightMap.GetPixel(x, y + 1).Red / byte.MaxValue * heightScale;

                    Vector3 normal = Vector3.Normalize(new Vector3(hl - hr, 2, hu - hd));
                    normals[x, y] = normal;
                }
            }

            // generate faces
            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    int v1 = y * width + x + 1;
                    int v2 = y * width + (x + 1) + 1;
                    int v3 = (y + 1) * width + x + 1;
                    int v4 = (y + 1) * width + (x + 1) + 1;

                    Vector3 face1 = new(v1, v2, v3);
                    Vector3 face2 = new(v2, v4, v3);

                    faces.Add(face1);
                    faces.Add(face2);
                }
            }

            // Write vertices
            objModelList.Add("# vertices");
            foreach (var vertex in vertices)
            {
                objModelList.Add($"v {vertex.X} {vertex.Y} {vertex.Z}");
            }

            // Write normals
            objModelList.Add("# normals");
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector3 normal = normals[x, y];
                    objModelList.Add(string.Format(CultureInfo.InvariantCulture, "vn {0} {1} {2}", normal.X, normal.Y, normal.Z));
                }
            }

            // write faces
            objModelList.Add("# faces");
            foreach (Vector3 face in faces)
            {
                string faceOutput = "f " + face.X + "//" + face.X + " "
                    + face.Y + "//" + face.Y + " "
                    + face.Z + "//" + face.Z;

                objModelList.Add(faceOutput);
            }

            Console.WriteLine("OBJ file generated successfully!");

            return objModelList;
        }
    }
}
