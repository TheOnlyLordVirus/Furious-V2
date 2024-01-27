using System;

using XDevkit;
using XDRPC;

namespace LordVirusMw2XboxLib;

internal static class Mw2GameFunctions
{
    private const UInt32 _cbufAddText = 0x82224990;
    private const UInt32 _svGameSendServerCommand = 0x822548D8;
    private const UInt32 _dvarGetBool = 0x8229EEE8;

    internal static bool Cg_DvarGetBool(IXboxConsole xboxConsole, string commandString)
        => xboxConsole.ExecuteRPC<bool>
        (
            new XDRPCExecutionOptions(XDRPCMode.Title, _dvarGetBool),
            new object[] { commandString }
        );

    internal static void Cbuf_AddText(IXboxConsole xboxConsole, string commandString)
        => xboxConsole.ExecuteRPC<int>
        (
            new XDRPCExecutionOptions(XDRPCMode.Title, _cbufAddText),
            new object[] { 0, commandString }
        );

    internal static void Cg_GameSendServerCommand(IXboxConsole xboxConsole, params object[] parameters)
        => xboxConsole.ExecuteRPC<int>
        (
            new XDRPCExecutionOptions(XDRPCMode.Title, _svGameSendServerCommand),
            parameters
        );

    internal static void iPrintLn(IXboxConsole xboxConsole, string text, int client = -1)
        => Cg_GameSendServerCommand(xboxConsole, client, 0, $"f \"{text}\"");

    internal static void iPrintLnBold(IXboxConsole xboxConsole, string text, int client = -1)
        => Cg_GameSendServerCommand(xboxConsole, client, 0, $"c \"{text}\"");
}