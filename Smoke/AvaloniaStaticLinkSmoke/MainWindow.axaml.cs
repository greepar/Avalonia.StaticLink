using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;

namespace AvaloniaStaticLinkSmoke;

public sealed partial class MainWindow : Window
{
    private readonly Action<string>? _renderResult;
    private int _renderResultReported;

    public MainWindow() : this(null)
    {
    }

    public MainWindow(Action<string>? renderResult)
    {
        _renderResult = renderResult;
        InitializeComponent();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (_renderResult is not null)
        {
            context.Custom(new MetalBackendProbe(new Rect(Bounds.Size), ReportMetalResult));
        }
    }

    private void ReportMetalResult(string backend)
    {
        if (Interlocked.Exchange(ref _renderResultReported, 1) == 0)
        {
            _renderResult?.Invoke(backend);
        }
    }

    private sealed class MetalBackendProbe(Rect bounds, Action<string> report) : ICustomDrawOperation
    {
        public Rect Bounds { get; } = bounds;

        public void Dispose()
        {
        }

        public bool Equals(ICustomDrawOperation? other) => false;

        public bool HitTest(Point point) => false;

        public void Render(ImmediateDrawingContext context)
        {
            var feature = context.TryGetFeature(typeof(ISkiaSharpApiLeaseFeature)) as ISkiaSharpApiLeaseFeature;
            using var lease = feature?.Lease();
            report(lease?.GrContext?.Backend.ToString() ?? "Software");
        }
    }
}
