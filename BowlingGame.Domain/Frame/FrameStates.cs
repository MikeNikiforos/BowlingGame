using System;

namespace BowlingGame.Domain
{
    public interface IFrameState
    {
        Frame Roll(int pinsKnockedOver);
    }

    public class StartOfFrameState : IFrameState
    {
        private readonly Action<IFrameState, FrameEvent> _addFrameEvent;
        private readonly Frame _frame;

        public StartOfFrameState(Frame frame, Action<IFrameState, FrameEvent> addFrameEvent)
        {
            _frame = frame;
            _addFrameEvent = addFrameEvent;
            _addFrameEvent(this, new StartOfFirstRollFrameEvent {FrameNumber = _frame.FrameNumber});
        }

        public Frame Roll(int pinsKnockedOver)
        {
            _addFrameEvent(this,
                new RollFrameEvent {FrameNumber = _frame.FrameNumber, PinsKnockedOver = pinsKnockedOver});
            return _frame;
        }
    }

    public class PlayedOneRollRegularFrameState : IFrameState
    {
        private readonly Action<IFrameState, FrameEvent> _addFrameEvent;
        private readonly Frame _frame;

        public PlayedOneRollRegularFrameState(Frame frame, Action<IFrameState, FrameEvent> addFrameEvent)
        {
            _frame = frame;
            _addFrameEvent = addFrameEvent;
            if (_frame.IsStrike)
            {
                var strikeEvent = new StrikeFrameEvent {FrameNumber = _frame.FrameNumber};
                _addFrameEvent(this, strikeEvent);
            }
        }

        public Frame Roll(int pinsKnockedOver)
        {
            if (pinsKnockedOver > _frame.NumOfRemainingPins)
                throw new InvalidFrameState(
                    $"Invalid state: A roll cannot knock down more than remaining pins in frame. Remaining pins are {_frame.NumOfRemainingPins}");
            _addFrameEvent(this,
                new RollFrameEvent {FrameNumber = _frame.FrameNumber, PinsKnockedOver = pinsKnockedOver});

            return _frame;
        }
    }

    public class PlayedTwoRollsRegularFrameState : IFrameState
    {
        private readonly Action<IFrameState, FrameEvent> _addFrameEvent;
        private readonly Frame _frame;

        public PlayedTwoRollsRegularFrameState(Frame frame, Action<IFrameState, FrameEvent> addFrameEvent)
        {
            _frame = frame;
            _addFrameEvent = addFrameEvent;
            if (_frame.IsSpare)
            {
                var spareEvent = new SpareFrameEvent {FrameNumber = _frame.FrameNumber};
                _addFrameEvent(this, spareEvent);
            }
            else
            {
                var spareEvent = new EndFrameEvent
                    {FrameNumber = _frame.FrameNumber, PinsKnockedOver = _frame.NumOfKnockedOverPins};
                _addFrameEvent(this, spareEvent);
            }
        }

        public Frame Roll(int pinsKnockedOver)
        {
            throw new InvalidFrameState("Invalid state: Cannot roll more than 2 times in a single frame");
        }
    }

    public class CompletedFrameState : IFrameState
    {
        private readonly Frame _frame;

        public CompletedFrameState(Frame frame)
        {
            _frame = frame;
        }

        public Frame Roll(int pinsKnockedOver)
        {
            throw new InvalidFrameState("Invalid state: Cannot roll again after frame has completed.");
        }
    }

    public class PlayedOneRollLastFrameState : IFrameState
    {
        private readonly Action<IFrameState, FrameEvent> _addFrameEvent;
        private readonly LastFrame _frame;

        public PlayedOneRollLastFrameState(LastFrame frame, Action<IFrameState, FrameEvent> addFrameEvent)
        {
            _frame = frame;
            _addFrameEvent = addFrameEvent;
            if (_frame.Rolls[0].PinsKnockedOver == 10)
            {
                var strikeEvent = new StrikeFrameEvent {FrameNumber = _frame.FrameNumber};
                _addFrameEvent(this, strikeEvent);
            }
        }

        public Frame Roll(int pinsKnockedOver)
        {
            if (pinsKnockedOver > _frame.NumOfRemainingPins)
                throw new InvalidFrameState(
                    "Invalid state: A roll cannot knock down more than remaining pins in frame");
            _addFrameEvent(this,
                new RollFrameEvent {FrameNumber = _frame.FrameNumber, PinsKnockedOver = pinsKnockedOver});

            return _frame;
        }
    }

    public class PlayedTwoRollsLastFrameState : IFrameState
    {
        private readonly Action<IFrameState, FrameEvent> _addFrameEvent;
        private readonly LastFrame _frame;

        public PlayedTwoRollsLastFrameState(LastFrame frame, Action<IFrameState, FrameEvent> addFrameEvent)
        {
            _frame = frame;
            _addFrameEvent = addFrameEvent;
            if (_frame.Rolls[1].PinsKnockedOver == 10 && _frame.Rolls[0].PinsKnockedOver == 10)
            {
                var strikeEvent = new StrikeFrameEvent {FrameNumber = _frame.FrameNumber};
                _addFrameEvent(this, strikeEvent);
            }
            else if (_frame.Rolls[0].PinsKnockedOver != 10 &&
                     _frame.Rolls[0].PinsKnockedOver + _frame.Rolls[1].PinsKnockedOver == 10)
            {
                var spareEvent = new SpareFrameEvent {FrameNumber = _frame.FrameNumber};
                _addFrameEvent(this, spareEvent);
            }
            else if (_frame.Rolls[0].PinsKnockedOver == 10)
            {
                var startOfThirdRollFrameEvent = new StartOfThirdRollFrameEvent
                {
                    FrameNumber = _frame.FrameNumber, PinsKnockedOver = _frame.NumOfKnockedOverPins,
                    PinsRemaining = _frame.NumOfRemainingPins
                };
                _addFrameEvent(this, startOfThirdRollFrameEvent);
            }
            else
            {
                var endFrameEvent = new EndFrameEvent
                    {FrameNumber = _frame.FrameNumber, PinsKnockedOver = _frame.NumOfKnockedOverPins};
                _addFrameEvent(this, endFrameEvent);
            }
        }

        public Frame Roll(int pinsKnockedOver)
        {
            if (pinsKnockedOver > _frame.NumOfRemainingPins)
                throw new InvalidFrameState(
                    "Invalid state: A roll cannot knock down more than remaining pins in frame");
            _addFrameEvent(this,
                new RollFrameEvent {FrameNumber = _frame.FrameNumber, PinsKnockedOver = pinsKnockedOver});

            return _frame;
        }
    }

    public class PlayedThreeRollsLastFrameState : IFrameState
    {
        private readonly Action<IFrameState, FrameEvent> _addFrameEvent;
        private readonly LastFrame _frame;

        public PlayedThreeRollsLastFrameState(LastFrame frame, Action<IFrameState, FrameEvent> addFrameEvent)
        {
            _frame = frame;
            _addFrameEvent = addFrameEvent;
            if (_frame.Rolls[2].PinsKnockedOver == 10)
            {
                var strikeEvent = new StrikeFrameEvent {FrameNumber = _frame.FrameNumber};
                _addFrameEvent(this, strikeEvent);
            }
            else
            {
                var endFrameEvent = new EndFrameEvent
                    {FrameNumber = _frame.FrameNumber, PinsKnockedOver = _frame.NumOfKnockedOverPins};
                _addFrameEvent(this, endFrameEvent);
            }
        }

        public Frame Roll(int pinsKnockedOver)
        {
            if (pinsKnockedOver > _frame.NumOfRemainingPins)
                throw new InvalidFrameState(
                    "Invalid state: A roll cannot knock down more than remaining pins in frame");
            _addFrameEvent(this,
                new RollFrameEvent {FrameNumber = _frame.FrameNumber, PinsKnockedOver = pinsKnockedOver});

            return _frame;
        }
    }
}