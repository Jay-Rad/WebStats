using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebStats.Implementations
{
    /// <summary>
    /// An HttpResponse filter stream for measuring the bytes written to the response body.
    /// </summary>
    public class MeasurementFilter : Stream
    {
        public MeasurementFilter(Stream responseStream)
        {
            ResponseStream = responseStream;
        }

        public long BytesWritten { get; set; }

        public override bool CanRead => ResponseStream.CanRead;

        public override bool CanSeek => ResponseStream.CanSeek;

        public override bool CanWrite => ResponseStream.CanWrite;

        public override long Length => ResponseStream.Length;

        public override long Position
        {
            get
            {
                return ResponseStream.Position;
            }
            set
            {
                ResponseStream.Position = value;
            }
        }

        public override void Flush()
        {
            ResponseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return ResponseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return ResponseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            ResponseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            BytesWritten += buffer.Length;
            ResponseStream.Write(buffer, offset, count);
        }

        private Stream ResponseStream { get; set; }

    }
}
