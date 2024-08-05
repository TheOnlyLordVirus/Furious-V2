using System.Windows;
using System.Windows.Controls;

namespace FuriousXbox.XboxLib.MW2;

internal sealed class G_Client_DataGridRow
{
    public readonly DataGridTextColumn ClientName = new();

    public readonly CheckBox GodModeCheckBox = new ();

    public G_Client_DataGridRow
        (
            string clientName, 
            RoutedEventHandler godModeEnableCallBack, 
            RoutedEventHandler godModeDisableCallBack
        )
    {
        //ClientName;

        GodModeCheckBox.IsChecked = false;
        GodModeCheckBox.Checked += godModeEnableCallBack;
        GodModeCheckBox.Unchecked += godModeDisableCallBack;
    }
}
