using BowlingGame.Domain;
using BowlingGame.Tests.Infrastructure;
using System;
using Xunit;

namespace BowlingGame.Tests
{
    public class LastFrameTests
    {
        public Action<GameEvent, IGameState> EmptyAddGameEvent => new Action<GameEvent, IGameState>((x, y) => { });

        [Theory]
        [Repeat(0, 30)]
        public void LastFrameCanOnlyHaveTwoRollsIfNoneIsStrikeOrSpare(int iter)
        {
            Random r = new Random();
            var sut = LastFrame.Create(Game.Create(), EmptyAddGameEvent);
            Assert.Equal(10, sut.FrameNumber);
            var firstRoll = r.Next(0, 10);
            var secondRoll = r.Next(0, 9 - firstRoll);
            sut.Roll(firstRoll);
            sut.Roll(secondRoll); // First anSecond roll should be lass than 10

            Assert.Equal(5, sut.Events.Count);

            {
                var evnt = Assert.IsType<StartOfFirstRollFrameEvent>(sut.Events[0]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }

            {
                var evnt = Assert.IsType<RollFrameEvent>(sut.Events[1]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(firstRoll, evnt.PinsKnockedOver);
            }

            {
                var evnt = Assert.IsType<StartOfSecondRollFrameEvent>(sut.Events[2]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(firstRoll, evnt.PinsKnockedOver);
                Assert.Equal(10 - firstRoll, evnt.PinsRemaining);
            }

            {
                var evnt = Assert.IsType<RollFrameEvent>(sut.Events[3]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(secondRoll, evnt.PinsKnockedOver);
            }

            {
                var evnt = Assert.IsType<EndFrameEvent>(sut.Events[4]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(firstRoll + secondRoll, evnt.PinsKnockedOver);
            }
        }

        [Fact]
        public void LastFrameCanHave3RollsIfAllAreStrikes()
        {
            var sut = LastFrame.Create(Game.Create(), EmptyAddGameEvent);
            Assert.Equal(10, sut.FrameNumber);

            sut.Roll(10);
            sut.Roll(10);
            sut.Roll(10);

            Assert.Equal(10, sut.Events.Count);

            {
                var evnt = Assert.IsType<StartOfFirstRollFrameEvent>(sut.Events[0]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }

            {
                var evnt = Assert.IsType<RollFrameEvent>(sut.Events[1]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(10, evnt.PinsKnockedOver);
            }

            {
                var evnt = Assert.IsType<StrikeFrameEvent>(sut.Events[2]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }

            {
                var evnt = Assert.IsType<StartOfSecondRollFrameEvent>(sut.Events[3]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }

            {
                var evnt = Assert.IsType<RollFrameEvent>(sut.Events[4]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(10, evnt.PinsKnockedOver);
            }

            {
                var evnt = Assert.IsType<StrikeFrameEvent>(sut.Events[5]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }

            {
                var evnt = Assert.IsType<StartOfThirdRollFrameEvent>(sut.Events[6]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }

            {
                var evnt = Assert.IsType<RollFrameEvent>(sut.Events[7]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(10, evnt.PinsKnockedOver);
            }

            {
                var evnt = Assert.IsType<StrikeFrameEvent>(sut.Events[8]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }

            {
                var evnt = Assert.IsType<EndFrameEvent>(sut.Events[9]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(30, evnt.PinsKnockedOver);
            }
        }

        [Fact]
        public void LastFrameCanHave3RollsIfFirstRollIsStrikeButOthersAreZero()
        {
            var sut = LastFrame.Create(Game.Create(), EmptyAddGameEvent);
            Assert.Equal(10, sut.FrameNumber);

            sut.Roll(10);
            sut.Roll(0);
            sut.Roll(0);

            Assert.Equal(8, sut.Events.Count);

            {
                var evnt = Assert.IsType<StartOfFirstRollFrameEvent>(sut.Events[0]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }

            {
                var evnt = Assert.IsType<RollFrameEvent>(sut.Events[1]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(10, evnt.PinsKnockedOver);
            }

            {
                var evnt = Assert.IsType<StrikeFrameEvent>(sut.Events[2]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }

            {
                var evnt = Assert.IsType<StartOfSecondRollFrameEvent>(sut.Events[3]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }

            {
                var evnt = Assert.IsType<RollFrameEvent>(sut.Events[4]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(0, evnt.PinsKnockedOver);
            }

            {
                var evnt = Assert.IsType<StartOfThirdRollFrameEvent>(sut.Events[5]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }

            {
                var evnt = Assert.IsType<RollFrameEvent>(sut.Events[6]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(0, evnt.PinsKnockedOver);
            }

            {
                var evnt = Assert.IsType<EndFrameEvent>(sut.Events[7]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(10, evnt.PinsKnockedOver);
            }
        }

        [Theory]
        [Repeat(0, 10)]
        public void LastFrameCanHave3RollsIfFirstRollsRollAreStrikesButIsnt(int numofpinsknocked)
        {
            //Random r = new Random();
            var sut = LastFrame.Create(Game.Create(), EmptyAddGameEvent);
            Assert.Equal(10, sut.FrameNumber);

            sut.Roll(10);
            sut.Roll(10);
            sut.Roll(numofpinsknocked);

            Assert.Equal(9, sut.Events.Count);

            {
                var evnt = Assert.IsType<StartOfFirstRollFrameEvent>(sut.Events[0]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }

            {
                var evnt = Assert.IsType<RollFrameEvent>(sut.Events[1]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(10, evnt.PinsKnockedOver);
            }

            {
                var evnt = Assert.IsType<StrikeFrameEvent>(sut.Events[2]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }

            {
                var evnt = Assert.IsType<StartOfSecondRollFrameEvent>(sut.Events[3]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }

            {
                var evnt = Assert.IsType<RollFrameEvent>(sut.Events[4]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(10, evnt.PinsKnockedOver);
            }

            {
                var evnt = Assert.IsType<StrikeFrameEvent>(sut.Events[5]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }
            {
                var evnt = Assert.IsType<StartOfThirdRollFrameEvent>(sut.Events[6]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }

            {
                var evnt = Assert.IsType<RollFrameEvent>(sut.Events[7]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(numofpinsknocked, evnt.PinsKnockedOver);
            }

            {
                var evnt = Assert.IsType<EndFrameEvent>(sut.Events[8]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(20 + numofpinsknocked, evnt.PinsKnockedOver);
            }
        }

        [Theory]
        [Repeat(0, 100)]
        public void LastFrameCanHave3RollsIfFirstTwoRollsAreSpareButLastRollNotStrike(int iter)
        {
            Random r = new Random();
            var sut = LastFrame.Create(Game.Create(), EmptyAddGameEvent);
            Assert.Equal(10, sut.FrameNumber);

            var firstRoll = r.Next(0, 10);
            sut.Roll(firstRoll);
            sut.Roll(10 - firstRoll);
            var thirdroll = r.Next(0, 10);
            sut.Roll(thirdroll);

            Assert.Equal(8, sut.Events.Count);

            {
                var evnt = Assert.IsType<StartOfFirstRollFrameEvent>(sut.Events[0]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }

            {
                var evnt = Assert.IsType<RollFrameEvent>(sut.Events[1]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(firstRoll, evnt.PinsKnockedOver);
            }

            {
                var evnt = Assert.IsType<StartOfSecondRollFrameEvent>(sut.Events[2]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }

            {
                var evnt = Assert.IsType<RollFrameEvent>(sut.Events[3]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(10 - firstRoll, evnt.PinsKnockedOver);
            }

            {
                var evnt = Assert.IsType<SpareFrameEvent>(sut.Events[4]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }

            {
                var evnt = Assert.IsType<StartOfThirdRollFrameEvent>(sut.Events[5]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }

            {
                var evnt = Assert.IsType<RollFrameEvent>(sut.Events[6]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(thirdroll, evnt.PinsKnockedOver);
            }

            {
                var evnt = Assert.IsType<EndFrameEvent>(sut.Events[7]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(10 + thirdroll, evnt.PinsKnockedOver);
            }
        }
    }
}