using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Resources;

namespace VillageSmartPOS.Services
{
    public class ThemeService
    {
        public static ThemeService Instance { get; } = new ThemeService();
        
        public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
        
        private ThemeService() { }

        public void ApplyTheme(string themeName)
        {
            try
            {
                var application = Application.Current;
                if (application == null) return;

                // Don't clear all resources, just update theme-specific ones
                // application.Resources.Clear();

                // Apply theme-specific resources
                if (themeName.Equals("Dark", StringComparison.OrdinalIgnoreCase))
                {
                    ApplyDarkTheme(application);
                }
                else
                {
                    ApplyLightTheme(application);
                }

                // Notify theme change
                ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(themeName));
                
                System.Diagnostics.Debug.WriteLine($"Theme applied: {themeName}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying theme: {ex.Message}");
            }
        }

        private void ApplyLightTheme(Application application)
        {
            // Light theme colors
            var resources = new Dictionary<string, object>();
            
            // Background colors
            resources["PrimaryBackground"] = new SolidColorBrush(Color.FromRgb(255, 255, 255)); // White
            resources["SecondaryBackground"] = new SolidColorBrush(Color.FromRgb(248, 249, 250)); // Light gray
            resources["CardBackground"] = new SolidColorBrush(Color.FromRgb(255, 255, 255)); // White
            
            // Text colors
            resources["PrimaryText"] = new SolidColorBrush(Color.FromRgb(33, 37, 41)); // Dark gray
            resources["SecondaryText"] = new SolidColorBrush(Color.FromRgb(108, 117, 125)); // Medium gray
            resources["MutedText"] = new SolidColorBrush(Color.FromRgb(134, 142, 150)); // Light gray
            
            // Border colors
            resources["BorderColor"] = new SolidColorBrush(Color.FromRgb(222, 226, 230)); // Light gray
            resources["InputBorder"] = new SolidColorBrush(Color.FromRgb(206, 212, 218)); // Medium gray
            
            // Button colors
            resources["PrimaryButton"] = new SolidColorBrush(Color.FromRgb(13, 110, 253)); // Blue
            resources["SuccessButton"] = new SolidColorBrush(Color.FromRgb(25, 135, 84)); // Green
            resources["WarningButton"] = new SolidColorBrush(Color.FromRgb(255, 193, 7)); // Yellow
            resources["DangerButton"] = new SolidColorBrush(Color.FromRgb(220, 53, 69)); // Red
            resources["InfoButton"] = new SolidColorBrush(Color.FromRgb(13, 202, 240)); // Cyan
            
            // Hover colors
            resources["PrimaryButtonHover"] = new SolidColorBrush(Color.FromRgb(10, 88, 202)); // Darker blue
            resources["SuccessButtonHover"] = new SolidColorBrush(Color.FromRgb(20, 108, 67)); // Darker green
            resources["WarningButtonHover"] = new SolidColorBrush(Color.FromRgb(255, 154, 0)); // Darker yellow
            resources["DangerButtonHover"] = new SolidColorBrush(Color.FromRgb(176, 42, 55)); // Darker red
            resources["InfoButtonHover"] = new SolidColorBrush(Color.FromRgb(10, 162, 192)); // Darker cyan
            
            // Shadow colors - use Color object for DropShadowEffect
            resources["ShadowColor"] = Color.FromRgb(0, 0, 0); // Black color for shadow
            
            // Bill Items Table colors
            resources["BillItemBackground"] = new SolidColorBrush(Color.FromRgb(240, 245, 250)); // Light blue-gray
            resources["BillItemHeaderBackground"] = new SolidColorBrush(Color.FromRgb(220, 235, 250)); // Medium blue
            resources["BillItemHoverBackground"] = new SolidColorBrush(Color.FromRgb(220, 240, 220)); // Light green
            
            // Apply resources
            foreach (var kvp in resources)
            {
                application.Resources[kvp.Key] = kvp.Value;
            }
        }

        private void ApplyDarkTheme(Application application)
        {
            // Dark theme colors
            var resources = new Dictionary<string, object>();
            
            // Background colors
            resources["PrimaryBackground"] = new SolidColorBrush(Color.FromRgb(33, 37, 41)); // Dark gray
            resources["SecondaryBackground"] = new SolidColorBrush(Color.FromRgb(52, 58, 64)); // Medium dark gray
            resources["CardBackground"] = new SolidColorBrush(Color.FromRgb(52, 58, 64)); // Medium dark gray
            
            // Text colors
            resources["PrimaryText"] = new SolidColorBrush(Color.FromRgb(248, 249, 250)); // Light gray
            resources["SecondaryText"] = new SolidColorBrush(Color.FromRgb(173, 181, 189)); // Medium light gray
            resources["MutedText"] = new SolidColorBrush(Color.FromRgb(108, 117, 125)); // Medium gray
            
            // Border colors
            resources["BorderColor"] = new SolidColorBrush(Color.FromRgb(73, 80, 87)); // Dark gray
            resources["InputBorder"] = new SolidColorBrush(Color.FromRgb(73, 80, 87)); // Dark gray
            
            // Button colors (adjusted for dark theme)
            resources["PrimaryButton"] = new SolidColorBrush(Color.FromRgb(13, 110, 253)); // Blue
            resources["SuccessButton"] = new SolidColorBrush(Color.FromRgb(25, 135, 84)); // Green
            resources["WarningButton"] = new SolidColorBrush(Color.FromRgb(255, 193, 7)); // Yellow
            resources["DangerButton"] = new SolidColorBrush(Color.FromRgb(220, 53, 69)); // Red
            resources["InfoButton"] = new SolidColorBrush(Color.FromRgb(13, 202, 240)); // Cyan
            
            // Hover colors (adjusted for dark theme)
            resources["PrimaryButtonHover"] = new SolidColorBrush(Color.FromRgb(10, 88, 202)); // Darker blue
            resources["SuccessButtonHover"] = new SolidColorBrush(Color.FromRgb(20, 108, 67)); // Darker green
            resources["WarningButtonHover"] = new SolidColorBrush(Color.FromRgb(255, 154, 0)); // Darker yellow
            resources["DangerButtonHover"] = new SolidColorBrush(Color.FromRgb(176, 42, 55)); // Darker red
            resources["InfoButtonHover"] = new SolidColorBrush(Color.FromRgb(10, 162, 192)); // Darker cyan
            
            // Shadow colors - use Color object for DropShadowEffect
            resources["ShadowColor"] = Color.FromRgb(0, 0, 0); // Black color for shadow
            
            // Bill Items Table colors (dark theme)
            resources["BillItemBackground"] = new SolidColorBrush(Color.FromRgb(65, 75, 85)); // Medium dark blue-gray
            resources["BillItemHeaderBackground"] = new SolidColorBrush(Color.FromRgb(50, 70, 90)); // Dark blue
            resources["BillItemHoverBackground"] = new SolidColorBrush(Color.FromRgb(60, 80, 60)); // Dark green
            
            // Apply resources
            foreach (var kvp in resources)
            {
                application.Resources[kvp.Key] = kvp.Value;
            }
        }

        public string GetCurrentTheme()
        {
            // Check if dark theme is applied by looking for dark background
            if (Application.Current?.Resources.Contains("PrimaryBackground") == true)
            {
                var background = Application.Current.Resources["PrimaryBackground"] as SolidColorBrush;
                if (background?.Color.R < 100 && background.Color.G < 100 && background.Color.B < 100)
                {
                    return "Dark";
                }
            }
            return "Light";
        }
    }

    public class ThemeChangedEventArgs : EventArgs
    {
        public string ThemeName { get; }
        
        public ThemeChangedEventArgs(string themeName)
        {
            ThemeName = themeName;
        }
    }
} 