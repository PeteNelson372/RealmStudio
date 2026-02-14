#nullable enable

using RealmStudioShapeRenderingLib;
using SkiaSharp;
using System.Diagnostics;
using System.Windows.Documents;

namespace RealmStudioX
{
    internal sealed class EditorController : IDisposable
    {
        private readonly System.Windows.Forms.Timer _uiStateTimer;

        // -------------------------------------------------
        // Core editor state
        // -------------------------------------------------

        public MapScene? Scene { get; private set; } = null!;
        public CommandManager Commands { get; } = new();

        private MainFormUIMediator? _mainMediator;
        private BackgroundUIMediator? _backgroundMediator;
        private BoxUIMediator? _boxMediator;
        private CameraUIMediator? _cameraMediator;
        private DrawingUIMediator? _drawingMediator;
        private FrameUIMediator? _frameMediator;
        private MapGridUIMediator? _gridMediator;
        private InteriorUIMediator? _interiorMediator;
        private LabelUIMediator? _labelMediator;
        private OceanUIMediator? _oceanMediator;
        private PathUIMediator? _pathMediator;
        private LabelPresetUIMediator? _presetMediator;
        private LandformUIMediator? _landformMediator;
        private MapMeasureUIMediator? _measureMediator;
        private MapScaleUIMediator? _scaleMediator;
        private SymbolUIMediator? _symbolMediator;
        private RegionUIMediator? _regionMediator;
        private VignetteUIMediator? _vignetteMediator;
        private WaterFeatureUIMediator? _waterFeatureMediator;
        private WindroseUIMediator? _windroseMediator;
        private MenuUIMediator? _menuMediator;

        private SKSize _viewportSize;

        private Shape2D? _selectedShape;

        // -------------------------------------------------
        // UI policy & resolved UI state
        // -------------------------------------------------

        private RealmTypeUiPolicy _uiPolicy = new();
        private UserInterfaceState _userInterfaceState = UserInterfaceState.Empty;

        public UserInterfaceState CurrentUiState => _userInterfaceState;

        public RealmTypeUiPolicy UserInterfacePolicy
        {
            get { return _uiPolicy; }
            set { _uiPolicy = value; }
        }

        public Shape2D? SelectedShape
        {
            get { return _selectedShape; }
            set { _selectedShape = value; }
        }

        // -------------------------------------------------
        // Mediators (injected)
        // -------------------------------------------------

        public MainFormUIMediator? MainMediator { get { return _mainMediator; } set { _mainMediator = value; } }

        public BackgroundUIMediator? BackgroundMediator { get { return _backgroundMediator; } set { _backgroundMediator = value; } }

        public BoxUIMediator? BoxMediator { get { return _boxMediator; } set { _boxMediator = value; } }

        public CameraUIMediator? CameraMediator { get { return _cameraMediator; } set { _cameraMediator = value; } }

        public DrawingUIMediator? DrawingMediator { get { return _drawingMediator; } set { _drawingMediator = value; } }

        public FrameUIMediator? FrameMediator { get { return _frameMediator; } set { _frameMediator = value; } }

        public MapGridUIMediator? GridMediator { get { return _gridMediator; } set { _gridMediator = value; } }

        public InteriorUIMediator? InteriorMediator { get { return _interiorMediator; } set { _interiorMediator = value; } }

        public LabelUIMediator? LabelMediator { get { return _labelMediator; } set { _labelMediator = value; } }

        public OceanUIMediator? OceanMediator { get { return _oceanMediator; } set { _oceanMediator = value; } }

        public PathUIMediator? PathMediator { get { return _pathMediator; } set { _pathMediator = value; } }

        public LabelPresetUIMediator? LabelPresetMediator { get { return _presetMediator; } set { _presetMediator = value; } }

        public LandformUIMediator? LandformMediator { get { return _landformMediator; } set { _landformMediator = value; } }

        public MapMeasureUIMediator? MeasureMediator { get { return _measureMediator; } set { _measureMediator = value; } }

        public MapScaleUIMediator? ScaleMediator { get { return _scaleMediator; } set { _scaleMediator = value; } }

        public SymbolUIMediator? SymbolMediator { get { return _symbolMediator; } set { _symbolMediator = value; } }

        public RegionUIMediator? RegionMediator { get { return _regionMediator; } set { _regionMediator = value; } }

        public VignetteUIMediator? VignetteMediator { get { return _vignetteMediator; } set { _vignetteMediator = value; } }

        public WaterFeatureUIMediator? WaterFeatureMediator { get { return _waterFeatureMediator; } set { _waterFeatureMediator = value; } }

        public WindroseUIMediator? WindroseMediator { get { return _windroseMediator; } set { _windroseMediator = value; } }

        public MenuUIMediator? MenuMediator { get { return _menuMediator; } set { _menuMediator = value; } }

        // -------------------------------------------------
        // Events (editor → UI)
        // -------------------------------------------------

        public event Action<UserInterfaceState>? UiStateChanged;
        public event Action<AppStatusBarUiState>? StatusBarUpdated;
        public event Action? SceneChanged;
        public event Action? RequestRedraw;

        // -------------------------------------------------
        // Camera interaction state
        // -------------------------------------------------

        private bool _isMiddlePanning;
        private SKPoint _lastMouseScreen;

        private bool _disposed;

        public EditorController()
        {
            _uiStateTimer = new System.Windows.Forms.Timer
            {
                Interval = 15   // 10–20ms ideal
            };
            _uiStateTimer.Tick += UiStateTimer_Tick;

            Commands.HistoryChanged += OnHistoryChanged;
        }

        private void OnHistoryChanged()
        {
            if (Scene == null)
                return;

            Scene.Map.MarkChanged();
            ScheduleUiStateResolve();
        }

        // ---------------------------------------------
        // Scene lifecycle
        // ---------------------------------------------

        public void OpenScene(MapScene scene)
        {
            Scene = scene;
            Commands.ClearAll();

            _uiPolicy = RealmTypeUiPolicyFactory.Create(Scene.Map.RealmType);

            ScheduleUiStateResolve();

            SceneChanged?.Invoke();
            RequestRedraw?.Invoke();
        }

        internal void WireMediatorEvents()
        {
            // Subscribe mediators' Changed events to corresponding handlers.
            _mainMediator?.Changed += OnMainMediatorChanged;
            _backgroundMediator?.Changed += OnBackgroundMediatorChanged;
            _boxMediator?.Changed += OnBoxMediatorChanged;
            _drawingMediator?.Changed += OnDrawingMediatorChanged;
            _frameMediator?.Changed += OnFrameMediatorChanged;
            _gridMediator?.Changed += OnGridMediatorChanged;
            _interiorMediator?.Changed += OnInteriorMediatorChanged;
            _labelMediator?.Changed += OnLabelMediatorChanged;
            _oceanMediator?.Changed += OnOceanMediatorChanged;
            _pathMediator?.Changed += OnPathMediatorChanged;
            _presetMediator?.Changed += OnPresetMediatorChanged;
            _landformMediator?.Changed += OnLandformMediatorChanged;
            _measureMediator?.Changed += OnMeasureMediatorChanged;
            _scaleMediator?.Changed += OnScaleMediatorChanged;
            _symbolMediator?.Changed += OnSymbolMediatorChanged;
            _regionMediator?.Changed += OnRegionMediatorChanged;
            _vignetteMediator?.Changed += OnVignetteMediatorChanged;
            _waterFeatureMediator?.Changed += OnWaterFeatureMediatorChanged;
            _windroseMediator?.Changed += OnWindroseMediatorChanged;
            _cameraMediator?.Changed += OnCameraMediatorChanged;
            _menuMediator?.Changed += OnMenuMediatorChanged;

            //_toolMediator.Changed += _ => OnToolMediatorChanged();
        }

        private void UiStateTimer_Tick(object? sender, EventArgs e)
        {
            _uiStateTimer.Stop();
            ResolveUiState();
        }

        private void ScheduleUiStateResolve()
        {
            _uiStateTimer.Stop();
            _uiStateTimer.Start();
        }


        private void OnMenuMediatorChanged()
        {
            ScheduleUiStateResolve();
        }

        private void OnMainMediatorChanged()
        {
            ScheduleUiStateResolve();
        }

        private void OnBackgroundMediatorChanged()
        {
            ScheduleUiStateResolve();
        }

        private void OnBoxMediatorChanged()
        {
            ScheduleUiStateResolve();
        }

        private void OnDrawingMediatorChanged()
        {
            ScheduleUiStateResolve();
        }

        private void OnFrameMediatorChanged()
        {
            ScheduleUiStateResolve();
        }

        private void OnGridMediatorChanged()
        {
            ScheduleUiStateResolve();
        }

        private void OnInteriorMediatorChanged()
        {
            ScheduleUiStateResolve();
        }

        private void OnLabelMediatorChanged()
        {
            ScheduleUiStateResolve();
        }

        private void OnOceanMediatorChanged()
        {
            ScheduleUiStateResolve();
        }

        private void OnPathMediatorChanged()
        {
            ScheduleUiStateResolve();
        }
        private void OnPresetMediatorChanged()
        {
            ScheduleUiStateResolve();
        }

        private void OnLandformMediatorChanged()
        {
            ApplyLandformChanges();

            ScheduleUiStateResolve();
        }

        private void OnMeasureMediatorChanged()
        {
            ScheduleUiStateResolve();
        }

        private void OnScaleMediatorChanged()
        {
            ScheduleUiStateResolve();
        }

        private void OnSymbolMediatorChanged()
        {
            ScheduleUiStateResolve();
        }

        private void OnRegionMediatorChanged()
        {
            ScheduleUiStateResolve();
        }

        private void OnVignetteMediatorChanged()
        {
            ScheduleUiStateResolve();
        }

        private void OnWaterFeatureMediatorChanged()
        {
            ScheduleUiStateResolve();
        }

        private void OnWindroseMediatorChanged()
        {
            ScheduleUiStateResolve();
        }

        private void OnCameraMediatorChanged()
        {
            ScheduleUiStateResolve();
        }

        private UserInterfaceState? _lastState;

        private int _resolveCounter;

        private void ResolveUiState()
        {
            if (_uiPolicy == null)
            {
                return;
            }

            UserInterfaceState _userInterfaceState = UserInterfaceStateBuilder.Build(_uiPolicy,
                Scene,
                this,
                Commands,
                MainMediator,
                BackgroundMediator,
                BoxMediator,
                DrawingMediator,
                FrameMediator,
                GridMediator,
                InteriorMediator,
                LabelMediator,
                OceanMediator,
                PathMediator,
                LabelPresetMediator,
                LandformMediator,
                MeasureMediator,
                ScaleMediator,
                SymbolMediator,
                RegionMediator,
                VignetteMediator,
                WaterFeatureMediator,
                WindroseMediator,
                MenuMediator);

            if (_lastState != null && _userInterfaceState.Equals(_lastState))
                return;

            _lastState = _userInterfaceState;

            UiStateChanged?.Invoke(_userInterfaceState);
        }


        // ---------------------------------------------
        // Input routing (screen → world)
        // ---------------------------------------------

        public void OnMouseDown(SKPoint screen, MouseButtons button)
        {
            if (button == MouseButtons.Middle)
            {
                BeginPan(screen);
                return;
            }

            MainMediator?.ActiveEditorTool?.OnMouseDown(ScreenToWorld(screen), button);
            RequestRedraw?.Invoke();
        }

        public void OnMouseMove(SKPoint screen, MouseButtons button)
        {
            Scene?.Camera.CurrentMouseLocation = screen;
            Scene?.Camera.CurrentCursorPoint = ScreenToWorld(screen);

            if (button == MouseButtons.Middle)
            {
                UpdatePan(screen);
                return;
            }


            StatusBarUpdated?.Invoke(UserInterfaceStateBuilder.BuildStatusBar(this, Scene));

            MainMediator?.ActiveEditorTool?.OnMouseMove(ScreenToWorld(screen), button);

            RequestRedraw?.Invoke();
        }

        public void OnMouseUp(SKPoint screen, MouseButtons button)
        {
            if (button == MouseButtons.Middle)
            {
                EndPan(button);
                return;
            }

            MainMediator?.ActiveEditorTool?.OnMouseUp(ScreenToWorld(screen), button);
            RequestRedraw?.Invoke();
        }

        public void OnMouseWheel(float delta, SKPoint screen)
        {
            
            float factor = delta > 0 ? 1.1f : 0.9f;
            Scene?.Camera.ZoomAtScreenPoint(
                Scene.Camera.Zoom * factor,
                screen);

            RequestRedraw?.Invoke();
        }

        // ---------------------------------------------
        // Camera helpers
        // ---------------------------------------------

        public void SetViewportSize(SKSize size)
        {
            _viewportSize = size;

            // Camera constraints depend on viewport size
            ClampCamera();
            RequestRedraw?.Invoke();
        }

        private SKPoint _lastPanScreen;

        private void BeginPan(SKPoint screen)
        {
            Scene?.Camera.IsPanning = true;
            _lastPanScreen = screen;
            Scene?.Camera.LastMouseMoveTime = DateTime.UtcNow;
        }

        private void UpdatePan(SKPoint screen)
        {
            if (Scene == null || !Scene.Camera.IsPanning)
                return;

            var delta = new SKPoint(
                screen.X - _lastPanScreen.X,
                screen.Y - _lastPanScreen.Y);

            Scene.Camera.PanBy(delta);

            var now = DateTime.UtcNow;
            float dt = (float)(now - Scene.Camera.LastMouseMoveTime).TotalSeconds;
            if (dt > 0)
            {
                Scene.Camera.AddVelocity(
                    new SKPoint(delta.X / dt, delta.Y / dt));
            }

            Scene.Camera.LastMouseMoveTime = now;
            _lastPanScreen = screen;
        }

        private void EndPan(MouseButtons button)
        {
            if (button == MouseButtons.Middle)
            {
                Scene?.Camera.IsPanning = false;
            }
        }

        public void ZoomAtScreenPoint(float zoomDelta, SKPoint screenPoint)
        {
            if (Scene == null)
                return;

            float newZoom = Scene.Camera.Zoom * zoomDelta;
            Scene.Camera.ZoomAtScreenPoint(newZoom, screenPoint);

            ClampCamera();
            RequestRedraw?.Invoke();
            ScheduleUiStateResolve();
        }

        public void ZoomToFit()
        {
            if (Scene == null)
                return;

            var map = Scene.Map;

            float zoomX = _viewportSize.Width / map.MapWidth;
            float zoomY = _viewportSize.Height / map.MapHeight;

            float zoom = MathF.Min(zoomX, zoomY);

            Scene.Camera.SetZoom(zoom);
            Scene.Camera.SetPan(new SKPoint(0, 0));
            ClampCamera();

            RequestRedraw?.Invoke();
            ScheduleUiStateResolve();
        }

        public void ResetCamera()
        {
            if (Scene == null)
                return;

            Scene.Camera.Reset();
            ClampCamera();

            RequestRedraw?.Invoke();
            ScheduleUiStateResolve();
        }

        private void ClampCamera()
        {
            if (Scene == null)
                return;

            Scene.Camera.ClampToWorld(
                new SKRect(0, 0,
                    Scene.Map.MapWidth,
                    Scene.Map.MapHeight),
                _viewportSize);
        }

        // ---------------------------------------------
        // Coordinate transforms
        // ---------------------------------------------

        public SKPoint ScreenToWorld(SKPoint screen)
        {
            var cam = Scene?.Camera;

            if (cam == null)
                return screen;

            return new SKPoint(
                (screen.X - cam.Pan.X) / cam.Zoom,
                (screen.Y - cam.Pan.Y) / cam.Zoom);
        }

        // -------------------------------------------------
        // Application Domain Changes
        // -------------------------------------------------

        private void ApplyLandformChanges()
        {
            ArgumentNullException.ThrowIfNull(_mainMediator, nameof(_mainMediator));
            ArgumentNullException.ThrowIfNull(_landformMediator, nameof(_landformMediator));

            if (_mainMediator.ActiveEditorTool is LandformTool landTool)
            {
                landTool.UpdateFromMediator(_mainMediator, _landformMediator);
            }

            if (SelectedShape is Landform selectedLandform)
            {
                var newShading = new LandformShadingSettings
                {
                    LandShadingDepth = _landformMediator.LandShadingDepth,
                    LandformTextureId = _landformMediator.LandformTextureId,
                    LandformOutlineColor = _landformMediator.LandOutlineColor,
                    LandformBackgroundColor = _landformMediator.LandBackgroundColor,
                    LandformOutlineWidth = _landformMediator.LandOutlineWidth,
                    FillWithTexture = _landformMediator.UseTextureBackground,
                };

                var newCoast = new CoastlineSettings
                {
                    CoastlineStyle = _landformMediator.CoastlineStyle,
                    CoastlineColor = _landformMediator.CoastlineColor,
                };

                Commands.Execute(
                    new Cmd_UpdateLandformProperties(
                        selectedLandform,
                        newShading,
                        newCoast));
            }
        }

        // -------------------------------------------------
        // IDisposable
        // -------------------------------------------------

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                try
                {
                    // Stop and dispose timer safely
                    try
                    {
                        _uiStateTimer.Stop();
                        _uiStateTimer.Tick -= UiStateTimer_Tick;
                        _uiStateTimer.Dispose();
                    }
                    catch
                    {
                        // swallow exceptions during disposal to follow Dispose pattern best-effort
                    }

                    // Unsubscribe from external events we subscribed to
                    try
                    {
                        Commands.HistoryChanged -= OnHistoryChanged;
                    }
                    catch
                    {
                    }

                }
                catch
                {
                    // Intentionally swallow; Dispose should not throw.
                }
            }

            _disposed = true;
        }
    }
}

