using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace WPFSamples.Controls.PixelLab.Common;

using Contracts;

/// <summary>
///     Contains general purpose extension methods.
/// </summary>
public static class Extensions
{
    [Pure]
    public static bool IsNullOrWhiteSpace(this string str)
    {
        return str == null || str.Trim().Length == 0;
    }

    /// <summary>
    /// Verifies that a property name exists in this ViewModel. This method
    /// can be called before the property is used, for instance before
    /// calling RaisePropertyChanged. It avoids errors when a property name
    /// is changed but some places are missed.
    /// <para>This method is only active in DEBUG mode.</para>
    /// </summary>
    /// <param name="element">The object to watch.</param>
    /// <remarks>Thanks to Laurent Bugnion for the idea.</remarks>
    [Conditional("DEBUG")]
    [DebuggerStepThrough]
    public static void VerifyPropertyNamesOnChange(this INotifyPropertyChanged element)
    {
        Contract.Requires(element != null);
        var myType = element!.GetType();
        element.PropertyChanged += (_, args) =>
        {
            Util.ThrowUnless<InvalidOperationException>(myType.HasPublicInstanceProperty(args.PropertyName),
                "The object '{0}' of type '{1}' raised a property change for '{2}' which isn't a public property on the type."
                    .DoFormat(element, myType, args.PropertyName));
        };
    }

    public static string DoFormat(this string source, params object[] args)
    {
        Contract.Requires(source != null);
        Contract.Requires(args != null);
        return string.Format(source!, args!);
    }

    [Pure]
    public static bool HasPublicInstanceProperty(this IReflect type, string name)
    {
        Contract.Requires(type != null);
        return type!.GetProperty(name, BindingFlags.Public | BindingFlags.Instance) != null;
    }

    #region impl

    private class FuncComparer<T> : IComparer<T>
    {
        public FuncComparer(Func<T, T, int> func)
        {
            Contract.Requires(func != null);
            _func = func;
        }

        public int Compare(T x, T y)
        {
            return _func(x, y);
        }

        private readonly Func<T, T, int> _func;
    }

    private class ComparisonComparer<T> : IComparer<T>
    {
        public ComparisonComparer(Comparison<T> func)
        {
            Contract.Requires(func != null);
            _func = func;
        }

        public int Compare(T x, T y)
        {
            return _func(x, y);
        }

        private readonly Comparison<T> _func;
    }

    #endregion
}