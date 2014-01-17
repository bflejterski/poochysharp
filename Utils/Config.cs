// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Config.cs" company="Bartek Flejterski">
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
// <summary>
//   Utility class for reading keys from app.config using generics.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Utils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Linq;

    /// <summary>
    ///     Utility class for reading keys from app.config using generics.
    /// </summary>
    public static class Config
    {
        #region Public Properties

        /// <summary>
        ///     Gets the all keys from
        /// </summary>
        public static IEnumerable<string> AllKeys
        {
            get
            {
                return ConfigurationManager.AppSettings.AllKeys;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets key from app.config converted to the given type
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="AppSettingsKeyNotFound">
        /// </exception>
        /// <exception cref="TypeConverterNotFound">
        /// </exception>
        public static T ReadKey<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            if (!ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                throw new PoochyUtilsException(
                    string.Format("Key [{0}] has not been found in the configuration file.", key));
            }

            try
            {
                object convertFrom =
                    TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(ConfigurationManager.AppSettings[key]);
                if (convertFrom != null)
                {
                    return (T)convertFrom;
                }
            }
            catch (NotSupportedException e)
            {
                throw new PoochyUtilsException(string.Format("There's no default converter for [{0}].", typeof(T)), e);
            }

            throw new PoochyUtilsException(string.Format("There's no default converter for [{0}].", typeof(T)));
        }

        #endregion
    }
}