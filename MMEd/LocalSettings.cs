using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

// Holds local settings like the last file opened

namespace MMEd
{
  public class LocalSettings
  {
    private static string GetExpectedFilename()
    {
      return Path.Combine(
             Path.Combine(
             System.Environment.GetEnvironmentVariable("APPDATA"),
             "MMEd"), "LocalSettings.xml");
    }

    public string LastOpenedFile;
    public string LastSavedFile;
    public string LastTIMFile;

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
  }
}
