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
    /// Interaction logic for ViewAllClasses.xaml
    /// </summary>
    public partial class ViewAllClasses : Page
    {
        public AdminModel adminModel { get; set; }
        public ViewAllClasses(int adminUserID)
        {
            InitializeComponent();
            DataContext = new ClassesVM();
            //get admin data
            AdminVM adminVM = new AdminVM(adminUserID);
            adminModel = adminVM.AdminData;
            // Set semester combo first
            GenerateSemester();
            semesterFilterCombo.SelectedIndex = 0;

            // Now fetch courses
            FetchCourses();
            courseFilterCombo.SelectedIndex = 0;

            // Fetch sections only if course index is not 0
            if (courseFilterCombo.SelectedIndex != 0)
            {
                FetchSectionsByCourse(courseFilterCombo.SelectedItem.ToString());
            }

            // Then assign DataContext
            DataContext = new ClassesVM();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (dataViewAllClass.Items.Count <= 0)
                {
                    dataViewAllClass.Visibility = Visibility.Collapsed;
                    displayErrorBox.Visibility = Visibility.Visible;
                    FilterSection.Visibility = Visibility.Collapsed;
                }
                else
                {
                    dataViewAllClass.Visibility = Visibility.Visible;
                    FilterSection.Visibility = Visibility.Visible;
                    displayErrorBox.Visibility = Visibility.Collapsed;
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
        //Select All Classes
        private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
        //Unselect all classes
        private void SelectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }
        
        //Add Class
        private void AddClass_Click(object sender, RoutedEventArgs e)
        {
            SuperAdminLayout? SAMainPage = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();

            if (SAMainPage != null)
            {
                SAMainPage.SAdminContent.Navigate(new AddClass(adminModel.User_id));
            }
        }
        //View Selected Class Info
        private void ViewClassInfo_Click(object sender, RoutedEventArgs e)
        {
            ClassModel selectedClass = (ClassModel)((Button)sender).Tag;

            SuperAdminLayout? SAMainPage = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();

            if (SAMainPage != null)
            {
                SAMainPage.SAdminContent.Navigate(new ViewClassDetails(selectedClass, adminModel.User_id));
            }
        }
        //Delete Selected Class
        private void DeleteClass_Click(object sender, RoutedEventArgs e)
        {
            ClassModel selectedClass = (ClassModel)((Button)sender).Tag;

            if (MessageBox.Show("Are you sure you want to delete this class?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ClassesOperations classOP = new();
                if (classOP.DeleteClass(selectedClass.Class_id, adminModel.User_id))
                {
                    AuditLogsOperations auditOP = new();
                    auditOP.AddLogs(adminModel.User_id, "Class", "Delete Class", "Delete", "Success");

                    SuperAdminLayout? mainPage = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();

                    if (mainPage != null)
                    {
                        mainPage.SAdminContent.Navigate(new ViewAllClasses(adminModel.User_id));
                    }
                }
            }
        }

        private void courseFilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string course = courseFilterCombo.SelectedItem.ToString();
            if (courseFilterCombo.SelectedIndex == 0)
            {
                List<string> sections = new() { "All Sections" };
                yearSectionFilterCombo.ItemsSource = sections;
                yearSectionFilterCombo.SelectedIndex = 0;
                semesterFilterCombo.SelectedIndex = 0;
                ClassesOperations classOP = new();
                dataViewAllClass.ItemsSource = null;
                dataViewAllClass.ItemsSource = classOP.GetAllClass();
                return;
            }
            if (yearSectionFilterCombo.SelectedIndex == 0 && courseFilterCombo.SelectedIndex != 0 && semesterFilterCombo.SelectedIndex != 0)
            {
                FetchAllClassByCourseSem(course, semesterFilterCombo.SelectedItem.ToString());
                return;
            }

            yearSectionFilterCombo.ItemsSource = null;
            dataViewAllClass.Items.Refresh();
            FetchAllClassByCourse(course);
            FetchSectionsByCourse(course);
        }
        //year selection changed
        private void yearSectionFilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (yearSectionFilterCombo.ItemsSource == null ||           yearSectionFilterCombo.SelectedItem == null)
                return;

            string course = courseFilterCombo.SelectedItem?.ToString();
            string semester = semesterFilterCombo.SelectedItem?.ToString();
            if (course == null) return;

            if (semesterFilterCombo.SelectedIndex != 0 && yearSectionFilterCombo.SelectedIndex != 0 && courseFilterCombo.SelectedIndex != 0)
            {
                int year = Convert.ToInt32(yearSectionFilterCombo.SelectedItem.ToString().Remove(1, 1));
                FetchAllClassByCourseYearSem(course, year, semester);
                return;
            }

            if (yearSectionFilterCombo.SelectedIndex != 0 && courseFilterCombo.SelectedIndex != 0)
            {
                int year = Convert.ToInt32(yearSectionFilterCombo.SelectedItem.ToString().Remove(1, 1));
                FetchAllClassByCourseYear(course, year);
                return;
            }
            FetchAllClassByCourse(course);
        }
        //Selection for semester
        private void semesterFilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (semesterFilterCombo.ItemsSource == null || semesterFilterCombo.SelectedItem == null)
                return;

            string course = courseFilterCombo.SelectedItem?.ToString();
            string semester = semesterFilterCombo.SelectedItem?.ToString();
            if (course == null) return;

            if (yearSectionFilterCombo.SelectedIndex == 0 && courseFilterCombo.SelectedIndex == 0 && semesterFilterCombo.SelectedIndex == 0)
            {
                ClassesOperations classOP = new();
                dataViewAllClass.ItemsSource = null;
                dataViewAllClass.ItemsSource = classOP.GetAllClass();
                return;
            }

            if (yearSectionFilterCombo.SelectedIndex == 0 && courseFilterCombo.SelectedIndex == 0 && semesterFilterCombo.SelectedIndex != 0)
            {
                FetchAllClassBySem(semester);
                return;
            }

            if (yearSectionFilterCombo.SelectedIndex != 0 && courseFilterCombo.SelectedIndex != 0 && semesterFilterCombo.SelectedIndex == 0)
            {
                FetchAllClassByCourseYear(course, Convert.ToInt32(yearSectionFilterCombo.SelectedItem.ToString().Remove(1, 1)));
                return;
            }

            if (courseFilterCombo.SelectedIndex != 0 && yearSectionFilterCombo.SelectedIndex == 0 && semesterFilterCombo.SelectedIndex != 0)
            {
                FetchAllClassByCourseSem(course, semester);
                return;
            }

            if (courseFilterCombo.SelectedIndex != 0 && yearSectionFilterCombo.SelectedIndex == 0 && semesterFilterCombo.SelectedIndex == 0)
            {
                FetchAllClassByCourse(course);
                return;
            }

            if (semesterFilterCombo.SelectedIndex != 0 && yearSectionFilterCombo.SelectedIndex != 0 && courseFilterCombo.SelectedIndex != 0)
            {
                int year = Convert.ToInt32(yearSectionFilterCombo.SelectedItem.ToString().Remove(1, 1));
                FetchAllClassByCourseYearSem(course, year, semester);
                return;
            }
        }
        //Generate Courses
        private void FetchCourses()
        {
            SectionOperations sectionOP = new();
            List<string> courses = sectionOP.GetAllCourse();
            courses.Insert(0, "All Courses");
            courseFilterCombo.ItemsSource = courses;
        }

        //Generate Sections By Course
        private void FetchSectionsByCourse(string course)
        {
            SectionOperations sectionOP = new();
            List<int> yearLevel = sectionOP.GetAllYearLevelByCourse(course);
            List<string> sections = sectionOP.GetListSections(course);
            List<string> sectionList = new();
            for (int i = 0; yearLevel.Count > i; i++)
            {
                sectionList.Add(yearLevel[i].ToString() + sections[i]);
            }
            sectionList.Insert(0, "All Sections");
            yearSectionFilterCombo.ItemsSource = sectionList;
            yearSectionFilterCombo.SelectedIndex = 0;
        }

        //Generate Semester
        private void GenerateSemester()
        {
            List<string> semester = new() { "All Semesters", "1st Semester", "2nd Semester" };
            semesterFilterCombo.ItemsSource = semester;
        }   


        //Fetch All Class By Course
        private void FetchAllClassByCourse(string course)
        {
            if (course != null)
            {
                ClassesOperations classOP = new();
                dataViewAllClass.ItemsSource = null;
                dataViewAllClass.ItemsSource = classOP.GetClassByCourse(course);
            }
        }
        //Fetch All Class By Course and Year
        private void FetchAllClassByCourseYear(string course, int? year)
        {
            if (course != null && year != null)
            {
                ClassesOperations classesOperations = new();
                dataViewAllClass.ItemsSource = classesOperations.GetClassByCourseYear(course, year);
            }
        }
        //Fetch All Class By Course, Section, and Semester
        private void FetchAllClassByCourseYearSem(string course, int? year, string semester)
        {
            if (course != null && year != null && semester != null)
            {
                ClassesOperations classeOP = new();
                dataViewAllClass.ItemsSource = classeOP.GetClassByCourseYearSem(course, year, semester);
            }
        }
        //Fetch All Class By Course and Semester
        private void FetchAllClassByCourseSem(string course, string semester)
        {
            if (course != null && semester != null)
            {
                ClassesOperations classeOP = new();
                dataViewAllClass.ItemsSource = classeOP.GetClassByCourseSem(course, semester);
            }
        }
        //Fetch All Class By Semester
        private void FetchAllClassBySem(string semester)
        {
            if (semester != null)
            {
                ClassesOperations classeOP = new();
                dataViewAllClass.ItemsSource = classeOP.GetClassBySem(semester);
            }
        }

        //Delete all class
        private void DeleteSelectedClassIcon_Click(object sender, RoutedEventArgs e)
        {
            ClassesOperations classOP = new();
            if (MessageBox.Show("Are you sure you want to delete all classes?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (classOP.DeleteAllClass(adminModel.User_id))
                {
                    SuperAdminLayout? mainPage = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
                    if (mainPage != null)
                    {
                        mainPage.SAdminContent.Navigate(new ViewAllClasses(adminModel.User_id));
                    }
                }
            }
        }
    }
}
