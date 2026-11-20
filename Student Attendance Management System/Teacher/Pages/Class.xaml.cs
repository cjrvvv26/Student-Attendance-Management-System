using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
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
    /// Interaction logic for Class.xaml
    /// </summary>
    public partial class Class : Page
    {
        private TeacherModel teacherData;
        public Class(TeacherModel teacher)
        {
            InitializeComponent();
            teacherData = teacher;
            ClassesVM classesVM = new ClassesVM();
            classesVM.LoadClassesByTeacher(teacher.Teacher_id);
            DataContext = classesVM;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (displayAllClass.Items.Count < 1)
                {
                    classSectionDisplay.Visibility = Visibility.Collapsed;
                    noClassText.Visibility = Visibility.Visible;
                    return;
                }

                if (todayClassTable.Items.Count > 0)
                {
                    todayClassTable.Visibility = Visibility.Visible;
                    noTodayClassText.Visibility = Visibility.Collapsed;
                }else
                {
                    todayClassTable.Visibility = Visibility.Collapsed;
                    noTodayClassText.Visibility = Visibility.Visible;
                }
            }),System.Windows.Threading.DispatcherPriority.Background);
        }

        //View Class Status
        private void ViewClassStatus_Click(object sender, RoutedEventArgs e)
        {
            ClassModel selectClass = (ClassModel)((Button)sender).Tag;
            ClassesOperations classOp = new();
            ClassModel selectedClassData = classOp.GetClassById(selectClass.Class_id);
            if (selectedClassData != null)
            {
                //teacher can only create attendance and print it not update the class itself
                //class details
                //in attendance page, must have print to pdf btn
                //create attendance button this will be enabled once the scheduled time is meet and once the scheduled time is ended all the student who's not in the class within 15mins then that student will be automatically absent.
                //button of enrolled students
                //page of list of students
                // in recording attendance it should have one button only "Not Here" and "Present"

                TeacherLayout? mainPage = Application.Current.Windows.OfType<TeacherLayout>().FirstOrDefault();

                if (mainPage != null)
                {
                    mainPage.TeacherFrame.Navigate(new ViewClassStatus(selectedClassData, teacherData));
                }
            }
        }
    }
}
