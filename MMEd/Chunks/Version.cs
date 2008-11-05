using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Microsoft.XmlDiffPatch;
using System.Xml;
using MMEd.Util;
using System.Windows.Forms;

namespace MMEd.Chunks
{
  ///==========================================================================
  /// Class : Version
  /// 
  /// <summary>
  /// 	A version of a level, consisting of:
  ///     - Serialised form of the level, potentially as a diff from a 
  ///       previous level
  ///     - The date/time this version was created
  ///     - qq:MWR Description of change?
  /// </summary>
  ///==========================================================================
  public class Version : Chunk
  {
    internal byte[] SerialisedLevel = null;
    internal DiffBlock[] DifferentialLevel = null;
    internal DateTime CreationDate;
    internal string Changes;

    ///========================================================================
    /// Constructor : Version
    /// 
    /// <summary>
    /// 	Create a new version
    /// </summary>
    /// <param name="xiLevel"></param>
    /// <param name="xiPreviousVersion"></param>
    /// <param name="xiStoreAsDiff"></param>
    /// <param name="xiExpectedSize"></param>
    ///========================================================================
    internal Version(Level xiLevel, Version xiPreviousVersion, bool xiStoreAsDiff, long xiExpectedSize)
    {
      CreationDate = DateTime.Now;

      //=======================================================================
      // Serialise the level
      //=======================================================================
      while (true)
      {
        using (MemoryStream lLevelStream = new MemoryStream())
        {
          xiLevel.Serialise(lLevelStream);
          lLevelStream.Seek(0, SeekOrigin.Begin);
          SerialisedLevel = new byte[lLevelStream.Length];
          lLevelStream.Read(SerialisedLevel, 0, SerialisedLevel.Length);
        }

        if (xiExpectedSize > 0 && SerialisedLevel.Length != xiExpectedSize)
        {
          long lSizeAdjustment = SerialisedLevel.Length - xiExpectedSize;

          if (lSizeAdjustment > 0 && xiLevel.SHET.TrailingZeroByteCount < lSizeAdjustment)
          {
            MessageBox.Show(string.Format(@"WARNING: The level is too large to fit in the expected size of file. Please remove some content from the level. Your level can be saved, but attempts to play the course will fail.

Expected size: {0} bytes
Actual size: {1} bytes
Spare space: {2} bytes
Space required: {3} bytes",
                         xiExpectedSize,
                         SerialisedLevel.Length,
                         xiLevel.SHET.TrailingZeroByteCount,
                         lSizeAdjustment - xiLevel.SHET.TrailingZeroByteCount), "MMEd", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          }
          else if (MessageBox.Show("WARNING: The level is not of the expected size for this course. Do you want to adjust the file size to match? This can be done without corrupting the level.", "MMEd", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
          {
            xiLevel.SHET.TrailingZeroByteCount -= (int)lSizeAdjustment;
            continue;
          }
        }

        break;
      }

      //=======================================================================
      // Calculate differences, and store as a diff if appropriate
      //=======================================================================
      if (xiPreviousVersion != null)
      {
        Changes = string.Join("\r\n", xiLevel.GetDifferences(xiPreviousVersion.GetLevel(null)).ToArray());
      }
      else
      {
        Changes = "";
      }

      if (xiStoreAsDiff)
      {
        DifferentialLevel = DiffBlock.GetDifferences(xiPreviousVersion.SerialisedLevel, SerialisedLevel);
      }
    }

    ///========================================================================
    /// Constructor : Version
    /// 
    /// <summary>
    /// 	Read a Version from a stream
    /// </summary>
    /// <param name="xiInputStream"></param>
    ///========================================================================
    internal Version(Stream xiInputStream)
    {
      Deserialise(xiInputStream);
    }

    ///========================================================================
    /// Method : GetLevel
    /// 
    /// <summary>
    /// 	Get the level represented by this Version
    /// </summary>
    /// <param name="xiPreviousVersion">
    ///   The previous version, which must already have had GetLevel called.
    /// </param>
    /// <returns></returns>
    ///========================================================================
    internal Level GetLevel(Version xiPreviousVersion)
    {
      //=======================================================================
      // Apply the diffs if necessary
      //=======================================================================
      if (SerialisedLevel == null)
      {
        if (xiPreviousVersion == null)
        {
          throw new Exception("Cannot access a differentially-serialised level if the previous version is not available");
        }
        else if (xiPreviousVersion.SerialisedLevel == null)
        {
          throw new Exception("Cannot access a differentially-serialised level if the previous version has not already been accessed");
        }

        SerialisedLevel = DiffBlock.ApplyPatch(xiPreviousVersion.SerialisedLevel, DifferentialLevel);
      }

      //=======================================================================
      // Deserialise the level
      //=======================================================================
      using (MemoryStream lLevelStream = new MemoryStream())
      {
        lLevelStream.Write(SerialisedLevel, 0, SerialisedLevel.Length);
        lLevelStream.Seek(0, SeekOrigin.Begin);
        return new Level(lLevelStream);
      }
    }

    ///========================================================================
    /// Method : Serialise
    /// 
    /// <summary>
    /// 	Serialise this Version to binary format
    /// </summary>
    /// <param name="xiOutStream"></param>
    ///========================================================================
    public override void Serialise(Stream xiOutStream)
    {
      BinaryWriter lOutput = new BinaryWriter(xiOutStream);

      //=======================================================================
      // Magic number to allow us to change the file format later
      //=======================================================================
      lOutput.Write(MAGIC_NUMBER);

      //=======================================================================
      // Write the differences if we have them, or otherwise write the whole
      // level
      //=======================================================================
      if (DifferentialLevel != null)
      {
        lOutput.Write(true);
        lOutput.Write(DifferentialLevel.Length);

        foreach (DiffBlock lDiff in DifferentialLevel)
        {
          lDiff.Serialise(lOutput);
        }
      }
      else
      {
        lOutput.Write(false);
        lOutput.Write(SerialisedLevel.Length);
        lOutput.Write(SerialisedLevel);
      }

      //=======================================================================
      // Write the creation date and change list
      //=======================================================================
      lOutput.Write(CreationDate.ToBinary());
      StreamUtils.WriteASCIINullTermString(xiOutStream, Changes);
    }

    private const int MAGIC_NUMBER = 42;

    ///========================================================================
    /// Method : Deserialise
    /// 
    /// <summary>
    /// 	Diesierliase this Version from binary format.
    /// </summary>
    /// <param name="xiInStream"></param>
    ///========================================================================
    public override void Deserialise(Stream xiInStream)
    {
      BinaryReader lInput = new BinaryReader(xiInStream);

      //=======================================================================
      // Check the magic number
      //=======================================================================
      int lMagicNumber = lInput.ReadInt32();

      if (lMagicNumber != MAGIC_NUMBER)
      {
        throw new DeserialisationException(string.Format(
          "Version has the wrong magic number! Expected {0}, found {1}", MAGIC_NUMBER, lMagicNumber));
      }

      //=======================================================================
      // Work out whether we've got differences or a whole level, and
      // read in accordingly.
      //=======================================================================
      bool lDifferential = lInput.ReadBoolean();
      int lLength = lInput.ReadInt32();

      if (lDifferential)
      {
        DifferentialLevel = new DiffBlock[lLength];

        for (int ii = 0; ii < DifferentialLevel.Length; ii++)
        {
          DifferentialLevel[ii] = new DiffBlock(lInput);
        }
      }
      else
      {
        SerialisedLevel = lInput.ReadBytes(lLength);
      }

      //=======================================================================
      // Read the creation date and change list
      //=======================================================================
      CreationDate = DateTime.FromBinary(lInput.ReadInt64());
      Changes = StreamUtils.ReadASCIINullTermString(xiInStream);
    }

    public override string Name
    {
      get { return "Version " + CreationDate.ToString("yyyy/MM/dd HH:mm:ss"); }
    }
    
    ///========================================================================
    /// Class : DiffBlock
    /// 
    /// <summary>
    /// 	A class representing a section of data that's different between 
    ///   two byte arrays - just the offset in the second array and the 
    ///   chunk of data that's different. E.g.:
    /// 
    ///   Array A:  1234123412341234
    ///   Array B:  123412ABCD34123412
    /// 
    ///   The two DiffBlocks representing the differences between A and B would 
    ///   contain:
    ///     Offset = 6
    ///     Data   = ABCD
    /// 
    ///     Offset = 16
    ///     Data   = 12
    /// 
    ///   If B were shorter than A, there would be a DiffBlock of length 0 
    ///   with Offset=B.Length.
    /// </summary>
    ///========================================================================
    internal class DiffBlock
    {
      private int mOffset;
      private byte[] mData;

      private DiffBlock(int xiOffset, byte[] xiData)
      {
        mOffset = xiOffset;
        mData = xiData;
      }

      ///======================================================================
      /// Constructor : DiffBlock
      /// 
      /// <summary>
      /// 	Create a new DiffBlock from data output by Serialise.
      /// </summary>
      /// <param name="xiInput"></param>
      ///======================================================================
      public DiffBlock(BinaryReader xiInput)
      {
        mOffset = xiInput.ReadInt32();
        int lLength = xiInput.ReadInt32();
        mData = xiInput.ReadBytes(lLength);
      }

      ///======================================================================
      /// Method : Serialise
      /// 
      /// <summary>
      /// 	Write out this DiffBlock in a form that can later be reconstructed.
      /// </summary>
      /// <param name="xiOutput"></param>
      ///======================================================================
      public void Serialise(BinaryWriter xiOutput)
      {
        xiOutput.Write(mOffset);
        xiOutput.Write(mData.Length);
        xiOutput.Write(mData);
      }

      ///======================================================================
      /// Static Method : GetDifferences
      /// 
      /// <summary>
      /// 	Return an array of DiffBlocks representing the differences between
      ///   two byte arrays.
      /// </summary>
      /// <param name="xiA"></param>
      /// <param name="xiB"></param>
      /// <returns></returns>
      ///======================================================================
      public static DiffBlock[] GetDifferences(byte[] xiA, byte[] xiB)
      {
        List<DiffBlock> lDifferences = new List<DiffBlock>();
        int lDiffStart = -1;

        for (int ii = 0; ii < xiA.Length && ii < xiB.Length; ii++)
        {
          if (xiA[ii] == xiB[ii])
          {
            //=================================================================
            // No difference here. If we're in the middle of a difference
            // already, create a DiffBlock for the chunk we've just finished
            // looking at.
            //=================================================================
            if (lDiffStart > -1)
            {
              byte[] lDiff = new byte[ii - lDiffStart];
              Array.Copy(xiB, lDiffStart, lDiff, 0, lDiff.Length);
              lDifferences.Add(new DiffBlock(lDiffStart, lDiff));
              lDiffStart = -1;
            }
          }
          else
          {
            //=================================================================
            // There's a difference. If we don't already know we're in the 
            // middle of a difference, start recording it.
            //=================================================================
            if (lDiffStart == -1)
            {
              lDiffStart = ii;
            }
          }
        }

        //=====================================================================
        // Work out whether there's any tidying up to be done.
        //=====================================================================
        if (xiA.Length != xiB.Length || lDiffStart > -1)
        {
          if (lDiffStart == -1)
          {
            //=================================================================
            // There's some trailing data in array B, or array B is shorter than
            // array A.
            //=================================================================
            lDiffStart = Math.Min(xiA.Length, xiB.Length);
          }

          byte[] lDiff = new byte[xiB.Length - lDiffStart];
          Array.Copy(xiB, lDiffStart, lDiff, 0, lDiff.Length);
          lDifferences.Add(new DiffBlock(lDiffStart, lDiff));
        }

        return lDifferences.ToArray();
      }

      ///======================================================================
      /// Static Method : ApplyPatch
      /// 
      /// <summary>
      /// 	Apply the supplied DiffBlocks to the source array to undo the 
      ///   operation of GetDifferences().
      /// </summary>
      /// <param name="xiA"></param>
      /// <param name="xiDiffs"></param>
      /// <returns></returns>
      ///======================================================================
      public static byte[] ApplyPatch(byte[] xiA, DiffBlock[] xiDiffs)
      {
        //=====================================================================
        // Calculate the length of the final byte array
        //=====================================================================
        int lNewLength = xiA.Length;

        if (xiDiffs.Length > 0)
        {
          DiffBlock lFinalDiff = xiDiffs[xiDiffs.Length - 1];

          if (lFinalDiff.mData.Length == 0)
          {
            lNewLength = lFinalDiff.mOffset;
          }
          else if (lFinalDiff.mOffset + lFinalDiff.mData.Length > xiA.Length)
          {
            lNewLength = lFinalDiff.mOffset + lFinalDiff.mData.Length;
          }
        }

        byte[] lRet = new byte[lNewLength];

        //=====================================================================
        // Fill in the data
        //=====================================================================
        int lOffset = 0;

        foreach (DiffBlock lDiff in xiDiffs)
        {
          int lCopyLength = lDiff.mOffset - lOffset;
          Array.Copy(xiA, lOffset, lRet, lOffset, lCopyLength);
          lOffset += lCopyLength;

          Array.Copy(lDiff.mData, 0, lRet, lOffset, lDiff.mData.Length);
          lOffset += lDiff.mData.Length;
        }

        if (lRet.Length != lOffset)
        {
          Array.Copy(xiA, lOffset, lRet, lOffset, lRet.Length - lOffset);
        }

        return lRet;
      }
    }

  }
}
