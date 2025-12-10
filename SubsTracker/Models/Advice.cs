namespace SubsTracker.Models
{
    public enum AdviceType
    {
        Info,    
        Warning,
        Good     
    }

    public class Advice
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public AdviceType Type { get; set; }

        public string HexColor => Type switch
        {
            AdviceType.Warning => "#FFF3CD", // Soft Yellow
            AdviceType.Good => "#D1E7DD",    // Soft Green
            _ => "#CFE2FF"                   // Soft Blue
        };

        public string TextColor => Type switch
        {
            AdviceType.Warning => "#856404",
            AdviceType.Good => "#0F5132",
            _ => "#084298"
        };
    }
}