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
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : Page
    {
        public Reports()
        {
            InitializeComponent();
            courseCombo.SelectedIndex = 0;
            termCombo.SelectedIndex = 0;
            SelectCourseCombo();
        }


        //update the class status by term
        private void courseCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectCourseCombo();
        }

        //update the class status by section
        private void sectionCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //update the class status by term
        private void termCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //Selects the course and section combo box
        private void SelectCourseCombo()
        {
            courseCombo.ItemsSource = new List<string> { "All", "BEEd", "BSIT", "BT-AUTO", "BTLEd" };
            termCombo.ItemsSource = new List<string> { "All", "Prelim", "Midterm", "Finals" };
            switch (courseCombo.SelectedItem)
            {
                case "All":
                    sectionCombo.Visibility = Visibility.Collapsed;
                    break;
                case "BEEd":
                    sectionCombo.Visibility = Visibility.Visible;
                    sectionCombo.ItemsSource = new List<string> { "1E", "2E", "3E", "4E" };
                    sectionCombo.SelectedIndex = 0;
                    break;
                case "BSIT":
                    sectionCombo.Visibility = Visibility.Visible;
                    sectionCombo.ItemsSource = new List<string> { "1D", "2D", "3D", "4D" };
                    sectionCombo.SelectedIndex = 0;
                    break;
                case "BT-AUTO":
                    sectionCombo.Visibility = Visibility.Visible;
                    sectionCombo.ItemsSource = new List<string> { "1A", "2A", "3A", "4A" };
                    sectionCombo.SelectedIndex = 0;
                    break;
                case "BTLEd":
                    sectionCombo.Visibility = Visibility.Visible;
                    sectionCombo.ItemsSource = new List<string> { "1L", "2L", "3L", "4L" };
                    sectionCombo.SelectedIndex = 0;
                    break;
            }
        }

        //Get overall class status
        private void GetAllClassStatus()
        {

        }

        //Get specified class status by course, section, and term
        private void GetClassStatusByCourseSectionTerm()
        {

        }

        //Export the class status to PDF
        private void ExportPDFClassStatus_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
