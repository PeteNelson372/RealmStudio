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
using RealmStudio.Properties;

namespace RealmStudio
{
    internal class MapToolMethods
    {
        public static List<NameGenerator> NameGenerators { get; set; } = [];
        public static List<NameBase> NameBases { get; set; } = [];
        public static List<string> NameLanguages { get; set; } = [];
        public static List<string> NameBaseNames { get; set; } = [];
        public static List<string> NameGeneratorNames { get; set; } = [];

        public static string GenerateRandomPlaceName()
        {
            string generatedName = string.Empty;

            List<INameGenerator> generators = [];

            foreach (NameGenerator generator in NameGenerators)
            {
                if (generator.IsSelected)
                {
                    generators.Add(generator);
                }
            }

            foreach (NameBase nameBase in NameBases)
            {
                if (nameBase.IsNameBaseSelected && nameBase.IsLanguageSelected)
                {
                    generators.Add(nameBase);
                }
            }

            if (generators.Count > 0)
            {
                int selectedGeneratorIndex = Random.Shared.Next(0, generators.Count);

                if (generators[selectedGeneratorIndex] is NameGenerator nameGen)
                {
                    generatedName = GenerateName(nameGen);
                }
                else if (generators[selectedGeneratorIndex] is NameBase nameBase)
                {
                    generatedName = GenerateName(nameBase);
                }
            }

            return generatedName;
        }

        private static string GenerateRandomNameBaseName()
        {
            string generatedName = string.Empty;
            List<INameGenerator> generators = [];

            foreach (NameBase nameBase in NameBases)
            {
                if (nameBase.IsLanguageSelected)
                {
                    generators.Add(nameBase);
                }
            }

            if (generators.Count > 0)
            {
                int selectedGeneratorIndex = Random.Shared.Next(0, generators.Count);
                if (generators[selectedGeneratorIndex] is NameBase nameBase)
                {
                    generatedName = GenerateName(nameBase);
                }
            }

            return generatedName;
        }

        private static string GenerateName(NameGenerator nameGen)
        {
            string generatedName = string.Empty;

            int column1Index = Random.Shared.Next(0, nameGen.Column1.Count);
            string column1Value = nameGen.Column1[column1Index];

            if (nameGen.Column2.Count > 0)
            {
                int column2Index = Random.Shared.Next(0, nameGen.Column2.Count);
                string column2Value = nameGen.Column2[column2Index];

                if (!string.IsNullOrEmpty(column2Value))
                {
                    generatedName = column1Value.Replace("%", column2Value);
                }
                else
                {
                    string nameBaseName = GenerateRandomNameBaseName();

                    if (!string.IsNullOrEmpty(nameBaseName))
                    {
                        generatedName = column1Value.Replace("%", nameBaseName);
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            else
            {
                string nameBaseName = GenerateRandomNameBaseName();

                if (!string.IsNullOrEmpty(nameBaseName))
                {
                    generatedName = column1Value.Replace("%", nameBaseName);
                }
                else
                {
                    return string.Empty;
                }
            }

            return generatedName;
        }

        private static string GenerateName(NameBase nameBase)
        {
            // simplified version of namebase name generation for now
            return nameBase.NameStrings[Random.Shared.Next(0, nameBase.NameStrings.Count)];
        }

        public static void LoadNameGeneratorFiles()
        {
            string assetDirectory = Settings.Default.MapAssetDirectory;
            string nameGeneratorsDirectory = assetDirectory + Path.DirectorySeparatorChar + "NameGenerators";

            var files = from file in Directory.EnumerateFiles(assetDirectory, "*.*", SearchOption.AllDirectories).Order()
                        where file.Contains(".txt")
                            || file.Contains(".csv")
                        select new
                        {
                            File = file
                        };

            foreach (var f in files)
            {
                string assetName = Path.GetFileNameWithoutExtension(f.File);
                string path = Path.GetFullPath(f.File);

                if (Path.GetExtension(path).EndsWith("csv"))
                {
                    LoadNameGeneratorFile(path);
                }
                else if (Path.GetExtension(path).EndsWith("txt"))
                {
                    LoadNameBaseFile(path);
                }
            }

            foreach (NameBase nameBase in NameBases)
            {
                if (!string.IsNullOrEmpty(nameBase.NameBaseLanguage) && !NameLanguages.Contains(nameBase.NameBaseLanguage))
                {
                    NameLanguages.Add(nameBase.NameBaseLanguage.Trim());
                }

                if (!NameBaseNames.Contains(nameBase.NameBaseName))
                {
                    NameBaseNames.Add(nameBase.NameBaseName);
                }
            }

            foreach (NameGenerator nameGenerator in NameGenerators)
            {
                if (!NameGeneratorNames.Contains(nameGenerator.NameGeneratorName))
                {
                    NameGeneratorNames.Add(nameGenerator.NameGeneratorName);
                }
            }
        }

        private static void LoadNameBaseFile(string path)
        {
            IEnumerable<string> lines = File.ReadLines(path);

            if (lines.Any())
            {
                foreach (var line in lines)
                {
                    NameBase nameBase = new()
                    {
                        NameBaseName = Path.GetFileNameWithoutExtension(path)
                    };

                    string[] lineParts = line.Split('|');

                    if (lineParts.Length == 6)
                    {
                        nameBase.NameBaseLanguage = lineParts[0].Trim();
                        nameBase.MinNameLength = int.Parse(lineParts[1]);
                        nameBase.MaxNameLength = int.Parse(lineParts[2]);

                        foreach (char c in lineParts[3])
                        {
                            nameBase.RepeatableCharacters.Add(c);
                        }

                        nameBase.SingleWordTransformProportion = float.Parse(lineParts[4]);

                        string[] nameBaseNames = lineParts[5].Split(",");

                        for (int i = 0; i < nameBaseNames.Length; i++)
                        {
                            nameBaseNames[i] = nameBaseNames[i].Trim();
                        }

                        nameBase.NameStrings.AddRange(nameBaseNames);

                        if (!string.IsNullOrEmpty(nameBase.NameBaseLanguage) && nameBase.NameStrings.Count > 0)
                        {
                            NameBases.Add(nameBase);
                        }
                    }

                }
            }
        }

        private static void LoadNameGeneratorFile(string path)
        {
            IEnumerable<string> lines = File.ReadLines(path);

            if (lines.Any())
            {
                NameGenerator generator = new();
                generator.NameGeneratorName = Path.GetFileNameWithoutExtension(path);

                foreach (var line in lines)
                {
                    string[] lineParts = line.Split(',');

                    if (!string.IsNullOrEmpty(lineParts[0]))
                    {
                        generator.Column1.Add(lineParts[0].Trim());
                    }

                    if (lineParts.Length > 1)
                    {
                        if (!string.IsNullOrEmpty(lineParts[1]))
                        {
                            generator.Column2.Add(lineParts[1].Trim());
                        }
                    }
                }

                NameGenerators.Add(generator);
            }
        }
    }
}
