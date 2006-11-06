using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;

// This stream will throw an assertion exception if you what you write to
// it doesn't match the provided reference stream

namespace MMEd.Tests
{
  class DebugOutputStreamWithExpectations : Stream
  {
    Stream mExpectedStream, mBaseStream;

    /// <summary>
    ///   Creates a new DebugOutputStreamWithExpectations
    /// </summary>
    /// <param name="xiExpectedStream">
    ///   The bytes written to the stream must match the bytes found
    /// in this stream, otherwise an assertion exception will be thrown
    /// </param>
    /// <param name="xiBaseStream">
    ///   The written bytes will be sent to this stream, or discarded
    /// if this parameter is null
    /// </param>
    public DebugOutputStreamWithExpectations(Stream xiExpectedStream, Stream xiBaseStream)
    {
      this.mExpectedStream = xiExpectedStream;
      this.mBaseStream = xiBaseStream;
    }

    /// <summary>
    ///   Creates a new DebugOutputStreamWithExpectations
    /// </summary>
    /// <param name="xiExpectedStream">
    ///   The bytes written to the stream must match the bytes found
    /// in this stream, otherwise an assertion exception will be thrown
    /// </param>
    public DebugOutputStreamWithExpectations(Stream xiExpectedStream) : this(xiExpectedStream, null) { }

    public override void Write(byte[] buffer, int offset, int count)
    {
      byte[] expectedBuffer = new byte[count];
      mExpectedStream.Read(expectedBuffer, 0, count);
      byte[] foundBuffer = buffer;
      if (offset != 0 || count != buffer.Length)
      {
        foundBuffer = new byte[count];
        Array.Copy(buffer, offset, foundBuffer, 0, count);
      }
      Assert.AreEqual(expectedBuffer, foundBuffer);
      if (mBaseStream != null)
      {
        mBaseStream.Write(buffer, offset, count);
      }

    }

    public override bool CanRead
    {
      get { return false; }
    }

    public override bool CanWrite
    {
      get { return true; }

    }

    public override bool CanSeek
    {
      get { return false; }
    }

    #region overrides of abstract methods which are unsupported

    public override long Length
    {
      get { throw new Exception("The method or operation is not implemented."); }
    }

    public override long Position
    {
      get
      {
        throw new Exception("The method or operation is not implemented.");
      }
      set
      {
        throw new Exception("The method or operation is not implemented.");
      }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public override void SetLength(long value)
    {
      throw new Exception("The method or operation is not implemented.");
    }
    public override void Flush()
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    #endregion
  }
}
