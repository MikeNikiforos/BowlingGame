using BowlingGame.Domain;
using BowlingGame.Tests.Infrastructure;
using System.Linq;
using Xunit;

namespace BowlingGame.Tests
{
    public class GameTests
    {
        [Fact]
        public void EveryNewGameHas10Frames()
        {
            Assert.Equal(10, Game.NumOfPinsAtStartOfFrame);
        }

        [Fact]
        public void EveryNewGameStartsInFrame1()
        {
            var sut = Game.Create();
            Assert.Equal(1, sut.CurrentFrame.FrameNumber);
        }

        [Fact]
        public void CannotAdvancePast10frames()
        {
            var game = Game.Create();
            var sut = game.CurrentFrame;
            for (int i = 1; i < 10; i++)
            {
                game.Roll(10);
                Assert.NotEqual(sut, game.CurrentFrame);
                Assert.Equal(sut, game.PreviousFrame);
                Assert.Equal(i + 1, game.CurrentFrame.FrameNumber);
                sut = game.CurrentFrame;
            }

            game.Roll(10).Roll(10).Roll(10);
            Assert.Equal(sut, game.CurrentFrame);
            Assert.NotEqual(sut, game.PreviousFrame);
            Assert.Equal(10, game.CurrentFrame.FrameNumber);

            var ex = Assert.Throws<InvalidGameState>(() => game.Roll(1));
            Assert.Equal("Invalid state: game is completed", ex.Message);
        }

        [Theory]
        [Repeat(1, 10)]
        public void GameAdvancesFrameOnStrike(int numOfFrames)
        {
            var game = Game.Create();
            var sut = game.CurrentFrame;
            for (int i = 0; i < numOfFrames - 1; i++)
            {
                game.Roll(10);
                Assert.NotEqual(sut, game.CurrentFrame);
                Assert.Equal(sut, game.PreviousFrame);
                Assert.Equal(i + 2, game.CurrentFrame.FrameNumber);
                sut = game.CurrentFrame;
            }
        }

        [Fact]
        public void PlayedPerfectGame()
        {
            var sut = Game.Create();

            //Frame 1
            sut.Roll(10);
            Assert.Equal(10, sut.Frames[0].GetScore());
            Assert.Equal(10, sut.GetScore());

            //Frame 2
            sut.Roll(10);
            Assert.Equal(10, sut.Frames[1].GetScore());
            Assert.Equal(20, sut.Frames[0].GetScore());
            Assert.Equal(30, sut.GetScore());

            //Frame 3
            sut.Roll(10);
            Assert.Equal(10, sut.Frames[2].GetScore());
            Assert.Equal(20, sut.Frames[1].GetScore());
            Assert.Equal(30, sut.Frames[0].GetScore());
            Assert.Equal(60, sut.GetScore());

            //Frame 4
            sut.Roll(10);
            Assert.Equal(10, sut.Frames[3].GetScore());
            Assert.Equal(20, sut.Frames[2].GetScore());
            Assert.Equal(30, sut.Frames[1].GetScore());
            Assert.Equal(30, sut.Frames[0].GetScore());
            Assert.Equal(90, sut.GetScore());

            //Frame 5
            sut.Roll(10);
            Assert.Equal(10, sut.Frames[4].GetScore());
            Assert.Equal(20, sut.Frames[3].GetScore());
            Assert.Equal(30, sut.Frames[2].GetScore());
            Assert.Equal(30, sut.Frames[1].GetScore());
            Assert.Equal(30, sut.Frames[0].GetScore());
            Assert.Equal(120, sut.GetScore());

            //Frame 6
            sut.Roll(10);
            Assert.Equal(10, sut.Frames[5].GetScore());
            Assert.Equal(20, sut.Frames[4].GetScore());
            Assert.Equal(30, sut.Frames[3].GetScore());
            Assert.Equal(30, sut.Frames[2].GetScore());
            Assert.Equal(30, sut.Frames[1].GetScore());
            Assert.Equal(30, sut.Frames[0].GetScore());
            Assert.Equal(150, sut.GetScore());

            //Frame 7
            sut.Roll(10);
            Assert.Equal(10, sut.Frames[6].GetScore());
            Assert.Equal(20, sut.Frames[5].GetScore());
            Assert.Equal(30, sut.Frames[4].GetScore());
            Assert.Equal(30, sut.Frames[3].GetScore());
            Assert.Equal(30, sut.Frames[2].GetScore());
            Assert.Equal(30, sut.Frames[1].GetScore());
            Assert.Equal(30, sut.Frames[0].GetScore());
            Assert.Equal(180, sut.GetScore());

            //Frame 8
            sut.Roll(10);
            Assert.Equal(10, sut.Frames[7].GetScore());
            Assert.Equal(20, sut.Frames[6].GetScore());
            Assert.Equal(30, sut.Frames[5].GetScore());
            Assert.Equal(30, sut.Frames[4].GetScore());
            Assert.Equal(30, sut.Frames[3].GetScore());
            Assert.Equal(30, sut.Frames[2].GetScore());
            Assert.Equal(30, sut.Frames[1].GetScore());
            Assert.Equal(30, sut.Frames[0].GetScore());
            Assert.Equal(210, sut.GetScore());

            //Frame 9
            sut.Roll(10);
            Assert.Equal(10, sut.Frames[8].GetScore());
            Assert.Equal(20, sut.Frames[7].GetScore());
            Assert.Equal(30, sut.Frames[6].GetScore());
            Assert.Equal(30, sut.Frames[5].GetScore());
            Assert.Equal(30, sut.Frames[4].GetScore());
            Assert.Equal(30, sut.Frames[3].GetScore());
            Assert.Equal(30, sut.Frames[2].GetScore());
            Assert.Equal(30, sut.Frames[1].GetScore());
            Assert.Equal(30, sut.Frames[0].GetScore());
            Assert.Equal(240, sut.GetScore());

            //Frame 10 Roll 1
            sut.Roll(10);
            Assert.Equal(10, sut.Frames[9].GetScore());
            Assert.Equal(20, sut.Frames[8].GetScore());
            Assert.Equal(30, sut.Frames[7].GetScore());
            Assert.Equal(30, sut.Frames[6].GetScore());
            Assert.Equal(30, sut.Frames[5].GetScore());
            Assert.Equal(30, sut.Frames[4].GetScore());
            Assert.Equal(30, sut.Frames[3].GetScore());
            Assert.Equal(30, sut.Frames[2].GetScore());
            Assert.Equal(30, sut.Frames[1].GetScore());
            Assert.Equal(30, sut.Frames[0].GetScore());
            Assert.Equal(270, sut.GetScore());

            //Frame 10 Roll 2
            sut.Roll(10);
            Assert.Equal(20, sut.Frames[9].GetScore());
            Assert.Equal(30, sut.Frames[8].GetScore());
            Assert.Equal(30, sut.Frames[7].GetScore());
            Assert.Equal(30, sut.Frames[6].GetScore());
            Assert.Equal(30, sut.Frames[5].GetScore());
            Assert.Equal(30, sut.Frames[4].GetScore());
            Assert.Equal(30, sut.Frames[3].GetScore());
            Assert.Equal(30, sut.Frames[2].GetScore());
            Assert.Equal(30, sut.Frames[1].GetScore());
            Assert.Equal(30, sut.Frames[0].GetScore());
            Assert.Equal(290, sut.GetScore());

            //Frame 10 Roll 3
            sut.Roll(10);
            Assert.Equal(30, sut.Frames[9].GetScore());
            Assert.Equal(30, sut.Frames[8].GetScore());
            Assert.Equal(30, sut.Frames[7].GetScore());
            Assert.Equal(30, sut.Frames[6].GetScore());
            Assert.Equal(30, sut.Frames[5].GetScore());
            Assert.Equal(30, sut.Frames[4].GetScore());
            Assert.Equal(30, sut.Frames[3].GetScore());
            Assert.Equal(30, sut.Frames[2].GetScore());
            Assert.Equal(30, sut.Frames[1].GetScore());
            Assert.Equal(30, sut.Frames[0].GetScore());
            Assert.Equal(300, sut.GetScore());

            Assert.Equal(11, sut.Events.Count);
            sut.Events.Take(1).All(x => Assert.IsType<StartedGameEvent>(x) != null);
            sut.Events.Skip(1).Take(9).All(x => Assert.IsType<AdvanceFrameGameEvent>(x) != null);
            sut.Events.Skip(10).Take(1).All(x => Assert.IsType<EndGameEvent>(x) != null);

        }

        [Fact]
        public void PlayedAllSparesWith5AsLastRoll()
        {
            var game = Game.Create();
            var sut = game.CurrentFrame;

            //Frame 1
            game.Roll(5).Roll(5);
            Assert.Equal(10, game.Frames[0].GetScore());
            Assert.Equal(10, game.GetScore());

            //Frame 2
            game.Roll(5).Roll(5);
            Assert.Equal(10, game.Frames[1].GetScore());
            Assert.Equal(15, game.Frames[0].GetScore());
            Assert.Equal(25, game.GetScore());

            //Frame 3
            game.Roll(5).Roll(5);
            Assert.Equal(10, game.Frames[2].GetScore());
            Assert.Equal(15, game.Frames[1].GetScore());
            Assert.Equal(15, game.Frames[0].GetScore());
            Assert.Equal(40, game.GetScore());

            //Frame 4
            game.Roll(5).Roll(5);
            Assert.Equal(10, game.Frames[3].GetScore());
            Assert.Equal(15, game.Frames[2].GetScore());
            Assert.Equal(15, game.Frames[1].GetScore());
            Assert.Equal(15, game.Frames[0].GetScore());
            Assert.Equal(55, game.GetScore());

            //Frame 5
            game.Roll(5).Roll(5);
            Assert.Equal(10, game.Frames[4].GetScore());
            Assert.Equal(15, game.Frames[3].GetScore());
            Assert.Equal(15, game.Frames[2].GetScore());
            Assert.Equal(15, game.Frames[1].GetScore());
            Assert.Equal(15, game.Frames[0].GetScore());
            Assert.Equal(70, game.GetScore());

            //Frame 6
            game.Roll(5).Roll(5);
            Assert.Equal(10, game.Frames[5].GetScore());
            Assert.Equal(15, game.Frames[4].GetScore());
            Assert.Equal(15, game.Frames[3].GetScore());
            Assert.Equal(15, game.Frames[2].GetScore());
            Assert.Equal(15, game.Frames[1].GetScore());
            Assert.Equal(15, game.Frames[0].GetScore());
            Assert.Equal(85, game.GetScore());

            //Frame 7
            game.Roll(5).Roll(5);
            Assert.Equal(10, game.Frames[6].GetScore());
            Assert.Equal(15, game.Frames[5].GetScore());
            Assert.Equal(15, game.Frames[4].GetScore());
            Assert.Equal(15, game.Frames[3].GetScore());
            Assert.Equal(15, game.Frames[2].GetScore());
            Assert.Equal(15, game.Frames[1].GetScore());
            Assert.Equal(15, game.Frames[0].GetScore());
            Assert.Equal(100, game.GetScore());

            //Frame 8
            game.Roll(5).Roll(5);
            Assert.Equal(10, game.Frames[7].GetScore());
            Assert.Equal(15, game.Frames[6].GetScore());
            Assert.Equal(15, game.Frames[5].GetScore());
            Assert.Equal(15, game.Frames[4].GetScore());
            Assert.Equal(15, game.Frames[3].GetScore());
            Assert.Equal(15, game.Frames[2].GetScore());
            Assert.Equal(15, game.Frames[1].GetScore());
            Assert.Equal(15, game.Frames[0].GetScore());
            Assert.Equal(115, game.GetScore());

            //Frame 9
            game.Roll(5).Roll(5);
            Assert.Equal(10, game.Frames[8].GetScore());
            Assert.Equal(15, game.Frames[7].GetScore());
            Assert.Equal(15, game.Frames[6].GetScore());
            Assert.Equal(15, game.Frames[5].GetScore());
            Assert.Equal(15, game.Frames[4].GetScore());
            Assert.Equal(15, game.Frames[3].GetScore());
            Assert.Equal(15, game.Frames[2].GetScore());
            Assert.Equal(15, game.Frames[1].GetScore());
            Assert.Equal(15, game.Frames[0].GetScore());
            Assert.Equal(130, game.GetScore());

            //Frame 10 Roll 1
            game.Roll(5);
            Assert.Equal(5, game.Frames[9].GetScore());
            Assert.Equal(15, game.Frames[8].GetScore());
            Assert.Equal(15, game.Frames[7].GetScore());
            Assert.Equal(15, game.Frames[6].GetScore());
            Assert.Equal(15, game.Frames[5].GetScore());
            Assert.Equal(15, game.Frames[4].GetScore());
            Assert.Equal(15, game.Frames[3].GetScore());
            Assert.Equal(15, game.Frames[2].GetScore());
            Assert.Equal(15, game.Frames[1].GetScore());
            Assert.Equal(15, game.Frames[0].GetScore());
            Assert.Equal(140, game.GetScore());

            //Frame 10 Roll 2
            game.Roll(5);
            Assert.Equal(10, game.Frames[9].GetScore());
            Assert.Equal(15, game.Frames[8].GetScore());
            Assert.Equal(15, game.Frames[7].GetScore());
            Assert.Equal(15, game.Frames[6].GetScore());
            Assert.Equal(15, game.Frames[5].GetScore());
            Assert.Equal(15, game.Frames[4].GetScore());
            Assert.Equal(15, game.Frames[3].GetScore());
            Assert.Equal(15, game.Frames[2].GetScore());
            Assert.Equal(15, game.Frames[1].GetScore());
            Assert.Equal(15, game.Frames[0].GetScore());
            Assert.Equal(145, game.GetScore());

            //Frame 10 Roll 3
            game.Roll(5);
            Assert.Equal(15, game.Frames[9].GetScore());
            Assert.Equal(15, game.Frames[8].GetScore());
            Assert.Equal(15, game.Frames[7].GetScore());
            Assert.Equal(15, game.Frames[6].GetScore());
            Assert.Equal(15, game.Frames[5].GetScore());
            Assert.Equal(15, game.Frames[4].GetScore());
            Assert.Equal(15, game.Frames[3].GetScore());
            Assert.Equal(15, game.Frames[2].GetScore());
            Assert.Equal(15, game.Frames[1].GetScore());
            Assert.Equal(15, game.Frames[0].GetScore());
            Assert.Equal(150, game.GetScore());
        }
    }
}