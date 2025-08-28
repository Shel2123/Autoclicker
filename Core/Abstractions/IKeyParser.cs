namespace Autoclicker.Core.Abstractions
{
    public interface IKeyParser
    {
        bool TryParseKey(string name, out ushort vkey, out string normalized);
    }
}