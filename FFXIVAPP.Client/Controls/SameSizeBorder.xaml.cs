using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.VisualTree;

namespace FFXIVAPP.Client.Controls
{
    public class SameSizeBorder : Border, IStyleable
    {
        public SameSizeBorder()
        {
            InitializeComponent();
        }

        Type IStyleable.StyleKey => typeof(Border);

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double width = this.MinWidth, height = this.MinHeight;
            for (var i = 0; i < this.VisualChildren.Count; i++)
            {
                IVisual visual = this.VisualChildren[i];
                if (visual is ILayoutable layoutable)
                {
                    layoutable.Measure(Size.Infinity);
                    width = Math.Max(layoutable.DesiredSize.Width, width);
                    height = Math.Max(layoutable.DesiredSize.Height, height);
                }
            }

            var padding = Math.Max(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom);
            var size = Math.Max(width, height);
            return new Size(size + padding, size + padding);
        }
    }
}