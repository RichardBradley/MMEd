using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MMEd.Util
{
  public partial class OverlaySelector : UserControl
  {
    public OverlaySelector()
    {
      InitializeComponent();
      this.Changed += new EventHandler(this_Changed);
      Checkbox.CheckedChanged += new EventHandler(Checkbox_CheckedChanged);
    }

    public string Label
    {
      get
      {
        return Checkbox.Text;
      }
      set
      {
        Checkbox.Text = value;
      }
    }

    public bool Checked
    {
      get
      {
        return Checkbox.Checked;
      }
      set
      {
        Checkbox.Checked = value;
      }
    }

    public Color CurrentColor
    {
      get
      {
        return mColor;
      }
      set
      {
        mColor = value;
      }
    }

    public Color DefaultColor
    {
      get
      {
        return mDefaultColor;
      }
      set
      {
        mDefaultColor = value;
      }
    }

    private void SetColor(Color xiColor)
    {
      if (Checked)
      {
        CurrentColor = xiColor;
        this.Changed(this, new EventArgs());
      }
      else
      {
        DefaultColor = xiColor;
      }
    }

    private void UpdateColors()
    {
      if (Checked)
      {
        mColor = mDefaultColor;
      }
      else
      {
        mDefaultColor = mColor;
        mColor = Color.Transparent;
      }
    }

    public event EventHandler Changed;

    private void this_Changed(object xiSender, EventArgs xiArgs) { }

    void Checkbox_CheckedChanged(object sender, EventArgs e)
    {
      UpdateColors();
      this.Changed(this, new EventArgs());
    }

    private void LinkWhite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      SetColor(Color.White);
    }

    private void LinkRed_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      SetColor(Color.Red);
    }

    private void LinkBlack_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      SetColor(Color.Black);
    }

    private void LinkGray_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      SetColor(Color.Gray);
    }

    private Color mColor = Color.Transparent;
    private Color mDefaultColor = Color.White;

  }
}
