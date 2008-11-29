using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MMEd.Util
{
  public partial class LabelledSlider : UserControl
  {
    public LabelledSlider()
    {
      InitializeComponent();
    }

    public int Minimum
    {
      get { return TrackBar.Minimum; }
      set { TrackBar.Minimum = value; }
    }

    public int Maximum
    {
      get { return TrackBar.Maximum; }
      set { TrackBar.Maximum = value; }
    }

    public int Value
    {
      get { return TrackBar.Value; }
      set { TrackBar.Value = value; }
    }

    public int LargeChange
    {
      get { return TrackBar.LargeChange; }
      set { TrackBar.LargeChange = value; }
    }

    public int SmallChange
    {
      get { return TrackBar.SmallChange; }
      set { TrackBar.SmallChange = value; }
    }

    public string MinimumLabel
    {
      get { return LabelMin.Text; }
      set { LabelMin.Text = value; }
    }

    public string MaximumLabel
    {
      get { return LabelMax.Text; }
      set { LabelMax.Text = value; }
    }

    public event EventHandler ValueChanged
    {
      add { TrackBar.ValueChanged += value; }
      remove { TrackBar.ValueChanged -= value; }
    }
  }
}
