// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PoochyWebClient.cs" company="Bartek Flejterski">
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
//   The poochy web client supporting cookies.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Web
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text;

    /// <summary>
    ///     The poochy web client supporting cookies (Bear in mind javascript is not supported).
    /// </summary>
    public class PoochyWebClient : WebClient
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PoochyWebClient" /> class.
        /// </summary>
        public PoochyWebClient()
            : this(new CookieContainer())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PoochyWebClient"/> class.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        public PoochyWebClient(CookieContainer c)
        {
            this.CookieContainer = c;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the cookie container.
        /// </summary>
        public CookieContainer CookieContainer { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The trust all certificate. Use this method with caution
        /// </summary>
        public void TrustAllCertificate()
        {
            ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, sslPolicyErrors) => true;
        }

        /// <summary>
        /// Uploads the files.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="files">
        /// The files.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public byte[] UploadFiles(string address, IEnumerable<PoochyFile> files, Dictionary<string, string> values)
        {
            WebRequest request = this.GetWebRequest(new Uri(address));
            if (request == null)
            {
                throw new PoochyWebException("Request couldn't be obtained.");
            }

            request.Method = "POST";
            string boundary = "---------------------------"
                              + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            boundary = "--" + boundary;
            using (var memoryStream = new MemoryStream())
            {
                foreach (string name in values.Keys)
                {
                    byte[] buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    memoryStream.Write(buffer, 0, buffer.Length);
                    buffer =
                        Encoding.ASCII.GetBytes(
                            string.Format(
                                "Content-Disposition: form-data; name=\"{0}\"{1}{1}", 
                                name, 
                                Environment.NewLine));
                    memoryStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(values[name] + Environment.NewLine);
                    memoryStream.Write(buffer, 0, buffer.Length);
                }

                foreach (PoochyFile file in files)
                {
                    byte[] buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    memoryStream.Write(buffer, 0, buffer.Length);
                    buffer =
                        Encoding.UTF8.GetBytes(
                            string.Format(
                                "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}", 
                                file.Name, 
                                file.Name, 
                                Environment.NewLine));
                    memoryStream.Write(buffer, 0, buffer.Length);
                    buffer =
                        Encoding.ASCII.GetBytes(
                            string.Format("Content-Type: {0}{1}{1}", file.ContentType, Environment.NewLine));
                    memoryStream.Write(buffer, 0, buffer.Length);
                    memoryStream.Write(file.Content, 0, file.Content.Length);
                    buffer = Encoding.ASCII.GetBytes(Environment.NewLine);
                    memoryStream.Write(buffer, 0, buffer.Length);
                }

                byte[] boundaryBuffer = Encoding.ASCII.GetBytes(boundary + "--");
                memoryStream.Write(boundaryBuffer, 0, boundaryBuffer.Length);

                request.ContentLength = memoryStream.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(memoryStream.ToArray(), 0, (int)memoryStream.Length);
                }
            }

            using (WebResponse response = request.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            using (var stream = new MemoryStream())
            {
                if (responseStream != null)
                {
                    responseStream.CopyTo(stream);
                }

                return stream.ToArray();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get web request.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// The <see cref="WebRequest"/>.
        /// </returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest r = base.GetWebRequest(address);
            var request = r as HttpWebRequest;
            if (request != null)
            {
                request.CookieContainer = this.CookieContainer;
            }

            return r;
        }

        /// <summary>
        /// The get web response.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="WebResponse"/>.
        /// </returns>
        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            WebResponse response = base.GetWebResponse(request, result);
            this.ReadCookies(response);
            return response;
        }

        /// <summary>
        /// The get web response.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="WebResponse"/>.
        /// </returns>
        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            this.ReadCookies(response);
            return response;
        }

        /// <summary>
        /// The read cookies.
        /// </summary>
        /// <param name="r">
        /// The r.
        /// </param>
        private void ReadCookies(WebResponse r)
        {
            var response = r as HttpWebResponse;
            if (response == null)
            {
                return;
            }
            CookieCollection cookies = response.Cookies;
            this.CookieContainer.Add(cookies);
        }

        #endregion
    }
}