using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Drawing;

// Holds local settings like the last file opened

namespace MMEd
{
  public class LocalSettings
  {
    #region MMEd local settings

    public string LastOpenedFile;
    public MainForm.eOpenType LastOpenedType = MainForm.eOpenType.LevelBinary;
    public MainForm.eSaveMode LastSavedMode = MainForm.eSaveMode.Mmv;
    public string LastSavedFile;
    public string LastTIMFile;
    public string LastTMDFile;
    public string LastSavedSceneFile;
    public string LastFlatImagesDir;
    public string LastPublishedBinaryFilePath;
    public string LastPublishedCDImage;
    public string LastPublishedCourse;
    public string LastPublishedCourseName;
    public bool LastPublishUpdatedCDImage = false;
    public bool LastPublishedKeepBackups = true;
    public int LastPublishedBackupCount = 5;
    public string LastCleanCDImage;
    public Size m_size;

    #endregion

    #region Tiler local settings

    public string TilerLastOpenedImage;
    public string TilerLastSaveTilesDir;

    #endregion

    #region persistence

    private static string GetExpectedFilename()
    {
      return Path.Combine(
             Path.Combine(
             System.Environment.GetEnvironmentVariable("APPDATA"),
             "MMEd"), "LocalSettings.xml");
    }

    // loads the local settings found on this machine,
    // or creates a new instance
    public static LocalSettings GetInstance()
    {
      if (File.Exists(GetExpectedFilename()))
      {
        XmlSerializer xs = new XmlSerializer(typeof(LocalSettings));
        try
        {
          using (FileStream fs = File.OpenRead(GetExpectedFilename()))
          {
            return (LocalSettings)xs.Deserialize(fs);
          }
        }
        catch { }
      }

      return new LocalSettings();
    }

    // attempts to save the local settings to their default location
    // may throw exceptions
    public void Save()
    {
      Directory.CreateDirectory(Path.GetDirectoryName(GetExpectedFilename()));
      using (FileStream fs = File.Create(GetExpectedFilename()))
      {
        XmlSerializer xs = new XmlSerializer(typeof(LocalSettings));
        xs.Serialize(fs, this);
      }
    }

    #endregion
  }
}
