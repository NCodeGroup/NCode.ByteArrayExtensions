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
using System.Collections.Generic;
using Moq;
using Xunit;

namespace NCode.ByteArrayExtensions.Tests
{
    public class ToHexTests
    {
        private const bool PrefixY = true;
        private const bool PrefixN = false;

        private const bool UCase = true;
        private const bool LCase = false;

        private static byte[] B(params byte[] array)
        {
            return array;
        }

        public static IEnumerable<object[]> TestData_ToHex()
        {
            yield return new object[] {Array.Empty<byte>(), PrefixN, LCase, ""};
            yield return new object[] {Array.Empty<byte>(), PrefixY, LCase, "0x"};
            yield return new object[] {Array.Empty<byte>(), PrefixN, UCase, ""};
            yield return new object[] {Array.Empty<byte>(), PrefixY, UCase, "0x"};

            yield return new object[] {B(0x00, 0x01), PrefixN, LCase, "0001"};
            yield return new object[] {B(0x00, 0x01), PrefixY, LCase, "0x0001"};
            yield return new object[] {B(0x00, 0x01), PrefixN, UCase, "0001"};
            yield return new object[] {B(0x00, 0x01), PrefixY, UCase, "0x0001"};

            yield return new object[] {B(0xaB, 0xCd), PrefixN, LCase, "abcd"};
            yield return new object[] {B(0xaB, 0xCd), PrefixY, LCase, "0xabcd"};
            yield return new object[] {B(0xaB, 0xCd), PrefixN, UCase, "ABCD"};
            yield return new object[] {B(0xaB, 0xCd), PrefixY, UCase, "0xABCD"};
        }

        [Theory]
        [MemberData(nameof(TestData_ToHex))]
        public void ToHex(byte[] array, bool prefix, bool uppercase, string expected)
        {
            var provider = new ByteArrayExtensionsProvider();

            var result = provider.ToHex(array, prefix, uppercase);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToHexHeap()
        {
            var mockProvider = new Mock<ByteArrayExtensionsProvider>(MockBehavior.Loose) {CallBase = true};
            var provider = mockProvider.Object;

            var array = new byte[ByteArrayExtensionsProvider.StackAllocThreshold + 10];
            var result = provider.ToHex(array, PrefixY, UCase);
            var expected = "0x" + new string('0', array.Length * 2);
            Assert.Equal(expected, result);

            mockProvider
                .Verify(
                    _ => _.ToHexStack(It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<bool>(),
                        It.IsAny<IReadOnlyList<int>>()), Times.Never);

            mockProvider
                .Verify(
                    _ => _.ToHexHeap(It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<bool>(),
                        It.IsAny<IReadOnlyList<int>>()), Times.Once);
        }

        [Fact]
        public void ToHexStack()
        {
            var mockProvider = new Mock<ByteArrayExtensionsProvider>(MockBehavior.Loose) {CallBase = true};
            var provider = mockProvider.Object;

            var array = new byte[] {0x00, 0xaB};
            var result = provider.ToHex(array, PrefixY, UCase);
            Assert.Equal("0x00AB", result);

            mockProvider
                .Verify(
                    _ => _.ToHexStack(It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<bool>(),
                        It.IsAny<IReadOnlyList<int>>()), Times.Once);

            mockProvider
                .Verify(_ => _.ToHexHeap(2, array, PrefixY, It.IsAny<IReadOnlyList<int>>()), Times.Never);
        }
    }
}