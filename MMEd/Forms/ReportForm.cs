using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MMEd.Forms
{
  public partial class ReportForm : Form
  {
    public ReportForm()
    {
      InitializeComponent();
    }

    private void CloseButton_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    public static void ShowReport(string xiCaption, string xiReportText)
    {
      ReportForm lReport = new ReportForm();
      lReport.Text = xiCaption;
      lReport.ReportBox.Text = xiReportText;
      lReport.Show();
    }
  }
}