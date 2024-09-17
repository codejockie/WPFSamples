using System.Collections;
using System.Windows;

namespace WPFSamples.Controls;

public partial class Carousel
{
    public Carousel()
    {
        InitializeComponent();
        DataContext = this;
    }

    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }
    
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(Carousel));
}