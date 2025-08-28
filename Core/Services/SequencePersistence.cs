using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Autoclicker.Core.Abstractions;
using Autoclicker.Core.Dto;
using Autoclicker.Core.Models;

namespace Autoclicker.Core.Services
{
    public sealed class SequencePersistence : ISequencePersistence
    {
        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            WriteIndented = true
        };

        public void Save(string filePath, bool loop, IEnumerable<StepRow> rows)
        {
            var dto = new SequenceDto { Loop = loop, Steps = new() };
            foreach (var r in rows)
                dto.Steps.Add(new StepDto
                {
                    Key = r.Key ?? string.Empty,
                    DelaySeconds = r.DelaySeconds,
                    HoldSeconds = r.HoldSeconds
                });

            var json = JsonSerializer.Serialize(dto, JsonOpts);
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }

        public (bool Loop, List<StepRow> Rows) Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found.", filePath);

            var text = File.ReadAllText(filePath, Encoding.UTF8).Trim();
            if (string.IsNullOrEmpty(text))
                throw new InvalidDataException("Empty JSON file.");

            var dto = JsonSerializer.Deserialize<SequenceDto>(text, JsonOpts)
                      ?? throw new InvalidDataException("Invalid JSON.");

            if (dto.Steps == null)
                throw new InvalidDataException("Missing 'Steps' array.");

            var rows = new List<StepRow>(dto.Steps.Count);
            foreach (var s in dto.Steps)
            {
                var key = s.Key?.Trim() ?? string.Empty;
                var hold = s.HoldSeconds <= 0 ? 0.12 : s.HoldSeconds; // бэкомпат на старые файлы
                rows.Add(new StepRow { Key = key, DelaySeconds = s.DelaySeconds, HoldSeconds = hold });
            }
            return (dto.Loop, rows);
        }
    }
}
