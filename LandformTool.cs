#nullable enable

using SkiaSharp;
using RealmStudioShapeRenderingLib;

namespace RealmStudioX
{
    public sealed class LandformTool : IToolEditor
    {
        // -------------------------------------------------
        // Dependencies
        // -------------------------------------------------

        private readonly CommandManager _commands;
        private readonly MapLayer _layer;

        // -------------------------------------------------
        // Configuration
        // -------------------------------------------------

        public float BrushRadius { get; set; } = 12f;
        public bool EraseMode { get; set; }

        public LandformShadingSettings ShadingSettings { get; private set; } = new();
        public CoastlineSettings CoastlineSettings { get; private set; } = new();

        // -------------------------------------------------
        // State
        // -------------------------------------------------

        private Landform? _activeLandform;
        private SKPoint _lastMouseWorld;
        private bool _painting;

        // -------------------------------------------------
        // Construction
        // -------------------------------------------------

        public LandformTool(
            CommandManager commands,
            MapLayer targetLayer)
        {
            _commands = commands;
            _layer = targetLayer;
        }

        public void UpdateFromMediator(MainFormUIMediator mainMediator, LandformUIMediator landformMediator)
        {
            BrushRadius = landformMediator.LandformBrushSize / 2;
            EraseMode = mainMediator.CurrentDrawingMode == MapDrawingMode.LandErase;

            ShadingSettings.LandShadingDepth = landformMediator.LandShadingDepth;
            ShadingSettings.LandformTextureId = landformMediator.LandformTextureId;
            ShadingSettings.LandformOutlineColor = landformMediator.LandOutlineColor;
            ShadingSettings.LandformBackgroundColor = landformMediator.LandBackgroundColor;
            ShadingSettings.LandformOutlineWidth = landformMediator.LandOutlineWidth;
            ShadingSettings.FillWithTexture = landformMediator.UseTextureBackground;

            CoastlineSettings.CoastlineStyle = landformMediator.CoastlineStyle;
            CoastlineSettings.CoastlineColor = landformMediator.CoastlineColor;
        }



        // -------------------------------------------------
        // Input handling
        // -------------------------------------------------


        public void OnMouseDown(SKPoint worldPos, MouseButtons button)
        {
            if (button != MouseButtons.Left)
                return;

            _lastMouseWorld = worldPos;

            if (EraseMode)
            {
                ApplyErase(worldPos);
            }
            else
            {
                BeginPaint(worldPos);
            }
        }

        public void OnMouseMove(SKPoint worldPos, MouseButtons button)
        {
            _lastMouseWorld = worldPos;

            if (!EraseMode && _activeLandform != null && button == MouseButtons.Left)
            {
                ContinuePaint(worldPos);
            }
            else if (EraseMode && button == MouseButtons.Left)
            {
                ApplyErase(worldPos);
            }
        }

        public void OnMouseUp(SKPoint worldPos, MouseButtons button)
        {
            if (button != MouseButtons.Left)
                return;

            if (!EraseMode && _activeLandform != null)
            {
                EndPaint();
            }
        }

        public void Cancel()
        {
            _activeLandform = null;
        }

        // -------------------------------------------------
        // Painting
        // -------------------------------------------------

        public void BeginPaint(SKPoint worldPos)
        {
            _activeLandform = new Landform
            {
                BrushRadius = BrushRadius,
                Shading = ShadingSettings.Clone(),
                Coastline = CoastlineSettings.Clone()
            };

            _activeLandform.BeginStroke(worldPos);
            _painting = true;
        }

        public void ContinuePaint(SKPoint worldPos)
        {
            if (!_painting || _activeLandform == null)
                return;

            _activeLandform.AddStrokePoint(worldPos);
        }

        public void EndPaint()
        {
            if (!_painting || _activeLandform == null)
                return;

            _activeLandform.EndStroke();

            _commands.Execute(new Cmd_AddShape(_layer, _activeLandform));

            _painting = false;
            _activeLandform = null;
        }

        // -------------------------------------------------
        // Erasing
        // -------------------------------------------------

        private void ApplyErase(SKPoint worldPos)
        {
            foreach (var shape in _layer.Shapes)
            {
                if (shape is not PaintedShape painted)
                    continue;

                if (!painted.Bounds.Contains(worldPos))
                    continue;

                // Wrap erasing in an undoable command
                _commands.Execute(
                    new Cmd_ErasePaintedShape(
                        painted,
                        worldPos,
                        BrushRadius)
                );
            }
        }

        // -------------------------------------------------
        // Overlay (lightweight, O(1))
        // -------------------------------------------------

        public void RenderOverlay(SKCanvas canvas)
        {
            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1f,
                IsAntialias = true,
                Color = EraseMode
                    ? new SKColor(220, 80, 80, 200)
                    : new SKColor(255, 255, 255, 200)
            };

            canvas.DrawCircle(
                _lastMouseWorld,
                BrushRadius,
                paint);
        }

        public void Activate()
        {

        }

        public void Deactivate()
        {

        }
    }
}
