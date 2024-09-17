using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace WPFSamples.Controls.PixelLab.Zap;

public class ZapScroller : ItemsControl
{
    static ZapScroller()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(ZapScroller),
            new FrameworkPropertyMetadata(typeof(ZapScroller)));

        FocusableProperty.OverrideMetadata(
            typeof(ZapScroller),
            new FrameworkPropertyMetadata(false));
    }

    public ZapScroller()
    {
        _firstCommand = new DelegateCommand(First, CanFirst);
        _previousCommand = new DelegateCommand(Previous, CanPrevious);
        _nextCommand = new DelegateCommand(Next, CanNext);
        _lastCommand = new DelegateCommand(Last, CanLast);
    }

    public ICommand FirstCommand => _firstCommand;

    public ICommand PreviousCommand => _previousCommand;

    public ICommand NextCommand => _nextCommand;

    public ICommand LastCommand => _lastCommand;

    public ReadOnlyObservableCollection<ZapCommandItem> Commands => new(_commandItems);

    public static readonly DependencyProperty CommandItemTemplateProperty =
        DependencyProperty.Register(nameof(CommandItemTemplate), typeof(DataTemplate), typeof(ZapScroller));

    public DataTemplate CommandItemTemplate
    {
        get => (DataTemplate)GetValue(CommandItemTemplateProperty);
        set => SetValue(CommandItemTemplateProperty, value);
    }

    public static readonly DependencyProperty CommandItemTemplateSelectorProperty =
        DependencyProperty.Register(nameof(CommandItemTemplateSelector), typeof(DataTemplateSelector),
            typeof(ZapScroller));

    public DataTemplateSelector CommandItemTemplateSelector
    {
        get => (DataTemplateSelector)GetValue(CommandItemTemplateSelectorProperty);
        set => SetValue(CommandItemTemplateSelectorProperty, value);
    }

    private static readonly DependencyPropertyKey ItemCountPropertyKey =
        DependencyProperty.RegisterReadOnly(nameof(ItemCount),
            typeof(int), typeof(ZapScroller), new PropertyMetadata(0));

    public static readonly DependencyProperty ItemCountProperty = ItemCountPropertyKey.DependencyProperty;

    private int ItemCount => (int)GetValue(ItemCountProperty);

    public static readonly DependencyProperty CurrentItemIndexProperty =
        DependencyProperty.Register(nameof(CurrentItemIndex), typeof(int), typeof(ZapScroller),
            new PropertyMetadata(currentItemIndex_changed));

    public int CurrentItemIndex
    {
        get => (int)GetValue(CurrentItemIndexProperty);
        set => SetValue(CurrentItemIndexProperty, value);
    }

    private void First()
    {
        if (CanFirst())
        {
            CurrentItemIndex = 0;
        }
    }

    private void Previous()
    {
        if (CanPrevious())
        {
            CurrentItemIndex--;
        }
    }

    private void Next()
    {
        if (CanNext())
        {
            CurrentItemIndex++;
        }
    }

    private void Last()
    {
        if (CanLast())
        {
            CurrentItemIndex = ItemCount - 1;
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        FindZapDecorator();

        return base.MeasureOverride(availableSize);
    }

    private static readonly RoutedEvent CurrentItemIndexChangedEvent =
        EventManager.RegisterRoutedEvent("CurrentItemIndexChanged", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<int>), typeof(ZapScroller));

    public event RoutedPropertyChangedEventHandler<int> CurrentItemChanged
    {
        add => AddHandler(CurrentItemIndexChangedEvent, value);
        remove => RemoveHandler(CurrentItemIndexChangedEvent, value);
    }

    protected virtual void OnCurrentItemIndexChanged(int oldValue, int newValue)
    {
        ResetEdgeCommands();
        var args = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue)
        {
            RoutedEvent = CurrentItemIndexChangedEvent
        };
        RaiseEvent(args);
    }

    protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
    {
        base.OnItemsSourceChanged(oldValue, newValue);

        var newItems = Items;

        if (newItems != _internalItemCollection)
        {
            _internalItemCollection = newItems;

            ResetProperties();
        }
    }

    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnItemsChanged(e);

        if (Items != _internalItemCollection)
        {
            _internalItemCollection = Items;
        }

        ResetProperties();
    }

    #region Implementation

    private static void currentItemIndex_changed(DependencyObject element, DependencyPropertyChangedEventArgs e)
    {
        var zapScroller = (ZapScroller)element;
        zapScroller.OnCurrentItemIndexChanged((int)e.OldValue, (int)e.NewValue);
    }

    private void ResetEdgeCommands()
    {
        _firstCommand.RaiseCanExecuteChanged();
        _lastCommand.RaiseCanExecuteChanged();
        _nextCommand.RaiseCanExecuteChanged();
        _previousCommand.RaiseCanExecuteChanged();
    }

    private void ResetCommands()
    {
        ResetEdgeCommands();

        var parentItemsCount = ItemCount;

        if (parentItemsCount != _commandItems.Count)
        {
            if (parentItemsCount > _commandItems.Count)
            {
                for (var i = _commandItems.Count; i < parentItemsCount; i++)
                {
                    _commandItems.Add(new ZapCommandItem(this, i));
                }
            }
            else
            {
                Debug.Assert(parentItemsCount < _commandItems.Count);
                var delta = _commandItems.Count - parentItemsCount;
                for (var i = 0; i < delta; i++)
                {
                    _commandItems.RemoveAt(_commandItems.Count - 1);
                }
            }
        }

        Debug.Assert(Items.Count == _commandItems.Count);

        for (var i = 0; i < parentItemsCount; i++)
        {
            _commandItems[i].Content = Items[i];
        }

#if DEBUG
        for (var i = 0; i < _commandItems.Count; i++)
        {
            Debug.Assert(_commandItems[i].Index == i);
        }
#endif
    }

    private void FindZapDecorator()
    {
        if (Template != null)
        {
            var temp = Template.FindName(PartZapDecorator, this) as ZapDecorator;
            if (_zapDecorator != temp)
            {
                _zapDecorator = temp;
                if (_zapDecorator == null) return;
                var binding = new Binding("CurrentItemIndex")
                {
                    Source = this
                };
                _zapDecorator.SetBinding(ZapDecorator.TargetIndexProperty, binding);
            }
            else
            {
                Debug.WriteLine("No element with name '" + PartZapDecorator + "' in the template.");
            }
        }
        else
        {
            Debug.WriteLine("No template defined for ZapScroller.");
        }
    }

    private void ResetProperties()
    {
        if (_internalItemCollection.Count != ItemCount)
        {
            SetValue(ItemCountPropertyKey, _internalItemCollection.Count);
        }

        if (CurrentItemIndex >= ItemCount)
        {
            CurrentItemIndex = ItemCount - 1;
        }
        else if (CurrentItemIndex == -1 && ItemCount > 0)
        {
            CurrentItemIndex = 0;
        }

        ResetCommands();
    }

    private bool CanFirst()
    {
        return ItemCount > 1 && CurrentItemIndex > 0;
    }

    private bool CanNext()
    {
        return CurrentItemIndex >= 0 && CurrentItemIndex < ItemCount - 1;
    }

    private bool CanPrevious()
    {
        return CurrentItemIndex > 0;
    }

    private bool CanLast()
    {
        return ItemCount > 1 && CurrentItemIndex < ItemCount - 1;
    }

    private ItemCollection _internalItemCollection;

    private ZapDecorator _zapDecorator;

    private readonly DelegateCommand _firstCommand, _previousCommand, _nextCommand, _lastCommand;

    private readonly ObservableCollection<ZapCommandItem> _commandItems = [];

    #endregion

    private const string PartZapDecorator = "PART_ZapDecorator";
}