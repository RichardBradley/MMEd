using System;
using System.Collections.Generic;
using System.Text;
using MMEd;
using MMEd.Chunks;
using System.Drawing;
using System.Windows.Forms;


namespace MMEd.Viewers
{
    public class GridViewer : Viewer
    {
        private GridViewer(MainForm xiMainForm)
            : base(xiMainForm)
        {

            xiMainForm.GridDisplayPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.GridDisplayPanel_Paint);
            foreach (string name in Enum.GetNames(typeof(FlatChunk.TexMetaDataEntries)))
            {
                mMainForm.GridViewMetaTypeCombo.Items.Add(name);
            }
            mMainForm.GridViewMetaTypeCombo.SelectedIndex = 0;


            mMainForm.GridViewRadioImages.CheckedChanged += new System.EventHandler(this.InvalidateGridDisplayEvent);
            mMainForm.GridViewRadioImgNum.CheckedChanged += new System.EventHandler(this.InvalidateGridDisplayEvent);
            mMainForm.GridViewMetaTypeCombo.SelectedIndexChanged += new System.EventHandler(this.InvalidateGridDisplayEvent);
            mMainForm.GridDisplayPanel.MouseMove += new MouseEventHandler(this.InvalidateGridDisplayMouseEvent);
            mMainForm.GridDisplayPanel.MouseLeave += new EventHandler(this.InvalidateGridDisplayEvent);
            mMainForm.GridDisplayPanel.MouseClick += new MouseEventHandler(this.GridDisplayMouseClick);

            //mMainForm.ViewerTabControl.KeyPress += new KeyPressEventHandler(this.ViewerTabControl_KeyPress);
        }

        private void GridDisplayPanel_Paint(object sender, PaintEventArgs e)
        {
            if (mSubject == null) return;

            bool lDrawNumbers = mMainForm.GridViewRadioImgNum.Checked;
            int lDrawNumType =
               (int) (FlatChunk.TexMetaDataEntries)
                Enum.Parse(typeof(FlatChunk.TexMetaDataEntries), (string)mMainForm.GridViewMetaTypeCombo.SelectedItem);

            int lNumberOffX = 0, lNumberOffY=0;
            Font lNumberFont = null;
            Brush lNumberFGBrush = null, lNumberBGBrush =null;
            if (lDrawNumbers)
            {
                lNumberFont = new Font(FontFamily.GenericMonospace, 10);
                lNumberFGBrush = new SolidBrush(Color.Black);
                lNumberBGBrush = new SolidBrush(Color.White);
                lNumberOffX = mSubjectTileWidth/2;
                lNumberOffY = mSubjectTileHeight/2;
            }

            for (int x = (e.ClipRectangle.Left / mSubjectTileWidth);
                x < (e.ClipRectangle.Right / mSubjectTileWidth) + 1
                && x < mSubject.Width; x++)
            {
                for (int y = (e.ClipRectangle.Top / mSubjectTileHeight);
                    y < (e.ClipRectangle.Bottom / mSubjectTileHeight) + 1
                    && y < mSubject.Height; y++)
                {
                    try
                    {
                        e.Graphics.DrawImageUnscaled(
                            mMainForm.Level.GetTileById(mSubject.TextureIds[x][y]).ToBitmap(),
                            x * mSubjectTileWidth,
                            y * mSubjectTileWidth);

                        if (lDrawNumbers)
                        {

                            string text = string.Format("{0:x}", mSubject.TexMetaData[x][y][lDrawNumType]);

                            SizeF size = e.Graphics.MeasureString(text, lNumberFont);

                            float xf = x * mSubjectTileWidth + lNumberOffX - size.Width / 2;
                            float yf = y * mSubjectTileHeight + lNumberOffY - size.Height / 2;

                            e.Graphics.FillRectangle(lNumberBGBrush, xf, yf, size.Width, size.Height);

                            e.Graphics.DrawString(
                                text,
                                lNumberFont,
                                lNumberFGBrush,
                                xf,
                                yf); 
                        }
                    }
                    catch (NullReferenceException err)
                    {
                        Console.Error.WriteLine(err);
                    }
                }
            }

            //highlight editable square
            if (lDrawNumbers)
            {
                Point lMousePos = mMainForm.GridDisplayPanel.PointToClient(Cursor.Position);
                //assume graphics clip will take care of clipping
                e.Graphics.DrawRectangle(
                    new Pen(Color.Red, 2.0f),
                    lMousePos.X/mSubjectTileWidth*mSubjectTileWidth,
                    lMousePos.Y/mSubjectTileHeight*mSubjectTileHeight,
                    mSubjectTileWidth,
                    mSubjectTileHeight);
            }
        }

        public override bool CanViewChunk(Chunk xiChunk)
        {
            return xiChunk is FlatChunk;
        }

        // Create an instance of the viewer manager class
        public static Viewer InitialiseViewer(MainForm xiMainForm)
        {
            return new GridViewer(xiMainForm);
        }

        private FlatChunk mSubject = null;
        int mSubjectTileWidth;
        int mSubjectTileHeight;


        public override void SetSubject(Chunk xiChunk)
        {

            if (!(xiChunk is FlatChunk)) xiChunk = null;
            if (mSubject == xiChunk) return;
            mSubject = (FlatChunk)xiChunk;

            mMainForm.GridViewRadioImages.Checked = true;

            if (xiChunk == null)
            {
                mMainForm.GridDisplayPanel.Width = 100;
                mMainForm.GridDisplayPanel.Height = 100;
                mMainForm.GridDisplayPanel.Controls.Clear();
                mMainForm.GridViewRadioImgNum.Enabled = false;
            }
            else
            {
                //qq
                //fix the containing panel, otherwise it seems to creep
                //even though auto-size is false
                //Size parentBefore = mMainForm.GridDisplayPanel.Parent.Size;

                //find the width and height of the tex components
                short topLeftTexIdx = mSubject.TextureIds[0][0];
                TIMChunk firstTim = mMainForm.Level.GetTileById(topLeftTexIdx);
                mSubjectTileHeight = firstTim.ImageHeight;
                mSubjectTileWidth = firstTim.ImageWidth;

                mMainForm.GridDisplayPanel.Width = mSubjectTileWidth * mSubject.Width;
                mMainForm.GridDisplayPanel.Height = mSubjectTileHeight * mSubject.Height;

                mMainForm.GridViewRadioImgNum.Enabled = mSubject.TexMetaData != null;
            }
        }

        public override System.Windows.Forms.TabPage Tab
        {
            get { return mMainForm.ViewTabGrid; }
        }

        private void InvalidateGridDisplayEvent(object sender, EventArgs e)
        {
           mMainForm.GridDisplayPanel.Invalidate();
        }

        private void InvalidateGridDisplayMouseEvent(object sender, MouseEventArgs e)
        {
            mMainForm.GridDisplayPanel.Invalidate();
        }

        private void GridDisplayMouseClick(object sender, MouseEventArgs e)
        {
            if (mSubject != null
                && mMainForm.GridViewRadioImgNum.Checked)
            {
                int x = e.X / mSubjectTileWidth;
                int y = e.Y / mSubjectTileHeight;
                if (mSubject.TexMetaData != null
                    && x < mSubject.TexMetaData.Length
                    && y < mSubject.TexMetaData[x].Length)
                {
                    int lDrawNumType =
                      (int)(FlatChunk.TexMetaDataEntries)
                       Enum.Parse(typeof(FlatChunk.TexMetaDataEntries), (string)mMainForm.GridViewMetaTypeCombo.SelectedItem);
                    MessageBox.Show(string.Format("Value at ({0},{1}) is {2:x}, and if I had an InputBox function, I'd allow you to update it :-(", e.X, e.Y, mSubject.TexMetaData[x][y][lDrawNumType]));
                }
            }
        }

        /*
         * Doesn't work, was going to be text entry...
        private void ViewerTabControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (mMainForm.ViewerTabControl.SelectedTab == this.Tab)
            {
                Point lMousePt = Cursor.Position;
                Point lClientPt = Tab.PointToClient(lMousePt);
                Control c = Tab.GetChildAtPoint(lClientPt);
                 
                if (c == mMainForm.GridDisplayPanelHolder)
                {
                    lClientPt = mMainForm.GridDisplayPanel.PointToClient(lMousePt);
                    int x = lClientPt.X / mSubjectTileWidth;
                    int y = lClientPt.Y / mSubjectTileHeight;
                    MessageBox.Show(string.Format("{0} {1}", x, y));
                }
            }

        }
        */
    }
}
