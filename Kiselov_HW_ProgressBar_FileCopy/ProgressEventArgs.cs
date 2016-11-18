using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiselov_HW_ProgressBar_FileCopy
{
    /// <summary>
    /// Class for events creation. 
    /// Each event will send information about current state of FileOperator object - 
    /// Amount of read bytes  
    /// </summary>
    public class ProgressEventArgs: EventArgs
    {
        public readonly int iCount;

        public ProgressEventArgs(int iTempCount)
        {
            iCount = iTempCount; 
        }
    }
}
