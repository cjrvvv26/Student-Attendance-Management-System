using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.Student.Components;
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

namespace Student_Attendance_Management_System.Student.Pages
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        private StudentModel studentModel;
        public Dashboard(StudentModel student)
        {
            InitializeComponent();
            studentModel = student;
            StudentSummary summary = new StudentSummary(student);
            StudentTodayClass studentTodayClass = new StudentTodayClass(student);
            NotificationList notificationList = new(student.User_id);
            OverallPerformance performance = new(student.Student_id);
            notificationList.LoadUnreadMessages(student.User_id);
            studentSummary.Children.Add(summary);
            studentTodayClass.LoadClasses(student);
            yourPerformance.Children.Add(performance);
            unreadNotificationBox.Children.Add(notificationList);
            classListBox.Children.Add(studentTodayClass);   
        }

        //Redirect to notification
        private void ViewMoreNotif_Click(object sender, RoutedEventArgs e)
        {
            StudentPageLayout? studentPage = Application.Current.Windows.OfType<StudentPageLayout>().FirstOrDefault();

            if (studentPage != null)
            {
                studentPage.StudentLayout.Navigate(new Notifications(studentModel));
            }
        }
        //Redirect to classes
        private void ViewMoreClass_Click(object sender, RoutedEventArgs e)
        {
            StudentPageLayout? studentPage = Application.Current.Windows.OfType<StudentPageLayout>().FirstOrDefault();

            if (studentPage != null)
            {
                studentPage.StudentLayout.Navigate(new Classes(studentModel));
            }

        }
    }
}
