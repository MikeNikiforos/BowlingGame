using System;

namespace BowlingGame.Domain
{
    public class InvalidRollState : Exception
    {
        public InvalidRollState(string msg) : base(msg)
        {
        }
    }

    public class InvalidFrameState : Exception
    {
        public int RemainingPins { get; set; }
        public InvalidFrameState(string msg) : base(msg)
        {
        }
    }

    public class InvalidGameState : Exception
    {
        public InvalidGameState(string msg) : base(msg)
        {
        }
    }
}