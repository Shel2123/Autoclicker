namespace Autoclicker.Core.Abstractions
{
    public interface IHotkeyService : IDisposable
    {
        int Register(Keys key, Action action, uint modifiers = 0);
        void Unregister(int id);
    }
}