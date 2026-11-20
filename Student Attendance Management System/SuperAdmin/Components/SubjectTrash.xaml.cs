using CommunityToolkit.Mvvm.ComponentModel;
using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.SuperAdmin.Pages;
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

namespace Student_Attendance_Management_System.SuperAdmin.Components
{
    /// <summary>
    /// Interaction logic for SubjectTrash.xaml
    /// </summary>
    /// 
    public partial class DisplayTrashSubjects : ObservableObject
    {
        public ObservableCollection<SubjectModel> TrashSubjects { get; set; }
        public DisplayTrashSubjects()
        {
            TrashSubjects = new ObservableCollection<SubjectModel>();
        }
    }

    public partial class SubjectTrash : Page
    {
        public AdminModel adminModel;
        public SubjectTrash(int adminUserID)
        {
            InitializeComponent();
            AdminVM adminVM = new(adminUserID);
            adminModel = adminVM.AdminData;
            SubjectVM subjectVm = new();
            DisplayTrashSubjects trashSubjects = new();
            DataContext = trashSubjects;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var subject in subjectVm.TrashSubjects)
                {
                    SubjectModel trashSub = new()
                    {
                        Subject_id = subject.Subject_id,
                        Subject_name = subject.Subject_name,
                        Subject_code = subject.Subject_code,
                        Course_availability = subject.Course_availability,
                        Year_availability = subject.Year_availability,
                        Subject_semester = subject.Subject_semester,
                        Year_section = $"{subject.Year_availability}{subject.Year_section}",
                        Created_at = subject.Created_at,
                    };
                    trashSubjects.TrashSubjects.Add(trashSub);
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
        // restore subject
        private void RestoreSubject_Click(object sender, RoutedEventArgs e)
        {
            SubjectModel selectedSubject = (SubjectModel)((Button)sender).Tag;
            if (MessageBox.Show("Are you sure you want to restore this subject?", "Restore Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                if (selectedSubject != null)
                    {
                        SubjectOperations subjectOP = new();
                        if (subjectOP.RestoreSubject(selectedSubject, adminModel.User_id))
                        {
                            AuditLogsOperations auditOP = new();
                            auditOP.AddLogs(adminModel.User_id, "Subject", "Restore trash subject", "Restore", "Success");
                            SuperAdminLayout? mainPage = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();

                            if (mainPage != null)
                            {
                                mainPage.SAdminContent.Navigate(new ViewSubjects(adminModel.User_id));
                            }
                        }

                    }
            }
                
        }
        // delete subject
        private void DeleteSubject_Click(object sender, RoutedEventArgs e)
        {
            SubjectModel selectedSubject = (SubjectModel)((Button)sender).Tag;
            if (MessageBox.Show("Are you sure you want to delete this subject?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (selectedSubject != null)
                {
                    TrashOperations trashOP = new();
                    if (trashOP.DeletePermanentTrash(selectedSubject.Subject_id, "subject"))
                    {
                        MessageBox.Show("Subject deleted permanently", "Successfully Deleted", MessageBoxButton.OK, MessageBoxImage.Information);

                        AuditLogsOperations auditOP = new();
                        auditOP.AddLogs(adminModel.User_id, "Subject", "Delete trash subject", "Delete", "Success");
                        SuperAdminLayout? mainPage = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
                        if (mainPage != null)
                        {
                            mainPage.SAdminContent.Navigate(new ViewSubjects(adminModel.User_id));
                        }
                    }
                }
            }
        }
    }
}
