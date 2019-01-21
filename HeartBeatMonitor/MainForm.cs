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

                // load file to view the details
                loadHrmData(fileName, ReadContentsCallbackImpl);

                // load parameters of the file
                loadParameters(fileName);
            }
        }

        /// <summary>
        /// Loads data from the specified file and callbacks the passed implementation
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="impl"></param>
        private void loadHrmData(string fileName, ReadContentsCallback impl)
        {
            ReadContentsCallback rcc = new ReadContentsCallback(impl);
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

        private List<string[]> dataFromFile;
        private List<string[]> dataFromFileOriginal;
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

            Calculator calc = new Calculator(dataFromFile);

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

            double avgPower = Calculator.Limit2Two(Calculator.GetAverage(dataFromFile, 4));
            if (averagePower.InvokeRequired)
            {
                averagePower.BeginInvoke((MethodInvoker)delegate () { averagePower.Text = avgPower.ToString(); });
            }
            else
            {
                averagePower.Text = avgPower.ToString();
            }

            double dblMaxPower = Calculator.GetMax(dataFromFile, 4);
            if (maxPower.InvokeRequired)
            {
                maxPower.BeginInvoke((MethodInvoker)delegate () { maxPower.Text = dblMaxPower.ToString(); });
            }
            else
            {
                maxPower.Text = dblMaxPower.ToString();
            }

            double dblAverageAltitude = Calculator.Limit2Two(Calculator.GetAverage(dataFromFile, 3));
            if (averageAltitude.InvokeRequired)
            {
                averageAltitude.BeginInvoke((MethodInvoker)delegate () { averageAltitude.Text = dblAverageAltitude.ToString(); });
            }
            else
            {
                averageAltitude.Text = dblAverageAltitude.ToString();
            }

            double dblMaxAltitude = Calculator.GetMax(dataFromFile, 3);
            if (maxAltitude.InvokeRequired)
            {
                maxAltitude.BeginInvoke((MethodInvoker)delegate () { maxAltitude.Text = dblMaxAltitude.ToString(); });
            }
            else
            {
                maxAltitude.Text = dblMaxAltitude.ToString();
            }

            double dblMaxHeartRate = Calculator.GetMax(dataFromFile, 0);
            if (maxHeartRate.InvokeRequired)
            {
                maxHeartRate.BeginInvoke((MethodInvoker)delegate () { maxHeartRate.Text = dblMaxHeartRate.ToString(); });
            }
            else
            {
                maxHeartRate.Text = dblMaxHeartRate.ToString();
            }

            double dblAverageHeartRate = Calculator.Limit2Two(Calculator.GetAverage(dataFromFile, 0));
            if (averageHeartRate.InvokeRequired)
            {
                averageHeartRate.BeginInvoke((MethodInvoker)delegate () { averageHeartRate.Text = dblAverageHeartRate.ToString(); });
            }
            else
            {
                averageHeartRate.Text = dblAverageHeartRate.ToString();
            }

            double dblMinHeartRate = Calculator.Limit2Two(Calculator.GetMin(dataFromFile, 0));
            if (minHeartRate.InvokeRequired)
            {
                minHeartRate.BeginInvoke((MethodInvoker)delegate () { minHeartRate.Text = dblMinHeartRate.ToString(); });
            }
            else
            {
                minHeartRate.Text = dblMinHeartRate.ToString();
            }

            double dblTotalDistanceCovered = Calculator.GetSpeed(5, dataFromFile.Count, Calculator.GetAverage(dataFromFile, 1));
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

        /// <summary>
        /// Hide the column at specified index.
        /// </summary>
        /// <param name="startIndex">the column index which requires hiding.</param>
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
                // only if the resulting list is greater than 0
                if (results.Count > 0)
                {
                    Console.WriteLine("Total number of objects are : " + dataTable.Rows.Count);
                    dataTable.Rows.Clear();
                    dataFromFile = results;
                    dataFromFileOriginal = results;
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
            foreach (string[] str in dataFromFile)
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
                    if (dataFromFile != null && dataFromFile.Count > 0) {
                        ShowGraph();
                    }
                    break;
                case 2:
                    if (dataFromFile != null && dataFromFile.Count > 0)
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

        /// <summary>
        /// Show multiple graph in a single place.
        /// </summary>
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
            
            for (int i = 0; i < dataFromFile.Count; i++)
            {
                firstPair.Add(i, double.Parse(dataFromFile[i][0]));
                secondPair.Add(i, double.Parse(dataFromFile[i][1]));
                thirdPair.Add(i, double.Parse(dataFromFile[i][2]));
                fourthPair.Add(i, double.Parse(dataFromFile[i][3]));
                fifthPair.Add(i, double.Parse(dataFromFile[i][4]));
            }

            LineItem lineCurve = graphPane.AddCurve("Heart Rate", firstPair, Color.Blue, SymbolType.None);
            LineItem lineCurve2 = graphPane.AddCurve("Speed", secondPair, Color.Red, SymbolType.None);
            LineItem lineCurve3 = graphPane.AddCurve("Cadence", thirdPair, Color.Green, SymbolType.None);
            LineItem lineCurve4 = graphPane.AddCurve("Altitude", fourthPair, Color.Aqua, SymbolType.None);
            LineItem lineCurve5 = graphPane.AddCurve("Power", fifthPair, Color.Purple, SymbolType.None);
            PointF centrePoint = new System.Drawing.PointF();
            zedGraphControl1.Size = new Size(500, 500);
            graphPane.Legend.Position = ZedGraph.LegendPos.Bottom;
            zedGraphControl1.AxisChange();
        }

        /// <summary>
        /// Show first single graph.
        /// </summary>
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

            for (int i = 0; i < dataFromFile.Count; i++)
            {
                firstPair.Add(i, double.Parse(dataFromFile[i][0]));
            }

            LineItem lineCurve = graphPane.AddCurve("Heart Rate", firstPair, Color.Pink, SymbolType.None);

            zedGraphControl2.AxisChange();
        }

        /// <summary>
        /// Show second single graph.
        /// </summary>
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

            for (int i = 0; i < dataFromFile.Count; i++)
            {
                firstPair.Add(i, double.Parse(dataFromFile[i][1]));
            }

            LineItem lineCurve = graphPane.AddCurve("Speed", firstPair, Color.Maroon, SymbolType.None);

            zedGraphControl3.AxisChange();
        }

        /// <summary>
        /// Show third single graph.
        /// </summary>
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

            for (int i = 0; i < dataFromFile.Count; i++)
            {
                firstPair.Add(i, double.Parse(dataFromFile[i][2]));
            }

            LineItem lineCurve = graphPane.AddCurve("Cadence", firstPair, Color.Orange, SymbolType.None);

            zedGraphControl4.AxisChange();
        }

        /// <summary>
        /// Show fourth single graph.
        /// </summary>
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

            for (int i = 0; i < dataFromFile.Count; i++)
            {
                firstPair.Add(i, double.Parse(dataFromFile[i][3]));
            }

            LineItem lineCurve = graphPane.AddCurve("Altitude", firstPair, Color.Gray, SymbolType.None);

            zedGraphControl5.AxisChange();
        }

        /// <summary>
        /// Show fifth single graph.
        /// </summary>
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

            for (int i = 0; i < dataFromFile.Count; i++)
            {
                firstPair.Add(i, double.Parse(dataFromFile[i][4]));
            }

            LineItem lineCurve = graphPane.AddCurve("Power", firstPair, Color.Red, SymbolType.None);

            zedGraphControl6.AxisChange();
        }

        /// <summary>
        /// Clear previous data from the grid.
        /// </summary>
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

        private void button1_Click(object sender, EventArgs e)
        {
            string fileName = ShowOpenFileDialog();
            label21.Text = fileName;

            // load first comparison data file
            loadHrmData(fileName, OpenOneCallbackImpl);
        }

        /// <summary>
        /// Callback method for the first data grid of the comparison.
        /// </summary>
        /// <param name="results">the list of HRM data file.</param>
        public void OpenOneCallbackImpl(List<string[]> results)
        {
            if (dataGridCompOne.InvokeRequired)
            {
                ReadContentsCallback selfCallback = new ReadContentsCallback(OpenOneCallbackImpl);
                Invoke(selfCallback, new object[] { results });
            }
            else
            {
                if (results.Count > 0)
                {
                    Console.WriteLine("Total number of objects are : " + dataGridCompOne.Rows.Count);
                    dataGridCompOne.Rows.Clear();
                    dataFromFile = results;
                    backgroundWorker1.WorkerReportsProgress = true;
                    backgroundWorker1.RunWorkerAsync();
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker work = sender as BackgroundWorker;
            int count = 0;
            foreach (string[] str in dataFromFile)
            {
                work.ReportProgress(count, str);
                ++count;
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string[] str = e.UserState as string[];
            dataGridCompOne.Rows.Add(str[0], Calculator.Str2Double(str[1]), Calculator.Km2Mile(Calculator.Str2Double(str[1])), str[2], str[3], str[4]);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string fileName = ShowOpenFileDialog();
            label22.Text = fileName;

            loadHrmData(fileName, OpenTwoCallbackImpl);
        }

        /// <summary>
        /// Callback method for the second data grid of the comparison.
        /// </summary>
        /// <param name="results">the list of HRM data file.</param>
        public void OpenTwoCallbackImpl(List<string[]> results)
        {
            if (dataGridCompOne.InvokeRequired)
            {
                ReadContentsCallback selfCallback = new ReadContentsCallback(OpenTwoCallbackImpl);
                Invoke(selfCallback, new object[] { results });
            }
            else
            {
                if (results.Count > 0)
                {
                    Console.WriteLine("Total number of objects are : " + dataGridCompTwo.Rows.Count);
                    dataGridCompTwo.Rows.Clear();
                    dataFromFile = results;
                    backgroundWorker2.WorkerReportsProgress = true;
                    backgroundWorker2.RunWorkerAsync();
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker work = sender as BackgroundWorker;
            int count = 0;
            foreach (string[] str in dataFromFile)
            {
                work.ReportProgress(count, str);
                ++count;
            }
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string[] str = e.UserState as string[];
            dataGridCompTwo.Rows.Add(str[0], Calculator.Str2Double(str[1]), Calculator.Km2Mile(Calculator.Str2Double(str[1])), str[2], str[3], str[4]);
        }

        private void dataGridCompTwo_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            // if data is not loaded or loaded only in one grid stop execution 
            // and show a message
            if(dataGridCompOne.RowCount < 2 || dataGridCompTwo.RowCount < 2)
            {
                MessageBox.Show("Two files must be loaded for comparison.\nPlease open two files and try again.", "STOP !");
                return;
            }

            string result = CompareHrmData(dataGridCompOne, dataGridCompTwo);
            MessageBox.Show(result, "Comparison Difference With Respect To Clicked DataGrid");
        }

        private void dataGridCompOne_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // if data is not loaded or loaded only in one grid stop execution 
            // and show a message
            if (dataGridCompOne.RowCount < 2 || dataGridCompTwo.RowCount < 2)
            {
                MessageBox.Show("Two files must be loaded for comparison.\nPlease open two files and try again.", "STOP !");
                return;
            }

            string result = CompareHrmData(dataGridCompTwo, dataGridCompOne);
            MessageBox.Show(result, "Comparison Difference With Respect To Clicked DataGrid");
        }

        /// <summary>
        /// Compare and show the data of two different datagrids.
        /// </summary>
        /// <param name="dataGridCompOne">first data grid.</param>
        /// <param name="dataGridCompTwo">second data grid.</param>
        /// <returns>the compared data, the original data and the + / - difference.</returns>
        private string CompareHrmData(DataGridView dataGridCompOne, DataGridView dataGridCompTwo)
        {
            // get both grids column count
            int totalColumnsGridTwo = dataGridCompTwo.ColumnCount;
            int totalColumnsGridOne = dataGridCompOne.ColumnCount;

            // user selected row index
            int selectedRowIndex = dataGridCompTwo.SelectedCells[0].RowIndex;

            // get selected rows of both grids
            DataGridViewRow selectedRowGridOne = dataGridCompOne.Rows[selectedRowIndex];
            DataGridViewRow selectedRowGridTwo = dataGridCompTwo.Rows[selectedRowIndex];

            // the resulting string
            string result = "";

            // column names header to show information
            string colNames = "Hrt Rt.\tSpd(Km)\tSpd(Mi)\tCadence\tAltitude\tPower\n\n";

            // store the difference between two selected rows
            string diff = "\n";

            // add header to result at first
            result += colNames;

            // provide space to the input
            string delimiter = "\t";

            for (int i = 0; i < totalColumnsGridTwo; i++)
            {
                string temp = Convert.ToString(selectedRowGridTwo.Cells[i].Value);
                result += temp;

                if (!string.IsNullOrEmpty(temp))
                {
                    // append data because there is some value
                    result += delimiter;
                    double num = 0.0;
                }
                else if (i == 0)
                {
                    // because the first column heading is extra large we need multiple spacing
                    result += "\t";
                }
                else
                {
                    // extra tab is added at the end, remove it for empty column
                    result = result.Substring(0, result.Length - delimiter.Length);
                }
            }

            result += "\n";

            for (int i = 0; i < totalColumnsGridOne; i++)
            {
                string temp = Convert.ToString(selectedRowGridOne.Cells[i].Value);
                result += temp;

                if (!string.IsNullOrEmpty(temp))
                {
                    // append data because there is some value
                    result += delimiter;
                }
                else if (i == 0)
                {
                    // because the first column heading is extra large we need multiple spacing
                    result += "\t";
                }
                else
                {
                    // extra tab is added at the end, remove it for empty column
                    result = result.Substring(0, result.Length - delimiter.Length);
                }
            }

            diff = CalculateDifference(result);

            return result + diff;
        }

        /// <summary>
        /// Calculate and return the difference of two input data grids.
        /// </summary>
        /// <param name="input">the actual input data of comparison data.</param>
        /// <returns>string containing the difference of the two input grids.</returns>
        private string CalculateDifference(string input)
        {
            string result = "\n\n";
            string delimiter = "\t";

            string[] arrayInputNewLine = input.Split('\n');

            string[] one = arrayInputNewLine[2].Split('\t');
            string[] two = arrayInputNewLine[3].Split('\t');

            for (int i = 0; i < one.Length; i++)
            {

                double dblOne = 0.0;
                double dblTwo = 0.0;
                double.TryParse(one[i], out dblOne);
                double.TryParse(two[i], out dblTwo);

                result += Convert.ToString(dblOne - dblTwo);
                result += '\t';
            }

            return result;
        }

        private void chunkDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            long chunks = 0;
            long.TryParse(textBox.Text, out chunks);

            if(chunks < 2)
            {
                textBox.Text = "2";
            }

            if (chunks > 7)
            {
                textBox.Text = "7";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            // get the total number of chunk input
            int numberOfChunks = 1;
            int.TryParse(txtNumberOfChunks.Text, out numberOfChunks);

            List<List<string[]>> totalData = BreakDataIntoChunks(numberOfChunks, dataFromFileOriginal);

            string average = CalculateAverageOfChunks(totalData);

            this.Cursor = Cursors.Arrow;
            MessageBox.Show(average);
        }

        /// <summary>
        /// Calculates and retruns the average of the chunked data.
        /// </summary>
        /// <param name="totalData">the chunk of data whose averages should be calculated.</param>
        /// <returns></returns>
        private string CalculateAverageOfChunks(List<List<string[]>> totalData)
        {
            string result = "";
            int chunkCounter = 1;
            int totalChunks = totalData.Count;

            foreach(List<string[]> strList in totalData)
            {
                result += ("Average For Chunk #" + chunkCounter + "\n");
                // heart rate, speed, cadence, altitude, power
                double heartRate = 0;
                double speed = 0;
                double cadence = 0;
                double altitude = 0;
                double power = 0;
                
                // calculating average now
                heartRate = Calculator.GetAverage(strList, 0);
                speed = Calculator.GetAverage(strList, 1);
                cadence = Calculator.GetAverage(strList, 2);
                altitude = Calculator.GetAverage(strList, 3);
                power = Calculator.GetAverage(strList, 4);
                
                result += "Average Heart Rate : " + heartRate + "\n";
                result += "Average Speed : " + speed + "\n";
                result += "Average Cadence : " + cadence + "\n";
                result += "Average Altitude : " + altitude + "\n";
                result += "Average Power : " + power + "\n\n\n";

                // next chunk
                chunkCounter++;
            }

            return result;
        }

        /// <summary>
        /// Break the passed data into specified number of chunks and return it as list of chunks.
        /// </summary>
        /// <param name="numberOfChunks">expected number of chunks for the data</param>
        /// <param name="data">the actual data which requires chunking</param>
        /// <returns></returns>
        private List<List<string[]>> BreakDataIntoChunks(int numberOfChunks, List<string[]> data)
        {
            int sizeOfData = data.Count;
            // this number of items will be placed in a chunk
            int splitAtEvery = sizeOfData / numberOfChunks;

            List<List<string[]>> comparisonData = new List<List<string[]>>();

            int endAt = 0;

            // iterate over number of times
            for (int i = 0; i < numberOfChunks; i++)
            {
                List<string[]> temp = new List<string[]>();

                // increase the amount of data to be retrieved from chunk
                endAt = endAt + splitAtEvery;

                for (int j = 0; j < endAt; j++)
                {
                    // get the values upto the specified index
                    temp.Add(data[0]);
                }

                // add the value to the file
                comparisonData.Add(temp);
            }

            return comparisonData;
        }

        private int singleChunkStartIndex = 0;
        private int singleChunkEndIndex = 1;

        private void button4_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rows = dataTable.SelectedRows;

            singleChunkEndIndex = singleChunkStartIndex + rows.Count - 1;

            List<string[]> requiredData = new List<string[]>();

            for (int dataCounter = 0; dataCounter < dataFromFileOriginal.Count; dataCounter++)
            {
                // get data only within specified index
                if(dataCounter >= singleChunkStartIndex && dataCounter <= singleChunkEndIndex)
                {
                    requiredData.Add(dataFromFileOriginal[dataCounter]);
                }
            }
            int numberOfChunks = 1; // the chunk is always 1
            List<List<string[]>> totalData = BreakDataIntoChunks(numberOfChunks, requiredData);
            string resultData = CalculateAverageOfChunks(totalData);
            MessageBox.Show(resultData);
        }

        private void dataTable_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // set indexing from
            singleChunkStartIndex = e.RowIndex;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            double normalizedPowerValue = PowerCalc.GetNormalizedPower(dataFromFileOriginal, -1);

            MessageBox.Show("Normalized power of the :: " + normalizedPowerValue);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            double ftp = PowerCalc.GetFtp(dataFromFileOriginal, -1);

            string msg = String.Format("Functional Threshhold Power is {0} watts", ftp);

            MessageBox.Show(msg);
        }
    }
}
