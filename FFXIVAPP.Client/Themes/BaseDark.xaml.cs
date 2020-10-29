using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace FFXIVAPP.Client.Themes
{
    public class BaseDark: Styles
    {
        public BaseDark() => AvaloniaXamlLoader.Load(this);
    }
}