﻿namespace ClosetUI.Client.Pages;

/// <summary>
/// Mouse information passed from JavaScript
/// </summary>
public class CanvasMouseArgs
{
    public int ScreenX { get; set; }
    public int ScreenY { get; set; }
    public int ClientX { get; set; }
    public int ClientY { get; set; }
    public int MovementX { get; set; }
    public int MovementY { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }
    public bool AltKey { get; set; }
    public bool CtrlKey { get; set; }
    public bool Bubbles { get; set; }
    public int Buttons { get; set; }
    public int Button { get; set; }
}

public class CanvasWheelArgs : CanvasMouseArgs // Inheriting to re-use mouse properties
{
    public double DeltaY { get; set; }
    public int DeltaMode { get; set; }
}
