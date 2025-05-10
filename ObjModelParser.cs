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
using OpenTK.Mathematics;

namespace RealmStudio
{
    internal sealed class ObjModelParser
    {
        private static readonly List<Vector3> vertices = [];
        private static readonly List<Vector3> normals = [];
        private static readonly List<Vector2> texCoords = [];
        private static readonly List<int[]> faces = [];
        internal static readonly char[] separator = [' '];

        public static void ParseObjModel(string[] modelLines)
        {
            foreach (string line in modelLines)
            {
                string[] tokens = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Length == 0)
                {
                    continue;
                }

                switch (tokens[0])
                {
                    case "v":
                        vertices.Add(new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3])));
                        break;
                    case "vn":
                        normals.Add(new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3])));
                        break;
                    case "vt":
                        texCoords.Add(new Vector2(float.Parse(tokens[1]), float.Parse(tokens[2])));
                        break;
                    case "f":
                        int[] face = new int[9];
                        for (int i = 0; i < 3; i++)
                        {
                            string[] vertexTokens = tokens[i + 1].Split(['/'], StringSplitOptions.None);
                            face[i * 3] = int.Parse(vertexTokens[0]) - 1; // vertex index
                            face[i * 3 + 1] = int.Parse(vertexTokens[1]) - 1; // texture coordinate index
                            face[i * 3 + 2] = int.Parse(vertexTokens[2]) - 1; // normal index
                        }
                        faces.Add(face);
                        break;
                }
            }
        }

        public static List<Vector3> Vertices { get { return vertices; } }
        public static List<Vector3> Normals { get { return normals; } }
        public static List<Vector2> TexCoords { get { return texCoords; } }
        public static List<int[]> Faces { get { return faces; } }
    }
}
