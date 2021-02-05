using System;
using System.Collections.Generic;
using System.Linq;

namespace BowlingGame.Domain
{
    public class Game
    {
        public const int NumOfPinsAtStartOfFrame = 10;

        private readonly List<Frame> _frames = new List<Frame>();
        private readonly Guid _gameId = Guid.NewGuid();

        private IGameState _gameState;
        public Action<Game, FrameEvent> RespondToFrameEvents;

        public bool IsGameCompleted => _gameState is EndGameState;

        public Action<Game, GameEvent> RespondToGameEvents;
         
        private Game(Action<Game, GameEvent> respondToGameEvents = null,
            Action<Game, FrameEvent> respondToFrameEvents = null)
        {
            RespondToFrameEvents = respondToFrameEvents;
            RespondToGameEvents = respondToGameEvents;
            _gameState = new StartOfGameState(this, AddGameEvent);
            AddGameEvent(new StartedGameEvent {FrameNumber = 1}, _gameState);

            AddFrame(Frame.Create(this, 1, AddGameEvent));
            CurrentFrame = _frames[_frames.Count - 1];
        }

        public List<Frame> Frames => _frames.ToList();
        public Frame PreviousFrame => _frames[^2];
        public Frame CurrentFrame { get; private set; }

        public List<GameEvent> Events { get; set; } = new List<GameEvent>();

        public static Game Create(Action<Game, GameEvent> respondToGameEvents = null,
            Action<Game, FrameEvent> respondToFrameEvents = null)
        {
            return new Game(respondToGameEvents, respondToFrameEvents);
        }

        private void ChangeState(IGameState gameState)
        {
            _gameState = gameState;
        }

        private void AddFrame(Frame frame)
        {
            _frames.Add(frame);
        }

        public Game Roll(int pinsKnockedOver)
        {
            _gameState.Roll(pinsKnockedOver);
            return this;
        }

        public int GetScore()
        {
            return Frames.Sum(x => x.GetScore());
        }

        private void AddGameEvent(GameEvent gameEvent, IGameState gameState = null)
        {
            switch (gameEvent)
            {
                case StartedGameEvent _:
                    Events.Add(gameEvent);
                    RespondToGameEvents?.Invoke(this, gameEvent);
                    ChangeState(new GameInProgressState(this, AddGameEvent));
                    break;

                case AdvanceFrameGameEvent {NewFrameNumber: 10}:
                    Events.Add(gameEvent);
                    RespondToGameEvents?.Invoke(this, gameEvent);
                    AddFrame(LastFrame.Create(this, AddGameEvent));
                    CurrentFrame = _frames[^1]; // gets next frame
                    break;

                case AdvanceFrameGameEvent _:
                    Events.Add(gameEvent);
                    RespondToGameEvents?.Invoke(this, gameEvent);
                    AddFrame(Frame.Create(this, CurrentFrame.FrameNumber + 1, AddGameEvent));
                    CurrentFrame = _frames[^1]; // gets next frame
                    break;

                case EndGameEvent _:
                    Events.Add(gameEvent);
                    RespondToGameEvents?.Invoke(this, gameEvent);
                    ChangeState(new EndGameState(this, AddGameEvent));
                    break;

                default:
                    throw new Exception("Unknown Event");
            }
        }
    }
}