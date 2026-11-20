using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
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

namespace Student_Attendance_Management_System.SuperAdmin.Pages
{
    /// <summary>
    /// Interaction logic for AddSubject.xaml
    /// </summary>

    public partial class AddSubject : Page
    {
        private AdminModel adminModel;
        private List<string> courseList = new List<string>();
        private List<string> sectionList = new List<string>();
        public AddSubject(int adminUserID)
        {
            InitializeComponent();
            DataContext = new SubjectVM();
            AdminVM adminVM = new(adminUserID);
            adminModel = adminVM.AdminData;

            courseAvailabilityCombo.SelectedIndex = 0;
            unitsCombo.SelectedIndex = 0;
            semesterCombo.SelectedIndex = 0;
            CourseSelection();
            SemesterSelection();
            UnitSelection();
        }

        private void courseAvailabilityCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (courseAvailabilityCombo.SelectedItem != null)
            {
                string? selectedCourse = courseAvailabilityCombo.SelectedItem.ToString();
                SectionOperations sectionOP = new();

                sectionList.Clear();

                switch (selectedCourse)
                {
                    case "BSIT":
                    case "BTLEd":
                    case "BT-AUTO":
                    case "BEEd":
                        foreach (int section in sectionOP.GetAllYearLevelByCourse(selectedCourse))
                        {
                            sectionList.Add(section.ToString());
                        }

                        yearCombo.ItemsSource = null;
                        yearCombo.ItemsSource = sectionList;
                        yearCombo.SelectedIndex = 0;
                        break;
                }
            }
        }

        private void ViewSubject_Click(object sender, RoutedEventArgs e)
        {
            SuperAdminLayout? superAdminLayout = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
            if (superAdminLayout != null)
            {
                superAdminLayout.SAdminContent.Navigate(new ViewSubjects(adminModel.User_id));
            }
        }

        private void semesterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CourseSelection()
        {
            SectionOperations sectionOP = new();
            foreach(string item in sectionOP.GetAllCourse())
            {
                courseList.Add(item);
            }
            courseAvailabilityCombo.ItemsSource = courseList;
        }
        private void SemesterSelection()
        {
            semesterCombo.ItemsSource = new List<string>
            {
                "1st Semester",
                "2nd Semester",
            };
        }

        private void UnitSelection()
        {
            unitsCombo.ItemsSource = new List<string>
            {
                "1",
                "2",
                "3",
                "4"
            };
        }

        private void AddSubject_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SubjectVM subjectVM)
            {
                subjectVM.Year_availability = yearCombo.Text;
                subjectVM.Subject_semester = semesterCombo.Text;
                subjectVM.Course_availability = courseAvailabilityCombo.Text;
                subjectVM.Units = Convert.ToInt32(unitsCombo.Text);

                if (subjectVM.AddSubjectData(adminModel.User_id))
                {
                    SuperAdminLayout? superAdminLayout = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
                    if (superAdminLayout != null)
                    {
                        superAdminLayout.SAdminContent.Navigate(new ViewSubjects(adminModel.User_id));
                    }
                }
            }
        }
    }
}
