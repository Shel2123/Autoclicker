using Autoclicker.Core.Models;

namespace Autoclicker.Core.Abstractions
{
    public interface ISequenceBuilder
    {
        bool TryBuild(IList<StepRow> rows, out List<KeyStep> steps, out string error);
    }
}