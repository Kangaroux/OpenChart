using Gtk;
using OpenChart.Charting;
using NativeObjects = OpenChart.Charting.Objects;
using OpenChart.Charting.Properties;
using OpenChart.UI.Actions;
using OpenChart.UI.Widgets;
using Serilog;

namespace OpenChart.UI.Windows
{
    /// <summary>
    /// The main window of the application.
    /// </summary>
    public class MainWindow : Window
    {
        const int InitialWindowWidth = 800;
        const int InitialWindowHeight = 600;
        const int MinimumWindowWidth = 360;
        const int MinimumWindowHeight = 240;

        Application application;
        VBox container;

        public MainWindow(Application application) : base("OpenChart")
        {
            this.application = application;
            Title = "OpenChart";
            DeleteEvent += onDelete;

            SetIconFromFile(System.IO.Path.Join("icons", "AppIcon.ico"));

            var chart = new Chart(4);
            chart.BPMList.BPMs.Add(new BPM(120, 0));

            var noteSkin = OpenChart
                .Application
                .GetInstance()
                .AppData
                .NoteSkins
                .GetNoteSkin("default_arrow")
                .GetKeyModeSkin(chart.KeyCount.Value);

            var noteFieldData = new NoteFieldData(
                chart,
                noteSkin,
                keyWidth: 96,
                pixelsPerSecond: 200,
                timeOffset: 0.5,
                centerObjectsOnBeatLines: true
            );

            var noteField = new NoteField(noteFieldData);

            chart.Objects[0].Add(new NativeObjects.TapNote(0, 0));
            chart.Objects[1].Add(new NativeObjects.TapNote(1, 0));
            chart.Objects[2].Add(new NativeObjects.TapNote(2, 0));
            chart.Objects[3].Add(new NativeObjects.TapNote(3, 0));

            chart.Objects[0].Add(new NativeObjects.TapNote(0, 1));
            chart.Objects[1].Add(new NativeObjects.TapNote(1, 1.25));
            chart.Objects[2].Add(new NativeObjects.TapNote(2, 1.5));
            chart.Objects[3].Add(new NativeObjects.TapNote(3, 1.75));

            chart.Objects[0].Add(new NativeObjects.HoldNote(0, 2, 2.4));

            container = new VBox();
            container.PackStart(new Widgets.MenuBar(new MenuModel().GetModel()), false, false, 0);
            container.Add(noteField);

            Add(container);

            SetGeometryHints(
                null,
                new Gdk.Geometry
                {
                    MinWidth = MinimumWindowWidth,
                    MinHeight = MinimumWindowHeight,
                },
                Gdk.WindowHints.MinSize
            );

            SetDefaultSize(InitialWindowWidth, InitialWindowHeight);
            SetPosition(WindowPosition.Center);
        }

        private void onDelete(object o, DeleteEventArgs e)
        {
            application.ActivateAction(QuitAction.Name, null);
        }
    }
}
