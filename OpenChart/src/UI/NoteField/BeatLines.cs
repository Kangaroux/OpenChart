using System;
using System.Collections.Generic;

namespace OpenChart.UI.NoteField
{
    /// <summary>
    /// Note field widget for displaying beat lines.
    /// </summary>
    public class BeatLines : IWidget
    {
        /// <summary>
        /// The drawing settings to use for the beat lines.
        /// </summary>
        public BeatLineSettings BeatLineSettings { get; private set; }

        /// <summary>
        /// The settings for the note field.
        /// </summary>
        public NoteFieldSettings NoteFieldSettings { get; private set; }

        Gtk.DrawingArea drawingArea;

        /// <summary>
        /// Returns the widget for displaying beat lines.
        /// </summary>
        public Gtk.Widget GetWidget() => drawingArea;

        /// <summary>
        /// Creates a new BeatLines instance.
        /// </summary>
        /// <param name="noteFieldSettings">The settings for the note field.</param>
        /// <param name="beatLineSettings">The settings for the BeatLines instance.</param>
        public BeatLines(NoteFieldSettings noteFieldSettings, BeatLineSettings beatLineSettings)
        {
            BeatLineSettings = beatLineSettings;
            NoteFieldSettings = noteFieldSettings;

            drawingArea = new Gtk.DrawingArea();
            drawingArea.Drawn += onDrawn;

            // Resize the widget when the note field is updated.
            NoteFieldSettings.ChartEventBus.Anything += delegate { resizeToFit(); };
            resizeToFit();
        }

        /// <summary>
        /// Draws horizontal beat lines at the given positions.
        /// </summary>
        private void drawBeatLines(Cairo.Context cr, int width, Color color, int thickness, List<int> positions)
        {
            cr.SetSourceColor(color.AsCairoColor());
            cr.LineWidth = thickness;

            // Fix the line not being the right thickness if the thickness is an odd number.
            var offset = (thickness % 2 == 1) ? 0.5 : 0;

            foreach (var y in positions)
            {
                var offsetY = y + offset;

                cr.MoveTo(0, offsetY);
                cr.LineTo(width, offsetY);
                cr.Stroke();
            }
        }

        private void onDrawn(object o, Gtk.DrawnArgs e)
        {
            // Get the area of the widget we are redrawing.
            var clip = e.Cr.ClipExtents();

            // The size of the vertical margin, in pixels. This fixes an issue where the first and
            // last beat lines drawn are partially cutoff due to drawing on the edge of the widget.
            var extendClip = 3;

            // Resize the clip region to use the new margin.
            e.Cr.ResetClip();
            e.Cr.Rectangle(clip.X, clip.Y - extendClip, clip.Width, clip.Height + (extendClip * 2));
            e.Cr.Clip();

            // Based on what's displayed on screen, get the time in the chart at the top of the
            // widget for the beat lines. This is to prevent drawing beat lines that aren't visible.
            // We don't need to worry about negative values here since clip.Y is never negative.
            var topTime = clip.Y / NoteFieldSettings.PixelsPerSecond;
            var bottomTime = (clip.Y + clip.Height) / NoteFieldSettings.PixelsPerSecond;

            var beatLines = new List<int>();
            var measureLines = new List<int>();

            // Iterate through the beats in the chart and record the y positions of each line.
            foreach (var beat in NoteFieldSettings.Chart.BPMList.Time.GetBeats(topTime))
            {
                if (beat.Time.Value >= bottomTime)
                    break;

                var y = (int)Math.Round(beat.Time.Value * NoteFieldSettings.PixelsPerSecond);

                if (beat.Beat.IsStartOfMeasure())
                    measureLines.Add(y);
                else
                    beatLines.Add(y);
            }

            // Draw the beat lines that occur at the start of a measure.
            drawBeatLines(
                e.Cr,
                (int)clip.Width,
                BeatLineSettings.MeasureLineColor,
                BeatLineSettings.MeasureLineThickness,
                measureLines
            );

            // Draw all of the other beat lines.
            drawBeatLines(
                e.Cr,
                (int)clip.Width,
                BeatLineSettings.BeatLineColor,
                BeatLineSettings.BeatLineThickness,
                beatLines
            );
        }

        /// <summary>
        /// Resizes the widget to fit the size of the note field.
        /// </summary>
        private void resizeToFit()
        {
            drawingArea.SetSizeRequest(NoteFieldSettings.NoteFieldWidth, NoteFieldSettings.NoteFieldHeight);
        }
    }
}