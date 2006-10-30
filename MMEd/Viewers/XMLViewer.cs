using System;
using System.Collections.Generic;
using System.Text;
using MMEd;
using MMEd.Chunks;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;

namespace MMEd.Viewers
{
    class XMLViewer : Viewer
    {
        private XMLViewer(MainForm xiMainForm) : base(xiMainForm) 
        {
            xiMainForm.XMLViewerCommitBtn.Click += new System.EventHandler(this.XMLViewerCommitBtn_Click);
        }

        public void XMLViewerCommitBtn_Click(object xiSender, EventArgs xiArgs)
        {
            if (mSubject == null)
            {
                MessageBox.Show("Can't do that -- mSubject is null!");
                return;
            }
            XmlSerializer xs = new XmlSerializer(mSubject.GetType());
            try
            {
                Chunk lReplacement = (Chunk)xs.Deserialize(new StringReader(mMainForm.XMLTextBox.Text));
                TreeNode selNode = mSubject.TreeNode;
                TreeNode parentNode = selNode.Parent;
                if (parentNode == null)
                {
                    if (!(lReplacement is Level))
                    {
                        throw new Exception("Only the level node should have a null parent!");
                    }
                    mMainForm.Level = (Level)lReplacement;
                }
                else
                {
                    ((Chunk)parentNode.Tag).ReplaceChild(mSubject, lReplacement);
                    mMainForm.Level = mMainForm.Level;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.StackTrace);
                MessageBox.Show("Exception: " + e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // all chunk types can be viewed...
        public override bool CanViewChunk(Chunk xiChunk)
        {
            return true;
        }

        // Create an instance of the viewer manager class
        public static Viewer InitialiseViewer(MainForm xiMainForm)
        {
            return new XMLViewer(xiMainForm);
        }

        private Chunk mSubject = null;

        public override void SetSubject(Chunk xiChunk)
        {
            if (mSubject == xiChunk) return;
            if (xiChunk == null)
            {
                mMainForm.XMLTextBox.Text = "";
            }
            else
            {
                XmlSerializer xs = new XmlSerializer(xiChunk.GetType());
                StringWriter sw = new StringWriter();
                xs.Serialize(sw, xiChunk);
                mMainForm.XMLTextBox.Text = sw.ToString();
            }
            mSubject = xiChunk;
        }

        public override System.Windows.Forms.TabPage Tab
        {
            get { return mMainForm.ViewTabXML; }
        }
    }
}
