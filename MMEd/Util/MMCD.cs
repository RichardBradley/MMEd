using System;
using System.Collections.Generic;
using System.Text;

namespace MMEd.Util
{
  ///==========================================================================
  /// Class : MMCD
  /// 
  /// <summary>
  /// 	Information about the MMs CD image
  /// </summary>
  /// <remarks>
  ///   To fill in course name offsets ("new long[] {}"):
  ///    * Make sure the course name is filled in below, with the newline
  ///       in the right place
  ///    * Run MMEdTool findcoursenameoffsets [cd-image]
  ///    * Wait a long time
  ///    * Concatenate all the offsets for your course into a comma-separated
  ///       list, and insert into the long[] for that course
  /// </remarks>
  ///==========================================================================
  public static class MMCD
  {
    public static List<Course> Courses = new List<Course>(
      new Course[] {
        new Course("SCHOOL1", "", 670091880, 450560, new long[] {}),
        new Course("SCHOOL2", "TRUCKER'S\nLUCK", 670609320, 512000, new long[] {136507224,160186576,162254000,164321200,166388828,168456240,182577272,184644688,186712100,188779520,190846928}),
        new Course("SCHOOL3", "TEXT BOOK\nMANOEUVER", 671197320, 583680, new long[] {136507100,160186452,162253876,164321076,166388704,168456116,182577148,184644564,186711976,188779396,190846804}),
        new Course("SCHOOL4", "", 671867640, 530432, new long[] {}),
        new Course("SCHOOL5", "", 672476808, 520192, new long[] {}),
        new Course("SCHOOL6", "", 673074216, 434176, new long[] {}),
        new Course("REST1", "VINDALOO\nDRIVE-THRU", 666756744, 403456, new long[] {136507360,160186712,162254136,164321336,166388964,168456376,182577408,184644824,186712236,188779656,190847064}),
        new Course("REST2", "", 667220088, 544768, new long[] {}),
        new Course("REST3", "", 667845720, 450560, new long[] {}),
        new Course("REST4", "", 668363160, 610304, new long[] {}),
        new Course("REST5", "", 669064056, 440320, new long[] {}),
        new Course("POOL1", "SWERVE\nSHOT", 372232248, 518144, new long[] {136507408,160186760,162254184,164321384,166389012,168456424,182577456,184644872,186712284,188779704,190847112}),
        new Course("POOL2", "RACK\n'N ROLL", 372827304, 573440, new long[] {136507292,160186644,162254068,164321268,166388896,168456308,182577340,184644756,186712168,188779588,190846996}),
        new Course("POOL3", "RIGHT\nON CUE", 373485864, 548864, new long[] {136507172,160186524,162253948,164321148,166388776,168456188,182577220,184644636,186712048,188779468,190846876}),
        new Course("POOL4", "POT\nLUCK", 374116200, 671744, new long[] {136507064,160186416,162253840,164321040,166388668,168456080,182577112,184644528,186711940,188779360,190846768}),
        new Course("POOL5", "LOVE\nTRIANGLE", 374887656, 743424, new long[] {136506956,160186308,162253732,164320932,166388560,168455972,182577004,184644420,186711832,188779252,190846660}),
        new Course("LAB1", "", 376209480, 600064, new long[] {}),
        new Course("LAB2", "", 376898616, 624640, new long[] {}),
        new Course("LAB3", "INTERESTING\nVOYAGE", 377615976, 780288, new long[] {136507152,160186504,162253928,164321128,166388756,168456168,182577200,184644616,186712028,188779448,190846856}),
        new Course("LAB4", "FORMULA X", 378512088, 702464, new long[] {136507052,160186404,162253828,164321028,166388656,168456068,182577100,184644516,186711928,188779348,190846756}),
        new Course("LAB5", "", 379318824, 765952, new long[] {}),
        new Course("LAB6", "PERIODIC\nPARK", 380198472, 630784, new long[] {136506872,160186224,162253648,164320848,166388476,168455888,182576920,184644336,186711748,188779168,190846576}),
        new Course("LAB7", "CHEMICAL\nWARFARE", 380922888, 575488, new long[] {136506784,160186136,162253560,164320760,166388388,168455800,182576832,184644248,186711660,188779080,190846488}),
        new Course("GARDEN1", "", 366462792, 714752, new long[] {}),
        new Course("GARDEN2", "BEWARE OF\nTHE DOG", 367283640, 669696, new long[] {136507308,160186660,162254084,164321284,166388912,168456324,182577356,184644772,186712184,188779604,190847012}),
        new Course("GARDEN3", "", 368052744, 704512, new long[] {}),
        new Course("GARDEN4", "", 368861832, 733184, new long[] {}),
        new Course("GARDEN5", "", 369703848, 751616, new long[] {}),
        new Course("GARDEN6", "", 370567032, 505856, new long[] {}),
        new Course("GARDEN7", "", 371147976, 440320, new long[] {}),
        new Course("BREAKY1", "CHEESEY\nJUMPS", 361504776, 477184, new long[] {136507440,160186792,162254216,164321416,166389044,168456456,182577488,184644904,186712316,188779736,190847144}),
        new Course("BREAKY2", "CEREAL\nKILLER", 362052792, 618496, new long[] {136507328,160186680,162254104,164321304,166388932,168456344,182577376,184644792,186712204,188779624,190847032}),
        new Course("BREAKY3", "BREAKFAST\nAT CHERRY'S", 362763096, 452608, new long[] {136507200,160186552,162253976,164321176,166388804,168456216,182577248,184644664,186712076,188779496,190846904}),
        new Course("BREAKY4", "", 363282888, 477184, new long[] {}),
        new Course("BREAKY5", "", 363830904, 677888, new long[] {}),
        new Course("BREAKY6", "", 364609416, 483328, new long[] {}),
        new Course("BREAKY7", "", 365164488, 638976, new long[] {}),
        new Course("BEACH1", "PEBBLE\nDASH", 661737576, 780288, new long[] {}),
        new Course("BEACH2", "BIKINI\nBLAZER", 662633688, 675840, new long[] {}),
        new Course("BEACH3", "BEACHED\nBUGGIES", -1 /* This one was missing from my CD image. Run 'mmedtool findcdoffsets' to plug the gap */, 432128, new long[] {}),
        new Course("BEACH4", "BUCKET\nAND SPEED", 663906120, 727040, new long[] {}),
        new Course("BEACH5", "SAND\nBLASTER", 664741080, 712704, new long[] {}),
        new Course("BEACH6", "DUNES OF\nHAZARD", 665559576, 567296, new long[] {}),
        new Course("CHERRY1", "", 382160040, 296960, new long[] {}),
        new Course("CHERRY2", "", 382501080, 241664, new long[] {}),
        new Course("CHERRY3", "", 382778616, 241664, new long[] {}),
        new Course("CHERRY4", "", 383056152, 309248, new long[] {}),
        new Course("CHERRY5", "", 383411304, 311296, new long[] {})
    });

    ///========================================================================
    /// Class : Course
    /// 
    /// <summary>
    /// 	Information about a course on the MMs CD
    /// </summary>
    ///========================================================================
    public class Course
    {
      public string FileName;
      public string CourseName;
      public string CourseNameWithLineBreaks;
      public long CDOffset;
      public long CDLength;
      public long[] NameOffsets;

      public Course(string xiFileName, string xiCourseName, long xiCDOffset, long xiCDLength, long[] xiNameOffsets)
      {
        FileName = xiFileName;
        CourseNameWithLineBreaks = xiCourseName;
        CourseName = xiCourseName.Replace('\n', ' ');
        CDOffset = xiCDOffset;
        CDLength = xiCDLength;
        NameOffsets = xiNameOffsets;
      }

      ///======================================================================
      /// Method : GetCDCourseName
      /// 
      /// <summary>
      /// 	Convert a course name into one that can be written to the CD.
      ///   The main purpose of this is to insert a linebreak somewhere 
      ///   appropriate.
      /// </summary>
      /// <param name="xiCourseName"></param>
      /// <returns></returns>
      ///======================================================================
      public string GetCDCourseName(string xiCourseName)
      {
        string lRet = xiCourseName.PadRight(CourseName.Length, ' ');

        if (xiCourseName.Length > CourseName.Length)
        {
          lRet = xiCourseName.Substring(0, CourseName.Length);
        }

        if (CourseNameWithLineBreaks == CourseName)
        {
          //===================================================================
          // No newlines in the original, so don't put any in the new one
          //===================================================================
          return lRet;
        }

        int lCentre = lRet.Length / 2;
        int lPrevSpace = lRet.LastIndexOf(' ', lCentre);
        int lNextSpace = lRet.IndexOf(' ', lCentre);
        int lChosenSpace;

        if (lNextSpace == -1 && lPrevSpace == -1)
        {
          //===================================================================
          // No spaces, so we can't have a newline anywhere sensible
          //===================================================================
          return lRet;
        }
        else if (lPrevSpace == -1)
        {
          lChosenSpace = lNextSpace;
        }
        else if (lNextSpace == -1)
        {
          lChosenSpace = lPrevSpace;
        }
        else if ((lCentre - lPrevSpace) < (lNextSpace - lCentre))
        {
          lChosenSpace = lNextSpace;
        }
        else
        {
          lChosenSpace = lPrevSpace;
        }

        return lRet.Substring(0, lChosenSpace) + "\n" + lRet.Substring(lChosenSpace + 1);
      }

      public override string ToString()
      {
        return FileName + " - " + CourseName;
      }

      public override bool Equals(object obj)
      {
        if (obj is string)
        {
          return obj.Equals(this.FileName);
        }

        return base.Equals(obj);
      }

      public override int GetHashCode()
      {
        return this.FileName.GetHashCode();
      }
    }
  }
}

/* mmed findcoursenameoffsets - output I haven't processed yet:
BEACH1: 136507380
BEACH1: 160186732
BEACH1: 162254156
BEACH1: 164321356
BEACH1: 166388984
BEACH1: 168456396
BEACH1: 182577428
BEACH1: 184644844
BEACH1: 186712256
BEACH1: 188779676
BEACH1: 190847084
BEACH2: 136507260
BEACH2: 160186612
BEACH2: 162254036
BEACH2: 164321236
BEACH2: 166388864
BEACH2: 168456276
BEACH2: 182577308
BEACH2: 184644724
BEACH2: 186712136
BEACH2: 188779556
BEACH2: 190846964
BEACH3: 136507136
BEACH3: 160186488
BEACH3: 162253912
BEACH3: 164321112
BEACH3: 166388740
BEACH3: 168456152
BEACH3: 182577184
BEACH3: 184644600
BEACH3: 186712012
BEACH3: 188779432
BEACH3: 190846840
BEACH4: 136507032
BEACH4: 160186384
BEACH4: 162253808
BEACH4: 164321008
BEACH4: 166388636
BEACH4: 168456048
BEACH4: 182577080
BEACH4: 184644496
BEACH4: 186711908
BEACH4: 188779328
BEACH4: 190846736
BEACH5: 136506928
BEACH5: 160186280
BEACH5: 162253704
BEACH5: 164320904
BEACH5: 166388532
BEACH5: 168455944
BEACH5: 182576976
BEACH5: 184644392
BEACH5: 186711804
BEACH5: 188779224
BEACH5: 190846632
BEACH6: 136506856
BEACH6: 160186208
BEACH6: 162253632
BEACH6: 164320832
BEACH6: 166388460
BEACH6: 168455872
BEACH6: 182576904
BEACH6: 184644320
BEACH6: 186711732
BEACH6: 188779152
BEACH6: 190846560
 */
