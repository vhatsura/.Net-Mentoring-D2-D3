using System;

namespace MemoryLeakExample
{
    public static class Settings
    {
        public static event Action<string> OnCurrencyChanged;

        public static void ChangeCurrency(string newCurrency)
        {
            //...
            OnCurrencyChanged?.Invoke(newCurrency);
        }
    }
}
