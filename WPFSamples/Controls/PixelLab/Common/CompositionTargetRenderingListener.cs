#define _FRAME_RATE

using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using WPFSamples.Controls.PixelLab.Contracts;

namespace WPFSamples.Controls.PixelLab.Common;

public class CompositionTargetRenderingListener : IDisposable
{
    public void StartListening()
    {
        RequireAccessAndNotDisposed();

        if (_isListening) return;
        IsListening = true;
        CompositionTarget.Rendering += compositionTarget_Rendering;
#if FRAME_RATE
        _startTicks = Environment.TickCount;
        _count = 0;
#endif
    }

    public void StopListening()
    {
        RequireAccessAndNotDisposed();

        if (!_isListening) return;
        IsListening = false;
        CompositionTarget.Rendering -= compositionTarget_Rendering;
#if FRAME_RATE
        var ticks = Environment.TickCount - _startTicks;
        var seconds = ticks / 1000.0;
        Debug.WriteLine("Seconds: {0}, Count: {1}, Frame rate: {2}", seconds, _count, _count/seconds);
#endif
    }

#if !WP7
    public void WireParentLoadedUnloaded(FrameworkElement parent)
    {
        Contract.Requires(parent != null);
        RequireAccessAndNotDisposed();

        parent!.Loaded += delegate { StartListening(); };

        parent.Unloaded += delegate { StopListening(); };
    }
#endif

    public bool IsListening
    {
        get => _isListening;
        private set
        {
            if (value == _isListening) return;
            _isListening = value;
            OnIsListeningChanged(EventArgs.Empty);
        }
    }

    public bool IsDisposed => _isDisposed;

    public event EventHandler Rendering;

    protected virtual void OnRendering(EventArgs args)
    {
        RequireAccessAndNotDisposed();

        var handler = Rendering;
        handler?.Invoke(this, args);
    }

    public event EventHandler IsListeningChanged;

    protected virtual void OnIsListeningChanged(EventArgs args)
    {
        var handler = IsListeningChanged;
        handler?.Invoke(this, args);
    }

    public void Dispose()
    {
        RequireAccessAndNotDisposed();
        StopListening();

        Rendering
            ?.GetInvocationList()
            .ForEach(d => Rendering -= (EventHandler)d);

        _isDisposed = true;
    }

    #region Implementation

    [DebuggerStepThrough]
    private void RequireAccessAndNotDisposed()
    {
        Util.ThrowUnless<ObjectDisposedException>(!_isDisposed, "This object has been disposed");
    }

    private void compositionTarget_Rendering(object sender, EventArgs e)
    {
#if FRAME_RATE
      _count++;
#endif
        OnRendering(e);
    }

    private bool _isListening;
    private bool _isDisposed;

#if FRAME_RATE
    private int _count = 0;
    private int _startTicks;
#endif

    #endregion
}