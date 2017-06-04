#region Copyright Preamble

// 
//    Copyright @ 2017 NCode Group
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

#endregion

using System;
using Moq;
using Xunit;

namespace NCode.ByteArrayExtensions.Tests
{
    public class EqualsTest
    {
        private static readonly byte[] NullBuffer = null;
        private static readonly byte[] EmptyBuffer = Array.Empty<byte>();

        [Fact]
        public void Empty()
        {
            var mockProvider = new Mock<ByteArrayExtensionsProvider>(MockBehavior.Loose) {CallBase = true};
            var provider = mockProvider.Object;

            var result = provider.Equals(EmptyBuffer, EmptyBuffer);
            Assert.True(result);

            mockProvider
                .Verify(_ => _.Equals(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Null()
        {
            var mockProvider = new Mock<ByteArrayExtensionsProvider>(MockBehavior.Loose) {CallBase = true};
            var provider = mockProvider.Object;

            var result = provider.Equals(NullBuffer, NullBuffer);
            Assert.True(result);

            mockProvider
                .Verify(_ => _.Equals(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void NullvsEmpty()
        {
            var mockProvider = new Mock<ByteArrayExtensionsProvider>(MockBehavior.Loose) {CallBase = true};
            var provider = mockProvider.Object;

            var result1 = provider.Equals(NullBuffer, EmptyBuffer);
            Assert.False(result1);

            var result2 = provider.Equals(EmptyBuffer, NullBuffer);
            Assert.False(result2);

            mockProvider
                .Verify(_ => _.Equals(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void SameReference()
        {
            var mockProvider = new Mock<ByteArrayExtensionsProvider>(MockBehavior.Loose) {CallBase = true};
            var provider = mockProvider.Object;

            var buffer = new byte[5];

            var result = provider.Equals(buffer, buffer);
            Assert.True(result);

            mockProvider
                .Verify(_ => _.Equals(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void SameSizeDifferentContents()
        {
            var mockProvider = new Mock<ByteArrayExtensionsProvider>(MockBehavior.Loose) {CallBase = true};
            var provider = mockProvider.Object;

            var buffer1 = new byte[5];
            var buffer2 = new byte[5];

            var random = new Random();
            random.NextBytes(buffer1);
            random.NextBytes(buffer2);

            Assert.NotSame(buffer1, buffer2);

            var result1 = provider.Equals(buffer1, buffer2);
            Assert.False(result1);

            var result2 = provider.Equals(buffer2, buffer1);
            Assert.False(result2);

            mockProvider
                .Verify(_ => _.Equals(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<int>()), Times.Exactly(2));
        }

        [Fact]
        public void SameSizeSameContents()
        {
            var mockProvider = new Mock<ByteArrayExtensionsProvider>(MockBehavior.Loose) {CallBase = true};
            var provider = mockProvider.Object;

            var buffer1 = new byte[5];
            var buffer2 = new byte[5];

            var random = new Random();
            random.NextBytes(buffer1);
            Buffer.BlockCopy(buffer1, 0, buffer2, 0, buffer1.Length);

            Assert.NotSame(buffer1, buffer2);

            var result1 = provider.Equals(buffer1, buffer2);
            Assert.True(result1);

            var result2 = provider.Equals(buffer2, buffer1);
            Assert.True(result2);

            mockProvider
                .Verify(_ => _.Equals(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<int>()), Times.Exactly(2));
        }

        [Fact]
        public void SizesDifferent()
        {
            var mockProvider = new Mock<ByteArrayExtensionsProvider>(MockBehavior.Loose) {CallBase = true};
            var provider = mockProvider.Object;

            var buffer1 = new byte[5];
            var buffer2 = new byte[10];

            var result1 = provider.Equals(buffer1, buffer2);
            Assert.False(result1);

            var result2 = provider.Equals(buffer2, buffer1);
            Assert.False(result2);

            mockProvider
                .Verify(_ => _.Equals(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<int>()), Times.Never);
        }
    }
}