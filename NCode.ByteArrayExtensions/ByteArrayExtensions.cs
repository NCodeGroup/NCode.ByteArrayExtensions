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

namespace NCode.ByteArrayExtensions
{
    /// <summary>
    /// Provides a set of extension methods for byte arrays.
    /// </summary>
    public static class ByteArrayExtensions
    {
        private static IByteArrayExtensionsProvider _provider;

        /// <summary>
        /// Gets or sets the <see cref="IByteArrayExtensionsProvider"/> provider for this extension class.
        /// </summary>
        public static IByteArrayExtensionsProvider Provider
        {
            get => _provider ?? (_provider = new ByteArrayExtensionsProvider());
            set => _provider = value;
        }

        /// <summary>
        /// Converts the specified byte array into a hexadecimal string.
        /// </summary>
        /// <param name="array">The byte array to convert into a hexadecimal string.</param>
        /// <param name="prefix"><c>true</c> to prefix the string with <c>0x</c>.</param>
        /// <param name="uppercase"><c>true</c> to return all upper case characters.</param>
        /// <returns>The byte array converted into a hexadecimal string.</returns>
        public static string ToHex(this byte[] array, bool prefix, bool uppercase)
        {
            return Provider.ToHex(array, prefix, uppercase);
        }

        /// <summary>
        /// Returns <c>true</c> or <c>false</c> whether the two byte arrays are equal.
        /// </summary>
        public static bool Equals(this byte[] buffer1, byte[] buffer2)
        {
            return Provider.Equals(buffer1, buffer2);
        }
    }
}