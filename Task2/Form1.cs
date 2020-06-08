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
using System.IO.Compression;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Task2 {

    public partial class Form1 : Form {

        static string ByteArrayToString(byte[] arrInput) {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length; i++) {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }

        BackgroundWorker worker = new BackgroundWorker();
        BackgroundWorker worker2 = new BackgroundWorker();

        string selectedDirectory = "";
        string selectedFile = "";
        List<string> hashes = new List<string>();

        public Form1() {
            InitializeComponent();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_Completed;
            worker2.WorkerReportsProgress = true;
            worker2.WorkerSupportsCancellation = true;
            worker2.ProgressChanged += Worker_ProgressChanged;
            worker2.DoWork += Worker_DoWork2;
            worker2.RunWorkerCompleted += Worker_Completed;
        }

        private void button1_Click(object sender, EventArgs e) {
            button1.Enabled = false;
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                selectedDirectory = dialog.FileName;
                worker.RunWorkerAsync();
            }
        }

        private void Encrypt(DoWorkEventArgs e) {
            string zipPath = @"C:\Users\s033972\Desktop\encrypted_result.zip";
            try {
                ZipFile.CreateFromDirectory(selectedDirectory, zipPath, CompressionLevel.Fastest, true);
                AESEncryption crypt = new AESEncryption();
                crypt.FileEncrypt(zipPath, "123", worker, e);
                using (var md5 = MD5.Create()) {
                    using (var stream = File.OpenRead(@"C:\Users\s033972\Desktop\encrypted_result.zip.aes")) {
                        hashes.Add(ByteArrayToString(md5.ComputeHash(stream)));
                    }
                }
                File.Delete(zipPath);
            } catch (Exception ex) {
            }
        }

        private void Decrypt(DoWorkEventArgs e) {
            try {
                AESEncryption crypt = new AESEncryption();
                crypt.FileDecrypt(selectedFile, @"C:\Users\s033972\Desktop\decrypted.zip", "123", worker2, e);
            } catch (Exception ex) {
            }
        }

        private void Worker_Completed(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Cancelled) {
                MessageBox.Show("canceled!");
            }
            else {
                MessageBox.Show("completed");
            }
            button1.Enabled = true;
            button2.Enabled = true;
            progressBar1.Value = 0;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e) {
            Encrypt(e);
        }

        private void Worker_DoWork2(object sender, DoWorkEventArgs e) {
            Decrypt(e);
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            ExtensionMethods.SetProgressNoAnimation(progressBar1, e.ProgressPercentage);
        }

        private void button2_Click(object sender, EventArgs e) {
            //decrypt worker
            button2.Enabled = false;
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                selectedFile = dialog.FileName;

                using (var md5 = MD5.Create()) {
                    using (var stream = File.OpenRead(selectedFile)) {
                        if (hashes.Contains(ByteArrayToString(md5.ComputeHash(stream)))) {
                            worker2.RunWorkerAsync();
                        } else {
                            MessageBox.Show("Incorrect hash");
                            button2.Enabled = true;
                        }
                    }
                }

                
               
            }
        }

        private void progressBar1_Click(object sender, EventArgs e) {

        }

        private void button3_Click(object sender, EventArgs e) {
            //cancel background worker
            
            this.worker.CancelAsync();
            this.worker2.CancelAsync();
        }
    }

    public static class ExtensionMethods {
        /// <summary>
        /// Sets the progress bar value, without using Windows Aero animation
        /// </summary>
        public static void SetProgressNoAnimation(this ProgressBar pb, int value) {
            // Don't redraw if nothing is changing.
            if (value == pb.Value)
                return;

            // To get around this animation, we need to move the progress bar backwards.
            if (value == pb.Maximum) {
                // Special case (can't set value > Maximum).
                pb.Value = value;           // Set the value
                pb.Value = value - 1;       // Move it backwards
            } else {
                pb.Value = value + 1;       // Move past
            }
            pb.Value = value;               // Move to correct value
        }
    }

}