using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Game2D
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainMenuPage : Page
    {
        public MainMenuPage()
        {
            InitializeComponent();

            //TODO: handle this better
            ApplicationView.PreferredLaunchViewSize = new Size(1366, 768);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }

        private void SinglePlayerButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GamePage), "single");
        }

        private void MultiPlayerButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MultiplayerPage));
        }

        private void SettingsButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}
