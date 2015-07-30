using System.Windows.Controls;

namespace mkkim1129.ASmallGoodThing.Controls
{
    public class AsCloseableTabItem : TabItem
    {
        private string headerText_;

        public string HeaderText
        {
            get
            {
                return headerText_;
            }
        }

        public void SetHeader(string headerText)
        {
            headerText_ = headerText;

            TextBlock header = new TextBlock { Text = headerText };
            var panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;
            panel.Children.Add(header);

            var closeButton = new AsTabCloseButton();
            var margin = closeButton.Margin;
            margin.Left += 5;
            closeButton.Margin = margin;
            closeButton.Click += (sender, e) =>
                {
                    var tabControl = Parent as ItemsControl;
                    tabControl.Items.Remove(this);
                };
            panel.Children.Add(closeButton);

            Header = panel;
        }
    }
}
