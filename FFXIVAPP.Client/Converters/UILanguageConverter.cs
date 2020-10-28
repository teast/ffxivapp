using System;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using FFXIVAPP.Client.Models;

namespace FFXIVAPP.Client.Converters
{
    /// <summary>
    /// Converts between <see cref="UILanguage" /> and the languages name.
    /// </summary>
    public class UILanguageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string language)
            {
                var result = AppViewModel.Instance.UILanguages.FirstOrDefault(l => l.Language == language) ?? value;
                return result;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UILanguage language)
            {
                return language.Language;
            }

            return value;
        }
    }
}