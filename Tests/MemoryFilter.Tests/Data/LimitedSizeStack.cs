using FluentAssertions;
using MemoryFilter.Data;
using Xunit;

namespace MemoryFilter.Tests.Data {

    public class MaxStackTests {
        [Fact]
        public void AddLessThanMaxCapacity_CountMatchesCountOfInsertedValues() {
            var collection = new DistinctMaxStack<int>(3);
            collection.Push(1);
            collection.Push(2);

            collection.Count.Should().Be(2);
        }

        [Fact]
        public void AddMoreThanMaxCapacity_SizeIsNotIncreased() {
            var collection = new DistinctMaxStack<int>(3);
            collection.Push(1);
            collection.Push(2);
            collection.Push(3);
            collection.Push(4);

            collection.Count.Should().Be(3);
        }

        [Fact]
        public void PushTwoOverLimit_RemoveFirstTwoInserted() {
            var collection = new DistinctMaxStack<int>(3);
            collection.Push(1);
            collection.Push(2);
            collection.Push(3);
            collection.Push(4);
            collection.Push(5);

            var expectedCollection = new[] {5, 4, 3};
            collection.Should().BeEquivalentTo(expectedCollection);
        }
    }

}