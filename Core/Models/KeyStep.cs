namespace Autoclicker.Core.Models
{
    public readonly record struct KeyStep(string KeyName, ushort VKey, int DelayMs, int HoldMs);
}
