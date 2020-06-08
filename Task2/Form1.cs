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



//4. use AES encrypting library or whatever, simply algorithm, to encrypt the Zip File
//5. Do it all on single thread at first, then figure out how I can use progress bar and do double threads.Look at my task 1, for example!


namespace Task2 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }
        
    private void button1_Click(object sender, EventArgs e) {
            Encrypt();
        }
 
        private void Encrypt() {

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                string startPath = dialog.FileName;
                string zipPath = @"C:\Users\s033972\Desktop\result.zip";
                try {
                    ZipFile.CreateFromDirectory(startPath, zipPath, CompressionLevel.Fastest, true);
                    AESEncryption crypt = new AESEncryption();
                    crypt.FileEncrypt(zipPath, "123");
                    File.Delete(zipPath);
                } catch (Exception ex) {
                }
            }
        }

        private void Decrypt() {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                string startPath = dialog.FileName;

                try {
                    AESEncryption crypt = new AESEncryption();
                    crypt.FileDecrypt(startPath, @"C:\Users\s033972\Desktop\decrypted.zip", "123");
                } catch (Exception ex) {
                }
            }
        }

    }


}
