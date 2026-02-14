#nullable enable

using SkiaSharp;
using RealmStudioShapeRenderingLib;

namespace RealmStudioX
{
    public sealed class PaintedShapeTool : IToolEditor
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

        // -------------------------------------------------
        // State
        // -------------------------------------------------

        private PaintedShape? _activePaintShape;
        private SKPoint _lastMouseWorld;

        // -------------------------------------------------
        // Construction
        // -------------------------------------------------

        public PaintedShapeTool(
            CommandManager commands,
            MapLayer targetLayer)
        {
            _commands = commands;
            _layer = targetLayer;
        }

        // -------------------------------------------------
        // Lifecycle
        // -------------------------------------------------

        public void Activate()
        {
            _activePaintShape = null;
        }

        public void Deactivate()
        {
            Cancel();
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

            if (!EraseMode && _activePaintShape != null)
            {
                _activePaintShape.AddStrokePoint(worldPos);
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

            if (!EraseMode && _activePaintShape != null)
            {
                EndPaint();
            }
        }

        public void Cancel()
        {
            _activePaintShape = null;
        }

        // -------------------------------------------------
        // Painting
        // -------------------------------------------------

        private void BeginPaint(SKPoint worldPos)
        {
            _activePaintShape = new PaintedShape
            {
                BrushRadius = BrushRadius
            };

            _activePaintShape.BeginStroke(worldPos);
        }

        private void EndPaint()
        {
            if (_activePaintShape == null)
                return;

            _activePaintShape.EndStroke();

            // One undo entry per completed paint stroke
            _commands.Execute(
                new Cmd_AddShape(_layer, _activePaintShape)
            );

            _activePaintShape = null;
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
    }
}
