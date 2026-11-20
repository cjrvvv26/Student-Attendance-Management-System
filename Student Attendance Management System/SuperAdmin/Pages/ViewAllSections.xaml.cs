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
    /// Interaction logic for ViewAllSections.xaml
    /// </summary>
    public partial class ViewAllSections : Page
    {
        private AdminModel adminModel;
        private ObservableCollection<SectionModel> _allSections = new();
        public ViewAllSections(int adminUserID)
        {
            InitializeComponent();
            AdminVM adminVm = new(adminUserID);
            adminModel = adminVm.AdminData;
            DataContext = new SectionVM();
            GetAllCourse();
            courseFilterCombo.SelectedIndex = 0;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (allSectionDataGrid.Items.Count > 0)
                {
                    noSectionDisplayText.Visibility = Visibility.Collapsed;
                    allSectionDataGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    noSectionDisplayText.Visibility = Visibility.Visible;
                    allSectionDataGrid.Visibility = Visibility.Collapsed;
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        //View Sections
        private void ViewSectionInfo_Click(object sender, RoutedEventArgs e)
        {
            SectionModel section = (SectionModel)((Button)sender).Tag;
            SectionModel selectedSection = (SectionModel)((Button)sender).DataContext;
            // Check if the section is null
            SuperAdminLayout? mainPageAdmin = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
            if (mainPageAdmin != null)
            {
                mainPageAdmin.SAdminContent.Navigate(new ViewSectionInfo(selectedSection, adminModel.User_id));
            }
        }

        //add section
        private void AddSection_Click(object sender, RoutedEventArgs e)
        {
            SuperAdminLayout? mainPageAdmin = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
            if (mainPageAdmin != null)
            {
                mainPageAdmin.SAdminContent.Navigate(new AddYearSection(adminModel.User_id));
            }
        }

        //delete section
        private void DeleteSection_Click(object sender, RoutedEventArgs e)
        {
            SectionModel section = (SectionModel)((Button)sender).Tag;
            if (MessageBox.Show("Are you sure you want to delete this section?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                SectionOperations sectionOP = new();
                if (sectionOP.DeleteSection(section.Section_id, adminModel.User_id))
                {
                    SuperAdminLayout? mainPage = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
                    if (mainPage != null)
                    {
                        mainPage.SAdminContent.Navigate(new ViewAllSections(adminModel.User_id));
                    }
                }
            }
        }

        //filter course
        private void FilterCourse()
        {
            // Get the selected course from the ComboBox
            string? selectedCourse = courseFilterCombo.SelectedItem?.ToString()?.Trim();
            ObservableCollection<SectionModel> source = _allSections;
            if (source is IEnumerable<SectionModel> sections)
            {
                var filteredSections = sections.Where(section => section.Course == selectedCourse).ToList();
                allSectionDataGrid.ItemsSource = filteredSections;
                // Update the DataGrid with the filtered sections
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (filteredSections.Count > 0)
                    {
                        noSectionDisplayText.Visibility = Visibility.Collapsed;
                        allSectionDataGrid.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        noSectionDisplayText.Visibility = Visibility.Visible;
                        allSectionDataGrid.Visibility = Visibility.Collapsed;
                    }
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }
        //set the values of combobox by course
        private void GetAllCourse()
        {
            // Get all course from the database
            SectionOperations sectionOP = new();
            List<string> sections = sectionOP.GetAllCourse();
            sections.Insert(0, "All");
            // Set the ComboBox items source to the list of courses
            courseFilterCombo.ItemsSource = sections;
        }

        //get all sections by course
        private void GetAllSectionsByCourse(string course)
        {
            SectionOperations sectionOP = new();
            _allSections = sectionOP.GetAllSectionByCourse(course);
        }

        //Update DataGrid when the course is selected
        private void courseFilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string? selectedCourse = courseFilterCombo.SelectedItem?.ToString()?.Trim();
            if (string.IsNullOrEmpty(selectedCourse) || selectedCourse == "All")
            {
                SectionOperations sectionOperations = new();
                _allSections = sectionOperations.GetAllSection();
                allSectionDataGrid.ItemsSource = _allSections;
            }
            else
            {
                GetAllSectionsByCourse(selectedCourse);
                FilterCourse();
            }
        }
    }
}
