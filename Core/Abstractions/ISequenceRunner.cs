using Autoclicker.Core.Models;

namespace Autoclicker.Core.Abstractions
{
    public interface ISequenceRunner
    {
        bool IsRunning { get; }
        event Action<string>? Logged;
        Task RunAsync(IReadOnlyList<KeyStep> steps, bool loop, bool dryRun, Keys stopKey);
        void Stop();
    }
}