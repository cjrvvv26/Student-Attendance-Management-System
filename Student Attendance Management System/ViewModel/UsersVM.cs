using CommunityToolkit.Mvvm.ComponentModel;
using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Student_Attendance_Management_System.ViewModel
{
    public partial class UsersVM : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<UserModel> users = new();

        UsersOperations userOp = new UsersOperations();
        public UsersVM()
        {
            UserData();
        }

        public void UserData()
        {
            Users = userOp.GetAllUsers();
        }
    }
}
