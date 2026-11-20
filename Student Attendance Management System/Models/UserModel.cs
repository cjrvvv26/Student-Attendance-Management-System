using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student_Attendance_Management_System.Models
{
    public partial class UserModel : ObservableObject
    {
        [ObservableProperty]
        public int user_id;

        [ObservableProperty]
        public int? user_PrimaryID;

        [ObservableProperty]
        public string user_no;

        [ObservableProperty]
        public string first_name;

        [ObservableProperty]
        public string middle_name;

        [ObservableProperty]
        public string last_name;

        [ObservableProperty]
        public string gender;

        [ObservableProperty]
        public string email;

        [ObservableProperty]
        public string password;

        [ObservableProperty]
        public string role;
    }
}
