// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigTests.cs" company="Bartek Flejterski">
//   The MIT License (MIT)
//   
//   Copyright (c) 2014 Bartek Flejterski
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;

namespace Utils.Test
{
    /// <summary>
    ///     The config tests.
    /// </summary>
    [TestClass]
    public class ConfigTests
    {
        /// <summary>
        ///     The test method 1.
        /// </summary>
        [TestMethod]
        public void ReadIntTest()
        {
            var result = Config.ReadKey<int>("int");
            Check.That(result).IsEqualTo(7);
        }

        /// <summary>
        ///     The read double test.
        /// </summary>
        [TestMethod]
        public void ReadDoubleTest()
        {
            var result = Config.ReadKey<double>("double");
            Check.That(result).IsEqualTo(2.5);
        }

        /// <summary>
        ///     The read string test.
        /// </summary>
        [TestMethod]
        public void ReadStringTest()
        {
            var result = Config.ReadKey<string>("string");
            Check.That(result).IsEqualTo("test");
        }

        /// <summary>
        ///     The read all keys test.
        /// </summary>
        [TestMethod]
        public void ReadAllKeysTest()
        {
            IEnumerable<string> allKeys = Config.AllKeys;
            Check.That(allKeys).ContainsExactly(new[] {"int", "double", "string"});
        }

        /// <summary>
        /// The read not existing key test.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof (UtilsException))]
        public void ReadNotExistingKeyTest()
        {
            Config.ReadKey<string>("notExistingKey");
        }

        /// <summary>
        /// The read key with non existing converter type test.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof (UtilsException))]
        public void ReadKeyWithNonExistingConverterTypeTest()
        {
            Config.ReadKey<NonExistingConverterType>("string");
        }
    }

    /// <summary>
    /// The non existing converter type class - used interally for testing.
    /// </summary>
    internal class NonExistingConverterType
    {
    }
}