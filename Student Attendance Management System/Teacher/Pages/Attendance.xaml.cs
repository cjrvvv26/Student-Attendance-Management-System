using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.SuperAdmin.Pages;
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

namespace Student_Attendance_Management_System.Teacher.Pages
{
    /// <summary>
    /// Interaction logic for Attendance.xaml
    /// </summary>
    public partial class Attendance : Page
    {
        ObservableCollection<StudentModel> studentModel = new();
        private ClassModel classModel;
        private TeacherModel teacherModel;
        public Attendance(int classID, TeacherModel teacherData)
        {
            InitializeComponent();
            ClassesOperations classOp = new();
            classModel = classOp.GetClassById(classID);
            teacherModel = teacherData;
            StudentVM studentVM = new StudentVM();
            AttendanceOperations attendanceOp = new();
            attendanceOp.AutoUpdateAttendanceStatus();
            studentModel = studentVM.GetAllEnrolledStudentsByClassID(classID);
            MessageBox.Show(studentModel.Count.ToString());
            DataContext = studentVM;
        }

        //Update Student Status
        private void UpdateStatus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is StudentModel selectedStudent)
            {
                switch(selectedStudent.Student_status)
                {
                    case "Late":
                        selectedStudent.Student_status = "Present";
                        button.Background = button.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#00C951");
                        break;
                    case "Present":
                        selectedStudent.Student_status = "Absent";
                        button.Background = button.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FB2C36");
                        break;
                    case "Absent":
                        selectedStudent.Student_status = "Pending";
                        button.Background = button.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFA500");
                        break;
                    case "Pending":
                        selectedStudent.Student_status = "Late";
                        button.Background = button.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#8f8e8e");
                        break;
                }
            }
        }
        
        //Create new record
        private void CreateAttendanceBtn_Click(object sender, RoutedEventArgs e)
        {
            AttendanceOperations attendanceOp = new();
            foreach(var item in studentModel)
            {
                AttendanceModel record = new()
                {
                    Student_id = item.Student_id,
                    Class_id = classModel.Class_id,
                    Class_term = classModel.Class_term,
                    Student_status = item.Student_status,
                    Date = DateTime.Now.ToString("MMM-dd"),
                };

                attendanceOp.AddAttendanceRecord(record);
            }

            MessageBox.Show("New attendance has been created", $"Record [{DateTime.Now.ToString("MMM-dd-yy")}]", MessageBoxButton.OK, MessageBoxImage.Information);

            TeacherLayout? teacherLayout = Application.Current.Windows.OfType<TeacherLayout>().FirstOrDefault();

            if (teacherLayout != null)
            {
                teacherLayout.TeacherFrame.Navigate(new ViewClassStatus(classModel, teacherModel));
            }
        }
    }
}
