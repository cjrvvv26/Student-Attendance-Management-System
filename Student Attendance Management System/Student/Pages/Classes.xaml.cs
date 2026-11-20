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

namespace Student_Attendance_Management_System.Student.Pages
{
    /// <summary>
    /// Interaction logic for Classes.xaml
    /// </summary>
    public partial class Classes : Page
    {
        private StudentModel student;
        public Classes(StudentModel studentData)
        {
            InitializeComponent();
            student = studentData;
            ClassesVM classesVM = new ClassesVM();
            classesVM.LoadClassesByStudent(studentData.Student_id);
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
                }
                else
                {
                    todayClassTable.Visibility = Visibility.Collapsed;
                    noTodayClassText.Visibility = Visibility.Visible;
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void ViewClassStatus_Click(object sender, RoutedEventArgs e)
        {
            ClassModel selectClass = (ClassModel)((Button)sender).Tag;
            ClassesOperations classOp = new();
            ClassModel selectedClassData = classOp.GetClassById(selectClass.Class_id);
            if (selectedClassData != null)
            {
                StudentPageLayout? mainPage = Application.Current.Windows.OfType<StudentPageLayout>().FirstOrDefault();
                if (mainPage != null)
                {
                    mainPage.StudentLayout.Navigate(new ViewClassStats(selectClass, student));
                }
            }
        }
    }
}
