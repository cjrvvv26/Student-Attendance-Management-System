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
    /// Interaction logic for ViewSubjects.xaml
    /// </summary>
    public partial class ViewSubjects : Page
    {
        private AdminModel adminModel;
        public ViewSubjects(int adminUserID)
        {
            InitializeComponent();
            DataContext = new SubjectVM();
            //get admin data
            AdminVM adminVM = new(adminUserID);
            adminModel = adminVM.AdminData;

            Dispatcher.BeginInvoke(new Action(() => {
                if (SubjectDataGrid.Items.Count > 0)
                {
                    noSubjectDisplay.Visibility = Visibility.Collapsed;
                    SubjectDataGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    noSubjectDisplay.Visibility = Visibility.Visible;
                    SubjectDataGrid.Visibility = Visibility.Collapsed;
                }
                //It delays the code execution until UI rendering is done.
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void AddSubject_Click(object sender, RoutedEventArgs e)
        {
            //Check if there's existing course and section
            SectionOperations sectionOP = new();
            ObservableCollection<SectionModel> allCourseSection = sectionOP.GetAllSection();
            if (allCourseSection.Count <= 0)
            {
                MessageBox.Show("Please add course and section first", "No Course and Section", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SuperAdminLayout? mainPageAdmin = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
            if (mainPageAdmin != null)
            {
                mainPageAdmin.SAdminContent.Navigate(new AddSubject(adminModel.User_id));
            }
        }

        private void ViewSubjectInfo_Click(object sender, RoutedEventArgs e)
        {
            SubjectModel subject = (SubjectModel)((Button)sender).Tag;

            // Check if the subject is null
            SubjectOperations subjectOP = new();
            SubjectModel clickedSubject = subjectOP.GetSubjectData(subject.Subject_id);
            if(clickedSubject != null)
            {
                SuperAdminLayout? mainPageAdmin = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();

                if (mainPageAdmin != null)
                {
                    mainPageAdmin.SAdminContent.Navigate(new ViewSubjectDetails(clickedSubject, adminModel.User_id));
                }
            }
        }

        private void DeleteSubject_Click(object sender, RoutedEventArgs e)
        {
            SubjectModel subject = (SubjectModel)((Button)sender).Tag;
            SubjectOperations subjectOP = new();
            if (MessageBox.Show("Are you sure you want to delete this subject?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (subjectOP.DeleteSubject(subject.Subject_id, adminModel.User_id))
                {

                    AuditLogsOperations auditOP = new();
                    auditOP.AddLogs(adminModel.User_id, "Subject", "Delete subject", "Delete", "Success");
                    SuperAdminLayout? mainPageAdmin = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();

                    if (mainPageAdmin != null)
                    {
                        mainPageAdmin.SAdminContent.Navigate(new ViewSubjects(adminModel.User_id));
                    }
                }
            }
        }
    }
}
