using System.Windows;
using System.Windows.Controls;

using XDevkit;

using LordVirusMw2XboxLib;

using FuriousXbox.XboxLib.MW2;

namespace FuriousXbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IXboxManager? xboxManager;
        private IXboxConsole? xboxConsole;

        #region Ui Client Selection Logic
        private const int _maxClientCount = 18;
        private G_Client?[] CurrentGameClients = new G_Client?[_maxClientCount];

        private G_Client? SelectedClient
        {
            get
            {
                if (ClientComboBox.SelectedValue is not G_ClientComboBoxItem g_ClientComboBox)
                    return null;

                return g_ClientComboBox.Client;
            }
        }

        private void Internal_RefreshClients()
        {
            if (xboxConsole is null)
                return;

            for (int clientIndex = 0; clientIndex < _maxClientCount; ++clientIndex)
            {
                if (CurrentGameClients[clientIndex] is null)
                {
                    CurrentGameClients[clientIndex] = new G_Client(xboxConsole!, clientIndex);

                    ClientComboBox.Items.Add(new G_ClientComboBoxItem()
                    {
                        Content = CurrentGameClients[clientIndex]?.ClientName,
                        Client = CurrentGameClients[clientIndex]
                    });

                    continue;
                }

                if (ClientComboBox.Items[clientIndex] is not G_ClientComboBoxItem g_ClientComboBoxItem)
                    continue;

                g_ClientComboBoxItem.Content = g_ClientComboBoxItem.Client?.ClientName ?? string.Empty;
            }
        }

        private void ClientComboBox_DropDownOpened(object sender, System.EventArgs e)
        {
            Internal_RefreshClients();
        }

        private void ClientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GClientNameTextBox.Text = SelectedClient?.ClientName;
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Internal_EnableWindowElements()
        {
            // TODO: Put any button, checkbox, dropdown, ect... that needs to be enabled in here when we succesfully connect.
            ClientComboBox.IsEnabled = true;
            GodModeCheatButton.IsEnabled = true;
            NoClipCheatButton.IsEnabled = true;
            UfoModeCheatButton.IsEnabled = true;
            InfAmmoCheatButton.IsEnabled = true;
            NoRecoilCheatButton.IsEnabled = true;
            ChangeGClientNameButton.IsEnabled = true;
            GClientNameTextBox.IsEnabled = true;
        }
        
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (Mw2GameFunctions.TryConnectToMw2(xboxManager, out xboxConsole))
                Internal_EnableWindowElements();
        }

        private void GodModeCheatButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedClient?.Godmode.Toggle();
        }

        private void NoClipCheatButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedClient?.NoClip.Toggle();
        }

        private void UfoModeCheatButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedClient?.UfoMode.Toggle();
        }

        private void NoRecoilCheatButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedClient?.NoRecoil.Toggle();
        }

        private void InfAmmoCheatButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedClient?.InfiniteAmmo.Toggle();
        }

        private void ChangeGClientNameButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedClient is null)
                return;

            SelectedClient!.ClientName = GClientNameTextBox.Text;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (xboxConsole is null)
                return;

            if (checkBox1.IsChecked ?? false)
                xexManager.call(xboxConsole, 0, 1);
            else
                xexManager.call(xboxConsole, 0, 0);

            if (xexManager.callBack(xboxConsole, 0))
                 MessageBox.Show("xex Callback");
        }
    }
}