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

namespace Student_Attendance_Management_System.SuperAdmin.Pages
{
    /// <summary>
    /// Interaction logic for AddStudent.xaml
    /// </summary>
    public partial class AddStudent : Page
    {
        public AddStudent()
        {
            InitializeComponent();
            DataContext = new StudentVM();
            SelectCourseSection();
            ComboCourse.SelectedIndex = 0;
        }

        private void SelectCourseSection()
        {
            SectionOperations sectionOP = new();
            List<string> courses = sectionOP.GetAllCourse();

            ComboCourse.ItemsSource = courses;
        }

        private void ComboCourse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectCourseSection();
            FetchYearSectionsByCourse(ComboCourse.SelectedItem.ToString());
        }

        private void FetchYearSectionsByCourse(string course)
        {
            if (course == null || course == "Select Course")
            {
                return; // Exit early if no valid course is selected
            }
            SectionOperations sectionOP = new();
            List<string> sections = sectionOP.GetListSections(course);
            List<int> yearLevels = sectionOP.GetAllYearLevelByCourse(course);

            List<string> yearSectionList = new();
            for (int i = 0; yearLevels.Count > i; i++)
            {
                yearSectionList.Add($"{yearLevels[i]}{sections[i]}");
            }
            ComboSection.ItemsSource = yearSectionList;
            ComboSection.SelectedIndex = 0;
        }

        private void AddStudent_Click(object sender, RoutedEventArgs e)
        {
            SectionOperations sectionOP = new();
            string course = ComboCourse.SelectedItem.ToString();
            string section = ComboSection.SelectedItem.ToString().Substring(1);
            int year = Convert.ToInt32(ComboSection.SelectedItem.ToString().Remove(1, 1));
            SectionModel selectedSection = sectionOP.GetIdByCourseSectionYear(course, year, section);

            StudentModel studentModel = new()
            {
                First_name = fnameBox.Text,
                Middle_name = mnameBox.Text,
                Last_name = lnameBox.Text,
                Email = email.Text,
                Section_id = selectedSection.Section_id,
            };
            if (DataContext is StudentVM studentVM)
            {
                if (maleRadio.IsChecked == true)
                {
                    studentVM.Gender = "Male";
                }

                if (femaleRadio.IsChecked == true)
                {
                    studentVM.Gender = "Female";
                }
                    studentVM.Course = ComboCourse.Text;
                    studentVM.Year_level = ComboSection.Text;
                if (studentVM.AddStudent(studentModel))
                {
                    Users users = new();
                    SuperAdminLayout? mainPageAdmin = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
                    if (mainPageAdmin != null)
                    {
                        mainPageAdmin.SAdminContent.Navigate(new Users());
                    }
                    this.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            Users users = new();
            SuperAdminLayout? mainPageAdmin = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
            if (mainPageAdmin != null)
            {
                mainPageAdmin.SAdminContent.Navigate(new Users());
            }
        }
    }
}
