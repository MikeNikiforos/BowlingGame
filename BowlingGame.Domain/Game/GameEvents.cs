namespace BowlingGame.Domain
{
    public abstract class GameEvent
    {
        public int FrameNumber { get; set; }
    }

    public class StartedGameEvent : GameEvent
    {
    }

    public class AdvanceFrameGameEvent : GameEvent
    {
        public int NewFrameNumber { get; set; }
    }

    public class EndGameEvent : GameEvent
    {
    }
}