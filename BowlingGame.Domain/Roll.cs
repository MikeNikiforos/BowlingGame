namespace BowlingGame.Domain
{
    public class Roll
    {
        private readonly Frame _frame;

        private Roll(Frame frame, int pinsKnockedOver)
        {
            _frame = frame;
            PinsKnockedOver = pinsKnockedOver;
        }

        public int PinsKnockedOver { get; }

        public static Roll Create(Frame frame, int pinsKnockedOver)
        {
            if (pinsKnockedOver < 0 || pinsKnockedOver > 10)
                throw new InvalidRollState("Invalid state: Pins knocked over must be between 1 and 10");
            if (frame == null)
                throw new InvalidRollState("Invalid state: Roll cannot be detached from frame");
            return new Roll(frame, pinsKnockedOver);
        }
    }
}