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
using ZedGraph;
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
            if (versionValue.InvokeRequired)
            {
                versionValue.BeginInvoke((MethodInvoker)delegate () { versionValue.Text = data["Version"].Trim(); });
            }
            else
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
            if (infoBox.InvokeRequired)
            {
                infoBox.BeginInvoke((MethodInvoker)delegate () { infoBox.Text = Helper.GetDeviceName(int.Parse(data["Monitor"])); });
            }
            else
            {
                infoBox.Text = Helper.GetDeviceName(int.Parse(data["Monitor"]));
            }

            ShowOrHideColumns(Helper.IdentifySmodeType(data["SMode"]));

            Calculator calc = new Calculator(strCmp);

            double avgSpeed = Math.Round(calc.GetAverageSpeed(1), 2, MidpointRounding.AwayFromZero);
            if (averageSpeed.InvokeRequired)
            {
                averageSpeed.BeginInvoke((MethodInvoker)delegate () { averageSpeed.Text = avgSpeed.ToString(); });
            }
            else
            {
                averageSpeed.Text = avgSpeed.ToString();
            }

            double maxSpeed = calc.GetMaxSpeed(1);
            if (maximumSpeed.InvokeRequired)
            {
                maximumSpeed.BeginInvoke((MethodInvoker)delegate () { maximumSpeed.Text = maxSpeed.ToString(); });
            }
            else
            {
                maximumSpeed.Text = maxSpeed.ToString();
            }
        }

        /// <summary>
        /// Toggle columns from 
        /// </summary>
        private void ShowOrHideColumns(int version)
        {
            
            switch (version)
            {
                case 105:
                    {
                        HideColumns(3);
                        break;
                    }
                case 106:
                    {

                        HideColumns(7);
                        break;
                    }
                default:
                    break;
            }
        }

        private void HideColumns(int startIndex)
        {
            
            foreach (DataGridViewColumn item in dataTable.Columns)
            {
                if(item.Index < startIndex)
                {
                    // we do not want to hide columns before the start index
                    continue;
                }

                if (dataTable.InvokeRequired)
                {
                    dataTable.BeginInvoke((MethodInvoker)delegate () { item.Visible = false; });
                }
                else
                {
                    item.Visible = false;
                }
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
                    Console.WriteLine("Total number of objects are : " + dataTable.Rows.Count);
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

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            switch (e.TabPageIndex)
            {
                case 0:

                    break;
                case 1:
                    if (strCmp != null && strCmp.Count > 0) {
                        ShowGraph();
                    }
                    break;
                default:

                    break;
            }
        }

        public void ShowGraph()
        {
            GraphPane graphPane = zedGraphControl1.GraphPane;
            graphPane.Title = "HeartRateAndSpeed";
            graphPane.XAxis.Title = "Heart Rate";
            graphPane.YAxis.Title = "Speed";
            graphPane.MinBarGap = 100F;

            PointPairList firstPair = new PointPairList();
            PointPairList secondPair = new PointPairList();
            PointPairList thirdPair = new PointPairList();
            PointPairList fourthPair = new PointPairList();
            PointPairList fifthPair = new PointPairList();
            PointPairList sixthPair = new PointPairList();

            for (int i = 0; i < strCmp.Count; i++)
            {
                    firstPair.Add(i, double.Parse(strCmp[i][0]));
                    secondPair.Add(i, double.Parse(strCmp[i][1]));
                    thirdPair.Add(i, double.Parse(strCmp[i][2]));
                    fourthPair.Add(i, double.Parse(strCmp[i][3]));
                    fifthPair.Add(i, double.Parse(strCmp[i][4]));
            }

            LineItem lineCurve = graphPane.AddCurve("Heart Rate", firstPair, Color.Blue, SymbolType.None);
            LineItem lineCurve2 = graphPane.AddCurve("Speed", secondPair, Color.Red, SymbolType.None);
            LineItem lineCurve3 = graphPane.AddCurve("Speed", thirdPair, Color.Green, SymbolType.None);
            LineItem lineCurve4 = graphPane.AddCurve("Speed", fourthPair, Color.Aqua, SymbolType.None);
            LineItem lineCurve5 = graphPane.AddCurve("Speed", fifthPair, Color.Purple, SymbolType.None);

            zedGraphControl1.AxisChange();
            graphPane.Legend.Position = ZedGraph.LegendPos.Bottom;

            zedGraphControl1.Size = new Size(this.ClientRectangle.Width - 20, this.ClientRectangle.Height - 50); ;


        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show("CHANGED!");
        }
    }

}
