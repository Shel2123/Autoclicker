namespace Autoclicker.Core.Abstractions
{
    public interface IKeyboard
    {
        bool KeyDown(ushort vkey, out int lastError);
        bool KeyUp(ushort vkey, out int lastError);
    }
}