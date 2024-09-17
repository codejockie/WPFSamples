using System.Diagnostics;
using WPFSamples.Controls.PixelLab.Common;

namespace WPFSamples.Controls.PixelLab.Contracts;

public static class Contract
{
    [DebuggerStepThrough]
    public static void Requires(bool truth, string message = null)
    {
        Util.ThrowUnless(truth, message);
    }

    [DebuggerStepThrough]
    public static void Requires<TException>(bool truth) where TException : Exception, new()
    {
        Util.ThrowUnless<TException>(truth);
    }
}

public class PureAttribute : Attribute
{
}

public class ContractClassAttribute : Attribute
{
    public ContractClassAttribute(Type contractType)
    {
    }
}

public class ContractClassForAttribute : Attribute
{
    public ContractClassForAttribute(Type contractForType)
    {
    }
}