namespace LordVirusMw2XboxLib;

internal interface IGameCheat
{
    bool GetValue();
    byte[] GetBytes();

    void Enable();
    void Disable();
    void Toggle();
}