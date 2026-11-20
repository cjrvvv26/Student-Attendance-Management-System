using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.SuperAdmin.Components;
using Student_Attendance_Management_System.Teacher.Components;
using Student_Attendance_Management_System.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Student_Attendance_Management_System.Teacher.Pages
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        public Dashboard(TeacherModel teacherData)
        {
            InitializeComponent();
            TodayClassList classList = new TodayClassList();
            classList.LoadClasses(teacherData);
            classListBox.Children.Add(classList);
            ReportSummary reportSum = new(teacherData);
            summaryReport.Children.Add(reportSum);
            ListOfUnreadMessages unreadMessages = new ListOfUnreadMessages(teacherData.User_id);
            unreadMessages.LoadUnreadMessages(teacherData.User_id);
            unreadNotificationBox.Children.Add(unreadMessages);
        }
    }
}
