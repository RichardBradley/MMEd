using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MMEd.Util
{
  ///==========================================================================
  /// Class : CDImage
  /// 
  /// <summary>
  /// 	Class representing a CD image (.bin file) in Mode2/2352 format.
  /// </summary>
  ///==========================================================================
  public class CDImage
  {
    #region Loading and saving CD images

    ///========================================================================
    /// Constructor : CDImage
    /// 
    /// <summary>
    /// 	Construct a new CD image based on a disk file
    /// </summary>
    /// <param name="xiImageFile"></param>
    ///========================================================================
    public CDImage(FileInfo xiImageFile)
    {
      mImageFile = xiImageFile;
    }
    
    ///========================================================================
    /// Method : EnsureBytes
    /// 
    /// <summary>
    /// 	Ensure that the given range of bytes of the CD image are in memory
    /// </summary>
    /// <param name="xiStart"></param>
    /// <param name="xiLength"></param>
    ///========================================================================
    private void EnsureBytes(long xiStart, long xiLength)
    {
      //=======================================================================
      // Calculate the union of the bit of the CD image we've already loaded
      // and the bit we now want
      //=======================================================================
      long lNewStart = mLoadedImageStart == -1 ? xiStart : Math.Min(xiStart, mLoadedImageStart);
      long lNewEnd = mLoadedImageStart == -1 ? xiStart + xiLength : Math.Max(xiStart + xiLength, mLoadedImageStart + mLoadedImage.Length);
      long lNewLength = lNewEnd - lNewStart;

      if (lNewStart == mLoadedImageStart && lNewLength == mLoadedImage.Length)
      {
        //=====================================================================
        // We already have the correct section of the CD image
        //=====================================================================
        return;
      }

      //=======================================================================
      // Reload the whole of the required section of the image, and then
      // overwrite with anything we've already got in memory (not optimally
      // efficient in theory, but perfectly good in most cases)
      //=======================================================================
      byte[] lLoadedBytes = new byte[lNewLength];
      using (FileStream lFileStream = mImageFile.OpenRead())
      {
        lFileStream.Seek(lNewStart, SeekOrigin.Begin);
        lFileStream.Read(lLoadedBytes, 0, lLoadedBytes.Length);
      }

      if (mLoadedImageStart > -1)
      {
        Array.Copy(mLoadedImage, 0, lLoadedBytes, lNewStart - mLoadedImageStart, mLoadedImage.Length);
      }

      mLoadedImage = lLoadedBytes;
      mLoadedImageStart = lNewStart;
    }

    ///========================================================================
    /// Method : WriteFile
    /// 
    /// <summary>
    /// 	Save a CD image to a file
    /// </summary>
    /// <param name="xiImageFile"></param>
    /// <remarks>
    ///   Only writes the in-memory portion of the image, for efficiency.
    ///   After writing, resets the in-memory buffer so you can change other
    ///   bits of the file. (Writing often tends to be more efficient, if 
    ///   you're making lots of small changes - otherwise too much of the CD
    ///   image gets loaded into memory, which is slow).
    /// </remarks>
    ///========================================================================
    public void WriteFile(FileInfo xiImageFile)
    {
      using (FileStream lNewImage = xiImageFile.Open(FileMode.Open, FileAccess.Write, FileShare.Read))
      {
        lNewImage.Seek(mLoadedImageStart, SeekOrigin.Begin);
        lNewImage.Write(mLoadedImage, 0, mLoadedImage.Length);
      }

      mLoadedImage = new byte[0];
      mLoadedImageStart = -1;
    }

    #endregion

    #region Finding and replacing tracks

    ///========================================================================
    /// Method : Find
    /// 
    /// <summary>
    /// 	Returns the first position after xiOffset that the byte array 
    ///   xiSearch is found within the CD image.
    /// </summary>
    /// <param name="xiSearch"></param>
    /// <param name="xiOffset"></param>
    /// <returns>Offset within xiSource, or -1 for no match</returns>
    ///========================================================================
    public long Find(byte[] xiSearch, long xiOffset)
    {
      EnsureBytes(xiOffset, mImageFile.Length - xiOffset);

      for (long lCDOffset = GetNextOffset(xiOffset - 1); 
        lCDOffset < mLoadedImage.Length - xiSearch.Length; 
        lCDOffset = GetNextOffset(lCDOffset))
      {
        bool lMatch = true;
        long lTempCDOffset = lCDOffset;

        for (long lSearchOffset = 0; 
          lSearchOffset < xiSearch.Length; 
          lSearchOffset++, lTempCDOffset = GetNextOffset(lTempCDOffset))
        {
          if (mLoadedImage[lTempCDOffset - mLoadedImageStart] != xiSearch[lSearchOffset])
          {
            lMatch = false;
            break;
          }
        }

        if (lMatch)
        {
          return lCDOffset;
        }
      }

      return -1;
    }

    ///========================================================================
    /// Method : Replace
    /// 
    /// <summary>
    /// 	Replaces the data in the CD image from the specified location with 
    ///   a new byte array, updating only the Data sections.
    /// 
    ///   Note that this method does not correctly update the ECC data, and
    ///   so the resultant CD image is technically corrupt.
    /// </summary>
    /// <param name="xiReplacement"></param>
    /// <param name="xiOffset"></param>
    ///========================================================================
    public void Replace(byte[] xiReplacement, long xiOffset)
    {
      //=======================================================================
      // Ensure we have enough of the file in memory.
      // Use a rough metric to calculate how much data we need to read in -
      // this will overestimate, but only by a respectable amount.
      //=======================================================================
      EnsureBytes(xiOffset, (long)(xiReplacement.Length * ((double)TRACK_LENGTH / (double)DATA_LENGTH)) + TRACK_LENGTH);

      //=======================================================================
      // Replace the requested section of the file.
      //=======================================================================
      long lCDOffset = GetNextOffset(xiOffset - 1);

      for (long lReplacementOffset = 0; lReplacementOffset < xiReplacement.Length; lReplacementOffset++)
      {
        mLoadedImage[lCDOffset - mLoadedImageStart] = xiReplacement[lReplacementOffset];

        lCDOffset = GetNextOffset(lCDOffset);
      }
    }

    ///========================================================================
    ///  Method : Extract
    /// 
    /// <summary>
    /// 	Extract a given number of bytes from the CD image.
    /// </summary>
    /// <param name="xiOffset"></param>
    /// <param name="xiLength"></param>
    /// <returns></returns>
    ///========================================================================
    public byte[] Extract(long xiOffset, long xiLength)
    {
      //=======================================================================
      // Ensure we have enough of the file in memory.
      // Use a rough metric to calculate how much data we need to read in -
      // this will overestimate, but only by a respectable amount.
      //=======================================================================
      EnsureBytes(xiOffset, (long)(xiLength * ((double)TRACK_LENGTH / (double)DATA_LENGTH)) + TRACK_LENGTH);

      //=======================================================================
      // Extract the requested section of the file.
      //=======================================================================
      long lCDOffset = GetNextOffset(xiOffset - 1);
      byte[] lRet = new byte[xiLength];

      for (long lExtractOffset = 0; lExtractOffset < xiLength; lExtractOffset++)
      {
        lRet[lExtractOffset] = mLoadedImage[lCDOffset - mLoadedImageStart];

        lCDOffset = GetNextOffset(lCDOffset);
      }

      return lRet;
    }

    ///========================================================================
    /// Method : GetNextOffset
    /// 
    /// <summary>
    /// 	Return the next offset in the data of the CD image after the given
    ///   offset
    /// </summary>
    /// <param name="xiOffset"></param>
    /// <returns></returns>
    /// <remarks>
    ///   Mode2/2352 has the following format:
    /// 
    ///   Sync (12), Address (3), Mode (1), Subheader (8), Data (2048), ECC (280)
    ///   
    ///   This method returns xiOffset+1 if you're in a Data section, or 
    ///   otherwise jumps to the next Data section.
    /// </remarks>
    ///========================================================================
    private long GetNextOffset(long xiOffset)
    {
      long lNewOffset = xiOffset + 1;
      long lTrackOffset = lNewOffset % TRACK_LENGTH;

      if (lTrackOffset >= HEADER_LENGTH && lTrackOffset < (HEADER_LENGTH + DATA_LENGTH))
      {
        //=====================================================================
        // We're still in the Data section
        //=====================================================================
        return lNewOffset;
      }
      else
      {
        //=====================================================================
        // Jump to the next track's Data section
        //=====================================================================
        return lNewOffset + (TRACK_LENGTH - lTrackOffset) + HEADER_LENGTH;
      }
    }

    #endregion

    private FileInfo mImageFile;
    private long mLoadedImageStart = -1;
    private byte[] mLoadedImage = new byte[0];

    private const long HEADER_LENGTH = 12 + 3 + 1 + 8; // Sync + Address + Mode + Subheader
    private const long DATA_LENGTH = 2048;
    private const long ECC_LENGTH = 280;
    private const long TRACK_LENGTH = HEADER_LENGTH + DATA_LENGTH + ECC_LENGTH;

  }
}
