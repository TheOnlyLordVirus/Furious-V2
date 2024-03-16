using System.Windows;
using System.Windows.Controls;

using XDevkit;

using LordVirusMw2XboxLib;

using FuriousXbox.XboxLib.MW2;
using XDRPC;

namespace FuriousXbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IXboxManager? xboxManager;
        private IXboxConsole? xboxConsole;

        private readonly Task?[] unlockAllTasks = new Task?[Mw2XboxLibConstants.MaxClientCount];
        private readonly G_Client?[] CurrentGameClients = new G_Client?[Mw2XboxLibConstants.MaxClientCount];

        private G_Client? SelectedClient
        {
            get
            {
                if (ClientComboBox.SelectedValue is not G_ClientComboBoxItem g_ClientComboBox)
                    return null;

                return g_ClientComboBox.Client;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Internal_InitDebugComboBox();
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
            ThermalRedBoxesCheatButton.IsEnabled = true;
            UnlockAllCheatButton.IsEnabled = true;
            DebugButton.IsEnabled = true;
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

        private void ThermalRedBoxesCheatButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedClient?.ThermalRedboxes.Toggle();
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

            if (XexManager.callBack(xboxConsole, 0))
                MessageBox.Show("xex Callback");
        }

        private void UnlockAllCheatButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedClient is null)
                return;

            if (unlockAllTasks[SelectedClient.ClientIndex] is not null &&
                !(unlockAllTasks[SelectedClient.ClientIndex]!.IsCompleted))
                return;

            unlockAllTasks[SelectedClient.ClientIndex] = SelectedClient.UnlockAll();
        }

        private void Internal_RefreshClients()
        {
            if (xboxConsole is null)
                return;

            for (int clientIndex = 0; clientIndex < Mw2XboxLibConstants.MaxClientCount; ++clientIndex)
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

        private void DebugButton_Click(object sender, RoutedEventArgs e)
        {
            //SelectedClient?.KillstreakBullet.Toggle();
        }

        #region xex main calling
        #region index
        enum CB_Index
        {
            fog = 0,
            light = 1,
            hud = 2,
        }
        int call_on = 2;
        int call_off = 1;
        #endregion
        #region RGB
        private void Internal_InitDebugComboBox()
        {
            var checkBox_RGB_fog = new CheckBox()
            {
                Content = "Fog"
            };
            var checkBox_RGB_light = new CheckBox()
            {
                Content = "Light"
            };
            var checkBox_RGB_hud = new CheckBox()
            {
                Content = "HUD"
            };
            checkBox_RGB_fog.Checked += checkBox_RGB_fog_Checked;
            checkBox_RGB_fog.Unchecked += checkBox_RGB_fog_UnChecked;

            checkBox_RGB_light.Checked += checkBox_RGB_light_Checked;
            checkBox_RGB_light.Unchecked += checkBox_RGB_light_UnChecked;

            checkBox_RGB_hud.Checked += checkBox_RGB_hud_Checked;
            checkBox_RGB_hud.Unchecked += checkBox_RGB_hud_UnChecked;

            comboxRGB.Items.Add(checkBox_RGB_fog);
            comboxRGB.Items.Add(checkBox_RGB_light);
            comboxRGB.Items.Add(checkBox_RGB_hud);

        }

        private void checkBox_RGB_fog_Checked(object? sender, EventArgs e)
        {
            if (xboxConsole is null)
                return;
            XexManager.call(xboxConsole, (int)CB_Index.fog, call_on);
        }

        private void checkBox_RGB_fog_UnChecked(object? sender, EventArgs e)
        {
            if (xboxConsole is null)
                return;
            XexManager.call(xboxConsole, (int)CB_Index.fog, call_off);
        }
        private void checkBox_RGB_light_Checked(object? sender, EventArgs e)
        {
            if (xboxConsole is null)
                return;
            XexManager.call(xboxConsole, (int)CB_Index.light, call_on);
        }

        private void checkBox_RGB_light_UnChecked(object? sender, EventArgs e)
        {
            if (xboxConsole is null)
                return;
            XexManager.call(xboxConsole, (int)CB_Index.light, call_off);
        }
        private void checkBox_RGB_hud_Checked(object? sender, EventArgs e)
        {
            if (xboxConsole is null)
                return;
            XexManager.call(xboxConsole, (int)CB_Index.hud, call_on);
        }

        private void checkBox_RGB_hud_UnChecked(object? sender, EventArgs e)
        {
            if (xboxConsole is null)
                return;
            XexManager.call(xboxConsole, (int)CB_Index.hud, call_off);
        }
        #endregion
        #endregion
    }
}