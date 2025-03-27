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
using RealmStudio.Properties;
using SkiaSharp;
using System.Timers;

namespace RealmStudio
{
    internal class TimerManager
    {
        private static MapSymbolUIMediator? _symbolUIMediator;

        private static System.Timers.Timer? _autosaveTimer;
        private static System.Timers.Timer? _brushTimer;
        private static System.Timers.Timer? _symbolAreaBrushTimer;

        private static bool autosaveEnabled;
        private static bool brushTimerEnabled;
        private static bool symbolAreaBrushEnabled;

        internal static MapSymbolUIMediator? SymbolUIMediator
        {
            get { return _symbolUIMediator; }
            set { _symbolUIMediator = value; }
        }

        public static bool AutosaveEnabled
        {
            get { return autosaveEnabled; }
            set
            {
                autosaveEnabled = value && Settings.Default.RealmAutosave && autosaveEnabled;

                if (autosaveEnabled)
                {
                    StartAutosaveTimer();
                }
                else
                {
                    StopAutosaveTimer();
                }
            }
        }

        internal static bool BrushTimerEnabled
        {
            get { return brushTimerEnabled; }
            set
            {
                brushTimerEnabled = value;

                if (brushTimerEnabled)
                {
                    StartBrushTimer();
                }
                else
                {
                    StopBrushTimer();
                }
            }
        }

        internal static bool SymbolAreaBrushTimerEnabled
        {
            get { return symbolAreaBrushEnabled; }
            set
            {
                symbolAreaBrushEnabled = value;

                if (symbolAreaBrushEnabled)
                {
                    StartSymbolAreaBrushTimer();
                }
                else
                {
                    StopSymbolAreaBrushTimer();
                }
            }
        }

        internal static void StartAutosaveTimer()
        {
            // stop the autosave timer
            StopAutosaveTimer();

            int saveIntervalMinutes = Settings.Default.AutosaveInterval;

            // save interval cannot be less than 5 minutes (300000 milliseconds)
            int saveIntervalMillis = Math.Max(5 * 60 * 1000, saveIntervalMinutes * 60 * 1000);

            // start the autosave timer
            _autosaveTimer = new System.Timers.Timer
            {
                Interval = saveIntervalMillis,
                AutoReset = true,
                SynchronizingObject = RealmMapState.GLRenderControl
            };

            _autosaveTimer.Elapsed += new ElapsedEventHandler(AutosaveTimerEventHandler);
            _autosaveTimer.Start();
        }

        internal static void StopAutosaveTimer()
        {
            _autosaveTimer?.Stop();
            _autosaveTimer?.Dispose();
            _autosaveTimer = null;
        }

        private static void AutosaveTimerEventHandler(object? sender, ElapsedEventArgs e)
        {
            try
            {
                RealmMapMethods.PruneOldBackupsOfMap(RealmMapState.CurrentMap, RealmMapState.BackupCount);

                if (!RealmMapState.CurrentMap.IsSaved)
                {
                    if (!string.IsNullOrWhiteSpace(RealmMapState.CurrentMap.MapPath))
                    {
                        MapFileMethods.SaveMap(RealmMapState.CurrentMap);
                    }

                    bool saveSuccess = RealmMapMethods.SaveRealmBackup(RealmMapState.CurrentMap, false);

                    if (saveSuccess)
                    {
                        if (Settings.Default.PlaySoundOnSave)
                        {
                            UtilityMethods.PlaySaveSound();
                        }

                        //SetStatusText("A backup of the realm has been saved.");
                    }
                }
            }
            catch (Exception ex)
            {
                Program.LOGGER.Error(ex);
            }
        }

        private static void StartBrushTimer()
        {
            if (SymbolUIMediator == null) return;

            // stop the brush timer if it is running
            StopBrushTimer();

            // start the brush timer
            _brushTimer = new System.Timers.Timer
            {
                Interval = 200.0F / SymbolUIMediator.SymbolPlacementRate,
                AutoReset = true,
                SynchronizingObject = RealmMapState.GLRenderControl,
            };

            _brushTimer.Elapsed += new ElapsedEventHandler(BrushTimerEventHandler);
            _brushTimer.Start();
        }

        private static void StopBrushTimer()
        {
            _brushTimer?.Stop();
            _brushTimer?.Dispose();
            _brushTimer = null;
        }

        private static void BrushTimerEventHandler(Object? eventObject, EventArgs eventArgs)
        {
            if (RealmMapState.GLRenderControl == null) return;

            Point cursorPoint = RealmMapState.GLRenderControl.PointToClient(Cursor.Position);

            SKPoint zoomedScrolledPoint = new((cursorPoint.X / RealmMapState.DrawingZoom) + RealmMapState.DrawingPoint.X, (cursorPoint.Y / RealmMapState.DrawingZoom) + RealmMapState.DrawingPoint.Y);
            RealmMapState.CurrentLayerPaintStroke?.AddLayerPaintStrokePoint(zoomedScrolledPoint);

            RealmMapState.GLRenderControl.Invalidate();
        }


        private static void StartSymbolAreaBrushTimer()
        {
            if (SymbolUIMediator == null) return;

            // stop the symbols area brush timer if it is running
            StopSymbolAreaBrushTimer();

            // start the brush timer
            _symbolAreaBrushTimer = new System.Timers.Timer
            {
                Interval = 200.0F / SymbolUIMediator.SymbolPlacementRate,
                AutoReset = true,
                SynchronizingObject = SymbolUIMediator.SymbolTable,
            };

            _symbolAreaBrushTimer.Elapsed += new ElapsedEventHandler(SymbolAreaBrushTimerEventHandler);
            _symbolAreaBrushTimer.Start();
        }

        private static void StopSymbolAreaBrushTimer()
        {
            _symbolAreaBrushTimer?.Stop();
            _symbolAreaBrushTimer?.Dispose();
            _symbolAreaBrushTimer = null;
        }

        private static void SymbolAreaBrushTimerEventHandler(object? sender, ElapsedEventArgs e)
        {
            if (SymbolUIMediator == null) return;

            float symbolScale = SymbolUIMediator.SymbolScale / 100.0F;
            float symbolRotation = SymbolUIMediator.SymbolRotation;

            RealmMapState.SelectedBrushSize = SymbolUIMediator.AreaBrushSize;

            SymbolManager.PlaceSelectedSymbolInArea(RealmMapState.CurrentCursorPoint,
                symbolScale, symbolRotation, (int)(SymbolUIMediator.AreaBrushSize / 2.0F));
        }
    }
}
