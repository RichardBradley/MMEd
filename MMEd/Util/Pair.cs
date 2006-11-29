using System;
using System.Collections.Generic;
using System.Text;

// I'm surprised there's not one of these in the libraries somewhere...

namespace MMEd.Util
{
  public class Pair<LeftType,RightType>
  {
    public Pair() { }
    public Pair(LeftType xiLeft, RightType xiRight)
    {
      Left = xiLeft;
      Right = xiRight;
    }

    public LeftType Left
    {
      get { return mLeft; }
      set { mLeft = value; }
    }

    public RightType Right
    {
      get { return mRight; }
      set { mRight = value; }
    }

    private LeftType mLeft;
    private RightType mRight;
  }
}
