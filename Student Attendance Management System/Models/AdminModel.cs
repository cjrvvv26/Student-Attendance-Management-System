using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student_Attendance_Management_System.Models
{
    public partial class AdminModel : ObservableObject
    {
        [ObservableProperty]
        private int admin_id;

        [ObservableProperty]
        private int user_id;

        [ObservableProperty]
        private string first_name;

        [ObservableProperty]
        private string last_name;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string role;

        [ObservableProperty]
        private DateTime created_at;

        [ObservableProperty]
        private DateTime updated_at;
    }
}
