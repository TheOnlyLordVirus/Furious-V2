using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Text.RegularExpressions;
using System.IO;
using System.Threading;

using XDevkit;
using LordVirusMw2XboxLib;

#nullable enable

namespace Furious;

public partial class Form1 : Form
{
    private IXboxManager XboxManager;
    private IXboxConsole Xbox;

    private const int _maxClientCount = 18;
    private G_Client?[] CurrentGameClients = new G_Client?[_maxClientCount];

    private void Internal_RefreshClients()
    {
        if (Xbox is null)
            return;

        for (int clientIndex = 0; clientIndex < _maxClientCount; ++clientIndex)
        {
            if (CurrentGameClients[clientIndex] is null)
                CurrentGameClients[clientIndex] = new G_Client(Xbox!, clientIndex);
        }
    }

    #region connection

    private void Internal_Connect()
    {
        try
        {
            XboxManager = new XboxManager();
            Xbox = XboxManager.OpenConsole(XboxManager.DefaultConsole);

            Mw2GameFunctions.Cbuf_AddText(Xbox, "loc_warningsUI 0; loc_warnings 0; cg_blood 0; cg_bloodLimit 1;");

            // Init clients
            Internal_RefreshClients();

            // If client 0 is in this game enable noclip for them.
            CurrentGameClients[0]?.NoClip.Enable();

            MessageBox.Show
            (
                "Successfuly Connected!",
                "Connection",
                MessageBoxButtons.OK
            );
        }

        catch (Exception ex)
        {

            MessageBox.Show
            (
                ex.Message,
                "Connect Error",
                MessageBoxButtons.OK
            );
        }
    }


    private void button1_Click(object sender, EventArgs e)
    {
        Internal_Connect();

        //string connect = "Not Connected";
        //string attach = "Not Attached";
        //bool canConnect = false;
        //try
        //{
        //    if (colorRadioButton2.Checked == true) { PS3.ChangeAPI(SelectAPI.TargetManager); canConnect = PS3.ConnectTarget(); }
        //    else if (colorRadioButton3.Checked == true) { new CcapiDialog().ShowDialog(); if (CcapiDialog.tryConnect == true) canConnect = PS3.ConnectTarget(CcapiDialog.ccapiIp); }


        //    if (canConnect == true)
        //    {
        //        connect = "Connected";
        //        try
        //        {
        //            if (PS3.AttachProcess())
        //            {
        //                attach = "Attached";
        //                AfterAttach();
        //            }
        //            else
        //            {
        //                attach = "Cannot Attach";
        //            }
        //        }
        //        catch
        //        {
        //            attach = "Impossible to Attach";
        //        }
        //    }
        //    else
        //    {
        //        connect = "Cannot Connect";
        //    }
        //}
        //catch
        //{
        //    connect = "Impossible To Connect";
        //}
        //label2.Text = connect + " | " + attach;


    }
    //private void PS3api()
    //{
    //    if (PS3.GetCurrentAPIName() == "Target Manager")
    //        PS3.ConnectTarget();
    //    else if (PS3.GetCurrentAPIName() == "Control Console")
    //        PS3.ConnectTarget(CcapiDialog.ccapiIp);
    //}
    #endregion
    #region Attach PS3
    //bool setOnce = true;
    //private void AfterAttach()
    //{
    //    RPC.EnableRPC();
    //    if (setOnce)
    //    {
    //        setOnce = false;
    //        //for (int i = 0; i < 24; i++)
    //        //    getDefaultText(zoneOft[i]);

    //        resetR1L1();
    //        //nameChanger.Start();
    //        //playersRGB.Start();
    //        //gameMods.Start();
    //        //ipLog.Start();
    //        //autoFire.Start();
    //        //if (!aimbotProcess.IsBusy)
    //        //    aimbotProcess.RunWorkerAsync();

    //        //if (!clientMods.IsBusy)
    //        //    clientMods.RunWorkerAsync();

    //        //if (!allClientProcess.IsBusy)
    //        //    allClientProcess.RunWorkerAsync();
    //    }
    //}
    #endregion
    #region Aimbot 2
    private void aimbotProcess_DoWork(object sender, DoWorkEventArgs e)
    {
        PS3api();
        for (; ; )
            nonHostAimbot();
    }
    int aFtog = 0;
    string aimbotMethod = "";
    bool runAimbot = false;
    bool useAuto = true;

    private void resetR1L1()
    {
        PS3.Extension.WriteBytes(0x0095b080, new byte[] { 0x00 });
        PS3.Extension.WriteBytes(0x0095afe0, new byte[] { 0x00 });
    }

    private void enableAimbot(string method)
    {
        aimbotMethod = method;
        runAimbot = true;
        enableAA();
    }

    private void enableAA()
    {
        if (runAimbot)
            RPC.cBuff_AddText_RPC("aim_slowdown_enabled 1;aim_slowdown_yaw_scale_ads 0;aim_slowdown_pitch_scale_ads 0;aim_target_sentient_radius 128;aim_autoaim_enabled 1;SelectStringTableEntryInDvar M M aim_autoAimRangeScale;SelectStringTableEntryInDvar M M aim_aimAssistRangeScale;aim_autoaim_region_height 480;aim_autoaim_region_width 640;aim_lockon_region_height 480;aim_lockon_region_width 640;aim_lockon_strength 1;aim_lockon_deflection 0;aim_lockon_enabled 1;aim_autoaim_lerp 100");
    }

    private void disableAA()
    {
        RPC.cBuff_AddText_RPC("reset aim_slowdown_yaw_scale_ads;reset aim_slowdown_pitch_scale_ads;reset aim_target_sentient_radius;reset aim_autoaim_enabled;reset aim_autoAimRangeScale;reset aim_aimAssistRangeScale;reset aim_autoaim_region_height 480;reset aim_autoaim_region_width;reset aim_lockon_region_height;reset aim_lockon_region_width;reset aim_lockon_strength;reset aim_lockon_deflection;reset aim_lockon_enabled;reset aim_autoaim_lerp");
    }

    private void fairMode()
    {
        for (int i = 0; i < 5; i++)
        {
            PS3.Extension.WriteByte(0x008B0673, 1);
        }
        PS3.Extension.WriteBytes(0x008AC9EF, new byte[] { 0xFF, 0x00, 0x00, 0x07, 0xFF, 0x00 });
    }
    bool checkAuto = false;
    private void autoFireBot()
    {
        if (PS3.Extension.ReadByte(0x008ac9ef) != 0x00)
        {
            if (aFtog == 0 || aFtog == 1)
            {
                PS3.Extension.WriteBytes(0x0095afe0, new byte[] { 0x01 });//aim
                if (useAuto)
                    PS3.Extension.WriteBytes(0x0095b080, new byte[] { 0x01 });//autofire
                else
                    checkAuto = true;
                aFtog = 2;

            }
            PS3.Extension.WriteBytes(0x008AC9F4, new byte[] { 0x00 });//aa

        }
        else
        {
            if (aFtog == 0 || aFtog == 2)
            {
                PS3.Extension.WriteBytes(0x0095afe0, new byte[] { 0x00 });//aim
                if (useAuto)
                    PS3.Extension.WriteBytes(0x0095b080, new byte[] { 0x00 });//autofire
                else
                    checkAuto = false;
                aFtog = 1;

            }
        }
    }

    private void colorCheckBox16_CheckedChanged(object sender, EventArgs e)
    {
        useAuto = colorCheckBox16.Checked;
        if (!useAuto)
            PS3.Extension.WriteBytes(0x0095b080, new byte[] { 0x00 });
    }

    private void nonHostAimbot()
    {
        if (runAimbot)
        {
            if (aimbotMethod == "bot")
                autoFireBot();
            if (RPC.NonHostButtonPressed(RPC.NonHostButtons.L1))
            {
                if (aimbotMethod == "normal")
                    PS3.Extension.WriteBytes(0x008AC9F4, new byte[] { 0x00 });
                else if (aimbotMethod == "fair")
                    fairMode();
            }
            else
            {
                if (aimbotMethod == "fair")
                    PS3.Extension.WriteByte(0x008B0673, 1);
            }
        }
    }

    private void colorRadioButton22_CheckedChanged(object sender, EventArgs e)
    {
        enableAimbot("normal");
    }

    private void colorRadioButton23_CheckedChanged(object sender, EventArgs e)
    {
        enableAimbot("fair");
    }

    private void colorRadioButton25_CheckedChanged(object sender, EventArgs e)
    {
        enableAimbot("bot");
    }

    private void colorRadioButton21_CheckedChanged(object sender, EventArgs e)
    {
        aimbotMethod = "";
        runAimbot = false;
        PS3.Extension.WriteBytes(0x0095afe0, new byte[] { 0x00 });
        PS3.Extension.WriteBytes(0x0095b080, new byte[] { 0x00 });
        disableAA();
    }
    #endregion
    #region RTE Changers
    #region Changer Timer
    string[] cardT = { };
    string[] cardI = { };
    private void nameChanger_Tick(object sender, EventArgs e)
    {
        if (colorCheckBox14.Checked)
            RPC.cBuff_AddText_Reg("setPlayerData cardTitle " + cardT[new Random().Next(0, cardT.Length)]);
        if (colorCheckBox15.Checked)
            RPC.cBuff_AddText_Reg("setPlayerData cardIcon " + cardI[new Random().Next(0, cardI.Length)]);

        if (!colorRadioButton6.Checked)
        {
            string flashInt = "";
            if (colorRadioButton4.Checked)
                flashInt = "^" + new Random().Next(0, 8);
            runNameChanger(flashInt);
        }
        if (colorCheckBox2.Checked)
        {
            if (colorComboBox3.SelectedIndex == 0)
                PS3.SetMemory(nameOfs, Encoding.ASCII.GetBytes(rainbowChanger(textBox2.Text)));
            else if (colorComboBox3.SelectedIndex == 1)
                PS3.SetMemory(nameOfs, Encoding.ASCII.GetBytes(slideChanger(textBox2.Text)));
        }

        if (!colorRadioButton10.Checked)
        {
            if (colorRadioButton12.Checked)
                lvlNum = new Random().Next(0, 70);
            else if (colorRadioButton11.Checked)
            {
                lvlNum++;
                if (lvlNum >= 70)
                    lvlNum = 0;
            }

            PS3.SetMemory(lvlOfs, BitConverter.GetBytes(levels[lvlNum]));
        }
        if (!colorRadioButton7.Checked)
        {
            if (colorRadioButton9.Checked)
                prgNum = new Random().Next(0, 12);
            else if (colorRadioButton8.Checked)
            {
                prgNum++;
                if (prgNum > 11)
                    prgNum = 0;
            }
            PS3.SetMemory(prgOfs, BitConverter.GetBytes(prgNum));
        }
        if (!colorRadioButton13.Checked)
        {
            string inGameFlashInt = "";
            if (colorRadioButton15.Checked)
                inGameFlashInt = "^" + new Random().Next(0, 8);
            RPC.setName(colorComboBox4.SelectedIndex, "^7" + inGameFlashInt + textBox3.Text);
        }
        if (!colorRadioButton26.Checked)
        {
            string inGameFlashInt = "";
            if (colorRadioButton28.Checked)
                inGameFlashInt = "^" + new Random().Next(0, 8);
            RPC.cBuff_AddText_Fix("say ^7" + inGameFlashInt + textBox4.Text);
        }

        if (!colorRadioButton31.Checked)
        {
            string inGameFlashInt = "";
            if (colorRadioButton33.Checked)
                inGameFlashInt = "^" + new Random().Next(0, 8);

            int index = dataGridView2.CurrentRow.Index;
            if (PS3.Extension.ReadByte(clientActiveOfs + (uint)index * 0x3700) == 0x02)
                RPC.SV_GameSendServerCommand(-1, "h \"^8" + PS3.Extension.ReadString(0x14e5490 + 0x3700 * (uint)index) + "^7: " + inGameFlashInt + textBox6.Text + "\"" + "\0");
            else
            {
                colorRadioButton31.Checked = true;
                MessageBox.Show("Select an active client from the list", "NOTICE", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        if (!colorRadioButton44.Checked)
        {
            string inGameFlashInt = "";
            if (colorRadioButton46.Checked)
                inGameFlashInt = "^" + new Random().Next(0, 8);
            RPC.cBuff_AddText_Reg("set doInfection \"ui_mapname \\\"\"mp_rust;^7" + inGameFlashInt + richTextBox1.Text + "\";vstr doInfection");
            PS3.SetMemory(prgOfs, BitConverter.GetBytes(new Random().Next(0, 12)));
        }
        if (!colorRadioButton47.Checked)
        {
            string inGameFlashInt = "";
            if (colorRadioButton49.Checked)
                inGameFlashInt = "^" + new Random().Next(0, 8);
            RPC.cBuff_AddText_Reg("ui_gametype ^7" + inGameFlashInt + richTextBox2.Text);
            PS3.SetMemory(prgOfs, BitConverter.GetBytes(new Random().Next(0, 12)));
        }
    }
    #endregion
    #region Normal Changers
    uint nameOfs = 0x01f9f11c;
    byte[] nameVal = new byte[] { };
    string[] nameButtons = new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "" };
    string verticalN = "";
    private void button16_Click(object sender, EventArgs e)
    {
        runNameChanger("");
    }

    private void runNameChanger(string flash)
    {
        if (colorComboBox2.SelectedIndex == 0)
            nameVal = Encoding.ASCII.GetBytes("^7" + flash + textBox1.Text + "\0");
        else if (colorComboBox2.SelectedIndex == 1)
            nameVal = Encoding.ASCII.GetBytes("^7" + flash + textBox1.Text + "\r " + "^7" + flash + textBox1.Text + "\0");
        else if (colorComboBox2.SelectedIndex == 2)
        {
            int num = new Random().Next(0, 13);
            nameVal = Encoding.ASCII.GetBytes(nameButtons[num] + " ^7" + flash + textBox1.Text + " " + nameButtons[num] + "\0");
        }
        else if (colorComboBox2.SelectedIndex == 3)
            nameVal = Encoding.ASCII.GetBytes(verticalN + "^7" + flash + textBox1.Text + "\0");

        PS3.SetMemory(nameOfs, nameVal);
    }

    private void colorComboBox2_SelectedIndexChanged(object sender, EventArgs e)
    {
        numericUpDown19.Visible = false;
        if (colorComboBox2.SelectedIndex == 3)
            numericUpDown19.Visible = true;
    }

    private void numericUpDown19_ValueChanged(object sender, EventArgs e)
    {
        verticalN = "";
        for (int i = 0; i < numericUpDown19.Value; i++)
        {
            verticalN += "\n";
        }
    }


    #endregion
    #region Special Changers
    int slideInt = 0;

    string rainbowChanger(string txt)
    {
        string val = txt;
        Random rVal = new Random();
        for (int i = 0; i < val.Length; i++)
        {
            txt = txt.Insert(i * 3, "^" + rVal.Next(0, 7));
        }
        return txt + "\0";
    }

    string slideChanger(string txt)
    {
        string realVal = txt;
        if (slideInt * 1 >= txt.Length)
            slideInt = 0;
        int val = slideInt * 1;
        txt = txt.Insert(val, "^" + new Random().Next(0, 7));
        if (val <= txt.Length && realVal.Length != 0)
            txt = txt.Insert(val + 3, "^7");
        ++slideInt;
        return txt + "\0";
    }

    private void numericUpDown15_ValueChanged(object sender, EventArgs e)
    {
        nameChanger.Interval = (int)numericUpDown15.Value;
    }
    #endregion
    #region Level / Prestige
    uint prgOfs = 0x01FF9A9C;
    uint lvlOfs = 0x01FF9A94;
    int lvlNum = 0;
    int prgNum = 0;
    int[] levels = new int[] { 0, 500, 1700, 3600, 6200, 9500, 13500, 18200, 23600, 29700, 36500, 44300, 53100, 62900, 73700, 85500, 96300, 112100, 126900, 142700, 159500, 177300, 196100, 215900, 236700, 258500, 281300, 305100, 329900, 355700, 382700, 410900, 440300, 470900, 502700, 535700, 569900, 605300, 641900, 679700, 718700, 758900, 800300, 842900, 886700, 931700, 977900, 1025300, 1073900, 1123700, 1175000, 1227500, 1282100, 1337900, 1395200, 1454000, 1514300, 1576100, 1639400, 1704200, 1770500, 1838300, 1907600, 1978400, 2050700, 2124500, 2199800, 2276600, 2354900, 2434700 };
    private void numericUpDown1_ValueChanged(object sender, EventArgs e)
    {
        PS3.SetMemory(prgOfs, BitConverter.GetBytes((int)numericUpDown1.Value));
    }

    private void numericUpDown2_ValueChanged(object sender, EventArgs e)
    {
        PS3.SetMemory(lvlOfs, BitConverter.GetBytes(levels[(int)numericUpDown2.Value - 1]));
    }
    #endregion
    #region card changers

    #endregion
    #region In-game Name
    private void button17_Click(object sender, EventArgs e)
    {
        RPC.setName(colorComboBox4.SelectedIndex, "^7" + textBox3.Text);
    }

    private void button59_Click(object sender, EventArgs e)
    {
        colorComboBox4.Items.Clear();
        for (int i = 0; i < 18; i++)
            colorComboBox4.Items.Add(RPC.getName(i));
    }
    #endregion

    #endregion
    #region Game Editors
    #region comboxes
    string[] onlineOrOffline = new string[] { "onlinegame 1;onlinegameandhost 1;xblive_privatematch 0;xblive_rankedmatch 1;wait 10;map_restart", "onlinegame 0;onlinegameandhost 0;xblive_privatematch 1;xblive_rankedmatch 0;wait 10;map_restart" };
    string[] changeMap = new string[] { "afghan", "derail", "estate", "favela", "highrise", "invasion", "checkpoint", "quarry", "rundown", "rust", "boneyard", "nightshift", "subbase", "terminal", "underpass", "brecourt", "complex", "crash", "overgrown", "compact", "storm", "abandon", "fuel2", "strike", "trailerpark", "vacant" };
    string[] changeGametype = new string[] { "ffa", "dm", "sd", "sab", "dom", "koth", "ctf", "dd", "gtnw", "oneflag", "vip", "arena", "dm;ui_gametype ^1Big ^2XP ^1Lobby;scr_dm_timelimit 0;scr_dm_numlives 0;scr_dm_scorelimit 0;set scr_dm_score_kill 2516000;onlinegame 1;onlinegameandhost 1;xblive_privatematch 0;xblive_rankedmatch 1", "sd;ui_gametype ^1SPAWN TRAP!;party_gametype sd;scr_sd_timelimit 0;scr_sd_numlives 0;scr_sd_scorelimit 0", "ffa;set scr_player_maxhealth 100000000", "sd;scr_sd_timelimit 0;scr_sd_scorelimit 0;scr_sd_winlimit 0;scr_game_forceuav 1;scr_player_maxhealth 10", "dm;scr_dm_timelimit 0;scr_dm_scorelimit 1000;scr_game_forceuav 1;scr_player_maxhealth 10" };
    private void colorComboBox5_SelectedIndexChanged(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_RPC("map mp_" + changeMap[colorComboBox5.SelectedIndex]);
    }

    private void colorComboBox6_SelectedIndexChanged(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_RPC("g_gametype " + changeGametype[colorComboBox6.SelectedIndex] + ";wait 10;map_restart");
    }

    private void colorComboBox23_SelectedIndexChanged(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_RPC(onlineOrOffline[colorComboBox23.SelectedIndex]);
    }
    #endregion
    #region players Timer
    private void playersRGB_Tick(object sender, EventArgs e)
    {
        if (colorRadioButton69.Checked)
            dvarColor("r_filmTweakEnable 1;r_filmUseTweaks 1;r_filmTweakBrightness 0.1", RGB2CFG(Color.FromArgb(visionR.Next(0, 256), visionR.Next(0, 256), visionR.Next(0, 256))), "r_filmtweakLighttint", "r_filmtweakMediumtint", "");
        else if (colorRadioButton70.Checked)
            dvarColor("r_filmTweakEnable 1;r_filmUseTweaks 1;r_filmTweakBrightness 0.1", RGB2CFG(rainbowRGB(visionInt, 5)), "r_filmtweakLighttint", "r_filmtweakMediumtint", "");
        if (colorRadioButton66.Checked)
            dvarColor("", RGB2CFG(Color.FromArgb(scoreR.Next(0, 256), scoreR.Next(0, 256), scoreR.Next(0, 256))), "g_ScoresColor_Allies", "g_ScoresColor_Axis", "g_ScoresColor_Free");
        else if (colorRadioButton67.Checked)
            dvarColor("", RGB2CFG(rainbowRGB(scoreInt, 5)), "g_ScoresColor_Allies", "g_ScoresColor_Axis", "g_ScoresColor_Free");

        if (!colorRadioButton55.Checked)
        {
            for (int i = 0; i < 18; i++)
            {
                string name = PS3.Extension.ReadString(0x014E5490 + (uint)i * 0x3700);
                if (!name.StartsWith("^9"))
                {
                    name = name.Insert(0, "^9");
                    PS3.Extension.WriteString(0x014E5490 + (uint)i * 0x3700, name);
                }
            }
        }
        if (colorRadioButton54.Checked || colorRadioButton57.Checked)
            dvarColor("", RGB2CFG(Color.FromArgb(scoreR.Next(0, 256), scoreR.Next(0, 256), scoreR.Next(0, 256))), "g_TeamColor_free", "g_TeamColor_axis", "g_TeamColor_allies");
        else if (colorRadioButton53.Checked || colorRadioButton58.Checked)
            dvarColor("", RGB2CFG(rainbowRGB(scoreInt, 5)), "g_TeamColor_free", "g_TeamColor_axis", "g_TeamColor_allies");

        if (colorRadioButton63.Checked)
            dvarColor("", RGB2CFG(Color.FromArgb(scoreR.Next(0, 256), scoreR.Next(0, 256), scoreR.Next(0, 256))), "g_teamColor_MyTeam", "g_teamColor_EnemyTeam", "");
        else if (colorRadioButton64.Checked)
            dvarColor("", RGB2CFG(rainbowRGB(scoreInt, 5)), "g_teamColor_MyTeam", "g_teamColor_EnemyTeam", "");


        if (colorRadioButton60.Checked)
            dvarColor("", RGB2CFG(Color.FromArgb(scoreR.Next(0, 256), scoreR.Next(0, 256), scoreR.Next(0, 256))), "lowAmmoWarningColor1", "lowAmmoWarningColor2", "");
        else if (colorRadioButton61.Checked)
            dvarColor("", RGB2CFG(rainbowRGB(scoreInt, 5)), "lowAmmoWarningColor1", "lowAmmoWarningColor2", "");


    }
    #endregion
    #region players mods

    private void dvarColor(string presetDvars, string color, string dvar1, string dvar2, string dvar3)
    {
        string setColor1 = "";
        string setColor2 = "";
        string setColor3 = "";
        if (dvar1 != "")
            setColor1 = dvar1 + " " + color + ";";
        if (dvar2 != "")
            setColor2 = dvar2 + " " + color + ";";
        if (dvar3 != "")
            setColor3 = dvar3 + " " + color;

        RPC.cBuff_AddText_RPC(presetDvars + ";" + setColor1 + setColor2 + setColor3);
    }
    ColorDialog visionRGB = new ColorDialog();
    int[] visionInt = new int[] { 255, 0, 0 };
    Random visionR = new Random();
    private void button66_Click(object sender, EventArgs e)
    {
        visionRGB.FullOpen = true;
        if (visionRGB.ShowDialog() == DialogResult.OK)
        {
            dvarColor("r_filmTweakEnable 1;r_filmUseTweaks 1;r_filmTweakBrightness 0.1", RGB2CFG(visionRGB.Color), "r_filmtweakLighttint", "r_filmtweakMediumtint", "");
        }
    }
    private void colorRadioButton68_CheckedChanged(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_RPC("reset r_filmTweakEnable;reset r_filmUseTweaks;reset r_filmTweakBrightness");
    }

    ColorDialog scoreRGB = new ColorDialog();
    int[] scoreInt = new int[] { 255, 0, 0 };
    Random scoreR = new Random();
    private void button65_Click(object sender, EventArgs e)
    {
        scoreRGB.FullOpen = true;
        if (scoreRGB.ShowDialog() == DialogResult.OK)
        {
            dvarColor("", RGB2CFG(scoreRGB.Color), "g_ScoresColor_Allies", "g_ScoresColor_Axis", "g_ScoresColor_Free");
        }
    }

    private void colorRadioButton65_CheckedChanged(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_RPC("reset g_ScoresColor_Allies;reset g_ScoresColor_Axis;reset g_ScoresColor_Free");
    }
    ColorDialog nameRGB = new ColorDialog();
    int[] nameInt = new int[] { 255, 0, 0 };
    Random nameR = new Random();
    private void button60_Click(object sender, EventArgs e)
    {
        nameRGB.FullOpen = true;
        if (nameRGB.ShowDialog() == DialogResult.OK)
        {
            for (int i = 0; i < 18; i++)
            {
                string name = PS3.Extension.ReadString(0x014E5490 + (uint)i * 0x3700);
                if (!name.StartsWith("^9"))
                {
                    name = name.Insert(0, "^9");
                    PS3.Extension.WriteString(0x014E5490 + (uint)i * 0x3700, name);
                }
            }
            dvarColor("", RGB2CFG(nameRGB.Color), "g_TeamColor_free", "g_TeamColor_axis", "g_TeamColor_allies");
        }
    }

    private void colorRadioButton55_CheckedChanged(object sender, EventArgs e)
    {
        for (int i = 0; i < 18; i++)
        {
            string name = PS3.Extension.ReadString(0x014E5490 + (uint)i * 0x3700);
            if (name.StartsWith("^9"))
            {
                name = name.Remove(0, 2);
                PS3.Extension.WriteString(0x014E5490 + (uint)i * 0x3700, name);
            }
        }
        RPC.cBuff_AddText_RPC("reset g_TeamColor_free;reset g_TeamColor_axis;reset g_TeamColor_allies");
    }


    ColorDialog nameTRGB = new ColorDialog();
    int[] nameTInt = new int[] { 255, 0, 0 };
    Random nameTR = new Random();
    private void button64_Click(object sender, EventArgs e)
    {
        nameTRGB.FullOpen = true;
        if (nameTRGB.ShowDialog() == DialogResult.OK)
        {
            dvarColor("", RGB2CFG(nameTRGB.Color), "g_teamColor_MyTeam", "g_teamColor_EnemyTeam", "");
        }
    }

    private void colorRadioButton62_CheckedChanged(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_RPC("reset g_teamColor_MyTeam;reset g_teamColor_EnemyTeam");
    }

    ColorDialog kfRGB = new ColorDialog();
    int[] kfInt = new int[] { 255, 0, 0 };
    Random kfR = new Random();
    private void button62_Click(object sender, EventArgs e)
    {
        kfRGB.FullOpen = true;
        if (kfRGB.ShowDialog() == DialogResult.OK)
        {
            dvarColor("", RGB2CFG(kfRGB.Color), "g_TeamColor_free", "g_TeamColor_axis", "g_TeamColor_allies");
        }
    }

    private void colorRadioButton56_CheckedChanged(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_RPC("reset g_TeamColor_free;reset g_TeamColor_axis;reset g_TeamColor_allies");
    }

    ColorDialog laRGB = new ColorDialog();
    int[] laInt = new int[] { 255, 0, 0 };
    Random laR = new Random();
    private void button63_Click(object sender, EventArgs e)
    {
        laRGB.FullOpen = true;
        if (laRGB.ShowDialog() == DialogResult.OK)
        {
            dvarColor("", RGB2CFG(laRGB.Color), "lowAmmoWarningColor1", "lowAmmoWarningColor2", "");
        }
    }
    private void colorRadioButton59_CheckedChanged(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_RPC("reset lowAmmoWarningColor1;reset lowAmmoWarningColor2");
    }

    #endregion
    #region players editables
    private void numericUpDown3_ValueChanged(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_RPC("jump_height " + numericUpDown3.Value);
        RPC.cBuff_AddText_RPC("bg_fallDamageMinHeight 99999;bg_fallDamageMaxHeight 99999");
    }

    private void button18_Click(object sender, EventArgs e)
    {
        numericUpDown3.Value = 39;
        RPC.cBuff_AddText_RPC("reset bg_fallDamageMinHeight;reset bg_fallDamageMaxHeight");
    }
    private void numericUpDown4_ValueChanged(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_RPC("g_speed " + numericUpDown4.Value);
    }

    private void button19_Click(object sender, EventArgs e)
    {
        numericUpDown4.Value = 190;
    }

    private void numericUpDown5_ValueChanged(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_RPC("g_gravity " + numericUpDown5.Value);
    }

    private void button20_Click(object sender, EventArgs e)
    {
        numericUpDown5.Value = 800;
    }

    private void numericUpDown6_ValueChanged(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_RPC("timescale " + numericUpDown6.Value);
    }

    private void button21_Click(object sender, EventArgs e)
    {
        numericUpDown6.Value = 1;
    }

    private void numericUpDown7_ValueChanged(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_RPC("g_knockback " + numericUpDown7.Value);
    }

    private void button22_Click(object sender, EventArgs e)
    {
        numericUpDown7.Value = 1000;
    }

    private void numericUpDown8_ValueChanged(object sender, EventArgs e)
    {
        RPC.SV_GameSendServerCommand(-1, "v cg_fovScale " + numericUpDown8.Value);
    }

    private void button23_Click(object sender, EventArgs e)
    {
        numericUpDown8.Value = 1;
    }

    private void numericUpDown9_ValueChanged(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_RPC("r_diffuseColorScale " + numericUpDown9.Value);
    }

    private void button24_Click(object sender, EventArgs e)
    {
        numericUpDown9.Value = 1;
    }

    private void numericUpDown10_ValueChanged(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_RPC("player_sprintSpeedScale " + numericUpDown10.Value);
    }

    private void button25_Click(object sender, EventArgs e)
    {
        numericUpDown10.Value = (decimal)1.5;
    }
    #endregion
    #endregion
    #region Game Mods
    #region non-host

    private void colorCheckBoxList2_ItemCheck(object sender, ItemCheckEventArgs e)
    {
        ColorCheckBoxList box = (ColorCheckBoxList)sender;
         File.AppendAllText(listPath, box.SelectedItem + ":" + e.NewValue + ":");

        if (e.Index == 0 && e.NewValue == CheckState.Checked)
            PS3.Extension.WriteInt32(0x496F4, 0x2F800001);
        else if (e.Index == 0 && e.NewValue == CheckState.Unchecked)
            PS3.Extension.WriteInt32(0x496F4, 0x2F800000);
        if (e.Index == 1 && e.NewValue == CheckState.Checked)
            PS3.Extension.WriteInt32(0x9342C, 0x60000000);
        else if (e.Index == 1 && e.NewValue == CheckState.Unchecked)
            PS3.Extension.WriteInt32(0x9342C, 0x4BFA10F5);
        if (e.Index == 2 && e.NewValue == CheckState.Checked)
            RPC.cBuff_AddText_RPC("cg_drawShellshock 0");
        else if (e.Index == 2 && e.NewValue == CheckState.Unchecked)
            RPC.cBuff_AddText_RPC("cg_drawShellshock 1");
        if (e.Index == 3 && e.NewValue == CheckState.Checked)
            RPC.cBuff_AddText_Reg("g_compassShowEnemies 1;bind apad_up g_compassShowEnemies 1");
        else if (e.Index == 3 && e.NewValue == CheckState.Unchecked)
            RPC.cBuff_AddText_RPC("unbind apad_up;g_compassShowEnemies 0");
        if (e.Index == 4 && e.NewValue == CheckState.Checked)
        {
            antiCounterUAV.Stop();
            antiCounterUAV.Start();
        }
        else if (e.Index == 4 && e.NewValue == CheckState.Unchecked)
            antiCounterUAV.Stop();
        if (e.Index == 5 && e.NewValue == CheckState.Checked)
            RPC.cBuff_AddText_RPC("cg_drawfps 1");
        else if (e.Index == 5 && e.NewValue == CheckState.Unchecked)
            RPC.cBuff_AddText_RPC("cg_drawfps 0");
        if (e.Index == 6 && e.NewValue == CheckState.Checked)
            RPC.cBuff_AddText_RPC("perk_weapSpreadMultiplier 0.0001");
        else if (e.Index == 6 && e.NewValue == CheckState.Unchecked)
            RPC.cBuff_AddText_RPC("perk_weapSpreadMultiplier 0.62");
        if (e.Index == 7 && e.NewValue == CheckState.Checked)
            RPC.cBuff_AddText_RPC("perk_fastSnipeScale 9;perk_quickDrawSpeedScale 6.5");
        else if (e.Index == 7 && e.NewValue == CheckState.Unchecked)
            RPC.cBuff_AddText_RPC("perk_fastSnipeScale 4;perk_quickDrawSpeedScale 1.5");
        if (e.Index == 8 && e.NewValue == CheckState.Checked)
            RPC.cBuff_AddText_RPC("laserForceOn 1;laserRangePlayer 100000;laserRadius 0.1;laserFlarePct 1");
        else if (e.Index == 8 && e.NewValue == CheckState.Unchecked)
            RPC.cBuff_AddText_RPC("laserForceOn 0;reset laserRangePlayer;reset laserRadius;reset laserFlarePct");
        if (e.Index == 9 && e.NewValue == CheckState.Checked)
            RPC.cBuff_AddText_RPC("r_znear 55");
        else if (e.Index == 9 && e.NewValue == CheckState.Unchecked)
            RPC.cBuff_AddText_RPC("r_znear 5");
        if (e.Index == 10 && e.NewValue == CheckState.Checked)
        {
            invisTog = 0;
            invis = true;
        }
        else if (e.Index == 10 && e.NewValue == CheckState.Unchecked)
        {
            invis = false;
            invisReset();
        }
        if (e.Index == 11 && e.NewValue == CheckState.Checked)
            RPC.cBuff_AddText_RPC("cg_thirdperson 1");
        else if (e.Index == 11 && e.NewValue == CheckState.Unchecked)
            RPC.cBuff_AddText_RPC("cg_thirdperson 0");
        if (e.Index == 12 && e.NewValue == CheckState.Checked)
        {
            RPC.cBuff_AddText_RPC("cg_thirdperson 1");
            PS3.Extension.WriteInt32(0x0006cae4, 0x2F000001);
        }
        else if (e.Index == 12 && e.NewValue == CheckState.Unchecked)
        {
            RPC.cBuff_AddText_RPC("cg_thirdperson 0");
            PS3.Extension.WriteInt32(0x0006cae4, 0x2F000000);
        }
        if (e.Index == 13 && e.NewValue == CheckState.Checked)
            PS3.Extension.WriteInt32(0x2383A8, 0x60000000);
        else if (e.Index == 13 && e.NewValue == CheckState.Unchecked)
            PS3.Extension.WriteInt32(0x2383A8, 0x4bffe3d1);
        if (e.Index == 14 && e.NewValue == CheckState.Checked)
            RPC.cBuff_AddText_RPC("cg_fovscale 1.39");
        else if (e.Index == 14 && e.NewValue == CheckState.Unchecked)
            RPC.cBuff_AddText_RPC("cg_fovscale 1");
        if (e.Index == 15 && e.NewValue == CheckState.Checked)
            RPC.cBuff_AddText_RPC("fx_enable 0");
        else if (e.Index == 15 && e.NewValue == CheckState.Unchecked)
            RPC.cBuff_AddText_RPC("fx_enable 1");
        if (e.Index == 16 && e.NewValue == CheckState.Checked)
            RPC.cBuff_AddText_RPC("xblive_privatematch 1;xblive_rankedmatch 0");
        else if (e.Index == 16 && e.NewValue == CheckState.Unchecked)
            RPC.cBuff_AddText_RPC("xblive_privatematch 0;xblive_rankedmatch 1");
        if (e.Index == 17 && e.NewValue == CheckState.Checked)
            l1Auto = true;
        else if (e.Index == 17 && e.NewValue == CheckState.Unchecked)
            l1Auto = false;
        if (e.Index == 18 && e.NewValue == CheckState.Checked)
        {
            RPC.ToggleON();
            health = true;
        }
        else if (e.Index == 18 && e.NewValue == CheckState.Unchecked)
        {
            health = false;
            RPC.ToggleOFF();
        }
        if (e.Index == 19 && e.NewValue == CheckState.Checked)
            PS3.Extension.WriteInt32(0x3FA40, 0x2F800000);
        else if (e.Index == 19 && e.NewValue == CheckState.Unchecked)
            PS3.Extension.WriteInt32(0x3FA40, 0x2F800001);
        if (e.Index == 20 && e.NewValue == CheckState.Checked)
            PS3.Extension.WriteInt32(0x6CAE4, 0x2F800001);
        else if (e.Index == 20 && e.NewValue == CheckState.Unchecked)
            PS3.Extension.WriteInt32(0x6CAE4, 0x2F800000);
        if (e.Index == 21 && e.NewValue == CheckState.Checked)
        {
            RPC.cBuff_AddText_RPC("cg_thirdperson 1");
            PS3.Extension.WriteInt32(0x24DFC, 0x2F800000);
        }
        else if (e.Index == 21 && e.NewValue == CheckState.Unchecked)
        {
            RPC.cBuff_AddText_RPC("cg_thirdperson 0");
            PS3.Extension.WriteInt32(0x24DFC, 0x2F800001);
        }
        if (e.Index == 22 && e.NewValue == CheckState.Checked)
            RPC.cBuff_AddText_RPC("cg_chatHeight 0");
        else if (e.Index == 22 && e.NewValue == CheckState.Unchecked)
            RPC.cBuff_AddText_RPC("reset cg_chatHeight");
        if (e.Index == 23 && e.NewValue == CheckState.Checked)
        {
            RPC.cBuff_AddText_RPC("cg_thirdperson 1");
            PS3.Extension.WriteInt32(0x62EA88, 0x30000000);
        }
        else if (e.Index == 23 && e.NewValue == CheckState.Unchecked)
        {
            RPC.cBuff_AddText_RPC("cg_thirdperson 0");
            PS3.Extension.WriteInt32(0x62EA88, 0x3F800000);
        }
        if (e.Index == 24 && e.NewValue == CheckState.Checked)
        {
            RPC.cBuff_AddText_RPC("cg_thirdperson 1");
            PS3.SetMemory(0x62EA88, new byte[] { 0xBF, 0x80, 0x00, 0x00 });
        }
        else if (e.Index == 24 && e.NewValue == CheckState.Unchecked)
        {
            RPC.cBuff_AddText_RPC("cg_thirdperson 0");
            PS3.Extension.WriteInt32(0x62EA88, 0x3F800000);
        }
        if (e.Index == 25 && e.NewValue == CheckState.Checked)
            PS3.Extension.WriteInt32(0x62A83C, 0x30000000);
        else if (e.Index == 25 && e.NewValue == CheckState.Unchecked)
            PS3.Extension.WriteInt32(0x62A83C, 0x3F800000);
        if (e.Index == 26 && e.NewValue == CheckState.Checked)
            PS3.Extension.WriteInt32(0x82668, 0x2F800001);
        else if (e.Index == 26 && e.NewValue == CheckState.Unchecked)
            PS3.Extension.WriteInt32(0x82668, 0x2F800000);
        if (e.Index == 27 && e.NewValue == CheckState.Checked)
            PS3.Extension.WriteInt32(0x8D06C, 0x39400001);
        else if (e.Index == 27 && e.NewValue == CheckState.Unchecked)
            PS3.Extension.WriteInt32(0x8D06C, 0x39400000);
        if (e.Index == 28 && e.NewValue == CheckState.Checked)
            PS3.Extension.WriteInt32(0x162A1C, 0x2F9A0001);
        else if (e.Index == 28 && e.NewValue == CheckState.Unchecked)
            PS3.Extension.WriteInt32(0x162A1C, 0x2F9A0000);
        if (e.Index == 29 && e.NewValue == CheckState.Checked)
        {
            RPC.cBuff_AddText_Reg("cg_thirdperson 1;wait 2;weapnext;");
            PS3.Extension.WriteInt32(0x17B90, 0x2F800001);
        }
        else if (e.Index == 29 && e.NewValue == CheckState.Unchecked)
        {
            PS3.Extension.WriteInt32(0x17B90, 0x2F800000);
            RPC.cBuff_AddText_Reg("cg_thirdperson 0;wait 2;weapnext;");
        }
        if (e.Index == 30 && e.NewValue == CheckState.Checked)
        {
            RPC.cBuff_AddText_RPC("cg_thirdperson 1");
            PS3.Extension.WriteInt32(0x84B18, 0x2F800001);
        }
        else if (e.Index == 30 && e.NewValue == CheckState.Unchecked)
        {
            PS3.Extension.WriteInt32(0x84B18, 0x2F800000);
            RPC.cBuff_AddText_RPC("cg_thirdperson 0");
        }
        if (e.Index == 31 && e.NewValue == CheckState.Checked)
        {
            RPC.cBuff_AddText_RPC("cg_thirdperson 1");
            PS3.Extension.WriteInt32(0x84B64, 0x2F800001);
        }
        else if (e.Index == 31 && e.NewValue == CheckState.Unchecked)
        {
            RPC.cBuff_AddText_RPC("cg_thirdperson 0");
            PS3.Extension.WriteInt32(0x84B64, 0x2F800000);
        }
        if (e.Index == 32 && e.NewValue == CheckState.Checked)
        {
            RPC.cBuff_AddText_RPC("cg_thirdperson 1");
            PS3.Extension.WriteInt32(0x6CAE4, 0x2F800001);
        }
        else if (e.Index == 32 && e.NewValue == CheckState.Unchecked)
        {
            RPC.cBuff_AddText_RPC("cg_thirdperson 0");
            PS3.Extension.WriteInt32(0x6CAE4, 0x2F800000);
        }
        if (e.Index == 33 && e.NewValue == CheckState.Checked)
            PS3.Extension.WriteInt32(0x87B88, 0x2F800001);
        else if (e.Index == 33 && e.NewValue == CheckState.Unchecked)
            PS3.Extension.WriteInt32(0x87B88, 0x2F800000);
        if (e.Index == 34 && e.NewValue == CheckState.Checked)
        {
            RPC.cBuff_AddText_RPC("cg_thirdperson 1");
            PS3.Extension.WriteInt32(0x13F84, 0x2F800001);
        }
        else if (e.Index == 34 && e.NewValue == CheckState.Unchecked)
        {
            RPC.cBuff_AddText_RPC("cg_thirdperson 0");
            PS3.Extension.WriteInt32(0x13F84, 0x2F800000);
        }
        if (e.Index == 35 && e.NewValue == CheckState.Checked)
            PS3.Extension.WriteInt32(0x52D28, 0x2F800001);
        else if (e.Index == 35 && e.NewValue == CheckState.Unchecked)
            PS3.Extension.WriteInt32(0x52D28, 0x2F800000);
        if (e.Index == 36 && e.NewValue == CheckState.Checked)
            PS3.Extension.WriteInt32(0x53A40, 0x2F800001);
        else if (e.Index == 36 && e.NewValue == CheckState.Unchecked)
            PS3.Extension.WriteInt32(0x53A40, 0x2F800000);
        if (e.Index == 37 && e.NewValue == CheckState.Checked)
            PS3.Extension.WriteInt32(0x328F8, 0x2F800001);
        else if (e.Index == 37 && e.NewValue == CheckState.Unchecked)
            PS3.Extension.WriteInt32(0x328F8, 0x2F800000);
        if (e.Index == 38 && e.NewValue == CheckState.Checked)
            PS3.SetMemory(0x63D380, new byte[] { 0x30, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00 });
        else if (e.Index == 38 && e.NewValue == CheckState.Unchecked)
            PS3.SetMemory(0x63D380, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 });
        if (e.Index == 39 && e.NewValue == CheckState.Checked)
            RPC.cBuff_AddText_RPC("selectStringTableEntryInDvar M M player_view_pitch_up;selectStringTableEntryInDvar M M player_view_pitch_down");
        else if (e.Index == 39 && e.NewValue == CheckState.Unchecked)
            RPC.cBuff_AddText_RPC("reset player_view_pitch_up;reset player_view_pitch_down");

        //Task.Delay(100).Wait();

        
    }

    int cg_s()
    {
        return PS3.Extension.ReadInt32(0x915254);
    }
    int GetHealth()
    {
        return PS3.Extension.ReadByte((uint)cg_s() + 0x153);
    }
    private void button31_Click(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_Reg("cmd mr " + PS3.Extension.ReadInt32(0x1BE5BE8) + " -1 endround");
    }
    private void button32_Click(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_Reg("cmd mr " + PS3.Extension.ReadInt32(0x1BE5BE8) + " 3 autoassign");
    }
    private void antiCounterUAV_Tick(object sender, EventArgs e)
    {
        uint addyPos;
        if (RPC.onlineMatch()) addyPos = 0x366FE1AF; else addyPos = 0x36AEE1AF;
        if (PS3.Extension.ReadByte(addyPos) == 0x01) PS3.SetMemory(0x7EC1B, new byte[] { 0x01 }); else PS3.SetMemory(0x7EC1B, new byte[] { 0x00 });
    }
    bool invis = false;
    bool health = false;
    bool l1Auto = false;
    int invisTog = 0;
    string[] healthC = new string[] { "^2", "^3", "^1" };
    private void autoFire_Tick(object sender, EventArgs e)
    {
        if (l1Auto)
            if (RPC.NonHostButtonPressed(RPC.NonHostButtons.R1))
                PS3.Extension.WriteBytes(0x0095b081, new byte[] { 0x01 });//singlefire

        if (!useAuto && checkAuto)
            PS3.Extension.WriteBytes(0x0095b081, new byte[] { 0x01 });//singlefire

    }
    #endregion

    #region Game Mods neutral
    private void colorComboBox10_SelectedIndexChanged(object sender, EventArgs e)
    {
        string[] hintTxt = { "Select A Bind", "Knife Required", "SOH Perk Required", "Use Any Gun" };
        if (colorComboBox10.SelectedIndex == 0) RPC.cBuff_AddText_Reg("bind button_rshldr +attack");
        if (colorComboBox10.SelectedIndex == 1) RPC.cBuff_AddText_Reg("set doR1TS \"+attack;-attack;wait 2;+frag;wait 30;weapnext;wait 20;-frag\";bind button_rshldr vstr doR1TS");
        if (colorComboBox10.SelectedIndex == 2) RPC.cBuff_AddText_Reg("set doR1TS \"+attack;wait 2;+usereload;-attack;wait 30;-usereload;wait 10;weapnext;wait 2;weapnext;wait 2;+attack;-attack\";bind button_rshldr vstr doR1TS");
        if (colorComboBox10.SelectedIndex == 3) RPC.cBuff_AddText_Reg("set doR1TS \"weapnext;wait 30;weapnext;wait 2;+attack;-attack\";bind button_rshldr vstr doR1TS");
    }

    private void colorComboBox11_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (colorComboBox11.SelectedIndex == 0) RPC.cBuff_AddText_Reg("bind dpad_down +actionslot 2");
        if (colorComboBox11.SelectedIndex == 1) RPC.cBuff_AddText_Reg("set doDwnTS \"+sprint;+forward;wait 10;weapnext;wait 130;weapnext;wait 10;weapnext;-forward;-sprint\";bind dpad_down vstr doDwnTS");
        if (colorComboBox11.SelectedIndex == 2) RPC.cBuff_AddText_Reg("set doDwnTS \"+sprint;+forward;weapnext;wait 50;weapnext;wait 10;weapnext;-forward;-sprint\";bind dpad_down vstr doDwnTS");
        if (colorComboBox11.SelectedIndex == 3) RPC.cBuff_AddText_Reg("set doDwnTS \"+sprint;+forward;wait 20;weapnext;wait 10;weapnext;-forward;-sprint\";bind dpad_down vstr doDwnTS");
        if (colorComboBox11.SelectedIndex == 4) RPC.cBuff_AddText_Reg("set doDwnTS \"+speed_throw;wait 20;+usereload;+sprint;wait 10;+forward;-usereload;-speed_throw;wait 200;-forward;-sprint\";bind dpad_down vstr doDwnTS");
        if (colorComboBox11.SelectedIndex == 5) RPC.cBuff_AddText_Reg("set doDwnTS \"bind button_a cg_blood 1; weapnext; wait 149; +melee; +frag; -melee; -frag; wait 50; bind button_a +gostand\";bind dpad_down vstr doDwnTS");
    }

    private void colorComboBox12_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (colorComboBox12.SelectedIndex == 0) RPC.cBuff_AddText_Reg("reset cl_yawspeed;bind button_ltrig +smoke");
        if (colorComboBox12.SelectedIndex == 1) RPC.cBuff_AddText_Reg("set TogSpin \"cl_yawspeed 1000;+right; wait 22;-right\";bind button_ltrig vstr TogSpin");
        if (colorComboBox12.SelectedIndex == 2) RPC.cBuff_AddText_Reg("set TogSpin \"cl_yawspeed 1000;+right; wait 44;-right\";bind button_ltrig vstr TogSpin");
        if (colorComboBox12.SelectedIndex == 3) RPC.cBuff_AddText_Reg("set TogSpin \"cl_yawspeed 1000;+right; wait 66;-right\";bind button_ltrig vstr TogSpin");
        if (colorComboBox12.SelectedIndex == 4) RPC.cBuff_AddText_Reg("set TogSpin \"cl_yawspeed 1000;+right; wait 88;-right\";bind button_ltrig vstr TogSpin");
        if (colorComboBox12.SelectedIndex == 5) RPC.cBuff_AddText_Reg("set TogSpin \"cl_yawspeed 1000;+right; wait 110;-right\";bind button_ltrig vstr TogSpin");
    }
    private void button77_Click(object sender, EventArgs e)
    {
        MessageBox.Show("Silent Shot: Throwing Knife\nFayde Shot: Sleight Of Hand Perk\nFake Switch: Use Any Gun\n\nBarrel Roll: Spas-12\nTac Knife Flip: Sniper + Tactical Knife\nCat Walk: Tactical Knife\nWrist Twist: Secondary Akimbo\nMala Glitch: One man army + tactical insertion. Use one man army and hover over a class then cancel. Press Down and spam the X button.", "Bind Hints", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void button26_Click(object sender, EventArgs e)
    {
        if (colorRadioButton1.Checked)
        {
            button26.Text = "Please wait...";
            button26.Update();
            for (int i = 0; i < 18; i++)
            {
                RPC.SetTypewriter(i, textBox4.Text + "\0", 9, 2, 50F, 50F, 40, 100, 0x2710, 0x5dc, Convert.ToInt16(twRGBc.Color.R), Convert.ToInt16(twRGBc.Color.G), Convert.ToInt16(twRGBc.Color.B), 0xff, Convert.ToInt16(twRGBg.Color.R), Convert.ToInt16(twRGBg.Color.G), Convert.ToInt16(twRGBg.Color.B), 200);
            }
            button26.Text = "Set";
        }
        else if (colorRadioButton16.Checked)
            RPC.cBuff_AddText_Fix("say ^7" + textBox4.Text);
        else if (colorRadioButton19.Checked)
            RPC.cBuff_AddText_Reg(textBox4.Text);
    }
    ColorDialog twRGBc = new ColorDialog();
    ColorDialog twRGBg = new ColorDialog();

    private void button30_Click(object sender, EventArgs e)
    {
        twRGBc.FullOpen = true;
        twRGBc.ShowDialog();
    }

    private void button29_Click(object sender, EventArgs e)
    {
        twRGBg.FullOpen = true;
        twRGBg.ShowDialog();
    }

    private void colorRadioButton1_CheckedChanged(object sender, EventArgs e)
    {
        if (colorRadioButton1.Checked)
        {
            button29.Enabled = true;
            button30.Enabled = true;
            colorRadioButton26.Checked = true;
            colorRadioButton26.Enabled = false;
            colorRadioButton27.Enabled = false;
            colorRadioButton28.Enabled = false;
        }
        else
        {
            button29.Enabled = false;
            button30.Enabled = false;
            colorRadioButton26.Enabled = true;
            colorRadioButton27.Enabled = true;
            colorRadioButton28.Enabled = true;
        }
    }

    private void colorRadioButton19_CheckedChanged(object sender, EventArgs e)
    {
        if (colorRadioButton19.Checked)
        {
            colorRadioButton26.Checked = true;
            colorRadioButton26.Enabled = false;
            colorRadioButton27.Enabled = false;
            colorRadioButton28.Enabled = false;
        }
        else if (colorRadioButton16.Checked)
        {
            colorRadioButton26.Enabled = true;
            colorRadioButton27.Enabled = true;
            colorRadioButton28.Enabled = true;
        }
    }
    private void button27_Click(object sender, EventArgs e)
    {
        if (colorRadioButton18.Checked)
            RPC.cBuff_AddText_Reg("xblive_privatematch 0;xblive_statusedmatch 1;disconnect;killserver;disconnect;xstartlobby;set party_connectToOthers 0;xsearchforgames;set party_hostmigration 0;set party_gameStartTimerLength 1;set party_pregameStartTimerLength 1;set party_timer 1;set badhost_endGameIfISuck 0;set party_minplayers 1;set party_autoteams 0");
        else if (colorRadioButton17.Checked)
            RPC.cBuff_AddText_Reg("xblive_privatematch 1;xblive_statusedmatch 0;disconnect;killserver;disconnect;xstartlobby;set party_connectToOthers 0;xsearchforgames;set party_hostmigration 0;set party_gameStartTimerLength 1;set party_pregameStartTimerLength 1;set party_timer 1;set badhost_endGameIfISuck 0;set party_minplayers 1;set party_autoteams 0");
    }

    private void button28_Click(object sender, EventArgs e)
    {
        if (colorRadioButton18.Checked == true)
            RPC.cBuff_AddText_Reg("reset party_autoteams;xblive_privatematch 0;xblive_statusedmatch 1;disconnect;killserver;disconnect;xstartlobby;set party_connectToOthers 0;xsearchforgames;wait 200;xsearchforgames;set party_hostmigration 0;set party_gameStartTimerLength 1;set party_pregameStartTimerLength 1;set party_timer 1;set party_minplayers 2;wait 200;reset party_autoteams;xsearchforgames;wait 200;xsearchforgames;wait 200;xsearchforgames;wait 200;xsearchforgames;wait 200;xsearchforgames;wait 200;xsearchforgames;wait 200;xsearchforgames;wait 200;xsearchforgames;reset party_autoteams");
        else if (colorRadioButton17.Checked == true)
            RPC.cBuff_AddText_Reg("reset party_autoteams;xblive_privatematch 1;xblive_statusedmatch 0;disconnect;killserver;disconnect;xstartlobby;set party_connectToOthers 0;xsearchforgames;wait 200;xsearchforgames;set party_hostmigration 0;set party_gameStartTimerLength 1;set party_pregameStartTimerLength 1;set party_timer 1;set party_minplayers 2;wait 200;reset party_autoteams;xsearchforgames;wait 200;xsearchforgames;wait 200;xsearchforgames;wait 200;xsearchforgames;wait 200;xsearchforgames;wait 200;xsearchforgames;wait 200;xsearchforgames;wait 200;xsearchforgames;reset party_autoteams");
    }
    #endregion
    private void invisReset()
    {
        RPC.cBuff_AddText_Reg("reset player_lean_shift_left;reset player_lean_shift_crouch_left;-leanleft");
    }
    #region Game Timer
    private void gameMods_Tick(object sender, EventArgs e)
    {
        if (useBuild)
        {
            curGridIndex = dataGridView2.CurrentRow.Index;
        }
        if (health)
        {
            int healthInt = GetHealth();
            int hC = 0;
            if (healthInt > 50)
                hC = 0;
            else if (healthInt <= 50 && healthInt > 25)
                hC = 1;
            else if (healthInt <= 25)
                hC = 2;
            RPC.Write(healthC[hC] + healthInt.ToString(), 0.4, -35, 160);
        }
        if (invis)
            if (PS3.Extension.ReadByte(0x8ABBC6) == 0x40 || RPC.NonHostButtonPressed(RPC.NonHostButtons.L3))
            {
                if (invisTog == 0 || invisTog == 1)
                {
                    invisTog = 2;
                    invisReset();
                }
            }
            else
            {
                if (invisTog == 0 || invisTog == 2)
                {
                    invisTog = 1;
                    RPC.cBuff_AddText_Reg("selectStringTableEntryInDvar M M player_lean_shift_left;selectStringTableEntryInDvar M M player_lean_shift_crouch_left;+leanleft");
                }
            }
        if (colorCheckBox6.Checked)
            updateInfo();
        if (colorCheckBox7.Checked)
            spoofIp();

        if (noLeave)
            RPC.SV_GameSendServerCommand(-1, "L ");
        if (noRB)
            RPC.SV_GameSendServerCommand(-1, "v set FoFIconSpawnTimeDelay \"10000\"");
        if (noCFG)
        {
            RPC.SV_GameSendServerCommand(-1, "v cl_noprint 1");
            RPC.SV_GameSendServerCommand(-1, "v con_minicon 0");
        }
        if (noAA)
            RPC.SV_GameSendServerCommand(-1, "v SelectStringTableEntryInDvar M M aim_target_sentient_radius");
        if (advUFO)
        {
            if (zeroArray.SequenceEqual((PS3.Extension.ReadBytes(0x0095ADD8, 8))))
            {
                if (advUFOtog == 0 || advUFOtog == 1)
                {
                    advUFOtog = 2;
                    PS3.SetMemory(0x14e5623 + (uint)hostNum * 0x3700, new byte[] { 0x00 });
                    RPC.cBuff_AddText_RPC("g_gravity 1");
                }
            }
            else
            {
                if (advUFOtog == 0 || advUFOtog == 2)
                {
                    advUFOtog = 1;
                    PS3.SetMemory(0x14e5623 + (uint)hostNum * 0x3700, new byte[] { 0x02 });
                    RPC.cBuff_AddText_RPC("g_gravity 800");
                }
            }
        }
        if (RPC.inGame())
        {
            if (inGameTog == 0 || inGameTog == 1)
            {
                inGameTog = 2;
                hostNum = RPC.getHostNum();
                RPC.SV_GameSendServerCommand(-1, "v loc_warnings 0");
                Task.Delay(500).Wait();
                RPC.cBuff_AddText_RPC("cg_drawgun 1");
                enableAA();
            }

        }
        else
        {
            if (inGameTog == 0 || inGameTog == 2)
            {
                inGameTog = 1;
                entOfs = 0;
                entList.Clear();
            }
        }
    }
    #endregion

    #region Host
    bool advUFO = false;
    private void colorCheckBoxList3_ItemCheck(object sender, ItemCheckEventArgs e)
    {
        if (e.Index == 0 && e.NewValue == CheckState.Checked)
            advUFO = true;
        else if (e.Index == 0 && e.NewValue == CheckState.Unchecked)
        {
            advUFO = false;
            PS3.SetMemory(0x14e5623 + (uint)hostNum * 0x3700, new byte[] { 0x00 });
            RPC.cBuff_AddText_RPC("g_gravity 800");
        }
        if (e.Index == 1 && e.NewValue == CheckState.Checked)
            RPC.cBuff_AddText_RPC("bg_bulletExplDmgFactor 100;bg_bulletExplRadius 10000");
        else if (e.Index == 1 && e.NewValue == CheckState.Unchecked)
            RPC.cBuff_AddText_RPC("reset bg_bulletExplDmgFactor;reset bg_bulletExplRadius");
        if (e.Index == 2 && e.NewValue == CheckState.Checked)
        {
            RPC.cBuff_AddText_RPC("scr_ctf_timelimit 0;scr_ctf_scorelimit 0;scr_ctf_roundlimit 0;scr_dd_timelimit 0;scr_dd_roundlimit 0;scr_dd_scorelimit 0;scr_dm_scorelimit 0;scr_dm_timelimit 0");
            Thread.Sleep(100);
            RPC.cBuff_AddText_RPC("scr_dm_roundlimit 0;scr_dom_timelimit 0;scr_dom_roundlimit 0;scr_dom_scorelimit 0;scr_koth_timelimit 0;scr_koth_roundlimit 0;scr_koth_scorelimit 0;scr_oneflag_timelimit 0");
            Thread.Sleep(100);
            RPC.cBuff_AddText_RPC("scr_oneflag_scorelimit 0;scr_oneflag_roundlimit 0;scr_sab_timelimit 0;scr_sab_scorelimit 0;scr_sab_roundlimit 0;scr_war_timelimit 0;scr_war_scorelimit 0;scr_war_roundlimit 0;scr_sd_timelimit 0;scr_sd_roundlimit 0;scr_sd_scorelimit 0");
        }
        else if (e.Index == 2 && e.NewValue == CheckState.Unchecked)
        {
            RPC.cBuff_AddText_RPC("reset scr_ctf_timelimit;reset scr_ctf_scorelimit;reset scr_ctf_roundlimit 1;scr_dd_timelimit;reset scr_dd_roundlimit 3;reset scr_dd_scorelimit;reset scr_dm_scorelimit;reset scr_dm_timelimit");
            Thread.Sleep(100);
            RPC.cBuff_AddText_RPC("reset scr_dm_roundlimit;reset scr_dom_timelimit;reset scr_dom_roundlimit;reset scr_dom_scorelimit;reset scr_koth_timelimit;reset scr_koth_roundlimit;reset scr_koth_scorelimit;reset scr_oneflag_timelimit");
            Thread.Sleep(100);
            RPC.cBuff_AddText_RPC("reset scr_oneflag_scorelimit;reset scr_oneflag_roundlimit;reset scr_sab_timelimit;reset scr_sab_scorelimit;reset scr_sab_roundlimit;reset scr_war_timelimit;reset scr_war_scorelimit;reset scr_war_roundlimit;reset scr_sd_timelimit;reset scr_sd_roundlimit;reset scr_sd_scorelimit");
        }
        if (e.Index == 3 && e.NewValue == CheckState.Checked)
            RPC.cBuff_AddText_RPC("player_meleeRange 999;player_meleeHeight 1000;player_meleeRange 1000;player_meleeWidth 1000");
        else if (e.Index == 3 && e.NewValue == CheckState.Unchecked)
            RPC.cBuff_AddText_RPC("reset player_meleeRange;reset player_meleeHeight;reset player_meleeRange;reset player_meleeWidth");
        if (e.Index == 4 && e.NewValue == CheckState.Checked)
            RPC.cBuff_AddText_RPC("player_burstFireCooldown 0");
        else if (e.Index == 4 && e.NewValue == CheckState.Unchecked)
            RPC.cBuff_AddText_RPC("reset player_burstFireCooldown");
        if (e.Index == 5 && e.NewValue == CheckState.Checked)
            RPC.cBuff_AddText_RPC("perk_weapReloadMultiplier 0.0001");
        else if (e.Index == 5 && e.NewValue == CheckState.Unchecked)
            RPC.cBuff_AddText_RPC("reset perk_weapReloadMultiplier");
        if (e.Index == 6 && e.NewValue == CheckState.Checked)
            PS3.SetMemory(0x2D95F, new byte[] { 0x01 });
        else if (e.Index == 6 && e.NewValue == CheckState.Unchecked)
            PS3.SetMemory(0x2D95F, new byte[] { 0x00 });
        if (e.Index == 7 && e.NewValue == CheckState.Checked)
            RPC.cBuff_AddText_RPC("glass_damageToWeaken 65535;glass_damageToDestroy 65535;glass_break 1;missileGlassShatterVel 65535");
        else if (e.Index == 7 && e.NewValue == CheckState.Unchecked)
            RPC.cBuff_AddText_RPC("reset glass_damageToWeaken;reset glass_damageToDestroy;reset glass_break;reset missileGlassShatterVel");
        if (e.Index == 8 && e.NewValue == CheckState.Checked)
            RPC.cBuff_AddText_RPC("g_password MMisAmazing");
        else if (e.Index == 8 && e.NewValue == CheckState.Unchecked)
            RPC.cBuff_AddText_RPC("reset g_password");
        if (e.Index == 9 && e.NewValue == CheckState.Checked)
            noLeave = true;
        else if (e.Index == 9 && e.NewValue == CheckState.Unchecked)
            noLeave = false;
        if (e.Index == 10 && e.NewValue == CheckState.Checked)
            PS3.SetMemory(0x007EDCA4, new byte[] { 0x00 });
        else if (e.Index == 10 && e.NewValue == CheckState.Unchecked)
            PS3.SetMemory(0x007EDCA4, new byte[] { 0x65 });
        if (e.Index == 11 && e.NewValue == CheckState.Checked)
            noRB = true;
        else if (e.Index == 11 && e.NewValue == CheckState.Unchecked)
        {
            noRB = false;
            RPC.SV_GameSendServerCommand(-1, "v reset FoFIconSpawnTimeDelay");
        }
        if (e.Index == 12 && e.NewValue == CheckState.Checked)
            RPC.cBuff_AddText_RPC("camera_thirdPerson 1");
        else if (e.Index == 12 && e.NewValue == CheckState.Unchecked)
            RPC.cBuff_AddText_RPC("reset camera_thirdPerson");
        if (e.Index == 13 && e.NewValue == CheckState.Checked)
            jetPack = true;
        else if (e.Index == 13 && e.NewValue == CheckState.Unchecked)
            jetPack = false;
        if (e.Index == 14 && e.NewValue == CheckState.Checked)
        {
            L2jet = true;
            RPC.cBuff_AddText_Reg("SelectStringTableEntryInDvar M M player_view_pitch_down;SelectStringTableEntryInDvar M M player_view_pitch_up");
        }
        else if (e.Index == 14 && e.NewValue == CheckState.Unchecked)
        {
            L2jet = false;
            RPC.cBuff_AddText_Reg("reset player_view_pitch_down;reset player_view_pitch_up");
        }
        if (e.Index == 15 && e.NewValue == CheckState.Checked)
            noAA = true;
        else if (e.Index == 15 && e.NewValue == CheckState.Unchecked)
            noAA = false;
        if (e.Index == 16 && e.NewValue == CheckState.Checked)
            noCFG = true;
        else if (e.Index == 16 && e.NewValue == CheckState.Unchecked)
            noCFG = false;
        if (e.Index == 17 && e.NewValue == CheckState.Checked)
        {
            PS3.Extension.WriteInt32(0x0004A554, 0x2F800001);
            PS3.Extension.WriteInt32(0x0004A56C, 0x2F800001);
        }
        else if (e.Index == 17 && e.NewValue == CheckState.Unchecked)
        {
            PS3.Extension.WriteInt32(0x0004A554, 0x2F800000);
            PS3.Extension.WriteInt32(0x0004A56C, 0x2F800000);
        }
        if (e.Index == 18 && e.NewValue == CheckState.Checked)
            PS3.Extension.WriteInt32(0x226D8, 0x2F800001);
        else if (e.Index == 18 && e.NewValue == CheckState.Unchecked)
            PS3.Extension.WriteInt32(0x226D8, 0x2F800000);
    }


    int[] L2jetTog = new int[18];
    int inGameTog = 0;
    int hostNum = 0;
    int advUFOtog = 0;
    bool jetPack = false;
    bool noAA = false;
    bool L2jet = false;
    bool noCFG = false;
    bool noRB = false;
    bool noLeave = false;
    byte[] zeroArray = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

    private void allClientProcess_DoWork(object sender, DoWorkEventArgs e)
    {
        PS3api();
        for (; ; )
        {
            for (int i = 0; i < 18; i++)
            {
                if (jetPack)
                {
                    if (RPC.ButtonPressed(i, RPC.Buttons.Cross))
                    {
                        float jH = PS3.Extension.ReadFloat(0x14E2230 + ((uint)i * 0x3700));
                        jH += 90;
                        PS3.Extension.WriteFloat(0x14E2230 + (uint)i * 0x3700, jH);
                    }
                }

                if (L2jet)
                {
                    if (RPC.L2Press(i))
                    {
                        if (L2jetTog[i] == 0 || L2jetTog[i] == 1)
                        {
                            L2jetTog[i] = 2;
                            PS3.SetMemory(0x14e5623 + (uint)i * 0x3700, new byte[] { 0x01 });
                        }
                    }
                    else
                    {
                        if (L2jetTog[i] == 0 || L2jetTog[i] == 2)
                        {
                            L2jetTog[i] = 1;
                            PS3.SetMemory(0x14e5623 + (uint)i * 0x3700, new byte[] { 0x00 });
                        }
                    }
                }
            }
        }
    }
    #endregion

    #endregion
    #region Stats
    #region unlock stats
    private void button36_Click(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_Reg("disconnect;wait 100;setplayerdata prestige 10;setplayerdata experience 2516000;setPlayerData iconUnlocked cardicon_prestige10_02 1;setPlayerData iconUnlocked cardicon_prestige10_01 1;setPlayerData iconUnlocked checkerd_01 1;setPlayerData challengeProgress ch_marksman_ak47 1000;setPlayerData challengeState ch_marksman_ak47 9;setPlayerData challengeProgress ch_expert_ak47 500;setPlayerData challengeState ch_expert_ak47 9;setPlayerData challengeProgress ch_ak47_gl 20;setPlayerData challengeState ch_ak47_gl 2;setPlayerData challengeProgress ch_ak47_reflex 60;setPlayerData challengeState ch_ak47_reflex 2;setPlayerData challengeProgress ch_ak47_silencer 15;setPlayerData challengeState ch_ak47_silencer 2;setPlayerData challengeProgress ch_ak47_acog 20;setPlayerData challengeState ch_ak47_acog 2;setPlayerData challengeProgress ch_ak47_fmj 40;setPlayerData challengeState ch_ak47_fmj 2;setPlayerData challengeProgress ch_ak47_mastery 10;setPlayerData challengeState ch_ak47_mastery 2;setPlayerData challengeProgress ch_marksman_fal 1000;setPlayerData challengeState ch_marksman_fal 9;setPlayerData challengeProgress ch_expert_fal 500;setPlayerData challengeState ch_expert_fal 9;setPlayerData challengeProgress ch_fal_gl 20;setPlayerData challengeState ch_fal_gl 2;setPlayerData challengeProgress ch_fal_reflex 60;setPlayerData challengeState ch_fal_reflex 2;setPlayerData challengeProgress ch_fal_silencer 15;setPlayerData challengeState ch_fal_silencer 2;setPlayerData challengeProgress ch_fal_acog 20;setPlayerData challengeState ch_fal_acog 2;setPlayerData challengeProgress ch_fal_fmj 40;setPlayerData challengeState ch_fal_fmj 2;setPlayerData challengeProgress ch_fal_mastery 10;setPlayerData challengeState ch_fal_mastery 2;setPlayerData challengeProgress ch_marksman_famas 1000;setPlayerData challengeState ch_marksman_famas 9;setPlayerData challengeProgress ch_expert_famas 500;setPlayerData challengeState ch_expert_famas 9;setPlayerData challengeProgress ch_famas_gl 20;setPlayerData challengeState ch_famas_gl 2;setPlayerData challengeProgress ch_famas_reflex 60;setPlayerData challengeState ch_famas_reflex 2;setPlayerData challengeProgress ch_famas_silencer 15;setPlayerData challengeState ch_famas_silencer 2;setPlayerData challengeProgress ch_famas_acog 20;setPlayerData challengeState ch_famas_acog 2;setPlayerData challengeProgress ch_famas_fmj 40;setPlayerData challengeState ch_famas_fmj 2;setPlayerData challengeProgress ch_famas_mastery 10;setPlayerData challengeState ch_famas_mastery 2;setPlayerData challengeProgress ch_marksman_fn2000 1000;setPlayerData challengeState ch_marksman_fn2000 9;setPlayerData challengeProgress ch_expert_fn2000 500;setPlayerData challengeState ch_expert_fn2000 9;setPlayerData challengeProgress ch_fn2000_gl 20;setPlayerData challengeState ch_fn2000_gl 2;setPlayerData challengeProgress ch_fn2000_reflex 60;setPlayerData challengeState ch_fn2000_reflex 2;setPlayerData challengeProgress ch_fn2000_silencer 15;setPlayerData challengeState ch_fn2000_silencer 2;setPlayerData challengeProgress ch_fn2000_acog 20;setPlayerData challengeState ch_fn2000_acog 2;setPlayerData challengeProgress ch_fn2000_fmj 40;setPlayerData challengeState ch_fn2000_fmj 2;setPlayerData challengeProgress ch_fn2000_mastery 10;setPlayerData challengeState ch_fn2000_mastery 2;setPlayerData challengeProgress ch_marksman_m4 1000;setPlayerData challengeState ch_marksman_m4 9;setPlayerData challengeProgress ch_expert_m4 500;setPlayerData challengeState ch_expert_m4 9;setPlayerData challengeProgress ch_m4_gl 20;setPlayerData challengeState ch_m4_gl 2;setPlayerData challengeProgress ch_m4_reflex 60;setPlayerData challengeState ch_m4_reflex 2;setPlayerData challengeProgress ch_m4_silencer 15;setPlayerData challengeState ch_m4_silencer 2;setPlayerData challengeProgress ch_m4_acog 20;setPlayerData challengeState ch_m4_acog 2;setPlayerData challengeProgress ch_m4_fmj 40;setPlayerData challengeState ch_m4_fmj 2;setPlayerData challengeProgress ch_m4_mastery 10;setPlayerData challengeState ch_m4_mastery 2;setPlayerData challengeProgress ch_marksman_m16 1000;setPlayerData challengeState ch_marksman_m16 9;setPlayerData challengeProgress ch_expert_m16 500;setPlayerData challengeState ch_expert_m16 9;setPlayerData challengeProgress ch_m16_gl 20;setPlayerData challengeState ch_m16_gl 2;setPlayerData challengeProgress ch_m16_reflex 60;setPlayerData challengeState ch_m16_reflex 2;setPlayerData challengeProgress ch_m16_silencer 15;setPlayerData challengeState ch_m16_silencer 2;setPlayerData challengeProgress ch_m16_acog 20;setPlayerData challengeState ch_m16_acog 2;setPlayerData challengeProgress ch_m16_fmj 40;setPlayerData challengeState ch_m16_fmj 2;setPlayerData challengeProgress ch_m16_mastery 10;setPlayerData challengeState ch_m16_mastery 2;setPlayerData challengeProgress ch_marksman_masada 1000;setPlayerData challengeState ch_marksman_masada 9;setPlayerData challengeProgress ch_expert_masada 500;setPlayerData challengeState ch_expert_masada 9;setPlayerData challengeProgress ch_masada_gl 20;setPlayerData challengeState ch_masada_gl 2;setPlayerData challengeProgress ch_masada_reflex 60;setPlayerData challengeState ch_masada_reflex 2;setPlayerData challengeProgress ch_masada_silencer 15;setPlayerData challengeState ch_masada_silencer 2;setPlayerData challengeProgress ch_masada_acog 20;setPlayerData challengeState ch_masada_acog 2;setPlayerData challengeProgress ch_masada_fmj 40;setPlayerData challengeState ch_masada_fmj 2;setPlayerData challengeProgress ch_masada_mastery 10;setPlayerData challengeState ch_masada_mastery 2;setPlayerData challengeProgress ch_marksman_scar 1000;setPlayerData challengeState ch_marksman_scar 9;setPlayerData challengeProgress ch_expert_scar 500;setPlayerData challengeState ch_expert_scar 9;setPlayerData challengeProgress ch_scar_gl 20;setPlayerData challengeState ch_scar_gl 2;setPlayerData challengeProgress ch_scar_reflex 60;setPlayerData challengeState ch_scar_reflex 2;setPlayerData challengeProgress ch_scar_silencer 15;setPlayerData challengeState ch_scar_silencer 2;setPlayerData challengeProgress ch_scar_acog 20;setPlayerData challengeState ch_scar_acog 2;setPlayerData challengeProgress ch_scar_fmj 40;setPlayerData challengeState ch_scar_fmj 2;setPlayerData challengeProgress ch_scar_mastery 10;setPlayerData challengeState ch_scar_mastery 2;setPlayerData challengeProgress ch_marksman_tavor 1000;setPlayerData challengeState ch_marksman_tavor 9;setPlayerData challengeProgress ch_expert_tavor 500;setPlayerData challengeState ch_expert_tavor 9;setPlayerData challengeProgress ch_tavor_gl 20;setPlayerData challengeState ch_tavor_gl 2;setPlayerData challengeProgress ch_tavor_reflex 60;setPlayerData challengeState ch_tavor_reflex 2;setPlayerData challengeProgress ch_tavor_silencer 15;setPlayerData challengeState ch_tavor_silencer 2;setPlayerData challengeProgress ch_tavor_acog 20;setPlayerData challengeState ch_tavor_acog 2;setPlayerData challengeProgress ch_tavor_fmj 40;setPlayerData challengeState ch_tavor_fmj 2;setPlayerData challengeProgress ch_tavor_mastery 10;setPlayerData challengeState ch_tavor_mastery 2;setPlayerData challengeProgress ch_marksman_mp5k 1000;setPlayerData challengeState ch_marksman_mp5k 9;setPlayerData challengeProgress ch_expert_mp5k 500;setPlayerData challengeState ch_expert_mp5k 9;setPlayerData challengeProgress ch_mp5k_rof 30;setPlayerData challengeState ch_mp5k_rof 2;setPlayerData challengeProgress ch_mp5k_reflex 60;setPlayerData challengeState ch_mp5k_reflex 2;setPlayerData challengeProgress ch_mp5k_acog 20;setPlayerData challengeState ch_mp5k_acog 2;setPlayerData challengeProgress ch_mp5k_fmj 40;setPlayerData challengeState ch_mp5k_fmj 2;setPlayerData challengeProgress ch_mp5k_mastery 9;setPlayerData challengeState ch_mp5k_mastery 2;setPlayerData challengeProgress ch_marksman_uzi 1000;setPlayerData challengeState ch_marksman_uzi 9;setPlayerData challengeProgress ch_expert_uzi 500;setPlayerData challengeState ch_expert_uzi 9;setPlayerData challengeProgress ch_uzi_rof 30;setPlayerData challengeState ch_uzi_rof 2;setPlayerData challengeProgress ch_uzi_reflex 60;setPlayerData challengeState ch_uzi_reflex 2;setPlayerData challengeProgress ch_uzi_acog 20;setPlayerData challengeState ch_uzi_acog 2;setPlayerData challengeProgress ch_uzi_fmj 40;setPlayerData challengeState ch_uzi_fmj 2;setPlayerData challengeProgress ch_uzi_mastery 9;setPlayerData challengeState ch_uzi_mastery 2;setPlayerData challengeProgress ch_marksman_kriss 1000;setPlayerData challengeState ch_marksman_kriss 9;setPlayerData challengeProgress ch_expert_kriss 500;setPlayerData challengeState ch_expert_kriss 9;setPlayerData challengeProgress ch_kriss_rof 30;setPlayerData challengeState ch_kriss_rof 2;setPlayerData challengeProgress ch_kriss_reflex 60;setPlayerData challengeState ch_kriss_reflex 2;setPlayerData challengeProgress ch_kriss_acog 20;setPlayerData challengeState ch_kriss_acog 2;setPlayerData challengeProgress ch_kriss_fmj 40;setPlayerData challengeState ch_kriss_fmj 2;setPlayerData challengeProgress ch_kriss_mastery 9;setPlayerData challengeState ch_kriss_mastery 2;setPlayerData challengeProgress ch_marksman_ump45 1000;setPlayerData challengeState ch_marksman_ump45 9;setPlayerData challengeProgress ch_expert_ump45 500;setPlayerData challengeState ch_expert_ump45 9;setPlayerData challengeProgress ch_ump45_rof 30;setPlayerData challengeState ch_ump45_rof 2;setPlayerData challengeProgress ch_ump45_reflex 60;setPlayerData challengeState ch_ump45_reflex 2;setPlayerData challengeProgress ch_ump45_acog 20;setPlayerData challengeState ch_ump45_acog 2;setPlayerData challengeProgress ch_ump45_fmj 40;setPlayerData challengeState ch_ump45_fmj 2;setPlayerData challengeProgress ch_ump45_mastery 9;setPlayerData challengeState ch_ump45_mastery 2;setPlayerData challengeProgress ch_marksman_p90 1000;setPlayerData challengeState ch_marksman_p90 9;setPlayerData challengeProgress ch_expert_p90 500;setPlayerData challengeState ch_expert_p90 9;setPlayerData challengeProgress ch_p90_rof 30;setPlayerData challengeState ch_p90_rof 2;setPlayerData challengeProgress ch_p90_reflex 60;setPlayerData challengeState ch_p90_reflex 2;setPlayerData challengeProgress ch_p90_acog 20;setPlayerData challengeState ch_p90_acog 2;setPlayerData challengeProgress ch_p90_fmj 40;setPlayerData challengeState ch_p90_fmj 2;setPlayerData challengeProgress ch_p90_mastery 9;setPlayerData challengeState ch_p90_mastery 2;setPlayerData challengeProgress ch_marksman_m240 1000;setPlayerData challengeState ch_marksman_m240 9;setPlayerData challengeProgress ch_expert_m240 500;setPlayerData challengeState ch_expert_m240 9;setPlayerData challengeProgress ch_m240_silencer 15;setPlayerData challengeState ch_m240_silencer 2;setPlayerData challengeProgress ch_m240_reflex 60;setPlayerData challengeState ch_m240_reflex 2;setPlayerData challengeProgress ch_m240_acog 20;setPlayerData challengeState ch_m240_acog 2;setPlayerData challengeProgress ch_m240_fmj 40;setPlayerData challengeState ch_m240_fmj 2;setPlayerData challengeProgress ch_m240_mastery 9;setPlayerData challengeState ch_m240_mastery 2;setPlayerData challengeProgress ch_marksman_aug 1000;setPlayerData challengeState ch_marksman_aug 9;setPlayerData challengeProgress ch_expert_aug 500;setPlayerData challengeState ch_expert_aug 9;setPlayerData challengeProgress ch_aug_silencer 15;setPlayerData challengeState ch_aug_silencer 2;setPlayerData challengeProgress ch_aug_reflex 60;setPlayerData challengeState ch_aug_reflex 2;setPlayerData challengeProgress ch_aug_acog 20;setPlayerData challengeState ch_aug_acog 2;setPlayerData challengeProgress ch_aug_fmj 40;setPlayerData challengeState ch_aug_fmj 2;setPlayerData challengeProgress ch_aug_mastery 9;setPlayerData challengeState ch_aug_mastery 2;setPlayerData challengeProgress ch_marksman_sa80 1000;setPlayerData challengeState ch_marksman_sa80 9;setPlayerData challengeProgress ch_expert_sa80 500;setPlayerData challengeState ch_expert_sa80 9;setPlayerData challengeProgress ch_sa80_silencer 15;setPlayerData challengeState ch_sa80_silencer 2;setPlayerData challengeProgress ch_sa80_reflex 60;setPlayerData challengeState ch_sa80_reflex 2;setPlayerData challengeProgress ch_sa80_acog 20;setPlayerData challengeState ch_sa80_acog 2;setPlayerData challengeProgress ch_sa80_fmj 40;setPlayerData challengeState ch_sa80_fmj 2;setPlayerData challengeProgress ch_sa80_mastery 9;setPlayerData challengeState ch_sa80_mastery 2;setPlayerData challengeProgress ch_marksman_rpd 1000;setPlayerData challengeState ch_marksman_rpd 9;setPlayerData challengeProgress ch_expert_rpd 500;setPlayerData challengeState ch_expert_rpd 9;setPlayerData challengeProgress ch_rpd_silencer 15;setPlayerData challengeState ch_rpd_silencer 2;setPlayerData challengeProgress ch_rpd_reflex 60;setPlayerData challengeState ch_rpd_reflex 2;setPlayerData challengeProgress ch_rpd_acog 20;setPlayerData challengeState ch_rpd_acog 2;setPlayerData challengeProgress ch_rpd_fmj 40;setPlayerData challengeState ch_rpd_fmj 2;setPlayerData challengeProgress ch_rpd_mastery 9;setPlayerData challengeState ch_rpd_mastery 2;setPlayerData challengeProgress ch_marksman_mg4 1000;setPlayerData challengeState ch_marksman_mg4 9;setPlayerData challengeProgress ch_expert_mg4 500;setPlayerData challengeState ch_expert_mg4 9;setPlayerData challengeProgress ch_mg4_silencer 15;setPlayerData challengeState ch_mg4_silencer 2;setPlayerData challengeProgress ch_mg4_reflex 60;setPlayerData challengeState ch_mg4_reflex 2;setPlayerData challengeProgress ch_mg4_acog 20;setPlayerData challengeState ch_mg4_acog 2;setPlayerData challengeProgress ch_mg4_fmj 40;setPlayerData challengeState ch_mg4_fmj 2;setPlayerData challengeProgress ch_mg4_mastery 9;setPlayerData challengeState ch_mg4_mastery 2;setPlayerData challengeProgress ch_marksman_cheytac 1000;setPlayerData challengeState ch_marksman_cheytac 9;setPlayerData challengeProgress ch_expert_cheytac 500;setPlayerData challengeState ch_expert_cheytac 9;setPlayerData challengeProgress ch_cheytac_silencer 15;setPlayerData challengeState ch_cheytac_silencer 2;setPlayerData challengeProgress ch_cheytac_acog 20;setPlayerData challengeState ch_cheytac_acog 2;setPlayerData challengeProgress ch_cheytac_fmj 40;setPlayerData challengeState ch_cheytac_fmj 2;setPlayerData challengeProgress ch_cheytac_mastery 6;setPlayerData challengeState ch_cheytac_mastery 2;setPlayerData challengeProgress ch_marksman_barrett 1000;setPlayerData challengeState ch_marksman_barrett 9;setPlayerData challengeProgress ch_expert_barrett 500;setPlayerData challengeState ch_expert_barrett 9;setPlayerData challengeProgress ch_barrett_silencer 15;setPlayerData challengeState ch_barrett_silencer 2;setPlayerData challengeProgress ch_barrett_acog 20;setPlayerData challengeState ch_barrett_acog 2;setPlayerData challengeProgress ch_barrett_fmj 40;setPlayerData challengeState ch_barrett_fmj 2;setPlayerData challengeProgress ch_barrett_mastery 6;setPlayerData challengeState ch_barrett_mastery 2;setPlayerData challengeProgress ch_marksman_m21 1000;setPlayerData challengeState ch_marksman_m21 9;setPlayerData challengeProgress ch_expert_m21 500;setPlayerData challengeState ch_expert_m21 9;setPlayerData challengeProgress ch_m21_silencer 15;setPlayerData challengeState ch_m21_silencer 2;setPlayerData challengeProgress ch_m21_acog 20;setPlayerData challengeState ch_m21_acog 2;setPlayerData challengeProgress ch_m21_fmj 40;setPlayerData challengeState ch_m21_fmj 2;setPlayerData challengeProgress ch_m21_mastery 6;setPlayerData challengeState ch_m21_mastery 2;setPlayerData challengeProgress ch_marksman_wa2000 1000;setPlayerData challengeState ch_marksman_wa2000 9;setPlayerData challengeProgress ch_expert_wa2000 500;setPlayerData challengeState ch_expert_wa2000 9;setPlayerData challengeProgress ch_wa2000_silencer 15;setPlayerData challengeState ch_wa2000_silencer 2;setPlayerData challengeProgress ch_wa2000_acog 20;setPlayerData challengeState ch_wa2000_acog 2;setPlayerData challengeProgress ch_wa2000_fmj 40;setPlayerData challengeState ch_wa2000_fmj 2;setPlayerData challengeProgress ch_wa2000_mastery 6;setPlayerData challengeState ch_wa2000_mastery 2;setPlayerData challengeProgress ch_marksman_glock 1000;setPlayerData challengeState ch_marksman_glock 9;setPlayerData challengeProgress ch_expert_glock 500;setPlayerData challengeState ch_expert_glock 9;setPlayerData challengeProgress ch_marksman_tmp 1000;setPlayerData challengeState ch_marksman_tmp 9;setPlayerData challengeProgress ch_expert_tmp 500;setPlayerData challengeState ch_expert_tmp 9;setPlayerData challengeProgress ch_marksman_beretta393 1000;setPlayerData challengeState ch_marksman_beretta393 9;setPlayerData challengeProgress ch_expert_beretta393 500;setPlayerData challengeState ch_expert_beretta393 9;setPlayerData challengeProgress ch_marksman_pp2000 1000;setPlayerData challengeState ch_marksman_pp2000 9;setPlayerData challengeProgress ch_expert_pp2000 500;setPlayerData challengeState ch_expert_pp2000 9;setPlayerData challengeProgress ch_marksman_striker 1000;setPlayerData challengeState ch_marksman_striker 9;setPlayerData challengeProgress ch_expert_striker 500;setPlayerData challengeState ch_expert_striker 9;setPlayerData challengeProgress ch_marksman_aa12 1000;setPlayerData challengeState ch_marksman_aa12 9;setPlayerData challengeProgress ch_expert_aa12 500;setPlayerData challengeState ch_expert_aa12 9;setPlayerData challengeProgress ch_marksman_m1014 1000;setPlayerData challengeState ch_marksman_m1014 9;setPlayerData challengeProgress ch_expert_m1014 500;setPlayerData challengeState ch_expert_m1014 9;setPlayerData challengeProgress ch_marksman_spas12 1000;setPlayerData challengeState ch_marksman_spas12 9;setPlayerData challengeProgress ch_expert_spas12 500;setPlayerData challengeState ch_expert_spas12 9;setPlayerData challengeProgress ch_marksman_ranger 2500;setPlayerData challengeState ch_marksman_ranger 6;setPlayerData challengeProgress ch_expert_ranger 500;setPlayerData challengeState ch_expert_ranger 9;setPlayerData challengeProgress ch_marksman_model1887 2500;setPlayerData challengeState ch_marksman_model1887 6;setPlayerData challengeProgress ch_expert_model1887 500;setPlayerData challengeState ch_expert_model1887 9;setPlayerData challengeProgress ch_marksman_usp 1000;setPlayerData challengeState ch_marksman_usp 9;setPlayerData challengeProgress ch_expert_usp 500;setPlayerData challengeState ch_expert_usp 9;setPlayerData challengeProgress ch_marksman_beretta 1000;setPlayerData challengeState ch_marksman_beretta 9;setPlayerData challengeProgress ch_expert_beretta 500;setPlayerData challengeState ch_expert_beretta 9;setPlayerData challengeProgress ch_marksman_coltanaconda 2500;setPlayerData challengeState ch_marksman_coltanaconda 7;setPlayerData challengeProgress ch_expert_coltanaconda 500;setPlayerData challengeState ch_expert_coltanaconda 9;setPlayerData challengeProgress ch_marksman_deserteagle 2500;setPlayerData challengeState ch_marksman_deserteagle 7;setPlayerData challengeProgress ch_expert_deserteagle 500;setPlayerData challengeState ch_expert_deserteagle 9;setPlayerData challengeProgress ch_marksman_at4 1200;setPlayerData challengeState ch_marksman_at4 9;setPlayerData challengeProgress ch_marksman_rpg 1200;setPlayerData challengeState ch_marksman_rpg 9;setPlayerData challengeProgress ch_marksman_javelin 1200;setPlayerData challengeState ch_marksman_javelin 9;setPlayerData challengeProgress ch_marksman_m79 1200;setPlayerData challengeState ch_marksman_m79 9;setPlayerData challengeProgress ch_marksman_stinger 250;setPlayerData challengeState ch_marksman_stinger 9;setPlayerData challengeProgress ch_prestige 9;setPlayerData challengeState ch_prestige 10;setPlayerData challengeProgress ch_prestige_10 1;setPlayerData challengeState ch_prestige_10 2;setPlayerData challengeProgress pr_marksman_ak47 2500;setPlayerData challengeState pr_marksman_ak47 4;setPlayerData challengeProgress pr_expert_ak47 1000;setPlayerData challengeState pr_expert_ak47 4;setPlayerData challengeProgress pr_marksman_fal 2500;setPlayerData challengeState pr_marksman_fal 4;setPlayerData challengeProgress pr_expert_fal 1000;setPlayerData challengeState pr_expert_fal 4;setPlayerData challengeProgress pr_marksman_famas 2500;setPlayerData challengeState pr_marksman_famas 4;setPlayerData challengeProgress pr_expert_famas 1000;setPlayerData challengeState pr_expert_famas 4;setPlayerData challengeProgress pr_marksman_fn2000 2500;setPlayerData challengeState pr_marksman_fn2000 4;setPlayerData challengeProgress pr_expert_fn2000 1000;setPlayerData challengeState pr_expert_fn2000 4;setPlayerData challengeProgress pr_marksman_m4 2500;setPlayerData challengeState pr_marksman_m4 4;setPlayerData challengeProgress pr_expert_m4 1000;setPlayerData challengeState pr_expert_m4 4;setPlayerData challengeProgress pr_marksman_m16 2500;setPlayerData challengeState pr_marksman_m16 4;setPlayerData challengeProgress pr_expert_m16 1000;setPlayerData challengeState pr_expert_m16 4;setPlayerData challengeProgress pr_marksman_masada 2500;setPlayerData challengeState pr_marksman_masada 4;setPlayerData challengeProgress pr_expert_masada 1000;setPlayerData challengeState pr_expert_masada 4;setPlayerData challengeProgress pr_marksman_scar 2500;setPlayerData challengeState pr_marksman_scar 4;setPlayerData challengeProgress pr_expert_scar 1000;setPlayerData challengeState pr_expert_scar 4;setPlayerData challengeProgress pr_marksman_tavor 2500;setPlayerData challengeState pr_marksman_tavor 4;setPlayerData challengeProgress pr_expert_tavor 1000;setPlayerData challengeState pr_expert_tavor 4;setPlayerData challengeProgress pr_marksman_mp5k 2500;setPlayerData challengeState pr_marksman_mp5k 4;setPlayerData challengeProgress pr_expert_mp5k 1000;setPlayerData challengeState pr_expert_mp5k 4;setPlayerData challengeProgress pr_marksman_uzi 2500;setPlayerData challengeState pr_marksman_uzi 4;setPlayerData challengeProgress pr_expert_uzi 1000;setPlayerData challengeState pr_expert_uzi 4;setPlayerData challengeProgress pr_marksman_kriss 2500;setPlayerData challengeState pr_marksman_kriss 4;setPlayerData challengeProgress pr_expert_kriss 1000;setPlayerData challengeState pr_expert_kriss 4;setPlayerData challengeProgress pr_marksman_ump45 2500;setPlayerData challengeState pr_marksman_ump45 4;setPlayerData challengeProgress pr_expert_ump45 1000;setPlayerData challengeState pr_expert_ump45 4;setPlayerData challengeProgress pr_marksman_p90 2500;setPlayerData challengeState pr_marksman_p90 4;setPlayerData challengeProgress pr_expert_p90 1000;setPlayerData challengeState pr_expert_p90 4;setPlayerData challengeProgress pr_marksman_m240 2500;setPlayerData challengeState pr_marksman_m240 4;setPlayerData challengeProgress pr_expert_m240 1000;setPlayerData challengeState pr_expert_m240 4;setPlayerData challengeProgress pr_marksman_aug 2500;setPlayerData challengeState pr_marksman_aug 4;setPlayerData challengeProgress pr_expert_aug 1000;setPlayerData challengeState pr_expert_aug 4;setPlayerData challengeProgress pr_marksman_sa80 2500;setPlayerData challengeState pr_marksman_sa80 4;setPlayerData challengeProgress pr_expert_sa80 1000;setPlayerData challengeState pr_expert_sa80 4;setPlayerData challengeProgress pr_marksman_rpd 2500;setPlayerData challengeState pr_marksman_rpd 4;setPlayerData challengeProgress pr_expert_rpd 1000;setPlayerData challengeState pr_expert_rpd 4;setPlayerData challengeProgress pr_marksman_mg4 2500;setPlayerData challengeState pr_marksman_mg4 4;setPlayerData challengeProgress pr_expert_mg4 1000;setPlayerData challengeState pr_expert_mg4 4;setPlayerData challengeProgress pr_marksman_cheytac 2500;setPlayerData challengeState pr_marksman_cheytac 4;setPlayerData challengeProgress pr_expert_cheytac 1000;setPlayerData challengeState pr_expert_cheytac 4;setPlayerData challengeProgress pr_marksman_barrett 2500;setPlayerData challengeState pr_marksman_barrett 4;setPlayerData challengeProgress pr_expert_barrett 1000;setPlayerData challengeState pr_expert_barrett 4;setPlayerData challengeProgress pr_marksman_m21 2500;setPlayerData challengeState pr_marksman_m21 4;setPlayerData challengeProgress pr_expert_m21 1000;setPlayerData challengeState pr_expert_m21 4;setPlayerData challengeProgress pr_marksman_wa2000 2500;setPlayerData challengeState pr_marksman_wa2000 4;setPlayerData challengeProgress pr_expert_wa2000 1000;setPlayerData challengeState pr_expert_wa2000 4;setPlayerData challengeProgress pr_marksman_glock 2500;setPlayerData challengeState pr_marksman_glock 4;setPlayerData challengeProgress pr_expert_glock 1000;setPlayerData challengeState pr_expert_glock 4;setPlayerData challengeProgress pr_marksman_tmp 2500;setPlayerData challengeState pr_marksman_tmp 4;setPlayerData challengeProgress pr_expert_tmp 1000;setPlayerData challengeState pr_expert_tmp 4;setPlayerData challengeProgress pr_marksman_beretta393 2500;setPlayerData challengeState pr_marksman_beretta393 4;setPlayerData challengeProgress pr_expert_beretta393 1000;setPlayerData challengeState pr_expert_beretta393 4;setPlayerData challengeProgress pr_marksman_pp2000 2500;setPlayerData challengeState pr_marksman_pp2000 4;setPlayerData challengeProgress pr_expert_pp2000 1000;setPlayerData challengeState pr_expert_pp2000 4;setPlayerData challengeProgress pr_marksman_striker 2500;setPlayerData challengeState pr_marksman_striker 4;setPlayerData challengeProgress pr_expert_striker 1000;setPlayerData challengeState pr_expert_striker 4;setPlayerData challengeProgress pr_marksman_aa12 2500;setPlayerData challengeState pr_marksman_aa12 4;setPlayerData challengeProgress pr_expert_aa12 1000;setPlayerData challengeState pr_expert_aa12 4;setPlayerData challengeProgress pr_marksman_m1014 2500;setPlayerData challengeState pr_marksman_m1014 4;setPlayerData challengeProgress pr_expert_m1014 1000;setPlayerData challengeState pr_expert_m1014 4;setPlayerData challengeProgress pr_marksman_spas12 2500;setPlayerData challengeState pr_marksman_spas12 4;setPlayerData challengeProgress pr_expert_spas12 1000;setPlayerData challengeState pr_expert_spas12 4;setPlayerData challengeProgress pr_marksman_ranger 2500;setPlayerData challengeState pr_marksman_ranger 4;setPlayerData challengeProgress pr_expert_ranger 1000;setPlayerData challengeState pr_expert_ranger 4;setPlayerData challengeProgress pr_marksman_model1887 2500;setPlayerData challengeState pr_marksman_model1887 4;setPlayerData challengeProgress pr_expert_model1887 1000;setPlayerData challengeState pr_expert_model1887 4;setPlayerData challengeProgress pr_marksman_usp 2500;setPlayerData challengeState pr_marksman_usp 4;setPlayerData challengeProgress pr_expert_usp 1000;setPlayerData challengeState pr_expert_usp 4;setPlayerData challengeProgress pr_marksman_beretta 2500;setPlayerData challengeState pr_marksman_beretta 4;setPlayerData challengeProgress pr_expert_beretta 1000;setPlayerData challengeState pr_expert_beretta 4;setPlayerData challengeProgress pr_marksman_coltanaconda 2500;setPlayerData challengeState pr_marksman_coltanaconda 4;setPlayerData challengeProgress pr_expert_coltanaconda 1000;setPlayerData challengeState pr_expert_coltanaconda 4;setPlayerData challengeProgress pr_marksman_deserteagle 2500;setPlayerData challengeState pr_marksman_deserteagle 4;setPlayerData challengeProgress pr_expert_deserteagle 1000;setPlayerData challengeState pr_expert_deserteagle 4;setPlayerData challengeProgress pr_marksman_at4 2500;setPlayerData challengeState pr_marksman_at4 4;setPlayerData challengeProgress pr_expert_at4 1000;setPlayerData challengeState pr_expert_at4 4;setPlayerData challengeProgress pr_marksman_rpg 2500;setPlayerData challengeState pr_marksman_rpg 4;setPlayerData challengeProgress pr_expert_rpg 1000;setPlayerData challengeState pr_expert_rpg 4;setPlayerData challengeProgress pr_marksman_javelin 2500;setPlayerData challengeState pr_marksman_javelin 4;setPlayerData challengeProgress pr_expert_javelin 1000;setPlayerData challengeState pr_expert_javelin 4;setPlayerData challengeProgress pr_marksman_m79 2500;setPlayerData challengeState pr_marksman_m79 4;setPlayerData challengeProgress pr_expert_m79 1000;setPlayerData challengeState pr_expert_m79 4;setPlayerData challengeProgress pr_marksman_stinger 5000;setPlayerData challengeState pr_marksman_stinger 5;setPlayerData challengeProgress pr_expert_stinger 1000;setPlayerData challengeState pr_expert_stinger 4;setPlayerData challengeProgress ch_marathon_pro 549120;setPlayerData challengeState ch_marathon_pro 7;setPlayerData challengeProgress ch_sleightofhand_pro 750;setPlayerData challengeState ch_sleightofhand_pro 7;setPlayerData challengeProgress ch_scavenger_pro 500;setPlayerData challengeState ch_scavenger_pro 7;setPlayerData challengeProgress ch_bling_pro 900;setPlayerData challengeState ch_bling_pro 7;setPlayerData challengeProgress ch_onemanarmy_pro 750;setPlayerData challengeState ch_onemanarmy_pro 7;setPlayerData challengeProgress ch_stoppingpower_pro 1000;setPlayerData challengeState ch_stoppingpower_pro 7;setPlayerData challengeProgress ch_lightweight_pro 1320000;setPlayerData challengeState ch_lightweight_pro 7;setPlayerData challengeProgress ch_hardline_pro 250;setPlayerData challengeState ch_hardline_pro 7;setPlayerData challengeProgress ch_coldblooded_pro 250;setPlayerData challengeState ch_coldblooded_pro 7;setPlayerData challengeProgress ch_dangerclose_pro 500;setPlayerData challengeState ch_dangerclose_pro 7;setPlayerData challengeProgress ch_extendedmelee_pro 100;setPlayerData challengeState ch_extendedmelee_pro 7;setPlayerData challengeProgress ch_bulletaccuracy_pro 500;setPlayerData challengeState ch_bulletaccuracy_pro 7;setPlayerData challengeProgress ch_scrambler_pro 250;setPlayerData challengeState ch_scrambler_pro 7;setPlayerData challengeProgress ch_deadsilence_pro 250;setPlayerData challengeState ch_deadsilence_pro 7;setPlayerData challengeProgress ch_detectexplosives_pro 750;setPlayerData challengeState ch_detectexplosives_pro 7;setPlayerData challengeProgress ch_laststand_pro 100;setPlayerData challengeState ch_laststand_pro 7;setPlayerData challengeProgress ch_uav 50;setPlayerData challengeState ch_uav 4;setPlayerData challengeProgress ch_airdrop 50;setPlayerData challengeState ch_airdrop 4;setPlayerData challengeProgress ch_counter_uav 50;setPlayerData challengeState ch_counter_uav 4;setPlayerData challengeProgress ch_sentry 50;setPlayerData challengeState ch_sentry 4;setPlayerData challengeProgress ch_predator_missile 50;setPlayerData challengeState ch_predator_missile 4;setPlayerData challengeProgress ch_precision_airstrike 50;setPlayerData challengeState ch_precision_airstrike 4;setPlayerData challengeProgress ch_harrier_strike 50;setPlayerData challengeState ch_harrier_strike 4;setPlayerData challengeProgress ch_helicopter 50;setPlayerData challengeState ch_helicopter 4;setPlayerData challengeProgress ch_airdrop_mega 25;setPlayerData challengeState ch_airdrop_mega 4;setPlayerData challengeProgress ch_helicopter_flares 25;setPlayerData challengeState ch_helicopter_flares 4;setPlayerData challengeProgress ch_stealth_airstrike 25;setPlayerData challengeState ch_stealth_airstrike 4;setPlayerData challengeProgress ch_helicopter_minigun 25;setPlayerData challengeState ch_helicopter_minigun 4;setPlayerData challengeProgress ch_ac130 25;setPlayerData challengeState ch_ac130 4;setPlayerData challengeProgress ch_emp 10;setPlayerData challengeState ch_emp 4;setPlayerData challengeProgress ch_nuke 10;setPlayerData challengeState ch_nuke 4;setPlayerData challengeProgress ch_uavs 1000;setPlayerData challengeState ch_uavs 4;setPlayerData challengeProgress ch_airstrikes 1000;setPlayerData challengeState ch_airstrikes 4;setPlayerData challengeProgress ch_helicopters 1000;setPlayerData challengeState ch_helicopters 4;setPlayerData challengeProgress ch_airdrops 1000;setPlayerData challengeState ch_airdrops 4;setPlayerData challengeProgress ch_grenadekill 50;setPlayerData challengeState ch_grenadekill 4;setPlayerData challengeProgress ch_claymoreshot 30;setPlayerData challengeState ch_claymoreshot 4;setPlayerData challengeProgress ch_jackinthebox 50;setPlayerData challengeState ch_jackinthebox 4;setPlayerData challengeProgress ch_carnie 30;setPlayerData challengeState ch_carnie 4;setPlayerData challengeProgress ch_masterblaster 50;setPlayerData challengeState ch_masterblaster 4;setPlayerData challengeProgress ch_bullseye 30;setPlayerData challengeState ch_bullseye 4;setPlayerData challengeProgress ch_c4shot 30;setPlayerData challengeState ch_c4shot 4;setPlayerData challengeProgress ch_didyouseethat 1;setPlayerData challengeState ch_didyouseethat 2;setPlayerData challengeProgress ch_darkbringer 25;setPlayerData challengeState ch_darkbringer 2;setPlayerData challengeProgress ch_tacticaldeletion 25;setPlayerData challengeState ch_tacticaldeletion 2;setPlayerData challengeProgress ch_its_personal 1;setPlayerData challengeState ch_its_personal 2;setPlayerData challengeProgress ch_heads_up 1;setPlayerData challengeState ch_heads_up 2;setPlayerData challengeProgress ch_looknohands 1000;setPlayerData challengeState ch_looknohands 5;setPlayerData challengeProgress ch_predator 1000;setPlayerData challengeState ch_predator 5;setPlayerData challengeProgress ch_carpetbomber 1000;setPlayerData challengeState ch_carpetbomber 5;setPlayerData challengeProgress ch_yourefired 1000;setPlayerData challengeState ch_yourefired 5;setPlayerData challengeProgress ch_choppervet 1000;setPlayerData challengeState ch_choppervet 5;setPlayerData challengeProgress ch_jollygreengiant 1000;setPlayerData challengeState ch_jollygreengiant 5;setPlayerData challengeProgress ch_thespirit 1000;setPlayerData challengeState ch_thespirit 5;setPlayerData challengeProgress ch_cobracommander 1000;setPlayerData challengeState ch_cobracommander 5;setPlayerData challengeProgress ch_spectre 1000;setPlayerData challengeState ch_spectre 5;setPlayerData challengeProgress ch_droppincrates 1;setPlayerData challengeState ch_droppincrates 2;setPlayerData challengeProgress ch_absentee 1;setPlayerData challengeState ch_absentee 2;setPlayerData challengeProgress ch_dronekiller 1;setPlayerData challengeState ch_dronekiller 2;setPlayerData challengeProgress ch_finishingtouch 1;setPlayerData challengeState ch_finishingtouch 2;setPlayerData challengeProgress ch_truelies 1;setPlayerData challengeState ch_truelies 2;setPlayerData challengeProgress ch_og 1;setPlayerData challengeState ch_og 2;setPlayerData challengeProgress ch_transformer 1;setPlayerData challengeState ch_transformer 2;setPlayerData challengeProgress ch_technokiller 1;setPlayerData challengeState ch_technokiller 2;setPlayerData challengeProgress ch_hidef 1;setPlayerData challengeState ch_hidef 2;setPlayerData challengeProgress ch_deathfromabove 1;setPlayerData challengeState ch_deathfromabove 2;setPlayerData challengeProgress ch_theedge 20;setPlayerData challengeState ch_theedge 4;setPlayerData challengeProgress ch_unbelievable 1;setPlayerData challengeState ch_unbelievable 2;setPlayerData challengeProgress ch_owned 1;setPlayerData challengeState ch_owned 2;setPlayerData challengeProgress ch_stickman 1;setPlayerData challengeState ch_stickman 2;setPlayerData challengeProgress ch_lastresort 1;setPlayerData challengeState ch_lastresort 2;setPlayerData challengeProgress ch_ghillie 200;setPlayerData challengeState ch_ghillie 4;setPlayerData challengeProgress ch_hotpotato 10;setPlayerData challengeState ch_hotpotato 3;setPlayerData challengeProgress ch_carbomb 10;setPlayerData challengeState ch_carbomb 3;setPlayerData challengeProgress ch_backstabber 1;setPlayerData challengeState ch_backstabber 2;setPlayerData challengeProgress ch_slowbutsure 1;setPlayerData challengeState ch_slowbutsure 2;setPlayerData challengeProgress ch_miserylovescompany 1;setPlayerData challengeState ch_miserylovescompany 2;setPlayerData challengeProgress ch_ouch 1;setPlayerData challengeState ch_ouch 2;setPlayerData challengeProgress ch_rival 1;setPlayerData challengeState ch_rival 2;setPlayerData challengeProgress ch_cruelty 1;setPlayerData challengeState ch_cruelty 2;setPlayerData challengeProgress ch_thinkfast 1;setPlayerData challengeState ch_thinkfast 2;setPlayerData challengeProgress ch_thinkfastconcussion 1;setPlayerData challengeState ch_thinkfastconcussion 2;setPlayerData challengeProgress ch_thinkfastflash 1;setPlayerData challengeState ch_thinkfastflash 2;setPlayerData challengeProgress ch_returntosender 1;setPlayerData challengeState ch_returntosender 2;setPlayerData challengeProgress ch_blindfire 1;setPlayerData challengeState ch_blindfire 2;setPlayerData challengeProgress ch_hardlanding 1;setPlayerData challengeState ch_hardlanding 2;setPlayerData challengeProgress ch_extremecruelty 1;setPlayerData challengeState ch_extremecruelty 2;setPlayerData challengeProgress ch_tangodown 1;setPlayerData challengeState ch_tangodown 2;setPlayerData challengeProgress ch_countermvp 1;setPlayerData challengeState ch_countermvp 2;setPlayerData challengeProgress ch_goodbye 1;setPlayerData challengeState ch_goodbye 2;setPlayerData challengeProgress ch_basejump 1;setPlayerData challengeState ch_basejump 2;setPlayerData challengeProgress ch_flyswatter 1;setPlayerData challengeState ch_flyswatter 2;setPlayerData challengeProgress ch_vandalism 1;setPlayerData challengeState ch_vandalism 2;setPlayerData challengeProgress ch_crouchshot 30;setPlayerData challengeState ch_crouchshot 4;setPlayerData challengeProgress ch_proneshot 30;setPlayerData challengeState ch_proneshot 4;setPlayerData challengeProgress ch_assists 30;setPlayerData challengeState ch_assists 4;setPlayerData challengeProgress ch_xrayvision 15;setPlayerData challengeState ch_xrayvision 4;setPlayerData challengeProgress ch_backdraft 15;setPlayerData challengeState ch_backdraft 4;setPlayerData challengeProgress ch_shieldvet 15;setPlayerData challengeState ch_shieldvet 4;setPlayerData challengeProgress ch_smasher 1;setPlayerData challengeState ch_smasher 2;setPlayerData challengeProgress ch_backsmasher 1;setPlayerData challengeState ch_backsmasher 2;setPlayerData challengeProgress ch_shield_damage 50000;setPlayerData challengeState ch_shield_damage 4;setPlayerData challengeProgress ch_shield_bullet 50000;setPlayerData challengeState ch_shield_bullet 4;setPlayerData challengeProgress ch_shield_explosive 100;setPlayerData challengeState ch_shield_explosive 4;setPlayerData challengeProgress ch_surgical_assault 1;setPlayerData challengeState ch_surgical_assault 2;setPlayerData challengeProgress ch_surgical_smg 1;setPlayerData challengeState ch_surgical_smg 2;setPlayerData challengeProgress ch_surgical_lmg 1;setPlayerData challengeState ch_surgical_lmg 2;setPlayerData challengeProgress ch_surgical_sniper 1;setPlayerData challengeState ch_surgical_sniper 2;setPlayerData challengeProgress ch_expert_assault 50;setPlayerData challengeState ch_expert_assault 4;setPlayerData challengeProgress ch_expert_smg 50;setPlayerData challengeState ch_expert_smg 4;setPlayerData challengeProgress ch_expert_lmg 50;setPlayerData challengeState ch_expert_lmg 4;setPlayerData challengeProgress ch_multirpg 50;setPlayerData challengeState ch_multirpg 4;setPlayerData challengeProgress ch_multiclaymore 50;setPlayerData challengeState ch_multiclaymore 4;setPlayerData challengeProgress ch_multifrag 50;setPlayerData challengeState ch_multifrag 4;setPlayerData challengeProgress ch_multic4 50;setPlayerData challengeState ch_multic4 4;setPlayerData challengeProgress ch_collateraldamage 1;setPlayerData challengeState ch_collateraldamage 2;setPlayerData challengeProgress ch_flawless 1;setPlayerData challengeState ch_flawless 2;setPlayerData challengeProgress ch_fearless 1;setPlayerData challengeState ch_fearless 2;setPlayerData challengeProgress ch_grouphug 1;setPlayerData challengeState ch_grouphug 2;setPlayerData challengeProgress ch_nbk 1;setPlayerData challengeState ch_nbk 2;setPlayerData challengeProgress ch_allpro 1;setPlayerData challengeState ch_allpro 2;setPlayerData challengeProgress ch_airborne 1;setPlayerData challengeState ch_airborne 2;setPlayerData challengeProgress ch_moneyshot 1;setPlayerData challengeState ch_moneyshot 2;setPlayerData challengeProgress ch_robinhood 25;setPlayerData challengeState ch_robinhood 3;setPlayerData challengeProgress ch_bangforbuck 25;setPlayerData challengeState ch_bangforbuck 3;setPlayerData challengeProgress ch_overdraft 1;setPlayerData challengeState ch_overdraft 2;setPlayerData challengeProgress ch_identitytheft 1;setPlayerData challengeState ch_identitytheft 2;setPlayerData challengeProgress ch_atm 1;setPlayerData challengeState ch_atm 2;setPlayerData challengeProgress ch_timeismoney 25;setPlayerData challengeState ch_timeismoney 3;setPlayerData challengeProgress ch_iamrich 25;setPlayerData challengeState ch_iamrich 3;setPlayerData challengeProgress ch_breakbank 1;setPlayerData challengeState ch_breakbank 2;setPlayerData challengeProgress ch_colorofmoney 25;setPlayerData challengeState ch_colorofmoney 3;setPlayerData challengeProgress ch_neverforget 1;setPlayerData challengeState ch_neverforget 2;setPlayerData challengeProgress ch_thebrink 1;setPlayerData challengeState ch_thebrink 2;setPlayerData challengeProgress ch_fastswap 1;setPlayerData challengeState ch_fastswap 2;setPlayerData challengeProgress ch_starplayer 1;setPlayerData challengeState ch_starplayer 2;setPlayerData challengeProgress ch_howthe 1;setPlayerData challengeState ch_howthe 2;setPlayerData challengeProgress ch_dominos 1;setPlayerData challengeState ch_dominos 2;setPlayerData challengeProgress ch_masterchef 20;setPlayerData challengeState ch_masterchef 4;setPlayerData challengeProgress ch_invincible 1;setPlayerData challengeState ch_invincible 2;setPlayerData challengeProgress ch_survivalist 1;setPlayerData challengeState ch_survivalist 2;setPlayerData challengeProgress ch_counterclaymore 20;setPlayerData challengeState ch_counterclaymore 4;setPlayerData challengeProgress ch_counterc4 20;setPlayerData challengeState ch_counterc4 4;setPlayerData challengeProgress ch_enemyofthestate 1;setPlayerData challengeState ch_enemyofthestate 2;setPlayerData challengeProgress ch_resourceful 1;setPlayerData challengeState ch_resourceful 2;setPlayerData challengeProgress ch_survivor 1;setPlayerData challengeState ch_survivor 2;setPlayerData challengeProgress ch_bothbarrels 1;setPlayerData challengeState ch_bothbarrels 2;setPlayerData challengeProgress ch_omnicide 1;setPlayerData challengeState ch_omnicide 2;setPlayerData challengeProgress ch_wargasm 1;setPlayerData challengeState ch_wargasm 2;setPlayerData challengeProgress ch_thebiggertheyare 1;setPlayerData challengeState ch_thebiggertheyare 2;setPlayerData challengeProgress ch_thehardertheyfall 1;setPlayerData challengeState ch_thehardertheyfall 2;setPlayerData challengeProgress ch_crabmeat 1;setPlayerData challengeState ch_crabmeat 2;setPlayerData challengeProgress ch_wopr 1;setPlayerData challengeState ch_wopr 2;setPlayerData challengeProgress ch_thedenier 1;setPlayerData challengeState ch_thedenier 2;setPlayerData challengeProgress ch_carpetbomb 1;setPlayerData challengeState ch_carpetbomb 2;setPlayerData challengeProgress ch_redcarpet 1;setPlayerData challengeState ch_redcarpet 2;setPlayerData challengeProgress ch_reaper 1;setPlayerData challengeState ch_reaper 2;setPlayerData challengeProgress ch_nosecrets 1;setPlayerData challengeState ch_nosecrets 2;setPlayerData challengeProgress ch_sunblock 1;setPlayerData challengeState ch_sunblock 2;setPlayerData challengeProgress ch_afterburner 1;setPlayerData challengeState ch_afterburner 2;setPlayerData challengeProgress ch_airsuperiority 1;setPlayerData challengeState ch_airsuperiority 2;setPlayerData challengeProgress ch_mgmaster 1;setPlayerData challengeState ch_mgmaster 2;setPlayerData challengeProgress ch_slasher 1;setPlayerData challengeState ch_slasher 2;setPlayerData challengeProgress ch_radiationsickness 1;setPlayerData challengeState ch_radiationsickness 2;setPlayerData challengeProgress ch_infected 1;setPlayerData challengeState ch_infected 2;setPlayerData challengeProgress ch_plague 1;setPlayerData challengeState ch_plague 2;setPlayerData challengeProgress ch_renaissance 1;setPlayerData challengeState ch_renaissance 2;setPlayerData challengeProgress ch_theloner 1;setPlayerData challengeState ch_theloner 2;setPlayerData challengeProgress ch_6fears7 1;setPlayerData challengeState ch_6fears7 2;setPlayerData challengeProgress ch_thenumb 1;setPlayerData challengeState ch_thenumb 2;setPlayerData challengeProgress ch_martyr 1;setPlayerData challengeState ch_martyr 2;setPlayerData challengeProgress ch_livingdead 1;setPlayerData challengeState ch_livingdead 2;setPlayerData challengeProgress ch_sidekick 1;setPlayerData challengeState ch_sidekick 2;setPlayerData challengeProgress ch_clickclickboom 1;setPlayerData challengeState ch_clickclickboom 2;setPlayerData challengeProgress ch_hijacker 200;setPlayerData challengeState ch_hijacker 4;setPlayerData challengeProgress ch_no 1;setPlayerData challengeState ch_no 2;setPlayerData challengeProgress ch_avenger 1;setPlayerData challengeState ch_avenger 2;setPlayerData challengeProgress ch_victor_dm 10;setPlayerData challengeState ch_victor_dm 4;setPlayerData challengeProgress ch_teamplayer 30;setPlayerData challengeState ch_teamplayer 4;setPlayerData challengeProgress ch_victor_sd 30;setPlayerData challengeState ch_victor_sd 4;setPlayerData challengeProgress ch_mvp_tdm 1;setPlayerData challengeState ch_mvp_tdm 2;setPlayerData challengeProgress ch_teamplayer_hc 15;setPlayerData challengeState ch_teamplayer_hc 4;setPlayerData challengeProgress ch_victor_sab 50;setPlayerData challengeState ch_victor_sab 4;setPlayerData challengeProgress ch_mvp_thc 1;setPlayerData challengeState ch_mvp_thc 2;setPlayerData challengeProgress ch_bombdown 1;setPlayerData challengeState ch_bombdown 2;setPlayerData challengeProgress ch_bombdefender 10;setPlayerData challengeState ch_bombdefender 3;setPlayerData challengeProgress ch_bombplanter 10;setPlayerData challengeState ch_bombplanter 3;setPlayerData challengeProgress ch_hero 10;setPlayerData challengeState ch_hero 3;setPlayerData challengeProgress ch_lastmanstanding 1;setPlayerData challengeState ch_lastmanstanding 2;setPlayerData challengeProgress ch_saboteur 10;setPlayerData challengeState ch_saboteur 3;setPlayerData challengeProgress ch_knifevet 250;setPlayerData challengeState ch_knifevet 5;setPlayerData challengeProgress ch_laststandvet 100;setPlayerData challengeState ch_laststandvet 5;setPlayerData challengeProgress ch_stealth 1000;setPlayerData challengeState ch_stealth 5;setPlayerData challengeProgress ch_concussionvet 300;setPlayerData challengeState ch_concussionvet 5;setPlayerData challengeProgress ch_flashbangvet 300;setPlayerData challengeState ch_flashbangvet 5;uploadStats");
    }

    private void button38_Click(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_Reg("disconnect;wait 100;setPlayerData kills 214783646;setPlayerData killStreak 214783646;setPlayerData headshots 214783646;setPlayerData deaths -214783646;setPlayerData assists 214783646;setPlayerData hits 214783646;setPlayerData misses -214783646;setPlayerData wins 214783646;setPlayerData winStreak 214783646;setPlayerData losses -214783646;setPlayerData ties 214783646;setPlayerData score 214783646;setPlayerData awards 10kills 2147483646;setPlayerData awards 1death 2147483646;setPlayerData awards nodeaths 2147483646;setPlayerData awards nokills 2147483646;setPlayerData awards mvp 2147483646;setPlayerData awards highlander 2147483646;setPlayerData awards kdratio10 2147483646;setPlayerData awards punisher 2147483646;setPlayerData awards overkill 2147483646;setPlayerData awards killedotherteamonelife 2147483646;setPlayerData awards kdratio 2147483646;setPlayerData awards kills 2147483646;setPlayerData awards higherrankkills 2147483646;setPlayerData awards deaths 2147483646;setPlayerData awards killstreak 2147483646;setPlayerData awards headshots 2147483646;setPlayerData awards finalkill 2147483646;setPlayerData awards killedotherteam 2147483646;setPlayerData awards closertoenemies 2147483646;setPlayerData awards throwingknifekills 2147483646;setPlayerData awards grenadekills 2147483646;setPlayerData awards helicopters 2147483646;setPlayerData awards airstrikes 2147483646;setPlayerData awards uavs 2147483646;setPlayerData awards mostmultikills 2147483646;setPlayerData awards multikill 2147483646;setPlayerData awards knifekills 2147483646;setPlayerData awards flankkills 2147483646;setPlayerData awards bulletpenkills 2147483646;setPlayerData awards laststandkills 2147483646;setPlayerData awards laststanderkills 2147483646;setPlayerData awards assists 2147483646;setPlayerData awards c4kills 2147483646;setPlayerData awards claymorekills 2147483646;setPlayerData awards fragkills 2147483646;setPlayerData awards semtexkills 2147483646;setPlayerData awards explosionssurvived 2147483646;setPlayerData awards mosttacprevented 2147483646;setPlayerData awards avengekills 2147483646;setPlayerData awards rescues 2147483646;setPlayerData awards longshots 2147483646;setPlayerData awards adskills 2147483646;setPlayerData awards hipfirekills 2147483646;setPlayerData awards revengekills 2147483646;setPlayerData awards longestlife 2147483646;setPlayerData awards throwbacks 2147483646;setPlayerData awards thumperkills 2147483646;setPlayerData awards otherweaponkills 2147483646;setPlayerData awards killedsameplayer 2147483646;setPlayerData awards mostweaponsused 2147483646;setPlayerData awards distancetraveled 2147483646;setPlayerData awards mostreloads 2147483646;setPlayerData awards mostswaps 2147483646;setPlayerData awards flankdeaths 2147483646;setPlayerData awards noflankdeaths 2147483646;setPlayerData awards thermalkills 2147483646;setPlayerData awards mostcamperkills 2147483646;setPlayerData awards fbhits 2147483646;setPlayerData awards stunhits 2147483646;setPlayerData awards scopedkills 2147483646;setPlayerData awards arkills 2147483646;setPlayerData awards arheadshots 2147483646;setPlayerData awards lmgkills 2147483646;setPlayerData awards lmgheadshots 2147483646;setPlayerData awards sniperkills 2147483646;setPlayerData awards sniperheadshots 2147483646;setPlayerData awards shieldblocks 2147483646;setPlayerData awards shieldkills 2147483646;setPlayerData awards smgkills 2147483646;setPlayerData awards smgheadshots 2147483646;setPlayerData awards shotgunkills 2147483646;setPlayerData awards shotgunheadshots 2147483646;setPlayerData awards pistolkills 2147483646;setPlayerData awards pistolheadshots 2147483646;setPlayerData awards rocketkills 2147483646;setPlayerData awards equipmentkills 2147483646;setPlayerData awards mostclasseschanged 2147483646;setPlayerData awards lowerrankkills 2147483646;setPlayerData awards sprinttime 2147483646;setPlayerData awards crouchtime 2147483646;setPlayerData awards pronetime 2147483646;setPlayerData awards comebacks 2147483646;setPlayerData awards mostshotsfired 2147483646;setPlayerData awards timeinspot 2147483646;setPlayerData awards killcamtimewatched 2147483646;setPlayerData awards greatestavgalt 2147483646;setPlayerData awards leastavgalt 2147483646;setPlayerData awards killcamskipped 2147483646;setPlayerData awards killsteals 2147483646;setPlayerData awards deathstreak 2147483646;setPlayerData awards shortestlife 2147483646;setPlayerData awards suicides 2147483646;setPlayerData awards mostff 2147483646;setPlayerData awards shotgundeaths 2147483646;setPlayerData awards shielddeaths 2147483646;setPlayerData awards participant 2147483646;setPlayerData awards afk 2147483646;setPlayerData awards noawards 2147483646;setPlayerData awards bombsplanted 2147483646;setPlayerData awards bombsdefused 2147483646;setPlayerData awards bombcarrierkills 2147483646;setPlayerData awards bombscarried 2147483646;setPlayerData awards killsasbombcarrier 2147483646;setPlayerData awards flagscaptured 2147483646;setPlayerData awards flagsreturned 2147483646;setPlayerData awards flagcarrierkills 2147483646;setPlayerData awards flagscarried 2147483646;setPlayerData awards killsasflagcarrier 2147483646;setPlayerData awards hqsdestroyed 2147483646;setPlayerData awards hqscaptured 2147483646;setPlayerData awards pointscaptured 2147483646;setPlayerData awards targetsdestroyed 2147483646;setPlayerData awards 10kills 2147483646;setPlayerData awards 1death 2147483646;setPlayerData awards nodeaths 2147483646;setPlayerData awards nokills 2147483646;setPlayerData awards mvp 2147483646;setPlayerData awards highlander 2147483646;setPlayerData awards kdratio10 2147483646;setPlayerData awards punisher 2147483646;setPlayerData awards overkill 2147483646;setPlayerData awards killedotherteamonelife 2147483646;setPlayerData awards kdratio 2147483646;setPlayerData awards kills 2147483646;setPlayerData awards higherrankkills 2147483646;setPlayerData awards deaths 2147483646;setPlayerData awards killstreak 2147483646;setPlayerData awards headshots 2147483646;setPlayerData awards finalkill 2147483646;setPlayerData awards killedotherteam 2147483646;setPlayerData awards closertoenemies 2147483646;setPlayerData awards throwingknifekills 2147483646;setPlayerData awards grenadekills 2147483646;setPlayerData awards helicopters 2147483646;setPlayerData awards airstrikes 2147483646;setPlayerData awards uavs 2147483646;setPlayerData awards mostmultikills 2147483646;setPlayerData awards multikill 2147483646;setPlayerData awards knifekills 2147483646;setPlayerData awards flankkills 2147483646;setPlayerData awards bulletpenkills 2147483646;setPlayerData awards laststandkills 2147483646;setPlayerData awards laststanderkills 2147483646;setPlayerData awards assists 2147483646;setPlayerData awards c4kills 2147483646;setPlayerData awards claymorekills 2147483646;setPlayerData awards fragkills 2147483646;setPlayerData awards semtexkills 2147483646;setPlayerData awards explosionssurvived 2147483646;setPlayerData awards mosttacprevented 2147483646;setPlayerData awards avengekills 2147483646;setPlayerData awards rescues 2147483646;setPlayerData awards longshots 2147483646;setPlayerData awards adskills 2147483646;setPlayerData awards hipfirekills 2147483646;setPlayerData awards revengekills 2147483646;setPlayerData awards longestlife 2147483646;setPlayerData awards throwbacks 2147483646;setPlayerData awards thumperkills 2147483646;setPlayerData awards otherweaponkills 2147483646;setPlayerData awards killedsameplayer 2147483646;setPlayerData awards mostweaponsused 2147483646;setPlayerData awards distancetraveled 2147483646;setPlayerData awards mostreloads 2147483646;setPlayerData awards mostswaps 2147483646;setPlayerData awards flankdeaths 2147483646;setPlayerData awards noflankdeaths 2147483646;setPlayerData awards thermalkills 2147483646;setPlayerData awards mostcamperkills 2147483646;setPlayerData awards fbhits 2147483646;setPlayerData awards stunhits 2147483646;setPlayerData awards scopedkills 2147483646;setPlayerData awards arkills 2147483646;setPlayerData awards arheadshots 2147483646;setPlayerData awards lmgkills 2147483646;setPlayerData awards lmgheadshots 2147483646;setPlayerData awards sniperkills 2147483646;setPlayerData awards sniperheadshots 2147483646;setPlayerData awards shieldblocks 2147483646;setPlayerData awards shieldkills 2147483646;setPlayerData awards smgkills 2147483646;setPlayerData awards smgheadshots 2147483646;setPlayerData awards shotgunkills 2147483646;setPlayerData awards shotgunheadshots 2147483646;setPlayerData awards pistolkills 2147483646;setPlayerData awards pistolheadshots 2147483646;setPlayerData awards rocketkills 2147483646;setPlayerData awards equipmentkills 2147483646;setPlayerData awards mostclasseschanged 2147483646;setPlayerData awards lowerrankkills 2147483646;setPlayerData awards sprinttime 2147483646;setPlayerData awards crouchtime 2147483646;setPlayerData awards pronetime 2147483646;setPlayerData awards comebacks 2147483646;setPlayerData awards mostshotsfired 2147483646;setPlayerData awards timeinspot 2147483646;setPlayerData awards killcamtimewatched 2147483646;setPlayerData awards greatestavgalt 2147483646;setPlayerData awards leastavgalt 2147483646;setPlayerData awards killcamskipped 2147483646;setPlayerData awards killsteals 2147483646;setPlayerData awards deathstreak 2147483646;setPlayerData awards shortestlife 2147483646;setPlayerData awards suicides 2147483646;setPlayerData awards mostff 2147483646;setPlayerData awards shotgundeaths 2147483646;setPlayerData awards shielddeaths 2147483646;setPlayerData awards participant 2147483646;setPlayerData awards afk 2147483646;setPlayerData awards noawards 2147483646;setPlayerData awards bombsplanted 2147483646;setPlayerData awards bombsdefused 2147483646;setPlayerData awards bombcarrierkills 2147483646;setPlayerData awards bombscarried 2147483646;setPlayerData awards killsasbombcarrier 2147483646;setPlayerData awards flagscaptured 2147483646;setPlayerData awards flagsreturned 2147483646;setPlayerData awards flagcarrierkills 2147483646;setPlayerData awards flagscarried 2147483646;setPlayerData awards killsasflagcarrier 2147483646;setPlayerData awards hqsdestroyed 2147483646;setPlayerData awards hqscaptured 2147483646;setPlayerData awards pointscaptured 2147483646;setPlayerData awards targetsdestroyed 2147483646;uploadStats");
    }

    private void button39_Click(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_Reg("disconnect;wait 100;setPlayerData kills 345674;setPlayerData killStreak 48;setPlayerData headshots 2446;setPlayerData deaths 155665;setPlayerData assists 55643;setPlayerData hits 1664478;setPlayerData misses 7787886;setPlayerData wins 466;setPlayerData winStreak 20;setPlayerData losses 257;setPlayerData ties 10;setPlayerData score 76557744;uploadStats;setPlayerData awards 10kills 1000;setPlayerData awards 1death 1000;setPlayerData awards nodeaths 1000;setPlayerData awards nokills 1000;setPlayerData awards mvp 1000;setPlayerData awards highlander 1000;setPlayerData awards kdratio10 1000;setPlayerData awards punisher 1000;setPlayerData awards overkill 1000;setPlayerData awards killedotherteamonelife 1000;setPlayerData awards kdratio 1000;setPlayerData awards kills 1000;setPlayerData awards higherrankkills 1000;setPlayerData awards deaths 1000;setPlayerData awards killstreak 1000;setPlayerData awards headshots 1000;setPlayerData awards finalkill 1000;setPlayerData awards killedotherteam 1000;setPlayerData awards closertoenemies 1000;setPlayerData awards throwingknifekills 1000;setPlayerData awards grenadekills 1000;setPlayerData awards helicopters 1000;setPlayerData awards airstrikes 1000;setPlayerData awards uavs 1000;setPlayerData awards mostmultikills 1000;setPlayerData awards multikill 1000;setPlayerData awards knifekills 1000;setPlayerData awards flankkills 1000;setPlayerData awards bulletpenkills 1000;setPlayerData awards laststandkills 1000;setPlayerData awards laststanderkills 1000;setPlayerData awards assists 1000;setPlayerData awards c4kills 1000;setPlayerData awards claymorekills 1000;setPlayerData awards fragkills 1000;setPlayerData awards semtexkills 1000;setPlayerData awards explosionssurvived 1000;setPlayerData awards mosttacprevented 1000;setPlayerData awards avengekills 1000;setPlayerData awards rescues 1000;setPlayerData awards longshots 1000;setPlayerData awards adskills 1000;setPlayerData awards hipfirekills 1000;setPlayerData awards revengekills 1000;setPlayerData awards longestlife 1000;setPlayerData awards throwbacks 1000;setPlayerData awards thumperkills 1000;setPlayerData awards otherweaponkills 1000;setPlayerData awards killedsameplayer 1000;setPlayerData awards mostweaponsused 1000;setPlayerData awards distancetraveled 1000;setPlayerData awards mostreloads 1000;setPlayerData awards mostswaps 1000;setPlayerData awards flankdeaths 1000;setPlayerData awards noflankdeaths 1000;setPlayerData awards thermalkills 1000;setPlayerData awards mostcamperkills 1000;setPlayerData awards fbhits 1000;setPlayerData awards stunhits 1000;setPlayerData awards scopedkills 1000;setPlayerData awards arkills 1000;setPlayerData awards arheadshots 1000;setPlayerData awards lmgkills 1000;setPlayerData awards lmgheadshots 1000;setPlayerData awards sniperkills 1000;setPlayerData awards sniperheadshots 1000;setPlayerData awards shieldblocks 1000;setPlayerData awards shieldkills 1000;setPlayerData awards smgkills 1000;setPlayerData awards smgheadshots 1000;setPlayerData awards shotgunkills 1000;setPlayerData awards shotgunheadshots 1000;setPlayerData awards pistolkills 1000;setPlayerData awards pistolheadshots 1000;setPlayerData awards rocketkills 1000;setPlayerData awards equipmentkills 1000;setPlayerData awards mostclasseschanged 1000;setPlayerData awards lowerrankkills 1000;setPlayerData awards sprinttime 1000;setPlayerData awards crouchtime 1000;setPlayerData awards pronetime 1000;setPlayerData awards comebacks 1000;setPlayerData awards mostshotsfired 1000;setPlayerData awards timeinspot 1000;setPlayerData awards killcamtimewatched 1000;setPlayerData awards greatestavgalt 1000;setPlayerData awards leastavgalt 1000;setPlayerData awards killcamskipped 1000;setPlayerData awards killsteals 1000;setPlayerData awards deathstreak 1000;setPlayerData awards shortestlife 1000;setPlayerData awards suicides 1000;setPlayerData awards mostff 1000;setPlayerData awards shotgundeaths 1000;setPlayerData awards shielddeaths 1000;setPlayerData awards participant 1000;setPlayerData awards afk 1000;setPlayerData awards noawards 1000;setPlayerData awards bombsplanted 1000;setPlayerData awards bombsdefused 1000;setPlayerData awards bombcarrierkills 1000;setPlayerData awards bombscarried 1000;setPlayerData awards killsasbombcarrier 1000;setPlayerData awards flagscaptured 1000;setPlayerData awards flagsreturned 1000;setPlayerData awards flagcarrierkills 1000;setPlayerData awards flagscarried 1000;setPlayerData awards killsasflagcarrier 1000;setPlayerData awards hqsdestroyed 1000;setPlayerData awards hqscaptured 1000;setPlayerData awards pointscaptured 1000;setPlayerData awards targetsdestroyed 1000;setPlayerData awards 10kills 1000;setPlayerData awards 1death 1000;setPlayerData awards nodeaths 1000;setPlayerData awards nokills 1000;setPlayerData awards mvp 1000;setPlayerData awards highlander 1000;setPlayerData awards kdratio10 1000;setPlayerData awards punisher 1000;setPlayerData awards overkill 1000;setPlayerData awards killedotherteamonelife 1000;setPlayerData awards kdratio 1000;setPlayerData awards kills 1000;setPlayerData awards higherrankkills 1000;setPlayerData awards deaths 1000;setPlayerData awards killstreak 1000;setPlayerData awards headshots 1000;setPlayerData awards finalkill 1000;setPlayerData awards killedotherteam 1000;setPlayerData awards closertoenemies 1000;setPlayerData awards throwingknifekills 1000;setPlayerData awards grenadekills 1000;setPlayerData awards helicopters 1000;setPlayerData awards airstrikes 1000;setPlayerData awards uavs 1000;setPlayerData awards mostmultikills 1000;setPlayerData awards multikill 1000;setPlayerData awards knifekills 1000;setPlayerData awards flankkills 1000;setPlayerData awards bulletpenkills 1000;setPlayerData awards laststandkills 1000;setPlayerData awards laststanderkills 1000;setPlayerData awards assists 1000;setPlayerData awards c4kills 1000;setPlayerData awards claymorekills 1000;setPlayerData awards fragkills 1000;setPlayerData awards semtexkills 1000;setPlayerData awards explosionssurvived 1000;setPlayerData awards mosttacprevented 1000;setPlayerData awards avengekills 1000;setPlayerData awards rescues 1000;setPlayerData awards longshots 1000;setPlayerData awards adskills 1000;setPlayerData awards hipfirekills 1000;setPlayerData awards revengekills 1000;setPlayerData awards longestlife 1000;setPlayerData awards throwbacks 1000;setPlayerData awards thumperkills 1000;setPlayerData awards otherweaponkills 1000;setPlayerData awards killedsameplayer 1000;setPlayerData awards mostweaponsused 1000;setPlayerData awards distancetraveled 1000;setPlayerData awards mostreloads 1000;setPlayerData awards mostswaps 1000;setPlayerData awards flankdeaths 1000;setPlayerData awards noflankdeaths 1000;setPlayerData awards thermalkills 1000;setPlayerData awards mostcamperkills 1000;setPlayerData awards fbhits 1000;setPlayerData awards stunhits 1000;setPlayerData awards scopedkills 1000;setPlayerData awards arkills 1000;setPlayerData awards arheadshots 1000;setPlayerData awards lmgkills 1000;setPlayerData awards lmgheadshots 1000;setPlayerData awards sniperkills 1000;setPlayerData awards sniperheadshots 1000;setPlayerData awards shieldblocks 1000;setPlayerData awards shieldkills 1000;setPlayerData awards smgkills 1000;setPlayerData awards smgheadshots 1000;setPlayerData awards shotgunkills 1000;setPlayerData awards shotgunheadshots 1000;setPlayerData awards pistolkills 1000;setPlayerData awards pistolheadshots 1000;setPlayerData awards rocketkills 1000;setPlayerData awards equipmentkills 1000;setPlayerData awards mostclasseschanged 1000;setPlayerData awards lowerrankkills 1000;setPlayerData awards sprinttime 1000;setPlayerData awards crouchtime 1000;setPlayerData awards pronetime 1000;setPlayerData awards comebacks 1000;setPlayerData awards mostshotsfired 1000;setPlayerData awards timeinspot 1000;setPlayerData awards killcamtimewatched 1000;setPlayerData awards greatestavgalt 1000;setPlayerData awards leastavgalt 1000;setPlayerData awards killcamskipped 1000;setPlayerData awards killsteals 1000;setPlayerData awards deathstreak 1000;setPlayerData awards shortestlife 1000;setPlayerData awards suicides 1000;setPlayerData awards mostff 1000;setPlayerData awards shotgundeaths 1000;setPlayerData awards shielddeaths 1000;setPlayerData awards participant 1000;setPlayerData awards afk 1000;setPlayerData awards noawards 1000;setPlayerData awards bombsplanted 1000;setPlayerData awards bombsdefused 1000;setPlayerData awards bombcarrierkills 1000;setPlayerData awards bombscarried 1000;setPlayerData awards killsasbombcarrier 1000;setPlayerData awards flagscaptured 1000;setPlayerData awards flagsreturned 1000;setPlayerData awards flagcarrierkills 1000;setPlayerData awards flagscarried 1000;setPlayerData awards killsasflagcarrier 1000;setPlayerData awards hqsdestroyed 1000;setPlayerData awards hqscaptured 1000;setPlayerData awards pointscaptured 1000;setPlayerData awards targetsdestroyed 1000;uploadStats");
    }

    private void button37_Click(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_Reg("disconnect;wait 100;setPlayerData kills 0;setPlayerData killStreak 0;setPlayerData headshots 0;setPlayerData deaths 0;setPlayerData assists 0;setPlayerData hits 0;setPlayerData misses 0;setPlayerData wins 0;setPlayerData winStreak 0;setPlayerData losses 0;setPlayerData ties 0;setPlayerData score 0;uploadStats;setPlayerData awards 10kills 0;setPlayerData awards 1death 0;setPlayerData awards nodeaths 0;setPlayerData awards nokills 0;setPlayerData awards mvp 0;setPlayerData awards highlander 0;setPlayerData awards kdratio10 0;setPlayerData awards punisher 0;setPlayerData awards overkill 0;setPlayerData awards killedotherteamonelife 0;setPlayerData awards kdratio 0;setPlayerData awards kills 0;setPlayerData awards higherrankkills 0;setPlayerData awards deaths 0;setPlayerData awards killstreak 0;setPlayerData awards headshots 0;setPlayerData awards finalkill 0;setPlayerData awards killedotherteam 0;setPlayerData awards closertoenemies 0;setPlayerData awards throwingknifekills 0;setPlayerData awards grenadekills 0;setPlayerData awards helicopters 0;setPlayerData awards airstrikes 0;setPlayerData awards uavs 0;setPlayerData awards mostmultikills 0;setPlayerData awards multikill 0;setPlayerData awards knifekills 0;setPlayerData awards flankkills 0;setPlayerData awards bulletpenkills 0;setPlayerData awards laststandkills 0;setPlayerData awards laststanderkills 0;setPlayerData awards assists 0;setPlayerData awards c4kills 0;setPlayerData awards claymorekills 0;setPlayerData awards fragkills 0;setPlayerData awards semtexkills 0;setPlayerData awards explosionssurvived 0;setPlayerData awards mosttacprevented 0;setPlayerData awards avengekills 0;setPlayerData awards rescues 0;setPlayerData awards longshots 0;setPlayerData awards adskills 0;setPlayerData awards hipfirekills 0;setPlayerData awards revengekills 0;setPlayerData awards longestlife 0;setPlayerData awards throwbacks 0;setPlayerData awards thumperkills 0;setPlayerData awards otherweaponkills 0;setPlayerData awards killedsameplayer 0;setPlayerData awards mostweaponsused 0;setPlayerData awards distancetraveled 0;setPlayerData awards mostreloads 0;setPlayerData awards mostswaps 0;setPlayerData awards flankdeaths 0;setPlayerData awards noflankdeaths 0;setPlayerData awards thermalkills 0;setPlayerData awards mostcamperkills 0;setPlayerData awards fbhits 0;setPlayerData awards stunhits 0;setPlayerData awards scopedkills 0;setPlayerData awards arkills 0;setPlayerData awards arheadshots 0;setPlayerData awards lmgkills 0;setPlayerData awards lmgheadshots 0;setPlayerData awards sniperkills 0;setPlayerData awards sniperheadshots 0;setPlayerData awards shieldblocks 0;setPlayerData awards shieldkills 0;setPlayerData awards smgkills 0;setPlayerData awards smgheadshots 0;setPlayerData awards shotgunkills 0;setPlayerData awards shotgunheadshots 0;setPlayerData awards pistolkills 0;setPlayerData awards pistolheadshots 0;setPlayerData awards rocketkills 0;setPlayerData awards equipmentkills 0;setPlayerData awards mostclasseschanged 0;setPlayerData awards lowerrankkills 0;setPlayerData awards sprinttime 0;setPlayerData awards crouchtime 0;setPlayerData awards pronetime 0;setPlayerData awards comebacks 0;setPlayerData awards mostshotsfired 0;setPlayerData awards timeinspot 0;setPlayerData awards killcamtimewatched 0;setPlayerData awards greatestavgalt 0;setPlayerData awards leastavgalt 0;setPlayerData awards killcamskipped 0;setPlayerData awards killsteals 0;setPlayerData awards deathstreak 0;setPlayerData awards shortestlife 0;setPlayerData awards suicides 0;setPlayerData awards mostff 0;setPlayerData awards shotgundeaths 0;setPlayerData awards shielddeaths 0;setPlayerData awards participant 0;setPlayerData awards afk 0;setPlayerData awards noawards 0;setPlayerData awards bombsplanted 0;setPlayerData awards bombsdefused 0;setPlayerData awards bombcarrierkills 0;setPlayerData awards bombscarried 0;setPlayerData awards killsasbombcarrier 0;setPlayerData awards flagscaptured 0;setPlayerData awards flagsreturned 0;setPlayerData awards flagcarrierkills 0;setPlayerData awards flagscarried 0;setPlayerData awards killsasflagcarrier 0;setPlayerData awards hqsdestroyed 0;setPlayerData awards hqscaptured 0;setPlayerData awards pointscaptured 0;setPlayerData awards targetsdestroyed 0;setPlayerData awards 10kills 0;setPlayerData awards 1death 0;setPlayerData awards nodeaths 0;setPlayerData awards nokills 0;setPlayerData awards mvp 0;setPlayerData awards highlander 0;setPlayerData awards kdratio10 0;setPlayerData awards punisher 0;setPlayerData awards overkill 0;setPlayerData awards killedotherteamonelife 0;setPlayerData awards kdratio 0;setPlayerData awards kills 0;setPlayerData awards higherrankkills 0;setPlayerData awards deaths 0;setPlayerData awards killstreak 0;setPlayerData awards headshots 0;setPlayerData awards finalkill 0;setPlayerData awards killedotherteam 0;setPlayerData awards closertoenemies 0;setPlayerData awards throwingknifekills 0;setPlayerData awards grenadekills 0;setPlayerData awards helicopters 0;setPlayerData awards airstrikes 0;setPlayerData awards uavs 0;setPlayerData awards mostmultikills 0;setPlayerData awards multikill 0;setPlayerData awards knifekills 0;setPlayerData awards flankkills 0;setPlayerData awards bulletpenkills 0;setPlayerData awards laststandkills 0;setPlayerData awards laststanderkills 0;setPlayerData awards assists 0;setPlayerData awards c4kills 0;setPlayerData awards claymorekills 0;setPlayerData awards fragkills 0;setPlayerData awards semtexkills 0;setPlayerData awards explosionssurvived 0;setPlayerData awards mosttacprevented 0;setPlayerData awards avengekills 0;setPlayerData awards rescues 0;setPlayerData awards longshots 0;setPlayerData awards adskills 0;setPlayerData awards hipfirekills 0;setPlayerData awards revengekills 0;setPlayerData awards longestlife 0;setPlayerData awards throwbacks 0;setPlayerData awards thumperkills 0;setPlayerData awards otherweaponkills 0;setPlayerData awards killedsameplayer 0;setPlayerData awards mostweaponsused 0;setPlayerData awards distancetraveled 0;setPlayerData awards mostreloads 0;setPlayerData awards mostswaps 0;setPlayerData awards flankdeaths 0;setPlayerData awards noflankdeaths 0;setPlayerData awards thermalkills 0;setPlayerData awards mostcamperkills 0;setPlayerData awards fbhits 0;setPlayerData awards stunhits 0;setPlayerData awards scopedkills 0;setPlayerData awards arkills 0;setPlayerData awards arheadshots 0;setPlayerData awards lmgkills 0;setPlayerData awards lmgheadshots 0;setPlayerData awards sniperkills 0;setPlayerData awards sniperheadshots 0;setPlayerData awards shieldblocks 0;setPlayerData awards shieldkills 0;setPlayerData awards smgkills 0;setPlayerData awards smgheadshots 0;setPlayerData awards shotgunkills 0;setPlayerData awards shotgunheadshots 0;setPlayerData awards pistolkills 0;setPlayerData awards pistolheadshots 0;setPlayerData awards rocketkills 0;setPlayerData awards equipmentkills 0;setPlayerData awards mostclasseschanged 0;setPlayerData awards lowerrankkills 0;setPlayerData awards sprinttime 0;setPlayerData awards crouchtime 0;setPlayerData awards pronetime 0;setPlayerData awards comebacks 0;setPlayerData awards mostshotsfired 0;setPlayerData awards timeinspot 0;setPlayerData awards killcamtimewatched 0;setPlayerData awards greatestavgalt 0;setPlayerData awards leastavgalt 0;setPlayerData awards killcamskipped 0;setPlayerData awards killsteals 0;setPlayerData awards deathstreak 0;setPlayerData awards shortestlife 0;setPlayerData awards suicides 0;setPlayerData awards mostff 0;setPlayerData awards shotgundeaths 0;setPlayerData awards shielddeaths 0;setPlayerData awards participant 0;setPlayerData awards afk 0;setPlayerData awards noawards 0;setPlayerData awards bombsplanted 0;setPlayerData awards bombsdefused 0;setPlayerData awards bombcarrierkills 0;setPlayerData awards bombscarried 0;setPlayerData awards killsasbombcarrier 0;setPlayerData awards flagscaptured 0;setPlayerData awards flagsreturned 0;setPlayerData awards flagcarrierkills 0;setPlayerData awards flagscarried 0;setPlayerData awards killsasflagcarrier 0;setPlayerData awards hqsdestroyed 0;setPlayerData awards hqscaptured 0;setPlayerData awards pointscaptured 0;setPlayerData awards targetsdestroyed 0;uploadStats");
    }
    #endregion
    string[] statNames = new string[] { "Prestige", "Score", "XP", "Kills", "Deaths", "Assists", "Headshots", "Hits", "Misses", "Kill Streaks", "Win Streaks", "Wins", "Loses", "Ties" };

    private void button67_Click(object sender, EventArgs e)
    {
        DialogResult resetDft = MessageBox.Show("All pre-set classes, stats, and unlocks will be lost.\n\nAre you sure you want to reset?", "You are about to reset your MW2 account to default!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

        if (resetDft == DialogResult.Yes)
        {
            RPC.cBuff_AddText_Reg("defaultStatsInit");
        }
    }

    private void button33_Click(object sender, EventArgs e)
    {
        decimal num = ((numericUpDown11.Value * 43200M) + (numericUpDown12.Value * 1800M)) + (numericUpDown13.Value * 30M);
        byte[] bytes = BitConverter.GetBytes(Convert.ToInt32(num.ToString()));
        PS3.SetMemory(0x1ff9ac8, bytes);
        PS3.SetMemory(0x1ff9acc, bytes);
    }

    private void button34_Click(object sender, EventArgs e)
    {
        PS3.SetMemory(0x01ff9e2c + (uint)numericUpDown14.Value * 0x40, Encoding.ASCII.GetBytes(textBox5.Text + "\0"));
    }

    private void button35_Click(object sender, EventArgs e)
    {
        for (int i = 1; i < 11; i++)
        {
            PS3.SetMemory(0x01ff9e2c + (uint)i * 0x40, Encoding.ASCII.GetBytes(textBox5.Text + "\0"));
        }
    }
    string[] stats = new string[] { "prestige", "score", "experience", "kills", "deaths", "assists", "headshots", "hits", "misses", "killStreak", "winStreak", "wins", "losses", "ties" };
    private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        var senderGrid = (DataGridView)sender;

        if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.ColumnIndex == 2)
        {
            if (dataGridView1[1, e.RowIndex].Value != null)
            {
                string val = dataGridView1[1, e.RowIndex].Value.ToString();
                if (new Regex("^[0-9]*$").IsMatch(val) || val.StartsWith("-") && new Regex("^[0-9-]*$").IsMatch(val) && val.Length >= 1)
                {
                    if (val.Length > 10)
                        val = val.Remove(9, val.Length - 9);

                    if (Convert.ToInt64(val) > 214783646 && e.RowIndex != 0 && e.RowIndex != 2)
                        val = "214783646";
                    else if (Convert.ToInt64(val) > 214783646 && e.RowIndex == 0)
                        val = "11";
                    else if (Convert.ToInt64(val) > 2516000 && e.RowIndex == 2)
                        val = "2516000";
                    RPC.cBuff_AddText_Reg("setPlayerData " + stats[e.RowIndex] + " " + val + ";uploadStats");
                    dataGridView1[1, e.RowIndex].Value = val;
                }
                else
                {
                    MessageBox.Show("Contains illegal characters!", "Invalid", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
    }
    #endregion
    #region Clients
    #region client misc
    string notifyCmd = "kf";
    private void colorRadioButton73_CheckedChanged(object sender, EventArgs e)
    {
        if (colorRadioButton73.Checked)
            notifyCmd = "gn";
    }

    private void colorRadioButton72_CheckedChanged(object sender, EventArgs e)
    {
        if (colorRadioButton72.Checked)
            notifyCmd = "kf";
    }

    private void colorRadioButton71_CheckedChanged(object sender, EventArgs e)
    {
        if (colorRadioButton71.Checked)
            notifyCmd = "none";
    }
    bool listUpdate = false;
    private void colorCheckBox1_CheckedChanged(object sender, EventArgs e)
    {
        listUpdate = colorCheckBox1.Checked;
    }

    private void button41_Click(object sender, EventArgs e)
    {
        int index = dataGridView2.CurrentRow.Index;
        RPC.SV_GameSendServerCommand(index, "v cg_thirdperson 0");
        PS3.SetMemory(0x01319968 + (uint)index * 0x280, Models(0));
        callNotify(index, "Third Person Off");
    }

    private void numericUpDown17_ValueChanged(object sender, EventArgs e)
    {
        int value = (int)numericUpDown17.Value;
        RPC.SV_GameSendServerCommand(dataGridView2.CurrentRow.Index, "v cg_thirdperson 1");
        PS3.SetMemory(0x01319968 + (uint)dataGridView2.CurrentRow.Index * 0x280, Models(value));
    }

    byte[] Models(int num)
    {
        List<byte[]> model = new List<byte[]>();
        model.Add(new byte[] { 0x00, 0x4B });
        model.Add(new byte[] { 0x00, 0x3C });
        model.Add(new byte[] { 0x00, 0x3B });
        model.Add(new byte[] { 0x00, 0x55 });
        model.Add(new byte[] { 0x00, 0x50 });
        model.Add(new byte[] { 0x00, 0x43 });
        model.Add(new byte[] { 0x00, 0x4C });
        model.Add(new byte[] { 0x00, 0x34 });
        model.Add(new byte[] { 0x00, 0x4A });
        model.Add(new byte[] { 0x00, 0x4C });
        model.Add(new byte[] { 0x00, 0x02 });
        model.Add(new byte[] { 0x00, 0x03 });
        model.Add(new byte[] { 0x00, 0x04 });
        model.Add(new byte[] { 0x00, 0x05 });
        model.Add(new byte[] { 0x00, 0x07 });
        model.Add(new byte[] { 0x00, 0x08 });
        model.Add(new byte[] { 0x00, 0x09 });
        model.Add(new byte[] { 0x00, 0x10 });
        model.Add(new byte[] { 0x00, 0x11 });
        model.Add(new byte[] { 0x00, 0x12 });
        model.Add(new byte[] { 0x01, 0x13 });
        model.Add(new byte[] { 0x01, 0x14 });
        model.Add(new byte[] { 0x01, 0x15 });
        model.Add(new byte[] { 0x01, 0x58 });
        model.Add(new byte[] { 0x01, 0x66 });
        model.Add(new byte[] { 0x01, 0x6D });
        model.Add(new byte[] { 0x01, 0x67 });
        model.Add(new byte[] { 0x01, 0x68 });
        model.Add(new byte[] { 0x01, 0x6B });
        model.Add(new byte[] { 0x01, 0x6C });
        model.Add(new byte[] { 0x01, 0x6E });
        model.Add(new byte[] { 0x01, 0x6F });
        model.Add(new byte[] { 0x01, 0x70 });
        model.Add(new byte[] { 0x01, 0x71 });
        model.Add(new byte[] { 0x01, 0x73 });
        model.Add(new byte[] { 0x01, 0x74 });
        model.Add(new byte[] { 0x01, 0x75 });
        model.Add(new byte[] { 0x01, 0x76 });
        model.Add(new byte[] { 0x01, 0x7A });
        model.Add(new byte[] { 0x01, 0x7B });
        model.Add(new byte[] { 0x01, 0x7C });
        model.Add(new byte[] { 0x01, 0x7D });
        model.Add(new byte[] { 0x01, 0x7E });
        model.Add(new byte[] { 0x01, 0x7F });
        model.Add(new byte[] { 0x01, 0x80 });
        model.Add(new byte[] { 0x01, 0x81 });
        model.Add(new byte[] { 0x01, 0x82 });
        model.Add(new byte[] { 0x01, 0x83 });
        model.Add(new byte[] { 0x01, 0x84 });
        model.Add(new byte[] { 0x00, 0x00 });

        return model[num];
    }

    private void button44_Click(object sender, EventArgs e)
    {
        int index = dataGridView2.CurrentRow.Index;
        if (PS3.Extension.ReadByte(clientActiveOfs + (uint)index * 0x3700) == 0x02)
            RPC.SV_GameSendServerCommand(-1, "h \"^8" + PS3.Extension.ReadString(0x14e5490 + 0x3700 * (uint)index) + "^7: " + textBox6.Text + "\"" + "\0");
        else
            MessageBox.Show("Select an active client from the list", "NOTICE", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    }


    #endregion
    #region Client list

    private void setClientMod(bool canNotify, string txt, uint ofs, float[] value, int client)
    {
        if (activeClient(client))
        {
            PS3.Extension.WriteFloats(ofs + (uint)client * 0x3700, value);
            if (txt != "" && canNotify)
                callNotify(client, txt);
        }
    }

    private void setClientMod(bool canNotify, string txt, uint ofs, byte[] value, int client)
    {
        if (activeClient(client))
        {
            PS3.SetMemory(ofs + (uint)client * 0x3700, value);
            if (txt != "" && canNotify)
                callNotify(client, txt);
        }
    }
    private void setClientMod(bool canNotify, string txt, uint ofs, uint interval, byte[] value, int client)
    {
        if (activeClient(client))
        {
            PS3.SetMemory(ofs + (uint)client * interval, value);
            if (txt != "" && canNotify)
                callNotify(client, txt);
        }
    }
    private void setClientMod(bool canNotify, string txt, string value, int client)
    {
        if (activeClient(client))
        {
            RPC.SV_GameSendServerCommand(client, "v " + value);
            if (txt != "" && canNotify)
                callNotify(client, txt);
        }
    }
    private void callNotify(int client, string txt)
    {
        RPC.SV_GameSendServerCommand(client, "v loc_warnings 0");
        if (txt.EndsWith("Off"))
            txt = txt.Insert(txt.Length - 3, "^1");
        else if (txt.EndsWith("On"))
            txt = txt.Insert(txt.Length - 2, "^2");
        else if (txt.EndsWith("Set"))
            txt = txt.Insert(txt.Length - 3, "^3");

        if (notifyCmd == "gn")
            RPC.iPrintlnBold(client, txt);
        else if (notifyCmd == "kf")
            RPC.iPrintln(client, txt);
    }
    private void button45_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < 18; i++)
            dataGridView2[1, i].Value = RPC.getName(i);
    }

    private void onToolStripMenuItem_Click(object sender, EventArgs e)
    {
        setClientMod(true, "God mode On", 0x14e5429, new byte[] { 0x0A, 0x0A, 0x0A, 0x0A }, dataGridView2.CurrentRow.Index);
    }

    private void offToolStripMenuItem_Click(object sender, EventArgs e)
    {
        setClientMod(true, "God mode Off", 0x14e5429, new byte[] { 0x00, 0x00, 0x64, 0x00 }, dataGridView2.CurrentRow.Index);
    }

    private void toolStripMenuItem2_Click(object sender, EventArgs e)
    {
        int index = dataGridView2.CurrentRow.Index;
        byte[] val = new byte[] { 0x15, 0xff, 0xff, 0xff };
        PS3.SetMemory(0x14e24ec + (uint)index * 0x3700, val);
        PS3.SetMemory(0x14e24dc + (uint)index * 0x3700, val);
        PS3.SetMemory(0x14e2554 + (uint)index * 0x3700, val);
        PS3.SetMemory(0x14e256c + (uint)index * 0x3700, val);
        PS3.SetMemory(0x14e2560 + (uint)index * 0x3700, val);
        PS3.SetMemory(0x14e2578 + (uint)index * 0x3700, val);
        callNotify(index, "Infinite Ammo On");
    }

    private void toolStripMenuItem3_Click(object sender, EventArgs e)
    {
        int index = dataGridView2.CurrentRow.Index;
        PS3.SetMemory(0x14e24ec + (uint)index * 0x3700, new byte[] { 0x00, 0x00, 0x00, 0x40 });
        PS3.SetMemory(0x14e24dc + (uint)index * 0x3700, new byte[] { 0x00, 0x00, 0x00, 0x40 });
        PS3.SetMemory(0x14e2554 + (uint)index * 0x3700, new byte[] { 0x00, 0x00, 0x00, 0x0A });
        PS3.SetMemory(0x14e256c + (uint)index * 0x3700, new byte[] { 0x00, 0x00, 0x00, 0x20 });
        PS3.SetMemory(0x14e2560 + (uint)index * 0x3700, new byte[] { 0x00, 0x00, 0x00, 0x01 });
        PS3.SetMemory(0x14e2578 + (uint)index * 0x3700, new byte[] { 0x00, 0x00, 0x00, 0x02 });
        callNotify(index, "Infinite Ammo Off");
    }

    private void toolStripMenuItem71_Click(object sender, EventArgs e)
    {
        setClientMod(true, "Redboxes On", 0x14e2213, new byte[] { 0x55 }, dataGridView2.CurrentRow.Index);
    }

    private void toolStripMenuItem72_Click(object sender, EventArgs e)
    {
        setClientMod(true, "Redboxes Off", 0x14e2213, new byte[] { 0x00 }, dataGridView2.CurrentRow.Index);
    }

    private void toolStripMenuItem68_Click(object sender, EventArgs e)
    {
        setClientMod(true, "Redboxes + Thermal On", 0x14e2213, new byte[] { 0x99 }, dataGridView2.CurrentRow.Index);
    }

    private void toolStripMenuItem69_Click(object sender, EventArgs e)
    {
        setClientMod(true, "Redboxes + Thermal Off", 0x14e2213, new byte[] { 0x00 }, dataGridView2.CurrentRow.Index);
    }

    private void toolStripMenuItem65_Click(object sender, EventArgs e)
    {
        setClientMod(true, "No Recoil On", 0x014e24be, new byte[] { 0x04 }, dataGridView2.CurrentRow.Index);
    }

    private void toolStripMenuItem66_Click(object sender, EventArgs e)
    {
        setClientMod(true, "No Recoil Off", 0x14e1f42, new byte[] { 0x00 }, dataGridView2.CurrentRow.Index);
    }

    private void toolStripMenuItem62_Click(object sender, EventArgs e)
    {
        setClientMod(true, "UFO Mode On", 0x14e5623, new byte[] { 0x02 }, dataGridView2.CurrentRow.Index);
    }

    private void toolStripMenuItem63_Click(object sender, EventArgs e)
    {
        setClientMod(true, "UFO Mode Off", 0x14e5623, new byte[] { 0x00 }, dataGridView2.CurrentRow.Index);
    }

    private void toolStripMenuItem59_Click(object sender, EventArgs e)
    {
        setClientMod(true, "No Clip On", 0x14e5623, new byte[] { 0x01 }, dataGridView2.CurrentRow.Index);
    }

    private void toolStripMenuItem60_Click(object sender, EventArgs e)
    {
        setClientMod(true, "No Clip Off", 0x14e5623, new byte[] { 0x00 }, dataGridView2.CurrentRow.Index);
    }
    private void setPerk(bool notifyx, int client, string notify, string perkName)
    {
        RPC.cBuff_AddText_Reg("setPerk " + RPC.getName(client) + " " + perkName);
        if (notifyx)
            callNotify(client, notify);
    }
    private void x10000DamageToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        setPerk(true, dataGridView2.CurrentRow.Index, "Explosive Bullets x10000 Set", "specialty_explosivebullets;set bg_bulletExplDmgFactor 100;set bg_bulletExplRadius 10000");
    }

    private void x10000DamageToolStripMenuItem_Click(object sender, EventArgs e)
    {
        setPerk(true, dataGridView2.CurrentRow.Index, "Explosive Bullets x9000 Set", "specialty_explosivebullets;set bg_bulletExplDmgFactor 90;set bg_bulletExplRadius 9000");
    }

    private void x2000DamageToolStripMenuItem6_Click(object sender, EventArgs e)
    {
        setPerk(true, dataGridView2.CurrentRow.Index, "Explosive Bullets x8000 Set", "specialty_explosivebullets;set bg_bulletExplDmgFactor 80;set bg_bulletExplRadius 8000");
    }

    private void x2000DamageToolStripMenuItem5_Click(object sender, EventArgs e)
    {
        setPerk(true, dataGridView2.CurrentRow.Index, "Explosive Bullets x7000 Set", "specialty_explosivebullets;set bg_bulletExplDmgFactor 70;set bg_bulletExplRadius 7000");
    }

    private void x2000DamageToolStripMenuItem4_Click(object sender, EventArgs e)
    {
        setPerk(true, dataGridView2.CurrentRow.Index, "Explosive Bullets x6000 Set", "specialty_explosivebullets;set bg_bulletExplDmgFactor 60;set bg_bulletExplRadius 6000");
    }

    private void x2000DamageToolStripMenuItem3_Click(object sender, EventArgs e)
    {
        setPerk(true, dataGridView2.CurrentRow.Index, "Explosive Bullets x5000 Set", "specialty_explosivebullets;set bg_bulletExplDmgFactor 50;set bg_bulletExplRadius 5000");
    }

    private void x2000DamageToolStripMenuItem2_Click(object sender, EventArgs e)
    {
        setPerk(true, dataGridView2.CurrentRow.Index, "Explosive Bullets x4000 Set", "specialty_explosivebullets;set bg_bulletExplDmgFactor 40;set bg_bulletExplRadius 4000");
    }

    private void x2000DamageToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        setPerk(true, dataGridView2.CurrentRow.Index, "Explosive Bullets x3000 Set", "specialty_explosivebullets;set bg_bulletExplDmgFactor 30;set bg_bulletExplRadius 3000");
    }

    private void toolStripMenuItem57_Click(object sender, EventArgs e)
    {
        setPerk(true, dataGridView2.CurrentRow.Index, "Explosive Bullets x2000 Set", "specialty_explosivebullets;set bg_bulletExplDmgFactor 20;set bg_bulletExplRadius 2000");
    }

    private void toolStripMenuItem56_Click(object sender, EventArgs e)
    {
        setPerk(true, dataGridView2.CurrentRow.Index, "Explosive Bullets x1000 Set", "specialty_explosivebullets;set bg_bulletExplDmgFactor 10;set bg_bulletExplRadius 1000");
    }

    private void x100DamageToolStripMenuItem_Click(object sender, EventArgs e)
    {
        setPerk(true, dataGridView2.CurrentRow.Index, "Explosive Bullets x100 Set", "specialty_explosivebullets;set bg_bulletExplDmgFactor 10;set bg_bulletExplRadius 100");
    }

    private void oNToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        setClientMod(true, "Primary Akimbo On", 0x14e2467, new byte[] { 1 }, dataGridView2.CurrentRow.Index);
    }

    private void oFFToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        setClientMod(true, "Primary Akimbo OFf", 0x14e2467, new byte[] { 0 }, dataGridView2.CurrentRow.Index);
    }

    private void oNToolStripMenuItem2_Click(object sender, EventArgs e)
    {
        setClientMod(true, "Secondary Akimbo On", 0x14e245d, new byte[] { 1 }, dataGridView2.CurrentRow.Index);
    }

    private void oFFToolStripMenuItem2_Click(object sender, EventArgs e)
    {
        setClientMod(true, "Secondary Akimbo Off", 0x14e245d, new byte[] { 0 }, dataGridView2.CurrentRow.Index);
    }

    private void onToolStripMenuItem4_Click(object sender, EventArgs e)
    {
        setClientMod(true, "Lag Strike On", "com_maxfps 10", dataGridView2.CurrentRow.Index);
    }

    private void offToolStripMenuItem4_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < 10; i++)
            setClientMod(true, "Lag Strike Off", "com_maxfps 60", dataGridView2.CurrentRow.Index);
    }

    private void toolStripMenuItem47_Click(object sender, EventArgs e)
    {
        setClientMod(true, "Freeze Movement On", 0x14e5623, new byte[] { 4 }, dataGridView2.CurrentRow.Index);
    }

    private void toolStripMenuItem48_Click(object sender, EventArgs e)
    {
        setClientMod(true, "Freeze Movement Off", 0x14e5623, new byte[] { 0 }, dataGridView2.CurrentRow.Index);
    }

    private void onToolStripMenuItem5_Click(object sender, EventArgs e)
    {
        setClientMod(true, "Set All Perks On", 0x14e2628, new byte[] { 0xFf, 0xFF }, dataGridView2.CurrentRow.Index);
    }

    private void oFFToolStripMenuItem5_Click(object sender, EventArgs e)
    {
        setClientMod(true, "Set All Perks Off", 0x14e2628, new byte[] { 0, 0 }, dataGridView2.CurrentRow.Index);
    }

    private void onToolStripMenuItem3_Click(object sender, EventArgs e)
    {
        int index = dataGridView2.CurrentRow.Index;
        PS3.SetMemory(0x1319901 + (uint)index * 0x280, new byte[] { 1 });
        callNotify(index, "Kill Player Cycle On");
    }

    private void offToolStripMenuItem3_Click(object sender, EventArgs e)
    {
        int index = dataGridView2.CurrentRow.Index;
        PS3.SetMemory(0x1319901 + (uint)index * 0x280, new byte[] { 0 });
        callNotify(index, "Kill Player Cycle Off");
    }
    byte[] saveGunByte = new byte[18];
    private void onToolStripMenuItem7_Click(object sender, EventArgs e)
    {
        int index = dataGridView2.CurrentRow.Index;
        RPC.SV_GameSendServerCommand(index, "v cg_gun_y -4");
        if (saveGunByte[index] != 0xF1) { saveGunByte[index] = PS3.Extension.ReadByte(0x014E5443 + (uint)index * 0x3700); }
        setClientMod(true, "Modded Gun Model On", 0x014E5443, new byte[] { 0xF1 }, index);
    }

    private void offToolStripMenuItem7_Click(object sender, EventArgs e)
    {
        int index = dataGridView2.CurrentRow.Index;
        RPC.SV_GameSendServerCommand(index, "v cg_gun_y 0");
        if (saveGunByte[index] != 0xF1) { saveGunByte[index] = PS3.Extension.ReadByte(0x014E5443 + (uint)index * 0x3700); }
        setClientMod(true, "Modded Gun Model Off", 0x014E5443, new byte[] { saveGunByte[index] }, index);
    }

    private void kickPlayerToolStripMenuItem_Click(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_Reg("clientKick " + dataGridView2.CurrentRow.Index);
        callNotify(dataGridView2.CurrentRow.Index, "Player ^5kicked");
    }

    private void kickAllToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_Reg("kick all");
        callNotify(dataGridView2.CurrentRow.Index, "All players ^5kicked");
    }

    private void unlockAllToolStripMenuItem_Click(object sender, EventArgs e)
    {
        doUnlock(dataGridView2.CurrentRow.Index);
    }

    private void derankToolStripMenuItem_Click(object sender, EventArgs e)
    {
        doDerank(dataGridView2.CurrentRow.Index);
    }
    private void doUnlock(int num)
    {
        callNotify(num, "Unlocking All Challenges...^2Please Wait^7...");
        RPC.SV_GameSendServerCommand(num, "o mp_bonus_start");
        RPC.SV_GameSendServerCommand(num, "N 2056 206426 6525 7F 3760 09 4623 E803 3761 09 4627 F430 3762 02 4631 14 3763 02 4635 3C 3764 02 4639 0F 3765 02 4643 14 3766 02 4647 28 3767 02 4651 0A 3752 09 4591 E803 3753 09 4595 0F40 3754 02 4599 14 3755 02 4603 3C 3756 02 4607 0F 3757 02 4611 14 3758 02 4615 28 3759 02 4619 0A 3736 09 4527 E803");
        RPC.SV_GameSendServerCommand(num, "N 3737 09 4531 0F40 3738 02 4535 14 3739 02 4539 3C 3740 02 4543 0F 3741 02 4547 14 3742 02 4551 28 3743 02 4555 0A 3799 09 4779 E803 3800 09 4783 0F40 3801 02 4787 14 3802 02 4791 3C 3803 02 4795 0F 3804 02 4799 14 3805 02 4803 28 3806 02 4807 0A");
        RPC.SV_GameSendServerCommand(num, "N 3775 09 4683 E803 3776 09 4687 0F40 3777 02 4691 14 3778 02 4695 3C 3779 02 4699 0F 3780 02 4703 14 3781 02 4707 28 3782 02 4711 0A 3728 09 4495 E803 3729 09 4499 0F40 3730 02 4503 14 3731 02 4507 3C 3732 02 4511 0F 3733 02 4515 14 3734 02 4519 28 3735 02 4523 0A 3783 09 4715 E803 3784 09 4719 0F40 3785 02 4723 14 3786 02 4727 3C");
        RPC.SV_GameSendServerCommand(num, "N 3787 02 4731 0F 3788 02 4735 14 3789 02 4739 28 3790 02 4743 0A 3791 09 4747 E803 3864 02 5039 14 3865 02 5043 28 3866 02 5047 09 3888 09 5135 E803 3887 09 5131 0F40");
        RPC.SV_GameSendServerCommand(num, "N 3792 09 4751 0F40 3793 02 4755 14 3794 02 4759 3C 3795 02 4763 0F 3796 02 4767 14 3797 02 4771 28 3798 02 4775 0A 3744 09 4559 E803 3745 09 4563 0F40 3746 02 4567 14 3889 02 5139 0F 3890 02 5143 3C 3891 02 5147 14 3892 02 5151 28 3893 02 5155 09 3807 09 4811 E803 3808 09 4815 0F40 3809 02 4819 0F 3810 02 4823 14 3811 02 4827 28");
        RPC.SV_GameSendServerCommand(num, "N 3747 02 4571 3C 3748 02 4575 0F 3749 02 4579 14 3750 02 4583 28 3751 02 4587 0A 3853 09 4995 E803 3854 09 4999 0F40 3855 02 5003 1E 3856 02 5007 3C 3857 02 5011 14 3858 02 5015 28 3859 02 5019 09 3839 09 4939 E803 3840 09 4943 0F40 3841 02 4947 1E 3842 02 4951 3C 3843 02 4955 14 3844 02 4959 28 3845 02 4963 09 3825 09 4883 E803");
        RPC.SV_GameSendServerCommand(num, "N 3826 09 4887 0F40 3827 02 4891 1E 3828 02 4895 3C 3829 02 4899 14 3830 02 4903 28 3831 02 4907 09 3832 09 4911 E803 3833 09 4915 0F40 3834 02 4919 1E 3835 02 4923 3C 3836 02 4927 14 3837 02 4931 28 3838 02 4935 09 3846 09 4967 E803 3847 09 4971 0F40");
        RPC.SV_GameSendServerCommand(num, "N 3848 02 4975 1E 3849 02 4979 3C 3850 02 4983 14 3851 02 4987 28 3852 02 4991 09 3768 09 4655 E803 3769 09 4659 0F40 3771 02 4667 0F 3770 02 4663 3C 3772 02 4671 14 3773 02 4675 28 3774 02 4679 09 3874 09 5079 E803 3875 09 5083 0F40 3876 02 5087 0F");
        RPC.SV_GameSendServerCommand(num, "N 3877 02 5091 3C 3878 02 5095 14 3879 02 5099 28 3880 02 5103 09 3867 09 5051 E803 3868 09 5055 0F40 3869 02 5059 0F 3870 02 5063 3C 3871 02 5067 14 3872 02 5071 28 3873 02 5075 09 3860 09 5023 E803 3861 09 5027 0F40 3862 02 5031 0F 3863 02 5035 3C");
        RPC.SV_GameSendServerCommand(num, "N 3812 02 4831 06 3813 09 4835 E803 3814 09 4839 0F40 3815 02 4843 0F 3816 02 4847 14 3817 02 4851 28 3818 02 4855 06 3819 09 4859 E803 3820 09 4863 0F40 3821 02 4867 0F 3822 02 4871 14 3823 02 4875 28 3824 02 4879 06 3881 09 5107 E803 3882 09 5111 0F40");
        RPC.SV_GameSendServerCommand(num, "N 3883 02 5115 0F 3884 02 5119 14 3885 02 5123 28 3886 02 5127 06 3898 09 5175 E803 3899 09 5179 0F40 3894 09 5159 E803 3895 09 5163 0F40 3900 09 5183 E803 3901 09 5187 0F40 3896 09 5167 E803 3897 09 5171 0F40 3902 09 5191 E803 3903 09 5195 0F40 3908 09 5215 E803");
        Thread.Sleep(250);
        RPC.SV_GameSendServerCommand(num, "N 3909 09 5219 0F40 3904 09 5199 E803 3905 09 5203 0F40 3906 09 5207 E803 3907 09 5211 0F40 3912 06 5231 C409 3913 09 5235 0F40 3910 06 5223 C409 3911 09 5227 0F40 3916 09 5247 E803 3917 09 5251 0F40 3914 09 5239 E803 3915 09 5243 0F40 3920 07 5263 C409 3921 09 5267 0F40");
        RPC.SV_GameSendServerCommand(num, "N 3918 07 5255 C409 3919 09 5259 0F40 3922 09 5271 B004 3923 09 5275 B004 3924 09 5279 B004 3925 09 5283 B004 3926 09 5287 FA 3643 0A 4155 09 3927 07 5292 6108 3931 07 5307 EE02 3938 07 5335 0F40 3932 07 5311 8403 3935 07 5323 EE02 3933 07 5315 E803 3941 07 5347 402414");
        RPC.SV_GameSendServerCommand(num, "N 3934 07 5319 FA 3936 07 5327 FA 3942 07 5351 0F40 3939 07 5339 64 3928 07 5295 0F40 3930 07 5303 FA 3929 07 5299 FA 3940 07 5343 EE02 3937 07 5331 64 3943 04 5355 32 3944 04 5359 32 3945 04 5363 32 3946 04 5367 32 3947 04 5371 32 3948 04 5375 32");
        RPC.SV_GameSendServerCommand(num, "N 3949 04 5379 32 3950 04 5383 32 3951 04 5387 19 3952 04 5391 19 3953 04 5395 19 3954 04 5399 19 3955 04 5403 19 3956 04 5407 0A 3957 04 5411 0A 3958 04 5415 E803 3959 04 5419 E803 3960 04 5423 E803 3961 04 5427 E803 3962 04 5431 32 3963 04 5435 1E 3964 04 5439 32 3965 04 5443 1E 3966 04 5447 32 3967 04 5451 1E 3968 04 5455 1E");
        RPC.SV_GameSendServerCommand(num, "N 3969 02 5459 FF 3972 02 5471 FF 3973 02 5475 FF 3983 02 5515 FF 3984 02 5519 FF 3985 02 5523 FF 3986 02 5527 FF 3987 02 5531 FF 3988 02 5535 FF 4100 02 5983 FF 3970 02 5463 19 3971 02 5467 19 4020 04 5663 1E 4021 04 5667 1E 4022 04 5671 1E 4023 04 5675 0F 4024 04 5679 0F 4025 04 5683 0F");
        RPC.SV_GameSendServerCommand(num, "N 3989 02 5539 FF 3990 02 5543 FF 3991 02 5547 FF 3992 02 5551 FF 3994 02 5559 FF 3995 02 5563 FF 3996 02 5567 FF 3997 02 5571 FF 4001 02 5587 FF 4002 02 5591 FF 4028 04 5695 50C3 4029 04 5699 50C3 4030 04 5703 64 4035 04 5723 32 4036 04 5727 32 4037 04 5731 32 4038 04 5735 32 4039 04 5739 32 4040 04 5743 32");
        RPC.SV_GameSendServerCommand(num, "N 4003 02 5595 FF 4004 02 5599 FF 4005 02 5603 FF 4006 02 5607 FF 4007 02 5611 FF 4008 02 5615 FF 4009 02 5619 FF 4010 02 5623 FF 4011 02 5627 FF 4012 02 5631 FF 4101 04 5987 C8 4103 04 5995 0A 4104 04 5999 1E 4105 04 6003 1E 3993 04 5555 14 3998 04 5575 C8 3999 03 5579 0A 4000 03 5583 0A 4107 04 6011 0F");
        RPC.SV_GameSendServerCommand(num, "N 4013 02 5635 FF 4014 02 5639 FF 4015 02 5643 FF 4016 02 5647 FF 4017 02 5651 FF 4018 02 5655 FF 4114 02 6039 FF 4110 02 6023 FF 4106 02 6007 FF 4019 02 5659 FF 4041 04 5747 32 4050 03 5783 19 4051 03 5787 19 4055 03 5803 19 4056 03 5807 19 4065 04 5843 14 4068 04 5855 14 4069 04 5859 14 4058 03 5815 19");
        RPC.SV_GameSendServerCommand(num, "N 4026 02 5687 FF 4027 02 5691 FF 4042 02 5751 FF 4031 02 5707 FF 4032 02 5711 FF 4033 02 5715 FF 4034 02 5719 FF 4043 02 5755 FF 4044 02 5759 FF 4045 02 5763 FF 4108 04 6015 32 4109 02 6019 0A 4111 03 6027 0A 4112 03 6031 0A 4113 03 6035 0A 4115 03 6043 0A 4116 05 6047 FA 4117 05 6051 64 4118 05 6055 E803");
        RPC.SV_GameSendServerCommand(num, "N 4046 02 5767 FF 4047 02 5771 FF 4048 02 5775 FF 4049 02 5779 FF 4052 02 5791 FF 4053 02 5795 FF 4054 02 5799 FF 4102 02 5991 FF 4121 02 6067 FF 4057 02 5811 FF 4119 05 6059 2C00 4120 05 6063 2C00 6525 7F");
        Thread.Sleep(250);
        callNotify(num, "Unlocking All Challenges...^2Please Wait^7...");
        RPC.SV_GameSendServerCommand(num, "N 4059 02 5819 OO 4060 02 5823 OO 4061 02 5827 OO 4062 02 5831 OO 4063 02 5835 OO 4064 02 5839 OO 4066 02 5847 OO 4067 02 5851 OO 4070 02 5863 OO 4071 02 5867 OO 4072 02 5871 OO 4073 02 5875 OO 4074 02 5879 OO 4075 02 5883 OO 4076 02 5887 OO 4077 02 5891 OO 4078 02 5895 OO 4079 02 5899 OO 4080 02 5903 OO 4081 02 5907 OO");
        RPC.SV_GameSendServerCommand(num, "N 4082 02 5911 OO 4083 02 5915 OO 4084 02 5919 OO 4085 02 5923 OO 4086 02 5927 OO 4087 02 5931 OO 4088 02 5935 OO 4089 02 5939 OO 4090 02 5943 OO 4091 02 5947 OO 4092 02 5951 OO 4093 02 5955 OO 4094 02 5959 OO 4095 02 5963 OO 4096 02 5967 OO 4097 02 5971 OO 4098 02 5975 OO 4099 02 5979 OO 4100 02 5983 OO 4099 02 5979 OO");
        RPC.SV_GameSendServerCommand(num, "N 3038 05 6695 80 6696 10 6697 02 6697 42 6696 11 6696 31 6697 46 6697 C6 6696 33 6696 73 6697 CE 6698 09 6696 7B 6697 CF 6697 EF 6698 0D 6696 7F 6696 FF 6697 FF 6698 0F 6637 84 6637 8C 6503 03 6637 9C 6637 BC 6503 07 6637 FC 6638 FF 6503 0F 6638 03 6638 07");
        RPC.SV_GameSendServerCommand(num, "N 6503 1F 6638 0F 6638 1F 6638 3F 6503 3F 6638 7F 6638 FF 6503 7F 6639 FF 6639 03 6639 07 6503 FF 6639 0F 6639 1F 6504 FF 6639 3F 6639 7F 6639 FF 6504 03 6640 09 6640 0B 6504 07 6640 0F 6640 1F 6504 0F 6640 3F 6640 7F 6504 1F 6640 FF 6641 23 6504 3F 6641 27");
        RPC.SV_GameSendServerCommand(num, "N 3038 05 3550 05 3614 05 3486 05 3422 05 3358 05 3294 05 3230 05 3166 05 3102 05 3038 05 2072 2D302E302F30O 2092 30303130 2128 3130 2136 3B05ZZ3C05 2152 3D05O");
        RPC.SV_GameSendServerCommand(num, "N 6641 2F 6504 7F 6641 3F 6641 7F 6504 FF 6641 FF 6642 85 6505 FF 6642 87 6642 8F 6505 03 6642 9F 6642 BF 6505 07 6642 FF 6643 11 6505 0F 6643 13 6643 17 6505 1F 6643 1F 6643 3F 6505 3F 6643 7F 6643 FF 6505 7F 6644 43 6644 47 6505 FF 6644 4F 6644 5F 6506 FF");
        RPC.SV_GameSendServerCommand(num, "N 6644 7F 6644 FF 6506 03 6645 09 6645 0B 6506 07 6645 0F 6645 1F 6506 0F 6645 3F 6645 7F 6506 1F 6645 FF 6646 23 6506 3F 6646 27 6646 2F 6506 7F 6646 3F 6646 7F 6506 FF 6646 FF 6647 85 6507 FF 6647 87 6647 8F 6507 03 6647 9F 6647 BF 6507 07 6647 FF 6648 11");
        RPC.SV_GameSendServerCommand(num, "N 6507 0F 6648 13 6648 17 6507 1F 6648 1F 6648 3F 6507 3F 6648 7F 6648 FF 6507 7F 6649 FF 6649 03 6649 07 6507 FF 6649 0F 6649 1F 6508 FF 6649 3F 6649 7F 6649 FF 6508 03 6650 FF 6650 03 6508 07 6650 07 6650 0F 6650 1F 6508 0F 6650 3F 6650 7F 6508 1F 6650 FF");
        RPC.SV_GameSendServerCommand(num, "N 6651 FF 6651 03 6508 3F 6651 07 6651 0F 6508 7F 6651 1F 6651 3F 6508 FF 6651 7F 6651 FF 6509 FF 6652 FF 6652 03 6509 03 6652 07 6652 0F 6509 07 6652 1F 6652 3F 6509 0F 6652 7F 6652 FF 6509 1F 6653 FF 6653 03 6509 3F 6653 07 6653 0F 6509 7F 6653 1F 6653 3F");
        RPC.SV_GameSendServerCommand(num, "N 6509 FF 6653 7F 6653 FF 6510 FF 6654 FF 6654 03 6510 03 6654 07 6654 0F 6510 07 6654 1F 6654 3F 6510 0F 6654 7F 6654 FF 6510 1F 6655 FF 6655 03 6510 3F 6655 07 6655 0F 6510 7F 6655 1F 6655 3F 6510 FF 6655 7F 6655 FF 6511 FF 6656 FF 6656 03 6511 03 6656 07");
        RPC.SV_GameSendServerCommand(num, "N 6656 0F 6511 07 6656 1F 6656 3F 6511 0F 6656 7F 6656 FF 6511 1F 6657 FF 6657 03 6511 3F 6657 07 6657 0F 6511 7F 6657 1F 6657 3F 6511 FF 6657 7F 6657 FF 6512 FF 6658 FF 6658 03 6512 03 6658 07 6658 0F 6512 07 6658 1F 6658 9F 6658 BF 6658 FF 6680 FF 6661 5B");
        RPC.SV_GameSendServerCommand(num, "N 6661 5F 6661 7F 6661 FF 6673 08 6673 18 6673 38 6673 78 6673 F8 6674 FF 6674 03 6674 07 6674 0F 6674 1F 6674 3F 6674 7F 6674 FF 6679 08 6673 F9 6673 FB 6673 FF 6675 FF 6677 FF 6677 03 6677 07 6677 0F 6677 1F 6677 3F 6677 7F 6677 FF 6679 09 6679 0B 6679 0F");
        Thread.Sleep(250);
        RPC.SV_GameSendServerCommand(num, "N 6679 1F 6679 3F 6679 7F 6679 FF 6680 03 6680 07 6680 0F 6680 1F 6680 3F 6680 BF 6681 FF 6681 03 6681 0B 6681 1B 6681 3B 6681 7B 6681 FB 6681 FF 6680 FF 6686 FF 6632 FF 6632 03 6632 07 6632 0F 6632 1F 6632 3F 6632 7F 6632 FF 6633 FF 6633 03 6633 07 6633 0F");
        RPC.SV_GameSendServerCommand(num, "N 6633 1F 6633 3F 6633 7F 6633 FF 6634 FF 6634 03 6634 07 6634 0F 6634 1F 6634 3F 6634 7F 6634 FF 6635 FF 6635 03 6635 07 6635 0F 6635 1F 6635 3F 6635 7F 6635 FF 6636 FF 6636 03 6636 07 6636 0F 6636 1F 6636 3F 6636 7F 6636 FF 6637 FD 6637 FF 6690 FF 6690 03");
        RPC.SV_GameSendServerCommand(num, "N 6690 07 6690 0F 6690 1F 6690 3F 6690 7F 6690 FF 6695 81 6695 83 6695 87 6695 8F 6695 9F 6695 BF 6698 1F 6698 3F 6698 7F 6698 FF 6699 C1 6699 C3 6699 C7 6699 CF 6699 DF 6699 FF 6700 1F 6700 3F 6700 7F 6700 FF 6701 03 6701 07 6701 0F 6701 1F 6701 3F 6701 7F");
        RPC.SV_GameSendServerCommand(num, "N 6701 FF 6702 FF 6702 03 6702 07 6524 10 6524 30 6524 70 6524 F0 6529 FF 6529 03 6529 07 6530 08 6529 0F 6529 1F 6529 3F 6529 7F 6529 FF 6530 09 6530 0B 6530 0F 6530 1F 6530 7F 6530 FF 6531 FF 6531 03 6531 07 6531 0F 6531 1F 6531 3F 6531 7F 6531 FF 6532 FF");
        RPC.SV_GameSendServerCommand(num, "N 6532 03 6532 07 6532 0F 6512 C7 6526 02 6512 D7 6526 06 6512 F7 6526 86 6532 1F 6532 3F 6532 BF 6533 F9 6533 FB 6533 FF 6532 FF 6526 87 6526 A7 6512 FF 6540 7F 6526 E7 6526 EF 6526 FF 6517 FF 6527 FF 6528 FF 6522 FF 6524 F1 6524 F3 6524 F7 6524 FF");
        RPC.SV_GameSendServerCommand(num, "N 3850 99 3851 99 3852 99 3853 99 3854 99 3855 99 3856 99 3857 99 3858 99 3859 99 3860 99 3861 99 3862 99 3863 99 3864 99 3865 99 3866 99 3867 99 3868 99 3869 99 3870 99 3871 99 3872 99 3873 99 3874 99 3875 99 3876 99 3877 99 3878 99 3879 99 3880 99 3881 99 3882 99 3883 99 3884 99 3885 99 3886 99 3887 99 3888 99 3889 99 3890 99 3891 99 3892 99 3893 99 3894 99 3895 99 3896 99 3897 99 3898 99 3899 99 3900 99");
        RPC.SV_GameSendServerCommand(num, "N 3900 99 3901 99 3902 99 3903 99 3904 99 3905 99 3906 99 3907 99 3908 99 3909 99 3910 99 3911 99 3912 99 3913 99 3914 99 3915 99 3916 99 3917 99 3918 99 3919 99 3920 99 3921 99 3922 99 3923 99 3924 99 3925 99 3926 99 3927 99 3928 99 3929 99 3930 99 3931 99 3932 99 3933 99 3934 99 3935 99 3936 99 3937 99 3938 99 3939 99 3940 99 3941 99 3942 99 3943 99 3944 99 3945 99 3946 99 3947 99 3948 99 3949 99 3950 99");
        RPC.SV_GameSendServerCommand(num, "N 3950 99 3951 99 3952 99 3953 99 3954 99 3955 99 3956 99 3957 99 3958 99 3959 99 3960 99 3961 99 3962 99 3963 99 3964 99 3965 99 3966 99 3967 99 3968 99 3969 99 3970 99 3971 99 3972 99 3973 99 3974 99 3975 99 3976 99 3977 99 3978 99 3979 99 3980 99 3981 99 3982 99 3983 99 3984 99 3985 99 3986 99 3987 99 3988 99 3989 99 3990 99 3991 99 3992 99 3993 99 3994 99 3995 99 3996 99 3997 99 3998 99 3999 99 4000 99");
        RPC.SV_GameSendServerCommand(num, "N 4000 99 4001 99 4002 99 4003 99 4004 99 4005 99 4006 99 4007 99 4008 99 4009 99 4010 99 4011 99 4012 99 4013 99 4014 99 4015 99 4016 99 4017 99 4018 99 4019 99 4020 99 4021 99 4022 99 4023 99 4024 99 4025 99 4026 99 4027 99 4028 99 4029 99 4030 99 4031 99 4032 99 4033 99 4034 99 4035 99 4036 99 4037 99 4038 99 4039 99 4040 99 4041 99 4042 99 4043 99 4044 99 4045 99 4046 99 4047 99 4048 99 4049 99 4050 99");
        RPC.SV_GameSendServerCommand(num, "N 4050 99 4051 99 4052 99 4053 99 4054 99 4055 99 4056 99 4057 99 4058 99 4059 99 4060 99 4061 99 4062 99 4063 99 4064 99 4065 99 4066 99 4067 99 4068 99 4069 99 4070 99 4071 99 4072 99 4073 99 4074 99 4075 99 4076 99 4077 99 4078 99 4079 99 4080 99 4081 99 4082 99 4083 99 4084 99 4085 99 4086 99 4087 99 4088 99 4089 99 4090 99 4091 99 4092 99 4093 99 4094 99 4095 99 4096 99 4097 99 4098 99 4099 99 4100 99");
        RPC.SV_GameSendServerCommand(num, "p ");
        RPC.SV_GameSendServerCommand(num, "o mp_level_up");
        callNotify(num, "Unlock all ^2Complete");
    }
    private void doDerank(int client)
    {
        RPC.SV_GameSendServerCommand(client, "v activeaction \"xblive_privatematch 0;onlinegame 1;resetStats;defaultStatsInit\"");
        if (client != -1)
            RPC.cBuff_AddText_Reg("clientKick " + client);
        else
            RPC.cBuff_AddText_Reg("kick all");
        callNotify(client, "Player kicked and ^5deranked");
    }

    private void freezePS3ToolStripMenuItem_Click(object sender, EventArgs e)
    {
        RPC.SV_GameSendServerCommand(dataGridView2.CurrentRow.Index, "v r_fullbright 1");
        callNotify(dataGridView2.CurrentRow.Index, "Player's PS3 ^5Froze");
    }

    #endregion

    #region Spawn Models
    int curGridIndex = 0;
    bool useBuild = false;
    bool setModel = false;
    uint entOfs = 0;
    int entIndex = 130;
    float[] entPos = { 0, 0, 0 };
    float[] entAngle = { 0, 0 };
    int _horizSpin = 0;
    int _vertSpin = 0;
    List<uint> entList = new List<uint>();
    List<int> indexList = new List<int>();
    List<float[]> posList = new List<float[]>();
    List<float[]> angleList = new List<float[]>();

    bool activeClient(int client)
    {
        if (PS3.Extension.ReadByte(0x014e53af + 0x3700 * (uint)client) == 0x02)
            return true;
        else
            return false;
    }

    bool activeGame(int client)
    {
        if (PS3.Extension.ReadByte(0x01319807 + 0x280 * (uint)client) != 0x00)
            return true;
        else
            return false;
    }

    int navEnts(string dir)
    {
        if (dir == "right")
        {
            return entIndex += 1;
        }
        else
        {
            if (entIndex > 0)
                return entIndex -= 1;
            else
                return entIndex;
        }
    }

    int horizSpin(string dir)
    {
        if (dir == "right")
        {
            if (_horizSpin == 0)
                _horizSpin = 360;
            return _horizSpin -= 10;

        }
        else
        {
            if (_horizSpin >= 360)
                _horizSpin = 0;
            return _horizSpin += 10;

        }
    }

    int vertSpin(string dir)
    {
        if (dir == "up")
        {
            if (_vertSpin == 0)
                _vertSpin = 360;
            return _vertSpin -= 10;

        }
        else
        {
            if (_vertSpin >= 360)
                _vertSpin = 0;
            return _vertSpin += 10;

        }
    }
    int L2Int = 100;
    int R2Int = 100;
    int entRadius = 180;
    public static List<bool> togs = new List<bool>() { false };

    bool runOnce(bool[] tog, int index, bool reset)
    {
        if (reset)
        {
            togs[index] = true;
            return true;
        }
        else if (tog[index])
        {
            togs[index] = false;
            return true;
        }
        else
            return false;
    }

    private PointF DegreesToXY(float degreesH, float degreesV, float radius, Point origin)
    {
        PointF xy = new PointF();
        double radians = degreesH * Math.PI / 180.0;

        xy.X = (float)Math.Cos(radians) * radius + origin.X;
        xy.Y = (float)Math.Sin(-radians) * radius + origin.Y;

        return xy;
    }
    double[] outPutXYZ = new double[4];
    private void buildMode()
    {
        if (useBuild)
        {
            if (entOfs == 0)
            {
                entOfs = SpawnEntity("com_plasticcase_friendly", -1, new float[] { 100000, 100000, -100000 }, new float[] { 0, 0, 0 }, false);
                entIndex = PS3.Extension.ReadInt32((uint)entOfs + 0x5C);
            }
            if (setModel)
            {
                setModel = false;
                entAngle = PS3.Extension.ReadFloats(entOfs + 0x48, 2);
                uint setEnt = SpawnEntity("com_plasticcase_friendly", entIndex, entPos, entAngle, true);
                entList.Add(setEnt);
                indexList.Add(entIndex);
                angleList.Add(entAngle);
                posList.Add(entPos);
            }

            float[] xyz = PS3.Extension.ReadFloats(0x014e221c + 0x3700 * (uint)curGridIndex, 3);
            float[] view = getView(curGridIndex);
            PointF cP = DegreesToXY(view[1], entRadius, new Point((int)xyz[1], (int)xyz[0]));
            entPos = new float[] { cP.Y, cP.X, xyz[2] - view[0] * 3 + 60 };
            PS3.Extension.WriteFloats(entOfs + 0x24, entPos);

            if (RPC.ButtonPressed(curGridIndex, RPC.Buttons.Square))
            {
                setModel = true;
                callNotify(curGridIndex, "Model ^2Set");
            }
            if (RPC.ButtonPressed(curGridIndex, RPC.Buttons.L3))
            {
                float curDisX = entPos[0];
                float curDisY = entPos[1];
                float curDisZ = entPos[2];
                for (int i = 0; i < posList.Count; i++)
                {
                    float entDisX = posList[i][0];
                    float entDisY = posList[i][1];
                    float entDisZ = posList[i][2];
                    int minNum = 10;
                    if (minValue(curDisX, entDisX) < minNum && minValue(curDisY, entDisY) < minNum && minValue(curDisZ, entDisZ) < minNum)
                    {
                        RPC.Call(SV_UnlinkEntity, entList[i]);
                        entList.RemoveAt(i);
                        indexList.RemoveAt(i);
                        angleList.RemoveAt(i);
                        posList.RemoveAt(i);
                        callNotify(curGridIndex, "Model ^1Deleted");
                    }
                }
            }
            if (RPC.ButtonPressed(curGridIndex, RPC.Buttons.R3))
            {
                if (entRadius > 360)
                    entRadius = 170;
                entRadius += 10;
                Task.Delay(100).Wait();
            }
            if (RPC.ButtonPressed(curGridIndex, RPC.Buttons.R2))
            {
                PS3.Extension.WriteInt32(entOfs + 0x5C, navEnts("right"));
                if (R2Int < 5)
                    R2Int = 5;
                Task.Delay(R2Int -= 5).Wait();
            }
            else
                R2Int = 100;
            if (RPC.ButtonPressed(curGridIndex, RPC.Buttons.L2))
            {
                PS3.Extension.WriteInt32(entOfs + 0x5C, navEnts("left"));
                if (L2Int < 5)
                    L2Int = 5;
                Task.Delay(L2Int -= 5).Wait();
            }
            else
                L2Int = 100;

            if (RPC.ButtonPressed(curGridIndex, RPC.Buttons.DpadLeft))
            {
                PS3.Extension.WriteFloat(entOfs + 0x4C, horizSpin("left"));
                Task.Delay(100).Wait();
            }
            if (RPC.ButtonPressed(curGridIndex, RPC.Buttons.DpadRight))
            {
                PS3.Extension.WriteFloat(entOfs + 0x4C, horizSpin("right"));
                Task.Delay(100).Wait();
            }
            if (RPC.ButtonPressed(curGridIndex, RPC.Buttons.DpadUp))
            {
                PS3.Extension.WriteFloat(entOfs + 0x48, vertSpin("up"));
                Task.Delay(100).Wait();
            }
            if (RPC.ButtonPressed(curGridIndex, RPC.Buttons.DpadDown))
            {
                PS3.Extension.WriteFloat(entOfs + 0x48, vertSpin("down"));
                Task.Delay(100).Wait();
            }
        }
    }

    private PointF DegreesToXY(float degrees, float radius, Point origin)
    {
        PointF xy = new PointF();
        double radians = degrees * Math.PI / 180.0;

        xy.X = (float)Math.Cos(radians) * radius + origin.X;
        xy.Y = (float)Math.Sin(-radians) * radius + origin.Y;

        return xy;
    }

    float minValue(float num1, float num2)
    {
        float sum = num1 - num2;
        if (sum < 0)
            sum = num2 - num1;
        return sum;
    }

    float[] Cpoint_(double radius, Point center, int horiz)
    {
        double slice = 2 * Math.PI / -360;
        double angle = slice * horiz - 11;
        return new float[] { (int)(center.X + radius * Math.Cos(angle)), (int)(center.Y + radius * Math.Sin(angle)) };
    }

    private void colorCheckBox13_CheckedChanged(object sender, EventArgs e)
    {
        if (colorCheckBox13.Checked)
        {
            setClientMod(true, "", 0x14e5623, new byte[] { 0x02 }, dataGridView2.CurrentRow.Index);
            setClientMod(true, "", "cg_drawgun 0", dataGridView2.CurrentRow.Index);
            setClientMod(true, "", "cg_drawcrosshair 0", dataGridView2.CurrentRow.Index);
            callNotify(dataGridView2.CurrentRow.Index, "[{+usereload}] Set | [{+smoke}][{+frag}] Models | [{+breath_sprint}] Delete | [{+melee}] Model Radius");
            callNotify(dataGridView2.CurrentRow.Index, "[{+actionslot 1}] [{+actionslot 2}]Flip Vertical | Flip Horizontal [{+actionslot 3}] [{+actionslot 4}]");
        }
        else
        {
            useBuild = colorCheckBox13.Checked;
            Task.Delay(500).Wait();
            PS3.Extension.WriteFloats(entOfs + 0x24, new float[] { 100000, 100000, -100000 });
            setClientMod(true, "Build Mode Off", 0x14e5623, new byte[] { 0x00 }, dataGridView2.CurrentRow.Index);
            setClientMod(true, "", "cg_drawgun 1", dataGridView2.CurrentRow.Index);
            setClientMod(true, "", "cg_drawcrosshair 1", dataGridView2.CurrentRow.Index);
        }
        useBuild = colorCheckBox13.Checked;
    }

    float[] getView(int client)
    {
        float[] view = PS3.Extension.ReadFloats(0x014e230c + 0x3700 * (uint)client, 2);
        view[1] -= 90;
        if (view[1] <= 0)
            view[1] = view[1] - -360;
        return view;
    }
    #region spawn Entity

    private uint
   SP_script_brushmodel = 0x001B52F0,
SV_UnlinkEntity = 0x002271C8,
SV_LinkEntity = 0x002285A0,
SV_SetBrushModel = 0x00219F08;

    private int G_Spawn()
    {
        return RPC.Call(0x001BCD10);
    }
    private uint SpawnEntity(string ModelName, int index, float[] Origin, float[] Angles, bool Solid = true)
    {
        uint Ent = (uint)G_Spawn();
        PS3.Extension.WriteFloats((uint)Ent + 0x138, new float[] { Origin[0], Origin[1], Origin[2] });
        PS3.Extension.WriteFloats((uint)Ent + 0x144, new float[] { Angles[0], Angles[1] });
        RPC.Call(0x001BE3F0, Ent, ModelName);

        RPC.Call(0x1B52A0, Ent);
        RPC.Call(SP_script_brushmodel, Ent);
        if (Solid)
            MakeSolid(Ent);
        if (index != -1)
            PS3.Extension.WriteInt32((uint)Ent + 0x5C, index);
        return Ent;
    }

    private void MakeSolid(uint Entity)
    {
        RPC.Call(SV_UnlinkEntity, Entity);
        PS3.Extension.WriteByte((uint)Entity + 0x101, 4);
        uint Brush = 0;
        string Mapname = getMap("");
        switch (Mapname)
        {
            case "Afghan":
                Brush = 0x1394A00;
                break;
            case "Highrise":
                Brush = 0x138C580;
                break;
            case "Rundown":
                Brush = 0x137BA00;
                break;
            case "Quarry":
                Brush = 0x13AF800;
                break;
            case "Skidrow":
                Brush = 0x1350A80;
                break;
            case "Terminal":
                Brush = 0x1323F80;
                break;
            case "Wasteland":
                Brush = 0x132B780;
                break;
            case "Derail":
                Brush = 0x1375880;
                break;
            case "Estate":
                Brush = 0x1361100;
                break;
            case "Favela":
                Brush = 0x1386E00;
                break;
            case "Invasion":
                Brush = 0x1389380;
                break;
            case "Rust":
                Brush = 0x1352B00;
                break;
            case "Scrapyard":
                Brush = 0x1321500;
                break;
            case "Subbase":
                Brush = 0x136F980;
                break;
            case "Underpass":
                Brush = 0x137CE00;
                break;
            case "Karachi":
                Brush = 0x1383480;
                break;
        }
        PS3.Extension.WriteUInt32((uint)Entity + 0x8B, PS3.Extension.ReadUInt32(Brush + 0x8B));
        RPC.Call(SV_SetBrushModel, Entity);
        PS3.Extension.WriteInt32((uint)Entity + 0x11C, PS3.Extension.ReadInt32((uint)Brush + 0x11C));
        RPC.Call(SV_LinkEntity, Entity);
    }
    public bool inGame()
    {
        return PS3.Extension.ReadBool(0x01D17A8C);
    }

    public string getMap(string dftName)
    {
        String str = PS3.Extension.ReadString(0xD495F4), MapStr = "Not in game";
        if (inGame())
        {
            if (dftName == "")
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
                    MapStr = "Subbase";
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
            else
                return str;
        }
        return MapStr;
    }

    #endregion

    string arry2Str(float[] arry)
    {
        string str = "";
        for (int i = 0; i < arry.Length; i++)
        {
            str += Math.Round(arry[i]) + Convert.ToString((i != arry.Length - 1) ? "." : "");
        }
        return str;
    }

    private void button76_Click(object sender, EventArgs e)
    {
        if (inGame())
        {
            if (posList.Count != 0)
            {
                SaveFileDialog file = new SaveFileDialog();
                file.Filter = "Entity files (*.ent)|*.ent";
                file.FileName = "" + getMap("") + " - Generated Models";

                if (file.ShowDialog() == DialogResult.OK)
                {
                    List<string> saveModels = new List<string>();
                    saveModels.Add(getMap("dft"));
                    for (int i = 0; i < posList.Count; i++)
                        saveModels.Add(arry2Str(posList[i]) + ":" + arry2Str(angleList[i]) + ":" + indexList[i]);
                    File.WriteAllLines(file.FileName, saveModels.ToArray());
                    MessageBox.Show("Models saved at: " + file.FileName, "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
                MessageBox.Show("No Entities Found", "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        else
            MessageBox.Show("You need to be in-game to save models.", "Not In-game", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    }
    private void button42_Click(object sender, EventArgs e)
    {
        if (inGame())
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Entity files (*.ent)|*.ent";
            if (file.ShowDialog() == DialogResult.OK)
            {
                string[] readInfo = File.ReadAllLines(file.FileName);
                string getName = readInfo[0];
                if (getName.StartsWith("mp_"))
                {
                    button42.Enabled = false;
                    button42.Update();
                    if (getMap("dft") != getName)
                    {
                        RPC.cBuff_AddText_RPC("map " + getName);
                        Task.Delay(3000).Wait();
                        while (!activeGame(RPC.getHostNum())) { }
                        Task.Delay(3000).Wait();
                    }
                    int cl = RPC.getHostNum();
                    int loadNum = 0;
                    for (int i = 0; i < readInfo.Length; i++)
                    {
                        try
                        {
                            string[] splitInfo = Regex.Split((readInfo[i] != "" && !readInfo[i].StartsWith("mp_")) ? readInfo[i] : "", ":");
                            if (splitInfo[0] != "" && splitInfo[1] != "" && splitInfo[2] != "")
                            {
                                float[] pos = Array.ConvertAll(splitInfo[0].Split('.'), float.Parse);
                                float[] agl = Array.ConvertAll(splitInfo[1].Split('.'), float.Parse);
                                SpawnEntity("com_plasticcase_friendly", Convert.ToInt32(splitInfo[2]), pos, agl, true);
                                if (i == loadNum || i == 1)
                                {
                                    loadNum += 10;
                                    callNotify(cl, "^2Loading ^7" + i + "/" + readInfo.Length + " Models");
                                }
                                Task.Delay(10).Wait();
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Error has occured when loading entities.\nPossible Syntax error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    callNotify(cl, "Load ^2Complete");
                    button42.Enabled = true;
                    MessageBox.Show("Models loaded.", "Load Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Could not read map name.", "Invalid Syntax", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        else
            MessageBox.Show("You need to be in-game to load models.", "Not In-game", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    }
    #endregion
    #region Client Aimbot
    #region set view
    private void setViewV(int client, int angle)
    {
        byte[] view = BitConverter.GetBytes((float)angle);
        Array.Reverse(view);
        PS3.Extension.WriteBytes(setViewPosOfs + (uint)client * clientInervalOfs, view);
    }
    private void setViewH(int client, int angle)
    {
        byte[] view = BitConverter.GetBytes((float)angle);
        Array.Reverse(view);
        PS3.Extension.WriteBytes(setViewPosOfs + 4 + (uint)client * clientInervalOfs, view);
    }
    #endregion

    #region defaults
    uint clientInervalOfs = 0x3700;
    uint clientPosOfs = 0x014e221c;
    uint setViewPosOfs = 0x014e2260;
    uint getViewPosOfs = 0x014e5648;
    uint clientTeamOfs = 0x014e5453;
    uint clientActiveOfs = 0x014e53af;
    uint clientAliveOfs = 0x014e5383;
    uint stanceOfs = 0x014e2327;
    bool[] aimbotVerified = new bool[18];
    float[] sumH = new float[18];
    float[] sumV = new float[18];
    int[] faTog = new int[18];
    int[] target = new int[18];
    #endregion

    #region outPuts
    float[] setViewPos(float[] you, float[] foe, int aimPos, int stanceClient, int stanceTarget)
    {
        foe[2] += aimPos + stanceTarget;
        you[2] += stanceClient;
        float getlookhfoe = XYToDegrees(new Point((int)foe[1], (int)foe[0]), new Point((int)you[1], (int)you[0])) - 270;
        float getlookvfoe = XYZToDegrees((int)you[0] - (int)foe[0], (int)you[1] - foe[1], (int)you[2] - (int)foe[2]);
        if (getlookhfoe <= 0) getlookhfoe = getlookhfoe - -360;
        return new float[] { getlookhfoe, getlookvfoe };
    }

    float[] getViewPos(int client)
    {
        float[] viewPos = PS3.Extension.ReadFloats(getViewPosOfs + (uint)client * clientInervalOfs, 2);
        if (viewPos[1] < 0)
            viewPos[1] += 360;

        return new float[] { (float)Math.Round(viewPos[1]), (float)Math.Round(viewPos[0]) };
    }

    int XYToDegrees(Point xy, Point origin)
    {
        int deltaX = origin.X - xy.X;
        int deltaY = origin.Y - xy.Y;

        double radAngle = Math.Atan2(deltaY, deltaX);
        double degreeAngle = radAngle * 180.0 / Math.PI;

        return (int)(180 - degreeAngle);
    }
    int XYZToDegrees(double deltaX, double deltaY, double deltaZ)
    {
        double deltaXY = Math.Sqrt(deltaY * deltaY + deltaX * deltaX);
        double radAngle = Math.Atan2(deltaZ, deltaXY);
        double degreeAngle = radAngle * 180 / Math.PI;

        return (int)degreeAngle;
    }
    #endregion

    #region fair aimbot
    private void fairAimbot(int client, int targetNum)
    {
        int plusStanceH = 0;
        int plusStanceF = 0;
        byte getStanceH = PS3.Extension.ReadByte(stanceOfs + (uint)client * clientInervalOfs);
        byte getStanceF = PS3.Extension.ReadByte(stanceOfs + (uint)targetNum * clientInervalOfs);

        if (getStanceH == 0x28)
            plusStanceH = -28;
        else if (getStanceH == 0x0B)
            plusStanceH = -55;

        if (getStanceF == 0x28)
            plusStanceF = -28;
        else if (getStanceF == 0x0B)
            plusStanceF = -55;

        float[] setPos = setViewPos(PS3.Extension.ReadFloats(clientPosOfs + (uint)client * clientInervalOfs), PS3.Extension.ReadFloats(clientPosOfs + (uint)targetNum * clientInervalOfs), -15, plusStanceH, plusStanceF);
        float[] getPos = getViewPos(client);

        sumH[client] += setPos[0] - getPos[0];
        sumV[client] += setPos[1] - getPos[1];

        if (sumH[client] != 0)
            setViewH(client, (int)sumH[client]);

        if (sumV[client] != 0)
            setViewV(client, (int)sumV[client]);

        if (PS3.Extension.ReadByte(clientAliveOfs + (uint)targetNum * clientInervalOfs) != 0x00)
            target[targetNum] = -1;
    }

    #endregion

    #region closest
    int closestClient(int client)
    {
        List<float[]> points = new List<float[]>();
        List<float[]> getInt = new List<float[]>();
        List<int> saveNum = new List<int>();
        int targetNum = -1;

        float[] point = PS3.Extension.ReadFloats(clientPosOfs + (uint)client * clientInervalOfs);
        byte clientTeam = PS3.Extension.ReadByte(clientTeamOfs + (uint)client * clientInervalOfs);

        for (int i = 0; i < 18; i++)
        {
            byte targetTeam = PS3.Extension.ReadByte(clientTeamOfs + (uint)i * clientInervalOfs);
            byte targetActive = PS3.Extension.ReadByte(clientActiveOfs + (uint)i * clientInervalOfs);
            byte targetAlive = PS3.Extension.ReadByte(clientAliveOfs + (uint)i * clientInervalOfs);

            if (targetActive == 0x02 && targetAlive == 0x00 && targetTeam != 0x03 && client != i)
            {
                float[] addPos = PS3.Extension.ReadFloats(clientPosOfs + (uint)i * clientInervalOfs);
                saveNum.Add(i);
                getInt.Add(addPos);
                points.Add(addPos);
            }
        }

        float[] closest = points.OrderBy(x => Math.Sqrt(Math.Pow(x[0] - point[0], 2) + Math.Pow(x[1] - point[1], 2) + Math.Pow(x[2] - point[2], 2))).FirstOrDefault();
        for (int i = 0; i < getInt.Count; i++)
            if (getInt[i] == closest)
            {
                targetNum = saveNum[i];
                break;
            }
        return targetNum;
    }

    #endregion

    #region process

    private void clientAimbot()
    {
        for (int i = 0; i < 18; i++)
        {
            if (aimbotVerified[i])
            {
                if (RPC.L1Press(i))
                {
                    if (target[i] == -1)
                        target[i] = closestClient(i);
                    else
                        fairAimbot(i, target[i]);
                }
                else
                {
                    target[i] = -1;
                }
            }
        }
    }

    private void onToolStripMenuItem8_Click(object sender, EventArgs e)
    {
        aimbotVerified[dataGridView2.CurrentRow.Index] = true;
    }

    private void offToolStripMenuItem8_Click(object sender, EventArgs e)
    {
        aimbotVerified[dataGridView2.CurrentRow.Index] = false;
    }
    #endregion

    #endregion
    #region client worker
    private void clientMods_DoWork(object sender, DoWorkEventArgs e)
    {
        PS3api();
        for (; ; )
        {
            try
            {
                if (listUpdate)
                    for (int i = 0; i < 18; i++)
                        dataGridView2[1, i].Value = RPC.getName(i);

                if (autoUpdateWeapList)
                    for (int i = 0; i < 18; i++)
                        dataGridView3.Rows[i].Cells[1].Value = RPC.getName(i);
            }
            catch
            {

            }
            if (canSpawn)
                runSpawn();

            runAutoGive();
            buildMode();
            clientAimbot();
            xrunForgeMode();
            zoneColorProcess();
            getInfo(updateIps);
        }
    }
    #endregion
    #region Quick Mods
    bool quickBool = true;
    private void colorRadioButton29_CheckedChanged(object sender, EventArgs e)
    {
        quickBool = colorRadioButton29.Checked;
    }

    private void runAutoGive()
    {
        if (autoGive)
            quickGive("on", autoGiveIndex, quickBool, false);
    }

    bool autoGive = false;
    int autoGiveIndex = 0;
    private void colorCheckBox8_CheckedChanged(object sender, EventArgs e)
    {
        autoGive = colorCheckBox8.Checked;
    }

    private void colorComboBox7_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (autoGive)
        {
            autoGive = false;
            Task.Delay(1000).Wait();
            for (int i = 0; i < 9; i++)
                quickGive("off", i, quickBool, false);
            autoGive = true;
        }
        autoGiveIndex = colorComboBox7.SelectedIndex;
    }

    private void button43_Click(object sender, EventArgs e)
    {
        int num;
        if (colorRadioButton29.Checked)
            num = hostNum;
        else
            num = -1;
        doUnlock(num);
    }

    private void button47_Click(object sender, EventArgs e)
    {
        int num;
        if (colorRadioButton29.Checked)
            num = hostNum;
        else
            num = -1;
        doDerank(num);
    }

    private void button79_Click(object sender, EventArgs e)
    {
        quickGive("on", colorComboBox7.SelectedIndex, quickBool, true);
    }

    private void button80_Click(object sender, EventArgs e)
    {
        quickGive("off", colorComboBox7.SelectedIndex, quickBool, true);
    }

    private void quickGive(string switchTxt, int index, bool lenCheck, bool notify)
    {
        int clientLen;
        if (lenCheck)
            clientLen = 1;
        else
            clientLen = 18;
        for (int i = 0; i < clientLen; i++)
        {
            if (clientLen == 1)
                i = hostNum;
            if (PS3.Extension.ReadByte(clientActiveOfs + (uint)i * 0x3700) == 0x02)
            {
                if (switchTxt == "on" && index == 0)
                    setClientMod(notify, "God mode / Invisible On", 0x01319968, 0x280, new byte[] { 0x00, 0x00 }, i);
                else if (switchTxt == "off" && index == 0)
                    setClientMod(notify, "God mode / Invisible Off", 0x01319968, 0x280, new byte[] { 0x00, 0x4C }, i);
                else if (switchTxt == "on" && index == 1)
                    setClientMod(notify, "God mode On", 0x14e5429, new byte[] { 0x0A, 0x0A, 0x0A, 0x0A }, i);
                else if (switchTxt == "off" && index == 1)
                    setClientMod(notify, "God mode Off", 0x14e5429, new byte[] { 0x00, 0x00, 0x64, 0x00 }, i);
                else if (switchTxt == "on" && index == 2)
                {
                    byte[] val = new byte[] { 0x15, 0xff, 0xff, 0xff };
                    PS3.SetMemory(0x14e24ec + (uint)i * 0x3700, val);
                    PS3.SetMemory(0x14e24dc + (uint)i * 0x3700, val);
                    PS3.SetMemory(0x14e2554 + (uint)i * 0x3700, val);
                    PS3.SetMemory(0x14e256c + (uint)i * 0x3700, val);
                    PS3.SetMemory(0x14e2560 + (uint)i * 0x3700, val);
                    PS3.SetMemory(0x14e2578 + (uint)i * 0x3700, val);
                    if (notify)
                        callNotify(i, "Infinite Ammo On");
                }
                else if (switchTxt == "off" && index == 2)
                {
                    PS3.SetMemory(0x14e24ec + (uint)i * 0x3700, new byte[] { 0x00, 0x00, 0x00, 0x40 });
                    PS3.SetMemory(0x14e24dc + (uint)i * 0x3700, new byte[] { 0x00, 0x00, 0x00, 0x40 });
                    PS3.SetMemory(0x14e2554 + (uint)i * 0x3700, new byte[] { 0x00, 0x00, 0x00, 0x0A });
                    PS3.SetMemory(0x14e256c + (uint)i * 0x3700, new byte[] { 0x00, 0x00, 0x00, 0x20 });
                    PS3.SetMemory(0x14e2560 + (uint)i * 0x3700, new byte[] { 0x00, 0x00, 0x00, 0x01 });
                    PS3.SetMemory(0x14e2578 + (uint)i * 0x3700, new byte[] { 0x00, 0x00, 0x00, 0x02 });
                    if (notify)
                        callNotify(i, "Infinite Ammo Off");
                }
                else if (switchTxt == "on" && index == 3)
                    setClientMod(notify, "Redboxes On", 0x14e2213, new byte[] { 0x55 }, i);
                else if (switchTxt == "off" && index == 3)
                    setClientMod(notify, "Redboxes Off", 0x14e2213, new byte[] { 0x00 }, i);
                else if (switchTxt == "on" && index == 4)
                    setClientMod(notify, "No Recoil On", 0x014e24be, new byte[] { 0x04 }, i);
                else if (switchTxt == "off" && index == 4)
                    setClientMod(notify, "No Recoil Off", 0x014e24be, new byte[] { 0x00 }, i);
                else if (switchTxt == "on" && index == 5)
                    setPerk(notify, i, "Explosive Bullets x10000 Set", "specialty_explosivebullets;set bg_bulletExplDmgFactor 100;set bg_bulletExplRadius 10000");
                else if (switchTxt == "off" && index == 5)
                    setPerk(notify, i, "Explosive Bullets Off", "specialty_explosivebullets;reset bg_bulletExplDmgFactor;reset bg_bulletExplRadius");
                else if (switchTxt == "on" && index == 6)
                    setClientMod(notify, "UFO Mode On", 0x14e5623, new byte[] { 0x02 }, i);
                else if (switchTxt == "off" && index == 6)
                    setClientMod(notify, "UFO Mode Off", 0x14e5623, new byte[] { 0x00 }, i);
                else if (switchTxt == "on" && index == 7)
                    setClientMod(notify, "No Clip On", 0x14e5623, new byte[] { 0x01 }, i);
                else if (switchTxt == "off" && index == 7)
                    setClientMod(notify, "No Clip Off", 0x14e5623, new byte[] { 0x00 }, i);
                else if (switchTxt == "on" && index == 8)
                    setClientMod(notify, "Set All Perks On", 0x14e2628, new byte[] { 0xFf, 0xFF }, i);
                else if (switchTxt == "off" && index == 8)
                    setClientMod(notify, "Set All Perks Off", 0x14e2628, new byte[] { 0x00, 0x00 }, i);
            }
        }
    }

    #endregion
    #region Spawn Bots
    #region defaults
    bool canSpawn = false;
    bool cantMove = false;
    string spawnMethod = "";
    bool randomSpawn = true;
    string spawnStr = "";
    #endregion
    #region methods
    private void spawnBot(float[] pos)
    {
        spawnMethod = "";
        for (int i = 0; i < 18; i++)
        {
            if (PS3.Extension.ReadString(0x14E5490 + 0x3700 * (uint)i) == "")
            {
                byte[] botid = PS3.Extension.ReadBytes(0x01319958 + (uint)i * 0x280, 3);
                RPC.Call(0x002189D8, 1);
                Thread.Sleep(500);
                method(hostNum, i, botid, pos);
                break;
            }
        }

    }

    private void respawnBot(int botNum, float[] pos)
    {
        byte[] botid = PS3.Extension.ReadBytes(0x01319958 + ((uint)botNum * 0x280), 3);
        method(hostNum, botNum, botid, pos);
    }

    private void method(int hostNum, int botnum, byte[] botId, float[] pos)
    {
        string saveCT = PS3.Extension.ReadString(0x014E54CC + (uint)hostNum * 0x3700);
        PS3.Extension.WriteBytes(0x014E54CB + (uint)hostNum * 0x3700, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 });
        Thread.Sleep(100);
        PS3.Extension.WriteBytes(0x014E2204 + ((uint)botnum * 0x3700), PS3.Extension.ReadBytes(0x014E2204 + ((uint)hostNum * 0x3700), 0x3700));
        PS3.Extension.WriteFloats(clientPosOfs + (uint)(0x3700 * botnum), pos);
        PS3.Extension.WriteString(0x014E54CC + ((uint)hostNum * 0x3700), saveCT);
        Thread.Sleep(100);
        PS3.Extension.WriteBytes(0x014E5490 + ((uint)botnum * 0x3700), Encoding.ASCII.GetBytes("Client Bot " + botnum + "\0"));
        PS3.Extension.WriteBytes(0x014E5408 + ((uint)botnum * 0x3700), Encoding.ASCII.GetBytes("Client Bot " + botnum + "\0"));
        Thread.Sleep(100);
        PS3.Extension.WriteBytes(0x01319807 + ((uint)botnum * 0x280), PS3.Extension.ReadBytes(0x01319807 + ((uint)hostNum * 0x280), 0x26c));
        PS3.Extension.WriteBytes(0x01319958 + ((uint)botnum * 0x280), botId);
        Thread.Sleep(100);
        if (cantMove)
        {
            PS3.SetMemory(0x14e5623 + (uint)(0x3700 * botnum), new byte[] { 0x04 });
        }
        Thread.Sleep(100);
        string str = PS3.Extension.ReadString(0xD495F4);
        if (str.Contains("afghan") || str.Contains("highrise") || str.Contains("rundown") || str.Contains("quarry") || str.Contains("nightshift") || str.Contains("brecourt") || str.Contains("terminal"))
        {
            PS3.SetMemory(0x014E24EA + (uint)(0x3700 * botnum), new byte[] { 0x01, 0x05 });
            PS3.SetMemory(0x014E24B6 + (uint)(0x3700 * botnum), new byte[] { 0x02, 0x32 });
            PS3.SetMemory(0x014E242A + (uint)(0x3700 * botnum), new byte[] { 0x02, 0x32 });
            PS3.SetMemory(0x014E256F + (uint)(0x3700 * botnum), new byte[] { 0x50 });
            PS3.SetMemory(0x014E24EF + (uint)(0x3700 * botnum), new byte[] { 0x50 });
        }
        if (str.Contains("derail") || str.Contains("estate") || str.Contains("favela") || str.Contains("invasion") || str.Contains("rust") || str.Contains("scrapyard") || str.Contains("boneyard") || str.Contains("sub") || str.Contains("underpass") || str.Contains("checkpoint"))
        {
            PS3.SetMemory(0x014E24EA + (uint)(0x3700 * botnum), new byte[] { 0x01, 0x04 });
            PS3.SetMemory(0x014E24B6 + (uint)(0x3700 * botnum), new byte[] { 0x02, 0x31 });
            PS3.SetMemory(0x014E242A + (uint)(0x3700 * botnum), new byte[] { 0x02, 0x31 });
            PS3.SetMemory(0x014E256F + (uint)(0x3700 * botnum), new byte[] { 0x50 });
            PS3.SetMemory(0x014E24EF + (uint)(0x3700 * botnum), new byte[] { 0x50 });
        }
    }

    private void runSpawn()
    {
        if (randomSpawn)
            spawnStr = RPC.get_MapName();
        else
            spawnStr = "";

        for (int i = 0; i < 18; i++)
        {
            string botName = PS3.Extension.ReadString(0x014E5408 + (uint)i * 0x3700);
            if (PS3.Extension.ReadByte(0x014e5383 + (uint)i * 0x3700) != 0x00 && botName.StartsWith("Client Bot"))
                respawnBot(i, mapSpawn(spawnStr));
        }
        if (spawnMethod == "spawn")
            spawnBot(mapSpawn(spawnStr));
    }
    #endregion
    #region ouputs
    float[] mapSpawn(string map)
    {
        if (map == "Afghan")
            return new float[] { 1870, 1015, 103 };
        else if (map == "Derail")
            return new float[] { 1309, -332, 188 };
        else if (map == "Estate")
            return new float[] { -598, 2212, -48 };
        else if (map == "Favela")
            return new float[] { 127, -82, 54 };
        else if (map == "Highrise")
            return new float[] { -1228, 6444, 2836 };
        else if (map == "Invasion")
            return new float[] { -1184, -2334, 324 };
        else if (map == "Karachi")
            return new float[] { 313, -1093, 60 };
        else if (map == "Quarry")
            return new float[] { -3844, 645, -252 };
        else if (map == "Rundown")
            return new float[] { 437, -34, 76 };
        else if (map == "Rust")
            return new float[] { -45, 1057, -176 };
        else if (map == "Scrapyard")
            return new float[] { 726, 380, -61 };
        else if (map == "Skidrow")
            return new float[] { -1219, -631, 48 };
        else if (map == "Sub Base")
            return new float[] { 235, -570, 148 };
        else if (map == "Terminal")
            return new float[] { 1143, 3768, 100 };
        else if (map == "Underpass")
            return new float[] { 1128, 2145, 444 };
        else if (map == "Wasteland")
            return new float[] { 806, 617, 34 };
        else
            return getPlayerPos(hostNum);

    }
    float[] getPlayerPos(int client)
    {
        float[] get = PS3.Extension.ReadFloats(clientPosOfs + (uint)client * 0x3700, 3);
        return new float[] { (float)Math.Round(get[0]), (float)Math.Round(get[1]), (float)Math.Round(get[2] + 60) };
    }
    #endregion
    #region btns
    private void button68_Click(object sender, EventArgs e)
    {
        MessageBox.Show("Known issues:\nSpawning/Killing may freeze ps3 or glitch player/PS3\nThe host can only kill a bot when the game mode is set to Free-for-all", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void button40_Click(object sender, EventArgs e)
    {
        spawnMethod = "spawn";
        canSpawn = true;
    }

    private void colorCheckBox11_CheckedChanged(object sender, EventArgs e)
    {
        randomSpawn = colorCheckBox11.Checked;
    }

    #endregion
    #endregion
    #region Client Forge Mode
    #region process
    private void xrunForgeMode()
    {
        for (int i = 0; i < 18; i++)
        {
            if (xclientForge[i])
            {
                if (RPC.L1Press(i))
                {
                    if (xforgeTarget != -1)
                        xforgeMode(i, xforgeTarget, radiusLen);
                    else
                        xforgeTarget = closestClient(i);
                }
                else
                {
                    if (xforgeL1Tog[i] == 0 || xforgeL1Tog[i] == 2)
                    {
                        xforgeL1Tog[i] = 1;
                        PS3.SetMemory(0x14e5623 + 0x3700 * (uint)xforgeTarget, new byte[] { 0x00 });
                        xforgeTarget = -1;
                    }
                }
            }
        }
    }

    private void xforgeMode(int client, int target, int radius)
    {
        if (xforgeL1Tog[client] == 0 || xforgeL1Tog[client] == 1)
        {
            xforgeL1Tog[client] = 2;
            PS3.SetMemory(0x14e5623 + 0x3700 * (uint)target, new byte[] { 0x04 });
        }
        xposy = ReadS(0x014E221C + 0x3700 * (uint)client, 1);
        xposx = ReadS(0x014E221C + 0x3700 * (uint)client, 2);
        xposz = ReadS(0x014E221C + 0x3700 * (uint)client, 3);
        xposa = PS3.Extension.ReadFloat(0x131984C + 0x280 * (uint)client);
        xCpoint(radius, new Point(Convert.ToInt16(xposx), Convert.ToInt16(xposy)));
        xposv = PS3.Extension.ReadFloat(0x1319848 + 0x280 * (uint)client);
        PS3.Extension.WriteFloats(0x014E221C + 0x3700 * (uint)target, new float[] { xangleY, xangleX, xposz - xposv * 3 });
    }
    #endregion
    #region defaults
    int xforgeTarget = -1;
    bool[] xclientForge = new bool[18];
    int[] xforgeL1Tog = new int[18];
    float xposy = 0;
    float xposx = 0;
    float xposz = 0;
    float xposa = 0;
    float xposv = 0;
    float xposac = 0;
    int xangleX = 0;
    int xangleY = 0;
    int xangleXc = 0;
    int xangleYc = 0;
    int radiusLen = 150;
    bool[] clientForge = new bool[18];
    int[] forgeL1Tog = new int[18];
    #endregion
    #region outputs
    private void xCpoint(double radius, Point center)
    {
        double slice = 2 * Math.PI / -360;
        double angle = slice * xposa - 11;
        xangleX = (int)(center.X + radius * Math.Cos(angle));
        xangleY = (int)(center.Y + radius * Math.Sin(angle));
    }
    private void xCpointc(double radius, Point center)
    {
        double slice = 2 * Math.PI / -360;
        double angle = slice * xposac - 11;
        xangleXc = (int)(center.X + radius * Math.Cos(angle));
        xangleYc = (int)(center.Y + radius * Math.Sin(angle));
    }
    public static float xReadS(uint address, int length)
    {
        byte[] memory = PS3.GetBytes(address, length * 4);
        Array.Reverse(memory);
        float single = new float();
        for (int i = 0; i < length; i++)
        {
            single = BitConverter.ToSingle(memory, (length - 1 - i) * 4);
        }
        return single;
    }

    public static string xByteArrayToString(byte[] ba)
    {
        string hex = BitConverter.ToString(ba);
        return hex.Replace("-", "");
    }


    public static float ReadS(uint address, int length)
    {
        byte[] memory = PS3.GetBytes(address, length * 4);
        Array.Reverse(memory);
        float single = new float();
        for (int i = 0; i < length; i++)
        {
            single = BitConverter.ToSingle(memory, (length - 1 - i) * 4);
        }
        return single;
    }

    public static string ByteArrayToString(byte[] ba)
    {
        string hex = BitConverter.ToString(ba);
        return hex.Replace("-", "");
    }
    #endregion
    #region controls

    private void onToolStripMenuItem9_Click(object sender, EventArgs e)
    {
        xclientForge[dataGridView2.CurrentRow.Index] = true;
        callNotify(dataGridView2.CurrentRow.Index, "Forge Mode ^2On ^7| Press L1 to activate");
    }

    private void offToolStripMenuItem9_Click(object sender, EventArgs e)
    {
        xclientForge[dataGridView2.CurrentRow.Index] = false;
        callNotify(dataGridView2.CurrentRow.Index, "Forge Mode Off");
    }
    #endregion

    #endregion
    #region Killstreak Bullets
    private void aC130105mmToolStripMenuItem_Click(object sender, EventArgs e)
    {
        setModdedGun("AC130 105mm");
    }

    private void aC130ToolStripMenuItem_Click(object sender, EventArgs e)
    {
        setModdedGun("AC130 40mm");
    }

    private void walkingAC13022mmToolStripMenuItem_Click(object sender, EventArgs e)
    {
        setModdedGun("AC130 25mm");
    }

    private void cobra20mmToolStripMenuItem_Click(object sender, EventArgs e)
    {
        setModdedGun("Cobra 20mm");
    }

    private void cobraMinigunToolStripMenuItem_Click(object sender, EventArgs e)
    {
        setModdedGun("Cobra Minigun");
    }

    private void cobraFfarToolStripMenuItem_Click(object sender, EventArgs e)
    {
        setModdedGun("Cobra Ffar");
    }

    private void harrierToolStripMenuItem_Click(object sender, EventArgs e)
    {
        setModdedGun("Harrier Ffar");
    }

    private void harrier20mmToolStripMenuItem_Click(object sender, EventArgs e)
    {
        setModdedGun("Harrier 20mm");
    }

    private void pavelowMinigunToolStripMenuItem_Click(object sender, EventArgs e)
    {
        setModdedGun("Pavelow Minigun");
    }

    private void littlebird20mmToolStripMenuItem_Click(object sender, EventArgs e)
    {
        setModdedGun("Little Bird 20mm");
    }

    private void sentryGunToolStripMenuItem_Click(object sender, EventArgs e)
    {
        setModdedGun("Sentry Minigun");
    }

    private void noobTubeToolStripMenuItem_Click(object sender, EventArgs e)
    {
        setModdedGun("Noobtube");
    }

    private void setModdedGun(string weapon)
    {
        byte[] wep = { };
        string str = PS3.Extension.ReadString(0xD495F4);
        if (str.Contains("afghan") || str.Contains("highrise") || str.Contains("rundown") || str.Contains("quarry") || str.Contains("nightshift") || str.Contains("brecourt") || str.Contains("terminal"))
        {
            if (weapon == "AC130 25mm")
                wep = new byte[] { 0x04, 0x97, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x97, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x20, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "AC130 40mm")
                wep = new byte[] { 0x04, 0x98, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x98, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x21, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "AC130 105mm")
                wep = new byte[] { 0x04, 0x99, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x99, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x22, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "Cobra 20mm")
                wep = new byte[] { 0x04, 0xA2, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0xA2, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x2A, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "Cobra Ffar")
                wep = new byte[] { 0x04, 0xA1, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0xA1, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x29, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "Cobra Minigun")
                wep = new byte[] { 0x04, 0xA3, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0xA3, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x2B, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "Harrier 20mm")
                wep = new byte[] { 0x04, 0x9E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x9E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x26, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "Harrier Ffar")
                wep = new byte[] { 0x04, 0x9F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x9F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x27, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "Little Bird 20mm")
                wep = new byte[] { 0x04, 0xA0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0xA0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x28, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "Pavelow Minigun")
                wep = new byte[] { 0x04, 0xA5, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0xA5, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x2D, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "Noobtube")
                wep = new byte[] { 0x01, 0x67, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x67, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAF, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "Sentry Minigun")
                wep = new byte[] { 0x04, 0xA6, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0xA6, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x2E, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        }
        else if (str.Contains("derail") || str.Contains("estate") || str.Contains("favela") || str.Contains("invasion") || str.Contains("rust") || str.Contains("scrapyard") || str.Contains("boneyard") || str.Contains("sub") || str.Contains("underpass") || str.Contains("checkpoint"))
        {
            if (weapon == "AC130 25mm")
                wep = new byte[] { 0x04, 0x96, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x96, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x1F, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "AC130 40mm")
                wep = new byte[] { 0x04, 0x97, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x97, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x20, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "AC130 105mm")
                wep = new byte[] { 0x04, 0x98, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x98, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x21, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "Cobra 20mm")
                wep = new byte[] { 0x04, 0xA1, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0xA1, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x29, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "Cobra Ffar")
                wep = new byte[] { 0x04, 0xA0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0xA0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x28, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "Cobra Minigun")
                wep = new byte[] { 0x04, 0xA2, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0xA2, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x2A, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "Harrier 20mm")
                wep = new byte[] { 0x04, 0x9D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x9D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x25, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "Harrier Ffar")
                wep = new byte[] { 0x04, 0x9E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x9E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x26, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "Little Bird 20mm")
                wep = new byte[] { 0x04, 0x9F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x9F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x27, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "Pavelow Minigun")
                wep = new byte[] { 0x04, 0xA4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0xA4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x2C, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "Noobtube")
                wep = new byte[] { 0x01, 0x66, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x66, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAE, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            else if (weapon == "Sentry Minigun")
                wep = new byte[] { 0x04, 0xA5, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0xA5, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x2D, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        }
        int index = dataGridView2.CurrentRow.Index;
        PS3.SetMemory(0x014e2422 + (uint)index * 0x3700, wep);
        callNotify(index, weapon + " Set");
    }
    #endregion
    #region Teleporting
    float[] savedPos = new float[] { 0, 0, 0 };
    private void skyToolStripMenuItem_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < 18; i++)
            setClientMod(true, "All To Sky Set", 0x14e2224, new byte[] { 0x47 }, i);
    }

    private void undergroundToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < 18; i++)
            setClientMod(true, "All To Underground Set", 0x14e2224, new byte[] { 0xC7 }, i);
    }

    private void customToolStripMenuItem_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < 18; i++)
            setClientMod(true, "All To Custom Location Set", 0x14e221C, savedPos, i);
    }

    private void skyToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        setClientMod(true, "To Sky Set", 0x14e2224, new byte[] { 0x47 }, dataGridView2.CurrentRow.Index);
    }

    private void undergroundToolStripMenuItem2_Click(object sender, EventArgs e)
    {
        setClientMod(true, "To Underground Set", 0x14e2224, new byte[] { 0xC7 }, dataGridView2.CurrentRow.Index);
    }

    private void customLocationToolStripMenuItem_Click(object sender, EventArgs e)
    {
        setClientMod(true, "Custom Teleport Set", 0x14e221C, savedPos, dataGridView2.CurrentRow.Index);
    }
    float[] clientPos(int client)
    {
        return PS3.Extension.ReadFloats(0x14e221C + (uint)client * 0x3700, 3);
    }
    private void saveLocationToolStripMenuItem_Click(object sender, EventArgs e)
    {
        savedPos = clientPos(dataGridView2.CurrentRow.Index);
    }
    #endregion
    #endregion
    #region Weapon Setup

    #region defaults
    bool autoUpdateWeapList = false;
    int camNumPrim;
    int weapNumPrim;
    int camNumSec;
    int weapNumSec;
    int savePrim;
    int saveSec;
    #endregion

    #region buttons

    private void button69_Click(object sender, EventArgs e)
    {
        WeaponIndexes weap = (WeaponIndexes)weapNumPrim;
        Weapon_Camos camos = (Weapon_Camos)camNumPrim;
        G_GivePlayerWeapon(dataGridView3.CurrentRow.Index, weap, camos, colorRadioButton20.Checked, colorRadioButton43.Checked);
    }

    private void button70_Click(object sender, EventArgs e)
    {
        WeaponIndexes weap = (WeaponIndexes)weapNumSec;
        Weapon_Camos camos = (Weapon_Camos)camNumSec;
        G_GivePlayerWeapon(dataGridView3.CurrentRow.Index, weap, camos, colorRadioButton41.Checked, colorRadioButton43.Checked);
    }

    private void button50_Click(object sender, EventArgs e)
    {
        MessageBox.Show("Custom Weapon Setup by MayhemModding\n\nThis unique setup allows you to make any gun with any possible attachment and camo.\n\nWhen generating the weapon it may not work due to the fact that isn't ment to be draw ingame, although most combinations do work well!", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void button48_Click(object sender, EventArgs e)
    {
        if (colorComboBox21.SelectedIndex == 0)
        {
            WeaponIndexes weap = (WeaponIndexes)1;
            Weapon_Camos camos = (Weapon_Camos)0;
            G_GivePlayerWeapon(dataGridView3.CurrentRow.Index, weap, camos, false, true);
        }
        if (colorComboBox21.SelectedIndex == 1)
        {
            WeaponIndexes weap = (WeaponIndexes)1;
            Weapon_Camos camos = (Weapon_Camos)0;
            G_GivePlayerWeapon(dataGridView3.CurrentRow.Index, weap, camos, true, true);
        }
        if (colorComboBox21.SelectedIndex == 2)
        {
            WeaponIndexes weap = (WeaponIndexes)2;
            Weapon_Camos camos = (Weapon_Camos)0;
            G_GivePlayerWeapon(dataGridView3.CurrentRow.Index, weap, camos, true, true);
        }
        if (colorComboBox21.SelectedIndex == 3)
        {
            WeaponIndexes weap = (WeaponIndexes)45;
            Weapon_Camos camos = (Weapon_Camos)0;
            G_GivePlayerWeapon(dataGridView3.CurrentRow.Index, weap, camos, false, true);
        }
        if (colorComboBox21.SelectedIndex == 4)
        {
            WeaponIndexes weap = (WeaponIndexes)994;
            Weapon_Camos camos = (Weapon_Camos)6;
            G_GivePlayerWeapon(dataGridView3.CurrentRow.Index, weap, camos, false, true);
            RPC.SV_GameSendServerCommand(dataGridView3.CurrentRow.Index, "v perk_weapReloadMultiplier 0.0001");
            RPC.SV_GameSendServerCommand(dataGridView3.CurrentRow.Index, "v perk_weapSpreadMultiplier 0.0001");
            RPC.SV_GameSendServerCommand(dataGridView3.CurrentRow.Index, "v perk_fastSnipeScale 9;perk_quickDrawSpeedScale 6.5");
            setPerk(false, dataGridView3.CurrentRow.Index, "", "specialty_fastreload");
            setPerk(false, dataGridView3.CurrentRow.Index, "", "specialty_bulletaccuracy");
            setPerk(false, dataGridView3.CurrentRow.Index, "", "specialty_bulletdamage");
            setPerk(false, dataGridView3.CurrentRow.Index, "", "specialty_armorpiercing");
            setPerk(false, dataGridView3.CurrentRow.Index, "", "specialty_quickdraw");
            PS3.SetMemory(0x14e24be + (uint)dataGridView3.CurrentRow.Index * 0x3700, new byte[] { 0x04 });
        }
    }

    private void button49_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < 18; i++)
            dataGridView3.Rows[i].Cells[1].Value = RPC.getName(i);
    }

    private void colorCheckBox4_CheckedChanged(object sender, EventArgs e)
    {
        autoUpdateWeapList = colorCheckBox4.Checked;
    }
    #endregion

    #region functions
    private void G_GivePlayerWeapon(int client, WeaponIndexes wpn, Weapon_Camos Camo, bool akimbo, bool infiteAmmo)
    {
        if (infiteAmmo)
            RemoveWeap();
        int Index = RPC.Call(0x00032f90, Enum.GetName(typeof(WeaponIndexes), wpn));
        RPC.Call(0x001C0890, RPC.G_Client(client), Index, (int)Camo, Convert.ToInt32(akimbo));
        RPC.Call(0x00174BF8, RPC.G_Entity(client), Index, 0, 9999, 1);
        RPC.SV_GameSendServerCommand(client, "a" + Index);
        if (infiteAmmo)
            DoInfAmmo();
        callNotify(client, wpn.ToString() + " Set");
    }

    public enum WeaponIndexes
    {
        None = 0,
        defaultweapon_mp = 1,
        riotshield_mp = 2,
        beretta_mp = 3,
        beretta_akimbo_mp = 4,
        beretta_fmj_mp = 5,
        beretta_silencer_mp = 6,
        beretta_tactical_mp = 7,
        beretta_xmags_mp = 8,
        beretta_akimbo_fmj_mp = 9,
        beretta_akimbo_silencer_mp = 10,
        beretta_akimbo_xmags_mp = 11,
        beretta_fmj_silencer_mp = 12,
        beretta_fmj_tactical_mp = 13,
        beretta_fmj_xmags_mp = 14,
        beretta_silencer_tactical_mp = 15,
        beretta_silencer_xmags_mp = 16,
        beretta_tactical_xmags_mp = 17,
        usp_mp = 18,
        usp_akimbo_mp = 19,
        usp_fmj_mp = 20,
        usp_silencer_mp = 21,
        usp_tactical_mp = 22,
        usp_xmags_mp = 23,
        usp_akimbo_fmj_mp = 24,
        usp_akimbo_silencer_mp = 25,
        usp_akimbo_xmags_mp = 26,
        usp_fmj_silencer_mp = 27,
        usp_fmj_tactical_mp = 28,
        usp_fmj_xmags_mp = 29,
        usp_silencer_tactical_mp = 30,
        usp_silencer_xmags_mp = 31,
        usp_tactical_xmags_mp = 32,
        deserteagle_mp = 33,
        deserteagle_akimbo_mp = 34,
        deserteagle_fmj_mp = 35,
        deserteagle_tactical_mp = 36,
        deserteagle_akimbo_fmj_mp = 37,
        deserteagle_fmj_tactical_mp = 38,
        coltanaconda_mp = 39,
        coltanaconda_akimbo_mp = 40,
        coltanaconda_fmj_mp = 41,
        coltanaconda_tactical_mp = 42,
        coltanaconda_akimbo_fmj_mp = 43,
        coltanaconda_fmj_tactical_mp = 44,
        deserteaglegold_mp = 45,
        glock_mp = 46,
        glock_akimbo_mp = 47,
        glock_eotech_mp = 48,
        glock_fmj_mp = 49,
        glock_reflex_mp = 50,
        glock_silencer_mp = 51,
        glock_xmags_mp = 52,
        glock_akimbo_fmj_mp = 53,
        glock_akimbo_silencer_mp = 54,
        glock_akimbo_xmags_mp = 55,
        glock_eotech_fmj_mp = 56,
        glock_eotech_silencer_mp = 57,
        glock_eotech_xmags_mp = 58,
        glock_fmj_reflex_mp = 59,
        glock_fmj_silencer_mp = 60,
        glock_fmj_xmags_mp = 61,
        glock_reflex_silencer_mp = 62,
        glock_reflex_xmags_mp = 63,
        glock_silencer_xmags_mp = 64,
        beretta393_mp = 65,
        beretta393_akimbo_mp = 66,
        beretta393_eotech_mp = 67,
        beretta393_fmj_mp = 68,
        beretta393_reflex_mp = 69,
        beretta393_silencer_mp = 70,
        beretta393_xmags_mp = 71,
        beretta393_akimbo_fmj_mp = 72,
        beretta393_akimbo_silencer_mp = 73,
        beretta393_akimbo_xmags_mp = 74,
        beretta393_eotech_fmj_mp = 75,
        beretta393_eotech_silencer_mp = 76,
        beretta393_eotech_xmags_mp = 77,
        beretta393_fmj_reflex_mp = 78,
        beretta393_fmj_silencer_mp = 79,
        beretta393_fmj_xmags_mp = 80,
        beretta393_reflex_silencer_mp = 81,
        beretta393_reflex_xmags_mp = 82,
        beretta393_silencer_xmags_mp = 83,
        pp2000_mp = 84,
        pp2000_akimbo_mp = 85,
        pp2000_eotech_mp = 86,
        pp2000_fmj_mp = 87,
        pp2000_reflex_mp = 88,
        pp2000_silencer_mp = 89,
        pp2000_xmags_mp = 90,
        pp2000_akimbo_fmj_mp = 91,
        pp2000_akimbo_silencer_mp = 92,
        pp2000_akimbo_xmags_mp = 93,
        pp2000_eotech_fmj_mp = 94,
        pp2000_eotech_silencer_mp = 95,
        pp2000_eotech_xmags_mp = 96,
        pp2000_fmj_reflex_mp = 97,
        pp2000_fmj_silencer_mp = 98,
        pp2000_fmj_xmags_mp = 99,
        pp2000_reflex_silencer_mp = 100,
        pp2000_reflex_xmags_mp = 101,
        pp2000_silencer_xmags_mp = 102,
        tmp_mp = 103,
        tmp_akimbo_mp = 104,
        tmp_eotech_mp = 105,
        tmp_fmj_mp = 106,
        tmp_reflex_mp = 107,
        tmp_silencer_mp = 108,
        tmp_xmags_mp = 109,
        tmp_akimbo_fmj_mp = 110,
        tmp_akimbo_silencer_mp = 111,
        tmp_akimbo_xmags_mp = 112,
        tmp_eotech_fmj_mp = 113,
        tmp_eotech_silencer_mp = 114,
        tmp_eotech_xmags_mp = 115,
        tmp_fmj_reflex_mp = 116,
        tmp_fmj_silencer_mp = 117,
        tmp_fmj_xmags_mp = 118,
        tmp_reflex_silencer_mp = 119,
        tmp_reflex_xmags_mp = 120,
        tmp_silencer_xmags_mp = 121,
        mp5k_mp = 122,
        mp5k_acog_mp = 123,
        mp5k_akimbo_mp = 124,
        mp5k_eotech_mp = 125,
        mp5k_fmj_mp = 126,
        mp5k_reflex_mp = 127,
        mp5k_rof_mp = 128,
        mp5k_silencer_mp = 129,
        mp5k_thermal_mp = 130,
        mp5k_xmags_mp = 131,
        mp5k_acog_fmj_mp = 132,
        mp5k_acog_rof_mp = 133,
        mp5k_acog_silencer_mp = 134,
        mp5k_acog_xmags_mp = 135,
        mp5k_akimbo_fmj_mp = 136,
        mp5k_akimbo_rof_mp = 137,
        mp5k_akimbo_silencer_mp = 138,
        mp5k_akimbo_xmags_mp = 139,
        mp5k_eotech_fmj_mp = 140,
        mp5k_eotech_rof_mp = 141,
        mp5k_eotech_silencer_mp = 142,
        mp5k_eotech_xmags_mp = 143,
        mp5k_fmj_reflex_mp = 144,
        mp5k_fmj_rof_mp = 145,
        mp5k_fmj_silencer_mp = 146,
        mp5k_fmj_thermal_mp = 147,
        mp5k_fmj_xmags_mp = 148,
        mp5k_reflex_rof_mp = 149,
        mp5k_reflex_silencer_mp = 150,
        mp5k_reflex_xmags_mp = 151,
        mp5k_rof_silencer_mp = 152,
        mp5k_rof_thermal_mp = 153,
        mp5k_rof_xmags_mp = 154,
        mp5k_silencer_thermal_mp = 155,
        mp5k_silencer_xmags_mp = 156,
        mp5k_thermal_xmags_mp = 157,
        uzi_mp = 158,
        uzi_acog_mp = 159,
        uzi_akimbo_mp = 160,
        uzi_eotech_mp = 161,
        uzi_fmj_mp = 162,
        uzi_reflex_mp = 163,
        uzi_rof_mp = 164,
        uzi_silencer_mp = 165,
        uzi_thermal_mp = 166,
        uzi_xmags_mp = 167,
        uzi_acog_fmj_mp = 168,
        uzi_acog_rof_mp = 169,
        uzi_acog_silencer_mp = 170,
        uzi_acog_xmags_mp = 171,
        uzi_akimbo_fmj_mp = 172,
        uzi_akimbo_rof_mp = 173,
        uzi_akimbo_silencer_mp = 174,
        uzi_akimbo_xmags_mp = 175,
        uzi_eotech_fmj_mp = 176,
        uzi_eotech_rof_mp = 177,
        uzi_eotech_silencer_mp = 178,
        uzi_eotech_xmags_mp = 179,
        uzi_fmj_reflex_mp = 180,
        uzi_fmj_rof_mp = 181,
        uzi_fmj_silencer_mp = 182,
        uzi_fmj_thermal_mp = 183,
        uzi_fmj_xmags_mp = 184,
        uzi_reflex_rof_mp = 185,
        uzi_reflex_silencer_mp = 186,
        uzi_reflex_xmags_mp = 187,
        uzi_rof_silencer_mp = 188,
        uzi_rof_thermal_mp = 189,
        uzi_rof_xmags_mp = 190,
        uzi_silencer_thermal_mp = 191,
        uzi_silencer_xmags_mp = 192,
        uzi_thermal_xmags_mp = 193,
        p90_mp = 194,
        p90_acog_mp = 195,
        p90_akimbo_mp = 196,
        p90_eotech_mp = 197,
        p90_fmj_mp = 198,
        p90_reflex_mp = 199,
        p90_rof_mp = 200,
        p90_silencer_mp = 201,
        p90_thermal_mp = 202,
        p90_xmags_mp = 203,
        p90_acog_fmj_mp = 204,
        p90_acog_rof_mp = 205,
        p90_acog_silencer_mp = 206,
        p90_acog_xmags_mp = 207,
        p90_akimbo_fmj_mp = 208,
        p90_akimbo_rof_mp = 209,
        p90_akimbo_silencer_mp = 210,
        p90_akimbo_xmags_mp = 211,
        p90_eotech_fmj_mp = 212,
        p90_eotech_rof_mp = 213,
        p90_eotech_silencer_mp = 214,
        p90_eotech_xmags_mp = 215,
        p90_fmj_reflex_mp = 216,
        p90_fmj_rof_mp = 217,
        p90_fmj_silencer_mp = 218,
        p90_fmj_thermal_mp = 219,
        p90_fmj_xmags_mp = 220,
        p90_reflex_rof_mp = 221,
        p90_reflex_silencer_mp = 222,
        p90_reflex_xmags_mp = 223,
        p90_rof_silencer_mp = 224,
        p90_rof_thermal_mp = 225,
        p90_rof_xmags_mp = 226,
        p90_silencer_thermal_mp = 227,
        p90_silencer_xmags_mp = 228,
        p90_thermal_xmags_mp = 229,
        kriss_mp = 230,
        kriss_acog_mp = 231,
        kriss_akimbo_mp = 232,
        kriss_eotech_mp = 233,
        kriss_fmj_mp = 234,
        kriss_reflex_mp = 235,
        kriss_rof_mp = 236,
        kriss_silencer_mp = 237,
        kriss_thermal_mp = 238,
        kriss_xmags_mp = 239,
        kriss_acog_fmj_mp = 240,
        kriss_acog_rof_mp = 241,
        kriss_acog_silencer_mp = 242,
        kriss_acog_xmags_mp = 243,
        kriss_akimbo_fmj_mp = 244,
        kriss_akimbo_rof_mp = 245,
        kriss_akimbo_silencer_mp = 246,
        kriss_akimbo_xmags_mp = 247,
        kriss_eotech_fmj_mp = 248,
        kriss_eotech_rof_mp = 249,
        kriss_eotech_silencer_mp = 250,
        kriss_eotech_xmags_mp = 251,
        kriss_fmj_reflex_mp = 252,
        kriss_fmj_rof_mp = 253,
        kriss_fmj_silencer_mp = 254,
        kriss_fmj_thermal_mp = 255,
        kriss_fmj_xmags_mp = 256,
        kriss_reflex_rof_mp = 257,
        kriss_reflex_silencer_mp = 258,
        kriss_reflex_xmags_mp = 259,
        kriss_rof_silencer_mp = 260,
        kriss_rof_thermal_mp = 261,
        kriss_rof_xmags_mp = 262,
        kriss_silencer_thermal_mp = 263,
        kriss_silencer_xmags_mp = 264,
        kriss_thermal_xmags_mp = 265,
        ump45_mp = 266,
        ump45_acog_mp = 267,
        ump45_akimbo_mp = 268,
        ump45_eotech_mp = 269,
        ump45_fmj_mp = 270,
        ump45_reflex_mp = 271,
        ump45_rof_mp = 272,
        ump45_silencer_mp = 273,
        ump45_thermal_mp = 274,
        ump45_xmags_mp = 275,
        ump45_acog_fmj_mp = 276,
        ump45_acog_rof_mp = 277,
        ump45_acog_silencer_mp = 278,
        ump45_acog_xmags_mp = 279,
        ump45_akimbo_fmj_mp = 280,
        ump45_akimbo_rof_mp = 281,
        ump45_akimbo_silencer_mp = 282,
        ump45_akimbo_xmags_mp = 283,
        ump45_eotech_fmj_mp = 284,
        ump45_eotech_rof_mp = 285,
        ump45_eotech_silencer_mp = 286,
        ump45_eotech_xmags_mp = 287,
        ump45_fmj_reflex_mp = 288,
        ump45_fmj_rof_mp = 289,
        ump45_fmj_silencer_mp = 290,
        ump45_fmj_thermal_mp = 291,
        ump45_fmj_xmags_mp = 292,
        ump45_reflex_rof_mp = 293,
        ump45_reflex_silencer_mp = 294,
        ump45_reflex_xmags_mp = 295,
        ump45_rof_silencer_mp = 296,
        ump45_rof_thermal_mp = 297,
        ump45_rof_xmags_mp = 298,
        ump45_silencer_thermal_mp = 299,
        ump45_silencer_xmags_mp = 300,
        ump45_thermal_xmags_mp = 301,
        ak47_mp = 302,
        ak47_acog_mp = 303,
        ak47_eotech_mp = 304,
        ak47_fmj_mp = 305,
        ak47_gl_mp = 306,
        gl_ak47_mp = 307,
        ak47_heartbeat_mp = 308,
        ak47_reflex_mp = 309,
        ak47_shotgun_mp = 310,
        ak47_shotgun_attach_mp = 311,
        ak47_silencer_mp = 312,
        ak47_thermal_mp = 313,
        ak47_xmags_mp = 314,
        ak47_acog_fmj_mp = 315,
        ak47_acog_gl_mp = 316,
        ak47_acog_heartbeat_mp = 317,
        ak47_acog_shotgun_mp = 318,
        ak47_acog_silencer_mp = 319,
        ak47_acog_xmags_mp = 320,
        ak47_eotech_fmj_mp = 321,
        ak47_eotech_gl_mp = 322,
        ak47_eotech_heartbeat_mp = 323,
        ak47_eotech_shotgun_mp = 324,
        ak47_eotech_silencer_mp = 325,
        ak47_eotech_xmags_mp = 326,
        ak47_fmj_gl_mp = 327,
        ak47_fmj_heartbeat_mp = 328,
        ak47_fmj_reflex_mp = 329,
        ak47_fmj_shotgun_mp = 330,
        ak47_fmj_silencer_mp = 331,
        ak47_fmj_thermal_mp = 332,
        ak47_fmj_xmags_mp = 333,
        ak47_gl_heartbeat_mp = 334,
        ak47_gl_reflex_mp = 335,
        ak47_gl_silencer_mp = 336,
        ak47_gl_thermal_mp = 337,
        ak47_gl_xmags_mp = 338,
        ak47_heartbeat_reflex_mp = 339,
        ak47_heartbeat_shotgun_mp = 340,
        ak47_heartbeat_silencer_mp = 341,
        ak47_heartbeat_thermal_mp = 342,
        ak47_heartbeat_xmags_mp = 343,
        ak47_reflex_shotgun_mp = 344,
        ak47_reflex_silencer_mp = 345,
        ak47_reflex_xmags_mp = 346,
        ak47_shotgun_silencer_mp = 347,
        ak47_shotgun_thermal_mp = 348,
        ak47_shotgun_xmags_mp = 349,
        ak47_silencer_thermal_mp = 350,
        ak47_silencer_xmags_mp = 351,
        ak47_thermal_xmags_mp = 352,
        m16_mp = 353,
        m16_acog_mp = 354,
        m16_eotech_mp = 355,
        m16_fmj_mp = 356,
        m16_gl_mp = 357,
        gl_m16_mp = 358,
        m16_heartbeat_mp = 359,
        m16_reflex_mp = 360,
        m16_shotgun_mp = 361,
        m16_shotgun_attach_mp = 362,
        m16_silencer_mp = 363,
        m16_thermal_mp = 364,
        m16_xmags_mp = 365,
        m16_acog_fmj_mp = 366,
        m16_acog_gl_mp = 367,
        m16_acog_heartbeat_mp = 368,
        m16_acog_shotgun_mp = 369,
        m16_acog_silencer_mp = 370,
        m16_acog_xmags_mp = 371,
        m16_eotech_fmj_mp = 372,
        m16_eotech_gl_mp = 373,
        m16_eotech_heartbeat_mp = 374,
        m16_eotech_shotgun_mp = 375,
        m16_eotech_silencer_mp = 376,
        m16_eotech_xmags_mp = 377,
        m16_fmj_gl_mp = 378,
        m16_fmj_heartbeat_mp = 379,
        m16_fmj_reflex_mp = 380,
        m16_fmj_shotgun_mp = 381,
        m16_fmj_silencer_mp = 382,
        m16_gl_reflex_mp = 383,
        m16_gl_silencer_mp = 384,
        m16_gl_thermal_mp = 385,
        m16_gl_xmags_mp = 386,
        m16_heartbeat_reflex_mp = 387,
        m16_heartbeat_shotgun_mp = 388,
        m16_heartbeat_silencer_mp = 389,
        m16_heartbeat_thermal_mp = 390,
        m16_heartbeat_xmags_mp = 391,
        m16_reflex_shotgun_mp = 392,
        m16_reflex_silencer_mp = 393,
        m16_reflex_xmags_mp = 394,
        m16_shotgun_silencer_mp = 395,
        m16_shotgun_thermal_mp = 396,
        m16_shotgun_xmags_mp = 397,
        m16_silencer_thermal_mp = 398,
        m16_silencer_xmags_mp = 399,
        m16_thermal_xmags_mp = 400,
        m4_mp = 404,
        m4_acog_mp = 405,
        m4_eotech_mp = 406,
        m4_fmj_mp = 407,
        m4_gl_mp = 408,
        gl_m4_mp = 409,
        m4_heartbeat_mp = 410,
        m4_reflex_mp = 411,
        m4_shotgun_mp = 412,
        m4_shotgun_attach_mp = 413,
        m4_silencer_mp = 414,
        m4_thermal_mp = 415,
        m4_xmags_mp = 416,
        m4_acog_fmj_mp = 417,
        m4_acog_gl_mp = 418,
        m4_acog_heartbeat_mp = 419,
        m4_acog_shotgun_mp = 420,
        m4_acog_silencer_mp = 421,
        m4_acog_xmags_mp = 422,
        m4_eotech_fmj_mp = 423,
        m4_eotech_gl_mp = 424,
        m4_eotech_heartbeat_mp = 425,
        m4_eotech_shotgun_mp = 426,
        m4_eotech_silencer_mp = 427,
        m4_eotech_xmags_mp = 428,
        m4_fmj_gl_mp = 429,
        m4_fmj_heartbeat_mp = 430,
        m4_fmj_reflex_mp = 431,
        m4_fmj_shotgun_mp = 432,
        m4_fmj_silencer_mp = 433,
        m4_fmj_thermal_mp = 434,
        m4_fmj_xmags_mp = 435,
        m4_gl_heartbeat_mp = 436,
        m4_gl_reflex_mp = 437,
        m4_gl_silencer_mp = 438,
        m4_gl_thermal_mp = 439,
        m4_gl_xmags_mp = 440,
        m4_heartbeat_reflex_mp = 441,
        m4_heartbeat_shotgun_mp = 442,
        m4_heartbeat_silencer_mp = 443,
        m4_heartbeat_thermal_mp = 444,
        m4_heartbeat_xmags_mp = 445,
        m4_reflex_shotgun_mp = 446,
        m4_reflex_silencer_mp = 447,
        m4_reflex_xmags_mp = 448,
        m4_shotgun_silencer_mp = 449,
        m4_shotgun_thermal_mp = 450,
        m4_shotgun_xmags_mp = 451,
        m4_silencer_thermal_mp = 452,
        m4_silencer_xmags_mp = 453,
        m4_thermal_xmags_mp = 454,
        fn2000_mp = 455,
        fn2000_acog_mp = 456,
        fn2000_eotech_mp = 457,
        fn2000_fmj_mp = 458,
        fn2000_gl_mp = 459,
        gl_fn2000_mp = 460,
        fn2000_heartbeat_mp = 461,
        fn2000_reflex_mp = 462,
        fn2000_shotgun_mp = 463,
        fn2000_shotgun_attach_mp = 464,
        fn2000_silencer_mp = 465,
        fn2000_thermal_mp = 466,
        fn2000_xmags_mp = 467,
        fn2000_acog_fmj_mp = 468,
        fn2000_acog_gl_mp = 469,
        fn2000_acog_heartbeat_mp = 470,
        fn2000_acog_shotgun_mp = 471,
        fn2000_acog_silencer_mp = 472,
        fn2000_acog_xmags_mp = 473,
        fn2000_eotech_fmj_mp = 474,
        fn2000_eotech_gl_mp = 475,
        fn2000_eotech_heartbeat_mp = 476,
        fn2000_eotech_shotgun_mp = 477,
        fn2000_eotech_silencer_mp = 478,
        fn2000_eotech_xmags_mp = 479,
        fn2000_fmj_gl_mp = 480,
        fn2000_fmj_heartbeat_mp = 481,
        fn2000_fmj_reflex_mp = 482,
        fn2000_fmj_shotgun_mp = 483,
        fn2000_fmj_silencer_mp = 484,
        fn2000_fmj_thermal_mp = 485,
        fn2000_fmj_xmags_mp = 486,
        fn2000_gl_heartbeat_mp = 487,
        fn2000_gl_reflex_mp = 488,
        fn2000_gl_silencer_mp = 489,
        fn2000_gl_thermal_mp = 490,
        fn2000_gl_xmags_mp = 491,
        fn2000_heartbeat_reflex_mp = 492,
        fn2000_heartbeat_shotgun_mp = 493,
        fn2000_heartbeat_silencer_mp = 494,
        fn2000_heartbeat_thermal_mp = 495,
        fn2000_heartbeat_xmags_mp = 496,
        fn2000_reflex_shotgun_mp = 497,
        fn2000_reflex_silencer_mp = 498,
        fn2000_reflex_xmags_mp = 499,
        fn2000_shotgun_silencer_mp = 500,
        fn2000_shotgun_thermal_mp = 501,
        fn2000_shotgun_xmags_mp = 502,
        fn2000_silencer_thermal_mp = 503,
        fn2000_silencer_xmags_mp = 504,
        fn2000_thermal_xmags_mp = 505,
        masada_mp = 506,
        masada_acog_mp = 507,
        masada_eotech_mp = 508,
        masada_fmj_mp = 509,
        masada_gl_mp = 510,
        gl_masada_mp = 511,
        masada_heartbeat_mp = 512,
        masada_reflex_mp = 513,
        masada_shotgun_mp = 514,
        masada_shotgun_attach_mp = 515,
        masada_silencer_mp = 516,
        masada_thermal_mp = 517,
        masada_xmags_mp = 518,
        masada_acog_fmj_mp = 519,
        masada_acog_gl_mp = 520,
        masada_acog_heartbeat_mp = 521,
        masada_acog_shotgun_mp = 522,
        masada_acog_silencer_mp = 523,
        masada_acog_xmags_mp = 524,
        masada_eotech_fmj_mp = 525,
        masada_eotech_gl_mp = 526,
        masada_eotech_heartbeat_mp = 527,
        masada_eotech_shotgun_mp = 528,
        masada_eotech_silencer_mp = 529,
        masada_eotech_xmags_mp = 530,
        masada_fmj_gl_mp = 531,
        masada_fmj_heartbeat_mp = 532,
        masada_fmj_reflex_mp = 533,
        masada_fmj_shotgun_mp = 534,
        masada_fmj_silencer_mp = 535,
        masada_fmj_thermal_mp = 536,
        masada_fmj_xmags_mp = 537,
        masada_gl_heartbeat_mp = 538,
        masada_gl_reflex_mp = 539,
        masada_gl_silencer_mp = 540,
        masada_gl_thermal_mp = 541,
        masada_gl_xmags_mp = 542,
        masada_heartbeat_reflex_mp = 543,
        masada_heartbeat_shotgun_mp = 544,
        masada_heartbeat_silencer_mp = 545,
        masada_heartbeat_thermal_mp = 546,
        masada_heartbeat_xmags_mp = 547,
        masada_reflex_shotgun_mp = 548,
        masada_reflex_silencer_mp = 549,
        masada_reflex_xmags_mp = 550,
        masada_shotgun_silencer_mp = 551,
        masada_shotgun_thermal_mp = 552,
        masada_shotgun_xmags_mp = 553,
        masada_silencer_thermal_mp = 554,
        masada_silencer_xmags_mp = 555,
        masada_thermal_xmags_mp = 556,
        famas_mp = 557,
        famas_acog_mp = 558,
        famas_eotech_mp = 559,
        famas_fmj_mp = 560,
        famas_gl_mp = 561,
        gl_famas_mp = 562,
        famas_heartbeat_mp = 563,
        famas_reflex_mp = 564,
        famas_shotgun_mp = 565,
        famas_shotgun_attach_mp = 566,
        famas_silencer_mp = 567,
        famas_thermal_mp = 568,
        famas_xmags_mp = 569,
        famas_acog_fmj_mp = 570,
        famas_acog_gl_mp = 571,
        famas_acog_heartbeat_mp = 572,
        famas_acog_shotgun_mp = 573,
        famas_acog_silencer_mp = 574,
        famas_acog_xmags_mp = 575,
        famas_eotech_fmj_mp = 576,
        famas_eotech_gl_mp = 577,
        famas_eotech_heartbeat_mp = 578,
        famas_eotech_shotgun_mp = 579,
        famas_eotech_silencer_mp = 580,
        famas_eotech_xmags_mp = 581,
        famas_fmj_gl_mp = 582,
        famas_fmj_heartbeat_mp = 583,
        famas_fmj_reflex_mp = 584,
        famas_fmj_shotgun_mp = 585,
        famas_fmj_silencer_mp = 586,
        famas_fmj_thermal_mp = 587,
        famas_fmj_xmags_mp = 588,
        famas_gl_heartbeat_mp = 589,
        famas_gl_reflex_mp = 590,
        famas_gl_silencer_mp = 591,
        famas_gl_thermal_mp = 592,
        famas_gl_xmags_mp = 593,
        famas_heartbeat_reflex_mp = 594,
        famas_heartbeat_shotgun_mp = 595,
        famas_heartbeat_silencer_mp = 596,
        famas_heartbeat_thermal_mp = 597,
        famas_heartbeat_xmags_mp = 598,
        famas_reflex_shotgun_mp = 599,
        famas_reflex_silencer_mp = 600,
        famas_reflex_xmags_mp = 601,
        famas_shotgun_silencer_mp = 602,
        famas_shotgun_thermal_mp = 603,
        famas_shotgun_xmags_mp = 604,
        famas_silencer_thermal_mp = 605,
        famas_silencer_xmags_mp = 606,
        famas_thermal_xmags_mp = 607,
        fal_mp = 608,
        fal_acog_mp = 609,
        fal_eotech_mp = 610,
        fal_fmj_mp = 611,
        fal_gl_mp = 612,
        gl_fal_mp = 613,
        fal_heartbeat_mp = 614,
        fal_reflex_mp = 615,
        fal_shotgun_mp = 616,
        fal_shotgun_attach_mp = 617,
        fal_silencer_mp = 618,
        fal_thermal_mp = 619,
        fal_xmags_mp = 620,
        fal_acog_fmj_mp = 621,
        fal_acog_gl_mp = 622,
        fal_acog_heartbeat_mp = 623,
        fal_acog_shotgun_mp = 624,
        fal_acog_silencer_mp = 625,
        fal_acog_xmags_mp = 626,
        fal_eotech_fmj_mp = 627,
        fal_eotech_gl_mp = 628,
        fal_eotech_heartbeat_mp = 629,
        fal_eotech_shotgun_mp = 630,
        fal_eotech_silencer_mp = 631,
        fal_eotech_xmags_mp = 632,
        fal_fmj_gl_mp = 633,
        fal_fmj_heartbeat_mp = 634,
        fal_fmj_reflex_mp = 635,
        fal_fmj_shotgun_mp = 636,
        fal_fmj_silencer_mp = 637,
        fal_fmj_thermal_mp = 638,
        fal_fmj_xmags_mp = 639,
        fal_gl_heartbeat_mp = 640,
        fal_gl_reflex_mp = 641,
        fal_gl_silencer_mp = 642,
        fal_gl_thermal_mp = 643,
        fal_gl_xmags_mp = 644,
        fal_heartbeat_reflex_mp = 645,
        fal_heartbeat_shotgun_mp = 646,
        fal_heartbeat_silencer_mp = 647,
        fal_heartbeat_thermal_mp = 648,
        fal_heartbeat_xmags_mp = 649,
        fal_reflex_shotgun_mp = 650,
        fal_reflex_silencer_mp = 651,
        fal_reflex_xmags_mp = 652,
        fal_shotgun_silencer_mp = 653,
        fal_shotgun_thermal_mp = 654,
        fal_shotgun_xmags_mp = 655,
        fal_silencer_thermal_mp = 656,
        fal_silencer_xmags_mp = 657,
        fal_thermal_xmags_mp = 658,
        scar_mp = 659,
        scar_acog_mp = 660,
        scar_eotech_mp = 661,
        scar_fmj_mp = 662,
        scar_gl_mp = 663,
        gl_scar_mp = 664,
        scar_heartbeat_mp = 665,
        scar_reflex_mp = 666,
        scar_shotgun_mp = 667,
        scar_shotgun_attach_mp = 668,
        scar_silencer_mp = 669,
        scar_thermal_mp = 670,
        scar_xmags_mp = 671,
        scar_acog_fmj_mp = 672,
        scar_acog_gl_mp = 673,
        scar_acog_heartbeat_mp = 674,
        scar_acog_shotgun_mp = 675,
        scar_acog_silencer_mp = 676,
        scar_acog_xmags_mp = 677,
        scar_eotech_fmj_mp = 678,
        scar_eotech_gl_mp = 679,
        scar_eotech_heartbeat_mp = 680,
        scar_eotech_shotgun_mp = 681,
        scar_eotech_silencer_mp = 682,
        scar_eotech_xmags_mp = 683,
        scar_fmj_gl_mp = 684,
        scar_fmj_heartbeat_mp = 685,
        scar_fmj_reflex_mp = 686,
        scar_fmj_shotgun_mp = 687,
        scar_fmj_silencer_mp = 688,
        scar_fmj_thermal_mp = 689,
        scar_fmj_xmags_mp = 690,
        scar_gl_heartbeat_mp = 691,
        scar_gl_reflex_mp = 692,
        scar_gl_silencer_mp = 693,
        scar_gl_thermal_mp = 694,
        scar_gl_xmags_mp = 695,
        scar_heartbeat_reflex_mp = 696,
        scar_heartbeat_shotgun_mp = 697,
        scar_heartbeat_silencer_mp = 698,
        scar_heartbeat_thermal_mp = 699,
        scar_heartbeat_xmags_mp = 700,
        scar_reflex_shotgun_mp = 701,
        scar_reflex_silencer_mp = 702,
        scar_reflex_xmags_mp = 703,
        scar_shotgun_silencer_mp = 704,
        scar_shotgun_thermal_mp = 705,
        scar_shotgun_xmags_mp = 706,
        scar_silencer_thermal_mp = 707,
        scar_silencer_xmags_mp = 708,
        scar_thermal_xmags_mp = 709,
        tavor_mp = 710,
        tavor_acog_mp = 711,
        tavor_eotech_mp = 712,
        tavor_fmj_mp = 713,
        tavor_gl_mp = 714,
        gl_tavor_mp = 715,
        tavor_heartbeat_mp = 716,
        tavor_reflex_mp = 717,
        tavor_shotgun_mp = 718,
        tavor_shotgun_attach_mp = 719,
        tavor_silencer_mp = 720,
        tavor_thermal_mp = 721,
        tavor_xmags_mp = 722,
        tavor_acog_fmj_mp = 723,
        tavor_acog_gl_mp = 724,
        tavor_acog_heartbeat_mp = 725,
        tavor_acog_shotgun_mp = 726,
        tavor_acog_silencer_mp = 727,
        tavor_acog_xmags_mp = 728,
        tavor_eotech_fmj_mp = 729,
        tavor_eotech_gl_mp = 730,
        tavor_eotech_heartbeat_mp = 731,
        tavor_eotech_shotgun_mp = 732,
        tavor_eotech_silencer_mp = 733,
        tavor_eotech_xmags_mp = 734,
        tavor_fmj_gl_mp = 735,
        tavor_fmj_heartbeat_mp = 736,
        tavor_fmj_reflex_mp = 737,
        tavor_fmj_shotgun_mp = 738,
        tavor_fmj_silencer_mp = 739,
        tavor_fmj_thermal_mp = 740,
        tavor_fmj_xmags_mp = 741,
        tavor_gl_heartbeat_mp = 742,
        tavor_gl_reflex_mp = 743,
        tavor_gl_silencer_mp = 744,
        tavor_gl_thermal_mp = 745,
        tavor_gl_xmags_mp = 746,
        tavor_heartbeat_reflex_mp = 747,
        tavor_heartbeat_shotgun_mp = 748,
        tavor_heartbeat_silencer_mp = 749,
        tavor_heartbeat_thermal_mp = 750,
        tavor_heartbeat_xmags_mp = 751,
        tavor_reflex_shotgun_mp = 752,
        tavor_reflex_silencer_mp = 753,
        tavor_reflex_xmags_mp = 754,
        tavor_shotgun_silencer_mp = 755,
        tavor_shotgun_thermal_mp = 756,
        tavor_shotgun_xmags_mp = 757,
        tavor_silencer_thermal_mp = 758,
        tavor_silencer_xmags_mp = 759,
        tavor_thermal_xmags_mp = 760,
        gl_mp = 761,
        m79_mp = 762,
        rpg_mp = 763,
        at4_mp = 764,
        stinger_mp = 765,
        javelin_mp = 766,
        barrett_mp = 767,
        barrett_acog_mp = 768,
        barrett_fmj_mp = 769,
        barrett_heartbeat_mp = 770,
        barrett_silencer_mp = 771,
        barrett_thermal_mp = 772,
        barrett_xmags_mp = 773,
        barrett_acog_fmj_mp = 774,
        barrett_acog_heartbeat_mp = 775,
        barrett_acog_silencer_mp = 776,
        barrett_acog_xmags_mp = 777,
        barrett_fmj_heartbeat_mp = 778,
        barrett_fmj_silencer_mp = 779,
        barrett_fmj_thermal_mp = 780,
        barrett_fmj_xmags_mp = 781,
        barrett_heartbeat_silencer_mp = 782,
        barrett_heartbeat_thermal_mp = 783,
        barrett_heartbeat_xmags_mp = 784,
        barrett_silencer_thermal_mp = 785,
        barrett_silencer_xmags_mp = 786,
        barrett_thermal_xmags_mp = 787,
        wa2000_mp = 788,
        wa2000_acog_mp = 789,
        wa2000_fmj_mp = 790,
        wa2000_heartbeat_mp = 791,
        wa2000_silencer_mp = 792,
        wa2000_thermal_mp = 793,
        wa2000_xmags_mp = 794,
        wa2000_acog_fmj_mp = 795,
        wa2000_acog_heartbeat_mp = 796,
        wa2000_acog_silencer_mp = 797,
        wa2000_acog_xmags_mp = 798,
        wa2000_fmj_heartbeat_mp = 799,
        wa2000_fmj_silencer_mp = 800,
        wa2000_fmj_thermal_mp = 801,
        wa2000_fmj_xmags_mp = 802,
        wa2000_heartbeat_silencer_mp = 803,
        wa2000_heartbeat_thermal_mp = 804,
        wa2000_heartbeat_xmags_mp = 805,
        wa2000_silencer_thermal_mp = 806,
        wa2000_silencer_xmags_mp = 807,
        wa2000_thermal_xmags_mp = 808,
        m21_mp = 809,
        m21_acog_mp = 810,
        m21_fmj_mp = 811,
        m21_heartbeat_mp = 812,
        m21_silencer_mp = 813,
        m21_thermal_mp = 814,
        m21_xmags_mp = 815,
        m21_acog_fmj_mp = 816,
        m21_acog_heartbeat_mp = 817,
        m21_acog_silencer_mp = 818,
        m21_acog_xmags_mp = 819,
        m21_fmj_heartbeat_mp = 820,
        m21_fmj_silencer_mp = 821,
        m21_fmj_thermal_mp = 822,
        m21_fmj_xmags_mp = 823,
        m21_heartbeat_silencer_mp = 824,
        m21_heartbeat_thermal_mp = 825,
        m21_heartbeat_xmags_mp = 826,
        m21_silencer_thermal_mp = 827,
        m21_silencer_xmags_mp = 828,
        m21_thermal_xmags_mp = 829,
        cheytac_mp = 830,
        cheytac_acog_mp = 831,
        cheytac_fmj_mp = 832,
        cheytac_heartbeat_mp = 833,
        cheytac_silencer_mp = 834,
        cheytac_thermal_mp = 835,
        cheytac_xmags_mp = 836,
        cheytac_acog_fmj_mp = 837,
        cheytac_acog_heartbeat_mp = 838,
        cheytac_acog_silencer_mp = 839,
        cheytac_acog_xmags_mp = 840,
        cheytac_fmj_heartbeat_mp = 841,
        cheytac_fmj_silencer_mp = 842,
        cheytac_fmj_thermal_mp = 843,
        cheytac_fmj_xmags_mp = 844,
        cheytac_heartbeat_silencer_mp = 845,
        cheytac_heartbeat_thermal_mp = 846,
        cheytac_heartbeat_xmags_mp = 847,
        cheytac_silencer_thermal_mp = 848,
        cheytac_silencer_xmags_mp = 849,
        cheytac_thermal_xmags_mp = 850,
        ranger_mp = 851,
        ranger_akimbo_mp = 852,
        ranger_fmj_mp = 853,
        ranger_akimbo_fmj_mp = 854,
        model1887_mp = 855,
        model1887_akimbo_mp = 856,
        model1887_fmj_mp = 857,
        model1887_akimbo_fmj_mp = 858,
        striker_mp = 859,
        striker_eotech_mp = 860,
        striker_fmj_mp = 861,
        striker_grip_mp = 862,
        striker_reflex_mp = 863,
        striker_silencer_mp = 864,
        striker_xmags_mp = 865,
        striker_eotech_fmj_mp = 866,
        striker_eotech_grip_mp = 867,
        striker_eotech_silencer_mp = 868,
        striker_eotech_xmags_mp = 869,
        striker_fmj_grip_mp = 870,
        striker_fmj_reflex_mp = 871,
        striker_fmj_silencer_mp = 872,
        striker_fmj_xmags_mp = 873,
        striker_grip_reflex_mp = 874,
        striker_grip_silencer_mp = 875,
        striker_grip_xmags_mp = 876,
        striker_reflex_silencer_mp = 877,
        striker_reflex_xmags_mp = 878,
        striker_silencer_xmags_mp = 879,
        aa12_mp = 880,
        aa12_eotech_mp = 881,
        aa12_fmj_mp = 882,
        aa12_grip_mp = 883,
        aa12_reflex_mp = 884,
        aa12_silencer_mp = 885,
        aa12_xmags_mp = 886,
        aa12_eotech_fmj_mp = 887,
        aa12_eotech_grip_mp = 888,
        aa12_eotech_silencer_mp = 889,
        aa12_eotech_xmags_mp = 890,
        aa12_fmj_grip_mp = 891,
        aa12_fmj_reflex_mp = 892,
        aa12_fmj_silencer_mp = 893,
        aa12_fmj_xmags_mp = 894,
        aa12_grip_reflex_mp = 895,
        aa12_grip_silencer_mp = 896,
        aa12_grip_xmags_mp = 897,
        aa12_reflex_silencer_mp = 898,
        aa12_reflex_xmags_mp = 899,
        aa12_silencer_xmags_mp = 900,
        m1014_mp = 901,
        m1014_eotech_mp = 902,
        m1014_fmj_mp = 903,
        m1014_grip_mp = 904,
        m1014_reflex_mp = 905,
        m1014_silencer_mp = 906,
        m1014_xmags_mp = 907,
        m1014_eotech_fmj_mp = 908,
        m1014_eotech_grip_mp = 909,
        m1014_eotech_silencer_mp = 910,
        m1014_eotech_xmags_mp = 911,
        m1014_fmj_grip_mp = 912,
        m1014_fmj_reflex_mp = 913,
        m1014_fmj_silencer_mp = 914,
        m1014_fmj_xmags_mp = 915,
        m1014_grip_reflex_mp = 916,
        m1014_grip_silencer_mp = 917,
        m1014_grip_xmags_mp = 918,
        m1014_reflex_silencer_mp = 919,
        m1014_reflex_xmags_mp = 920,
        m1014_silencer_xmags_mp = 921,
        spas12_mp = 922,
        spas12_eotech_mp = 923,
        spas12_fmj_mp = 924,
        spas12_grip_mp = 925,
        spas12_reflex_mp = 926,
        spas12_silencer_mp = 927,
        spas12_xmags_mp = 928,
        spas12_eotech_fmj_mp = 929,
        spas12_eotech_grip_mp = 930,
        spas12_eotech_silencer_mp = 931,
        spas12_eotech_xmags_mp = 932,
        spas12_fmj_grip_mp = 933,
        spas12_fmj_reflex_mp = 934,
        spas12_fmj_silencer_mp = 935,
        spas12_fmj_xmags_mp = 936,
        spas12_grip_reflex_mp = 937,
        spas12_grip_silencer_mp = 938,
        spas12_grip_xmags_mp = 939,
        spas12_reflex_silencer_mp = 940,
        spas12_reflex_xmags_mp = 941,
        spas12_silencer_xmags_mp = 942,
        rpd_mp = 943,
        rpd_acog_mp = 944,
        rpd_eotech_mp = 945,
        rpd_fmj_mp = 946,
        rpd_grip_mp = 947,
        rpd_heartbeat_mp = 948,
        rpd_reflex_mp = 949,
        rpd_silencer_mp = 950,
        rpd_thermal_mp = 951,
        rpd_xmags_mp = 952,
        rpd_acog_fmj_mp = 953,
        rpd_acog_grip_mp = 954,
        rpd_acog_heartbeat_mp = 955,
        rpd_acog_silencer_mp = 956,
        rpd_acog_xmags_mp = 957,
        rpd_eotech_fmj_mp = 958,
        rpd_eotech_grip_mp = 959,
        rpd_eotech_heartbeat_mp = 960,
        rpd_eotech_silencer_mp = 961,
        rpd_eotech_xmags_mp = 962,
        rpd_fmj_grip_mp = 963,
        rpd_fmj_heartbeat_mp = 964,
        rpd_fmj_reflex_mp = 965,
        rpd_fmj_silencer_mp = 966,
        rpd_fmj_thermal_mp = 967,
        rpd_fmj_xmags_mp = 968,
        rpd_grip_heartbeat_mp = 969,
        rpd_grip_reflex_mp = 970,
        rpd_grip_silencer_mp = 971,
        rpd_grip_thermal_mp = 972,
        rpd_grip_xmags_mp = 973,
        rpd_heartbeat_reflex_mp = 974,
        rpd_heartbeat_silencer_mp = 975,
        rpd_heartbeat_thermal_mp = 976,
        rpd_heartbeat_xmags_mp = 977,
        rpd_reflex_silencer_mp = 978,
        rpd_reflex_xmags_mp = 979,
        rpd_silencer_thermal_mp = 980,
        rpd_silencer_xmags_mp = 981,
        rpd_thermal_xmags_mp = 982,
        sa80_mp = 983,
        sa80_acog_mp = 984,
        sa80_eotech_mp = 985,
        sa80_fmj_mp = 986,
        sa80_grip_mp = 987,
        sa80_heartbeat_mp = 988,
        sa80_reflex_mp = 989,
        sa80_silencer_mp = 990,
        sa80_thermal_mp = 991,
        sa80_xmags_mp = 992,
        sa80_acog_fmj_mp = 993,
        sa80_acog_grip_mp = 994,
        sa80_acog_heartbeat_mp = 995,
        sa80_acog_silencer_mp = 996,
        sa80_acog_xmags_mp = 997,
        sa80_eotech_fmj_mp = 998,
        sa80_eotech_grip_mp = 999,
        sa80_eotech_heartbeat_mp = 1000,
        sa80_eotech_silencer_mp = 1001,
        sa80_eotech_xmags_mp = 1002,
        sa80_fmj_grip_mp = 1003,
        sa80_fmj_heartbeat_mp = 1004,
        sa80_fmj_reflex_mp = 1005,
        sa80_fmj_silencer_mp = 1006,
        sa80_fmj_thermal_mp = 1007,
        sa80_fmj_xmags_mp = 1008,
        sa80_grip_heartbeat_mp = 1009,
        sa80_grip_reflex_mp = 1010,
        sa80_grip_silencer_mp = 1011,
        sa80_grip_thermal_mp = 1012,
        sa80_grip_xmags_mp = 1013,
        sa80_heartbeat_reflex_mp = 1014,
        sa80_heartbeat_silencer_mp = 1015,
        sa80_heartbeat_thermal_mp = 1016,
        sa80_heartbeat_xmags_mp = 1017,
        sa80_reflex_silencer_mp = 1018,
        sa80_reflex_xmags_mp = 1019,
        sa80_silencer_thermal_mp = 1020,
        sa80_silencer_xmags_mp = 1021,
        sa80_thermal_xmags_mp = 1022,
        mg4_mp = 1023,
        mg4_acog_mp = 1024,
        mg4_eotech_mp = 1025,
        mg4_fmj_mp = 1026,
        mg4_grip_mp = 1027,
        mg4_heartbeat_mp = 1028,
        mg4_reflex_mp = 1029,
        mg4_silencer_mp = 1030,
        mg4_thermal_mp = 1031,
        mg4_xmags_mp = 1032,
        mg4_acog_fmj_mp = 1033,
        mg4_acog_grip_mp = 1034,
        mg4_acog_heartbeat_mp = 1035,
        mg4_acog_silencer_mp = 1036,
        mg4_acog_xmags_mp = 1037,
        mg4_eotech_fmj_mp = 1038,
        mg4_eotech_grip_mp = 1039,
        mg4_eotech_heartbeat_mp = 1040,
        mg4_eotech_silencer_mp = 1041,
        mg4_eotech_xmags_mp = 1042,
        mg4_fmj_grip_mp = 1043,
        mg4_fmj_heartbeat_mp = 1044,
        mg4_fmj_reflex_mp = 1045,
        mg4_fmj_silencer_mp = 1046,
        mg4_fmj_thermal_mp = 1047,
        mg4_fmj_xmags_mp = 1048,
        mg4_grip_heartbeat_mp = 1049,
        mg4_grip_reflex_mp = 1050,
        mg4_grip_silencer_mp = 1051,
        mg4_grip_thermal_mp = 1052,
        mg4_grip_xmags_mp = 1053,
        mg4_heartbeat_reflex_mp = 1054,
        mg4_heartbeat_silencer_mp = 1055,
        mg4_heartbeat_thermal_mp = 1056,
        mg4_heartbeat_xmags_mp = 1057,
        mg4_reflex_silencer_mp = 1058,
        mg4_reflex_xmags_mp = 1059,
        mg4_silencer_thermal_mp = 1060,
        mg4_silencer_xmags_mp = 1061,
        mg4_thermal_xmags_mp = 1062,
        m240_mp = 1063,
        m240_acog_mp = 1064,
        m240_eotech_mp = 1065,
        m240_fmj_mp = 1066,
        m240_grip_mp = 1067,
        m240_heartbeat_mp = 1068,
        m240_reflex_mp = 1069,
        m240_silencer_mp = 1070,
        m240_thermal_mp = 1071,
        m240_xmags_mp = 1072,
        m240_acog_fmj_mp = 1073,
        m240_acog_grip_mp = 1074,
        m240_acog_heartbeat_mp = 1075,
        m240_acog_silencer_mp = 1076,
        m240_acog_xmags_mp = 1077,
        m240_eotech_fmj_mp = 1078,
        m240_eotech_grip_mp = 1079,
        m240_eotech_heartbeat_mp = 1080,
        m240_eotech_silencer_mp = 1081,
        m240_eotech_xmags_mp = 1082,
        m240_fmj_grip_mp = 1083,
        m240_fmj_heartbeat_mp = 1084,
        m240_fmj_reflex_mp = 1085,
        m240_fmj_silencer_mp = 1086,
        m240_fmj_thermal_mp = 1087,
        m240_fmj_xmags_mp = 1088,
        m240_grip_heartbeat_mp = 1089,
        m240_grip_reflex_mp = 1090,
        m240_grip_silencer_mp = 1091,
        m240_grip_thermal_mp = 1092,
        m240_grip_xmags_mp = 1093,
        m240_heartbeat_reflex_mp = 1094,
        m240_heartbeat_silencer_mp = 1095,
        m240_heartbeat_thermal_mp = 1096,
        m240_heartbeat_xmags_mp = 1097,
        m240_reflex_silencer_mp = 1098,
        m240_reflex_xmags_mp = 1099,
        m240_silencer_thermal_mp = 1100,
        m240_silencer_xmags_mp = 1101,
        m240_thermal_xmags_mp = 1102,
        aug_mp = 1103,
        aug_acog_mp = 1104,
        aug_eotech_mp = 1105,
        aug_fmj_mp = 1106,
        aug_grip_mp = 1107,
        aug_heartbeat_mp = 1108,
        aug_reflex_mp = 1109,
        aug_silencer_mp = 1110,
        aug_thermal_mp = 1111,
        aug_xmags_mp = 1112,
        aug_acog_fmj_mp = 1113,
        aug_acog_grip_mp = 1114,
        aug_acog_heartbeat_mp = 1115,
        aug_acog_silencer_mp = 1116,
        aug_acog_xmags_mp = 1117,
        aug_eotech_fmj_mp = 1118,
        aug_eotech_grip_mp = 1119,
        aug_eotech_heartbeat_mp = 1120,
        aug_eotech_silencer_mp = 1121,
        aug_eotech_xmags_mp = 1122,
        aug_fmj_grip_mp = 1123,
        aug_fmj_heartbeat_mp = 1124,
        aug_fmj_reflex_mp = 1125,
        aug_fmj_silencer_mp = 1126,
        aug_fmj_thermal_mp = 1127,
        aug_fmj_xmags_mp = 1128,
        aug_grip_heartbeat_mp = 1129,
        aug_grip_reflex_mp = 1130,
        aug_grip_silencer_mp = 1131,
        aug_grip_thermal_mp = 1132,
        aug_grip_xmags_mp = 1133,
        aug_heartbeat_reflex_mp = 1134,
        aug_heartbeat_silencer_mp = 1135,
        aug_heartbeat_thermal_mp = 1136,
        aug_heartbeat_xmags_mp = 1137,
        aug_reflex_silencer_mp = 1138,
        aug_reflex_xmags_mp = 1139,
        aug_silencer_thermal_mp = 1140,
        aug_silencer_xmags_mp = 1141,
        aug_thermal_xmags_mp = 1142,
        c4_mp = 1143,
        claymore_mp = 1144,
        airdrop_marker_mp = 1145,
        semtex_mp = 1146,
        frag_grenade_mp = 1147,
        flash_grenade_mp = 1148,
        smoke_grenade_mp = 1149,
        concussion_grenade_mp = 1150,
        throwingknife_mp = 1151,
        onemanarmy_mp = 1152,
        flare_mp = 1153,
        scavenger_bag_mp = 1154,
        frag_grenade_short_mp = 1155,
        briefcase_bomb_mp = 1157,
        briefcase_bomb_defuse_mp = 1158,
        killstreak_uav_mp = 1159,
        killstreak_helicopter_mp = 1160,
        killstreak_ac130_mp = 1161,
        killstreak_predator_missile_mp = 1162,
        killstreak_helicopter_minigun_mp = 1163,
        killstreak_nuke_mp = 1164,
        killstreak_precision_airstrike_mp = 1165,
        killstreak_counter_uav_mp = 1166,
        killstreak_sentry_mp = 1167,
        airdrop_sentry_marker_mp = 1168,
        killstreak_helicopter_flares_mp = 1169,
        killstreak_emp_mp = 1170,
        airdrop_mega_marker_mp = 1171,
        killstreak_stealth_airstrike_mp = 1172,
        killstreak_harrier_airstrike_mp = 1173,
        ac130_25mm_mp = 1174,
        ac130_40mm_mp = 1175,
        ac130_105mm_mp = 1176,
        remotemissile_projectile_mp = 1177,
        stealth_bomb_mp = 1178,
        artillery_mp = 1179,
        harrier_missile_mp = 1180,
        harrier_20mm_mp = 1181,
        harrier_ffar_mp = 1182,
        heli_remote_mp = 1187,
        pavelow_minigun_mp = 1188,
        nuke_mp = 1190,
        barrel_mp = 1191,
        lightstick_mp = 1192,
        throwingknife_rhand_mp = 1193
    }
    public enum Weapon_Camos
    {
        None = 0,
        Woodland = 1,
        Digital = 2,
        Desert = 3,
        Arctic = 4,
        Urban = 5,
        Red_Tiger = 6,
        Blue_Tiger = 7,
        Fall = 8
    }

    private void RemoveWeap()
    {
        PS3.SetMemory(0x014e2422 + ((uint)dataGridView3.CurrentRow.Index * 0x3700), new byte[] { 0x04, 0x95, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x95, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x22, 0x0F, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
    }
    private void DoInfAmmo()
    {
        int index = dataGridView3.CurrentRow.Index;
        byte[] val = new byte[] { 0x15, 0xff, 0xff, 0xff };
        PS3.SetMemory(0x14e24ec + (uint)index * 0x3700, val);
        PS3.SetMemory(0x14e24dc + (uint)index * 0x3700, val);
        PS3.SetMemory(0x14e2554 + (uint)index * 0x3700, val);
        PS3.SetMemory(0x14e256c + (uint)index * 0x3700, val);
        PS3.SetMemory(0x14e2560 + (uint)index * 0x3700, val);
        PS3.SetMemory(0x14e2578 + (uint)index * 0x3700, val);
    }

    #endregion

    #region Attachments
    string[] amgAttach() { return new string[] { "Acog", "Holographic Sight", "FMJ", "Grenade Launcher", "Heartbeat Sensor", "Red Dot Sight", "Shotgun", "Silencer", "Thermal", "Extended Mags", "Acog + FMJ", "Acog + Grenade Launcher", "Acog + Heartbeat Sensor", "Acog + Shotgun", "Acog + Silencer", "Acog + Extended Mags", "Holographic Sight + FMJ", "Holographic Sight + Grenade Launcher", "Holographic Sight + Heartbeat Sensor", "Holographic Sight + Shotgun", "Holographic Sight + Silencer", "Holographic Sight + Extended Mags", "FMJ + Grenade Launcher", "FMJ + Heartbeat Sensor", "FMJ + Red Dot Sight", "FMJ + Shotgun", "FMJ + Silencer", "FMJ + Thermal", "FMJ + Extended Mags", "Grenade Launcher + Heartbeat Sensor", "Grenade Launcher + Red Dot Sight", "Grenade Launcher + Silencer", "Grenade Launcher + Thermal", "Grenade Launcher + Extended Mags", "Heartbeat Sensor + Red Dot Sight", "Heartbeat Sensor + Shotgun", "Heartbeat Sensor + Silencer", "Heartbeat Sensor + Thermal", "Heartbeat Sensor + Extended Mags", "Red Dot Sight + Shotgun", "Red Dot Sight + Silencer", "Red Dot Sight + Extended Mags", "Shotgun + Silencer", "Shotgun + Thermal", "Shotgun + Extended Mags", "Silencer + Thermal", "Silencer + Extended Mags", "Thermal + Extended Mags" }; }
    string[] smgAttach() { return new string[] { "Acog", "Akimbo", "Holographic Sight", "FMJ", "Red Dot Sight", "Rapid Fire", "Silencer", "Thermal", "Extended Mags", "Acog + FMJ", "Acog + Rapid Fire", "Acog + Silencer", "Acog + Extended Mags", "Akimbo + FMJ", "Akimbo + Rapid Fire", "Akimbo + Silencer", "Akimbo + Extended Mags", "Holographic Sight + FMJ", "Holographic Sight + Rapid Fire", "Holographic Sight + Silencer", "Holographic Sight + Extended Mags", "FMJ + Red Dot Sight", "FMJ + Rapid Fire", "FMJ + Silencer", "FMJ + Thermal", "FMJ + Extended Mags", "Red Dot Sight + Rapid Fire", "Red Dot Sight + Silencer", "Red Dot Sight + Extended Mags", "Rapid Fire + Silencer", "Rapid Fire + Thermal", "Rapid Fire + Extended Mags", "Silencer + Thermal", "Silencer + Extended Mags", "Thermal + Extended Mags" }; }
    string[] lmgAttach() { return new string[] { "Acog", "Holographic Sight", "FMJ", "Grip", "Heartbeat Sensor", "Red Dot Sight", "Silencer", "Thermal", "Extended Mags", "Acog + FMJ", "Acog + Grip", "Acog + Heartbeat Sensor", "Acog + Silencer", "Acog + Extended Mags", "Holographic Silencer + FMJ", "Holographic Silencer + Grip", "Holographic Silencer + Heartbeat Sensor", "Holographic Silencer + Silencer", "Holographic Silencer + Extended Mags", "FMJ + Grip", "FMJ + Heartbeat Sensor", "FMJ + Red Dot Sight", "FMJ + Silencer", "FMJ + Thermal", "FMJ + Extended Mags", "Grip + Heartbeat Sensor", "Grip + Red Dot Sight", "Grip + Silencer", "Grip + Thermal", "Grip + Extended Mags", "Heartbeat Sensor + Red Dot Sight", "Heartbeat Sensor + Silencer", "Heartbeat Sensor + Thermal", "Heartbeat Sensor + Extended Mags", "Red Dot Sight + Silencer", "Red Dot Sight + Extended Mags", "Silencer + Thermal", "Silencer + Extended Mags", "Thermal + Extended Mags" }; }
    string[] snipeAttach() { return new string[] { "Acog", "FMJ", "Heartbeat Sensor", "Silencer", "Thermal", "Extended Mags", "Acog + FMJ", "Acog + Heartbeat Sensor", "Acog + Silencer", "Acog + Extended Mags", "FMJ + Heartbeat Sensor", "FMJ + Silencer", "FMJ + Thermal", "FMJ + Extended Mags", "Heartbeat Sensor + Silencer", "Heartbeat Sensor + Thermal", "Heartbeat Sensor + Extended Mags", "Silencer + Thermal", "Silencer + Extended Mags", "Thermal + Extended Mags" }; }

    string[] mpAttach() { return new string[] { "Akimbo", "Holographic Sight", "FMJ", "Red Dot Sight", "Silencer", "Extended Mags", "Akimbo + FMJ", "Akimbo + Silencer", "Akimbo + Extended Mags", "Holographic Sight + FMJ", "Holographic Sight + Silencer", "Holographic Sight + Extended Mags", "FMJ + Red Dot Sight", "FMJ + Silencer", "FMJ + Extended Mags", "Red Dot Sight + Silencer", "Red Dot Sight + Extended Mags", "Silencer + Extended Mags" }; }
    string[] shotgunAttach() { return new string[] { "Holographic Sight", "FMJ", "Grip", "Red Dot Sight", "Silencer", "Extended Mags", "Holographic Sight + FMJ", "Holographic Sight + Grip", "Holographic Sight + Silencer", "Holographic Sight + Extended Mags", "FMJ + Grip", "FMJ + Red Dot Sight", "FMJ + Silencer", "FMJ + Extended Mags", "Grip + Red Dot Sight", "Grip + Silencer", "Grip + Extended Mags", "Red Dot Sight + Silencer", "Red Dot Sight + Extended Mags", "Silencer + Extended Mags" }; }
    string[] shotgunSpecialAttach() { return new string[] { "Akimbo", "FMJ", "Akimbo + FMJ" }; }
    string[] hgAttach() { return new string[] { "Akimbo", "FMJ", "Silencer", "Tactical", "Extended Mags", "Akimbo + FMJ", "Akimbo + Silencer", "Akimbo + Extended Mags", "FMJ + Silencer", "FMJ + Tactical", "FMJ + Extended Mags", "Silencer + Tactical", "Silencer + Extended Mags", "Tactical + Extended Mags" }; }
    string[] hgSpecialAttach() { return new string[] { "Akimbo", "FMJ", "Tactical", "Akimbo + FMJ", "FMJ + Tactical" }; }

    string[] camoAttach() { return new string[] { "None", "Woodland", "Desert", "Acrtic", "Digital", "Urban", "Red Tiger", "Blue Tiger", "Fall" }; }
    #endregion

    #region Primary
    private void colorComboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        colorComboBox14.Items.Clear();
        colorComboBox15.Items.Clear();
        colorComboBox16.Items.Clear();
        camNumPrim = 0;
        weapNumPrim = 0;

        int index = colorComboBox1.SelectedIndex;
        string[] comboBoxStr = { };
        if (index == 0) comboBoxStr = new string[] { "M4A1", "Famas", "Scar-H", "Tar-21", "Fal", "M16A4", "ACR", "F2000", "AK47" };
        else if (index == 1) comboBoxStr = new string[] { "MP5K", "UMP45", "Vector", "P90", "Mini-Uzi" };
        else if (index == 2) comboBoxStr = new string[] { "L86 LSW", "RPD", "MG4", "Aug HBAR", "M240" };
        else if (index == 3) comboBoxStr = new string[] { "Intervention", "Barrett .50cal", "WA2000", "M21 EBR" };
        else if (index == 4) weapNumPrim = 2;
        colorComboBox14.Items.AddRange(comboBoxStr);
    }

    private void colorComboBox15_SelectedIndexChanged(object sender, EventArgs e)
    {
        weapNumPrim = savePrim;
        weapNumPrim += colorComboBox15.SelectedIndex + 1;
    }

    private void colorComboBox14_SelectedIndexChanged(object sender, EventArgs e)
    {
        colorComboBox15.Items.Clear();
        colorComboBox16.Items.Clear();
        colorComboBox16.Items.AddRange(camoAttach());
        if (colorComboBox1.SelectedIndex == 0)
        {
            colorComboBox15.Items.AddRange(amgAttach());
            if (colorComboBox14.SelectedIndex == 0) weapNumPrim = 404;
            else if (colorComboBox14.SelectedIndex == 1) weapNumPrim = 557;
            else if (colorComboBox14.SelectedIndex == 2) weapNumPrim = 659;
            else if (colorComboBox14.SelectedIndex == 3) weapNumPrim = 710;
            else if (colorComboBox14.SelectedIndex == 4) weapNumPrim = 608;
            else if (colorComboBox14.SelectedIndex == 5) weapNumPrim = 353;
            else if (colorComboBox14.SelectedIndex == 6) weapNumPrim = 506;
            else if (colorComboBox14.SelectedIndex == 7) weapNumPrim = 455;
            else if (colorComboBox14.SelectedIndex == 8) weapNumPrim = 302;
        }
        if (colorComboBox1.SelectedIndex == 1)
        {
            colorComboBox15.Items.AddRange(smgAttach());
            if (colorComboBox14.SelectedIndex == 0) weapNumPrim = 122;
            else if (colorComboBox14.SelectedIndex == 1) weapNumPrim = 266;
            else if (colorComboBox14.SelectedIndex == 2) weapNumPrim = 230;
            else if (colorComboBox14.SelectedIndex == 3) weapNumPrim = 194;
            else if (colorComboBox14.SelectedIndex == 4) weapNumPrim = 158;
        }
        if (colorComboBox1.SelectedIndex == 2)
        {
            colorComboBox15.Items.AddRange(lmgAttach());
            if (colorComboBox14.SelectedIndex == 0) weapNumPrim = 983;
            else if (colorComboBox14.SelectedIndex == 1) weapNumPrim = 943;
            else if (colorComboBox14.SelectedIndex == 2) weapNumPrim = 1023;
            else if (colorComboBox14.SelectedIndex == 3) weapNumPrim = 1103;
            else if (colorComboBox14.SelectedIndex == 4) weapNumPrim = 1063;
        }
        if (colorComboBox1.SelectedIndex == 3)
        {
            colorComboBox15.Items.AddRange(snipeAttach());
            if (colorComboBox14.SelectedIndex == 0) weapNumPrim = 830;
            else if (colorComboBox14.SelectedIndex == 1) weapNumPrim = 767;
            else if (colorComboBox14.SelectedIndex == 2) weapNumPrim = 788;
            else if (colorComboBox14.SelectedIndex == 3) weapNumPrim = 809;
        }
        savePrim = weapNumPrim;
    }

    private void colorComboBox16_SelectedIndexChanged(object sender, EventArgs e)
    {
        camNumPrim = colorComboBox16.SelectedIndex;
    }
    #endregion

    #region Secondary
    private void colorComboBox20_SelectedIndexChanged(object sender, EventArgs e)
    {
        colorComboBox17.Items.Clear();
        colorComboBox18.Items.Clear();
        colorComboBox19.Items.Clear();
        camNumSec = 0;
        weapNumSec = 0;

        int index = colorComboBox20.SelectedIndex;
        string[] comboBoxStr = { };
        if (index == 0) comboBoxStr = new string[] { "PP2000", "G18", "M93 Raffica", "TMP" };
        else if (index == 1) comboBoxStr = new string[] { "Spas-12", "AA-12", "Striker", "Ranger", "M1014", "Model 1887" };
        else if (index == 2) comboBoxStr = new string[] { "USP .45", ".44 Magum", "M9", "Desert Eagle" };
        else if (index == 3) comboBoxStr = new string[] { "AT4-HS", "Thumper", "Stinger", "Javelin", "RPG-7" };
        colorComboBox19.Items.AddRange(comboBoxStr);
    }

    private void colorComboBox18_SelectedIndexChanged(object sender, EventArgs e)
    {
        weapNumSec = saveSec;
        weapNumSec += colorComboBox18.SelectedIndex + 1;
    }

    private void colorComboBox19_SelectedIndexChanged(object sender, EventArgs e)
    {
        colorComboBox17.Items.Clear();
        colorComboBox18.Items.Clear();
        colorComboBox17.Items.AddRange(camoAttach());

        if (colorComboBox20.SelectedIndex == 0)
        {
            colorComboBox18.Items.AddRange(mpAttach());
            if (colorComboBox19.SelectedIndex == 0) weapNumSec = 84;
            else if (colorComboBox19.SelectedIndex == 1) weapNumSec = 46;
            else if (colorComboBox19.SelectedIndex == 2) weapNumSec = 65;
            else if (colorComboBox19.SelectedIndex == 3) weapNumSec = 103;
        }

        if (colorComboBox20.SelectedIndex == 1)
        {
            if (colorComboBox19.SelectedIndex == 0)
            {
                colorComboBox18.Items.AddRange(shotgunAttach());
                weapNumSec = 922;
            }
            else if (colorComboBox19.SelectedIndex == 1)
            {
                colorComboBox18.Items.AddRange(shotgunAttach());
                weapNumSec = 880;
            }
            else if (colorComboBox19.SelectedIndex == 2)
            {
                colorComboBox18.Items.AddRange(shotgunAttach());
                weapNumSec = 859;
            }
            else if (colorComboBox19.SelectedIndex == 3)
            {
                colorComboBox18.Items.AddRange(shotgunSpecialAttach());
                weapNumSec = 851;
            }
            else if (colorComboBox19.SelectedIndex == 4)
            {
                colorComboBox18.Items.AddRange(shotgunAttach());
                weapNumSec = 901;
            }
            else if (colorComboBox19.SelectedIndex == 5)
            {
                colorComboBox18.Items.AddRange(shotgunSpecialAttach());
                weapNumSec = 855;
            }
        }

        if (colorComboBox20.SelectedIndex == 2)
        {
            if (colorComboBox19.SelectedIndex == 0)
            {
                colorComboBox18.Items.AddRange(hgAttach());
                weapNumSec = 18;
            }
            else if (colorComboBox19.SelectedIndex == 1)
            {
                colorComboBox18.Items.AddRange(hgSpecialAttach());
                weapNumSec = 3;
            }
            else if (colorComboBox19.SelectedIndex == 2)
            {
                colorComboBox18.Items.AddRange(hgAttach());
                weapNumSec = 39;
            }
            else if (colorComboBox19.SelectedIndex == 3)
            {
                colorComboBox18.Items.AddRange(hgSpecialAttach());
                weapNumSec = 33;
            }
        }
        if (colorComboBox20.SelectedIndex == 3)
        {
            if (colorComboBox19.SelectedIndex == 0) weapNumSec = 764;
            else if (colorComboBox19.SelectedIndex == 1) weapNumSec = 761;
            else if (colorComboBox19.SelectedIndex == 2) weapNumSec = 765;
            else if (colorComboBox19.SelectedIndex == 3) weapNumSec = 766;
            else if (colorComboBox19.SelectedIndex == 4) weapNumSec = 763;
        }
        saveSec = weapNumSec;
    }

    private void colorComboBox17_SelectedIndexChanged(object sender, EventArgs e)
    {
        camNumSec = colorComboBox17.SelectedIndex;
    }
    #endregion

    #endregion
    #region Infections
    string[] infections = new string[] {
        "^5CFG Infection ^7;bind BUTTON_BACK exec ../../../dev_usb000/buttons_default.cfg",
        "^5UAV + Strong Aimassist ^7;bind APAD_LEFT g_compassShowEnemies 1;bind APAD_RIGHT aim_autoaim_enabled 2;aim_lockon_region_height 480;aim_lockon_region_width 640;aim_lockon_enabled 1;aim_lockon_strength 1;aim_lockon_deflection 0;aim_autoaim_enabled 1;aim_autoaim_region_height 480;aim_autoaim_region_width 640;aim_slowdown_yaw_scale_ads 0;aim_slowdown_yaw_scale 0;aim_slowdown_pitch_scale 0;aim_slowdown_pitch_scale_ads 0;aim_slowdown_region_height 0;aim_slowdown_region_width 0;aim_slowdown_enabled 1;SelectStringTableEntryInDvar M M aim_autoAimRangeScale; SelectStringTableEntryInDvar M M aim_aimAssistRangeScale; set cg_drawfps 2;set perk_weapSpreadMultiplier 0.0001",
        "^511th Prestige ^7;disconnect;setplayerdata prestige 11;setplayerdata experience 2516000;uploadStats",
        "^510th Prestige ^7;disconnect;setplayerdata prestige 10;setplayerdata experience 2516000;uploadStats",
        "^59th Prestige ^7;disconnect;setplayerdata prestige 9;setplayerdata experience 2516000;uploadStats",
        "^5Mini-Menu ^7;bind APAD_LEFT bind DPAD_UP vstr 1;set 1 \\\"\"set g_compassShowEnemies 1; set cg_drawfps 2; set clanname Class 1;say ^1Infectable Mini-^5Menu;say ^1>^6Super_Jump^1<;say ^5Timescale;say ^5Gravity;bind dpad_up vstr 3;bind dpad_down vstr 2;bind dpad_left vstr e; bind button_a toggle jump_height 39 150 999; set 2 \\\"\"say ^5Infectable Mini-^5Menu;say ^5Super_Jump;say ^1>^6Timescale^1<;say ^5Gravity;bind dpad_up vstr 1;bind dpad_down vstr 3;bind button_a toggle timescale 0.5 1 1.5; set 3 \\\"\"say ^5Infectable Mini-^5Menu;say ^5Super_Jump;say ^5Timescale;say ^1>^6Gravity^1<;bind dpad_up vstr 2;bind button_a toggle g_gravity 200 800 3000; set e \\\"\"say ^7;say ^7;say ^7;say ^1MENU ^5CLOSED;say ^1;say ^1;say ^1;say ^1;bind dpad_up vstr 1;bind dpad_left +actionslot 3;bind BUTTON_A +gostand",
        "^5GodMode ^7;bind dpad_down toggle xblive_privatematch 0 1; toggle xblive_rankedmatch 1 0; bind APAD_DOWN g_compassShowEnemies 1",
        "^5Ump45 Unlocks; ^7disconnect;setPlayerData challengeState ch_expert_ump45 9;setPlayerData challengeState ch_marksman_ump45 9;setPlayerData challengeState ch_ump45_acog 2;setPlayerData challengeState ch_ump45_fmj 2;setPlayerData challengeState ch_ump45_mastery 2;setPlayerData challengeState ch_ump45_reflex 2;setPlayerData challengeState ch_ump45_rof 2;setPlayerData challengeState pr_expert_ump45 4;setPlayerData challengeState pr_marksman_ump45 4;uploadstats",
        "^5Sniper Unlocks ^7;disconnect;setPlayerData challengeState ch_marksman_barrett 9;setPlayerData challengeState ch_expert_barrett 9;setPlayerData challengeState ch_barrett_silencer 2;setPlayerData challengeState ch_barrett_acog 2;setPlayerData challengeState ch_barrett_fmj 2;setPlayerData challengeState ch_barrett_mastery 2;setPlayerData challengeState ch_marksman_cheytac 9;setPlayerData challengeState ch_expert_cheytac 9;setPlayerData challengeState ch_cheytac_silencer 2;setPlayerData challengeState ch_cheytac_acog 2;setPlayerData challengeState ch_cheytac_fmj 2;setPlayerData challengeState ch_cheytac_mastery 2;setPlayerData challengeState pr_marksman_cheytac 4;setPlayerData challengeState pr_expert_cheytac 4;setPlayerData challengeState pr_marksman_barrett 4;setPlayerData challengeState pr_expert_barrett 4;uploadStats",
        "^5Pistol Unlocks ^7;disconnect;setPlayerData challengeState ch_marksman_usp 9;setPlayerData challengeState ch_expert_usp 9;setPlayerData challengeState pr_marksman_usp 4;setPlayerData challengeState pr_expert_usp 4;setPlayerData challengeState ch_marksman_beretta 9;setPlayerData challengeState ch_expert_beretta 9;setPlayerData challengeState pr_marksman_beretta 4;setPlayerData challengeState pr_expert_beretta 4;setPlayerData challengeState ch_marksman_coltanaconda 7;setPlayerData challengeState ch_expert_coltanaconda 9;setPlayerData challengeState pr_marksman_coltanaconda 4;setPlayerData challengeState pr_expert_coltanaconda 4;setPlayerData challengeState ch_marksman_deserteagle 7;setPlayerData challengeState ch_expert_deserteagle 9;setPlayerData challengeState pr_marksman_deserteagle 4;setPlayerData challengeState pr_expert_deserteagle 4;uploadstats",
        "^5Pro Perks ^7;disconnect;setPlayerData challengeState ch_bling_pro 7;setPlayerData challengeState ch_bulletaccuracy_pro 7;setPlayerData challengeState ch_coldblooded_pro 7;setPlayerData challengeState ch_dangerclose_pro 7;setPlayerData challengeState ch_deadsilence_pro 7;setPlayerData challengeState ch_detectexplosives_pro 7;setPlayerData challengeState ch_extendedmelee_pro 7;setPlayerData challengeState ch_hardline_pro 7;setPlayerData challengeState ch_laststand_pro 7;setPlayerData challengeState ch_lightweight_pro 7;setPlayerData challengeState ch_marathon_pro 7;setPlayerData challengeState ch_onemanarmy_pro 7;setPlayerData challengeState ch_scavenger_pro 7;setPlayerData challengeState ch_scrambler_pro 7;setPlayerData challengeState ch_sleightofhand_pro 7;setPlayerData challengeState ch_stoppingpower_pro 7",
    };

    private void button71_Click(object sender, EventArgs e)
    {
        if (colorRadioButton35.Checked)
        {
            RPC.cBuff_AddText_Reg("ui_mapname \\\"\"mp_rust;" + infections[colorComboBox22.SelectedIndex]);
            PS3.SetMemory(prgOfs, BitConverter.GetBytes(new Random().Next(0, 12)));
        }
        else if (colorRadioButton77.Checked)
        {
            RPC.SV_GameSendServerCommand(-1, "v activeaction \"" + infections[colorComboBox22.SelectedIndex] + "\"");
            RPC.cBuff_AddText_Reg("map_restart");
        }
    }

    private void button51_Click(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_Reg("set doInfection \"ui_mapname \\\"\"mp_rust;^7" + richTextBox1.Text + "\";vstr doInfection");
        PS3.SetMemory(prgOfs, BitConverter.GetBytes(new Random().Next(0, 12)));
    }

    private void button52_Click(object sender, EventArgs e)
    {
        RPC.cBuff_AddText_Reg("ui_gametype ^7" + richTextBox2.Text);
        PS3.SetMemory(prgOfs, BitConverter.GetBytes(new Random().Next(0, 12)));
    }
    #endregion
    #region RTE Zones
    string[] zoneDftText = { "Play Online", "Split Screen", "Lan Party", "Friend List", "Options", "Main Menu", "New Maps", "PS Network Title", "Find Game", "Private Match", "Create a Class", "Callsign / Killstreaks", "Barracks", "Invite", "Private Match Title", "Start Game", "Game Setup", "Vote to Skip", "Invitation Subject", "Invitation to Lobby", "Invitation to Party", "Invitation to Game", "Game Summary", "Back" };
    int[] zoneTextLen = { 26, 22, 26, 26, 18, 20, 53, 21, 24, 24, 25, 21, 19, 17, 25, 21, 21, 23, 16, 33, 33, 32, 27, 19 };
    uint[] zoneOft = { 0x319D00B6, 0x319D06DC, 0x319D0B44, 0x319D0F64, 0x319D1438, 0x319D1755, 0x31B0FBC2, 0x31AC1B1C, 0x31AC21D8, 0x31AC26ED, 0x31AC2D79, 0x31AC3F60, 0x31AC4ABE, 0x31AC54F1, 0x31AF92E4, 0x31AFA3BC, 0x31AFA85D, 0x31AE1718, 0x31B13B56, 0x31B10D65, 0x31B10D9C, 0x31B10DD3, 0x31ACC56C, 0x31A8C9F4 };
    string[] fThemeText = { "Go Online", "Dual A Friend", "Local Network", "Show Friends", "Game Options", "Quit Multiplayer", "You are using Furious V2!    Created by MayhemModding", "Furious V2", "Find A Game To Mod", "Custom Game", "Custom Classes", "Callsigns + Killstreaks", "View Barracks", "Invite A Friend", "Furious V2", "Ready To Mod!", "Setup Your Game", "Skip This Map", "[> INVITE <]", "Join My Lobby. Using Furious V2!", "Join My Party. Using Furious V2!", "Join My Game. Using Furious V2!", "View Aftermath [{weapnext}]", "Go Back [{+stance}]" };
    string[] zoneColorDftText = { "Main", "Party", "Private Match", "Searching Online", "Choose Maps", "Choose Bonus Maps", "Barracks Online", "Barracks Offline" };
    uint[] zoneColorOfs = { 0x319CEBD4, 0x319CE8BC, 0x319CE6D8, 0x31AC1264, 0x31AC1090, 0x31AC0EC4, 0x31AF8A2C, 0x31AF8858, 0x31AF868C, 0x31ADE7D4, 0x31ADE600, 0x31ADE434, 0x31AB1D9C, 0x31AB1BC8, 0x31AB19FC, 0x31AB9D08, 0x31AB9B34, 0x31AB9968, 0x31A89718, 0x31A89544, 0x31A89378, 0x31A961AC, 0x31A95FD8, 0x31A95E0C };
    string zoneTheme = "";
    string[] zoneColor = new string[24];
    ColorDialog zoneRGB = new ColorDialog();
    List<byte[]> defaultText = new List<byte[]>();

    private void getDefaultText(uint ofs)
    {
        byte[] read = PS3.Extension.ReadBytes(ofs, 100);
        List<byte> write = new List<byte>();
        for (int i = 0; i < 100; i++)
        {
            write.Add(read[i]);
            if (read[i] == 0x00)
                break;
        }
        defaultText.Add(write.ToArray());
    }

    private void updateZoneTheme()
    {
        if (zoneTheme == "furious")
            for (int i = 0; i < 24; i++)
                PS3.SetMemory(zoneColorOfs[i], ConvertHexToBytes(RGB2HEX(selectedTheme)));
    }

    int[] zoneThemeRGB = new int[] { 255, 0, 0 };
    private void zoneColorProcess()
    {
        if (zoneTheme == "rainbow")
            for (int i = 0; i < 24; i++)
                PS3.SetMemory(zoneColorOfs[i], ConvertHexToBytes(RGB2HEX(rainbowRGB(zoneThemeRGB, 1))));
    }

    private void button53_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < 24; i++)
        {
            string checkLen = (string)dataGridView4.Rows[i].Cells[1].Value;
            if (checkLen != null)
            {
                if (checkLen.Length > zoneTextLen[i]) { checkLen = checkLen.Remove(zoneTextLen[i], checkLen.Length - zoneTextLen[i]); }
                dataGridView4.Rows[i].Cells[1].Value = checkLen;
                int leftSpace = zoneTextLen[i] - checkLen.Length;
                string addSpace = "";
                for (int x = 0; x < leftSpace; x++)
                {
                    addSpace += " ";
                }

                if (i >= 21)
                {
                    PS3.Extension.WriteBytes(zoneOft[i], Encoding.ASCII.GetBytes(checkLen + addSpace + "\0"));
                }
                else
                {
                    PS3.Extension.WriteBytes(zoneOft[i], Encoding.ASCII.GetBytes(addSpace + checkLen + "\0"));
                }
            }
        }
    }

    private void button72_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < zoneColor.Length; i++)
            PS3.SetMemory(zoneColorOfs[i], ConvertHexToBytes(zoneColor[i]));
    }

    private void button55_Click(object sender, EventArgs e)
    {
        SaveFileDialog save = new SaveFileDialog();
        save.Title = "Save Zone Text";
        save.FileName = "Furious Zone Text";
        save.Filter = "Furious Zone Text Files (*.fzt)|*.fzt";
        if (save.ShowDialog() == DialogResult.OK)
        {
            string[] saveText = new string[24];
            for (int i = 0; i < saveText.Length; i++)
            {
                string val = (string)dataGridView4[1, i].Value;
                saveText[i] = val;
            }
            File.WriteAllLines(save.FileName, saveText);
        }
    }

    private void button57_Click(object sender, EventArgs e)
    {
        OpenFileDialog open = new OpenFileDialog();
        open.Title = "Open Zone Text";
        open.FileName = "Furious Zone Text";
        open.Filter = "Furious Zone Text Files (*.fzt)|*.fzt";
        if (open.ShowDialog() == DialogResult.OK)
        {
            string[] openText = File.ReadAllLines(open.FileName);
            for (int i = 0; i < openText.Length; i++)
            {
                if (openText[i] != "")
                    dataGridView4[1, i].Value = openText[i];
            }
        }
    }


    private void button56_Click(object sender, EventArgs e)
    {
        SaveFileDialog save = new SaveFileDialog();
        save.Title = "Save Zone Color";
        save.FileName = "Furious Zone Color";
        save.Filter = "Furious Zone Color Files (*.fzc)|*.fzc";
        if (save.ShowDialog() == DialogResult.OK)
        {
            File.WriteAllLines(save.FileName, zoneColor);
        }
    }

    private void button58_Click(object sender, EventArgs e)
    {
        OpenFileDialog open = new OpenFileDialog();
        open.Title = "Open Zone Color";
        open.FileName = "Furious Zone Color";
        open.Filter = "Furious Zone Color Files (*.fzc)|*.fzc";
        if (open.ShowDialog() == DialogResult.OK)
        {
            zoneColor = File.ReadAllLines(open.FileName);
        }
    }

    private void dataGridView5_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        var senderGrid = (DataGridView)sender;

        if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.ColumnIndex != 0 && e.RowIndex != -1)
        {
            zoneRGB.FullOpen = true;
            if (zoneRGB.ShowDialog() == DialogResult.OK)
            {
                int math = e.RowIndex * 3 + e.ColumnIndex - 1;
                zoneColor[math] = RGB2HEX(zoneRGB.Color);
                PS3.SetMemory(zoneColorOfs[math], ConvertHexToBytes(RGB2HEX(zoneRGB.Color)));
            }
        }
    }

    private void colorRadioButton50_CheckedChanged(object sender, EventArgs e)
    {
        zoneTheme = "";
    }

    private void colorRadioButton51_CheckedChanged(object sender, EventArgs e)
    {
        zoneTheme = "furious";
        updateZoneTheme();
        for (int i = 0; i < 24; i++)
            PS3.SetMemory(zoneOft[i], Encoding.ASCII.GetBytes(fThemeText[i] + "\0"));
    }

    private void colorRadioButton52_CheckedChanged(object sender, EventArgs e)
    {
        zoneTheme = "rainbow";
    }
    private void button74_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < defaultText.Count; i++)
            PS3.SetMemory(zoneOft[i], defaultText[i]);
    }

    private void button73_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < zoneColor.Length; i++)
        {
            zoneColor[i] = "3F8000003F8000003F8000003F800000";
            PS3.SetMemory(zoneColorOfs[i], ConvertHexToBytes(zoneColor[i]));
        }
    }
    #endregion
    #region Get Info
    public static uint[] infoPartyOfs = { 0x00a11460, 0x00a129a2, 0x00a114c8 };
    public static uint[] infoGameOfs = { 0x00a135f8, 0x00a135ea, 0x00a13660 };
    public static uint[] infoOfs = new uint[4];
    string[] addNames = new string[18];
    string[] addIps = new string[18];
    string[] addLocalIps = new string[18];
    string[] addXUID = new string[18];
    string[] infoLine = new string[18];
    bool inParty()
    {
        return PS3.Extension.ReadBool(0x00a12a7b);
    }
    private static string convertIp(byte[] ip)
    {
        return string.Format("{0}.{1}.{2}.{3}", ip[0], ip[1], ip[2], ip[3]);
    }

    private void updateInfo()
    {
        int getLines = 0;
        for (int i = 0; i < 18; i++)
        {
            if (addNames[i] != "")
                getLines++;
        }
        dataGridView6.RowCount = getLines;
        int count = 0;
        for (int i = 0; i < 18; i++)
        {
            if (addNames[i] != "")
            {
                dataGridView6[0, count].Value = addNames[i];
                dataGridView6[1, count].Value = addIps[i];
                dataGridView6[2, count].Value = addLocalIps[i];
                dataGridView6[3, count].Value = addXUID[i];
                count++;
            }
        }
    }
    bool updateIps = false;
    private void colorCheckBox6_CheckedChanged(object sender, EventArgs e)
    {
        updateIps = colorCheckBox6.Checked;
    }
    uint[] getInfoOfs()
    {
        if (inParty())
            return infoPartyOfs;
        else
            return infoGameOfs;
    }
    private void getInfo(bool canGet)
    {
        if (canGet)
        {
            infoOfs = getInfoOfs();
            int c = 0;
            for (int i = 0; i < 18; i++)
            {
                string name = PS3.Extension.ReadString(infoOfs[0] + (uint)i * 0xD8);
                byte[] ip = PS3.Extension.ReadBytes(infoOfs[1] + (uint)i * 0xD8, 6);
                byte[] ipL = PS3.Extension.ReadBytes(infoOfs[1] - 0x12 + (uint)i * 0xD8, 4);
                byte[] xuid = PS3.Extension.ReadBytes(infoOfs[2] + (uint)i * 0xD8, 8);
                if (name != "")
                {
                    addNames[c] = name;
                    addIps[c] = convertIp(ip);
                    addLocalIps[c] = convertIp(ipL);
                    addXUID[c] = ByteArrayToString(xuid);
                    infoLine[c] = "Name: " + addNames[c] + " | IP: " + addIps[c] + " | Local IP: " + addLocalIps[c] + " | XUID: " + addXUID[c];
                    c++;
                }
                else
                {
                    addNames[c] = "";
                    addIps[c] = "";
                    addLocalIps[c] = "";
                    addXUID[c] = "";
                    c++;
                }
            }
        }
    }

    private void button78_Click(object sender, EventArgs e)
    {
        if (!inParty())
        {
            string name = PS3.Extension.ReadString(infoGameOfs[0] + (uint)RPC.getHostNum() * 0xD8);
            if (name != "")
                label22.Text = "Host: " + name;
            else
                label22.Text = "Host: N/A";
        }
        else
            label22.Text = "Host: N/A";
    }

    private void ipLog_Tick(object sender, EventArgs e)
    {
        logInfo();
    }

    private void copyItem(int index)
    {
        try
        {
            if (dataGridView6.RowCount != 0)
            {
                string item = dataGridView6[index, dataGridView6.CurrentRow.Index].Value.ToString();
                if (item != "")
                    Clipboard.SetText(item);
            }
        }
        catch
        {

        }
    }

    private void copyNameToolStripMenuItem_Click(object sender, EventArgs e)
    {
        copyItem(0);
    }

    private void copyIpToolStripMenuItem_Click(object sender, EventArgs e)
    {
        copyItem(1);
    }

    private void copyLocalIpToolStripMenuItem_Click(object sender, EventArgs e)
    {
        copyItem(2);
    }

    private void copyXUIDToolStripMenuItem_Click(object sender, EventArgs e)
    {
        copyItem(3);
    }

    private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            if (dataGridView6.RowCount != 0)
            {
                string items = "";
                for (int i = 0; i < 4; i++)
                {
                    string item = dataGridView6[i, dataGridView6.CurrentRow.Index].Value.ToString();
                    if (item != "")
                        items += item += " ";
                }
                if (items != "")
                    Clipboard.SetText(items);
            }
        }
        catch
        {

        }
    }

    private void button46_Click(object sender, EventArgs e)
    {

        //RPC.SV_SendServerCommand(0, "say hi");
        //MessageBox.Show(SL_GetString("menuresponse", 0).ToString());
       Cmd_MenuResponse_f(0, "popup_endgame", "endround");
    }
void Cmd_MenuResponse_f(int entity, string menu, string response)
{
scr_addString(response);
Task.Delay(1000).Wait();
scr_addString(menu);
Task.Delay(1000).Wait();
scr_notify(entity, SL_GetString("menuresponse", 0), 2);
}
    private void scr_addString(string txt)
    {
        RPC.Call(0x0020C428, txt);
    }
    private void scr_notify(int ent, int response, int paraCount)
    {
        RPC.Call(0x001B74F0, ent, response, paraCount);
    }

    int SL_GetString(string str, int user)
    {
        return RPC.Call(0x201688, str, user);
    }

    private void logInfo()
    {
        if (colorCheckBox3.Checked)
        {
            string path = Application.StartupPath + @"\MW2 - Info Log.txt";
            if (!File.Exists(path))
                File.WriteAllText(path, "");
            else
            {
                string[] infoDump = File.ReadAllLines(path);
                getInfo(true);
                for (int x = 0; x < 18; x++)
                {
                    if (!String.IsNullOrEmpty(infoLine[x]))
                    {
                        int[] num = infoDump.Select((b, i) => b == infoLine[x] ? i : -1).Where(i => i != -1).ToArray();
                        if (num.Length == 0)
                            File.AppendAllText(path, infoLine[x] + "\n");
                    }
                }
            }
        }
    }

    private void clientStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
       // ToolStripItem tool = (ToolStripItem)sender;

//           File.AppendAllText(listPath, tool.Text + ":");

    }

    private void spoofIp()
    {
        for (int i = 0; i < 18; i++)
        {
            uint[] spoofInfo = getInfoOfs();
            string name = PS3.Extension.ReadString(spoofInfo[0] + (uint)i * 0xD8);
            if (PS3.Extension.ReadString(nameOfs) == name)
            {
                PS3.SetMemory(spoofInfo[1] + (uint)i * 0xD8, new byte[] { 0xA8, 0x0C, 0x7D, 0x41 });
                break;
            }
        }
    }

    private void button54_Click(object sender, EventArgs e)
    {
        getInfo(true);
        updateInfo();
    }

    #endregion

    private void colorCheckBoxList2_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}