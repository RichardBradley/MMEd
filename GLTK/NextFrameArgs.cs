using System;

namespace GLTK
{
  class NextFrameArgs : EventArgs
  {
    public NextFrameArgs(bool xiPickMode)
    {
      mPickMode = xiPickMode;
    }

    public bool PickMode
    {
      get { return mPickMode; }
    }

    private bool mPickMode = false;
  }
}