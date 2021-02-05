namespace BowlingGame.Domain
{
    public abstract class FrameEvent
    {
        public int FrameNumber { get; set; }
    }

    public class StartOfFirstRollFrameEvent : FrameEvent
    {
    }

    public class RollFrameEvent : FrameEvent
    {
        public int PinsKnockedOver { get; set; }
    }

    public class StartOfSecondRollFrameEvent : FrameEvent
    {
        public int PinsKnockedOver { get; set; }
        public int PinsRemaining { get; set; }
    }

    public class StartOfThirdRollFrameEvent : FrameEvent
    {
        public int PinsKnockedOver { get; set; }
        public int PinsRemaining { get; set; }
    }

    public class StrikeFrameEvent : FrameEvent
    {
    }

    public class SpareFrameEvent : FrameEvent
    {
    }

    public class EndFrameEvent : FrameEvent
    {
        public int PinsKnockedOver { get; set; }
    }
}