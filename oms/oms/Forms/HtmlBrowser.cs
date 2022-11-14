using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace oms.Forms
{
    public partial class HtmlBrowser : Form
    {
        private string htmlFile = string.Empty;
        public HtmlBrowser(string htmlFile)
        {
            InitializeComponent();

            this.htmlFile = htmlFile;

            WebBrowser webBrowser = new WebBrowser();
            this.Controls.Add(webBrowser);
            webBrowser.Dock = DockStyle.Fill;
            webBrowser.Navigate(htmlFile);
        }

        private void HtmlBrowser_Load(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
