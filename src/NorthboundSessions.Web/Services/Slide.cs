namespace NorthboundSessions.Web.Services
{
    public class Slide
    {
        public int SlideNumber { get; set; }
        public required string Title { get; set; }
        public required string BodyText { get; set; }
    }
}
