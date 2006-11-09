using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

// updates children during scrolling, not just on scroll stop
// see http://brajeshs.blogspot.com/2006/01/net-smooth-scrolling-panel.html

namespace MMEd.Util
{
  public class SmoothScrollingPanel : Panel
  {
    private const int WM_HSCROLL = 0x114;
    private const int WM_VSCROLL = 0x115;

    protected override void WndProc(ref Message m)
    {
      if ((m.Msg == WM_HSCROLL || m.Msg == WM_VSCROLL)
          && (((int)m.WParam & 0xFFFF) == 5))
      {
        // Change SB_THUMBTRACK to SB_THUMBPOSITION
        m.WParam = (IntPtr)(((int)m.WParam & ~0xFFFF)
                   | 4);
      }
      base.WndProc(ref m);
    }
  }
}   

