using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using XDevkit;

using XDRPC;
using XDRPCPlusPlus;

namespace LordVirusXboxLib;

#nullable enable

//gclient_s
//{
//    playerState_s ps; // 0x0
//    clientSession_s sess; // 0x3190
//    int spectatorClient; // 0x341C
//    int mFlags; // 0x3420
//    char padding[4]; // 0x3424
//    int lastCmdTime; // 0x3428
//    int buttons; // 0x342C
//    int oldButtons; // 0x3430
//    int latched_buttons; // 0x3434
//    int buttonsSinceLastFrame; // 0x3438
//    vec3 oldOrigin; // 0x343C
//    float fGunPitch; // 0x3448
//    float fGunYaw; // 0x344C
//    char padding2[8]; // 0x3450
//    vec3 damage_from; // 0x3458
//    int damage_fromWorld; // 0x3464
//    int accurateCount; // 0x3468
//    int accuracy_shots; // 0x346C
//    int accuracy_hits; // 0x3470
//    int inactivityTime; // 0x3474
//    int inactivityWarning; // 0x3478
//    int lastVoiceTime; // 0x347C
//    int switchTeamTime; // 0x3480
//    float currentAimSpreadScale; // 0x3484
//    float prevLinkedInvQuat[4]; // 0x3488
//    bool link_rotationMovesEyePos; // 0x3498
//    bool link_doCollision; // 0x3499
//    bool link_useTagAnglesForViewAngles; // 0x349A
//    bool link_useBaseAnglesForViewClamp; // 0x349B
//    float linkAnglesFrac; // 0x349C
//    viewClampState link_viewClamp; // 0x34A0
//}

internal enum G_ClientStructOffsets : uint
{
    Array_BaseAddress = 0x830CBF80,
    StructSize = 0x3700,
    Redboxes = 0x13,
    PlayerOrgin = 0x1C,
    Name = 0x3290,
    Godmode = 0x3228,
    NoRecoil = 0x2BE,
    MovementFlag = 0x3423,
    PrimaryAkimbo = 0x1C,
    SecondaryAkimbo = 0x25D,
    AllPerks = 0x428,
    //ModGun = 0x3243,
    InfAmmo1 = 0x2EC,
    InfAmmo2 = 0x2DC,
    InfAmmo3 = 0x354,
    InfAmmo4 = 0x36C,
    InfAmmo5 = 0x360,
    InfAmmo6 = 0x378,
    KillstreakBullet = 0x222,

#if DEBUG
    DebugOffset = 0x3228
#endif
}

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

internal sealed class G_Client
{
    private const int _maxNameCharCount = 35;

    private const byte _redBoxesOn = 0x55;
    private const byte _thermalRedBoxesOn = 0x99;
    private const byte _ufoModeOn = 0x02;
    private const byte _noClipOn = 0x99;
    private const byte _noRecoilOn = 0x04;

    private static readonly byte[] _godModeOn = [0x00, 0xFF, 0xFF, 0xFF];
    private static readonly byte[] _godModeOff = [0x00, 0x00, 0x00, 0x64];

    private static readonly byte[] _allPerksOn = [0xFF, 0xFF];
    private static readonly byte[] _allPerksOff = [0x00, 0x00];

    private readonly IXboxConsole _xboxConsole;
    public IXboxConsole XboxConsole
    {
        get => _xboxConsole;
    }

    private readonly uint _correctedNameAddress;
    private string _clientName = string.Empty;
    public string ClientName
    {
        get
        {
            ReadOnlySpan<byte> bytes =
                _xboxConsole.ReadBytes
                (
                    _correctedNameAddress,
                    _maxNameCharCount
                );

            bytes = bytes.Slice(0, bytes.IndexOf((byte)0x00));
            _clientName = Encoding.UTF8.GetString(bytes.ToArray());

            return _clientName;
        }

        set
        {
            if (value.Length > _maxNameCharCount)
                value = value.AsSpan().Slice(0, _maxNameCharCount).ToString();

            Span<byte> nameBytes = stackalloc byte[_maxNameCharCount];
            nameBytes = Encoding.ASCII.GetBytes(value);

            _xboxConsole
                .DebugTarget
                .SetMemory
                (
                    _correctedNameAddress,
                    (uint)nameBytes.Length,
                    nameBytes.ToArray(),
                    out _
                );
        }
    }

    private readonly int _clientIndex;
    public int ClientIndex => _clientIndex;

    public G_Client(IXboxConsole xbox, int clientIndex = 0)
    {
        _xboxConsole = xbox;
        _clientIndex = clientIndex;

        _correctedNameAddress =
            (uint)((uint)G_ClientStructOffsets.Array_BaseAddress +
            ((uint)G_ClientStructOffsets.StructSize * _clientIndex) +
            (uint)G_ClientStructOffsets.Name);

        _redboxes = new G_ClientCheat(_xboxConsole, G_ClientStructOffsets.Redboxes, _clientIndex, onByte: _redBoxesOn, cheatName: "Red Boxes");
        _thermalRedboxes = new G_ClientCheat(_xboxConsole, G_ClientStructOffsets.Redboxes, _clientIndex, onByte: _thermalRedBoxesOn, cheatName: "Thermal Red Boxes");
        _godmode = new G_ClientCheat(_xboxConsole, G_ClientStructOffsets.Godmode, _clientIndex, onBytes: _godModeOn, offBytes: _godModeOff, cheatName: "God Mode");
        _noRecoil = new G_ClientCheat(_xboxConsole, G_ClientStructOffsets.NoRecoil, _clientIndex, onByte: _noRecoilOn, cheatName: "No Recoil");
        _noClip = new G_ClientCheat(_xboxConsole, G_ClientStructOffsets.MovementFlag, _clientIndex, onByte: _noClipOn, cheatName: "No Clip");
        _ufoMode = new G_ClientCheat(_xboxConsole, G_ClientStructOffsets.MovementFlag, _clientIndex, onByte: _ufoModeOn, cheatName: "Ufo Mode");

        _primaryAkimbo = new G_ClientCheat(_xboxConsole, G_ClientStructOffsets.PrimaryAkimbo, _clientIndex, cheatName: "Primary Akimbo");
        _secondaryAkimbo = new G_ClientCheat(_xboxConsole, G_ClientStructOffsets.SecondaryAkimbo, _clientIndex, cheatName: "Secondary Akimbo");

        _allPerks = new G_ClientLoopingCheat(_xboxConsole, G_ClientStructOffsets.AllPerks, _clientIndex, onBytes: _allPerksOn, offBytes: _allPerksOff, cheatName: "All Perks");

        _infiniteAmmo = new G_ClientLoopingCheat(_xboxConsole, _clientIndex, new G_ClientInfiniteAmmo(_xboxConsole, _clientIndex), cheatName: "Infinite Ammo");

#if DEBUG
        DebugCheat = new G_ClientCheat(_xboxConsole, G_ClientStructOffsets.DebugOffset, _clientIndex);
#endif
    }

    // TODO: Get the in game check for the current client without RPC calling GetDvarBool.
    // TODO: Fix missing G_Client mods / addresses
    // TODO: Add magic bullets for clients.

    //private IGameCheat? _teleport;
    //public IGameCheat? Teleport;

    //private IGameCheat? _killstreakBullet;
    //public IGameCheat? KillstreakBullet => _killstreakBullet;

#if DEBUG
    public IGameCheat? DebugCheat;
#endif

    private readonly IGameCheat _redboxes;
    public IGameCheat Redboxes => _redboxes;

    private readonly IGameCheat _thermalRedboxes;
    public IGameCheat ThermalRedboxes => _thermalRedboxes;

    private readonly IGameCheat _godmode;
    public IGameCheat Godmode => _godmode;

    private readonly IGameCheat _noRecoil;
    public IGameCheat NoRecoil => _noRecoil;

    private readonly IGameCheat _noClip;
    public IGameCheat NoClip => _noClip;

    private readonly IGameCheat _ufoMode;
    public IGameCheat UfoMode => _ufoMode;

    private readonly IGameCheat _primaryAkimbo;
    public IGameCheat PrimaryAkimbo => _primaryAkimbo;

    private readonly IGameCheat _secondaryAkimbo;
    public IGameCheat SecondaryAkimbo => _secondaryAkimbo;

    private readonly IGameCheat _allPerks;
    public IGameCheat AllPerks => _allPerks;

    //private IGameCheat? _modGun;
    //public IGameCheat? ModGun => _modGun;

    private IGameCheat _infiniteAmmo;
    public IGameCheat InfiniteAmmo => _infiniteAmmo;
}

internal interface IGameCheat
{
    bool GetValue();
    byte[] GetBytes();

    void Enable();
    void Disable();
    void Toggle();
}

internal class G_ClientCheat : IGameCheat
{
    private uint CorrectedCheatAddress =>
        (uint)G_ClientStructOffsets.Array_BaseAddress +
        ((uint)G_ClientStructOffsets.StructSize * (uint)_clientNumber) +
        (uint)_cheatAddress;

    private readonly IXboxConsole _xboxConsole;
    private readonly G_ClientStructOffsets _cheatAddress;

    private bool _enabled = false;

    private readonly string? _cheatName;
    private readonly int _clientNumber = 0;
    private readonly uint _byteCount = 1;

    private readonly byte[] _onBytes;
    private readonly byte[] _offBytes;

    public G_ClientCheat
    (
        IXboxConsole xboxConsole,
        G_ClientStructOffsets cheatAddress,
        int clientNumber,
        byte onByte = 0x01,
        byte offByte = 0x00,
        string? cheatName = null
    )
    {
        _xboxConsole = xboxConsole;
        _cheatAddress = cheatAddress;
        _clientNumber = clientNumber;
        _cheatName = cheatName;

        _onBytes = [onByte];
        _offBytes = [offByte];
    }

    public G_ClientCheat
    (
        IXboxConsole xboxConsole,
        G_ClientStructOffsets cheatAddress,
        int clientNumber,
        byte[] onBytes,
        byte[] offBytes,
        string? cheatName = null
    )
    {
        if (onBytes.Length != offBytes.Length)
            throw new ArgumentOutOfRangeException("Error: onBytes and offBytes must have the same number of bytes.");

        _byteCount = (uint)onBytes.Length;

        _xboxConsole = xboxConsole;
        _cheatAddress = cheatAddress;
        _clientNumber = clientNumber;
        _cheatName = cheatName;

        _onBytes = onBytes;
        _offBytes = offBytes;
    }

    public void Enable()
    {
        try
        {
            _xboxConsole
                .WriteBytes
                (
                    CorrectedCheatAddress,
                    _onBytes
                );

            if (_cheatName is not null)
                Mw2GameFunctions.iPrintLn(_xboxConsole, $"{_cheatName}^7: ^2Enabled", _clientNumber);
        }

        catch
        {
            return;
        }

        _enabled = true;
    }

    public void Disable()
    {
        try
        {
            _xboxConsole
                .WriteBytes
                (
                    CorrectedCheatAddress,
                    _offBytes
                );

            if (_cheatName is not null)
                Mw2GameFunctions.iPrintLn(_xboxConsole, $"{_cheatName}^7: ^1Disabled", _clientNumber);
        }

        catch
        {
            return;
        }

        _enabled = false;
    }

    public byte[] GetBytes()
    {
        return _xboxConsole
                .ReadBytes
                (
                    CorrectedCheatAddress,
                    _byteCount
                );
    }

    public bool GetValue()
    {
        try
        {
            return Enumerable.SequenceEqual
                (
                    GetBytes(),
                    _onBytes
                );
        }

        catch { return false; }
    }

    public void Toggle()
    {
        _enabled = !(GetValue());

        if (_enabled)
            Enable();
        else
            Disable();
    }
}

internal class G_ClientInfiniteAmmo : IGameCheat
{
    private bool _enabled = false;

    private static readonly byte[] _infiniteAmmoOn = [0x0F, 0x00, 0x00, 0x00];
    private static readonly byte[] _infiniteAmmoOff = [0x00, 0x00, 0x00, 0x64];

    private readonly int _clientIndex = 0;

    private readonly IXboxConsole _xboxConsole;
    private readonly IGameCheat _infAmmo1;
    private readonly IGameCheat _infAmmo2;
    private readonly IGameCheat _infAmmo3;
    private readonly IGameCheat _infAmmo4;
    private readonly IGameCheat _infAmmo5;
    private readonly IGameCheat _infAmmo6;

    public G_ClientInfiniteAmmo(IXboxConsole xboxConsole, int clientIndex)
    {
        _xboxConsole = xboxConsole;

        _clientIndex = clientIndex;

        // TODO: Label these for what weapon slot they are giving inf ammo too.
        _infAmmo1 = new G_ClientCheat(_xboxConsole, G_ClientStructOffsets.InfAmmo1, _clientIndex, onBytes: _infiniteAmmoOn, offBytes: _infiniteAmmoOff);
        _infAmmo2 = new G_ClientCheat(_xboxConsole, G_ClientStructOffsets.InfAmmo2, _clientIndex, onBytes: _infiniteAmmoOn, offBytes: _infiniteAmmoOff);
        _infAmmo3 = new G_ClientCheat(_xboxConsole, G_ClientStructOffsets.InfAmmo3, _clientIndex, onBytes: _infiniteAmmoOn, offBytes: _infiniteAmmoOff);
        _infAmmo4 = new G_ClientCheat(_xboxConsole, G_ClientStructOffsets.InfAmmo4, _clientIndex, onBytes: _infiniteAmmoOn, offBytes: _infiniteAmmoOff);
        _infAmmo5 = new G_ClientCheat(_xboxConsole, G_ClientStructOffsets.InfAmmo5, _clientIndex, onBytes: _infiniteAmmoOn, offBytes: _infiniteAmmoOff);
        _infAmmo6 = new G_ClientCheat(_xboxConsole, G_ClientStructOffsets.InfAmmo6, _clientIndex, onBytes: _infiniteAmmoOn, offBytes: _infiniteAmmoOff);
    }

    public void Enable()
    {
        try
        {
            _infAmmo1.Enable();
            _infAmmo2.Enable();
            _infAmmo3.Enable();
            _infAmmo4.Enable();
            _infAmmo5.Enable();
            _infAmmo6.Enable();
        }

        catch
        {
            return;
        }

        _enabled = true;
    }

    public void Disable()
    {
        try
        {
            _infAmmo1.Disable();
            _infAmmo2.Disable();
            _infAmmo3.Disable();
            _infAmmo4.Disable();
            _infAmmo5.Disable();
            _infAmmo6.Disable();
        }

        catch
        {
            return;
        }

        _enabled = false;
    }

    public byte[] GetBytes()
    {
        return Array.Empty<byte>();
    }

    public bool GetValue()
    {
        return _enabled;
    }

    public void Toggle()
    {
        _enabled = !_enabled;

        if (_enabled)
            Enable();
        else
            Disable();
    }
}

internal class G_ClientLoopingCheat : IGameCheat
{
    private uint CorrectedCheatAddress =>
        (uint)G_ClientStructOffsets.Array_BaseAddress +
        ((uint)G_ClientStructOffsets.StructSize * (uint)_clientIndex) +
        (uint)_cheatOffset;

    private CancellationTokenSource? _updaterCancellationTokenSource = new CancellationTokenSource();

    private readonly IXboxConsole _xboxConsole;
    private readonly G_ClientStructOffsets _cheatOffset;

    private bool _enabled = false;

    private readonly string? _cheatName;
    private readonly int _clientIndex = 0;
    private readonly uint _byteCount = 1;

    private readonly bool _usingBytes = true;
    private readonly byte[]? _onBytes;
    private readonly byte[]? _offBytes;

    private readonly IEnumerable<IGameCheat>? _gameCheats;

    public G_ClientLoopingCheat(
        IXboxConsole xboxConsole, 
        G_ClientStructOffsets cheatOffset, 
        int clientIndex, 
        byte onByte = 0x01, 
        byte offByte = 0x00,
        string? cheatName = null)
    {
        _xboxConsole = xboxConsole;
        _cheatOffset = cheatOffset;
        _clientIndex = clientIndex;

        _onBytes = [onByte];
        _offBytes = [offByte];

        _cheatName = cheatName;
    }

    public G_ClientLoopingCheat(
        IXboxConsole xboxConsole,
        G_ClientStructOffsets cheatOffset,
        int clientIndex, 
        byte[] onBytes, 
        byte[] offBytes,
        string? cheatName = null)
    {
        _xboxConsole = xboxConsole;
        _cheatOffset = cheatOffset;
        _clientIndex = clientIndex;

        _onBytes = onBytes;
        _offBytes = offBytes;

        _cheatName = cheatName;
    }

    public G_ClientLoopingCheat(
        IXboxConsole xboxConsole,
        int clientIndex,
        IEnumerable<IGameCheat> gameCheats,
        string? cheatName = null)
    {
        _xboxConsole = xboxConsole;
        _clientIndex = clientIndex;

        _onBytes = null;
        _offBytes = null;

        _usingBytes = false;
        _gameCheats = gameCheats;

        _cheatName = cheatName;
    }

    public G_ClientLoopingCheat(
        IXboxConsole xboxConsole,
        int clientIndex,
        IGameCheat gameCheats,
        string? cheatName = null)
    {
        _xboxConsole = xboxConsole;
        _clientIndex = clientIndex;

        _onBytes = null;
        _offBytes = null;

        _usingBytes = false;
        _gameCheats = [gameCheats];

        _cheatName = cheatName;
    }

    public async Task SetLoop(CancellationToken cancellationToken)
    {
        try
        {
            if (_cheatName is not null)
                Mw2GameFunctions.iPrintLn(_xboxConsole, $"{_cheatName}^7: ^2Enabled", _clientIndex);

            do
            {
                if (_usingBytes)
                    _xboxConsole
                        .WriteBytes
                        (
                            CorrectedCheatAddress,
                            _onBytes
                        );

                else if (_gameCheats is not null)
                    foreach (var cheat in _gameCheats)
                        cheat.Enable();

                await Task
                    .Delay(TimeSpan.FromSeconds(3), cancellationToken);
            }
            while (!cancellationToken.IsCancellationRequested);
        }

        finally
        {
            _enabled = false;

            if (_cheatName is not null)
                Mw2GameFunctions.iPrintLn(_xboxConsole, $"{_cheatName}^7: ^1Disabled", _clientIndex);
        }
    }

    public void Enable()
    {
        try
        {
            if (_updaterCancellationTokenSource is null)
                _updaterCancellationTokenSource = new CancellationTokenSource();

            _ = SetLoop(_updaterCancellationTokenSource.Token)
                .ConfigureAwait(false);
        }

        catch
        {
            return;
        }

        _enabled = true;
    }

    public void Disable()
    {
        _updaterCancellationTokenSource?.Cancel();
        _updaterCancellationTokenSource = null;
        _enabled = false;
    }

    public byte[] GetBytes()
    {
        if (_usingBytes)
            return _enabled ? _onBytes! : _offBytes!;
        else
            return Array.Empty<byte>();
    }

    public bool GetValue()
    {
        return _enabled;
    }

    public void Toggle()
    {
        _enabled = !_enabled;

        if (_enabled)
            Enable();
        else
            Disable();
    }
}