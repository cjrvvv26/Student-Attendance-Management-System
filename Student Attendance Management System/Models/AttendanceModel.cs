using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student_Attendance_Management_System.Models
{
    public partial class AttendanceModel : ClassModel
    {
        //Date
        [ObservableProperty]
        public string date;

        //DateTime
        [ObservableProperty]
        public string date_time;

        //Attendance list viewing
        [ObservableProperty]
        public int record_id;

        [ObservableProperty]
        public int total_present;

        [ObservableProperty]
        public int total_absent;

        [ObservableProperty]
        public int total_late;

        [ObservableProperty]
        public int total_students;

        [ObservableProperty]
        public int abjusted_absent;

        //student details viewing
        [ObservableProperty]
        public int student_id;

        [ObservableProperty]
        public string student_number;

        [ObservableProperty]
        public string student_name;

        [ObservableProperty]
        public string student_gender;

        [ObservableProperty]
        public int student_age;

        [ObservableProperty]
        public string student_status;

        [ObservableProperty]
        public int student_total_absent;

        [ObservableProperty]
        public int student_total_present;

        [ObservableProperty]
        public int student_total_late;

        [ObservableProperty]
        public string final_status;

        //Gender Performance
        [ObservableProperty]
        public string two_gender;

        [ObservableProperty]
        public int two_month;

        [ObservableProperty]
        public int total_session;

        [ObservableProperty]
        public int present_count;

        [ObservableProperty]
        public double performance_percentage;
    }
}
