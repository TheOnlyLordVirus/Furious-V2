using System;
using System.Collections.Generic;
using System.Text;

using XDevkit;

using XDRPCPlusPlus;

namespace LordVirusMw2XboxLib;

#nullable enable

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

    private static readonly byte[] _infiniteAmmoOn = [0x0F, 0x00, 0x00, 0x00];
    private static readonly byte[] _infiniteAmmoOff = [0x00, 0x00, 0x00, 0x64];

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

        _redboxes = new G_ClientCheat
            (
                _xboxConsole,
                G_ClientStructOffsets.Redboxes,
                _clientIndex, 
                onByte: _redBoxesOn, 
                cheatName: "Red Boxes"
            );


        _thermalRedboxes = new G_ClientCheat
            (
                _xboxConsole, 
                G_ClientStructOffsets.Redboxes, 
                _clientIndex, 
                onByte: _thermalRedBoxesOn, 
                cheatName: "Thermal Red Boxes"
            );


        _godmode = new G_ClientCheat
            (
                _xboxConsole,
                G_ClientStructOffsets.Godmode, 
                _clientIndex, 
                onBytes: _godModeOn,
                offBytes: _godModeOff, 
                cheatName: "God Mode"
            );

        _noRecoil = new G_ClientCheat
            (
                _xboxConsole, 
                G_ClientStructOffsets.NoRecoil, 
                _clientIndex, 
                onByte: _noRecoilOn, 
                cheatName: "No Recoil"
            );

        _noClip = new G_ClientCheat
            (
                _xboxConsole, 
                G_ClientStructOffsets.MovementFlag, 
                _clientIndex,
                onByte: _noClipOn, 
                cheatName: "No Clip"
            );

        _ufoMode = new G_ClientCheat
            (
                _xboxConsole, 
                G_ClientStructOffsets.MovementFlag, 
                _clientIndex, 
                onByte: _ufoModeOn, 
                cheatName: "Ufo Mode"
            );

        _primaryAkimbo = new G_ClientCheat
            (
                _xboxConsole, 
                G_ClientStructOffsets.PrimaryAkimbo,
                _clientIndex, 
                cheatName: "Primary Akimbo"
            );

        _secondaryAkimbo = new G_ClientCheat
            (
                _xboxConsole, 
                G_ClientStructOffsets.SecondaryAkimbo, 
                _clientIndex, 
                cheatName: "Secondary Akimbo"
            );

        _allPerks = new G_ClientLoopingCheat
            (
                _xboxConsole, 
                G_ClientStructOffsets.AllPerks, 
                _clientIndex, 
                onBytes: _allPerksOn, 
                offBytes: _allPerksOff, 
                cheatName: "All Perks"
            );

        _infiniteAmmo = new G_ClientLoopingCheat
            (
                _xboxConsole, 
                _clientIndex,
                Internal_BuildInfAmmoCheats(), 
                cheatName: "Infinite Ammo"
            );

#if DEBUG
        DebugCheat = new G_ClientCheat
            (
                _xboxConsole, 
                G_ClientStructOffsets.DebugOffset, 
                _clientIndex, 
                onBytes: [0x00, 0x01], 
                offBytes: [0x00, 0x00], 
                "Debug Cheat"
            );
#endif
    }

    private IEnumerable<IGameCheat> Internal_BuildInfAmmoCheats()
    {
        try
        {
            G_ClientStructOffsets[] infAmmoOffsets =
                [
                    G_ClientStructOffsets.InfAmmo1,
                    G_ClientStructOffsets.InfAmmo2,
                    G_ClientStructOffsets.InfAmmo3,
                    G_ClientStructOffsets.InfAmmo4,
                    G_ClientStructOffsets.InfAmmo5,
                    G_ClientStructOffsets.InfAmmo6
                ];

            var infAmmoIGameCheats = new IGameCheat[4];

            // Set looping max ammo capacity for all 6 Weapon slots.
            foreach (var offset in infAmmoOffsets)
                infAmmoIGameCheats[0] = new G_ClientCheat
                    (
                        _xboxConsole,
                        offset,
                        _clientIndex,
                        onBytes: _infiniteAmmoOn,
                        offBytes: _infiniteAmmoOff
                    );

            return infAmmoIGameCheats;
        }

        catch
        {
            return Array.Empty<IGameCheat>();
        }
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

    private IGameCheat _infiniteAmmo;
    public IGameCheat InfiniteAmmo => _infiniteAmmo;
}