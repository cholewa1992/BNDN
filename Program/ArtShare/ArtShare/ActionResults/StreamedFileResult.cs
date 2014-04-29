using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace ArtShare.ActionResults
{
    /// <summary>
    /// Adapted from http://forums.asp.net/t/1408527.aspx?Why+does+FileStreamResult+buffer+the+whole+file+before+sending+it+
    /// </summary>
    public class StreamedFileResult : FileResult
    {
        // default buffer size as defined in BufferedStream type
        private const int _bufferSize = 0x1000;
        private readonly string FileName;

        public StreamedFileResult(Stream fileStream, string contentType, string fileName)
            : base(contentType)
        {
            if (fileStream == null)
            {
                throw new ArgumentNullException("fileStream");
            }

            FileStream = fileStream;
            if(fileName == null)
                fileName = "file";
            FileName = fileName;
        }

        public Stream FileStream
        {
            get;
            private set;
        }

        public long? FileSize
        {
            get;
            set;
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            response.BufferOutput = false;
            if (FileSize.HasValue)
                response.AddHeader("Content-Length", FileSize.ToString());
            if(FileName != null)
                response.AppendHeader("content-disposition", "attachment; filename='" + FileName + "'");

            // grab chunks of data and write to the output stream
            Stream outputStream = response.OutputStream;
            using (FileStream)
            {
                byte[] buffer = new byte[_bufferSize];
                int bytesRead = FileStream.Read(buffer, 0, _bufferSize);
                while (bytesRead > 0)
                {
                    outputStream.Write(buffer, 0, _bufferSize);
                    bytesRead = FileStream.Read(buffer, 0, _bufferSize);
                }
            }
        }
    }
}