namespace Autoclicker.Core.Abstractions
{
    public interface IHotkeyServiceFactory
    {
        IHotkeyService Create(Form host);
    }
}