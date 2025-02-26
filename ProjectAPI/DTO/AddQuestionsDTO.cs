namespace ProjectAPI.DTO
{
    public class AddQuestionsDTO
    {
        public string MainTrackName { get; set; }
        public string FrameworkName { get; set; }

        public string Questions { get; set; }

        public string? Answers { get; set; }
    }
}
