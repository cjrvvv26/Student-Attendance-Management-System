using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
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

namespace Student_Attendance_Management_System.Student.Components
{
    /// <summary>
    /// Interaction logic for StudentTodayClass.xaml
    /// </summary>
    public partial class StudentTodayClass : UserControl
    {
        public ObservableCollection<ClassModel> todayClassList { get; set; } = new();
        private StudentModel studentModel;
        public StudentTodayClass(StudentModel studentData)
        {
            InitializeComponent();
            ClassesOperations classOp = new();
            studentModel = studentData;
            DataContext = this;
        }
        public void LoadClasses(StudentModel studentData)
        {
            ClassesOperations classOps = new();
            todayClassList = classOps.GetClassesByStudentAndDay(studentData.Student_id);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (todayClassList.Count <= 0)
                {
                    noClassText.Visibility = Visibility.Visible;
                    dataGridClassList.Visibility = Visibility.Collapsed;
                    return;
                }
                noClassText.Visibility = Visibility.Collapsed;
                dataGridClassList.Visibility = Visibility.Visible;
            }), System.Windows.Threading.DispatcherPriority.Background);

            DataContext = this;
        }
        //Modify student stats
        private void ModifyAttendance_Click(object sender, RoutedEventArgs e)
        {
            Button modifyBtn = (Button)sender;
            modifyBtn.IsEnabled = false;
            modifyBtn.Visibility = Visibility.Collapsed;
            ClassModel selectedClass = (ClassModel)((Button)sender).Tag;
            AttendanceOperations attendOp = new();
            int alreadyCreatedAttendance = attendOp.IfTeacherAlreadyCreateRecordToday(selectedClass.Class_id, studentModel.Student_id);
            if (alreadyCreatedAttendance > 0)
            {
                attendOp.StudentModifyStats(selectedClass.Class_id, studentModel.Student_id);
                MessageBox.Show("Recorded Successfully", "Success Modify Status", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            modifyBtn.IsEnabled = true;
            modifyBtn.Visibility = Visibility.Visible;
            MessageBox.Show("Can't modify status, attendance not created by teacher", "Failed Modify Status", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
