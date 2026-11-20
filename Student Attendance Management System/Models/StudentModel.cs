using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Student_Attendance_Management_System.Models
{
    public partial class StudentModel : UserModel
    {
        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("student_id")]
        public int student_id;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("student_number")]
        public string student_number;

        [ObservableProperty]
        public string full_name;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("first_name")]
        private string first_name;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("middle_name")]
        private string middle_name;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("last_name")]
        private string last_name;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("email")]
        private string email;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("password")]
        private string password;

        [ObservableProperty]
        [JsonPropertyName("section_id")]
        private int section_id;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("birth_date")]
        public DateTime? birth_date;

        [ObservableProperty]
        [JsonInclude]
        [JsonPropertyName("age")]
        public int age;

        [ObservableProperty]
        [JsonPropertyName("gender")]
        private string gender;

        [ObservableProperty]
        public string student_status = "Pending";

        [ObservableProperty]
        public DateTime created_at;

        [ObservableProperty]
        public DateTime updated_at;

        [ObservableProperty]
        public string course;

        [ObservableProperty]
        public bool canUpdate = true;

        [ObservableProperty]
        public string year_section;

        partial void OnBirth_dateChanged(DateTime? value)
        {
            calculateAge();
        }

        private void calculateAge()
        {
            if (Birth_date.HasValue)
            {
                DateTime currentDate = DateTime.Today;
                int age = currentDate.Year - Birth_date.Value.Year;
                if (Birth_date.Value > currentDate.AddYears(-age))
                {
                    age--;
                }
                Age = age;
                OnPropertyChanged(nameof(Age));
            }
        }
    }
}
