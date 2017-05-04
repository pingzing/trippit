namespace DigiTransit10.Models
{
    public enum WalkingSpeedType
    {
        Glacial,
        Slow,
        Normal,
        Fast,
        Breakneck
    }

    public class WalkingSpeed
    {
        public WalkingSpeedType SpeedType { get; set; }
        public string DisplayName { get; set; }
        public string DisplaySubtitle { get; set; }
    }
}
