using UnityEngine;

[System.Serializable]
public struct GameSettings
{
    public KeyCode lightsTumbler;
    public KeyCode braking;
    public Quality quality;
    [Range(0, 1)]
    public float soundVolume;
    [Range(0, 1)]
    public float musicVolume;
}
public enum Quality
{
    Low, Medium, High
}
