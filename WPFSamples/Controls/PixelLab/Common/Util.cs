using System.Diagnostics;

namespace WPFSamples.Controls.PixelLab.Common;

/// <summary>
///     Contains general helper methods.
/// </summary>
public static class Util
{
    [DebuggerStepThrough]
    public static void ThrowUnless(bool truth, string message = null)
    {
        ThrowUnless<Exception>(truth, message);
    }

    [DebuggerStepThrough]
    public static void ThrowUnless<TException>(bool truth, string message) where TException : Exception
    {
        if (!truth)
        {
            throw InstanceFactory.CreateInstance<TException>(message);
        }
    }

    [DebuggerStepThrough]
    public static void ThrowUnless<TException>(bool truth) where TException : Exception, new()
    {
        if (!truth)
        {
            throw new TException();
        }
    }
}