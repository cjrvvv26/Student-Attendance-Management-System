using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Student_Attendance_Management_System.Models
{
    public partial class SectionModel : ObservableObject
    {
        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("section_id")]
        public int section_id;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("course")]
        public string course;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("year_level")]
        public int year_level;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("section")]
        public string section;

        [ObservableProperty]
        public DateTime created_at;
    }
}
