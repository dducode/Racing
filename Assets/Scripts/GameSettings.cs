using UnityEngine;

namespace Settings
{
    [System.Serializable]
    public struct GameSettings
    {
        [SerializeField] KeyCode _lightsTumbler;
        [SerializeField] KeyCode _braking;
        [SerializeField] Resolution _resolution;
        [SerializeField] Quality _quality;

        public KeyCode braking
        {
            get { return _braking; }
            set { _braking = value; }
        }
        public KeyCode lightsTumbler
        {
            get { return _lightsTumbler; }
            set { _lightsTumbler = value; }
        }
        public int resolution
        {
            get { return (int)_resolution; }
            set { _resolution = (Resolution)value; }
        }
        public int quality
        {
            get { return (int)_quality; }
            set { _quality = (Quality)value; }
        }
    }
    public enum Resolution
    {
        Auto, QuarterHD, HD, FullHD, QuadHD
    }
    public enum Quality
    {
        Low, Medium, High, VeryHigh, Ultra
    }
}
