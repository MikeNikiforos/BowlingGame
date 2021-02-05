using System;
using System.Linq;

namespace BowlingGame.Domain
{
    public class LastFrame : Frame
    {
        private int _numOfPinsRemaining = 10;

        private LastFrame(Game game, Action<GameEvent, IGameState> addGameEvent) : base(game, 10, addGameEvent)
        {
        }

        public override bool IsStrike => throw new Exception();
        public override bool IsSpare => throw new Exception();

        public override int NumOfKnockedOverPins => _rolls.Sum(x => x.PinsKnockedOver);

        public override int NumOfRemainingPins => _numOfPinsRemaining;

        public override int GetScore()
        {
            return NumOfKnockedOverPins;
        }

        public static LastFrame Create(Game game, Action<GameEvent, IGameState> addGameEvent)
        {
            return new LastFrame(game, addGameEvent);
        }

        protected override void AddFrameEvent(IFrameState frameState, FrameEvent frameEvent)
        {
            switch (frameEvent)
            {
                case StartOfFirstRollFrameEvent _:
                    Events.Add(frameEvent);
                    Game.RespondToFrameEvents?.Invoke(Game, frameEvent);
                    break;

                case StartOfSecondRollFrameEvent sf:
                    _numOfPinsRemaining = sf.PinsRemaining;
                    Events.Add(frameEvent);
                    Game.RespondToFrameEvents?.Invoke(Game, frameEvent);
                    break;

                case StartOfThirdRollFrameEvent tr:
                    _numOfPinsRemaining = tr.PinsRemaining;
                    Events.Add(frameEvent);
                    Game.RespondToFrameEvents?.Invoke(Game, frameEvent);
                    break;

                case RollFrameEvent re when frameState is StartOfFrameState:
                    var roll1 = Domain.Roll.Create(this, re.PinsKnockedOver);
                    AddRoll(roll1);
                    Events.Add(frameEvent);
                    Game.RespondToFrameEvents?.Invoke(Game, frameEvent);
                    ChangeState(new PlayedOneRollLastFrameState(this, AddFrameEvent));
                    break;

                case StrikeFrameEvent _ when frameState is PlayedOneRollLastFrameState:
                    Events.Add(frameEvent);
                    Game.RespondToFrameEvents?.Invoke(Game, frameEvent);
                    AddFrameEvent(FrameState,
                        new StartOfSecondRollFrameEvent
                            {FrameNumber = FrameNumber, PinsKnockedOver = 10, PinsRemaining = 10});
                    break;

                case StrikeFrameEvent _ when frameState is PlayedTwoRollsLastFrameState:
                    Events.Add(frameEvent);
                    Game.RespondToFrameEvents?.Invoke(Game, frameEvent);
                    AddFrameEvent(FrameState,
                        new StartOfThirdRollFrameEvent
                            {FrameNumber = FrameNumber, PinsKnockedOver = NumOfKnockedOverPins, PinsRemaining = 10});
                    break;

                case StrikeFrameEvent _ when frameState is PlayedThreeRollsLastFrameState:
                    Events.Add(frameEvent);
                    Game.RespondToFrameEvents?.Invoke(Game, frameEvent);
                    AddFrameEvent(FrameState,
                        new EndFrameEvent {FrameNumber = FrameNumber, PinsKnockedOver = NumOfKnockedOverPins});
                    break;

                case RollFrameEvent re when frameState is PlayedOneRollLastFrameState:
                    var roll2 = Domain.Roll.Create(this, re.PinsKnockedOver);
                    AddRoll(roll2);
                    Events.Add(frameEvent);
                    Game.RespondToFrameEvents?.Invoke(Game, frameEvent);
                    ChangeState(new PlayedTwoRollsLastFrameState(this, AddFrameEvent));
                    break;

                case RollFrameEvent re when frameState is PlayedTwoRollsLastFrameState:
                    var roll3 = Domain.Roll.Create(this, re.PinsKnockedOver);
                    AddRoll(roll3);
                    Events.Add(frameEvent);
                    Game.RespondToFrameEvents?.Invoke(Game, frameEvent);
                    ChangeState(new PlayedThreeRollsLastFrameState(this, AddFrameEvent));
                    break;

                case SpareFrameEvent _ when frameState is PlayedTwoRollsLastFrameState:
                    Events.Add(frameEvent);
                    Game.RespondToFrameEvents?.Invoke(Game, frameEvent);
                    AddFrameEvent(FrameState,
                        new StartOfThirdRollFrameEvent
                            {FrameNumber = FrameNumber, PinsKnockedOver = NumOfKnockedOverPins, PinsRemaining = 10});
                    break;

                case SpareFrameEvent _ when frameState is PlayedThreeRollsLastFrameState:
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

        protected override void ChangeState(IFrameState frameState, Action preActions = null, Action postActions = null)
        {
            FrameState = frameState;
            switch (frameState)
            {
                case PlayedOneRollLastFrameState _ when Rolls[0].PinsKnockedOver < 10:
                    AddFrameEvent(FrameState,
                        new StartOfSecondRollFrameEvent
                        {
                            FrameNumber = _frameNumber, PinsKnockedOver = Rolls[0].PinsKnockedOver,
                            PinsRemaining = 10 - Rolls[0].PinsKnockedOver
                        });
                    break;

                case PlayedOneRollLastFrameState _:
                case PlayedTwoRollsLastFrameState _:
                case PlayedThreeRollsLastFrameState _:
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
        }
    }
}