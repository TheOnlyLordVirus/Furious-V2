using System.Windows;

using XDevkit;

using LordVirusMw2XboxLib;

namespace FuriousXbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IXboxManager? xboxManager;
        private IXboxConsole? xboxConsole;

        private const int _maxClientCount = 18;
        private G_Client?[] CurrentGameClients = new G_Client?[_maxClientCount];

        private void Internal_RefreshClients()
        {
            if (xboxConsole is null)
                return;

            for (int clientIndex = 0; clientIndex < _maxClientCount; ++clientIndex)
            {
                if (CurrentGameClients[clientIndex] is null)
                    CurrentGameClients[clientIndex] = new G_Client(xboxConsole!, clientIndex);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (Mw2GameFunctions.TryConnectToMw2(xboxManager, out xboxConsole) && 
                    xboxConsole is not null)
                Internal_EnableWindowElements();
        }

        private void Internal_EnableWindowElements()
        {
            // TODO: Put any button, checkbox, dropdown, ect... that needs to be enabled in here when we succesfully connect.
            DebugCheatButton.IsEnabled = true;
        }

        private void DebugCheatButton_Click(object sender, RoutedEventArgs e)
        {
            Internal_RefreshClients();

            CurrentGameClients[0]?.Godmode.Enable();
        }
    }
}