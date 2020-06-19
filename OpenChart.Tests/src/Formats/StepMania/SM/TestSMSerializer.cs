using NUnit.Framework;
using OpenChart.Formats.StepMania.SM;
using System.Text;

namespace OpenChart.Tests.Formats.StepMania.SM
{
    public class TestSMSerializer
    {
        SMSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            serializer = new SMSerializer();
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(",")]
        public void Test_ParseBPMs_NoBPMs(string val)
        {
            var bpms = serializer.ParseBPMs(val);
            Assert.AreEqual(0, bpms.Count);
        }

        [TestCase("100=0", 100, 0)]
        [TestCase("123.45=1.2345", 123.45, 1.2345)]
        [TestCase("0=-1", 0, -1)]
        public void Test_ParseBPMs_Single(string val, double beat, double bpm)
        {
            var bpms = serializer.ParseBPMs(val);
            Assert.AreEqual(1, bpms.Count);
            Assert.AreEqual(beat, bpms[0].Beat);
            Assert.AreEqual(bpm, bpms[0].Value);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(",")]
        public void Test_ParseBPMs_NoStops(string val)
        {
            var stops = serializer.ParseStops(val);
            Assert.AreEqual(0, stops.Count);
        }

        [TestCase("100=0", 100, 0)]
        [TestCase("123.45=1.2345", 123.45, 1.2345)]
        public void Test_ParseStops_Single(string val, double beat, double seconds)
        {
            var stops = serializer.ParseStops(val);
            Assert.AreEqual(1, stops.Count);
            Assert.AreEqual(beat, stops[0].Beat);
            Assert.AreEqual(seconds, stops[0].Seconds);
        }

        [Test]
        public void Test_Deserialize_NoCharts()
        {
            var str = @"
            #BANNER:banner;
            #BACKGROUND:background;
            #CDTITLE:cdtitle;
            #SELECTABLE:yes;
            #CREDIT:credit;
            #OFFSET:123.45;
            #BPMS:0=120;
            #STOPS:0=1;
            #GENRE:genre;
            #LYRICSPATH:lyricspath;
            #MUSIC:music;
            #SAMPLELENGTH:3.33;
            #SAMPLESTART:6.66;
            #SUBTITLE:subtitle;
            #TITLE:title;
            #ARTISTTRANSLIT:artisttranslit;
            #SUBTITLETRANSLIT:subtitletranslit;
            #TITLETRANSLIT:titletranslit;
            ";

            var data = serializer.Deserialize(Encoding.UTF8.GetBytes(str));

            Assert.AreEqual("banner", data.DisplayData.Banner);
            Assert.AreEqual("background", data.DisplayData.Background);
            Assert.AreEqual("cdtitle", data.DisplayData.CDTitle);
            Assert.AreEqual(true, data.DisplayData.Selectable);

            Assert.AreEqual("credit", data.MetaData.Credit);

            Assert.AreEqual(123.45, data.PlayData.Offset);
            Assert.AreEqual(1, data.PlayData.BPMs.Count);
            Assert.AreEqual(1, data.PlayData.Stops.Count);

            Assert.AreEqual("genre", data.SongData.Genre);
            Assert.AreEqual("lyricspath", data.SongData.LyricsPath);
            Assert.AreEqual("music", data.SongData.Music);
            Assert.AreEqual(3.33, data.SongData.SampleLength);
            Assert.AreEqual(6.66, data.SongData.SampleStart);
            Assert.AreEqual("subtitle", data.SongData.Subtitle);
            Assert.AreEqual("title", data.SongData.Title);
            Assert.AreEqual("artisttranslit", data.SongData.TransliteratedArtist);
            Assert.AreEqual("subtitletranslit", data.SongData.TransliteratedSubtitle);
            Assert.AreEqual("titletranslit", data.SongData.TransliteratedTitle);

        }
    }
}
