using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Neo.Gui.Wpf.MarkupExtensions
{
    public static class FocusHelper
    {
        #region Dependency Properties
        public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached(
            "IsFocused", 
            typeof(bool?),
            typeof(FocusHelper), 
            new FrameworkPropertyMetadata(IsFocusedChanged));

        public static bool? GetIsFocused(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (bool?)element.GetValue(IsFocusedProperty);
        }
        
        public static void SetIsFocused(DependencyObject element, bool? value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(IsFocusedProperty, value);
        }
        #endregion Dependency Properties

        #region Event Handlers
        private static void IsFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Ensure it is a FrameworkElement instance.
            if (d is FrameworkElement fe && e.OldValue == null && e.NewValue != null && (bool)e.NewValue)
            {
                // Attach to the Loaded event to set the focus there. If we do it here it will
                // be overridden by the view rendering the framework element.
                fe.Loaded += FrameworkElementLoaded;
            }
        }
        
        private static void FrameworkElementLoaded(object sender, RoutedEventArgs e)
        {
            // Ensure it is a FrameworkElement instance.
            if (!(sender is FrameworkElement fe)) return;
            
            // Remove the event handler registration.
            fe.Loaded -= FrameworkElementLoaded;
            // Set the focus to the given framework element.
            fe.Focus();
            // Determine if it is a text box like element.
            if (fe is TextBoxBase tb)
            {
                // Select all text to be ready for replacement.
                tb.SelectAll();
            }
        }
        #endregion Event Handlers
    }
}
