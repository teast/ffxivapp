using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace FFXIVAPP.Client.Themes
{
    public class BaseLight: Styles
    {
        public BaseLight() => AvaloniaXamlLoader.Load(this);
    }
}