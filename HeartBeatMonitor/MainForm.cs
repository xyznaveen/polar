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

            // set Kilometer selected by default
            selectSpeedUnit.SelectedIndex = 0;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = ShowOpenFileDialog();
            
            if (!string.IsNullOrEmpty(fileName))
            {

                loadHrmData(fileName);
                loadParameters(fileName);
            }
        }

        /// <summary>
        /// Loads data into table from the file provided
        /// </summary>
        private void loadHrmData(string fileName)
        {
            ReadContentsCallback rcc = new ReadContentsCallback(ReadContentsCallbackImpl);
            FileHandler tws = new FileHandler(fileName, rcc);
            Thread thread = new Thread(new ThreadStart(tws.FetchHrmData));
            thread.Start();
        }

        /// <summary>
        /// Loads paramers from HRM thread in a separate thread.
        /// </summary>
        private void loadParameters(string fileName)
        {
            ConfigurationLoadedCallback clc = new ConfigurationLoadedCallback(ConfigurationLoadedCallbackImpl);
            FileHandler tclc = new FileHandler(fileName, clc);
            Thread thread2 = new Thread(new ThreadStart(tclc.FetchParameters));
            thread2.Start();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            updateControls();
        }

        /// <summary>
        /// Calculates new height and width of components on resize and sets acordingly.
        /// </summary>
        private void updateControls()
        {
            int heightToDeduct = (menuStrip1.Height + 45);
            infoBox.Height = this.Height - heightToDeduct;

            // info box
            infoBoxPanel.Height = infoBox.Height - 40;

            dataPanel.Width = this.Width - infoBoxPanel.Width - 40;
            dataTable.Location = new Point(0, 0);
            dataTable.Width = dataPanel.Width;
            dataTable.Height = dataPanel.Height;
        }

        private List<string[]> strCmp;

        public void ConfigurationLoadedCallbackImpl(Dictionary<string, string> data)
        {
            foreach (var pair in data)
            {
                Console.WriteLine(pair.Key + " = " + pair.Value);
            }
            
            if(versionValue.InvokeRequired)
            {
                versionValue.BeginInvoke((MethodInvoker)delegate () { versionValue.Text = data["Version"].Trim(); });
            } else
            {
                versionValue.Text = data["Version"].Trim(); 
            }
            if (dateValue.InvokeRequired)
            {
                dateValue.BeginInvoke((MethodInvoker)delegate () { dateValue.Text = data["Date"].Trim(); });
            }
            else
            {
                dateValue.Text = data["Date"].Trim();
            }
            if (startTimeValue.InvokeRequired) 
            {
                startTimeValue.BeginInvoke((MethodInvoker)delegate () { startTimeValue.Text = data["StartTime"].Trim(); });
            }
            else
            {
                startTimeValue.Text = data["StartTime"].Trim();
            }
            if (intervalValue.InvokeRequired)
            {
                intervalValue.BeginInvoke((MethodInvoker)delegate () { intervalValue.Text = data["Interval"].Trim(); });
            }
            else
            {
                intervalValue.Text = data["Interval"].Trim();
            }
            if (weightValue.InvokeRequired) 
            {
                weightValue.BeginInvoke((MethodInvoker)delegate () { weightValue.Text = data["Weight"].Trim(); });
            }
            else
            {
                weightValue.Text = data["Weight"].Trim(); 
            }
            if (lengthValue.InvokeRequired) 
            {
                lengthValue.BeginInvoke((MethodInvoker)delegate () { lengthValue.Text = data["Length"].Trim(); });
            }
            else
            {
                lengthValue.Text = data["Length"].Trim();
            }
            if (dataTable.InvokeRequired)
            {
                dataTable.BeginInvoke((MethodInvoker)delegate () { dataTable.Columns[dataTable.Columns.Count - 1].Visible = false; });
            }
            else
            {
                dataTable.Columns[dataTable.Columns.Count - 1].Visible = false;
            }
            if (infoBox.InvokeRequired)
            {                
                infoBox.BeginInvoke((MethodInvoker)delegate () { infoBox.Text = Helper.GetDeviceName(int.Parse(data["Monitor"])); });
            }
            else
            {
                infoBox.Text = Helper.GetDeviceName(int.Parse(data["Monitor"]));
            }
        }

        public void ReadContentsCallbackImpl(List<string[]> results)
        {
            if (dataTable.InvokeRequired)
            {

                ReadContentsCallback selfCallback = new ReadContentsCallback(ReadContentsCallbackImpl);
                Invoke(selfCallback, new object[] { results });
            }
            else
            {
                if (results.Count > 0)
                {
                    Console.WriteLine("TOtal number of objects are : " + dataTable.Rows.Count);
                    dataTable.Rows.Clear();
                    strCmp = results;
                    fetchDataBackground.WorkerReportsProgress = true;
                    fetchDataBackground.RunWorkerAsync();
                }
            }
        }

        /// <summary>
        /// Opens file choser dialog to select realtime data dump.
        /// </summary>
        /// <returns> path of the selected file</returns>
        private string ShowOpenFileDialog()
        {
            OpenFileDialog chooser = new OpenFileDialog();
            // we have our own specific files to read
            chooser.Filter = "Realtime Data Dump File| *.hrm";
            chooser.Title = OpenFileTitle;
            chooser.ShowHelp = true;
            chooser.ShowDialog();
            return chooser.FileName;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void dataPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void fetchDataBackground_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker work = sender as BackgroundWorker;
            int count = 0;
            foreach (string[] str in strCmp)
            {
                work.ReportProgress(count, str);
                ++count;
            }
        }

        private void fetchDataBackground_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string[] str = e.UserState as string[];
            dataTable.Rows.Add(str[0], str[1], str[2], str[3], str[4]);
        }
    }

}
