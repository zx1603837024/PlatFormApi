using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace F2Api.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class MultipartFileDataStream : MultipartFileData, IDisposable
    {
        /// <summary>
        /// file content stream
        /// </summary>
        public Stream Stream { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="localFileName"></param>
        /// <param name="stream"></param>
        public MultipartFileDataStream(HttpContentHeaders headers, string localFileName, Stream stream)
                : base(headers, localFileName)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            Stream = stream;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Stream.Dispose();
        }
    }
}