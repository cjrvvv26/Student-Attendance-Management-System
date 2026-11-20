using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Student_Attendance_Management_System.SuperAdmin.Pages
{
    /// <summary>
    /// Interaction logic for AllUsers.xaml
    /// </summary>
    public partial class AllUsers : Page
    {
        private object specifiedUser { get; set; }
        public AllUsers()
        {
            InitializeComponent();
            DataContext = new UsersVM();
        }

        //View and Update Data Here
        private void UpdateData_Click(object sender, RoutedEventArgs e)
        {
            UserModel user = (UserModel)((Button)sender).Tag;
            SuperAdminLayout? mainPageAdmin = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
            if (user.Role == "Teacher")
            {
                TeacherOperations teacherOP = new();
                TeacherModel? teacher = teacherOP.GetTeacherData(user.User_id);
                if (teacher != null)
                {
                    specifiedUser = teacher;
                }
                else
                {
                    MessageBox.Show("User doesn't found", "Not found", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (user.Role == "Student")
            {
                StudentOperations studentOP = new();
                StudentModel? student = studentOP.GetStudentData(user.User_id, DataHelper.connectDB());
                if (student != null)
                {
                    specifiedUser = student;
                }else
                {
                    MessageBox.Show("User doesn't found", "Not found", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (mainPageAdmin != null)
            {
                mainPageAdmin.SAdminContent.Navigate(new ViewUser(specifiedUser));
            }
            else
            {
                MessageBox.Show("Main window is not open! Please try again" , "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteData_Click(object sender, RoutedEventArgs e)
        {
            UserModel user = (UserModel)((Button)sender).Tag;

            if (user != null) {
                if (user.Role == "Teacher")
                {
                    if (MessageBox.Show("Are you sure you want to delete this user?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        if (MessageBox.Show("All classes handled by this teacher will be deleted.", "Class Warning", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            UsersOperations usersOP = new();
                            usersOP.DeleteUserData(user.User_id);
                        }
                    }
                    return;
                }
                if (MessageBox.Show("Are you sure you want to delete this user?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    UsersOperations usersOP = new();
                    usersOP.DeleteUserData(user.User_id);
                }
            }
        }
    }
}
