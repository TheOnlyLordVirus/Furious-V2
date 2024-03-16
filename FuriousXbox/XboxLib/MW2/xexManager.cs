using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDevkit;
using XDRPC;
using XDRPCPlusPlus;

namespace FuriousXbox.XboxLib.MW2
{
    internal class XexManager
    {
        public static uint callAddr = 0x82D67100;
        public static uint rCallAddr = 0x82D67200;

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
}
