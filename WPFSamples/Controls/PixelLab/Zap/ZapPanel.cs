using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFSamples.Controls.PixelLab.Zap;

public class ZapPanel : Panel
{
    public ZapPanel()
    {
        LayoutUpdated += ZapPanel_LayoutUpdated;
    }

    protected override Geometry GetLayoutClip(Size layoutSlotSize)
    {
        return null;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        //if the parent isn't a FrameworkElement, then measure all the children to infinity
        //and return a size that would result if all of them were as big as the biggest

        //otherwise, measure all at the size of the parent and return the size accordingly

        var max = new Size();

        for (var i = 0; i < InternalChildren.Count; i++)
        {
            var child = InternalChildren[i];
            child.Measure(availableSize);
            max.Width = Math.Max(max.Width, child.DesiredSize.Width);
            max.Height = Math.Max(max.Height, child.DesiredSize.Height);
        }

        //for now, keep with horizontal orientation
        Size returnSize;
        if (double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height))
        {
            returnSize = new Size(max.Width * InternalChildren.Count, max.Height);
        }
        else
        {
            returnSize = new Size(availableSize.Width * InternalChildren.Count, availableSize.Height);
        }

        return returnSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _lastVisualParentSize = _visualParent?.RenderSize ?? finalSize;

        for (var i = 0; i < InternalChildren.Count; i++)
        {
            var child = InternalChildren[i];
            var arrangeRect = new Rect(new Point(_lastVisualParentSize.Width * i, 0), _lastVisualParentSize);
            child.Arrange(arrangeRect);
        }

        return new Size(_lastVisualParentSize.Width * InternalChildren.Count, _lastVisualParentSize.Height);
    }

    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
        base.OnVisualParentChanged(oldParent);

        _visualParent = VisualParent as FrameworkElement;
    }

    #region implementation

    private void ZapPanel_LayoutUpdated(object sender, EventArgs e)
    {
        if (_visualParent == null) return;
        if (_visualParent.RenderSize != _lastVisualParentSize)
        {
            InvalidateArrange();
        }
    }

    private Size _lastVisualParentSize;
    private FrameworkElement _visualParent;

    #endregion
}