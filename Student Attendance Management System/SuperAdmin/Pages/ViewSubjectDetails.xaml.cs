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
    /// Interaction logic for ViewSubjectDetails.xaml
    /// </summary>
    public partial class ViewSubjectDetails : Page
    {
        private object _subject { get; set; }
        private List<string> courseList = new List<string>();
        private List<string> sectionList = new List<string>();
        private AdminModel adminModel;  

        public ViewSubjectDetails(SubjectModel subject, int adminUserID)
        {
            InitializeComponent();
            _subject = subject;
            DataContext = subject;
            AdminVM adminVm = new(adminUserID);
            adminModel = adminVm.AdminData;
            CourseSelection();
            SemesterSelection();
            UnitSelection();
        }

        private void ViewSubjects_Click(object sender, RoutedEventArgs e)
        {
            SuperAdminLayout? mainSuperAdmin = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();

            if (mainSuperAdmin != null)
            {
                mainSuperAdmin.SAdminContent.Navigate(new ViewSubjects(adminModel.User_id));
            }
        }
        //Update Subject Button
        private void UpdateSubject_Click(object sender, RoutedEventArgs e)
        {
            //Check if there's existing course and section before you can update
            SectionOperations sectionOP = new();
            ObservableCollection<SectionModel> allCourseSection = sectionOP.GetAllSection();
            if (allCourseSection.Count <= 0)
            {
                MessageBox.Show("Course and section are required before you update the subject", "No Course and Section", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            UpdateUIComponents();
            updateBtn.Visibility = Visibility.Collapsed;
        }

        private void UpdateUIComponents()
        {
            //Hide view components
            subjectCodeText.Visibility = Visibility.Collapsed;
            subjectNameText.Visibility = Visibility.Collapsed;
            courseAvailabilityText.Visibility = Visibility.Collapsed;
            yearAvailabilityText.Visibility = Visibility.Collapsed;
            subjectUnitsText.Visibility = Visibility.Collapsed;
            subjectSemesterText.Visibility = Visibility.Collapsed;

            //Show textbox components

            subjectCodeBox.Visibility = Visibility.Visible;
            subjectNameBox.Visibility = Visibility.Visible;
            UpdateComponentBox.Visibility = Visibility.Visible;
            courseAvailabilityBox.Visibility = Visibility.Visible;
            yearAvailabilityBox.Visibility = Visibility.Visible;
            subjectUnitsBox.Visibility = Visibility.Visible;
            subjectSemesterBox.Visibility = Visibility.Visible;

            //Set the values of the textboxes
            if (_subject is SubjectModel subject)
            {
                subjectCodeBox.Text = subject.Subject_code;
                subjectNameBox.Text = subject.Subject_name;
                courseAvailabilityBox.Text = subject.Course_availability;
                yearAvailabilityBox.Text = subject.Year_availability;
                subjectUnitsBox.Text = subject.Units.ToString();
                subjectSemesterBox.Text = subject.Subject_semester;
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            //Make visible view components
            subjectCodeText.Visibility = Visibility.Visible;
            subjectNameText.Visibility = Visibility.Visible;
            updateBtn.Visibility = Visibility.Visible;
            courseAvailabilityText.Visibility = Visibility.Visible;
            yearAvailabilityText.Visibility = Visibility.Visible;
            subjectUnitsText.Visibility = Visibility.Visible;
            subjectSemesterText.Visibility = Visibility.Visible;

            //Hide textbox components

            subjectCodeBox.Visibility = Visibility.Collapsed;
            subjectNameBox.Visibility = Visibility.Collapsed;
            UpdateComponentBox.Visibility = Visibility.Collapsed;
            courseAvailabilityBox.Visibility = Visibility.Collapsed;
            yearAvailabilityBox.Visibility = Visibility.Collapsed;
            subjectUnitsBox.Visibility = Visibility.Collapsed;
            subjectSemesterBox.Visibility = Visibility.Collapsed;
        }

        private void courseAvailabilityBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (courseAvailabilityBox.SelectedItem != null)
            {
                string? selectedCourse = courseAvailabilityBox.SelectedItem.ToString();
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

                        yearAvailabilityBox.ItemsSource = null;
                        yearAvailabilityBox.ItemsSource = sectionList;
                        yearAvailabilityBox.SelectedIndex = 0;
                        break;
                }
            }
        }

        private void CourseSelection()
        {
            SectionOperations sectionOP = new();
            foreach (string item in sectionOP.GetAllCourse())
            {
                courseList.Add(item);
            }
            courseAvailabilityBox.ItemsSource = courseList;
        }

        private void SemesterSelection()
        {
            subjectSemesterBox.ItemsSource = new List<string>
            {
                "1st Semester",
                "2nd Semester",
            };
        }

        private void UnitSelection()
        {
            subjectUnitsBox.ItemsSource = new List<string>
            {
                "1",
                "2",
                "3",
                "4"
            };
        }
        private void ProceedUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_subject is SubjectModel subject)
            {
                subject.Subject_id = subject.Subject_id;
                subject.Subject_code = subjectCodeBox.Text;
                subject.Subject_name = subjectNameBox.Text;
                subject.Course_availability = courseAvailabilityBox.Text;
                subject.Year_availability = yearAvailabilityBox.Text;
                subject.Units = int.Parse(subjectUnitsBox.Text);
                subject.Subject_semester = subjectSemesterBox.Text;



                SubjectOperations subjectOP = new();

                if (subjectOP.UpdateSubject(subject, adminModel.User_id))
                {
                    SuperAdminLayout? mainSuperAdmin = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();

                    if (mainSuperAdmin != null)
                    {
                        mainSuperAdmin.SAdminContent.Navigate(new ViewSubjects(adminModel.User_id));
                    }
                }
            }
        }
    }
}
