using System.ComponentModel;
using System.Windows.Input;
using WPFSamples.Controls.PixelLab.Common;

namespace WPFSamples.Controls.PixelLab.Zap;

using Contracts;

public class ZapCommandItem : Changeable, ICommand
{
    protected internal ZapCommandItem(ZapScroller zapScroller, int index)
    {
        Contract.Requires<ArgumentNullException>(zapScroller != null);
        Contract.Requires<ArgumentOutOfRangeException>(index >= 0);

        _zapScroller = zapScroller;

        _zapScroller!.CurrentItemChanged += delegate
        {
            OnCanExecuteChanged(EventArgs.Empty);
        };

        _index = index;
        _content = _zapScroller.Items[_index];
    }

    public object Content
    {
        get => _content;
        protected internal set
        {
            if (_content == value) return;
            _content = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Content)));
        }
    }

    public int Index => _index;

    /// <remarks>
    ///     For public use. Most people don't like zero-base indices.
    /// </remarks>
    public int Number => _index + 1;

    public bool CanExecute => _index != _zapScroller.CurrentItemIndex;

    bool ICommand.CanExecute(object parameter)
    {
        return CanExecute;
    }

    public event EventHandler CanExecuteChanged;

    protected virtual void OnCanExecuteChanged(EventArgs e)
    {
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(CanExecute)));

        var handler = CanExecuteChanged;
        handler?.Invoke(this, e);
    }

    private void MakeCurrent()
    {
        _zapScroller.CurrentItemIndex = _index;
    }

    void ICommand.Execute(object parameter)
    {
        MakeCurrent();
    }

    public override string ToString()
    {
        var output = Content == null ? "*null*" : Content.ToString();

        return $"ZapCommandItem - Index: {_index}, Content: {output}";
    }

    #region Implementation

    private object _content;

    private readonly int _index;
    private readonly ZapScroller _zapScroller;

    #endregion
}