using System;

namespace OpenChart.Charting
{
    /// <summary>
    /// The base class for any object that the player interacts with in a chart.
    /// </summary>
    public abstract class ChartObject : Timing
    {
        int _key;

        /// <summary>
        /// The key index this object occurs on.
        /// </summary>
        public int Key
        {
            get => _key;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Key cannot be less than 1.");
                }

                _key = value;
            }
        }

        public ChartObject(int key, double beat) : base(beat)
        {
            Key = key;
        }
    }
}