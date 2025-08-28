using Autoclicker.Core.Abstractions;

namespace Autoclicker.Infrastructure
{
    internal sealed class HotkeyServiceFactory : IHotkeyServiceFactory
    {
        public IHotkeyService Create(Form host) => new HotkeyManager(host);
    }
}