using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FFXIVAPP.Client.Controls
{
    public class Badge : UserControl
    {
        public static readonly DirectProperty<Badge, string> BadgeTextProperty =
            AvaloniaProperty.RegisterDirect<Badge, string>(
                nameof(BadgeText),
                o => o.BadgeText,
                (o, v) => o.BadgeText = v );

        private string _badgeText;
        public string BadgeText
        {
            get { return _badgeText; }
            set { SetAndRaise(BadgeTextProperty, ref _badgeText, value); }
        }

        public Badge()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}