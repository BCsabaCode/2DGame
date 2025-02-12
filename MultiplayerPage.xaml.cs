using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using GameEngine;

namespace Game2D
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MultiplayerPage : Page
    {
        public MultiplayerPage()
        {
            InitializeComponent();
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void HostGameButtonClick(object sender, RoutedEventArgs e)
        {
            //TODO: add server starting
            Frame.Navigate(typeof(GamePage), "host");
        }

        private void ConnectButtonClick(object sender, RoutedEventArgs e)
        {
            //TODO: add client starting
            Frame.Navigate(typeof(GamePage), "client");
        }

        private void TextBoxChanged(object sender, TextChangedEventArgs e)
        {
            Config.IP = (sender as TextBox).Text;
        }
    }
}
