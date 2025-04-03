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

using System.IO;

namespace RealmStudio
{
    internal sealed class LabelPresetManager : IMapComponentManager
    {
        private static List<LabelPreset> _labelPresets = [];
        private static LabelPresetUIMediator? _presetMediator;
        private static string _labelPresetsDirectory = string.Empty;

        internal static LabelPresetUIMediator? PresetMediator
        {
            get { return _presetMediator; }
            set { _presetMediator = value; }
        }

        internal static string LabelPresetsDirectory
        {
            get { return _labelPresetsDirectory; }
            set { _labelPresetsDirectory = value; }
        }

        internal static List<LabelPreset> LabelPresets
        {
            get { return _labelPresets; }
            set { _labelPresets = value; }
        }

        public static IMapComponent? Create()
        {
            throw new NotImplementedException();
        }

        public static bool Delete()
        {
            ArgumentNullException.ThrowIfNull(PresetMediator);
            LabelPreset? presetToDelete = PresetMediator.DeletingPreset;

            if (presetToDelete != null)
            {
                try
                {
                    File.Delete(presetToDelete.PresetXmlFilePath);
                    return true;
                }
                catch (Exception ex)
                {
                    // error of some kind, so return false
                    Program.LOGGER.Error(ex);
                    return false;
                }
            }

            // nothing to delete, so return true
            return true;
        }

        public static IMapComponent? GetComponentById(Guid componentGuid)
        {
            throw new NotImplementedException();
        }

        public static bool Update()
        {
            throw new NotImplementedException();
        }

        internal static int LoadLabelPresets()
        {
            LabelPresets.Clear();
            int numPresets = 0;

            var files = from file in Directory.EnumerateFiles(LabelPresetManager.LabelPresetsDirectory, "*.*", SearchOption.AllDirectories).Order()
                        where file.Contains(".mclblprst")
                        select new
                        {
                            File = file
                        };

            foreach (var f in files)
            {
                LabelPreset? preset = MapFileMethods.ReadLabelPreset(f.File);

                if (preset != null)
                {
                    LabelPresets.Add(preset);
                }
            }

            return numPresets;
        }

        internal static void ClearLabelPresets()
        {
            LabelPresets.Clear();
        }

        internal static void AddPreset(LabelPreset preset)
        {
            // guard against adding the preset more than once
            bool newPreset = true;

            foreach (LabelPreset lp in LabelPresets)
            {
                if (lp.PresetXmlFilePath == preset.PresetXmlFilePath)
                {
                    newPreset = false;
                }
            }

            if (newPreset)
            {
                LabelPresets.Add(preset);
            }
        }
    }
}
