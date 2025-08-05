using System.Configuration;
using System.Data;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media;
using VillageSmartPOS.Services;

namespace VillageSmartPOS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
            protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Initialize with light theme by default
        ThemeService.Instance.ApplyTheme("Light");
        
        // Initialize with English language by default
        LanguageService.Instance.ApplyLanguage("English");
        
        // Debug: Check if resources are applied
        System.Diagnostics.Debug.WriteLine($"SalesBilling resource: {Application.Current.Resources["SalesBilling"]}");
        System.Diagnostics.Debug.WriteLine($"BillItems resource: {Application.Current.Resources["BillItems"]}");
        System.Diagnostics.Debug.WriteLine($"Total resource: {Application.Current.Resources["Total"]}");
        
        // Force a complete UI refresh after resources are applied
        System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
        {
            // Force refresh of all UI elements
            if (MainWindow != null)
            {
                MainWindow.InvalidateVisual();
                MainWindow.UpdateLayout();
                
                // Force refresh of all child elements recursively
                RefreshAllElements(MainWindow);
                
                // Force a second refresh after a short delay
                System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                {
                    if (MainWindow != null)
                    {
                        MainWindow.InvalidateVisual();
                        MainWindow.UpdateLayout();
                        RefreshAllElements(MainWindow);
                    }
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }), System.Windows.Threading.DispatcherPriority.Loaded);
    }
    
    private static void RefreshAllElements(FrameworkElement element)
    {
        if (element == null) return;
        
        // Force refresh of this element
        element.InvalidateVisual();
        element.UpdateLayout();
        
        // Recursively refresh all child elements
        var count = VisualTreeHelper.GetChildrenCount(element);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;
            if (child != null)
            {
                RefreshAllElements(child);
            }
        }
    }
    
    private static IEnumerable<FrameworkElement> GetVisualChildren(FrameworkElement parent)
    {
        var children = new List<FrameworkElement>();
        var count = VisualTreeHelper.GetChildrenCount(parent);
        
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
            if (child != null)
            {
                children.Add(child);
                children.AddRange(GetVisualChildren(child));
            }
        }
        
        return children;
    }
    }

}
