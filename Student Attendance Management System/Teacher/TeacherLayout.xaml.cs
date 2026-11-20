using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.Teacher.Pages;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Student_Attendance_Management_System.Teacher
{
    /// <summary>
    /// Interaction logic for TeacherLayout.xaml
    /// </summary>
    public partial class TeacherLayout : Window
    {
        private TeacherModel teacherModel;
        public TeacherLayout(TeacherModel teacher)
        {
            InitializeComponent();
            TeacherOperations teacherOp = new();

            teacherModel = teacherOp.GetTeacherDataByID(teacher.Teacher_id);
            //Display the profile pic
            if (teacherModel.Profile_pic != null)
            {
                using (MemoryStream ms = new(teacherModel.Profile_pic))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = ms;
                    bitmapImage.EndInit();
                    ImageBrush profileImg = new(bitmapImage);
                    profileImg.Stretch = Stretch.UniformToFill;
                    profilePicBox.Fill = profileImg;
                }
            }
            teacherModel.Full_name = $"{teacherModel.First_name} {teacherModel.Last_name}";
            teacherModel.Role = $"[{teacherModel.Role}]";
            DataContext = teacherModel;
            TeacherFrame.Navigate(new Dashboard(teacherModel));
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            TeacherFrame.Navigate(new Dashboard(teacherModel));
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            TeacherFrame.Navigate(new Profile(teacherModel));
        }

        private void Class_Click(object sender, RoutedEventArgs e)
        {
            TeacherFrame.Navigate(new Class(teacherModel));
        }

        private void Notifications_Click(object sender, RoutedEventArgs e)
        {
            TeacherFrame.Navigate(new Notifications(teacherModel));
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            TeacherFrame.Navigate(new TeacherReports(teacherModel));
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
