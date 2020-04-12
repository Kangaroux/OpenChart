using System;

namespace OpenChart.Charting
{
    public abstract class ChartObject : Timing
    {
        int _key;

        /// <summary>
        /// The key index this object occurs on. The first key starts at zero.
        /// </summary>
        public int Key
        {
            get => _key;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Key cannot be less than 0.");
                }

                _key = value;
            }
        }

        public ChartObject(int key, double beat) : base(beat)
        {
            Key = key;
        }

        /// <summary>
        /// Checks if the intended location for the object is valid or not. This method should
        /// throw a `ChartException` if the location is invalid (with the exception message
        /// being the reason why it's invalid).
        ///
        /// It's never valid to insert two objects that occur on the same beat and key.
        /// </summary>
        /// <param name="prev">The object immediately before this (or null).</param>
        /// <param name="next">The object immediately after this (or null).</param>
        public virtual void CanBeInserted(ChartObject prev, ChartObject next) { }
    }
}