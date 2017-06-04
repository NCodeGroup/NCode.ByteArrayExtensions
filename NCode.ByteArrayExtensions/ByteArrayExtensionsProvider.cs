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
using System.Linq;

namespace NCode.ByteArrayExtensions
{
    /// <summary>
    /// Provides the implementation for the <see cref="ByteArrayExtensions"/> methods.
    /// </summary>
    public interface IByteArrayExtensionsProvider
    {
        /// <summary>
        /// Converts the specified byte array into a hexadecimal string.
        /// </summary>
        /// <param name="array">The byte array to convert into a hexadecimal string.</param>
        /// <param name="prefix"><c>true</c> to prefix the string with <c>0x</c>.</param>
        /// <param name="uppercase"><c>true</c> to return all upper case characters.</param>
        /// <returns>The byte array converted into a hexadecimal string.</returns>
        string ToHex(byte[] array, bool prefix, bool uppercase);

        /// <summary>
        /// Returns <c>true</c> or <c>false</c> whether the two byte arrays are equal.
        /// </summary>
        bool Equals(byte[] buffer1, byte[] buffer2);
    }

    /// <summary>
    /// Provides the default implementation for <see cref="IByteArrayExtensionsProvider"/>.
    /// Can be used to register in a DI container.
    /// </summary>
    public class ByteArrayExtensionsProvider : IByteArrayExtensionsProvider
    {
        #region ToHex Members

        private static int _stackAllocThreshold = 2048;

        /// <summary>
        /// Used in <see cref="ToHex"/> for when to allocate the buffer for a
        /// string on the stack vs heap. Please be careful with this setting,
        /// you have been warned. See the following for stack sizes in C# and
        /// in general:
        /// https://stackoverflow.com/questions/28656872/why-is-stack-size-in-c-sharp-exactly-1-mb
        /// </summary>
        public static int StackAllocThreshold
        {
            get => _stackAllocThreshold;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
                _stackAllocThreshold = value;
            }
        }

        // 0-9
        private static IEnumerable<int> Base10 => Enumerable.Range('0', 10);

        // 0-9 and A-F
        private static readonly int[] HexLookupUpper =
            Base10.Concat(Enumerable.Range('A', 6)).ToArray();

        // 0-9 and a-f
        private static readonly int[] HexLookupLower =
            Base10.Concat(Enumerable.Range('a', 6)).ToArray();

        /// <inheritdoc />
        public virtual string ToHex(byte[] array, bool prefix, bool uppercase)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            var len = array.Length;
            if (len == 0)
                return prefix ? "0x" : string.Empty;

            if (prefix)
                len += 1; // 0x (we add 1 vs 2 because we multiply by 2 later)

            var lookup = uppercase ? HexLookupUpper : HexLookupLower;

            return len <= StackAllocThreshold
                ? ToHexStack(len, array, prefix, lookup)
                : ToHexHeap(len, array, prefix, lookup);
        }

        public virtual unsafe string ToHexStack(int len, byte[] array, bool prefix, IReadOnlyList<int> lookup)
        {
            var target = stackalloc char[len * 2];
            return ToHexCore(len, array, target, prefix, lookup);
        }

        public virtual unsafe string ToHexHeap(int len, byte[] array, bool prefix, IReadOnlyList<int> lookup)
        {
            fixed (char* target = new char[len * 2])
            {
                return ToHexCore(len, array, target, prefix, lookup);
            }
        }

        /// <remarks>
        /// http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/3928b8cb-3703-4672-8ccd-33718148d1e3/
        /// </remarks>
        private static unsafe string ToHexCore(int len, byte[] array, char* target, bool prefix,
            IReadOnlyList<int> lookup)
        {
            fixed (byte* source = array)
            {
                var i = 0;
                var pIn = source;
                var pOut = target;

                if (prefix)
                {
                    *pOut++ = (char) 0x30; // '0'
                    *pOut++ = (char) 0x78; // 'x'
                }

                while (i++ < len)
                {
                    *pOut++ = (char) lookup[*pIn >> 4];
                    *pOut++ = (char) lookup[*pIn++ & 0xF];
                }
            }

            return new string(target, 0, len * 2);
        }

        #endregion

        #region Equals Members

        /// <inheritdoc />
        public virtual bool Equals(byte[] buffer1, byte[] buffer2)
        {
            if (ReferenceEquals(buffer1, buffer2)) return true;
            if (ReferenceEquals(buffer1, null)) return false;
            if (ReferenceEquals(buffer2, null)) return false;

            if (buffer1.Length != buffer2.Length)
                return false;

            if (buffer1.Length == 0)
                return true;

            return Equals(buffer1, buffer2, buffer1.Length);
        }

        public virtual unsafe bool Equals(byte[] buffer1, byte[] buffer2, int length)
        {
            fixed (byte* pBuffer1 = buffer1)
            fixed (byte* pBuffer2 = buffer2)
            {
                int num;
                var iter1 = pBuffer1;
                var iter2 = pBuffer2;

                for (num = length >> 2; num > 0; num--)
                {
                    if (*((int*) iter1) != *((int*) iter2)) return false;

                    iter1 += 4;
                    iter2 += 4;
                }

                for (num = length & 3; num > 0; num--)
                {
                    if (iter1[0] != iter2[0]) return false;

                    iter1++;
                    iter2++;
                }
            }
            return true;
        }

        #endregion
    }
}