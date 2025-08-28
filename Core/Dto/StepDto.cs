namespace Autoclicker.Core.Dto
{
    public sealed class StepDto
    {
        public string Key { get; set; } = string.Empty;
        public double DelaySeconds { get; set; }
        public double HoldSeconds { get; set; } = 0.12;
    }
}