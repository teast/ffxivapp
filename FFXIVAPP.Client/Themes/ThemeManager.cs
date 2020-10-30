using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Styling;
using FFXIVAPP.Client.Properties;

namespace FFXIVAPP.Client.Themes
{
    public static class ThemeManager
    {
        public static List<ThemeItem> AvailableThemes => new List<ThemeItem>
        {
            Default,
            new ThemeItem("Red|Light", () => new Themes.RedLight()),
        };

        public static ThemeItem Default => new ThemeItem("Blue|Light", () => new Themes.BlueLight());
        
        private static string _currentTheme = null;
        public static void SetTheme(string themeName)
        {
            if (themeName != null && themeName == _currentTheme)
                return;

            Action<Styles> setter;
            if (Application.Current.Styles.Count == 0)
                setter = (s) => Application.Current.Styles.Add(s);
            else
                setter = (s) => Application.Current.Styles[0] = s;

            var theme = AvailableThemes.FirstOrDefault(t => t.Name == themeName);
            if (theme == null)
            {
                theme = AvailableThemes.Single(t => t.Name == "Blue|Light");
                Settings.Default.Theme = theme.Name;
            }

            setter(theme.Theme());
        }
    }

    public class ThemeItem
    {
        public string Name { get; set; }
        public Func<Styles> Theme { get; set; }

        public ThemeItem(string name, Func<Styles> theme)
        {
            Name = name;
            Theme = theme;
        }
    }
}