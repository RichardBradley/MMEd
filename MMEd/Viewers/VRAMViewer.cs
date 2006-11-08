using System;
using System.Collections.Generic;
using System.Text;
using MMEd;
using MMEd.Chunks;
using System.Drawing;
using System.Windows.Forms;
using MMEd.Util;

// views and edits how the images are laid out in VRAM

namespace MMEd.Viewers
{
  public class VRAMViewer : Viewer
  {
    // the VRAM is scaled horizontally by a factor of
    // 4, to allow for 4bpp TIMs (I would have expected this to be 2...)
    // How to deal with TIMs of other bit depths is not yet decided
    public const int WIDTH_SCALE_FOR_4BPP_TIMS = 4;

    public const int TEX_PAGE_WIDTH = 64 * WIDTH_SCALE_FOR_4BPP_TIMS;
    public const int TEX_PAGE_HEIGHT = 256;

    private VRAMViewer(MainForm xiMainForm) : base(xiMainForm) 
    {
      //add view mode menus:
      mOptionsMenu = new ToolStripMenuItem("VRAM");
      //
      // create menu options
      PropertyController.NamedValueHolder[] lPropValues = new PropertyController.NamedValueHolder[33];
      lPropValues[0] = new PropertyController.NamedValueHolder("Show entire VRAM", -1);
      for (int i = 0; i < 32; i++)
        lPropValues[i+1] = new PropertyController.NamedValueHolder(string.Format("Page {0}", i), i);
      PropertyController lPageCtrl = new PropertyController(this, "SelectedPage", lPropValues);
      mOptionsMenu.DropDownItems.AddRange(lPageCtrl.CreateMenuItems());

      mMainForm.mMenuStrip.Items.Add(mOptionsMenu);
    }

    public override bool CanViewChunk(Chunk xiChunk)
    {
      return xiChunk is Level
        || xiChunk is NamedImageGroup
        || xiChunk is TIMChunk;
    }

    // Create an instance of the viewer manager class
    public static Viewer InitialiseViewer(MainForm xiMainForm)
    {
      return new VRAMViewer(xiMainForm);
    }

    ToolStripMenuItem mOptionsMenu;

    public static VRAMViewer GetInstance()
    {
      return (VRAMViewer)Viewer.GetInstance(typeof(VRAMViewer));
    }

    public int SelectedPage
    {
      get
      {
        return mSelectedPage;
      }
      set
      {
        mSelectedPage = value;
        mMainForm.VRAMPictureBox.Image = null;
        SetSubject(mSubject);
      }
    }
    private int mSelectedPage = 0;

    private Chunk mSubject = null;

    public override void SetSubject(Chunk xiChunk)
    {
      if (!CanViewChunk(xiChunk)) xiChunk = null;
      if (mSubject == xiChunk && mMainForm.VRAMPictureBox.Image != null) return;
      mSubject = xiChunk;

      mOptionsMenu.Visible = (mSubject != null);

      if (xiChunk == null)
      {
        //don't bother freeing the image, as it'll just cause memory churn
        return;
      }
      else
      {
        if (mMainForm.VRAMPictureBox.Image == null)
        {
          Bitmap lNewImage;
          if (SelectedPage == -1)
          {
            lNewImage = new Bitmap(16 * TEX_PAGE_WIDTH, 2 * TEX_PAGE_HEIGHT);
            Graphics g = Graphics.FromImage(lNewImage);
            g.Clear(Color.Black);
            AddChunkToImage(mSubject, g);
          }
          else
          {
            lNewImage = GetTexturePage(mMainForm.Level, SelectedPage);
          }
          
          mMainForm.VRAMPictureBox.Image = lNewImage;
          mMainForm.VRAMPictureBox.SizeMode = PictureBoxSizeMode.Normal;
          mMainForm.VRAMPictureBox.Width = lNewImage.Width;
          mMainForm.VRAMPictureBox.Height = lNewImage.Height;
        }
        mMainForm.VRAMPictureBox.Invalidate();
      }
    }

    // returns a Bitmap holding the textures in the given Texture page 
    // from VRAM
    public Bitmap GetTexturePage(Level xiLevel, int xiPageId)
    {
      //check that the level hasn't changed, which would invalidate our cache
      if (mMainForm.Level != mLevelForCachedTexPages)
      {
        mCachedTexPages = new Bitmap[32];
        mLevelForCachedTexPages = xiLevel;
      }
      //cache miss?
      if (mCachedTexPages[xiPageId] == null)
      {
        Bitmap lPage = new Bitmap(WIDTH_SCALE_FOR_4BPP_TIMS * 64, 256); //(i.e. 256x256)
        Graphics g = Graphics.FromImage(lPage);
        g.Clear(Color.Black);
        g.Clip = new Region(new Rectangle(new Point(), lPage.Size));
        g.TranslateTransform(-(xiPageId % 16) * (WIDTH_SCALE_FOR_4BPP_TIMS * 64), -xiPageId / 16 * 256);
        AddChunkToImage(mLevelForCachedTexPages, g);
        mCachedTexPages[xiPageId] = lPage;
      }

      return mCachedTexPages[xiPageId];
    }

    private Level mLevelForCachedTexPages;
    private Bitmap[] mCachedTexPages;

    private void AddChunkToImage(Chunk c, Graphics g)
    {
      if (c is Level)
      {
        AddChunkToImage((Level)c, g);
      }
      else if (c is NamedImageGroup)
      {
        AddChunkToImage((NamedImageGroup)c, g);
      }
      else if (c is TIMChunk)
      {
        AddChunkToImage((TIMChunk)c, g);
      }
      else
      {
        throw new Exception(string.Format("Unexpected type: {0}", c.GetType()));
      }
    }

    private void AddChunkToImage(Level c, Graphics g)
    {
      foreach (Chunk nig in c.NamedImageGroups.mChildren)
      {
        if (nig is NamedImageGroup)
        {
          AddChunkToImage((NamedImageGroup)nig, g);
        }
      }
    }

    private void AddChunkToImage(NamedImageGroup c, Graphics g)
    {
      foreach (Chunk tim in c.mChildren)
      {
        if (tim is TIMChunk)
        {
          AddChunkToImage((TIMChunk)tim, g);
        }
      }
    }

    private void AddChunkToImage(TIMChunk c, Graphics g)
    {
      RectangleF lDestRect = new RectangleF(WIDTH_SCALE_FOR_4BPP_TIMS * c.ImageOrgX, c.ImageOrgY, c.ImageWidth, c.ImageHeight);
      RectangleF lClipRect = g.ClipBounds;
      if (!(lClipRect.IntersectsWith(lDestRect)))
      {
        return;
      }
      g.DrawImage(c.ToBitmap(), lDestRect);
    }

    public override System.Windows.Forms.TabPage Tab
    {
      get { return mMainForm.ViewTabVRAM; }
    }
  }
}
