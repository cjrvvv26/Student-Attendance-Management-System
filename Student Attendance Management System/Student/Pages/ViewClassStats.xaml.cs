using FontAwesome.WPF;
using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.Teacher.Pages;
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

namespace Student_Attendance_Management_System.Student.Pages
{
    /// <summary>
    /// Interaction logic for ViewClassStats.xaml
    /// </summary>
    public partial class ViewClassStats : Page
    {
        private StudentModel student;
        private int classID;
        private bool isEnable = false;
        private AttendanceVM attendanceVM = new();
        private ClassModel _classModel;
        public ViewClassStats(ClassModel classData, StudentModel studentData)
        {
            InitializeComponent();
            AttendanceOperations attendanceOp = new();
            student = studentData;
            attendanceVM.GetClassById(classData.Class_id);
            classID = classData.Class_id;
            _classModel = classData;
            DataContext = attendanceVM;
        }

        private void ViewAllClass_Click(object sender, RoutedEventArgs e)
        {
            StudentPageLayout? mainPage = Application.Current.Windows.OfType<StudentPageLayout>().FirstOrDefault();
            if (mainPage != null)
            {
                mainPage.StudentLayout.Navigate(new Classes(student));
            }
        }
    }
}
