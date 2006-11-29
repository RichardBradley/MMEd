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
    public const int WIDTH_SCALE = 4;

    public const int TEX_PAGE_WIDTH = 64 * WIDTH_SCALE;
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
      return true;
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
    private int mSelectedPage = -1;

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
          lNewImage = GetTexturePage(mSubject, SelectedPage);
        }

        mMainForm.VRAMPictureBox.Image = lNewImage;
        mMainForm.VRAMPictureBox.SizeMode = PictureBoxSizeMode.Normal;
        mMainForm.VRAMPictureBox.Width = lNewImage.Width;
        mMainForm.VRAMPictureBox.Height = lNewImage.Height;

        mMainForm.VRAMPictureBox.Invalidate();
      }
    }

    // returns a Bitmap holding the textures in the given Texture page 
    // from VRAM
    public Bitmap GetTexturePage(Chunk xiRootChunk, int xiPageId)
    {
      //check that the level hasn't changed, which would invalidate our cache
      if (xiRootChunk != mRootChunkForCachedTexPages)
      {
        mCachedTexPages = new Bitmap[32];
        mRootChunkForCachedTexPages = xiRootChunk;
      }
      //cache miss?
      if (mCachedTexPages[xiPageId] == null)
      {
        Bitmap lPage = new Bitmap(WIDTH_SCALE * 64, 256); //(i.e. 256x256)
        Graphics g = Graphics.FromImage(lPage);
        g.Clear(Color.Black);
        g.Clip = new Region(new Rectangle(new Point(), lPage.Size));
        g.TranslateTransform(-(xiPageId % 16) * (WIDTH_SCALE * 64), -xiPageId / 16 * 256);
        AddChunkToImage(mRootChunkForCachedTexPages, g);
        mCachedTexPages[xiPageId] = lPage;
      }

      return mCachedTexPages[xiPageId];
    }

    private Chunk mRootChunkForCachedTexPages;
    private Bitmap[] mCachedTexPages;

    private void AddChunkToImage(Chunk c, Graphics g)
    {
      if (c is TIMChunk) AddChunkToImage((TIMChunk)c, g);

      foreach (Chunk child in c.GetChildren())
      {
        if (child is TIMChunk)
        {
          AddChunkToImage((TIMChunk)child, g);
        }
        else
        {
          AddChunkToImage(child, g);
        }
      }
    }

    private void AddChunkToImage(TIMChunk c, Graphics g)
    {
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

      RectangleF lDestRect = new RectangleF(
        WIDTH_SCALE * c.ImageOrgX,
        c.ImageOrgY,
        c.ImageWidth * WIDTH_SCALE / lPixelsPerTwoBytes,
        c.ImageHeight);
      RectangleF lClipRect = g.ClipBounds;
      if (!(lClipRect.IntersectsWith(lDestRect)))
      {
        return;
      }
      g.DrawImage(c.ToBitmap(), lDestRect);
      
      if (c.Palette != null)
      {
        if (c.ClutCount != 1) throw new Exception("Don't know what to do with multi-CLUT TIMs");
        for (int palIdx=0; palIdx<c.Palette.Length; palIdx++)
        {
          Color col = Color.FromArgb(Utils.PS16bitColorToARGB(c.Palette[palIdx]));
          Brush br = new SolidBrush(col);
          Rectangle rect = new Rectangle(
            WIDTH_SCALE * (c.PaletteOrgX + palIdx),
            c.PaletteOrgY,
            WIDTH_SCALE,
            1);
          g.FillRectangle(br, rect);
        }
      }
    }

    public override System.Windows.Forms.TabPage Tab
    {
      get { return mMainForm.ViewTabVRAM; }
    }
  }
}
