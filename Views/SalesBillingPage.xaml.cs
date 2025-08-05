using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VillageSmartPOS.ViewModels;
using VillageSmartPOS.Models;

namespace VillageSmartPOS.Views
{
    /// <summary>
    /// Interaction logic for SalesBillingPage.xaml
    /// </summary>
    public partial class SalesBillingPage : UserControl
    {
        private int selectedSuggestionIndex = -1;

        // Dependency property for selected suggestion
        public static readonly DependencyProperty SelectedSuggestionProperty =
            DependencyProperty.Register("SelectedSuggestion", typeof(Product), typeof(SalesBillingPage), 
                new PropertyMetadata(null, OnSelectedSuggestionChanged));

        public Product? SelectedSuggestion
        {
            get { return (Product?)GetValue(SelectedSuggestionProperty); }
            set { SetValue(SelectedSuggestionProperty, value); }
        }

        private static void OnSelectedSuggestionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SalesBillingPage page)
            {
                page.UpdateSuggestionSelection();
            }
        }

        public SalesBillingPage()
        {
            InitializeComponent();
            
            // Ensure the search textbox gets focus when the page loads
            Loaded += (s, e) => SearchTextBox.Focus();
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"SearchTextBox_KeyDown: Key={e.Key}, Focused={SearchTextBox.IsFocused}, SuggestionsCount={viewModel?.SearchSuggestions.Count ?? 0}");
            
            if (e.Key == Key.Enter)
            {
                if (selectedSuggestionIndex >= 0 && viewModel != null && selectedSuggestionIndex < viewModel.SearchSuggestions.Count)
                {
                    // If a suggestion is highlighted, select it and populate fields
                    SelectHighlightedSuggestion();
                    // Move focus to quantity field
                    QuantityTextBox.Focus();
                    QuantityTextBox.SelectAll();
                }
                else if (DataContext is SalesBillingViewModel viewModel)
                {
                    // Just search for the product - don't automatically add to bill
                    // The SearchProduct method will populate the fields
                    // Move focus to quantity field after product is found
                    if (!string.IsNullOrWhiteSpace(viewModel.Name) && !string.IsNullOrWhiteSpace(viewModel.Barcode))
                    {
                        // Product found, move focus to quantity field
                        QuantityTextBox.Focus();
                        QuantityTextBox.SelectAll();
                    }
                    System.Diagnostics.Debug.WriteLine("Searching for product...");
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                System.Diagnostics.Debug.WriteLine("Down arrow pressed - navigating suggestions");
                NavigateSuggestions(1);
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                System.Diagnostics.Debug.WriteLine("Up arrow pressed - navigating suggestions");
                NavigateSuggestions(-1);
                e.Handled = true;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Reset selection when search term changes
            selectedSuggestionIndex = -1;
            
            // Update selection after a small delay to ensure suggestions are loaded
            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateSuggestionSelection();
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void SearchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Handle arrow keys immediately when suggestions are visible
            if (viewModel?.SearchSuggestions.Count > 0 && SuggestionsBorder.Visibility == Visibility.Visible)
            {
                if (e.Key == Key.Down)
                {
                    System.Diagnostics.Debug.WriteLine("PreviewKeyDown: Down arrow pressed - navigating suggestions");
                    NavigateSuggestions(1);
                    e.Handled = true;
                    return;
                }
                else if (e.Key == Key.Up)
                {
                    System.Diagnostics.Debug.WriteLine("PreviewKeyDown: Up arrow pressed - navigating suggestions");
                    NavigateSuggestions(-1);
                    e.Handled = true;
                    return;
                }
            }
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("SearchTextBox got focus");
            // Ensure the textbox is ready for keyboard input
            SearchTextBox.SelectAll();
            
            // Check if suggestions are visible
            var suggestionsVisible = SuggestionsBorder.Visibility == Visibility.Visible;
            System.Diagnostics.Debug.WriteLine($"Suggestions visible: {suggestionsVisible}");
        }

        private void SearchTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("SearchTextBox mouse down");
            // Ensure focus is set when clicked
            SearchTextBox.Focus();
        }

        private void SuggestionsBorder_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Suggestions visibility changed: {e.NewValue}");
            if ((bool)e.NewValue)
            {
                // Suggestions are now visible, ensure we can navigate
                System.Diagnostics.Debug.WriteLine("Suggestions are now visible - ready for navigation");
            }
        }

        private void SuggestionButton_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is Button button && button.DataContext is Product product)
                {
                    if (DataContext is SalesBillingViewModel viewModel)
                    {
                        viewModel.SelectProductCommand.Execute(product);
                        // Don't automatically add to bill - let user decide
                        // viewModel.AddToBillCommand.Execute(null);
                        
                        // Move focus to quantity field
                        QuantityTextBox.Focus();
                        QuantityTextBox.SelectAll();
                    }
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                NavigateSuggestions(1);
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                NavigateSuggestions(-1);
                e.Handled = true;
            }
        }

        private void SelectHighlightedSuggestion()
        {
            if (viewModel != null && selectedSuggestionIndex >= 0 && selectedSuggestionIndex < viewModel.SearchSuggestions.Count)
            {
                var selectedProduct = viewModel.SearchSuggestions[selectedSuggestionIndex];
                viewModel.SelectProductCommand.Execute(selectedProduct);
                // Don't automatically add to bill - let user decide
                // viewModel.AddToBillCommand.Execute(null);
                
                // Move focus to quantity field
                QuantityTextBox.Focus();
                QuantityTextBox.SelectAll();
            }
        }

        private void NavigateSuggestions(int direction)
        {
            if (DataContext is not SalesBillingViewModel viewModel || viewModel.SearchSuggestions.Count == 0)
                return;

            // Update selected index
            selectedSuggestionIndex += direction;

            // Wrap around
            if (selectedSuggestionIndex >= viewModel.SearchSuggestions.Count)
                selectedSuggestionIndex = 0;
            else if (selectedSuggestionIndex < 0)
                selectedSuggestionIndex = viewModel.SearchSuggestions.Count - 1;

            // Debug output
            System.Diagnostics.Debug.WriteLine($"NavigateSuggestions: direction={direction}, selectedIndex={selectedSuggestionIndex}, suggestionsCount={viewModel.SearchSuggestions.Count}");

            // Update the selected suggestion
            if (selectedSuggestionIndex >= 0 && selectedSuggestionIndex < viewModel.SearchSuggestions.Count)
            {
                SelectedSuggestion = viewModel.SearchSuggestions[selectedSuggestionIndex];
            }
            else
            {
                SelectedSuggestion = null;
            }

            // Update visual selection with a small delay to ensure UI is ready
            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateSuggestionSelection();
                ScrollToSelectedSuggestion();
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void UpdateSuggestionSelection()
        {
            if (viewModel == null) return;

            System.Diagnostics.Debug.WriteLine($"UpdateSuggestionSelection: selectedIndex={selectedSuggestionIndex}, suggestionsCount={viewModel.SearchSuggestions.Count}");

            // Find all buttons in the ItemsControl
            var buttons = FindVisualChildren<Button>(SuggestionsItemsControl);
            var buttonList = buttons.ToList();

            System.Diagnostics.Debug.WriteLine($"Found {buttonList.Count} buttons");

            // Update the visual selection
            for (int i = 0; i < buttonList.Count; i++)
            {
                var button = buttonList[i];
                if (i == selectedSuggestionIndex)
                {
                    button.Background = new SolidColorBrush(Color.FromRgb(240, 248, 255)); // Light blue
                    button.BorderBrush = new SolidColorBrush(Color.FromRgb(33, 150, 243)); // Blue border
                    button.BorderThickness = new Thickness(1);
                    System.Diagnostics.Debug.WriteLine($"Highlighted button {i}: {button.DataContext}");
                }
                else
                {
                    button.Background = Brushes.Transparent;
                    button.BorderBrush = Brushes.Transparent;
                    button.BorderThickness = new Thickness(0);
                }
            }
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T t)
                    yield return t;

                foreach (T childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }

        private void ScrollToSelectedSuggestion()
        {
            if (selectedSuggestionIndex >= 0)
            {
                var buttons = FindVisualChildren<Button>(SuggestionsItemsControl).ToList();
                if (selectedSuggestionIndex < buttons.Count)
                {
                    var selectedButton = buttons[selectedSuggestionIndex];
                    selectedButton.BringIntoView();
                }
            }
        }

        private void QuantityTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Add to bill when Enter is pressed in quantity field
                if (DataContext is SalesBillingViewModel viewModel)
                {
                    viewModel.AddToBillCommand.Execute(null);
                    // Move focus back to search field for next item
                    SearchTextBox.Focus();
                    SearchTextBox.SelectAll();
                }
                e.Handled = true;
            }
        }

        private SalesBillingViewModel? viewModel => DataContext as SalesBillingViewModel;

        // Manual Product Entry Event Handlers
        private void ManualProductNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ManualProductPriceTextBox.Focus();
                ManualProductPriceTextBox.SelectAll();
                e.Handled = true;
            }
        }

        private void ManualProductPriceTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ManualProductQuantityTextBox.Focus();
                ManualProductQuantityTextBox.SelectAll();
                e.Handled = true;
            }
        }

        private void ManualProductQuantityTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddManualProductButton.Focus();
                e.Handled = true;
            }
        }

        private void AddManualProductButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (viewModel != null && viewModel.AddManualProductCommand.CanExecute(null))
                {
                    viewModel.AddManualProductCommand.Execute(null);
                    // Clear fields and focus back to product name
                    ManualProductNameTextBox.Focus();
                }
                e.Handled = true;
            }
        }
    }
}
