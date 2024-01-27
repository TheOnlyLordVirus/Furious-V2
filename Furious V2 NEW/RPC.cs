using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PS3Lib;

namespace Furious
{
    class RPC
    {
        #region RPC
        public static uint fxBirthTime = 0x90;
        public static uint fxDecayDuration = 0x9c;
        public static uint fxDecayStartTime = 0x98;
        public static uint fxLetterTime = 0x94;


        private static uint function_address = 0x38EDE8;

        public static int Call(UInt32 func_address, params object[] parameters)
        {
            int length = parameters.Length;
            int index = 0;
            UInt32 num3 = 0;
            UInt32 num4 = 0;
            UInt32 num5 = 0;
            UInt32 num6 = 0;
            while (index < length)
            {
                if (parameters[index] is int)
                {
                    Form1.PS3.Extension.WriteInt32(0x10050000 + (num3 * 4), (int)parameters[index]);
                    num3++;
                }
                else if (parameters[index] is UInt32)
                {
                    Form1.PS3.Extension.WriteUInt32(0x10050000 + (num3 * 4), (UInt32)parameters[index]);
                    num3++;
                }
                else
                {
                    UInt32 num7;
                    if (parameters[index] is string)
                    {
                        num7 = 0x10052000 + (num4 * 0x400);
                        Form1.PS3.Extension.WriteString(num7, Convert.ToString(parameters[index]));
                        Form1.PS3.Extension.WriteUInt32(0x10050000 + (num3 * 4), num7);
                        num3++;
                        num4++;
                    }
                    else if (parameters[index] is float)
                    {
                        Form1.PS3.Extension.WriteFloat(0x10050024 + (num5 * 4), (float)parameters[index]);
                        num5++;
                    }
                    else if (parameters[index] is float[])
                    {
                        float[] input = (float[])parameters[index];
                        num7 = 0x10051000 + (num6 * 4);
                        WriteSingle(num7, input);
                        Form1.PS3.Extension.WriteUInt32(0x10050000 + (num3 * 4), num7);
                        num3++;
                        num6 += (UInt32)input.Length;
                    }
                }
                index++;
            }
            Form1.PS3.Extension.WriteUInt32(0x1005004C, func_address);
            Thread.Sleep(20);
            return Form1.PS3.Extension.ReadInt32(0x10050050);
        }

        public static void EnableRPC()
        {
            byte[] RPC = { 0xF8, 0x21, 0xFF, 0x91, 0x7C, 0x08, 0x02, 0xA6, 0xF8, 0x01, 0x00, 0x80, 0x3C, 0x40, 0x00, 0x72, 0x30, 0x42, 0x4C, 0x38, 0x3C, 0x60, 0x10, 0x05, 0x81, 0x83, 0x00, 0x4C, 0x2C, 0x0C, 0x00, 0x00, 0x41, 0x82, 0x00, 0x64, 0x80, 0x83, 0x00, 0x04, 0x80, 0xA3, 0x00, 0x08, 0x80, 0xC3, 0x00, 0x0C, 0x80, 0xE3, 0x00, 0x10, 0x81, 0x03, 0x00, 0x14, 0x81, 0x23, 0x00, 0x18, 0x81, 0x43, 0x00, 0x1C, 0x81, 0x63, 0x00, 0x20, 0xC0, 0x23, 0x00, 0x24, 0xC0, 0x43, 0x00, 0x28, 0xC0, 0x63, 0x00, 0x2C, 0xC0, 0x83, 0x00, 0x30, 0xC0, 0xA3, 0x00, 0x34, 0xC0, 0xC3, 0x00, 0x38, 0xC0, 0xE3, 0x00, 0x3C, 0xC1, 0x03, 0x00, 0x40, 0xC1, 0x23, 0x00, 0x48, 0x80, 0x63, 0x00, 0x00, 0x7D, 0x89, 0x03, 0xA6, 0x4E, 0x80, 0x04, 0x21, 0x3C, 0x80, 0x10, 0x05, 0x38, 0xA0, 0x00, 0x00, 0x90, 0xA4, 0x00, 0x4C, 0x90, 0x64, 0x00, 0x50, 0x3C, 0x40, 0x00, 0x73, 0x30, 0x42, 0x4B, 0xE8, 0xE8, 0x01, 0x00, 0x80, 0x7C, 0x08, 0x03, 0xA6, 0x38, 0x21, 0x00, 0x70, 0x4E, 0x80, 0x00, 0x20 };
            Form1.PS3.SetMemory(function_address, RPC);
            Form1.PS3.SetMemory(0x10050000, new byte[0x2854]);
        }
        #endregion
        #region HUD / Text
        public class HudStruct
        {
            public static uint
            xOffset = 0x08,
            yOffset = 0x04,
            textOffset = 0x84,
            GlowColor = 0x8C,
            fxBirthTime = 0x90,
            fadeStartTime = 0x3C,
            fxLetterTime = 0x94,
            fadeTime = 0x40,
            fromColor = 0x38,
            fxDecayStartTime = 0x98,
            fxDecayDuration = 0x9C,
            fontOffset = 0x28,
            fontSizeOffset = 0x14,
            colorOffset = 0x34,
            scaleStartTime = 0x58,
            fromFontScale = 0x18,
            fontScaleTime = 0x20,
            relativeOffset = 0x2c,
            widthOffset = 0x48,
            heightOffset = 0x44,
            shaderOffset = 0x4C,
            alignOffset = 0x30,
            fromAlignOrg = 0x68,
            fromAlignScreen = 0x6C,
            alignOrg = 0x2C,
            alignScreen = 0x30,
            fromY = 0x60,
            fromX = 0x64,
            moveStartTime = 0x70,
            moveTime = 0x74,
            flags = 0xA4,
            soundID = 160U,
            clientIndex = 0xA8;
        }

        public static class HudAlloc
        {
            public static uint
                IndexSlot = 50,
                g_hudelem = 0x012E9858;


            public static bool
                Start = true;
        }


        public static class HUDAlign
        {
            public static uint
                RIGHT = 2,
                CENTER = 5,
                LEFT = 1;
        }


        public class HudTypes
        {
            public static uint
                Text = 1,
                Shader = 6,
                Null = 0;
        }
        public static void ChangeFont(uint elem, uint font)
        {
            Form1.PS3.Extension.WriteUInt32(elem + HudStruct.fontOffset, font);
        }
        public class Material
        {
            public static uint
                White = 1,
                Black = 2,
                Prestige0 = 0x1A,
                Prestige1 = 0x1B,
                Prestige2 = 0x1C,
                Prestige3 = 0x1D,
                Prestige4 = 0x1E,
                Prestige5 = 0x1F,
                Prestige6 = 0x20,
                Prestige7 = 0x21,
                Prestige8 = 0x22,
                Prestige9 = 0x23,
                Prestige10 = 0x24,
                WhiteRectangle = 0x25,
                NoMap = 0x29;
        }
        public static int RGB2INT(int r, int g, int b, int a)
        {
            byte[] newRGB = new byte[4];
            newRGB[0] = (byte)r;
            newRGB[1] = (byte)g;
            newRGB[2] = (byte)b;
            newRGB[3] = (byte)a;
            Array.Reverse(newRGB);
            return BitConverter.ToInt32(newRGB, 0);
        }

        private static uint HudElem_Alloc()
        {
            uint num;
            uint num1 = 120;
            while (true)
            {
                if (num1 < 1024)
                {
                    uint gHudelems = HudStructLib.G_Hudelems + num1 * HudStructLib.IndexSize;
                    if (Form1.PS3.Extension.ReadInt32(gHudelems + HudStructLib.type) != 0)
                    {
                        num1++;
                    }
                    else
                    {
                        Form1.PS3.Extension.WriteBytes(gHudelems, new byte[180]);
                        num = gHudelems;
                        break;
                    }
                }
                else
                {
                    num = 0;
                    break;
                }
            }
            return num;
        }

        public static Int32 getLevelTime()
        {
            Byte[] LevelTime = new Byte[4];
            Form1.PS3.GetMemory(0x12e0304, LevelTime);
            Array.Reverse(LevelTime, 0, 4);
            return BitConverter.ToInt32(LevelTime, 0);
        }
        public static void FadeOverTime(uint elem, int Time, int R, int G, int B, int A)
        {
            Form1.PS3.Extension.WriteInt32(elem + HudStruct.fadeStartTime, getLevelTime());
            Form1.PS3.Extension.WriteBytes(elem + HudStruct.fromColor, Form1.PS3.Extension.ReadBytes(elem + HudStruct.colorOffset, 4));
            Form1.PS3.Extension.WriteInt32(elem + HudStruct.fadeTime, Time);
            Form1.PS3.Extension.WriteInt32(elem + HudStruct.colorOffset, RGB2INT(R, G, B, A));
        }
        public static void FadeGlowOverTime(uint elem, int Time, int GlowR, int GlowG, int GlowB, int GlowA)
        {
            Form1.PS3.Extension.WriteInt32(elem + HudStruct.fadeStartTime, getLevelTime());
            Form1.PS3.Extension.WriteBytes(elem + HudStruct.fromColor, Form1.PS3.Extension.ReadBytes(elem + HudStruct.colorOffset, 4));
            Form1.PS3.Extension.WriteInt32(elem + HudStruct.fadeTime, Time);
            Form1.PS3.Extension.WriteInt32(elem + HudStruct.GlowColor, RGB2INT(GlowR, GlowG, GlowB, GlowA));
        }

        public static void MoveOverTime(uint elemIndex, short time, float X, float Y)
        {
            Form1.PS3.Extension.WriteFloat(elemIndex + HudStruct.fromX, Form1.PS3.Extension.ReadFloat(elemIndex + HudStruct.xOffset));
            Form1.PS3.Extension.WriteFloat(elemIndex + HudStruct.fromY, Form1.PS3.Extension.ReadFloat(elemIndex + HudStruct.yOffset));
            Form1.PS3.Extension.WriteInt32(elemIndex + HudStruct.moveStartTime, (int)getLevelTime());
            Form1.PS3.Extension.WriteInt32(elemIndex + HudStruct.moveTime, time);
            Form1.PS3.Extension.WriteFloat(elemIndex + HudStruct.xOffset, X);
            Form1.PS3.Extension.WriteFloat(elemIndex + HudStruct.yOffset, Y);
        }

        public static void setPulseFX(uint elem, int speed, int decayStart, int decayDuration)
        {
            Form1.PS3.Extension.WriteInt32(elem + HudStruct.fxBirthTime, getLevelTime());
            Form1.PS3.Extension.WriteInt32(elem + HudStruct.fxLetterTime, speed);
            Form1.PS3.Extension.WriteInt32(elem + HudStruct.fxDecayStartTime, decayStart);
            Form1.PS3.Extension.WriteInt32(elem + HudStruct.fxDecayDuration, decayDuration);
        }
        public static void DestroyElem(uint elem)
        {
            Form1.PS3.SetMemory(elem, new byte[0xB4]);
        }

        public static void SetElement(uint Element, uint HudTypes)
        {
            Form1.PS3.Extension.WriteUInt32(Element, HudTypes);
        }
        public static uint HudElemAlloc(bool Reset = false)
        {
            if (Reset == true)
                HudAlloc.IndexSlot = 50;
            uint Output = HudAlloc.g_hudelem + (HudAlloc.IndexSlot * 0xB4);
            HudAlloc.IndexSlot++;
            return Output;
        }
        public static uint HudElemAlloc_Game(int clientNumber)
        {
            return (uint)Call(0x001806E0, clientNumber);
        }
        public static void WritePowerPc(bool Active)
        {
            byte[] NewPPC = new byte[] { 0xF8, 0x21, 0xFF, 0x61, 0x7C, 0x08, 0x02, 0xA6, 0xF8, 0x01, 0x00, 0xB0, 0x3C, 0x60, 0x10, 0x03, 0x80, 0x63, 0x00, 0x00, 0x60, 0x62, 0x00, 0x00, 0x3C, 0x60, 0x10, 0x04, 0x80, 0x63, 0x00, 0x00, 0x2C, 0x03, 0x00, 0x00, 0x41, 0x82, 0x00, 0x28, 0x3C, 0x60, 0x10, 0x04, 0x80, 0x63, 0x00, 0x04, 0x3C, 0xA0, 0x10, 0x04, 0x38, 0x80, 0x00, 0x00, 0x30, 0xA5, 0x00, 0x10, 0x4B, 0xE8, 0xB2, 0x7D, 0x38, 0x60, 0x00, 0x00, 0x3C, 0x80, 0x10, 0x04, 0x90, 0x64, 0x00, 0x00, 0x3C, 0x60, 0x10, 0x05, 0x80, 0x63, 0x00, 0x00, 0x2C, 0x03, 0x00, 0x00, 0x41, 0x82, 0x00, 0x24, 0x3C, 0x60, 0x10, 0x05, 0x30, 0x63, 0x00, 0x10, 0x4B, 0xE2, 0xF9, 0x7D, 0x3C, 0x80, 0x10, 0x05, 0x90, 0x64, 0x00, 0x04, 0x38, 0x60, 0x00, 0x00, 0x3C, 0x80, 0x10, 0x05, 0x90, 0x64, 0x00, 0x00, 0x3C, 0x60, 0x10, 0x03, 0x80, 0x63, 0x00, 0x04, 0x60, 0x62, 0x00, 0x00, 0xE8, 0x01, 0x00, 0xB0, 0x7C, 0x08, 0x03, 0xA6, 0x38, 0x21, 0x00, 0xA0, 0x4E, 0x80, 0x00, 0x20 };
            byte[] RestorePPC = new byte[] { 0x81, 0x62, 0x92, 0x84, 0x7C, 0x08, 0x02, 0xA6, 0xF8, 0x21, 0xFF, 0x01, 0xFB, 0xE1, 0x00, 0xB8, 0xDB, 0x01, 0x00, 0xC0, 0x7C, 0x7F, 0x1B, 0x78, 0xDB, 0x21, 0x00, 0xC8, 0xDB, 0x41, 0x00, 0xD0, 0xDB, 0x61, 0x00, 0xD8, 0xDB, 0x81, 0x00, 0xE0, 0xDB, 0xA1, 0x00, 0xE8, 0xDB, 0xC1, 0x00, 0xF0, 0xDB, 0xE1, 0x00, 0xF8, 0xFB, 0x61, 0x00, 0x98, 0xFB, 0x81, 0x00, 0xA0, 0xFB, 0xA1, 0x00, 0xA8, 0xFB, 0xC1, 0x00, 0xB0, 0xF8, 0x01, 0x01, 0x10, 0x81, 0x2B, 0x00, 0x00, 0x88, 0x09, 0x00, 0x0C, 0x2F, 0x80, 0x00, 0x00, 0x40, 0x9E, 0x00, 0x64, 0x7C, 0x69, 0x1B, 0x78, 0xC0, 0x02, 0x92, 0x94, 0xC1, 0xA2, 0x92, 0x88, 0xD4, 0x09, 0x02, 0x40, 0xD0, 0x09, 0x00, 0x0C, 0xD1, 0xA9, 0x00, 0x04, 0xD0, 0x09, 0x00, 0x08, 0xE8, 0x01, 0x01, 0x10, 0xEB, 0x61, 0x00, 0x98, 0xEB, 0x81, 0x00, 0xA0, 0x7C, 0x08, 0x03, 0xA6, 0xEB, 0xA1, 0x00, 0xA8, 0xEB, 0xC1, 0x00, 0xB0, 0xEB, 0xE1, 0x00, 0xB8, 0xCB, 0x01, 0x00, 0xC0, 0xCB, 0x21, 0x00, 0xC8 };
            if (Active)
                Form1.PS3.SetMemory(0x0038EDE8, NewPPC);
            else
                Form1.PS3.SetMemory(0x0038EDE8, RestorePPC);
        }
        public static uint G_LocalizedString(string input)
        {
            uint StrIndex = 0;
            bool isRunning = true;
            WritePowerPc(true);
            Form1.PS3.Extension.WriteString(0x10050010, input);
            Form1.PS3.Extension.WriteBool(0x10050000 + 3, true);
            do { StrIndex = Form1.PS3.Extension.ReadUInt32(0x10050004); } while (StrIndex == 0);
            Form1.PS3.Extension.WriteUInt32(0x10050004, 0);
            do { isRunning = Form1.PS3.Extension.ReadBool(0x10050003); } while (isRunning != false);
            WritePowerPc(false);
            return StrIndex;
        }
        public static uint G_MaterialIndex(string shader)
        {
            return (uint)Call(0x001BE758, shader);
        }

        private static byte[] ReverseBytes(byte[] inArray)
        {
            Array.Reverse(inArray);
            return inArray;
        }
        public static void WriteSingle(uint address, float[] input)
        {
            int length = input.Length;
            byte[] array = new byte[length * 4];
            for (int i = 0; i < length; i++)
            {
                ReverseBytes(BitConverter.GetBytes(input[i])).CopyTo(array, (int)(i * 4));
            }
            Form1.PS3.SetMemory(address, array);
        }
        public static short G_LocalizedStringIndex(string Text)
        {
            Thread.Sleep(100);
            uint gLocalizedStringIndex = 1828808;
            object[] text = new object[] { Text };
            return (short)Call(gLocalizedStringIndex, text);
        }
        public static void ChangeText(uint elemIndex, string Text)
        {
            uint num = Offsets.HudElem_Alloc;
            Form1.PS3.Extension.WriteInt32(elemIndex + HudStruct.textOffset, G_LocalizedStringIndex(Text));
        }
        public static void ChangeAlpha(uint elemIndex, byte Alpha)
        {
            WriteByte(elemIndex + HudStructLib.color + 3, Alpha);
        }

        public static void WriteByte(uint address, byte input)
        {
            Form1.PS3.SetMemory(address, new byte[] { input });
        }
        public static void Huds_DestroyAll()
        {
            uint num = 120;
            Form1.PS3.SetMemory(19830872 + num * 0xB4, new byte[0xB4 * (1024 - num)]);
        }
        public static UInt32 client_s(Int32 clientIndex)
        {
            return 0x34740000 + (0x97F80 * (UInt32)clientIndex);
        }

        public static UInt32 G_Entity(Int32 clientIndex)
        {
            return 0x1319800 + (0x280 * (UInt32)clientIndex);
        }
        public static UInt32 G_Client(Int32 clientIndex)
        {
            return 0x14E2200 + (0x3700 * (UInt32)clientIndex);
        }

        //int index,
        public static uint SetText(int clientIndex, string TextString, int Font, float fontScale, float X, float Y, int align = 0, int r = 255, int g = 255, int b = 255, int a = 255, int glowr = 255, int glowg = 255, int glowb = 255, int glowa = 0)
        {
            //uint Ind = Convert.ToUInt32(index);
            uint num = HudElem_Alloc();//+ ((uint)Ind * 0xB4)
            Form1.PS3.Extension.WriteInt32(num + HudStructLib.type, 1);
            Form1.PS3.Extension.WriteFloat(num + HudStructLib.fontScale, fontScale);
            Form1.PS3.Extension.WriteInt32(num + HudStructLib.font, Font);
            if (align == 0)
            {
                Form1.PS3.Extension.WriteFloat(num + HudStructLib.X, X);
                Form1.PS3.Extension.WriteFloat(num + HudStructLib.Y, Y);
            }
            else
            {
                Form1.PS3.Extension.WriteInt32(num + HudStructLib.alignOrg, 5);
                Form1.PS3.Extension.WriteInt32(num + HudStructLib.alignScreen, align);
            }
            Form1.PS3.Extension.WriteInt32(num + HudStructLib.clientIndex, clientIndex);
            Form1.PS3.Extension.WriteInt32(num + HudStructLib.text, G_LocalizedStringIndex(TextString));
            uint num1 = num + HudStructLib.color;
            byte[] numArray = new byte[] { (byte)r, (byte)g, (byte)b, (byte)a };
            Form1.PS3.Extension.WriteBytes(num1, numArray);
            uint num2 = num + HudStructLib.glowColor;
            numArray = new byte[] { (byte)glowr, (byte)glowg, (byte)glowb, (byte)glowa };
            Form1.PS3.Extension.WriteBytes(num2, numArray);
            return num;
        }

        public static int getMaterialIndex(string Material)
        {
            return Call(Offsets.G_MaterialIndex, new object[] { Material });
        }

        public static int precacheShader(string Shader)
        {
            Form1.PS3.Extension.WriteInt32(Offsets.AllowPrecache, 1);
            uint gMaterialIndex = Offsets.G_MaterialIndex;
            object[] shader = new object[] { Shader };
            int num = Call(gMaterialIndex, shader);
            Form1.PS3.Extension.WriteInt32(Offsets.AllowPrecache, 0);
            return num;
        }

        public static uint SetShader(int clientIndex, object Material, short Width, short Height, float X, float Y, int align = 0, int r = 255, int g = 255, int b = 255, int a = 255)
        {
            uint num = HudElem_Alloc();
            Form1.PS3.Extension.WriteInt32(num + HudStructLib.type, (int)HudTypes.Shader);
            if (Material is string)
            {
                precacheShader((string)Material);
                Form1.PS3.Extension.WriteInt32(num + HudStructLib.materialIndex, getMaterialIndex((string)Material));
            }
            if (Material is int)
            {
                Form1.PS3.Extension.WriteInt32(num + HudStructLib.materialIndex, (int)Material);
            }
            Form1.PS3.Extension.WriteInt32(num + HudStructLib.height, Height);
            Form1.PS3.Extension.WriteInt32(num + HudStructLib.width, Width);
            if (align == 0)
            {
                Form1.PS3.Extension.WriteFloat(num + HudStructLib.X, X);
                Form1.PS3.Extension.WriteFloat(num + HudStructLib.Y, Y);
            }
            else
            {
                Form1.PS3.Extension.WriteInt32(num + HudStructLib.alignOrg, 5);
                Form1.PS3.Extension.WriteInt32(num + HudStructLib.alignScreen, align);
            }
            Form1.PS3.Extension.WriteInt32(num + HudStructLib.clientIndex, clientIndex);
            uint num1 = num + HudStructLib.color;
            byte[] numArray = new byte[] { (byte)r, (byte)g, (byte)b, (byte)a };
            Form1.PS3.Extension.WriteBytes(num1, numArray);
            return num;
        }

        public static uint SetTypewriter(int clientIndex, string Text, int Font, float FontSize, float X, float Y, int align, ushort Lettertime = 200, ushort fxDecayStartTime = 7000, ushort fxDecayDuration = 1000, int r = 255, int g = 255, int b = 255, int a = 255, int glowr = 255, int glowg = 255, int glowb = 255, int glowa = 0)
        {
            uint num = SetText(clientIndex, Text, Font, FontSize, X, Y, align, r, g, b, a, glowr, glowg, glowb, glowa);
            Form1.PS3.Extension.WriteUInt32(num + HudStructLib.fxBirthTime, (uint)getLevelTime());
            Form1.PS3.Extension.WriteUInt32(num + HudStructLib.fxLetterTime, Lettertime);
            Form1.PS3.Extension.WriteUInt32(num + HudStructLib.fxDecayStartTime, fxDecayStartTime);
            Form1.PS3.Extension.WriteUInt32(num + HudStructLib.fxDecayDuration, fxDecayDuration);
            return num;
        }

        public static UInt32 G_Client(int clientIndex, UInt32 Mod)
        {
            return Offsets.G_Client + (UInt32)Mod + ((UInt32)clientIndex * HudStruct.clientIndex);
        }

        #region Hud Lib
        public class Offsets
        {
            public static uint G_Entity;

            public static uint G_MaterialIndex;

            public static uint G_LocalizedStringIndex;

            public static uint G_HudElems;

            public static uint HudElem_Alloc;

            public static uint Weapon_RocketLauncher_Fire;

            public static uint Dvar_GetString;

            public static uint G_AddEvent;

            public static uint cl_ingame;

            public static uint LocalPlayerName;

            public static uint G_FireGrenade;

            public static uint level_locals_t;

            public static uint levelTime;

            public static uint AllowPrecache;

            public static uint ServerDetails;

            public static uint SV_GameSendServerCommand;

            public static uint BG_GetWeaponDef;

            public static uint G_GetPlayerViewOrigin;

            public static uint ObjectiveIndex;

            public static uint Trace_GetEntityHitID;

            public static uint G_LocationalTrace;

            public static uint G_Client;

            public static uint ClientAngles;

            public static uint ClientOrigin;

            public static uint ClientButtonMonitoring;

            public static uint ClientName;

            static Offsets()
            {
                Offsets.G_Entity = 20027392;
                Offsets.G_MaterialIndex = 1828696;
                Offsets.G_LocalizedStringIndex = 1828808;
                Offsets.G_HudElems = 19830872;
                Offsets.HudElem_Alloc = 1574624;
                Offsets.Weapon_RocketLauncher_Fire = 1838352;
                Offsets.Dvar_GetString = 2584416;
                Offsets.G_AddEvent = 1820168;
                Offsets.cl_ingame = 30505612;
                Offsets.LocalPlayerName = 33157404;
                Offsets.G_FireGrenade = 1633800;
                Offsets.level_locals_t = 20015232;
                Offsets.levelTime = Offsets.level_locals_t + 940;
                Offsets.AllowPrecache = Offsets.level_locals_t + 28;
                Offsets.ServerDetails = 10134233;
                Offsets.SV_GameSendServerCommand = 2203808;
                Offsets.BG_GetWeaponDef = 207000;
                Offsets.G_GetPlayerViewOrigin = 1493824;
                Offsets.ObjectiveIndex = 20015268;
                Offsets.Trace_GetEntityHitID = 1905216;
                Offsets.G_LocationalTrace = 1607008;
                Offsets.G_Client = 21897728;
                Offsets.ClientAngles = 268;
                Offsets.ClientOrigin = 28;
                Offsets.ClientButtonMonitoring = 12765;
                Offsets.ClientName = 12808;
            }

            public Offsets()
            {
            }

            public class Funcs
            {
                public Funcs()
                {
                }


            }
        }


        public class HudStructLib
        {
            public static uint G_Hudelems;

            public static uint IndexSize;

            public static uint type;

            public static uint X;

            public static uint Y;

            public static uint Z;

            public static uint targetEntNum;

            public static uint fontScale;

            public static uint fromFontScale;

            public static uint fontScaleStartTime;

            public static uint fontScaleTime;

            public static uint label;

            public static uint font;

            public static uint alignOrg;

            public static uint alignScreen;

            public static uint color;

            public static uint fromColor;

            public static uint fadeStartTime;

            public static uint fadeTime;

            public static uint height;

            public static uint width;

            public static uint materialIndex;

            public static uint fromHeight;

            public static uint fromWidth;

            public static uint scaleStartTime;

            public static uint scaleTime;

            public static uint fromY;

            public static uint fromX;

            public static uint fromAlignOrg;

            public static uint fromAlignScreen;

            public static uint moveStartTime;

            public static uint moveTime;

            public static uint @value;

            public static uint time;

            public static uint duration;

            public static uint text;

            public static uint sort;

            public static uint glowColor;

            public static uint fxBirthTime;

            public static uint fxLetterTime;

            public static uint fxDecayStartTime;

            public static uint fxDecayDuration;

            public static uint soundID;

            public static uint flags;

            public static uint clientIndex;

            static HudStructLib()
            {
                HudStructLib.G_Hudelems = Offsets.G_HudElems;
                HudStructLib.IndexSize = 180;
                HudStructLib.type = 0;
                HudStructLib.X = 8;
                HudStructLib.Y = 4;
                HudStructLib.Z = 12;
                HudStructLib.targetEntNum = 16;
                HudStructLib.fontScale = 20;
                HudStructLib.fromFontScale = 24;
                HudStructLib.fontScaleStartTime = 28;
                HudStructLib.fontScaleTime = 32;
                HudStructLib.label = 36;
                HudStructLib.font = 40;
                HudStructLib.alignOrg = 44;
                HudStructLib.alignScreen = 48;
                HudStructLib.color = 52;
                HudStructLib.fromColor = 56;
                HudStructLib.fadeStartTime = 60;
                HudStructLib.fadeTime = 64;
                HudStructLib.height = 68;
                HudStructLib.width = 72;
                HudStructLib.materialIndex = 76;
                HudStructLib.fromHeight = 80;
                HudStructLib.fromWidth = 84;
                HudStructLib.scaleStartTime = 88;
                HudStructLib.scaleTime = 92;
                HudStructLib.fromY = 96;
                HudStructLib.fromX = 100;
                HudStructLib.fromAlignOrg = 104;
                HudStructLib.fromAlignScreen = 108;
                HudStructLib.moveStartTime = 112;
                HudStructLib.moveTime = 116;
                HudStructLib.@value = 120;
                HudStructLib.time = 124;
                HudStructLib.duration = 128;
                HudStructLib.text = 132;
                HudStructLib.sort = 136;
                HudStructLib.glowColor = 140;
                HudStructLib.fxBirthTime = 144;
                HudStructLib.fxLetterTime = 148;
                HudStructLib.fxDecayStartTime = 152;
                HudStructLib.fxDecayDuration = 156;
                HudStructLib.soundID = 160;
                HudStructLib.flags = 164;
                HudStructLib.clientIndex = 168;
            }

            public HudStructLib()
            {
            }

        }
        #endregion
        #endregion
        #region FPS Text

        public class Struct
        {
            public static uint
                Size = 0x4D56DC,//0x7291a4
                xPosition = 0x4D5950,
                yPosition = 0x4D5850,
                Text = 0x323AA8;
        }

        private static uint FPS(uint Offset = 0)
        {
            return 0x253AC8 + Offset;
        }

        public static void ToggleON()
        {
            Form1.PS3.Extension.WriteByte(FPS(), 0x40);
        }

        public static void ToggleOFF()
        {
            Form1.PS3.Extension.WriteByte(FPS(), 0x41);
            Write(" ", Convert.ToSingle((float)0.5), 25, 25);
            byte[] Text = new byte[] { 0x66, 0x70, 0x73, 0x3A, 0x20, 0x25, 0x66, 0x00, 0x66, 0x61, 0x64, 0x65, 0x69, 0x6E, 0x00, 0x00, 0x66, 0x61, 0x64, 0x65, 0x6F, 0x75, 0x74, 0x00, 0x73, 0x68, 0x6F, 0x77, 0x00, 0x00, 0x00, 0x00, 0x68, 0x69, 0x64, 0x65, 0x00, 0x00, 0x00, 0x00, 0x73, 0x68, 0x6F, 0x77, 0x4D, 0x65, 0x6E, 0x75, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x68, 0x69, 0x64, 0x65, 0x4D, 0x65, 0x6E, 0x75, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x73, 0x65, 0x74, 0x63, 0x6F, 0x6C, 0x6F, 0x72, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x6F, 0x70, 0x65, 0x6E, 0x00, 0x00, 0x00, 0x00, 0x63, 0x6C, 0x6F, 0x73, 0x65, 0x00, 0x00, 0x00, 0x63, 0x6C, 0x6F, 0x73, 0x65, 0x46, 0x6F, 0x72, 0x41, 0x6C, 0x6C, 0x50, 0x6C, 0x61, 0x79, 0x65, 0x72, 0x73, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x69, 0x6E, 0x67, 0x61, 0x6D, 0x65, 0x6F, 0x70, 0x65, 0x6E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x69, 0x6E, 0x67, 0x61, 0x6D, 0x65, 0x63, 0x6C, 0x6F, 0x73, 0x65, 0x00, 0x00, 0x00, 0x00, 0x00, 0x73, 0x65, 0x74, 0x62, 0x61, 0x63, 0x6B, 0x67, 0x72, 0x6F, 0x75, 0x6E, 0x64, 0x00, 0x00, 0x00, 0x73, 0x65, 0x74, 0x69, 0x74, 0x65, 0x6D, 0x63, 0x6F, 0x6C, 0x6F, 0x72, 0x00, 0x00, 0x00, 0x00, 0x66, 0x6F, 0x63, 0x75, 0x73, 0x66, 0x69, 0x72, 0x73, 0x74, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x73, 0x65, 0x74, 0x66, 0x6F, 0x63, 0x75, 0x73, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x73, 0x65, 0x74, 0x66, 0x6F, 0x63, 0x75, 0x73, 0x62, 0x79, 0x64, 0x76, 0x61, 0x72, 0x00, 0x00, 0x73, 0x65, 0x74, 0x64, 0x76, 0x61, 0x72, 0x00, 0x65, 0x78, 0x65, 0x63, 0x00, 0x00, 0x00, 0x00, 0x65, 0x78, 0x65, 0x63, 0x6E, 0x6F, 0x77, 0x00, 0x65, 0x78, 0x65, 0x63, 0x4F, 0x6E, 0x44, 0x76, 0x61, 0x72, 0x53, 0x74, 0x72, 0x69, 0x6E, 0x67, 0x56, 0x61, 0x6C, 0x75, 0x65, 0x00, 0x00, 0x00, 0x65, 0x78, 0x65, 0x63, 0x4F, 0x6E, 0x44, 0x76, 0x61, 0x72, 0x49, 0x6E, 0x74, 0x56, 0x61, 0x6C, 0x75, 0x65, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x65, 0x78, 0x65, 0x63, 0x4F, 0x6E, 0x44, 0x76, 0x61, 0x72, 0x46, 0x6C, 0x6F, 0x61, 0x74, 0x56, 0x61, 0x6C, 0x75, 0x65, 0x00, 0x00, 0x00, 0x00, 0x65, 0x78, 0x65, 0x63, 0x4E, 0x6F, 0x77, 0x4F, 0x6E, 0x44, 0x76, 0x61, 0x72, 0x53, 0x74, 0x72, 0x69, 0x6E, 0x67, 0x56, 0x61, 0x6C, 0x75, 0x65, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x65, 0x78, 0x65, 0x63, 0x4E, 0x6F, 0x77, 0x4F, 0x6E, 0x44, 0x76, 0x61, 0x72, 0x49, 0x6E, 0x74, 0x56, 0x61, 0x6C, 0x75, 0x65, 0x00, 0x00, 0x00, 0x65, 0x78, 0x65, 0x63, 0x4E, 0x6F, 0x77, 0x4F, 0x6E, 0x44, 0x76, 0x61, 0x72, 0x46, 0x6C, 0x6F, 0x61, 0x74, 0x56, 0x61, 0x6C, 0x75, 0x65, 0x00, 0x70, 0x6C, 0x61, 0x79, 0x00, 0x00, 0x00, 0x00, 0x73, 0x63, 0x72, 0x69, 0x70, 0x74, 0x6D, 0x65, 0x6E, 0x75, 0x72, 0x65, 0x73, 0x70, 0x6F, 0x6E, 0x73, 0x65, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x73, 0x63, 0x72, 0x69, 0x70, 0x74, 0x4D, 0x65, 0x6E, 0x75, 0x52, 0x65, 0x73, 0x70, 0x6F, 0x6E, 0x64, 0x4F, 0x6E, 0x44, 0x76, 0x61, 0x72, 0x53, 0x74, 0x72, 0x69, 0x6E, 0x67, 0x56, 0x61, 0x6C, 0x75, 0x65, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x73, 0x63, 0x72, 0x69, 0x70, 0x74, 0x4D, 0x65, 0x6E, 0x75, 0x52, 0x65, 0x73, 0x70, 0x6F, 0x6E, 0x64, 0x4F, 0x6E, 0x44, 0x76, 0x61, 0x72, 0x49, 0x6E, 0x74, 0x56, 0x61, 0x6C, 0x75, 0x65, 0x00, 0x73, 0x63, 0x72, 0x69, 0x70, 0x74, 0x4D, 0x65, 0x6E, 0x75, 0x52, 0x65, 0x73, 0x70, 0x6F, 0x6E, 0x64, 0x4F, 0x6E, 0x44, 0x76, 0x61, 0x72, 0x46, 0x6C, 0x6F, 0x61, 0x74, 0x56, 0x61, 0x6C, 0x75, 0x65, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x73, 0x65, 0x74, 0x50, 0x6C, 0x61, 0x79, 0x65, 0x72, 0x44, 0x61, 0x74, 0x61, 0x00, 0x00, 0x00, 0x73, 0x65, 0x74, 0x50, 0x6C, 0x61, 0x79, 0x65, 0x72, 0x44, 0x61, 0x74, 0x61, 0x53, 0x70, 0x6C, 0x69, 0x74, 0x53, 0x63, 0x72, 0x65, 0x65, 0x6E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x75, 0x70, 0x64, 0x61, 0x74, 0x65, 0x4D, 0x61, 0x69, 0x6C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x6F, 0x70, 0x65, 0x6E, 0x4D, 0x61, 0x69, 0x6C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x64, 0x65, 0x6C, 0x65, 0x74, 0x65, 0x4D, 0x61, 0x69, 0x6C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x64, 0x6F, 0x4D, 0x61, 0x69, 0x6C, 0x4C, 0x6F, 0x74, 0x74, 0x65, 0x72, 0x79, 0x00, 0x00, 0x00, 0x72, 0x65, 0x73, 0x65, 0x74, 0x53, 0x74, 0x61, 0x74, 0x73, 0x43, 0x6F, 0x6E, 0x66, 0x69, 0x72, 0x6D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x72, 0x65, 0x73, 0x65, 0x74, 0x53, 0x74, 0x61, 0x74, 0x73, 0x43, 0x61, 0x6E, 0x63, 0x65, 0x6C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x73, 0x65, 0x74, 0x47, 0x61, 0x6D, 0x65, 0x4D, 0x6F, 0x64, 0x65, 0x00, 0x00, 0x00, 0x00, 0x00, 0x73, 0x65, 0x74, 0x4C, 0x6F, 0x63, 0x61, 0x6C, 0x56, 0x61, 0x72, 0x42, 0x6F, 0x6F, 0x6C, 0x00, 0x73, 0x65, 0x74, 0x4C, 0x6F, 0x63, 0x61, 0x6C, 0x56, 0x61, 0x72, 0x49, 0x6E, 0x74, 0x00, 0x00, 0x73, 0x65, 0x74, 0x4C, 0x6F, 0x63, 0x61, 0x6C, 0x56, 0x61, 0x72, 0x46, 0x6C, 0x6F, 0x61, 0x74, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x73, 0x65, 0x74, 0x4C, 0x6F, 0x63, 0x61, 0x6C };
            Form1.PS3.SetMemory(0x00577570, Text);
            byte[] FPSreset = new byte[] { 0xD0, 0x1D, 0x00, 0x00, 0xC1, 0xBE, 0x00, 0x04, 0xC1, 0x9D, 0x00, 0x04, 0xD1, 0xBD, 0x00, 0x04, 0xC0, 0x1E, 0x00, 0x08, 0xC1, 0xBD, 0x00, 0x08, 0xD0, 0x1D, 0x00, 0x08, 0x81, 0x3D, 0x00, 0x0C, 0x80, 0x1E, 0x00, 0x0C, 0x90, 0x1D, 0x00, 0x0C, 0x91, 0x3E, 0x00, 0x0C, 0xD1, 0xBE, 0x00, 0x08, 0xD1, 0x9E, 0x00, 0x04, 0xD1, 0x7E, 0x00, 0x00, 0xF8, 0x41, 0x00, 0x28, 0x80, 0x1B, 0x00, 0x00, 0x7F, 0xC3, 0xF3, 0x78, 0x7F, 0x84, 0xE3, 0x78, 0x80, 0x5B, 0x00, 0x04, 0x7C, 0x09, 0x03, 0xA6, 0x4E, 0x80, 0x04, 0x21, 0xE8, 0x41, 0x00, 0x28, 0x54, 0x63, 0x06, 0x3E, 0x2F, 0x83, 0x00, 0x00, 0x41, 0x9E, 0x00, 0x44, 0xC0, 0x1C, 0x00, 0x00, 0xC1, 0x9E, 0x00, 0x00, 0xD0, 0x1E, 0x00, 0x00, 0xC1, 0xBC, 0x00, 0x04, 0xC1, 0x7E, 0x00, 0x04, 0xD1, 0xBE, 0x00, 0x04, 0xC0, 0x1C, 0x00, 0x08, 0xC1, 0xBE, 0x00, 0x08, 0xD0, 0x1E, 0x00, 0x08, 0x81, 0x3E, 0x00, 0x0C, 0x80, 0x1C, 0x00, 0x0C, 0x90, 0x1E, 0x00, 0x0C, 0xD1, 0x9C, 0x00, 0x00, 0x91, 0x3C, 0x00, 0x0C, 0xD1, 0xBC, 0x00, 0x08, 0xD1, 0x7C, 0x00, 0x04, 0xF8, 0x41, 0x00, 0x28, 0x80, 0x1B, 0x00, 0x00, 0x7F, 0xE3, 0xFB, 0x78, 0x7F, 0x44, 0xD3, 0x78, 0x80, 0x5B, 0x00, 0x04, 0x7C, 0x09, 0x03, 0xA6, 0x4E, 0x80, 0x04, 0x21, 0xE8, 0x41, 0x00, 0x28, 0x54, 0x63, 0x06, 0x3E, 0x2F, 0x83, 0x00, 0x00, 0x41, 0x9E, 0x00, 0x44, 0xC0, 0x1A, 0x00, 0x00, 0xC1, 0x7F, 0x00, 0x00, 0xD0, 0x1F, 0x00, 0x00, 0xC1, 0xBA, 0x00, 0x04, 0xC1, 0x9F, 0x00, 0x04, 0xD1, 0xBF, 0x00, 0x04, 0xC0, 0x1A, 0x00, 0x08, 0xC1, 0xBF, 0x00, 0x08, 0xD0, 0x1F, 0x00, 0x08, 0x81, 0x3F, 0x00, 0x0C, 0x80, 0x1A, 0x00, 0x0C, 0x90, 0x1F, 0x00, 0x0C, 0x91, 0x3A, 0x00, 0x0C, 0xD1, 0xBA, 0x00, 0x08, 0xD1, 0x9A, 0x00, 0x04, 0xD1, 0x7A, 0x00, 0x00, 0xF8, 0x41, 0x00, 0x28, 0x80, 0x1B, 0x00, 0x00, 0x7F, 0xC3, 0xF3, 0x78, 0x7F, 0xE4, 0xFB, 0x78, 0x80, 0x5B, 0x00, 0x04, 0x7C, 0x09, 0x03, 0xA6, 0x4E, 0x80, 0x04, 0x21, 0xE8, 0x41, 0x00, 0x28, 0x54, 0x63, 0x06, 0x3E, 0x2F, 0x83, 0x00, 0x00, 0x41, 0x9E, 0x00, 0x44, 0xC0, 0x1F, 0x00, 0x00, 0xC1, 0x7E, 0x00, 0x00, 0xD0, 0x1E, 0x00, 0x00, 0xC1, 0xBF, 0x00, 0x04, 0xC1, 0x9E, 0x00, 0x04, 0xD1, 0xBE, 0x00, 0x04, 0xC0, 0x1F, 0x00, 0x08, 0xC1, 0xBE, 0x00, 0x08, 0xD0, 0x1E, 0x00, 0x08, 0x81, 0x3E, 0x00, 0x0C, 0x80, 0x1F, 0x00, 0x0C, 0x90, 0x1E, 0x00, 0x0C, 0x91, 0x3F, 0x00, 0x0C, 0xD1, 0xBF, 0x00, 0x08, 0xD1, 0x9F, 0x00, 0x04, 0xD1, 0x7F, 0x00, 0x00, 0xF8, 0x41, 0x00, 0x28, 0x80, 0x1B, 0x00, 0x00, 0x7F, 0xE3, 0xFB, 0x78, 0x7F, 0x44, 0xD3, 0x78, 0x80, 0x5B, 0x00, 0x04, 0x7C, 0x09, 0x03, 0xA6, 0x4E, 0x80, 0x04, 0x21, 0xE8, 0x41, 0x00, 0x28, 0x54, 0x63, 0x06, 0x3E, 0x2F, 0x83, 0x00, 0x00, 0x41, 0x9E, 0x00, 0x44, 0xC0, 0x1A, 0x00, 0x00, 0xC1, 0x9F, 0x00, 0x00, 0xD0, 0x1F, 0x00, 0x00, 0xC1, 0xBA, 0x00, 0x04, 0xC1, 0x7F, 0x00, 0x04, 0xD1, 0xBF, 0x00, 0x04, 0xC0, 0x1A, 0x00, 0x08, 0xC1, 0xBF, 0x00, 0x08, 0xD0, 0x1F, 0x00, 0x08, 0x81, 0x3F, 0x00, 0x0C, 0x80, 0x1A, 0x00, 0x0C, 0x90, 0x1F, 0x00, 0x0C, 0xD1, 0x9A, 0x00, 0x00, 0x91, 0x3A, 0x00, 0x0C, 0xD1, 0xBA, 0x00, 0x08, 0xD1, 0x7A, 0x00, 0x04, 0x7F, 0x98, 0xC8, 0x40, 0x3A, 0xF9, 0x00, 0x10, 0x41, 0x9C, 0x00, 0x34, 0x48, 0x00, 0x07, 0x90, 0xF8, 0x41, 0x00, 0x28, 0x80, 0x1B, 0x00, 0x00, 0x80, 0x5B, 0x00, 0x04, 0x7C, 0x09, 0x03, 0xA6, 0x4E, 0x80, 0x04, 0x21, 0xE8, 0x41, 0x00, 0x28, 0x7F, 0x18, 0xC8, 0x40, 0x54, 0x63, 0x06, 0x3E, 0x2F, 0x83, 0x00, 0x00, 0x40, 0x9E, 0x00, 0x50, 0x40, 0x98, 0x07, 0x64, 0x38, 0x19, 0xFF, 0xF0, 0xF8, 0x41, 0x00, 0x28, 0x78, 0x1D, 0x00, 0x20, 0x81, 0x3B, 0x00, 0x00, 0x7F, 0xE4, 0xFB, 0x78, 0x80, 0x5B, 0x00, 0x04, 0x7D, 0x29, 0x03, 0xA6, 0x7F, 0xA3, 0xEB, 0x78, 0x7F, 0x3A, 0xCB, 0x78, 0x7C, 0x19, 0x03, 0x78, 0x4E, 0x80, 0x04, 0x21, 0xE8, 0x41, 0x00, 0x28, 0x7F, 0xA4, 0xEB, 0x78, 0x54, 0x60, 0x06, 0x3E, 0x7F, 0xE3, 0xFB, 0x78, 0x2F, 0x80, 0x00, 0x00, 0x7F, 0xBF, 0xEB, 0x78, 0x41, 0x9E, 0xFF, 0x90, 0x7F, 0x96, 0xB8, 0x40, 0x7E, 0xE0, 0xBB, 0x78, 0x7B, 0x5E, 0x00, 0x20, 0x41, 0x9D, 0x00, 0x3C, 0x48, 0x00, 0x00, 0x6C, 0xF8, 0x41, 0x00, 0x28, 0x80, 0x1B, 0x00, 0x00, 0x80, 0x5B, 0x00, 0x04, 0x7C, 0x09, 0x03, 0xA6, 0x4E, 0x80, 0x04, 0x21, 0xE8, 0x41, 0x00, 0x28, 0x38, 0x17, 0x00, 0x10, 0x54, 0x63, 0x06, 0x3E, 0x7F, 0x16, 0x00, 0x40, 0x2F, 0x83, 0x00, 0x00, 0x40, 0x9E, 0x00, 0x40, 0x7C, 0x17, 0x03, 0x78, 0x40, 0x99, 0x06, 0xF8, 0xF8, 0x41, 0x00, 0x28, 0x78, 0x03, 0x00, 0x20, 0x80, 0x1B, 0x00, 0x00, 0x7F, 0xC4, 0xF3, 0x78, 0x80, 0x5B, 0x00, 0x04, 0x7C, 0x09, 0x03, 0xA6, 0x4E, 0x80, 0x04, 0x21, 0xE8, 0x41, 0x00, 0x28, 0x7A, 0xE4, 0x00, 0x20, 0x54, 0x60, 0x06, 0x3E, 0x7F, 0xC3, 0xF3, 0x78, 0x2F, 0x80, 0x00, 0x00, 0x41, 0x9E, 0xFF, 0x9C, 0x7E, 0xF9, 0xBB, 0x78, 0x7F, 0x5C, 0xD3, 0x78, 0x7F, 0x96, 0xC8, 0x40, 0x41, 0x9D, 0x01, 0x3C, 0x7F, 0x98, 0xE0, 0x40, 0x7B, 0x5E, 0x00, 0x20, 0x41, 0x9C, 0x00, 0x14, 0x48, 0x00, 0x01, 0xE4, 0x7F, 0x98, 0xF8, 0x40, 0x7F, 0xFC, 0xFB, 0x78, 0x40, 0x9C, 0x00, 0xB8, 0x3B, 0xFC, 0xFF, 0xF0, 0xF8, 0x41, 0x00, 0x28, 0x80, 0x1B, 0x00, 0x00 };
            Form1.PS3.SetMemory(0x4D56DC, FPSreset);
        }

        public static void Write(string Text, double FontSize, float X, float Y)
        {
            Form1.PS3.Extension.WriteFloat(FPS(Struct.Size), (float)FontSize); //Font size is a float (ALWAYS)
            Form1.PS3.Extension.WriteString(FPS(Struct.Text), Text); //Writes your input directly to the FPS string
            Form1.PS3.Extension.WriteFloat(FPS(Struct.xPosition), X); //X Position is ALWAYS a float (Huds, Origin, Angles etc.)
            Form1.PS3.Extension.WriteFloat(FPS(Struct.yPosition), Y); //Y Position is ALWAYS a float (Huds, Origin, Angles etc.)
        }

        public static void WriteTxt(string Text)
        {
            Form1.PS3.Extension.WriteString(FPS(Struct.Text), Text); //Writes your input directly to the FPS string
        }

        public static void WriteSize(float FontSize)
        {
            Form1.PS3.Extension.WriteFloat(FPS(Struct.Size), FontSize); //Font size is a float (ALWAYS)
        }

        public static void WritePos(float X, float Y)
        {
            Form1.PS3.Extension.WriteFloat(FPS(Struct.xPosition), X); //X Position is ALWAYS a float (Huds, Origin, Angles etc.)
            Form1.PS3.Extension.WriteFloat(FPS(Struct.yPosition), Y); //Y Position is ALWAYS a float (Huds, Origin, Angles etc.)
        }
        #endregion
        #region Functions

        public static int getHostNum()
        {
            if (inMP())
                return Form1.PS3.Extension.ReadByte(0x00A14B67);
            else
                return 0;
        }

        public static Boolean onlineMatch()
        {
            if (Form1.PS3.Extension.ReadInt32(0x367345D8) != 0x00)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static string getName(int client)
        {
            return Form1.PS3.Extension.ReadString(0x014E5490 + (uint)client * 0x3700);
        }
        public static void setName(int client, string txt)
        {
            if (getName(client) != "")
                Form1.PS3.Extension.WriteString(0x014E5490 + (uint)client * 0x3700, txt + "\0");
        }
        public static class NonHostButtons
        {
            public static uint
                R1 = 0x4D,
                R2 = 0xE9,
                R3 = 0xD1,
                L1 = 0x41,
                L2 = 0xDD,
                L3 = 0xC5,
                DpadUp = 0xF5,
                DpadDown = 0x105,
                DpadLeft = 0x10D,
                DpadRight = 0x119,
                Cross = 0x11,
                Square = 0x29,
                Circle = 0x1D,
                Triangle = 0x35,
                Select = 0xB9,
                Start = 0xAD;
        }

        public static bool NonHostButtonPressed(uint Button)
        {
            if (Form1.PS3.Extension.ReadByte(0x0095C08A + Button) == 0x01)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool L1Press(int client)
        {
            if (Form1.PS3.Extension.ReadByte(0x014e562d + 0x3700 * (uint)client) == 0x08)
                return true;
            else
                return false;
        }
        public static bool sqPress(int client)
        {
            if (Form1.PS3.Extension.ReadByte(0x014e563b + 0x3700 * (uint)client) == 0x20)
                return true;
            else
                return false;
        }
        public static bool L2Press(int client)
        {
            if (Form1.PS3.Extension.ReadByte(0x014e53b6 + 0x3700 * (uint)client) == 0x80)
                return true;
            else
                return false;
        }
        public static bool R2Press(int client)
        {
            if (Form1.PS3.Extension.ReadByte(0x014e53b6 + 0x3700 * (uint)client) == 0x40)
                return true;
            else
                return false;
        }
      
        public class Buttons
        {
            public static string
                DpadUp = "+actionslot 1",
                DpadDown = "+actionslot 2",
                DpadRight = "+actionslot 4",
                DpadLeft = "+actionslot 3",
                Cross = "+gostand",
                Circle = "+stance",
                Triangle = "weapnext",
                Square = "+usereload",
                R3 = "+melee",
                R2 = "+frag",
                R1 = "+attack",
                L3 = "+breath_sprint",
                L2 = "+smoke",
                L1 = "+speed_throw",
                Select = "togglescores",
                Start = "togglemenu";
        }

        public static bool ButtonPressed(int client, string Button)
        {
            if (inMP())
            {
                if (Form1.PS3.Extension.ReadString(0x34750E9F + ((uint)client * 0x97F80)) == Button)
                    return true;
                else return false;
            }
            else
            {
                if (Form1.PS3.Extension.ReadString(0x34760E9F + ((uint)client * 0x97F80)) == Button)
                    return true;
                else return false;
            }
        }

        public static bool L1_R3(int client)
        {
            if (Form1.PS3.Extension.ReadByte(0x34750e71 + ((uint)client * 0x97F80)) == 0x08 && Form1.PS3.Extension.ReadByte(0x34750e73 + ((uint)client * 0x97F80)) == 0x04)
                return true;
            else return false;
        }
        public static bool inMP()
        {
            return Form1.PS3.Extension.ReadBool(0x00A1147C);
        }
        public static String KeyBoard(String Title, String PresetText = "", Int32 MaxLength = 20)
        {
            Call(0x238070, 0, Title, PresetText, MaxLength, 0x70B4D8);
            while (Form1.PS3.Extension.ReadInt32(0x203B4C8) != 0) continue;
            return Form1.PS3.Extension.ReadString(0x2510E22);
        }

        public static Boolean IsMW2()
        {
            foreach (String temp in new String[] { "BLUS30377", "BLKS20159", "BLES00683", "BLES00686", "BLES00685", "BLES00684", "BLES00687" })
                if (temp == Form1.PS3.Extension.ReadString(0x10010251))
                    return true;
            return false;
        }
        public static void KFnotify(string txt)
        {
            Form1.PS3.SetMemory(0x318D5B07, Encoding.ASCII.GetBytes("&&1\0"));
            Form1.PS3.SetMemory(0x2005000, Encoding.UTF8.GetBytes("\nset CbufFIX \"set CbufFIX vstr CbufFIXoff;\\\"" + txt + "\\\".\"" + "\0"));
            Form1.PS3.SetMemory(0x253AB8, new byte[] { 0x38, 0x60, 0x00, 0x00, 0x3C, 0x80, 0x02, 0x00, 0x30, 0x84, 0x50, 0x00, 0x4B, 0xF8, 0x63, 0xFD });
            Thread.Sleep(100);
            Form1.PS3.SetMemory(0x2005000, Encoding.UTF8.GetBytes("\nvstr CbufFIX" + ";\0"));
            Thread.Sleep(100);
            Form1.PS3.SetMemory(0x253AB8, new byte[] { 0x81, 0x22, 0x45, 0x10, 0x81, 0x69, 0x00, 0x00, 0x88, 0x0B, 0x00, 0x0C, 0x2F, 0x80, 0x00, 0x00 });
        }

        public static void cBuff_AddText_Fix(string dvar)
        {
            Form1.PS3.SetMemory(0x2005000, Encoding.UTF8.GetBytes("\nset CbufFIX \"set CbufFIX vstr CbufFIXoff;" + dvar + "\"" + "\0"));
            Form1.PS3.SetMemory(0x253AB8, new byte[] { 0x38, 0x60, 0x00, 0x00, 0x3C, 0x80, 0x02, 0x00, 0x30, 0x84, 0x50, 0x00, 0x4B, 0xF8, 0x63, 0xFD });
            Thread.Sleep(100);
            Form1.PS3.SetMemory(0x2005000, Encoding.UTF8.GetBytes("\nvstr CbufFIX" + ";\0"));
            Thread.Sleep(100);
            Form1.PS3.SetMemory(0x253AB8, new byte[] { 0x81, 0x22, 0x45, 0x10, 0x81, 0x69, 0x00, 0x00, 0x88, 0x0B, 0x00, 0x0C, 0x2F, 0x80, 0x00, 0x00 });
        }

        public static void cBuff_AddText_Reg(string dvar)
        {
            Form1.PS3.SetMemory(0x2005000, Encoding.UTF8.GetBytes(dvar + ";\0"));
            Form1.PS3.SetMemory(0x253AB8, new byte[] { 0x38, 0x60, 0x00, 0x00, 0x3C, 0x80, 0x02, 0x00, 0x30, 0x84, 0x50, 0x00, 0x4B, 0xF8, 0x63, 0xFD });
            Thread.Sleep(15);
            Form1.PS3.SetMemory(0x253AB8, new byte[] { 0x81, 0x22, 0x45, 0x10, 0x81, 0x69, 0x00, 0x00, 0x88, 0x0B, 0x00, 0x0C, 0x2F, 0x80, 0x00, 0x00 });
        }
        public static void cBuff_AddText_RPC(string dvar)
        {
            Call(0x001D9EC0, 1, dvar);
        }
        public static void SV_GameSendServerCommand(Int32 clientIndex, String Cmd)
        {
            Call(0x0021A0A0, clientIndex, 0, Cmd);
        }
        public static void setClientDvar(int clientNumber, string dvar, string Val)
        {
            SV_GameSendServerCommand(clientNumber, "v " + dvar + " \"" + Val + "\"");
        }
        public static void setClientJustDvar(int clientNumber, string dvar)
        {
            SV_GameSendServerCommand(clientNumber, "v " + dvar);
        }
        public static void iPrintln(int clientNumber, string Txt)
        {
            SV_GameSendServerCommand(clientNumber, "f \"" + Txt + "\"");
        }
        public static void iPrintlnBold(int clientNumber, string Txt)
        {
            SV_GameSendServerCommand(clientNumber, "g \"" + Txt + "\"");
        }
        public static void Notivation(int clientNumber, string Txt)
        {
            SV_GameSendServerCommand(clientNumber, "g \"" + "                                                                      " + Txt + "\"");
        }
        public static void playSound(int clientNumber, string soundName)
        {
            SV_GameSendServerCommand(clientNumber, "o \"" + soundName + "\"");
        }

        public static void SV_SendServerCommand(int clientIndex, string Command)
        {
            WritePowerPc(true);
            Form1.PS3.Extension.WriteString(0x10040010, Command);
            Form1.PS3.Extension.WriteInt32(0x10040004, clientIndex);
            Form1.PS3.Extension.WriteBool(0x10040003, true);
            bool isRunning;
            do { isRunning = Form1.PS3.Extension.ReadBool(0x10040003); } while (isRunning != false);
            WritePowerPc(false);
        }

        public static string getDetails(string type)
        {
            string str = Form1.PS3.Extension.ReadString(Offsets.ServerDetails);
            string[] strArrays = str.Split(new char[] { '\\' });
            int length = str.Length;
            int num = 0;
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                if (strArrays[i] == type)
                {
                    i++;
                    num = i;
                }
            }
            return strArrays[num];
        }

        #region server details
        #region Boolean's
        public static Boolean inGame()
        {
            if (Form1.PS3.Extension.ReadByte(0x0179b12f) != 0x00)
                return true;
            else
                return false;
        }
        private static String ReturnInfos(Int32 Index)
        {
            byte[] buffer = new byte[0x234];
            Form1.PS3.GetMemory(0x17A54E0, buffer);
            return Encoding.ASCII.GetString(buffer).Replace(@"\", "|").Split('|')[Index];
        }
        private static String OnlineInfos(Int32 Index)
        {
            byte[] buffer = new byte[0x234];
            Form1.PS3.GetMemory(0x009aa2d9, buffer);
            return Encoding.ASCII.GetString(buffer).Replace(@"\", "|").Split('|')[Index];
        }
        private static Boolean Online()
        {
            if (ReturnInfos(1) == "cg_predictItems")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
        #region Host Name?
        public static String getHostName()
        {
            if (inGame())
            {
                if (Online())
                {
                    return ReturnInfos(8);
                }
                else
                {
                    return ReturnInfos(16);
                }
            }
            else { return "Not in game"; }
        }
        #endregion
        #region Max Players?
        public static String getMaxPlayers()
        {
            if (inGame())
            {
                if (Online())
                {
                    return OnlineInfos(18);
                }
                else
                {
                    return ReturnInfos(18);
                }
            }
            else { return "Not in game"; }
        }
        #endregion
        #region Gamemode
        public static String getGameMode()
        {
            if (inGame())
            {
                if (Online())
                {
                    //Online
                    switch (OnlineInfos(2))
                    {
                        default: return "Unknown Gametype";
                        case "1": return "Loading Game";
                        case "war": return "Team Deathmatch";
                        case "dm": return "Free for All";
                        case "sd": return "Search and Destroy";
                        case "dom": return "Domination";
                        case "dem": return "Demolition";
                        case "gtnw": return "Global Thermonuclear War";
                        case "ctf": return "Capture The Flag";
                        case "arena": return "Arena";
                    }
                }
                else
                {
                    //Private Match
                    switch (ReturnInfos(2))
                    {
                        default: return "Unknown Gametype";
                        case "1": return "Loading Game";
                        case "war": return "Team Deathmatch";
                        case "dm": return "Free for All";
                        case "sd": return "Search and Destroy";
                        case "dom": return "Domination";
                        case "dem": return "Demolition";
                        case "gtnw": return "Global Thermonuclear War";
                        case "ctf": return "Capture The Flag";
                        case "arena": return "Arena";
                    }
                }
            }
            else { return "Not in game"; }
        }
        #endregion
        #region Hardcore?
        public static String getHardcore()
        {
            if (inGame())
            {
                if (Online())
                {
                    switch (OnlineInfos(4))
                    {
                        default: return "Unknown Gametype";
                        case "20000": return "Loading Game";
                        case "0": return "Off";
                        case "1": return "On";
                    }
                }
                else
                {
                    switch (ReturnInfos(4))
                    {
                        default: return "Unknown Gametype";
                        case "20000": return "Loading Game";
                        case "0": return "Off";
                        case "1": return "On";
                    }
                }
            }
            else { return "Not in game"; }
        }
        #endregion
        #region Map
        public static String get_MapName()
        {
            String str = Form1.PS3.Extension.ReadString(0xD495F4), MapStr = "Not in game";
            if (inGame())
            {
                if (str.Contains("afghan"))
                    MapStr = "Afghan";
                if (str.Contains("highrise"))
                    MapStr = "Highrise";
                if (str.Contains("rundown"))
                    MapStr = "Rundown";
                if (str.Contains("quarry"))
                    MapStr = "Quarry";
                if (str.Contains("nightshift"))
                    MapStr = "Skidrow";
                if (str.Contains("terminal"))
                    MapStr = "Terminal";
                if (str.Contains("brecourt"))
                    MapStr = "Wasteland";
                if (str.Contains("derail"))
                    MapStr = "Derail";
                if (str.Contains("estate"))
                    MapStr = "Estate";
                if (str.Contains("favela"))
                    MapStr = "Favela";
                if (str.Contains("invasion"))
                    MapStr = "Invasion";
                if (str.Contains("rust"))
                    MapStr = "Rust";
                if (str.Contains("scrapyard") || str.Contains(("boneyard")))
                    MapStr = "Scrapyard";
                if (str.Contains("sub"))
                    MapStr = "Sub Base";
                if (str.Contains("underpass"))
                    MapStr = "Underpass";
                if (str.Contains("checkpoint"))
                    MapStr = "Karachi";
                if (str.Contains("bailout"))
                    MapStr = "Bailout";
                if (str.Contains("compact"))
                    MapStr = "Salvage";
                if (str.Contains("storm") || str.Contains(("storm2")))
                    MapStr = "Storm";
                if (str.Contains("crash"))
                    MapStr = "Crash";
                if (str.Contains("overgrown"))
                    MapStr = "Overgrown";
                if (str.Contains("strike"))
                    MapStr = "Strike";
                if (str.Contains("vacant"))
                    MapStr = "Vacant";
                if (str.Contains("trailerpark"))
                    MapStr = "Trailer Park";
                if (str.Contains("fuel"))
                    MapStr = "Fuel";
                if (str.Contains("abandon"))
                    MapStr = "Carnival";
                if (str.Contains("dlc2_ui_mp"))
                    MapStr = "Not in game";
            }
            return MapStr;
        }
        #endregion
        #endregion
        #endregion
    }
}