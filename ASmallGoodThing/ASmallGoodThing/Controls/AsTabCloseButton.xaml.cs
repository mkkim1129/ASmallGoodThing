using System;
using System.Windows;
using System.Windows.Controls;

namespace mkkim1129.ASmallGoodThing.Controls
{
    /// <summary>
    /// Interaction logic for AsTabCloseButton.xaml
    /// </summary>
    public partial class AsTabCloseButton : UserControl
    {
        public event EventHandler Click;

        public AsTabCloseButton()
        {
            InitializeComponent();
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (Click != null)
            {
                Click(sender, e);
            }
        }
    }
}
