using System;
using System.Collections.Generic;
using System.Linq;

namespace BowlingGame.Domain
{
    public class Frame
    {
        private const int NumOfPinsAtStartOfFrame = 10;
        protected readonly int _frameNumber;

        protected readonly List<Roll> _rolls = new List<Roll>();
        protected readonly Action<GameEvent, IGameState> AddGameEvent;
        protected readonly Game Game;

        protected IFrameState FrameState;

        protected Frame(Game game, int frameNumber, Action<GameEvent, IGameState> addGameEvent)
        {
            Game = game;
            _frameNumber = frameNumber;
            FrameState = new StartOfFrameState(this, AddFrameEvent);
            AddGameEvent = addGameEvent;
        }

        public int FrameNumber => _frameNumber;
        public List<Roll> Rolls => _rolls.ToList();

        public virtual bool IsStrike => _rolls.Count == 1 && NumOfRemainingPins == 0;
        public virtual bool IsSpare => _rolls.Count == 2 && NumOfRemainingPins == 0;

        public virtual int NumOfKnockedOverPins => _rolls.Sum(x => x.PinsKnockedOver);

        public virtual int NumOfRemainingPins => NumOfPinsAtStartOfFrame - NumOfKnockedOverPins;

        public List<FrameEvent> Events { get; set; } = new List<FrameEvent>();

        public static Frame Create(Game game, int frameNumber, Action<GameEvent, IGameState> addGameEvent)
        {
            if (frameNumber < 1 || frameNumber > 9)
                throw new InvalidFrameState("Invalid state: Frame must be between 1 and 9");
            return new Frame(game, frameNumber, addGameEvent);
        }

        public virtual int GetScore()
        {
            if (IsStrike)
            {
                var strikeScore =
                    Game.Frames
                        .Skip(FrameNumber - 1)
                        .SelectMany(x => x.Rolls)
                        .Take(3)
                        .Sum(x => x.PinsKnockedOver);
                return strikeScore;
            }

            if (IsSpare)
            {
                var spareScore =
                    Game.Frames
                        .Skip(FrameNumber - 1)
                        .SelectMany(x => x.Rolls)
                        .Take(3)
                        .Sum(x => x.PinsKnockedOver);
                return spareScore;
            }

            return NumOfKnockedOverPins;
        }

        public Frame Roll(int pinsKnockedOver)
        {
            FrameState.Roll(pinsKnockedOver);
            return this;
        }

        protected virtual void AddFrameEvent(IFrameState frameState, FrameEvent frameEvent)
        {
            switch (frameEvent)
            {
                case StartOfFirstRollFrameEvent _:
                    Events.Add(frameEvent);
                    Game.RespondToFrameEvents?.Invoke(Game, frameEvent);
                    break;

                case StartOfSecondRollFrameEvent _:
                    Events.Add(frameEvent);
                    Game.RespondToFrameEvents?.Invoke(Game, frameEvent);
                    break;

                case RollFrameEvent re when frameState is StartOfFrameState:
                    var roll1 = Domain.Roll.Create(this, re.PinsKnockedOver);
                    AddRoll(roll1);
                    Events.Add(frameEvent);
                    Game.RespondToFrameEvents?.Invoke(Game, frameEvent);
                    ChangeState(new PlayedOneRollRegularFrameState(this, AddFrameEvent));
                    break;

                case StrikeFrameEvent _ when frameState is PlayedOneRollRegularFrameState:
                    Events.Add(frameEvent);
                    Game.RespondToFrameEvents?.Invoke(Game, frameEvent);
                    AddFrameEvent(FrameState,
                        new EndFrameEvent {FrameNumber = FrameNumber, PinsKnockedOver = NumOfKnockedOverPins});
                    break;

                case RollFrameEvent re when frameState is PlayedOneRollRegularFrameState:
                    var roll2 = Domain.Roll.Create(this, re.PinsKnockedOver);
                    AddRoll(roll2);
                    Events.Add(frameEvent);
                    Game.RespondToFrameEvents?.Invoke(Game, frameEvent);
                    ChangeState(new PlayedTwoRollsRegularFrameState(this, AddFrameEvent));
                    break;

                case SpareFrameEvent _:
                    Events.Add(frameEvent);
                    Game.RespondToFrameEvents?.Invoke(Game, frameEvent);
                    AddFrameEvent(FrameState,
                        new EndFrameEvent {FrameNumber = FrameNumber, PinsKnockedOver = NumOfKnockedOverPins});
                    break;

                case EndFrameEvent _:
                    Events.Add(frameEvent);
                    Game.RespondToFrameEvents?.Invoke(Game, frameEvent);
                    ChangeState(new CompletedFrameState(this));
                    break;

                default:
                    throw new Exception("Unknown Event");
            }
        }

        protected virtual void ChangeState(IFrameState frameState, Action preActions = null, Action postActions = null)
        {
            FrameState = frameState;
            switch (frameState)
            {
                case PlayedOneRollRegularFrameState _ when !IsStrike:
                    AddFrameEvent(FrameState,
                        new StartOfSecondRollFrameEvent
                        {
                            FrameNumber = _frameNumber, PinsKnockedOver = NumOfKnockedOverPins,
                            PinsRemaining = NumOfRemainingPins
                        });
                    break;

                case PlayedOneRollRegularFrameState _:
                case PlayedTwoRollsRegularFrameState _:
                    break;

                case CompletedFrameState _ when FrameNumber < 10:
                    var advanceGameFrameEvent = new AdvanceFrameGameEvent
                        {FrameNumber = FrameNumber, NewFrameNumber = FrameNumber + 1};
                    AddGameEvent(advanceGameFrameEvent, null);
                    break;

                case CompletedFrameState _:
                    var endGameEvent = new EndGameEvent {FrameNumber = FrameNumber};
                    AddGameEvent(endGameEvent, null);
                    break;

                default:
                    throw new Exception("Unknown State");
            }

            //_frameState = frameState;
        }

        protected void AddRoll(Roll roll)
        {
            _rolls.Add(roll);
        }
    }
}