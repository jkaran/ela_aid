using System.Windows.Controls;

namespace Agent.Interaction.Desktop.CustomControls
{
    public class AutoCompleteBoxEx : AutoCompleteBox
    {
        private bool _selectorIsListBox = false;
        private bool _selectorSelectionChangedHandlerRegisterd = false;
        private const int SELECTOR_MAX_HEIGHT = 250;
        private TextBox _textBox;
        private ListBox _selector;

        public AutoCompleteBoxEx()
        {
            Loaded += AutoCompleteBoxExLoaded;
        }

        protected void AutoCompleteBoxExLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_selector == null)
            {
                _selector = Template.FindName("Selector", this) as ListBox;

                if (_selector != null)
                    _selectorIsListBox = true;
            }

            if (!_selectorSelectionChangedHandlerRegisterd && _selectorIsListBox && _selector != null)
            {
                _selector.SelectionChanged += ListBoxSelectionChanged;
                _selector.MaxHeight = SELECTOR_MAX_HEIGHT;
                _selectorSelectionChangedHandlerRegisterd = true;
            }
        }

        private static void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox box = ((ListBox)sender);
            box.ScrollIntoView(box.SelectedItem);
            e.Handled = true;
        }
    }
}