using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kiselov_HW_ProgressBar_FileCopy
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Field for creation of object which will operate with files 
        /// </summary>
        readonly FileOperator fileOperator;

        // variables for demonstration of speed pf copying process 
        // --------------------------
        long lCopiedPrev;

        long lCopiedCurr;

        int iSpeedDevider;

        string strSpeed;

        int iTimeLoop; 
        //----------------------------
        public Form1()
        {
            InitializeComponent();
            // 100 - amount of percents 
            progressBar1.Maximum = 100; 
            fileOperator = new FileOperator();
            lCopiedCurr = 0; 
            lCopiedPrev = 0;
            iSpeedDevider = 1;
            strSpeed = string.Empty;
            // amount of passed timers loops 
            iTimeLoop = 0; 
            // addition of event 
            // Each event will send information about current state of FileOperator object - 
            // Amount of read bytes  
            fileOperator.ReadProcess += ShowReadProcess;
            // interval for changes =1 sec
            timer1.Interval = 1000; 
        }

        /// <summary>
        /// Method describes tree structure with folders to point on the current file 
        /// Adress of this file will be described in textbox1 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            FileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = fileDialog.FileName; 
            }
        }

        /// <summary>
        /// Method which receives the event of button "Browse..." 
        /// Sets the path to folder - new folder for copied file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // chtcking textbox1 for its empty value 
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    throw new Exception("The textbox with path to file is empty");
                } 

                FileInfo fileInfoBase = new FileInfo(textBox1.Text);
                // if pointed file doesn't exist - exception 
                if (!fileInfoBase.Exists)
                {
                    throw new Exception("File with such adress does not exist");
                }
                // temparary variable - name of file with extension 
                string strFileName = fileInfoBase.Name;

                // method opens the dialog window with right to choose necessary directory 
                FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
                DialogResult result = folderBrowser.ShowDialog();
                if (!string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
                {
                    textBox2.Text = folderBrowser.SelectedPath;
                }
                
                // method creates object FileInfo for checking and comparing with existing files 
                FileInfo fileInfoCopyTo = new FileInfo(textBox2.Text);
                
                // method will throw new exception if new path is empty  
                if (string.IsNullOrEmpty(textBox2.Text))
                {
                    throw new Exception("The path to copying file is empty");
                }

                // method will add name of base file by default to the path of copying file  
                // if it doesn't contain such 
                if (string.IsNullOrEmpty(fileInfoCopyTo.Name))
                {
                    textBox2.Text += fileInfoBase.Name; 
                }

                fileInfoCopyTo = new FileInfo(textBox2.Text);
                // method will throw new exception if such file already exists 
                if (fileInfoCopyTo.Exists)
                {
                    throw new Exception("File with such name already exists");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                FileInfo fileInfoCopyTo = new FileInfo(textBox2.Text);
                // method will throw new exception if such file already exists 
                if (fileInfoCopyTo.Exists)
                {
                    throw new Exception("File with such name already exists");
                }

                if (!radioButton1.Checked && !radioButton2.Checked)
                {
                    throw new Exception("Please choose the type of copying speed");
                }

                timer1.Tick+=timer1_Tick;
                timer1.Start();

                Func<string, string, bool> func = fileOperator.FileCopyTo;
                IAsyncResult iAsyncResult = func.BeginInvoke(textBox1.Text, textBox2.Text, null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Each event will call this method 
        /// Current method will describe the state of progressbar using percents 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="progressEvent"></param>
        public void ShowReadProcess(object sender, ProgressEventArgs progressEvent)
        {
            if (sender is FileOperator)
            {
                if (progressBar1.Value < progressBar1.Maximum)
                {
                    progressBar1.Step = progressEvent.iCount-progressBar1.Value;
                    progressBar1.PerformStep();
                    if (progressBar1.Value == progressBar1.Maximum)
                    {
                        Thread.Sleep(1000);
                        MessageBox.Show("Completed");
                    }
                }
            }
        }

        /// <summary>
        /// Each second this method will give an indormation into speed bars
        /// About current and average speed of copying process 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            iTimeLoop++; 
            lCopiedCurr = fileOperator.CopyCounter;
            double dCopiedTotal = (double)lCopiedCurr/iSpeedDevider/iTimeLoop; 
            double dCopiedCurr = 0.00;
            dCopiedCurr = (double) (lCopiedCurr - lCopiedPrev)/iSpeedDevider;
            if (dCopiedCurr > 0)
            {
                textBox3.Text = string.Format(string.Format("{0:0.00}", dCopiedCurr) + " " + strSpeed);
                textBox4.Text = string.Format(string.Format("{0:0.00}", dCopiedTotal) + " " + strSpeed);
            }

            lCopiedPrev = lCopiedCurr;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton) sender;
            if (rb.Equals(radioButton1))
            {
                iSpeedDevider = 1048576;
                strSpeed = "MB/s"; 
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Equals(radioButton2))
            {
                iSpeedDevider = 1;
                strSpeed = "b/s"; 
            }
        }
    }
}
