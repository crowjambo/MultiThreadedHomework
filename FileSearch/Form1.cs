using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

/*
 * input
 * The name of a searched item;
   Search initial folder.
 * 
 * output
 * list of folders there the searched item is found.
   search duration.
 */

namespace FileSearch
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Sets the progress bar value, without using Windows Aero animation
        /// </summary>
        public static void SetProgressNoAnimation(this ProgressBar pb, int value)
        {
            // Don't redraw if nothing is changing.
            if (value == pb.Value)
                return;

            // To get around this animation, we need to move the progress bar backwards.
            if (value == pb.Maximum)
            {
                // Special case (can't set value > Maximum).
                pb.Value = value;           // Set the value
                pb.Value = value - 1;       // Move it backwards
            }
            else
            {
                pb.Value = value + 1;       // Move past
            }
            pb.Value = value;               // Move to correct value
        }
    }

    public struct WorkerData
    {
        public string directories;
        public double time;
    }

    public partial class Form1 : Form
    {

        BackgroundWorker worker = new BackgroundWorker();

        public Form1()
        {
            InitializeComponent();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_Completed;
        }

        private void Worker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = (WorkerData)e.Result;
            MessageBox.Show($"{result.directories} - Time: {result.time} ms");
            button1.Enabled = true;
            progressBar1.Value = 0;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var results = SearchFile(directoryTextBox.Text, fileNameTextBox.Text);
            e.Result = results;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ExtensionMethods.SetProgressNoAnimation(progressBar1, e.ProgressPercentage);
        }

        private WorkerData SearchFile(string directory, string fileName)
        {
            WorkerData outputData = new WorkerData();
  
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            
            string rootPath = @directory;
            string[] files = Directory.GetFiles(rootPath, fileName, SearchOption.AllDirectories);
           
            if(files.Length !=0)
            {
                string file = files[0];
                rootPath = Path.GetDirectoryName(file);
            }
            else
            {
                return new WorkerData();
            }
            
            string[] dirs = Directory.GetDirectories(rootPath, "*", SearchOption.TopDirectoryOnly);
            string outputDirectories = String.Empty;

            float totalDirs = dirs.Length;
            float counter = 1f;

            foreach (string dir in dirs)
            {
                int progress = CheckProgressBarInput(((counter / totalDirs) * 100));
                outputDirectories += $"count: {counter} - name: {dir} \n";
                counter++;
                worker.ReportProgress(CheckProgressBarInput(progress));
            }

            watch.Stop();

            outputData.directories = outputDirectories;
            outputData.time = watch.Elapsed.TotalMilliseconds;
            return outputData;
        }

        private int CheckProgressBarInput(float value)
        {
            if (value > 100) return 100;
            else return (int)value;
        }

        private bool CheckTextBoxInput()
        {
            if (directoryTextBox.Text.Length == 0 || fileNameTextBox.Text.Length == 0) return false;
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (CheckTextBoxInput())
            {
                worker.RunWorkerAsync();
                button1.Enabled = false;
            } else
            {
                MessageBox.Show("Bad inputs");
            }

        }

    }
}


