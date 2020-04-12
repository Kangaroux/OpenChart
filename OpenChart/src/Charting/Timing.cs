using System;

namespace OpenChart.Charting
{
    /// <summary>
    /// The base class for chart objects that are timing-based.
    /// </summary>
    public abstract class Timing
    {
        double _beat;

        /// <summary>
        /// The beat number. The time that this occurs during the chart is dependent on
        /// the BPM changes that come before it.
        /// </summary>
        public double Beat
        {
            get => _beat;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Beat cannot be negative.");
                }

                _beat = value;
            }
        }

        public Timing(double beat)
        {
            Beat = beat;
        }
    }
}