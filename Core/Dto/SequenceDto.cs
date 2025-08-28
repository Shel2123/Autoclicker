namespace Autoclicker.Core.Dto
{
    public sealed class SequenceDto
    {
        public bool Loop { get; set; }
        public List<StepDto> Steps { get; } = new();
    }
}
