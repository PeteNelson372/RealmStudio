using SkiaSharp;

namespace RealmStudioX
{
    public interface IToolEditor
    {
        void Activate();
        void Deactivate();

        void OnMouseDown(SKPoint worldPos, MouseButtons button);
        void OnMouseMove(SKPoint worldPos, MouseButtons button);
        void OnMouseUp(SKPoint worldPos, MouseButtons button);

        void Cancel();

        void RenderOverlay(SKCanvas canvas);
    }
}
