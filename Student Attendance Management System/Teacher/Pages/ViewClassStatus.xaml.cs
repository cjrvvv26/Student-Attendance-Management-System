using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    /// Interaction logic for ViewClassStatus.xaml
    /// </summary>
    public partial class ViewClassStatus : Page
    {
        private TeacherModel teacherData;
        private int classID; 
        private bool isEnable = false;
        private AttendanceVM attendanceVM = new();
        private ClassModel _classModel;
        public ViewClassStatus(ClassModel classData, TeacherModel teacher)
        {
            InitializeComponent();
            GenerateTerm();
            termRecordFilter.SelectedIndex = 0;
            AttendanceOperations attendanceOp = new();
            teacherData = teacher;
            attendanceVM.GetClassById(classData.Class_id);
            classID = classData.Class_id;
            _classModel = classData;
            DisplayRecordByTerm(classID, termRecordFilter.SelectedItem.ToString(), attendanceVM);
            DataContext = attendanceVM;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (recordDataTable.Items.Count < 1)
                {
                    noRecordText.Visibility = Visibility.Visible;
                    recordDataTable.Visibility = Visibility.Collapsed;
                }else
                {
                    noRecordText.Visibility = Visibility.Collapsed;
                    recordDataTable.Visibility = Visibility.Visible;
                }

                ClassesOperations classOp = new();
                if (classOp.VerifyIfOnGoing(classID))
                {
                    isEnable = true;
                }else
                {
                    isEnable = false;
                }

            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        //Back to viewing all classes
        private void ViewAllClass_Click(object sender, RoutedEventArgs e)
        {
            TeacherLayout? layout = Application.Current.Windows.OfType<TeacherLayout>().FirstOrDefault();

            if (layout != null)
            {
                layout.TeacherFrame.Navigate(new Class(teacherData));
            }
        }
        //View attendance
        private void ViewRecord_Click(object sender, RoutedEventArgs e)
        {
            AttendanceModel selectedRecord = (AttendanceModel)((Button)sender).Tag;
            TeacherLayout? teacherPage = Application.Current.Windows.OfType<TeacherLayout>().FirstOrDefault();
            if (teacherPage != null)
            {
                teacherPage.TeacherFrame.Navigate(new ViewAttendanceRecord(classID, DateTime.Parse(selectedRecord.Date)));
            }
        }
        //Create Attendance
        private void CreateAttendance_Click(object sender, RoutedEventArgs e)
        {
            if (isEnable == false)
            {
                MessageBox.Show("Attendance can only be created while the class is in session.", "Reminder", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            AttendanceOperations attendOp = new();
            if (attendOp.HasAttendanceBeenCreatedToday(classID))
            {
                MessageBox.Show("Can only create one attendance per session", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                TeacherLayout? mainPage = Application.Current.Windows.OfType<TeacherLayout>().FirstOrDefault(); 
                if (mainPage != null)
                {
                    mainPage.TeacherFrame.Navigate(new Attendance(classID, teacherData));
                }   
            }
        }
        
        //View Overall Attendance
        private void ViewOverallAttendance_Click(object sender, RoutedEventArgs e)
        {
            TeacherLayout? mainPage = Application.Current.Windows.OfType<TeacherLayout>().FirstOrDefault();

            if (mainPage != null)
            {
                mainPage.TeacherFrame.Navigate(new OverallRecord(classID, _classModel.Class_term));
            }
        }

        private void GenerateTerm()
        {
            termRecordFilter.ItemsSource = new List<string>()
            {
                "All" ,"Prelim", "Midterm", "Finals"
            };
        }
        //Change the attendance record base on class id and term
        private void termRecordFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayRecordByTerm(classID, termRecordFilter.SelectedItem.ToString(), attendanceVM);

            DisplayNoDataText();
        }

        //Display record by term
        private void DisplayRecordByTerm(int classID, string term, AttendanceVM attendVM)
        {
            AttendanceOperations attendanceOp = new();
            if (termRecordFilter.SelectedIndex == 0)
            {
                AttendanceVM attendNewVm = new();
                attendVM.LoadSavedRecordsAll(classID);
                return;
            }
            attendVM.LoadSavedRecords(classID, term);
        }
        //Proper error handling
        private void DisplayNoDataText()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (recordDataTable.Items.Count > 0)
                {
                    recordDataTable.Visibility = Visibility.Visible;
                    noRecordText.Visibility = Visibility.Collapsed;
                    return;
                }
                recordDataTable.Visibility = Visibility.Collapsed;
                noRecordText.Visibility = Visibility.Visible;
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}
