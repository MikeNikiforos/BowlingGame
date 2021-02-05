using System;

namespace BowlingGame.Domain
{
    public interface IGameState
    {
        void Roll(int pinsKnockedOver);
    }

    public class StartOfGameState : IGameState
    {
        private readonly Game _game;

        public StartOfGameState(Game game, Action<GameEvent, IGameState> addGameEvent)
        {
            _game = game;
        }

        public void Roll(int pinsKnockedOver)
        {
            _game.CurrentFrame.Roll(pinsKnockedOver);
        }
    }

    public class GameInProgressState : IGameState
    {
        private readonly Action<GameEvent, IGameState> _addGameEvent;
        private readonly Game _game;

        public GameInProgressState(Game game, Action<GameEvent, IGameState> addGameEvent)
        {
            _game = game;
            _addGameEvent = addGameEvent;
        }

        public void Roll(int pinsKnockedOver)
        {
            _game.CurrentFrame.Roll(pinsKnockedOver);
        }
    }

    public class EndGameState : IGameState
    {
        private readonly Action<GameEvent, IGameState> _addGameEvent;
        private readonly Game _game;

        public EndGameState(Game game, Action<GameEvent, IGameState> addGameEvent)
        {
            _game = game;
            _addGameEvent = addGameEvent;
        }

        public void Roll(int pinsKnockedOver)
        {
            throw new InvalidGameState("Invalid state: game is completed");
        }
    }
}