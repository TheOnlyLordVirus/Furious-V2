using System;
using System.Windows;

using XDevkit;
using XDRPC;

namespace LordVirusMw2XboxLib;

using Constants = Mw2XboxLibConstants;

internal static class Mw2GameFunctions
{
    internal static bool TryConnectToMw2(IXboxManager? xboxManager, out IXboxConsole? connectedXbox)
    {
        bool result = false;

        try
        {
            if (xboxManager is null)
                xboxManager = new XboxManager();

            connectedXbox = xboxManager.OpenConsole(xboxManager.DefaultConsole);

            Cbuf_AddText(connectedXbox!, "loc_warningsUI 0; loc_warnings 0; cg_blood 0; cg_bloodLimit 1;");
            
            result = true;

            MessageBox.Show
            (
                "Successfuly Connected!",
                "Connection",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        catch (Exception ex)
        {
            result = false;
            connectedXbox = null;

            MessageBox.Show
            (
                ex.Message,
                "Connect Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        return result;
    }

    private static bool LocalClientInGame(IXboxConsole xboxConsole) => Cg_DvarGetBool(xboxConsole!, "cl_ingame");

    internal static async Task UnlockAll(IXboxConsole xbox, int client = -1)
    {
        if (!LocalClientInGame(xbox))
        {
            MessageBox.Show("You must be in game to unlock all!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        Cg_GameSendServerCommand(xbox!, client, 0, "s loc_warningsUI 0; loc_warnings 0;");

        iPrintLnBold(xbox!, "^2Starting Challenges!", client);

        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks0);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks1);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks2);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks3);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks4);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks5);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks6);
        iPrintLn(xbox!, "25 ^9Percent ^4Unlocked", client);
        await Task.Delay(TimeSpan.FromSeconds(4));

        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks7);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks8);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks9);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks10);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks11);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks12);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks13);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks14);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks15);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks16);
        iPrintLn(xbox!, "50 ^9Percent ^4Unlocked", client);
        await Task.Delay(TimeSpan.FromSeconds(4));

        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks17);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks18);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks19);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks20);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks21);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks22);
        iPrintLn(xbox!, "75 ^9Percent ^4Unlocked", client);
        await Task.Delay(TimeSpan.FromSeconds(4));

        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks23);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks24);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks25);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks26);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks27);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks28);
        Cg_GameSendServerCommand(xbox!, client, 0, Constants.Unlocks29);
        iPrintLn(xbox!, "100 ^9Percent ^4Unlocked", client);
        await Task.Delay(TimeSpan.FromMilliseconds(200));

        iPrintLnBold(xbox!, "^2Completed Challenges!", client);
    }

    internal static bool Cg_DvarGetBool(IXboxConsole xboxConsole, string commandString)
        => xboxConsole.ExecuteRPC<bool>
        (
            new XDRPCExecutionOptions(XDRPCMode.Title, Constants.Dvar_GetBool),
            new object[] { commandString }
        );

    internal static void Cbuf_AddText(IXboxConsole xboxConsole, string commandString)
        => xboxConsole.ExecuteRPC<int>
        (
            new XDRPCExecutionOptions(XDRPCMode.Title, Constants.Cbuf_AddText),
            new object[] { 0, commandString }
        );

    internal static void Cg_GameSendServerCommand(IXboxConsole xboxConsole, params object[] parameters)
        => xboxConsole.ExecuteRPC<int>
        (
            new XDRPCExecutionOptions(XDRPCMode.Title, Constants.Sv_GameSendServerCommand),
            parameters
        );

    internal static void iPrintLn(IXboxConsole xboxConsole, string text, int client = -1)
        => Cg_GameSendServerCommand(xboxConsole, client, 0, $"f \"{text}\"");

    internal static void iPrintLnBold(IXboxConsole xboxConsole, string text, int client = -1)
        => Cg_GameSendServerCommand(xboxConsole, client, 0, $"c \"{text}\"");
}