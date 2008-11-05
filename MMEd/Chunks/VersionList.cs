using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using MMEd.Util;

namespace MMEd.Chunks
{
  ///==========================================================================
  /// Class : VersionList
  /// 
  /// <summary>
  /// 	A list containing multiple versions of a level, as edited.
  /// 
  ///   Also includes:
  ///   - The name of the binary file to export to on save
  ///   - The filename on CD
  ///   - The name of this course
  /// </summary>
  ///==========================================================================
  public class VersionList : Chunk
  {
    public List<Version> Versions = new List<Version>();

    public Level CurrentLevel = null;
    public string BinaryFilename = null;
    public string CDFilename = null;
    public string CourseName = null;

    public VersionList(Level xiInitialLevel, string xiCourseName, string xiCDFilename)
    {
      CurrentLevel = xiInitialLevel;
      CourseName = xiCourseName;
      CDFilename = xiCDFilename;
      AddLevel(CurrentLevel);
    }

    public VersionList(Stream xiStream)
    {
      Deserialise(xiStream);
    }

    ///========================================================================
    /// Method : AddLevel
    /// 
    /// <summary>
    /// 	Add a new level to the version list
    /// </summary>
    /// <param name="xiNewLevel"></param>
    ///========================================================================
    public void AddLevel(Level xiNewLevel)
    {
      Version lDiffFrom = GetLastVersion();
      bool lStoreAsDiff = (Versions.Count % 5 != 0);
      long lExpectedSize = CDFilename == null ? -1L : MMCD.Courses.Find(new Predicate<MMCD.Course>(
        delegate(MMCD.Course xiCourse) { return xiCourse.FileName == CDFilename; })).CDLength;

      Version lNewVersion = new Version(xiNewLevel, lDiffFrom, lStoreAsDiff, lExpectedSize);
      Versions.Add(lNewVersion);
      CurrentLevel = xiNewLevel;
    }

    ///========================================================================
    /// Method : GetLastVersion
    /// 
    /// <summary>
    /// 	Return the most recent level
    /// </summary>
    /// <returns></returns>
    ///========================================================================
    public Version GetLastVersion()
    {
      if (Versions.Count == 0)
      {
        return null;
      }

      return GetVersion(Versions.Count - 1);
    }

    ///========================================================================
    /// Method : GetLevel
    /// 
    /// <summary>
    /// 	Get the level from a particular Version
    /// </summary>
    /// <param name="xiVersion"></param>
    /// <returns></returns>
    ///========================================================================
    public Level GetLevel(Version xiVersion)
    {
      if (xiVersion.SerialisedLevel != null)
      {
        return xiVersion.GetLevel(null);
      }

      int lIndex = Versions.IndexOf(xiVersion);

      if (lIndex == -1)
      {
        throw new Exception("Cannot retrieve Level from Version - Version does not appear to be in the VersionList");
      }

      return GetVersion(lIndex).GetLevel(null);
    }

    ///========================================================================
    /// Method : GetVersion
    /// 
    /// <summary>
    /// 	Get a particular level in the list
    /// </summary>
    /// <param name="xiIndex"></param>
    /// <returns></returns>
    /// <remarks>
    ///   The fact that we have to call GetLevel like this is a bit confused -
    ///   should tidy this up really.
    /// </remarks>
    ///========================================================================
    private Version GetVersion(int xiIndex)
    {
      Version lVersion = Versions[xiIndex];

      if (lVersion.SerialisedLevel == null)
      {
        if (xiIndex == 0)
        {
          throw new InvalidOperationException("Error: First level in the version list was differentially serialised, but there's no previous version to compare the diff against");
        }

        lVersion.GetLevel(GetVersion(xiIndex - 1));
      }

      return lVersion;
    }

    ///========================================================================
    /// Method : Serialise
    /// 
    /// <summary>
    /// 	Serialise this VersionList to binary format 
    /// </summary>
    /// <param name="outStr"></param>
    ///========================================================================
    public override void Serialise(Stream xiOutStream)
    {
      BinaryWriter lOutput = new BinaryWriter(xiOutStream);

      //=======================================================================
      // Magic number to allow us to change the file format later
      //=======================================================================
      lOutput.Write(MAGIC_NUMBER);

      //=======================================================================
      // Write the versions
      //=======================================================================
      lOutput.Write(Versions.Count);

      foreach (Version lVersion in Versions)
      {
        lVersion.Serialise(xiOutStream);
      }

      //=======================================================================
      // Write the strings
      //=======================================================================
      StreamUtils.WriteASCIINullTermString(xiOutStream, BinaryFilename);
      StreamUtils.WriteASCIINullTermString(xiOutStream, CDFilename);
      StreamUtils.WriteASCIINullTermString(xiOutStream, CourseName);
    }

    private const int MAGIC_NUMBER = 41;

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
          "VersionList has the wrong magic number! Expected {0}, found {1}", MAGIC_NUMBER, lMagicNumber));
      }

      //=======================================================================
      // Read the versions
      //=======================================================================
      int lVersionCount = lInput.ReadInt32();
      Versions = new List<Version>(lVersionCount);

      for (int ii = 0; ii < lVersionCount; ii++)
      {
        Versions.Add(new Version(xiInStream));
      }

      //=======================================================================
      // Read the strings
      //=======================================================================
      BinaryFilename = StreamUtils.ReadASCIINullTermString(xiInStream);
      CDFilename = StreamUtils.ReadASCIINullTermString(xiInStream);
      CourseName = StreamUtils.ReadASCIINullTermString(xiInStream);

      //=======================================================================
      // Set the CurrentLevel
      //=======================================================================
      CurrentLevel = GetLastVersion().GetLevel(null);
    }

    public override string Name
    {
      get { return CourseName; }
    }

    ///========================================================================
    /// Method : GetChildren
    /// 
    /// <summary>
    /// 	Return first the current level, and then any earlier versions.
    /// </summary>
    /// <returns></returns>
    ///========================================================================
    public override Chunk[] GetChildren()
    {
      Chunk[] lRet = new Chunk[Versions.Count + 1];

      if (Versions.Count > 0)
      {
        Array.Copy(Versions.ToArray(), lRet, Versions.Count);
      }
      lRet[lRet.Length - 1] = CurrentLevel;

      Array.Reverse(lRet);

      return lRet;
    }

  }
}
