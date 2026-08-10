namespace MarketAutomation2.Desktop.Helpers
{
    public static class QuantityHelper
    {
        public static decimal GetScanIncrement(string unit)
        {
            return IsWeightUnit(unit) ? 1m : 1m;
        }

        public static decimal GetStep(string unit)
        {
            return unit?.Trim().ToLowerInvariant() switch
            {
                "kg" or "kilogram" => 0.250m,
                "gram" or "gr" => 100m,
                "litre" or "lt" or "l" => 0.250m,
                _ => 1m
            };
        }

        public static bool IsWeightUnit(string unit)
        {
            return unit?.Trim().ToLowerInvariant() is "kg" or "kilogram" or "gram" or "gr"
                or "litre" or "lt" or "l";
        }

        public static string FormatQuantity(decimal quantity, string unit)
        {
            if (IsWeightUnit(unit))
                return $"{quantity:N3} {unit}";

            return $"{quantity:N0} {unit}";
        }

        public static string FormatUnitPrice(decimal price, string unit)
        {
            if (IsWeightUnit(unit))
                return $"{price:N2} ₺/{unit}";

            return $"{price:N2} ₺";
        }
    }
}
