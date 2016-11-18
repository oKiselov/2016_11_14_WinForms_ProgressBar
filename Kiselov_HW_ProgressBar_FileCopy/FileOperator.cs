using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kiselov_HW_ProgressBar_FileCopy
{
    class FileOperator
    {
        // events 
        public delegate void ProgressDelegte(object sender, ProgressEventArgs progressEvent);

        public event ProgressDelegte ReadProcess; 

        /// <summary>
        /// field for safe multithread operations  
        /// </summary>
        private object threadLock = new object();

        /// <summary>
        /// Field and property for size of read bytes 
        /// </summary>
        private long iCounterCopy;

        public long CopyCounter
        {
            get
            {
                lock (threadLock)
                {
                    return iCounterCopy;
                }
            }
            private set
            {
                lock (threadLock)
                {
                    iCounterCopy = value; 
                }
            }
        }

        /// <summary>
        /// Field and property for file size 
        /// </summary>
        private long iCounterSizeOfFile;

        public long FileSize
        {
            get
            {
                lock (threadLock)
                {
                    return iCounterSizeOfFile;
                }
            }
            private set
            {
                lock (threadLock)
                {
                    iCounterSizeOfFile = value;
                }
            }
        }

        /// <summary>
        /// Field and property for amount (in percent) of read bytes 
        /// </summary>
        private int lPercent;

        public int CopyCounterPercent
        {
            get
            {
                lock (threadLock)
                {
                    return lPercent; 
                }
            }
            private set
            {
                lock (threadLock)
                {
                    lPercent = value;
                }
            }
        }

        public FileOperator()
        {
            FileSize = 0;
            CopyCounter = 0;
            CopyCounterPercent = 0; 
        }

        /// <summary>
        /// Method for increasion of field with percent 
        /// of read bytes 
        /// </summary>
        /// <returns></returns>
        private bool IncreaseCopyCounterPercent()
        {
            bool bRet = false; 
            double lPercentProcess = (double)CopyCounter/FileSize*100;
            if ((lPercentProcess - CopyCounterPercent) >=5)
            {
                lPercent += 5;
                bRet = true; 
            }
            return bRet; 
        }

        public bool FileCopyTo(string strPathFrom, string strPathTo)
        {
            try
            {
                bool bRet = false; 
                using (FileStream fs = new FileStream(strPathFrom, FileMode.Open, FileAccess.Read))
                {
                    FileSize = fs.Length;
                    byte[] arrBytes = new byte[(int)SizeCluster.ThirtyTwo];
                    CopyCounter = 0;
                    while (CopyCounter < FileSize)
                    {
                        int iRead = fs.Read(arrBytes, 0, arrBytes.Length);
                        if (iRead == 0)
                        {
                            break;
                        }

                        if (!CopyIntoCurrentFile(arrBytes, strPathTo))
                        {
                            throw new Exception("Error occurred while file was being copied");
                        }
                        CopyCounter += iRead;
                        // addition of event 
                        // Each event will send information about current state of FileOperator object - 
                        // Amount of read bytes  
                        if (IncreaseCopyCounterPercent())
                        {
                            ReadProcess(this, new ProgressEventArgs(CopyCounterPercent));
                        }
                    }
                    if (CopyCounter == FileSize)
                    {
                        bRet = true; 
                    }
                }
                return bRet;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private bool CopyIntoCurrentFile(byte[] arrBytes, string strPathTo)
        {
            try
            {
                FileInfo fi = new FileInfo(strPathTo);
                // variables for demonstation of difference between states of stream 
                // before and after copy process 
                long iSizeOfStreamBefore, iSizeOfStreamAfter; 
                using (FileStream fs = new FileStream(strPathTo, FileMode.Append, FileAccess.Write))
                {
                    iSizeOfStreamBefore = fi.Length; 
                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write(arrBytes);
                }
                fi = new FileInfo(strPathTo);
                iSizeOfStreamAfter = fi.Length;
                // inspection for difference between states of stream 
                // before and after copy process + length of copied array of bytes  
                // if there is 0 - its true => copy process was successfull 
                return (iSizeOfStreamAfter-iSizeOfStreamBefore)==arrBytes.Length;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
