using BowlingGame.Domain;
using BowlingGame.Tests.Infrastructure;
using System;
using Xunit;

namespace BowlingGame.Tests
{
    public class RegularFrameTests
    {
        public Action<GameEvent, IGameState> EmptyAddGameEvent => new Action<GameEvent, IGameState>((x, y) => { });

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void FrameNumberLessThan1CannotBeCreated(int frameNumber)
        {
            Action sut = () => Frame.Create(Game.Create(), frameNumber, EmptyAddGameEvent);
            var ex = Assert.Throws<InvalidFrameState>(sut);
            Assert.Equal("Invalid state: Frame must be between 1 and 9", ex.Message);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(110)]
        [InlineData(1110)]
        public void RegularFrameNumberMoreThan9CannotBeCreated(int frameNumber)
        {
            Action sut = () => Frame.Create(Game.Create(), frameNumber, EmptyAddGameEvent);
            var ex = Assert.Throws<InvalidFrameState>(sut);
            Assert.Equal("Invalid state: Frame must be between 1 and 9", ex.Message);
        }

        [Theory]
        [Repeat(1, 9)]
        public void RegularFrameNumberBetween1And9CanBeCreated(int frameNumber)
        {
            var sut = Frame.Create(Game.Create(), frameNumber, EmptyAddGameEvent);
            Assert.Equal(frameNumber, sut.FrameNumber);
        }

        [Theory]
        [Repeat(1, 9)]
        public void EveryFrameExceptLastOneBeginsWith10RemainingPins(int frameNumber)
        {
            var sut = Frame.Create(Game.Create(), frameNumber, EmptyAddGameEvent);
            Assert.Equal(10, sut.NumOfRemainingPins);
        }

        [Theory]
        [Repeat(0, 6)]
        public void SecondRollInFrameCanKnockLessThanOrEqualToRemainingPins(int pinsKnockedOver)
        {
            var frame = Frame.Create(Game.Create(), 1, EmptyAddGameEvent).Roll(5);
            //There should be no remaining pins left

            var sut = frame.Roll(pinsKnockedOver);
            Assert.Equal(5 - pinsKnockedOver, sut.NumOfRemainingPins);
        }

        [Theory]
        [Repeat(6, 5)]
        public void SecondRollInFrameCannotKnockMoreThanRemainingPins(int pinsKnockedOver)
        {
            var frame = Frame.Create(Game.Create(), 1, EmptyAddGameEvent).Roll(5);
            //There should be no remaining pins left

            Action sut = () => frame.Roll(pinsKnockedOver);
            var ex = Assert.Throws<InvalidFrameState>(sut);
            Assert.Equal($"Invalid state: A roll cannot knock down more than remaining pins in frame. Remaining pins are 5", ex.Message);
        }

        [Fact]
        public void CannotRollAThirdTimeInASingleFrame()
        {
            //There should be no remaining pins left
            Action sut = () => Frame.Create(Game.Create(), 1, EmptyAddGameEvent).Roll(1).Roll(1).Roll(1);
            var ex = Assert.Throws<InvalidFrameState>(sut);
            Assert.Equal("Invalid state: Cannot roll more than 2 times in a single frame", ex.Message);
        }

        [Fact]
        public void OneRollThatKnocksAll10PinsIsAStrike()
        {
            var game = Game.Create();
            var sut = game.CurrentFrame;
            game.Roll(10);
            Assert.True(sut.IsStrike);
            Assert.Equal(4, sut.Events.Count);

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
                var evnt = Assert.IsType<EndFrameEvent>(sut.Events[3]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(10, evnt.PinsKnockedOver);
            }

            Assert.NotEqual(sut, game.CurrentFrame);
            Assert.Equal(2, game.CurrentFrame.FrameNumber);
        }

        [Fact]
        public void TwoRollsThatKnocksAll10PinsIsASpare()
        {
            var game = Game.Create();
            var sut = game.CurrentFrame;
            game.Roll(9).Roll(1);
            Assert.True(sut.IsSpare);
            Assert.Equal(6, sut.Events.Count);

            {
                var evnt = Assert.IsType<StartOfFirstRollFrameEvent>(sut.Events[0]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }

            {
                var evnt = Assert.IsType<RollFrameEvent>(sut.Events[1]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(9, evnt.PinsKnockedOver);
            }

            {
                var evnt = Assert.IsType<StartOfSecondRollFrameEvent>(sut.Events[2]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(9, evnt.PinsKnockedOver);
                Assert.Equal(1, evnt.PinsRemaining);
            }

            {
                var evnt = Assert.IsType<RollFrameEvent>(sut.Events[3]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(1, evnt.PinsKnockedOver);
            }

            {
                var evnt = Assert.IsType<SpareFrameEvent>(sut.Events[4]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
            }

            {
                var evnt = Assert.IsType<EndFrameEvent>(sut.Events[5]);
                Assert.Equal(sut.FrameNumber, evnt.FrameNumber);
                Assert.Equal(10, evnt.PinsKnockedOver);
            }

            Assert.NotEqual(sut, game.CurrentFrame);
            Assert.Equal(2, game.CurrentFrame.FrameNumber);
        }

        [Theory]
        [Repeat(0, 30)]
        public void TwoRollsThatKnocksLessThan10PinsIsASpare(int iter)
        {
            Random r = new Random();
            var frameNum = r.Next(1, 10);
            var sut = Frame.Create(Game.Create(), frameNum, EmptyAddGameEvent);
            Assert.Equal(frameNum, sut.FrameNumber);
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
    }
}