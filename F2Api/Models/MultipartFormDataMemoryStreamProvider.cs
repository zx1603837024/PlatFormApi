using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace F2Api.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class MultipartFormDataMemoryStreamProvider : MultipartFormDataStreamProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public MultipartFormDataMemoryStreamProvider() : base("/")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }
            MemoryStream stream = new MemoryStream();
            if (IsFileContent(parent, headers))
            {
                MultipartFileData item = new MultipartFileDataStream(headers, string.Empty, stream);
                this.FileData.Add(item);
            }
            return stream;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        private bool IsFileContent(HttpContent parent, HttpContentHeaders headers)
        {
            ContentDispositionHeaderValue contentDisposition = headers.ContentDisposition;
            if (contentDisposition == null)
            {
                throw new InvalidOperationException("Content-Disposition error");
            }
            return !string.IsNullOrEmpty(contentDisposition.FileName);
        }
    }
}