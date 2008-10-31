using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using MMEd;
using MMEd.Util;
using MMEd.Chunks;

// a command line tool for MMEd
//
// performs various tasks which are unsuited to a GUI

namespace MMEdTool
{
  public class MMEdTool
  {
    static int Main(string[] args)
    {
      const string USAGE = "usage: mmedtool <command>\n\ntry mmedtool help for more info";
      const string GET_ALL_XML_USAGE = "usage: mmedtool getallxml <path/to/MICRO> <outfile.xml>\n\nparses all the level files found in the given tree, and writes them to the given XML file";
      const string UNUSED_VRAM_USAGE = "usage: mmedtool vram <path/to/MICRO>\n\nwrites a few png files representing the VRAM";
      const string FIND_CD_OFFSETS_USAGE = "usage: mmedtool findcdoffsets <path/to/cdimage.bin> <path/to/MICRO>\n\nsearches the CD image for all the level files found in the given tree, and outputs their offsets within the bin file";
      const string FIND_COURSENAME_OFFSETS_USAGE = "usage: mmedtool findcoursenameoffsets <path/to/cdimage.bin>";

      if (args.Length == 0)
      {
        Console.Error.WriteLine(USAGE);
        return 1;
      }
      switch (args[0].ToLower())
      {
        case "vram":
          {
            DirectoryInfo lRootDir;
            try
            {
              if (args.Length != 2) throw new Exception("wrong number of args. expecting 2");
              lRootDir = new DirectoryInfo(args[1]);
              if (!lRootDir.Exists) throw new Exception(string.Format("directory {0} doesn't exist", lRootDir.FullName));
            }
            catch (Exception e)
            {
              Console.Error.WriteLine("Error: {0}\n\n{1}", e, UNUSED_VRAM_USAGE);
              return 1;
            }
            return VramUsage(lRootDir);
          }

        case "getallxml":
          {
            DirectoryInfo lRootDir; FileInfo lXmlOut;
            try
            {
              if (args.Length != 3) throw new Exception("wrong number of args. expecting 3");
              lRootDir = new DirectoryInfo(args[1]);
              if (!lRootDir.Exists) throw new Exception(string.Format("directory {0} doesn't exist", lRootDir.FullName));
              lXmlOut = new FileInfo(args[2]);
            }
            catch (Exception e)
            {
              Console.Error.WriteLine("Error: {0}\n\n{1}", e, GET_ALL_XML_USAGE);
              return 1;
            }
            return GetAllXml(lRootDir, lXmlOut);
          }

        case "findcdoffsets":
          {
            FileInfo lCDImage;
            DirectoryInfo lRootDir;
            try
            {
              if (args.Length != 3)
              {
                throw new Exception("wrong number of args. expecting 3");
              }
              lCDImage = new FileInfo(args[1]);
              lRootDir = new DirectoryInfo(args[2]);

              if (!lCDImage.Exists)
              {
                throw new Exception(string.Format("CD Image {0} doesn't exist", lCDImage.FullName));
              }
              if (!lRootDir.Exists)
              {
                throw new Exception(string.Format("Directory {0} doesn't exist", lRootDir.FullName));
              }
            }
            catch (Exception e)
            {
              Console.Error.WriteLine("Error: {0}\n\n{1}", e, FIND_CD_OFFSETS_USAGE);
              return 1;
            }
            return FindCDOffsets(lCDImage, lRootDir);
          }

        case "findcoursenameoffsets":
          {
            FileInfo lCDImage;
            try
            {
              if (args.Length != 2)
              {
                throw new Exception("wrong number of args. expecting 2");
              }
              lCDImage = new FileInfo(args[1]);

              if (!lCDImage.Exists)
              {
                throw new Exception(string.Format("CD Image {0} doesn't exist", lCDImage.FullName));
              }
            }
            catch (Exception e)
            {
              Console.Error.WriteLine("Error: {0}\n\n{1}", e, FIND_COURSENAME_OFFSETS_USAGE);
              return 1;
            }
            return FindCourseNameOffsets(lCDImage);
          }

        case "help":
          if (args.Length == 1)
          {
            Console.Out.WriteLine("usage: mmedtool <command>\n\nwhere command is one of:\n  getallxml\n  vram\n  findcdoffsets");
            return 0;
          }
          else if (args.Length == 2)
          {
            switch (args[1].ToLower())
            {
              case "getallxml":
                Console.Out.WriteLine(GET_ALL_XML_USAGE);
                return 0;
              case "vram":
                Console.Out.WriteLine(UNUSED_VRAM_USAGE);
                return 0;
              case "findcdoffsets":
                Console.Out.WriteLine(FIND_CD_OFFSETS_USAGE);
                return 0;
              default:
                Console.Error.WriteLine("mmedtool: help: unrecognised command");
                return 1;
            }
          }
          else
          {
            Console.Error.WriteLine("mmedtool: help: unrecognised command");
            return 1;
          }

        default:
          Console.Error.WriteLine(USAGE);
          return 1;
      }
    }

    #region CD image processing stuff

    ///========================================================================
    /// Static Method : FindCourseNameOffsets
    /// 
    /// <summary>
    /// 	Find the locations in the CD image of all the course names.
    /// 
    ///   This only finds course names that are coded into the MMCD class.
    /// </summary>
    /// <param name="xiCDImage"></param>
    /// <returns></returns>
    ///========================================================================
    private static int FindCourseNameOffsets(FileInfo xiCDImage)
    {
      CDImage lCDImage = new CDImage(xiCDImage);

      foreach (MMCD.Course lCourse in MMCD.Courses)
      {
        if (lCourse.CourseName == "")
        {
          continue;
        }

        byte[] lBytes = Encoding.ASCII.GetBytes(lCourse.CourseNameWithLineBreaks);

        long lIndex = 0;
        bool lFound = false;
        while ((lIndex = lCDImage.Find(lBytes, lIndex)) > 0)
        {
          WriteLine("{0}: {1}", lCourse.FileName, lIndex++, lCourse.CourseName);
          lFound = true;
        }

        if (!lFound)
        {
          WriteLine("{0}: Not found", lCourse.FileName, lCourse.CourseName);
        }
      }

      return 0;
    }

    ///========================================================================
    /// Static Method : FindCDOffsets
    /// 
    /// <summary>
    /// 	Find the offsets in the CD image of all the courses in the given
    ///   directory (and subdirectories).
    /// </summary>
    /// <param name="xiCDImage"></param>
    /// <param name="xiRootDir"></param>
    /// <returns></returns>
    ///========================================================================
    private static int FindCDOffsets(FileInfo xiCDImage, DirectoryInfo xiRootDir)
    {
      CDImage lCDImage = new CDImage(xiCDImage);

      //=======================================================================
      // Find all the levels
      //=======================================================================
      Regex lLevelNameRegex = new Regex("[A-Z]+\\\\[A-Z]+[0-9]\\.DAT$", RegexOptions.IgnoreCase);
      foreach (FileInfo lFile in xiRootDir.GetFiles("*.DAT", SearchOption.AllDirectories))
      {
        //=====================================================================
        // Read the entire level into memory
        //=====================================================================
        byte[] lLevelBytes = new byte[lFile.Length];
        using (FileStream lFileStream = lFile.OpenRead())
        {
          lFileStream.Read(lLevelBytes, 0, lLevelBytes.Length);
        }

        //=====================================================================
        // Find the level in the CD image and output
        //=====================================================================
        long lIndex = lCDImage.Find(lLevelBytes, 0);

        if (lIndex > -1)
        {
          WriteLine("{0}: {1}", lFile.Name, lIndex);
        }
        else
        {
          WriteLine("{0}: Not found", lFile.Name);
        }
      }

      return 0;
    }

    #endregion

    #region VRAM stuff

    const int TEX_PAGE_WIDTH = MMEd.Viewers.VRAMViewer.TEX_PAGE_WIDTH;
    const int TEX_PAGE_HEIGHT = MMEd.Viewers.VRAMViewer.TEX_PAGE_HEIGHT;
    const int WIDTH_SCALE = MMEd.Viewers.VRAMViewer.WIDTH_SCALE;

    private static int VramUsage(DirectoryInfo xiTreeRoot)
    {
      DateTime lStartTime = DateTime.Now;
      Write("Loading files...");
      Files files = LoadAllFilesFromRootPath(xiTreeRoot);
      WriteLine("{0} binary files loaded in {1}ms", files.FileHolders.Count, DateTime.Now.Subtract(lStartTime).TotalMilliseconds);
      WriteLine("Computing aggregate VRAM image...");

      Bitmap lCommonImage = new Bitmap(16 * TEX_PAGE_WIDTH, 2 * TEX_PAGE_HEIGHT);
      int[,] lPixelUsageMap = new int[lCommonImage.Width, lCommonImage.Height];
      Graphics g = Graphics.FromImage(lCommonImage);
      g.Clear(Color.Black);

      foreach (FileHolder fh in files.FileHolders)
      {
        WriteLine("Adding level {0}", fh.FileName);
        AddChunkToImage(fh.Level, lCommonImage, lPixelUsageMap);
      }

      WriteLine("saving used image to used.png:");
      lCommonImage.Save("used.png", System.Drawing.Imaging.ImageFormat.Png);

      WriteLine("saving common image to common.png:");
      for (int x = 0; x < lCommonImage.Width; x++)
      {
        for (int y = 0; y < lCommonImage.Height; y++)
        {
          if (lPixelUsageMap[x, y] <= 1)
          {
            lCommonImage.SetPixel(x, y, Color.Black);
          }
        }
      }
      lCommonImage.Save("common.png", System.Drawing.Imaging.ImageFormat.Png);

      WriteLine("saving bw used image to usedbw.png:");
      for (int x = 0; x < lCommonImage.Width; x++)
      {
        for (int y = 0; y < lCommonImage.Height; y++)
        {
          lCommonImage.SetPixel(x, y, lPixelUsageMap[x, y] == -1 ? Color.White : Color.Black);
        }
      }
      lCommonImage.Save("usedbw.png", System.Drawing.Imaging.ImageFormat.Png);

      WriteLine("Total time elapsed: {0}ms", DateTime.Now.Subtract(lStartTime).TotalMilliseconds);
      return 0;
    }

    private static void AddChunkToImage(Chunk xiChunk, Bitmap xiBmp, int[,] xiPixelUsageMap)
    {
      if (xiChunk is TIMChunk)
      {
        try
        {
          TIMChunk c = (TIMChunk)xiChunk;
          Bitmap lTIMImage = c.ToBitmap();
          int lPixelsPerTwoBytes;
          switch (c.BPP)
          {
            case TIMChunk.TimBPP._4BPP:
              lPixelsPerTwoBytes = 4;
              break;
            case TIMChunk.TimBPP._8BPP:
              lPixelsPerTwoBytes = 2;
              break;
            case TIMChunk.TimBPP._16BPP:
              lPixelsPerTwoBytes = 1;
              break;
            default: throw new Exception("Can't deal with this BPP");
          }

          Rectangle lDestRect = new Rectangle(
            WIDTH_SCALE * c.ImageOrgX,
            c.ImageOrgY,
            c.ImageWidth * WIDTH_SCALE / lPixelsPerTwoBytes,
            c.ImageHeight);
          int lWidthScale = WIDTH_SCALE / lPixelsPerTwoBytes;
          for (int y = lDestRect.Top; y < lDestRect.Bottom; y++)
          {
            for (int x = lDestRect.Left; x < lDestRect.Right; x++)
            {
              Color lFromTIM = lTIMImage.GetPixel((x - lDestRect.Left) * lWidthScale, y - lDestRect.Top);
              SetPixel(xiBmp, x, y, lFromTIM, xiPixelUsageMap);
            }
          }

          if (c.Palette != null)
          {
            if (c.ClutCount != 1) throw new Exception("Don't know what to do with multi-CLUT TIMs");
            for (int palIdx = 0; palIdx < c.Palette.Length; palIdx++)
            {
              Color col = Color.FromArgb(Utils.PS16bitColorToARGB(c.Palette[palIdx]));
              for (int x = 0; x < WIDTH_SCALE; x++)
              {
                SetPixel(xiBmp, WIDTH_SCALE * (c.PaletteOrgX + palIdx) + x, c.PaletteOrgY, col, xiPixelUsageMap);
              }
            }
          }
        }
        catch (Exception e)
        {
          Console.Error.WriteLine("Error: {0}\nSkipping this TIM", e);
        }
      }
      else
      {
        foreach (Chunk c in xiChunk.GetChildren())
        {
          AddChunkToImage(c, xiBmp, xiPixelUsageMap);
        }
      }
    }

    private static void SetPixel(Bitmap xiBmp, int x, int y, Color xiCol, int[,] xiPixelUsageMap)
    {
      int lPixUsage = xiPixelUsageMap[x, y];
      if (lPixUsage == 0)
      {
        xiBmp.SetPixel(x, y, xiCol);
        xiPixelUsageMap[x, y] = 1;
      }
      else if (lPixUsage > 0)
      {
        Color lFromAcc = xiBmp.GetPixel(x, y);
        if (xiCol == lFromAcc)
        {
          xiPixelUsageMap[x, y]++;
        }
        else
        {
          xiPixelUsageMap[x, y] = -1;
        }
      }
    }

    #endregion

    private static int GetAllXml(DirectoryInfo xiTreeRoot, FileInfo xiOutFile)
    {
      DateTime lStartTime = DateTime.Now;
      Files lAcc = LoadAllFilesFromRootPath(xiTreeRoot);
      Write("Serialising to file...");
      XmlSerializer xs = new XmlSerializer(typeof(Files));
      using (TextWriter tw = xiOutFile.CreateText())
      {
        xs.Serialize(tw, lAcc);
      }
      WriteLine("done");
      WriteLine("{0} files written in {1}ms", lAcc.FileHolders.Count, DateTime.Now.Subtract(lStartTime).TotalMilliseconds);
      return 0;
    }

    private static Files LoadAllFilesFromRootPath(DirectoryInfo xiTreeRoot)
    {
      Files lAcc = new Files();
      Regex lLevelNameRegex = new Regex("[A-Z]+\\\\[A-Z]+[0-9]\\.DAT$", RegexOptions.IgnoreCase);
      foreach (FileInfo file in xiTreeRoot.GetFiles("*.DAT", SearchOption.AllDirectories))
      {
        if (lLevelNameRegex.IsMatch(file.FullName))
        {
          using (FileStream fs = file.OpenRead())
          {
            Level lev = new Level(fs);
            FileHolder fh = new FileHolder(file.FullName, lev);
            lAcc.FileHolders.Add(fh);
          }
        }
      }
      return lAcc;
    }

    #region XML proxy classes

    public class Files
    {
      public List<FileHolder> FileHolders = new List<FileHolder>();
    }

    public class FileHolder
    {
      [XmlAttribute()]
      public string FileName;

      public Level Level;

      public FileHolder() { }

      public FileHolder(string xiFileName, Level xiLevel)
      {
        FileName = xiFileName;
        Level = xiLevel;
      }
    }

    #endregion

    private static void WriteLine(string xiFormat, params object[] xiArgs)
    {
      Console.Out.WriteLine(xiFormat, xiArgs);
    }

    private static void Write(string xiFormat, params object[] xiArgs)
    {
      Console.Out.Write(xiFormat, xiArgs);
    }
  }
}
