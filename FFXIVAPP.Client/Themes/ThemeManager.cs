using System;
using Avalonia;
using Avalonia.Styling;
using FFXIVAPP.Client.Properties;

namespace FFXIVAPP.Client.Themes
{
    public static class ThemeManager
    {
        private static string _currentTheme = null;
        public static void SetTheme(string theme)
        {
            if (theme != null && theme == _currentTheme)
                return;

            Action<Styles> setter;
            if (Application.Current.Styles.Count == 0)
                setter = (s) => Application.Current.Styles.Add(s);
            else
                setter = (s) => Application.Current.Styles[0] = s;

            switch(theme)
            {
                case "BaseDark":
                    setter(new Themes.BaseDark());
                    _currentTheme = "BaseDark";
                    break;
                case "BaseLight":
                    setter(new Themes.BaseLight());
                    _currentTheme = "BaseLight";
                    break;
                default:
                    setter(new Themes.BaseLight());
                    _currentTheme = "BaseLight";
                    Settings.Default.Theme = "BaseLight";
                    break;
            }
        }
    }
}