using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.Student.Components;
using Student_Attendance_Management_System.Student.Pages;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Student_Attendance_Management_System.Student
{
    /// <summary>
    /// Interaction logic for StudentPageLayout.xaml
    /// </summary>
    public partial class StudentPageLayout : Window
    {
        private StudentModel _studentData;
        public StudentPageLayout(StudentModel student)
        {
            InitializeComponent();
            _studentData = student;
            student.Full_name = $"{student.First_name} {student.Last_name}";
            student.Role = $"[{student.Role}]";
            DataContext = student;
            StudentLayout.Navigate(new Dashboard(student));
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            StudentLayout.Navigate(new Dashboard(_studentData));
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            StudentLayout.Navigate(new Profile(_studentData));
        }

        private void Classes_Click(object sender, RoutedEventArgs e)
        {
            StudentLayout.Navigate(new Classes(_studentData));
        }

        private void Notifications_Click(object sender, RoutedEventArgs e)
        {
            StudentLayout.Navigate(new Notifications(_studentData));
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                        "Do you want to exit the application or go back to the login page?",
                        "Logout Confirmation",
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    // Exit the app
                    Application.Current.Shutdown();
                    break;

                case MessageBoxResult.No:
                    // Go back to login page (MainWindow)
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close(); // Assuming this is called from another window like Dashboard
                    break;

                case MessageBoxResult.Cancel:
                    // Do nothing, stay on current window
                    break;
            }

        }
    }
}
