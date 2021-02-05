using System;

namespace BowlingGame.Domain
{
    public class GameBuilder
    {
        private Action<Game, FrameEvent> _respondToFrameEvents;


        private Action<Game, GameEvent> _respondToGameEvents;

        private GameBuilder()
        {
        }

        public static GameBuilder Create()
        {
            return new GameBuilder();
        }

        public GameBuilder ConfigureGameEventResponses(Action<Game, GameEvent> action)
        {
            _respondToGameEvents = action;
            return this;
        }

        public GameBuilder ConfigureFrameEventResponses(Action<Game, FrameEvent> action)
        {
            _respondToFrameEvents = action;
            return this;
        }

        public Game Build()
        {
            return Game.Create(_respondToGameEvents, _respondToFrameEvents);
        }
    }
}