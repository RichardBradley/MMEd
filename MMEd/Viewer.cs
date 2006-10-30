using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using TabPage = System.Windows.Forms.TabPage;

// A class which can display and / or edit a Chunk
//
// This should probably be implemented as a control, but for
// now it's going to be hardcoded to some controls on the main form
// since that's much easier.

namespace MMEd
{
    public abstract class Viewer
    {
        protected MainForm mMainForm;

        // there should only be one Viewer of each type
        protected Viewer(MainForm xiMainForm)
        {
            mMainForm = xiMainForm;
            if (mViewers.Contains(this.GetType())) 
            {
                throw new Exception("Only one viewer of each name should be instantiated");
            }
            mViewers.Add(this.GetType(), this);
        }

         private static ListDictionary mViewers = new ListDictionary();

        public static Viewer GetInstance(Type xiType)
        {
            return (Viewer)mViewers[xiType];
        }

        public static System.Collections.IEnumerable GetViewers()
        {
            return mViewers.Values;
        }

        public abstract bool CanViewChunk(Chunk xiChunk);

        /// <summary>
        ///  Instructs the viewer to display the given object,
        ///  or to free up resources, if appropriate, if the argument is null
        /// </summary>
        public abstract void SetSubject(Chunk xiChunk);

        public void ClearSubject() { SetSubject(null); }

        public abstract TabPage Tab
        {
            get;
        }

        //public abstract static Viewer InitialiseViewer(MainForm xiMainForm);
    }
}
