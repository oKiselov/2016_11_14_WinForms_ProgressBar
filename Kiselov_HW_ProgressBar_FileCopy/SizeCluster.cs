﻿namespace Kiselov_HW_ProgressBar_FileCopy
{
    /// <summary>
    /// For choosing the size of copying buffer 
    /// </summary>
    public enum SizeCluster:int
    {
        Four = 4096, 
        Eight = 8192,
        Sixteen = 16384,
        ThirtyTwo = 38768
    }
}