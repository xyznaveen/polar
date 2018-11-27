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
        public static bool newData = false;

        public MainForm()
        {
            InitializeComponent();

            // set Kilometer selected by default
            selectSpeedUnit.SelectedIndex = 0;
            dataTable.Font = label11.Font;
            tabPage2.AutoScroll = true;
            dataTable.Columns[dataTable.ColumnCount - 1].Visible = false;
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
        public double globMaxSpeed = 0;
        public double globAvgSpeed = 0;
        public double globTotalDistanceSpeed = 0;

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
            globAvgSpeed = avgSpeed;
            if (averageSpeed.InvokeRequired)
            {
                averageSpeed.BeginInvoke((MethodInvoker)delegate () { averageSpeed.Text = avgSpeed.ToString(); });
            }
            else
            {
                averageSpeed.Text = avgSpeed.ToString();
            }

            double maxSpeed = calc.GetMaxSpeed(1);
            globMaxSpeed = maxSpeed;
            if (maximumSpeed.InvokeRequired)
            {
                maximumSpeed.BeginInvoke((MethodInvoker)delegate () { maximumSpeed.Text = maxSpeed.ToString(); });
            }
            else
            {
                maximumSpeed.Text = maxSpeed.ToString();
            }

            double avgPower = Calculator.Limit2Two(Calculator.GetAverage(strCmp, 4));
            if (averagePower.InvokeRequired)
            {
                averagePower.BeginInvoke((MethodInvoker)delegate () { averagePower.Text = avgPower.ToString(); });
            }
            else
            {
                averagePower.Text = avgPower.ToString();
            }

            double dblMaxPower = Calculator.GetMax(strCmp, 4);
            if (maxPower.InvokeRequired)
            {
                maxPower.BeginInvoke((MethodInvoker)delegate () { maxPower.Text = dblMaxPower.ToString(); });
            }
            else
            {
                maxPower.Text = dblMaxPower.ToString();
            }

            double dblAverageAltitude = Calculator.Limit2Two(Calculator.GetAverage(strCmp, 3));
            if (averageAltitude.InvokeRequired)
            {
                averageAltitude.BeginInvoke((MethodInvoker)delegate () { averageAltitude.Text = dblAverageAltitude.ToString(); });
            }
            else
            {
                averageAltitude.Text = dblAverageAltitude.ToString();
            }

            double dblMaxAltitude = Calculator.GetMax(strCmp, 3);
            if (maxAltitude.InvokeRequired)
            {
                maxAltitude.BeginInvoke((MethodInvoker)delegate () { maxAltitude.Text = dblMaxAltitude.ToString(); });
            }
            else
            {
                maxAltitude.Text = dblMaxAltitude.ToString();
            }

            double dblMaxHeartRate = Calculator.GetMax(strCmp, 0);
            if (maxHeartRate.InvokeRequired)
            {
                maxHeartRate.BeginInvoke((MethodInvoker)delegate () { maxHeartRate.Text = dblMaxHeartRate.ToString(); });
            }
            else
            {
                maxHeartRate.Text = dblMaxHeartRate.ToString();
            }

            double dblAverageHeartRate = Calculator.Limit2Two(Calculator.GetAverage(strCmp, 0));
            if (averageHeartRate.InvokeRequired)
            {
                averageHeartRate.BeginInvoke((MethodInvoker)delegate () { averageHeartRate.Text = dblAverageHeartRate.ToString(); });
            }
            else
            {
                averageHeartRate.Text = dblAverageHeartRate.ToString();
            }

            double dblMinHeartRate = Calculator.Limit2Two(Calculator.GetMin(strCmp, 0));
            if (minHeartRate.InvokeRequired)
            {
                minHeartRate.BeginInvoke((MethodInvoker)delegate () { minHeartRate.Text = dblMinHeartRate.ToString(); });
            }
            else
            {
                minHeartRate.Text = dblMinHeartRate.ToString();
            }

            double dblTotalDistanceCovered = Calculator.GetSpeed(5, strCmp.Count, Calculator.GetAverage(strCmp, 1));
            globTotalDistanceSpeed = dblTotalDistanceCovered;
            if (totalDistanceCovered.InvokeRequired)
            {
                totalDistanceCovered.BeginInvoke((MethodInvoker)delegate () { totalDistanceCovered.Text = dblTotalDistanceCovered.ToString(); });
            }
            else
            {
                totalDistanceCovered.Text = dblTotalDistanceCovered.ToString();
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
                        //HideColumns(3);
                        break;
                    }
                case 106:
                    {

                        //HideColumns(7);
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
            dataTable.Rows.Add(str[0], Calculator.Str2Double(str[1]), Calculator.Km2Mile(Calculator.Str2Double(str[1])), str[2], str[3], str[4]);
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
                case 2:
                    if (strCmp != null && strCmp.Count > 0)
                    {
                        ShowOne();
                        ShowTwo();
                        ShowThree();
                        ShowFour();
                        ShowFive();
                    }
                    break;
                default:

                    break;
            }
        }

        public void ShowGraph()
        {

            GraphPane graphPane = zedGraphControl1.GraphPane;

            // clear graph to start adding new items
            graphPane.CurveList.Clear();

            graphPane.Title = "Multi Graph";
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
            LineItem lineCurve3 = graphPane.AddCurve("Cadence", thirdPair, Color.Green, SymbolType.None);
            LineItem lineCurve4 = graphPane.AddCurve("Altitude", fourthPair, Color.Aqua, SymbolType.None);
            LineItem lineCurve5 = graphPane.AddCurve("Power", fifthPair, Color.Purple, SymbolType.None);

            zedGraphControl1.AxisChange();
            zedGraphControl1.Size = new Size(500, 500);
            graphPane.Legend.Position = ZedGraph.LegendPos.Bottom;
        }

        public void ShowOne()
        {
            GraphPane graphPane = zedGraphControl2.GraphPane;

            // clear graph to start adding new items
            graphPane.CurveList.Clear();

            graphPane.Title = "Heart Rate";
            graphPane.XAxis.Title = "Time";
            graphPane.YAxis.Title = "Heart Rate";
            graphPane.MinBarGap = 100F;

            PointPairList firstPair = new PointPairList();

            for (int i = 0; i < strCmp.Count; i++)
            {
                firstPair.Add(i, double.Parse(strCmp[i][0]));
            }

            LineItem lineCurve = graphPane.AddCurve("Heart Rate", firstPair, Color.Pink, SymbolType.None);

            zedGraphControl2.AxisChange();
        }

        public void ShowTwo()
        {
            GraphPane graphPane = zedGraphControl3.GraphPane;

            // clear graph to start adding new items
            graphPane.CurveList.Clear();

            graphPane.Title = "Speed";
            graphPane.XAxis.Title = "Time";
            graphPane.YAxis.Title = "Speed";
            graphPane.MinBarGap = 100F;

            PointPairList firstPair = new PointPairList();

            for (int i = 0; i < strCmp.Count; i++)
            {
                firstPair.Add(i, double.Parse(strCmp[i][1]));
            }

            LineItem lineCurve = graphPane.AddCurve("Speed", firstPair, Color.Maroon, SymbolType.None);

            zedGraphControl3.AxisChange();
        }

        public void ShowThree()
        {
            GraphPane graphPane = zedGraphControl4.GraphPane;

            // clear graph to start adding new items
            graphPane.CurveList.Clear();

            graphPane.Title = "Cadence";
            graphPane.XAxis.Title = "Time";
            graphPane.YAxis.Title = "Cadence";
            graphPane.MinBarGap = 100F;

            PointPairList firstPair = new PointPairList();

            for (int i = 0; i < strCmp.Count; i++)
            {
                firstPair.Add(i, double.Parse(strCmp[i][2]));
            }

            LineItem lineCurve = graphPane.AddCurve("Cadence", firstPair, Color.Orange, SymbolType.None);

            zedGraphControl4.AxisChange();
        }

        public void ShowFour()
        {
            GraphPane graphPane = zedGraphControl5.GraphPane;

            // clear graph to start adding new items
            graphPane.CurveList.Clear();

            graphPane.Title = "Altitude";
            graphPane.XAxis.Title = "Time";
            graphPane.YAxis.Title = "Altitude";
            graphPane.MinBarGap = 100F;

            PointPairList firstPair = new PointPairList();

            for (int i = 0; i < strCmp.Count; i++)
            {
                firstPair.Add(i, double.Parse(strCmp[i][3]));
            }

            LineItem lineCurve = graphPane.AddCurve("Altitude", firstPair, Color.Gray, SymbolType.None);

            zedGraphControl5.AxisChange();
        }

        public void ShowFive()
        {
            GraphPane graphPane = zedGraphControl6.GraphPane;

            // clear graph to start adding new items
            graphPane.CurveList.Clear();

            graphPane.Title = "Power";
            graphPane.XAxis.Title = "Time";
            graphPane.YAxis.Title = "Power";
            graphPane.MinBarGap = 100F;

            PointPairList firstPair = new PointPairList();

            for (int i = 0; i < strCmp.Count; i++)
            {
                firstPair.Add(i, double.Parse(strCmp[i][4]));
            }

            LineItem lineCurve = graphPane.AddCurve("Power", firstPair, Color.Red, SymbolType.None);

            zedGraphControl6.AxisChange();
        }

        private void ClearGraphPane()
        {

            GraphPane gp = zedGraphControl1.GraphPane;
        }

        private void selectSpeedUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(selectSpeedUnit.SelectedIndex == 1) {
                if (averageSpeed != null && maximumSpeed !=null)
                {
                    averageSpeed.Text = Calculator.Km2Mile(globAvgSpeed).ToString();
                    maximumSpeed.Text = Calculator.Km2Mile(globMaxSpeed).ToString();
                    totalDistanceCovered.Text = Calculator.Km2Mile(globTotalDistanceSpeed).ToString();
                }

                dataTable.Columns[1].Visible = false;
                dataTable.Columns[2].Visible = true;
                return;
            }

            dataTable.Columns[1].Visible = true;
            dataTable.Columns[2].Visible = false;
            if (averageSpeed != null && maximumSpeed != null)
            {
                averageSpeed.Text = Calculator.Mile2Km(double.Parse(averageSpeed.Text)).ToString();
                maximumSpeed.Text = Calculator.Mile2Km(double.Parse(maximumSpeed.Text)).ToString();
                totalDistanceCovered.Text = Calculator.Mile2Km(double.Parse(totalDistanceCovered.Text)).ToString();
            }
        }
    }

}
