using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Student_Attendance_Management_System.Models
{
    public partial class ClassModel : ObservableObject
    {
        [ObservableProperty]
        public ClassModel newClassModel;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("class_id")]
        public int class_id;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("subject_id")]
        public int subject_id;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("section_id")]
        public int section_id;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("teacher_id")]
        public int teacher_id;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("class_semester")]
        public string class_semester;

        [ObservableProperty]
        public string class_term;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("schedule_day")]
        public string schedule_day;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("schedule_time")]
        public string schedule_time;

        //For data view

        //Teacher
        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("employee_number")]
        public string teacher_no;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("teacher_name")]
        public string teacher_name;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("gender")]
        public string gender;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("age")]
        public int age;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("department")]
        public string department;

        //Course & Section
        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("course")]
        public string course;

        [ObservableProperty]
        public string year_section;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("year_level")]
        public int year_level;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("section")]
        public string section;

        //Subject
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
        [JsonPropertyName("subject_unit")]
        public int subject_unit;

    }
}
