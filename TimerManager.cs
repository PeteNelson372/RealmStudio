﻿/**************************************************************************************************************************
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
using System.Timers;

namespace RealmStudio
{
    internal sealed class TimerManager(RealmStudioMainForm mainForm) : IDisposable
    {
        private bool disposedValue;

        private readonly RealmStudioMainForm MainForm = mainForm;
        private SymbolUIMediator? _symbolUIMediator;

        private System.Timers.Timer? _autosaveTimer;
        private System.Timers.Timer? _brushTimer;
        private System.Timers.Timer? _symbolAreaBrushTimer;
        private System.Timers.Timer? _versionCheckTimer;

        private bool _autosaveEnabled;
        private bool _brushTimerEnabled;
        private bool _symbolAreaBrushEnabled;
        private bool _versionCheckEnabled = true;

        internal SymbolUIMediator? SymbolUIMediator
        {
            get { return _symbolUIMediator; }
            set { _symbolUIMediator = value; }
        }

        public bool AutosaveEnabled
        {
            get { return _autosaveEnabled; }
            set
            {
                _autosaveEnabled = value && Settings.Default.RealmAutosave && _autosaveEnabled;

                if (_autosaveEnabled)
                {
                    StartAutosaveTimer();
                }
                else
                {
                    StopAutosaveTimer();
                }
            }
        }

        internal bool BrushTimerEnabled
        {
            get { return _brushTimerEnabled; }
            set
            {
                _brushTimerEnabled = value;

                if (_brushTimerEnabled)
                {
                    StartBrushTimer();
                }
                else
                {
                    StopBrushTimer();
                }
            }
        }

        internal bool SymbolAreaBrushTimerEnabled
        {
            get { return _symbolAreaBrushEnabled; }
            set
            {
                _symbolAreaBrushEnabled = value;

                if (_symbolAreaBrushEnabled)
                {
                    StartSymbolAreaBrushTimer();
                }
                else
                {
                    StopSymbolAreaBrushTimer();
                }
            }
        }

        internal bool VersionCheckEnabled
        {
            get { return _versionCheckEnabled; }
            set
            {
                _versionCheckEnabled = value;
            }
        }

        internal void StartAutosaveTimer()
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
                SynchronizingObject = MainForm,
            };

            _autosaveTimer.Elapsed += new ElapsedEventHandler(AutosaveTimerEventHandler);
            _autosaveTimer.Start();
        }

        internal void StopAutosaveTimer()
        {
            _autosaveTimer?.Stop();
            _autosaveTimer?.Dispose();
            _autosaveTimer = null;
        }

        private void AutosaveTimerEventHandler(object? sender, ElapsedEventArgs e)
        {
            try
            {
                RealmMapMethods.PruneOldBackupsOfMap(MapStateMediator.CurrentMap, MapStateMediator.BackupCount);

                if (!MapStateMediator.CurrentMap.IsSaved)
                {
                    if (!string.IsNullOrWhiteSpace(MapStateMediator.CurrentMap.MapPath))
                    {
                        MapFileMethods.SaveMap(MapStateMediator.CurrentMap);
                    }

                    bool saveSuccess = RealmMapMethods.SaveRealmBackup(MapStateMediator.CurrentMap, false);

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

        private void StartBrushTimer()
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);

            // stop the brush timer if it is running
            StopBrushTimer();

            // start the brush timer
            _brushTimer = new System.Timers.Timer
            {
                Interval = MapStateMediator.MainUIMediator.CurrentBrushVelocity,
                AutoReset = true,
                SynchronizingObject = MainForm.SKGLRenderControl,
            };

            _brushTimer.Elapsed += new ElapsedEventHandler(BrushTimerEventHandler);
            _brushTimer.Start();
        }

        private void StopBrushTimer()
        {
            _brushTimer?.Stop();
            _brushTimer?.Dispose();
            _brushTimer = null;
        }

        private void BrushTimerEventHandler(object? eventObject, EventArgs eventArgs)
        {
            MapStateMediator.CurrentLayerPaintStroke?.AddLayerPaintStrokePoint(MapStateMediator.CurrentCursorPoint);
            MainForm.SKGLRenderControl.Invalidate();
        }


        private void StartSymbolAreaBrushTimer()
        {
            ArgumentNullException.ThrowIfNull(SymbolUIMediator);

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

        private void StopSymbolAreaBrushTimer()
        {
            _symbolAreaBrushTimer?.Stop();
            _symbolAreaBrushTimer?.Dispose();
            _symbolAreaBrushTimer = null;
        }

        private void SymbolAreaBrushTimerEventHandler(object? sender, ElapsedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(MapStateMediator.MainUIMediator);
            ArgumentNullException.ThrowIfNull(SymbolUIMediator);

            if (SymbolUIMediator.AreaBrushSize > 4.0F)
            {
                float symbolScale = SymbolUIMediator.SymbolScale / 100.0F;
                float symbolRotation = SymbolUIMediator.SymbolRotation;

                MapStateMediator.MainUIMediator.SelectedBrushSize = SymbolUIMediator.AreaBrushSize;

                SymbolManager.PlaceSelectedSymbolInArea(MapStateMediator.CurrentCursorPoint,
                    symbolScale, symbolRotation, (int)(SymbolUIMediator.AreaBrushSize / 2.0F));
            }
        }

        internal void StopVersionCheckTimer()
        {
            _versionCheckTimer?.Stop();
            _versionCheckTimer?.Dispose();
            _versionCheckTimer = null;
        }

        internal void StartVersionCheckTimer()
        {
            // stop the version check timer if it is running
            StopVersionCheckTimer();
            // start the version check timer
            _versionCheckTimer = new System.Timers.Timer
            {
                Interval = 2 * 60 * 1000, // 2 minutes in milliseconds
                AutoReset = false,
                SynchronizingObject = MainForm,
            };
            _versionCheckTimer.Elapsed += new ElapsedEventHandler(VersionCheckTimerEventHandler);
            _versionCheckTimer.Start();
        }

        private void VersionCheckTimerEventHandler(object? sender, ElapsedEventArgs e)
        {
            ReleaseChecker.FetchRealmStudioGithubReleasesAsync().ContinueWith(releasesTask =>
            {
                List<(string, string)> releases = releasesTask.Result;

                string? newReleaseVersion = ReleaseChecker.HasNewRelease(releases, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown");

                if (newReleaseVersion != null)
                {
                    MainForm.Invoke(new Action(() =>
                    {
                        MainForm.NewVersionButton.Visible = true;
                    }));
                }
                else
                {
                    MainForm.Invoke(new Action(() =>
                    {
                        MainForm.NewVersionButton.Visible = false;
                    }));
                }
            });
        }



        #region IDisposable Implementation
        internal void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _autosaveTimer?.Dispose();
                    _brushTimer?.Dispose();
                    _symbolAreaBrushTimer?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
