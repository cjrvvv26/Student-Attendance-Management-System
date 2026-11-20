using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Student_Attendance_Management_System.Models
{
    public partial class SubjectModel : ObservableObject
    {
        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("subject_id")]
        public int subject_id;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("subject_name")]
        public string subject_name;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("subject_code")]
        public string subject_code;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("subject_semester")]
        public string subject_semester;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("course_availability")]
        public string course_availability;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("year_availability")]
        public string year_availability;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("year_section")]
        public string year_section;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("units")]
        public int units;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public string created_at;
    }
}
