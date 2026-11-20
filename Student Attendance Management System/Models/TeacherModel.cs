using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Student_Attendance_Management_System.Models
{
    public partial class TeacherModel : UserModel
    {

        [ObservableProperty]
        public int teacher_id;

        [ObservableProperty]
        public string employee_number;
        
        [ObservableProperty]
        public byte[]? profile_pic;

        [ObservableProperty]
        public int age;

        [ObservableProperty]
        public DateTime? birth_date;

        [ObservableProperty]
        public string department;

        [ObservableProperty]
        public string phone_no;

        [ObservableProperty]
        public DateTime updated_at;

        //For viewing
        [ObservableProperty]
        public string full_name;

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
