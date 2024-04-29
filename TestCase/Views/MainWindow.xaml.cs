using System.Windows;
using System.Windows.Controls;

namespace TestCase;

public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();
    }

    private void dataGrid_Scroll(object sender, ScrollChangedEventArgs e)
    {
        if (e.ExtentHeight < e.ViewportHeight)
            return;

        if (Grid.Items.Count <= 0)
            return;

        if (e is { ExtentHeightChange: 0.0, ViewportHeightChange: 0.0 })
            return;

        // Если близки к низу, то прокручиваем вниз.
        var oldExtentHeight = e.ExtentHeight - e.ExtentHeightChange;
        var oldVerticalOffset = e.VerticalOffset - e.VerticalChange;
        var oldViewportHeight = e.ViewportHeight - e.ViewportHeightChange;
        if (oldVerticalOffset + oldViewportHeight + 5 >= oldExtentHeight)
        {
            var lastElement = Grid.Items[^1];
            if (lastElement != null)
            {
                Grid.ScrollIntoView(lastElement);
            }
        }
    }
}