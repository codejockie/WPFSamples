using System.Diagnostics;

namespace WPFSamples.Controls.PixelLab.Common;

using Contracts;

public static class GeoHelper
{
    [Pure]
    public static bool IsValid(this double value)
    {
        return !double.IsInfinity(value) && !double.IsNaN(value);
    }

    public static bool Animate(
        double currentValue, double currentVelocity, double targetValue,
        double attractionFactor, double dampening,
        double terminalVelocity, double minValueDelta, double minVelocityDelta,
        out double newValue, out double newVelocity)
    {
        Debug.Assert(currentValue.IsValid());
        Debug.Assert(currentVelocity.IsValid());
        Debug.Assert(targetValue.IsValid());

        Debug.Assert(dampening.IsValid());
        Debug.Assert(dampening > 0 && dampening < 1);

        Debug.Assert(attractionFactor.IsValid());
        Debug.Assert(attractionFactor > 0);

        Debug.Assert(terminalVelocity > 0);

        Debug.Assert(minValueDelta > 0);
        Debug.Assert(minVelocityDelta > 0);

        var diff = targetValue - currentValue;

        if (diff.Abs() > minValueDelta || currentVelocity.Abs() > minVelocityDelta)
        {
            newVelocity = currentVelocity * (1 - dampening);
            newVelocity += diff * attractionFactor;
            if (currentVelocity.Abs() > terminalVelocity)
            {
                newVelocity *= terminalVelocity / currentVelocity.Abs();
            }

            newValue = currentValue + newVelocity;

            return true;
        }

        newValue = targetValue;
        newVelocity = 0;
        return false;
    }

    public static double Abs(this double value)
    {
        return Math.Abs(value);
    }
}