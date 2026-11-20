using MySql.Data.MySqlClient;
using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.ViewModel;
using System;
using System.Windows;

namespace Student_Attendance_Management_System.Student
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        // ✅ Connection string should be a class-level variable
        public bool isRegisterClick { get; private set; } = false;
        
        public SignUp()
        {
            InitializeComponent();
            DataContext = new StudentVM();
        }

        public void Register_Click(object sender, RoutedEventArgs e)
        {
            StudentModel student = new();
            if (DataContext is StudentVM studentVM)
            {
                if (maleBtn.IsChecked == true)
                {
                    studentVM.Gender = "Male";
                }

                if (femaleBtn.IsChecked == true)
                {
                    studentVM.Gender = "Female";
                }
                isRegisterClick = true;
                if (studentVM.AddStudent(student))
                {
                    this.Close();
                }
            }
        }
    }
}
