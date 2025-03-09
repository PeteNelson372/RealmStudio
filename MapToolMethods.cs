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
namespace RealmStudio
{
    internal class MapToolMethods
    {
        public static List<NameGenerator> NameGenerators { get; set; } = [];
        public static List<NameBase> NameBases { get; set; } = [];
        public static List<NameBaseLanguage> NameLanguages { get; set; } = [];

        public static string GenerateRandomPlaceName(List<INameGenerator> generators)
        {
            string generatedName = string.Empty;

            if (generators.Count > 0)
            {
                int selectedGeneratorIndex = -1;
                int guardCount = 0;
                int maxTries = generators.Count * generators.Count;

                while (guardCount < maxTries && (selectedGeneratorIndex < 0 || generators[selectedGeneratorIndex] is NameBaseLanguage))
                {
                    guardCount++;
                    selectedGeneratorIndex = Random.Shared.Next(0, generators.Count);
                }

                Random.Shared.Next(0, generators.Count);

                if (generators[selectedGeneratorIndex] is NameGenerator nameGen)
                {
                    generatedName = GenerateName(nameGen, generators);
                }
                else if (generators[selectedGeneratorIndex] is NameBase nameBase)
                {
                    generatedName = GenerateName(nameBase, generators);
                }
                //else if (generators[selectedGeneratorIndex] is NameBaseLanguage language)
                //{
                //    generatedName = GenerateName(language);
                //}
            }

            return generatedName;
        }

        private static string GenerateRandomNameBaseName()
        {
            string generatedName = string.Empty;
            List<INameGenerator> generators = [];

            foreach (NameBase nameBase in NameBases)
            {
                if (nameBase.IsNameBaseSelected)
                {
                    foreach (NameBaseLanguage language in nameBase.Languages)
                    {
                        if (language.IsLanguageSelected)
                        {
                            generators.Add(language);
                        }
                    }
                }
            }

            if (generators.Count > 0)
            {
                int selectedGeneratorIndex = Random.Shared.Next(0, generators.Count);
                if (generators[selectedGeneratorIndex] is NameBase nameBase)
                {
                    generatedName = GenerateName(nameBase, generators);
                }
            }

            return generatedName;
        }

        private static string GenerateRandomNameForLanguage(List<INameGenerator> selectedGenerators)
        {
            if (selectedGenerators.Count > 0)
            {
                List<NameBaseLanguage> SelectedNameLanguages = [];

                foreach (INameGenerator gen in selectedGenerators)
                {
                    if (gen is NameBaseLanguage l && l.IsLanguageSelected)
                    {
                        SelectedNameLanguages.Add(l);
                    }
                }

                if (SelectedNameLanguages.Count > 0)
                {
                    // select a random language from the namebase
                    int languageIndex = Random.Shared.Next(0, SelectedNameLanguages.Count);

                    // simplified version of namebase name generation for now; select a name from the language
                    return SelectedNameLanguages[languageIndex].NameStrings[Random.Shared.Next(0, SelectedNameLanguages[languageIndex].NameStrings.Count)];

                }
                else
                {
                    return string.Empty;
                }
            }

            return string.Empty;
        }

        private static string GenerateName(NameGenerator nameGen, List<INameGenerator> selectedGenerators)
        {
            string generatedName;

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
                string name = GenerateRandomNameForLanguage(selectedGenerators);

                if (!string.IsNullOrEmpty(name))
                {
                    generatedName = column1Value.Replace("%", name);
                }
                else
                {
                    return string.Empty;
                }
            }

            return generatedName;
        }

        private static string GenerateName(NameBase nameBase, List<INameGenerator> selectedGenerators)
        {
            List<NameBaseLanguage> SelectedNameLanguages = [];

            foreach (INameGenerator gen in selectedGenerators)
            {
                if (gen is NameBaseLanguage l && l.IsLanguageSelected)
                {
                    foreach (NameBaseLanguage nbl in nameBase.Languages)
                    {
                        if (nbl.Language == l.Language)
                        {
                            SelectedNameLanguages.Add(nbl);
                        }
                    }
                }
            }

            if (SelectedNameLanguages.Count > 0)
            {
                // select a random language from the namebase
                int languageIndex = Random.Shared.Next(0, SelectedNameLanguages.Count);

                // simplified version of namebase name generation for now; select a name from the language
                return SelectedNameLanguages[languageIndex].NameStrings[Random.Shared.Next(0, SelectedNameLanguages[languageIndex].NameStrings.Count)];
            }

            return string.Empty;
        }

        private static string GenerateName(NameBaseLanguage language)
        {
            // simplified version of namebase name generation for now; select a name from the language
            return language.NameStrings[Random.Shared.Next(0, language.NameStrings.Count)];
        }

        public static void LoadNameGeneratorFiles()
        {
            //string assetDirectory = Settings.Default.MapAssetDirectory;
            string nameGeneratorsDirectory = AssetManager.ASSET_DIRECTORY + Path.DirectorySeparatorChar + "NameGenerators";

            var files = from file in Directory.EnumerateFiles(AssetManager.ASSET_DIRECTORY, "*.*", SearchOption.AllDirectories).Order()
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
                if (nameBase.Languages.Count > 0)
                {
                    foreach (var language in nameBase.Languages)
                    {
                        if (!NameLanguages.Contains(language))
                        {
                            NameLanguages.Add(language);
                        }
                    }
                }
            }
        }

        private static void LoadNameBaseFile(string path)
        {
            IEnumerable<string> lines = File.ReadLines(path);

            if (lines.Any())
            {
                NameBase nameBase = new()
                {
                    NameBaseName = Path.GetFileNameWithoutExtension(path)
                };

                foreach (var line in lines)
                {
                    string[] lineParts = line.Split('|');

                    if (lineParts.Length == 6)
                    {
                        NameBaseLanguage language = new()
                        {
                            Language = lineParts[0].Trim(),
                            MinNameLength = int.Parse(lineParts[1]),
                            MaxNameLength = int.Parse(lineParts[2])
                        };

                        foreach (char c in lineParts[3])
                        {
                            language.RepeatableCharacters.Add(c);
                        }

                        language.SingleWordTransformProportion = float.Parse(lineParts[4]);

                        string[] nameBaseNames = lineParts[5].Split(",");

                        for (int i = 0; i < nameBaseNames.Length; i++)
                        {
                            nameBaseNames[i] = nameBaseNames[i].Trim();
                        }

                        language.NameStrings.AddRange(nameBaseNames);

                        if (!string.IsNullOrEmpty(language.Language) && language.NameStrings.Count > 0)
                        {
                            nameBase.Languages.Add(language);
                        }
                    }
                }

                if (nameBase.Languages.Count > 0)
                {
                    NameBases.Add(nameBase);
                }
            }
        }

        private static void LoadNameGeneratorFile(string path)
        {
            IEnumerable<string> lines = File.ReadLines(path);

            if (lines.Any())
            {
                NameGenerator generator = new()
                {
                    NameGeneratorName = Path.GetFileNameWithoutExtension(path)
                };

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

        internal static void LoadShapingFunctions()
        {
            //string assetDirectory = Settings.Default.MapAssetDirectory;
            string nameGeneratorsDirectory = AssetManager.ASSET_DIRECTORY + Path.DirectorySeparatorChar + "ShapingFunctions";

            var files = from file in Directory.EnumerateFiles(AssetManager.ASSET_DIRECTORY, "*.*", SearchOption.AllDirectories).Order()
                        where file.Contains(".bmp")
                        select new
                        {
                            File = file
                        };

            foreach (var f in files)
            {
                string path = Path.GetFullPath(f.File);

                LandformShapingFunction? shapingFunction = MapFileMethods.ReadShapingFunction(path);

                if (shapingFunction != null)
                {
                    AssetManager.LANDFORM_SHAPING_FUNCTIONS.Add(shapingFunction);
                }
            }

        }
    }
}
