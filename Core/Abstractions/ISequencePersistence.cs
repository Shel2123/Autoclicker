using Autoclicker.Core.Models;

namespace Autoclicker.Core.Abstractions
{
    public interface ISequencePersistence
    {
        void Save(string filePath, bool loop, IEnumerable<StepRow> rows);
        (bool Loop, List<StepRow> Rows) Load(string filePath);
    }
}