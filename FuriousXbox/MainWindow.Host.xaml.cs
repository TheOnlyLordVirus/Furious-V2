using System.Windows;

using LordVirusMw2XboxLib;

namespace FuriousXbox;

using Constants = Mw2XboxLibConstants;

public sealed partial class MainWindow
{
    private readonly Task?[] unlockAllTasks = new Task?[Constants.MaxClientCount];
    private readonly G_Client?[] CurrentGameClients = new G_Client?[Constants.MaxClientCount];

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

        for (int clientIndex = 0; clientIndex < Constants.MaxClientCount; ++clientIndex)
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

    private void SendGameServerCommandButton_Click(object sender, RoutedEventArgs e)
    {
        if (xboxConsole is null)
            return;

        int client = -1; // TODO: Get current client from the drop box.

        Mw2GameFunctions.Sv_GameSendServerCommand
            (
                xboxConsole!,
                client,
                0,
                SendGameServerCommandTextBox.Text
            );
    }

    private void UnlockAllButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedClient is null)
            return;

        if (unlockAllTasks[SelectedClient.ClientIndex] is not null &&
            !(unlockAllTasks[SelectedClient.ClientIndex]!.IsCompleted))
            return;

        unlockAllTasks[SelectedClient.ClientIndex] = SelectedClient.UnlockAll();
    }
}
