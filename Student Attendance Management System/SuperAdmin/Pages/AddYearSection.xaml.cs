using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for AddYearSection.xaml
    /// </summary>
    public partial class AddYearSection : Page
    {
        private AdminModel adminModel;
        public AddYearSection(int adminUserID)
        {
            InitializeComponent();
            AdminVM adminVm = new(adminUserID);
            adminModel = adminVm.AdminData;
        }
        private void ViewCourses_Click(object sender, RoutedEventArgs e)
        {
            SuperAdminLayout? superAdminLayout = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
            if (superAdminLayout != null)
            {
                superAdminLayout.SAdminContent.Navigate(new ViewAllSections(adminModel.User_id));
            }
        }
        //Add course section
        private void AddCourseSection_Click(object sender, RoutedEventArgs e)
        {
            //validations
            bool containsNumberAndSpecial = Regex.IsMatch(sectionText.Text, @"(?=.*\d)(?=.*[^\w\s])");
            bool courseContainsNumberAndSpecial = Regex.IsMatch(courseText.Text, @"(?=.*\d)(?=.*[^\w\s])");
            bool containsLetters = Regex.IsMatch(yearText.Text, @"[a - zA - Z]");

            if (containsLetters)
            {
                MessageBox.Show("Year must be numbers only", "Invalid Year Level", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Convert.ToInt32(yearText.Text) > 4)
            {
                MessageBox.Show("Year shouldn't greather than 4 years", "Invalid Year Level", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (containsNumberAndSpecial)
            {
                MessageBox.Show("Only letter for section", "Invalid Section", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (courseContainsNumberAndSpecial)
            {
                MessageBox.Show("Only letters for course", "Invalid Course", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (sectionText.Text.Length > 1)
            {
                MessageBox.Show("Only one character is valid for section", "Invalid Section", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SectionModel section = new SectionModel()
            {
                Course = courseText.Text,
                Year_level = Convert.ToInt32(yearText.Text),
                Section = sectionText.Text,
            };
            //add course & section
            SectionOperations sectionOP = new();
            if (sectionOP.AddNewSection(section, adminModel.User_id))
            {
                SuperAdminLayout? mainPage = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
                if (mainPage != null)
                {
                    mainPage.SAdminContent.Navigate(new ViewAllSections(adminModel.User_id));
                }
            }
        }
    }
}