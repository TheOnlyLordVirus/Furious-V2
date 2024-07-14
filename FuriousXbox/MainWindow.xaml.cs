using System.Windows;

using XDevkit;
using LordVirusMw2XboxLib;

namespace FuriousXbox;

using Constants = Mw2XboxLibConstants;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private IXboxManager? xboxManager;
    private IXboxConsole? xboxConsole;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Internal_SetWindowElements(bool enabled)
    {
        // TODO: Put any button, checkbox, dropdown, ect... that needs to be enabled in here when we succesfully connect.
        NonHostAimbotCheckBox.IsEnabled = enabled;
        NonHostLaserCheckBox.IsEnabled = enabled;
        NonHostRedBoxesCheckBox.IsEnabled = enabled;
        NonHostThermalRedBoxesCheckBox.IsEnabled = enabled;
        NonHostNoRecoilCheckBox.IsEnabled = enabled;
        NonHostProModCheckBox.IsEnabled = enabled;
        NonHostCartoonCheckBox.IsEnabled = enabled;
        NonHostChromeCheckBox.IsEnabled = enabled;
        NonHostUiDebugCheckBox.IsEnabled = enabled;
        NonHostGameFxCheckBox.IsEnabled = enabled;

        NameChangerTextBox.IsEnabled = enabled;
        ChangeNameButton.IsEnabled = enabled;
        RealTimeNameChangeCheckBox.IsEnabled = enabled;

        ClanNameChangerTextBox.IsEnabled = enabled;
        ChangeClanNameButton.IsEnabled = enabled;

        PrestigeIntegerUpDown.Value = 10;
        PrestigeIntegerUpDown.Maximum = Constants.MaxPrestige;
        PrestigeIntegerUpDown.Minimum = Constants.MinPrestige;
        PrestigeIntegerUpDown.IsEnabled = enabled;
        ChangePrestigeButton.IsEnabled = enabled;
        LoopPrestigeCheckBox.IsEnabled = enabled;

        LevelIntegerUpDown.Value = Constants.MaxLevel;
        LevelIntegerUpDown.Maximum = Constants.MaxLevel;
        LevelIntegerUpDown.Minimum = Constants.MinLevel;
        LevelIntegerUpDown.IsEnabled = enabled;
        ChangeLevelButton.IsEnabled = enabled;
        LoopLevelCheckBox.IsEnabled = enabled;

        CBuffAddTextBox.IsEnabled = enabled;
        CBuffAddTextButton.IsEnabled = enabled;

        SendGameServerCommandTextBox.IsEnabled = enabled;
        SendGameServerCommandButton.IsEnabled = enabled;

        UnlockAllButton.IsEnabled = enabled;
        EndGameButton.IsEnabled = enabled;

        ClientComboBox.IsEnabled = enabled;

        Internal_InitRGBComboBox();
    }

    private void ConnectButton_Click(object sender, RoutedEventArgs e)
    {
        Internal_SetWindowElements(Mw2GameFunctions.TryConnectToMw2(xboxManager, out xboxConsole));
    }
}