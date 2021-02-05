using BowlingGame.Domain;
using BowlingGame.Tests.Infrastructure;
using System;
using Xunit;

namespace BowlingGame.Tests
{
    public class RollTests
    {

        [Fact]
        public void RollCannotBeDeattachedFromFrame()
        {
            Action sut = () => Roll.Create(null, 1);
            var ex = Assert.Throws<InvalidRollState>(sut);
            Assert.Equal("Invalid state: Roll cannot be detached from frame", ex.Message);
        }


        [Theory]
        [InlineData(-1)]
        [InlineData(-10)]
        public void RollLessThan1PinCannotBeCreated(int pinsKnockedOver)
        {
            Action sut = () => Roll.Create(Frame.Create(Game.Create(), 1, null), pinsKnockedOver);
            var ex = Assert.Throws<InvalidRollState>(sut);
            Assert.Equal("Invalid state: Pins knocked over must be between 1 and 10", ex.Message);
        }


        [Theory]
        [InlineData(11)]
        [InlineData(110)]
        [InlineData(1110)]
        public void RollHavingMoreThan10PinsKnockedDownCannotBeCreated(int pinsKnockedOver)
        {
            Action sut = () => Roll.Create(Frame.Create(Game.Create(), 1, null), pinsKnockedOver);
            var ex = Assert.Throws<InvalidRollState>(sut);
            Assert.Equal("Invalid state: Pins knocked over must be between 1 and 10", ex.Message);
        }

        [Theory]
        [Repeat(0, 11)]
        public void RollHavingBetween1To10PinsKnockedDownCanBeCreated(int pinsKnockedOver)
        {
            var sut = Roll.Create(Frame.Create(Game.Create(), 1, null), pinsKnockedOver);
            Assert.Equal(pinsKnockedOver, sut.PinsKnockedOver);
        }
    }
}

