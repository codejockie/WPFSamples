using System.Windows;
using System.Windows.Controls;

namespace WPFSamples.Controls;

public class FlexPanel : Panel
{
    // Attached property: Should a child control flex/expand to use the remaining available space
    public static bool GetFlex(DependencyObject obj) => (bool)obj.GetValue(FlexProperty);
    public static void SetFlex(DependencyObject obj, bool value) => obj.SetValue(FlexProperty, value);

    public static readonly DependencyProperty FlexProperty =
        DependencyProperty.RegisterAttached("Flex", typeof(bool), typeof(FlexPanel), new PropertyMetadata(false));

    // Attached property: How many parts of the available space does the child control want
    public static int GetFlexWeight(DependencyObject obj) => (int)obj.GetValue(FlexWeightProperty);
    public static void SetFlexWeight(DependencyObject obj, int value) => obj.SetValue(FlexWeightProperty, value);

    public static readonly DependencyProperty FlexWeightProperty =
        DependencyProperty.RegisterAttached("FlexWeight", typeof(int), typeof(FlexPanel), new PropertyMetadata(1));

    // Dependency property: Which way should child elements stack
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation),
        typeof(Orientation), typeof(FlexPanel), new PropertyMetadata(Orientation.Vertical));

    protected override Size MeasureOverride(Size availableSize) // The panel's passed the space available to it
    {
        var desiredSize = new Size(); // For tracking the minimum size of the panel
        foreach (UIElement child in Children)
        {
            child.Measure(new Size(double.PositiveInfinity,
                double.PositiveInfinity)); // Make it report the smallest desired size

            if (Orientation == Orientation.Vertical)
            {
                // Add the child control's height to the minimum desired height
                desiredSize.Height += child.DesiredSize.Height;
                // Check if this child control is the widest yet
                // and if it is, assign it the minimum desired width
                desiredSize.Width = Math.Max(desiredSize.Width, child.DesiredSize.Width);
            }
            else
            {
                // Add the child control's width to the minimum desired width
                desiredSize.Width += child.DesiredSize.Width;
                // Check if this child control is the highest yet
                // and if it is, assign it the minimum desired height
                desiredSize.Height = Math.Max(desiredSize.Height, child.DesiredSize.Height);
            }
        }

        if (double.IsPositiveInfinity(availableSize.Height) || double.IsPositiveInfinity(availableSize.Width))
            return desiredSize; // If the available size is infinite, return the minimum size
        return availableSize; // Return that it wants to use all the space available
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var currentLength = 0d; // The offset to lay out the next control
        var totalLength = 0d; // The length used by the non-flexing controls
        var flexChildrenWeightParts = 0; // How many parts to divide the leftover space into

        if (Orientation == Orientation.Vertical)
        {
            foreach (UIElement child in Children)
            {
                if (GetFlex(child)) // If the control should flex, add its weight to the number of parts to divide the remaining space into
                    flexChildrenWeightParts += GetFlexWeight(child);
                else // For non-flex controls, add their height to the total length used by non-flexing controls
                    totalLength += child.DesiredSize.Height;
            }

            // Define how large one part of the remaining size is
            var flexSize = Math.Max(0, (finalSize.Height - totalLength) / flexChildrenWeightParts);

            foreach (UIElement child in Children)
            {
                var arrangeRect =
                    GetFlex(child) // If the control should flex, make its height the flexSize times it's FlexWeight
                        ? new Rect(0, currentLength, finalSize.Width, flexSize * GetFlexWeight(child))
                        : // For non-flexing controls, make it only as high as it needs.
                        new Rect(0, currentLength, finalSize.Width, child.DesiredSize.Height);

                child.Arrange(arrangeRect); // Tell the child to position itself according to the Rect
                currentLength += arrangeRect.Height; // Keep track of where to position the next control
            }
        }
        else
        {
            foreach (UIElement child in Children)
            {
                if (GetFlex(child)) // If the control should flex, add it's weight to the number of parts to divide the remaining space into
                    flexChildrenWeightParts += GetFlexWeight(child);
                else // For non-flex controls, add their width to the total length used by non-flexing controls
                    totalLength += child.DesiredSize.Width;
            }

            // Define how large one part of the remaining size is
            var flexSize = Math.Max(0, (finalSize.Width - totalLength) / flexChildrenWeightParts);

            foreach (UIElement child in Children)
            {
                Rect arrangeRect;
                if (GetFlex(child))
                    arrangeRect =
                        new Rect(currentLength, 0, flexSize * GetFlexWeight(child),
                            finalSize
                                .Height); // If the control should flex, make it's width the flexSize times it's FlexWeight
                else
                    arrangeRect =
                        new Rect(currentLength, 0, child.DesiredSize.Width,
                            finalSize.Height); // For non-flexing controls, make it only as wide as it needs.

                child.Arrange(arrangeRect); // Tell the child to position itself according to the Rect
                currentLength += arrangeRect.Width; // Keep track of where to position the next control
            }
        }

        return finalSize; // Always use as much space as possible
    }
}