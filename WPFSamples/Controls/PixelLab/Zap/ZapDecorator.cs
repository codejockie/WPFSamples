using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFSamples.Controls.PixelLab.Common;

namespace WPFSamples.Controls.PixelLab.Zap;

public class ZapDecorator : Decorator
{
    public ZapDecorator()
    {
        _listener.Rendering += ListenerRendering;
        _listener.WireParentLoadedUnloaded(this);
    }

    public static readonly DependencyProperty TargetIndexProperty =
        DependencyProperty.Register(nameof(TargetIndex), typeof(int), typeof(ZapDecorator),
            new FrameworkPropertyMetadata(0, TargetIndexChanged));

    public int TargetIndex
    {
        get => (int)GetValue(TargetIndexProperty);
        set => SetValue(TargetIndexProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var child = Child;
        if (child == null) return new Size();
        _listener.StartListening();
        child.Measure(availableSize);

        return new Size();
    }

    #region Implementation

    private void ListenerRendering(object sender, EventArgs e)
    {
        if (Child != _zapPanel)
        {
            _zapPanel = (ZapPanel)Child;
            _zapPanel.RenderTransform = _translateTransform;
        }

        if (_zapPanel == null) return;
        var actualTargetIndex = Math.Max(0, Math.Min(_zapPanel.Children.Count - 1, TargetIndex));

        var targetPercentOffset = -actualTargetIndex / (double)_zapPanel.Children.Count;
        targetPercentOffset = double.IsNaN(targetPercentOffset) ? 0 : targetPercentOffset;

        var stopListening = !GeoHelper.Animate(
            _percentOffset, _velocity, targetPercentOffset,
            .05, .3, .1, Diff, Diff,
            out _percentOffset, out _velocity);

        var targetPixelOffset = _percentOffset * (RenderSize.Width * _zapPanel.Children.Count);
        _translateTransform.X = targetPixelOffset;

        if (stopListening)
        {
            _listener.StopListening();
        }
    }

    private static void TargetIndexChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
    {
        ((ZapDecorator)element)._listener.StartListening();
    }

    private double _velocity;
    private double _percentOffset;

    private ZapPanel _zapPanel;
    private readonly TranslateTransform _translateTransform = new();
    private readonly CompositionTargetRenderingListener _listener = new();

    private const double Diff = .00001;

    #endregion
}