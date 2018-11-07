using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HeartBeatMonitor.Delegates;

namespace HeartBeatMonitor
{
    public partial class MainForm : Form
    {
        private const string OpenFileTitle = "Chose Input File";

        public MainForm()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string fileName = ShowOpenFileDialog();
            if (!String.IsNullOrEmpty(fileName))
            {
                ReadContentsCallback rcc = new ReadContentsCallback(ReadContentsCallbackImpl);
                FileHandler tws = new FileHandler( fileName, rcc);
                Thread t = new Thread(new ThreadStart(tws.FetchHrmData));
                t.Start();
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            // will be called when the Form is maximized
            // and on every resize
            dataTable.Width = this.Width - 15;
        }

        public void ReadContentsCallbackImpl(List<string[]> results)
        {
            if(this.dataTable.InvokeRequired) {

                ReadContentsCallback selfCallback = new ReadContentsCallback(ReadContentsCallbackImpl);
                this.Invoke(selfCallback, new object[] { results });
            } else {
                foreach (var result in results)
                {
                    // this will be modified later based on version of the device
                    this.dataTable.Rows.Add(result[0], result[1], result[2], result[3], result[4]);
                }
            }
        }

        /// <summary>
        /// Opens a file choser dialog which will be used to select the defined file type by the user.
        /// </summary>
        /// <returns> path of the selected file</returns>
        private string ShowOpenFileDialog()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            // we have our own specific files to read
            ofd.Filter = "Realtime Data Dump File| *.hrm";
            ofd.Title = OpenFileTitle;
            ofd.ShowDialog();
            return ofd.FileName;
        }
    }

}
