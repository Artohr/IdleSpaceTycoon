using System;
public static class NumberFormatter
{
    public static string FormatCompact(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value)) return "0";
        double abs = Math.Abs(value);
        if (abs >= 1_000_000_000_000) return (value / 1_000_000_000_000D).ToString("0.##") + "T";
        if (abs >= 1_000_000_000) return (value / 1_000_000_000D).ToString("0.##") + "B";
        if (abs >= 1_000_000) return (value / 1_000_000D).ToString("0.##") + "M";
        if (abs >= 1_000) return (value / 1_000D).ToString("0.##") + "K";
        return value.ToString("0.##");
    }
}
