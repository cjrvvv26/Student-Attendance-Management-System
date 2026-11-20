using CommunityToolkit.Mvvm.ComponentModel;
using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student_Attendance_Management_System.ViewModel
{
    public partial class AdminVM : ObservableObject
    {
        [ObservableProperty]
        public AdminModel adminData;

        [ObservableProperty]
        public string fullName;

        [ObservableProperty]
        public string role;

        public AdminVM(int userID)
        {
            AdminOperations adminOP = new();
            AdminData = adminOP.GetAdminData(userID);
            FullName = AdminData.First_name + " " + AdminData.Last_name;
            Role = $"[{AdminData.Role}]";
        }
    }
}
