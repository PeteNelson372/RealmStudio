namespace RealmStudioX
{
    public sealed class StatusBarPresenter
            : IUIPresenter<AppStatusBarUiState>
    {
        private readonly ToolStripStatusLabel? _mapSizeLabel;
        private readonly ToolStripStatusLabel? _zoomLevelLabel;
        private readonly ToolStripStatusLabel? _drawingModeLabel;
        private readonly ToolStripStatusLabel? _drawingLayerLabel;
        private readonly ToolStripStatusLabel? _drawingPointLabel;

        private readonly ToolStripStatusLabel? _appStatusMessageLabel;
        private readonly ToolStripStatusLabel? _appHelpMessageLabel;

        public StatusBarPresenter(StatusStrip drawingStatusStrip, StatusStrip appStatusStrip)
        {
            ArgumentNullException.ThrowIfNull(drawingStatusStrip);
            ArgumentNullException.ThrowIfNull(appStatusStrip);

            // DrawingStatusStrip items
            // Safe pattern match: only assign when the item exists and is the expected type.
            var item = drawingStatusStrip.Items["MapSizeLabel"];
            if (item is ToolStripStatusLabel l1)
            {
                _mapSizeLabel = l1;
            }
            else
            {
                throw new InvalidOperationException("Required ToolStripStatusLabel 'MapSizeLabel' not found in drawingStatusStrip.");
            }

            item = drawingStatusStrip.Items["ZoomLevelLabel"];
            if (item is ToolStripStatusLabel l2)
            {
                _zoomLevelLabel = l2;
            }
            else
            {
                throw new InvalidOperationException("Required ToolStripStatusLabel 'ZoomLevelLabel' not found in drawingStatusStrip.");
            }

            item = drawingStatusStrip.Items["DrawingModeLabel"];
            if (item is ToolStripStatusLabel l3)
            {
                _zoomLevelLabel = l3;
            }
            else
            {
                throw new InvalidOperationException("Required ToolStripStatusLabel 'DrawingModeLabel' not found in drawingStatusStrip.");
            }

            item = drawingStatusStrip.Items["DrawingLayerLabel"];
            if (item is ToolStripStatusLabel l4)
            {
                _drawingLayerLabel = l4;
            }
            else
            {
                throw new InvalidOperationException("Required ToolStripStatusLabel 'DrawingLayerLabel' not found in drawingStatusStrip.");
            }

            item = drawingStatusStrip.Items["DrawingPointLabel"];
            if (item is ToolStripStatusLabel l5)
            {
                _drawingPointLabel = l5;
            }
            else
            {
                throw new InvalidOperationException("Required ToolStripStatusLabel 'DrawingPointLabel' not found in drawingStatusStrip.");
            }

            // AppStatusStripItems

            item = appStatusStrip.Items["ApplicationStatusMessage"];
            if (item is ToolStripStatusLabel l6)
            {
                _appStatusMessageLabel = l6;
            }
            else
            {
                throw new InvalidOperationException("Required ToolStripStatusLabel 'ApplicationStatusMessage' not found in drawingStatusStrip.");
            }

            item = appStatusStrip.Items["ApplicationHelpMessage"];
            if (item is ToolStripStatusLabel l7)
            {
                _appHelpMessageLabel = l7;
            }
            else
            {
                throw new InvalidOperationException("Required ToolStripStatusLabel 'ApplicationStatusMessage' not found in drawingStatusStrip.");
            }

        }

        public void Apply(AppStatusBarUiState state)
        {
            _mapSizeLabel?.Text = state.MapSize;
            _zoomLevelLabel?.Text = state.ZoomLevel;
            _drawingModeLabel?.Text = state.DrawingMode;
            _drawingLayerLabel?.Text = state.DrawingLayer;
            _drawingPointLabel?.Text = state.DrawingPoint;

            _appStatusMessageLabel?.Text = state.ApplicationStatusMessage;
            _appHelpMessageLabel?.Text = state.ApplicationHelpMessage;
        }
    }

}
