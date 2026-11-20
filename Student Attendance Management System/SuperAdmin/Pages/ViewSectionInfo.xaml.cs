using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ViewSectionInfo.xaml
    /// </summary>
    public partial class ViewSectionInfo : Page
    {
        private AdminModel adminModel;
        private SectionModel _section = new();    
        public ViewSectionInfo(SectionModel section, int adminUserID)
        {
            InitializeComponent();
            AdminVM adminVm = new(adminUserID);
            adminModel = adminVm.AdminData;
            _section = section;
            DataContext = section;
        }

        private void ViewAllCourseSections_Click(object sender, RoutedEventArgs e)
        {
            SuperAdminLayout? mainPageAdmin = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
            if (mainPageAdmin != null)
            {
                mainPageAdmin.SAdminContent.Navigate(new ViewAllSections(adminModel.User_id));
            }
        }
        //Change the UI to Update 
        private void ChangeToUpdate_Click(object sender, RoutedEventArgs e)
        {
            ChangeUIUpdate();
        }

        private void ChangeUIUpdate()
        {
            // Hide the course, section, year and update button
            courseText.Visibility = Visibility.Collapsed;
            sectionText.Visibility = Visibility.Collapsed;
            yearText.Visibility = Visibility.Collapsed;
            updateBtn.Visibility = Visibility.Collapsed;

            // Show the text boxes for updating
            courseBox.Visibility = Visibility.Visible;
            sectionBox.Visibility = Visibility.Visible;
            yearBox.Visibility = Visibility.Visible;
            forUpdateBox.Visibility = Visibility.Visible;

            //Set the text boxes with the current values
            courseBox.Text = courseText.Text;
            sectionBox.Text = sectionText.Text;
            yearBox.Text = yearText.Text;

        }

        private void ResetUIViewPage()
        {
            courseText.Visibility = Visibility.Visible;
            sectionText.Visibility = Visibility.Visible;
            yearText.Visibility = Visibility.Visible;
            updateBtn.Visibility = Visibility.Visible;

            // Hide the course, section, year and update button
            courseBox.Visibility = Visibility.Collapsed;
            sectionBox.Visibility = Visibility.Collapsed;
            yearBox.Visibility = Visibility.Collapsed;
            forUpdateBox.Visibility = Visibility.Collapsed;
        }

        private void ProceedUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(courseBox.Text) ||
                string.IsNullOrEmpty(yearBox.Text) ||
                string.IsNullOrEmpty(sectionBox.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }
            bool containsNumberAndSpecial = Regex.IsMatch(sectionBox.Text, @"(?=.*\d)(?=.*[^\w\s])");
            bool courseContainsNumberAndSpecial = Regex.IsMatch(courseBox.Text, @"(?=.*\d)(?=.*[^\w\s])");
            bool containsLetters = Regex.IsMatch(yearBox.Text, @"[a - zA - Z]");

            if (containsLetters)
            {
                MessageBox.Show("Year must be numbers only", "Invalid Year Level", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Convert.ToInt32(yearBox.Text) > 4)
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

            if (sectionBox.Text.Length > 1)
            {
                MessageBox.Show("Only one character is valid for section" ,"Invalid Section", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SectionOperations sectionOperations = new();
            MessageBox.Show(_section.Section_id.ToString());
            SectionModel sectionModel = new()
            {
                Section_id = _section.Section_id,
                Course = courseBox.Text,
                Year_level = Convert.ToInt32(yearText.Text),
                Section = sectionBox.Text,
            };
            if (sectionOperations.UpdateSection(sectionModel, adminModel.User_id))
            {
                MessageBox.Show(_section.Section_id.ToString() + "hello");
                SuperAdminLayout? mainPage = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();

                if (mainPage != null)
                {
                    mainPage.SAdminContent.Navigate(new ViewAllSections(adminModel.User_id));
                }
            }
        }

        private void CancelUpdate_Click(object sender, RoutedEventArgs e)
        {
            ResetUIViewPage();
        }
    }
}
