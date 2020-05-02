using OpenChart.Formats.OpenChart.Version0_1.Data;
using OpenChart.Formats.OpenChart.Version0_1.JsonConverters;
using System;
using System.Text;
using System.Text.Json;

namespace OpenChart.Formats.OpenChart.Version0_1
{
    /// <summary>
    /// Serializer for the OpenChart format. Uses JSON to serialize/deserialize FileData objects.
    /// </summary>
    public class OpenChartSerializer : IFormatSerializer<ProjectData>
    {
        public static JsonSerializerOptions jsonOptions;

        static OpenChartSerializer()
        {
            jsonOptions = new JsonSerializerOptions();

            jsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            jsonOptions.Converters.Add(new BeatConverter());
            jsonOptions.Converters.Add(new BeatDurationConverter());
            jsonOptions.Converters.Add(new KeyIndexConverter());
            jsonOptions.Converters.Add(new KeyCountConverter());
            jsonOptions.Converters.Add(new ChartObjectConverter());
        }

        /// <summary>
        /// Deserializes raw JSON data into a FileData object.
        /// </summary>
        /// <param name="data">JSON data.</param>
        public ProjectData Deserialize(byte[] data)
        {
            return (ProjectData)JsonSerializer.Deserialize(data, typeof(ProjectData), jsonOptions);
        }

        /// <summary>
        /// Serializes a FileData object into JSON.
        /// </summary>
        /// <param name="fd">The FileData object.</param>
        public byte[] Serialize(ProjectData pd)
        {
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(pd, jsonOptions));
        }
    }
}
