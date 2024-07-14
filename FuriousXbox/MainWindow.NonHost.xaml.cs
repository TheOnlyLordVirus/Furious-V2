using System.Windows;

using LordVirusMw2XboxLib;
using XDRPCPlusPlus;

namespace FuriousXbox;

using Constants = Mw2XboxLibConstants;

public sealed partial class MainWindow
{
    private readonly Random _random = new Random();

    private CancellationTokenSource? nameChangerCancellationTokenSource = null;
    private CancellationTokenSource? levelCancellationTokenSource = null;
    private CancellationTokenSource? prestigeCancellationTokenSource = null;

    private void NonHostLaserCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        xboxConsole?.WriteByte(Constants.LaserAddress, Constants.TrueByte);
    }

    private void NonHostLaserCheckBox_UnChecked(object sender, RoutedEventArgs e)
    {
        xboxConsole?.WriteByte(Constants.LaserAddress, Constants.FalseByte);
    }

    private void NonHostNoRecoilCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        xboxConsole?.DebugTarget
            .SetMemory
            (
                Constants.NoRecoilAddress,
                (uint)Constants.NoRecoilOn.Length,
                Constants.NoRecoilOn,
                out _
            );
    }

    private void NonHostNoRecoilCheckBox_UnChecked(object sender, RoutedEventArgs e)
    {
        xboxConsole?.DebugTarget
            .SetMemory
            (
                Constants.NoRecoilAddress,
                (uint)Constants.NoRecoilOff.Length,
                Constants.NoRecoilOff,
                out _
            );
    }

    private void NonHostRedBoxesCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        xboxConsole?.DebugTarget
            .SetMemory
            (
                Constants.RedBoxAddress,
                (uint)Constants.RedBoxesOn.Length,
                Constants.RedBoxesOn,
                out _
            );
    }

    private void NonHostRedBoxesCheckBox_UnChecked(object sender, RoutedEventArgs e)
    {
        xboxConsole?.DebugTarget
            .SetMemory
            (
                Constants.RedBoxAddress,
                (uint)Constants.RedBoxesOff.Length,
                Constants.RedBoxesOff,
                out _
            );
    }

    private void NonHostThermalRedBoxesCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        xboxConsole?.DebugTarget
            .SetMemory
            (
                Constants.ThermalAddress,
                (uint)Constants.ThermalOn.Length,
                Constants.ThermalOn,
                out _
            );
    }

    private void NonHostThermalRedBoxesCheckBox_UnChecked(object sender, RoutedEventArgs e)
    {
        xboxConsole?.DebugTarget
            .SetMemory
            (
                Constants.ThermalAddress,
                (uint)Constants.ThermalOff.Length,
                Constants.ThermalOff,
                out _
            );
    }

    private void NonHostProModCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if (xboxConsole is null)
            return;

        Mw2GameFunctions.Cbuf_AddText(xboxConsole, ("cg_fov 100;"));
    }

    private void NonHostProModCheckBox_UnChecked(object sender, RoutedEventArgs e)
    {
        if (xboxConsole is null)
            return;

        Mw2GameFunctions.Cbuf_AddText(xboxConsole, ("reset cg_fov;"));
    }

    private void NonHostCartoonCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if (xboxConsole is null)
            return;

        Mw2GameFunctions.Cbuf_AddText(xboxConsole, ("r_fullbright 1;"));
    }

    private void NonHostCartoonCheckBox_UnChecked(object sender, RoutedEventArgs e)
    {
        if (xboxConsole is null)
            return;

        Mw2GameFunctions.Cbuf_AddText(xboxConsole, ("r_fullbright 0;"));
    }

    private void NonHostChromeCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if (xboxConsole is null)
            return;

        Mw2GameFunctions.Cbuf_AddText(xboxConsole, ("r_specularmap 2;"));
    }

    private void NonHostChromeCheckBox_UnChecked(object sender, RoutedEventArgs e)
    {
        if (xboxConsole is null)
            return;

        Mw2GameFunctions.Cbuf_AddText(xboxConsole, ("r_specularmap 0;"));
    }

    private void NonHostGameFxCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if (xboxConsole is null)
            return;

        Mw2GameFunctions.Cbuf_AddText(xboxConsole, "fx_enable 1;");
    }

    private void NonHostGameFxCheckBox_UnChecked(object sender, RoutedEventArgs e)
    {
        if (xboxConsole is null)
            return;

        Mw2GameFunctions.Cbuf_AddText(xboxConsole, "fx_enable 0;");
    }

    private void NonHostUiDebugCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if (xboxConsole is null)
            return;

        Mw2GameFunctions.Cbuf_AddText(xboxConsole, ("ui_debugmode 1;"));
    }

    private void NonHostUiDebugCheckBox_UnChecked(object sender, RoutedEventArgs e)
    {
        if (xboxConsole is null)
            return;

        Mw2GameFunctions.Cbuf_AddText(xboxConsole, ("ui_debugmode 0;"));
    }

    private void ChangeClanNameButton_Click(object sender, RoutedEventArgs e)
    {
        if (xboxConsole is null)
            return;

        Mw2GameFunctions.SetClanName(xboxConsole!, ClanNameChangerTextBox.Text);
    }

    private void ChangeNameButton_Click(object sender, RoutedEventArgs e)
    {
        if (xboxConsole is null)
            return;

        Mw2GameFunctions.SetName(xboxConsole!, NameChangerTextBox.Text);
    }

    private void RealTimeNameChangeCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        Internal_SetRealTimeNameChanging(true);
    }

    private void RealTimeNameChangeCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        Internal_SetRealTimeNameChanging(false);
    }

    private void RainbowCheckBox_Checked(object sender, RoutedEventArgs e)
    {
    }

    private void RainbowCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
    }

    private void ChangePrestigeButton_Click(object sender, RoutedEventArgs e)
    {
        if (PrestigeIntegerUpDown.Value is null)
            return;

        if (xboxConsole is null)
            return;

        Mw2GameFunctions.SetPrestige(xboxConsole!, (int)PrestigeIntegerUpDown.Value);
    }

    private void ChangeLevelButton_Click(object sender, RoutedEventArgs e)
    {
        if (LevelIntegerUpDown.Value is null)
            return;

        if (xboxConsole is null)
            return;

        Mw2GameFunctions.SetLevel(xboxConsole, (int)LevelIntegerUpDown.Value);
    }

    private void LoopPrestigeCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        Internal_SetPrestigeLooping(true);
    }

    private void LoopPrestigeCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        Internal_SetPrestigeLooping(false);
    }

    private void LoopLevelCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        Internal_SetLevelLooping(true);
    }

    private void LoopLevelCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        Internal_SetLevelLooping(false);
    }

    private void CBuffAddTextButton_Click(object sender, RoutedEventArgs e)
    {
        if (xboxConsole is null)
            return;

        Mw2GameFunctions.Cbuf_AddText(xboxConsole!, CBuffAddTextBox.Text);
    }

    private void EndGameButton_Click(object sender, RoutedEventArgs e)
    {
        if (xboxConsole is null)
            return;

        Mw2GameFunctions.EndGame(xboxConsole!);
    }

    private void Internal_SetRealTimeNameChanging(bool toggleValue)
    {
        if (toggleValue)
        {
            nameChangerCancellationTokenSource = new CancellationTokenSource();
            _ = Internal_AutoUpdateName(nameChangerCancellationTokenSource.Token);

            ChangeNameButton.IsEnabled = false;
            RainbowCheckBox.IsEnabled = true;
            ButtonCheckBox.IsEnabled = true;

            return;
        }

        if (nameChangerCancellationTokenSource is null)
            return;

        nameChangerCancellationTokenSource.Cancel();
        nameChangerCancellationTokenSource = null;

        ChangeNameButton.IsEnabled = true;

        RainbowCheckBox.IsEnabled = false;
        RainbowCheckBox.IsChecked = false;

        ButtonCheckBox.IsEnabled = false;
        ButtonCheckBox.IsChecked = false;

        NameChangerTextBox.MaxLength = Constants.MaxNameCharLength;
    }

    private void Internal_SetLevelLooping(bool toggleValue)
    {
        if (toggleValue)
        {
            levelCancellationTokenSource = new CancellationTokenSource();
            _ = Internal_LoopLevels(levelCancellationTokenSource.Token);

            LevelIntegerUpDown.IsEnabled = false;
            ChangeLevelButton.IsEnabled = false;

            return;
        }

        if (levelCancellationTokenSource is null)
            return;

        levelCancellationTokenSource.Cancel();
        levelCancellationTokenSource = null;

        LevelIntegerUpDown.IsEnabled = true;
        ChangeLevelButton.IsEnabled = true;
    }

    private void Internal_SetPrestigeLooping(bool toggleValue)
    {
        if (toggleValue)
        {
            prestigeCancellationTokenSource = new CancellationTokenSource();
            _ = Internal_LoopPrestiges(prestigeCancellationTokenSource.Token);

            PrestigeIntegerUpDown.IsEnabled = false;
            ChangePrestigeButton.IsEnabled = false;

            return;
        }

        if (prestigeCancellationTokenSource is null)
            return;

        prestigeCancellationTokenSource.Cancel();
        prestigeCancellationTokenSource = null;

        PrestigeIntegerUpDown.IsEnabled = true;
        ChangePrestigeButton.IsEnabled = true;
    }

    private ReadOnlySpan<char> Internal_BuildAutoUpdatingNameString()
    {
        byte tempMaxNameInputLength = Constants.MaxNameCharLength;

        ReadOnlySpan<char> newNameBuffer = NameChangerTextBox.Text;

        if (newNameBuffer.Length < Constants.MaxNameCharLength)
            goto ParseSpan;

        for (int i = 0; i < newNameBuffer.Length; ++i)
        {
            ++i;
            if (!(newNameBuffer[i - 1] == '^' &&
                (newNameBuffer[i] == 'B' ||
                    newNameBuffer[i] == 'b')))
                continue;

            tempMaxNameInputLength++;
        }

        // Slice to buffer length;
        newNameBuffer = newNameBuffer[..tempMaxNameInputLength];

    ParseSpan:
        if (RainbowCheckBox.IsChecked ?? false)
            Internal_ParseFlashingCodes(newNameBuffer, out newNameBuffer);

        if (ButtonCheckBox.IsChecked ?? false)
            Internal_ParseButtonCodes(newNameBuffer, out newNameBuffer);

        return newNameBuffer;
    }

    private void Internal_ParseFlashingCodes(ReadOnlySpan<char> name, out ReadOnlySpan<char> newName)
    {
        Span<char> tempName = stackalloc char[name.Length];
        name.CopyTo(tempName);

        int flashIndex = name.IndexOf("^F", StringComparison.OrdinalIgnoreCase);
        while (flashIndex > -1)
        {
            tempName[flashIndex] = '^'; flashIndex++;
            tempName[flashIndex] = (char)(_random.Next(6) + 48); flashIndex++;

            var newflashIndex = name[flashIndex..].IndexOf("^F", StringComparison.OrdinalIgnoreCase);

            if (newflashIndex == -1)
                break;

            flashIndex = newflashIndex + flashIndex;
        }

        newName = tempName.ToArray();
    }

    private void Internal_ParseButtonCodes(ReadOnlySpan<char> inputName, out ReadOnlySpan<char> newName)
    {
        Span<char> tempName = stackalloc char[inputName.Length];
        inputName.CopyTo(tempName);

        int buttonIndex = inputName.IndexOf("^B", StringComparison.OrdinalIgnoreCase);
        int removalCount = 0;
        while (buttonIndex > -1)
        {
            // Generate random button char.
            tempName[buttonIndex - removalCount] = Constants
                .ButtonCharMap[_random.Next(Constants.ButtonCharMap.Length)];

            Span<char> tempNameSlice = tempName[(buttonIndex - removalCount + 2)..].ToArray();
            tempNameSlice.TryCopyTo(tempName[(buttonIndex - removalCount + 1)..]);

            removalCount++;

            if (inputName.Length < buttonIndex + 2)
                break;

            int tempIndex = buttonIndex + 2;
            buttonIndex = inputName[tempIndex..].IndexOf("^B", StringComparison.OrdinalIgnoreCase);

            if (buttonIndex == -1)
                break;

            buttonIndex += tempIndex;
        }

        newName = tempName[..(tempName.Length - removalCount)].ToArray();
    }

    private async Task Internal_AutoUpdateName(CancellationToken cancellationToken)
    {
        do
        {
            if (xboxConsole is null)
                break;

            Mw2GameFunctions.SetName(xboxConsole!, Internal_BuildAutoUpdatingNameString());

            await Task
                .Delay(TimeSpan.FromMilliseconds(150), cancellationToken)
                .ConfigureAwait(true);
        }
        while (!cancellationToken.IsCancellationRequested);
    }

    private async Task Internal_LoopLevels(CancellationToken cancellationToken)
    {
        int level = 1;

        do
        {
            if (xboxConsole is null)
                break;

            Mw2GameFunctions.SetLevel(xboxConsole!, level++);

            level %= (Constants.MaxLevel + 1);

            await Task.Delay(TimeSpan.FromMilliseconds(70), cancellationToken);
        }
        while (!cancellationToken.IsCancellationRequested);
    }

    private async Task Internal_LoopPrestiges(CancellationToken cancellationToken)
    {
        int prestige = 1;

        do
        {
            if (xboxConsole is null)
                break;

            Mw2GameFunctions.SetPrestige(xboxConsole!, prestige++);

            prestige %= (Constants.MaxPrestige + 1);

            await Task.Delay(TimeSpan.FromMilliseconds(600), cancellationToken);
        }
        while (!cancellationToken.IsCancellationRequested);
    }
}
