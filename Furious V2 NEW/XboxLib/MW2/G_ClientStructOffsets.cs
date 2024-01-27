namespace LordVirusMw2XboxLib;

public enum G_ClientStructOffsets : uint
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
    InfAmmo1 = 0x2EC,
    InfAmmo2 = 0x2DC,
    InfAmmo3 = 0x354,
    InfAmmo4 = 0x36C,
    InfAmmo5 = 0x360,
    InfAmmo6 = 0x378,
    KillstreakBullet = 0x222,

#if DEBUG
    DebugOffset = 0x0000
#endif
}
