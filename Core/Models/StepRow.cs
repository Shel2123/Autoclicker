namespace Autoclicker.Core.Models
{
    public sealed class StepRow
    {
        public string Key { get; set; } = "W";
        public double DelaySeconds { get; set; } = 5.0;
        public double HoldSeconds { get; set; } = 0.12;
    }
}