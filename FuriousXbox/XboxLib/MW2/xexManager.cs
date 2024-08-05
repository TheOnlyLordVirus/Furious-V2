using XDevkit;
using XDRPCPlusPlus;

namespace FuriousXbox.XboxLib.MW2;

internal sealed class XexManager
{
    public enum Callback_Index
    {
        fog = 0,
        light = 1,
        hud = 2,
        aimbot = 3
    }

    public const int call_on = 2;
    public const int call_off = 1;

    public const uint callAddr = 0x82D67100;
    public const uint rCallAddr = 0x82D67200;

    public static void call(IXboxConsole xbox, int index, int value)
    {
        xbox.WriteInt32(callAddr + ((uint)index * 4), value);
    }

    public static int rCall(IXboxConsole xbox, int index)
    {
        return xbox.ReadInt32(rCallAddr + ((uint)index * 4));
    }
    public static void rCalled(IXboxConsole xbox, int index)
    {
        xbox.WriteInt32(rCallAddr + ((uint)index * 4), 0);
    }
    public static bool callBack(IXboxConsole xbox, int index)
    {
        if (rCall(xbox, index) > 0)
        {
            rCalled(xbox, index);
            return true;
        }
        else
            return false;
    }
}
