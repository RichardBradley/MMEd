using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MMEd.Util
{
  public class UserPaintDoubleBufferedPanel : Panel
  {
    public UserPaintDoubleBufferedPanel()
    {
       SetStyle(ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
    }
  }
}
