using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void StatsChangedEventHandler(StatsChangedEventArgs e);

public class StatsChangedEventArgs : EventArgs
{
    public float? MaxTimePoints { get; set; }
    public uint? Lives { get; set; }
    public uint? MaxLives { get; set; }
    public float? Score { get; set; }
}