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
    /// Interaction logic for SectionTrash.xaml
    /// </summary>
    /// 
    public partial class DisplayTrashSections : ObservableObject
    {
        public ObservableCollection<SectionModel> Sections { get; set; }

        public DisplayTrashSections()
        {
            Sections = new ObservableCollection<SectionModel>();
        }
    }
    public partial class SectionTrash : Page
    {
        public AdminModel adminModel;
        public SectionTrash(int adminUserID)
        {
            InitializeComponent();
            AdminVM adminVM = new(adminUserID);
            adminModel = adminVM.AdminData;
            SectionVM sectionVm = new();
            DisplayTrashSections displayTrashSections = new();
            DataContext = displayTrashSections;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach(var section in sectionVm.TrashSections)
                {
                    SectionModel trashSectionData = new()
                    {
                        Section_id = section.Section_id,
                        Course = section.Course,
                        Year_level = section.Year_level,
                        Section = section.Section
                    };

                    displayTrashSections.Sections.Add(trashSectionData);
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void RestoreSection_Click(object sender, RoutedEventArgs e)
        {
            SectionModel selectedSection = (SectionModel)((Button)sender).Tag;

            if (MessageBox.Show("Are you sure you want to restore this section?", "Restore Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                if (selectedSection != null)
                {
                    SectionOperations sectionOp = new();
                    if (sectionOp.RestoreCourseSection(selectedSection, adminModel.User_id))
                    {
                        AuditLogsOperations auditOP = new();
                        auditOP.AddLogs(adminModel.User_id, "Section", "Restore trash section", "Restore", "Success");
                        SuperAdminLayout? mainPage = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();

                        if (mainPage != null)
                        {
                            mainPage.SAdminContent.Navigate(new ViewAllSections(adminModel.User_id));
                        }
                    }

                }
            }
        }

        private void DeleteSection_Click(object sender, RoutedEventArgs e)
        {
            SectionModel selectedSection = (SectionModel)((Button)sender).Tag;
            if (MessageBox.Show("Are you sure you want to delete this section?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (selectedSection != null)
                {
                    TrashOperations trashOP = new();
                    if (trashOP.DeletePermanentTrash(selectedSection.Section_id, "section"))
                    {
                        MessageBox.Show("Section deleted permanently", "Successfully Deleted", MessageBoxButton.OK, MessageBoxImage.Information);

                        AuditLogsOperations auditOP = new();
                        auditOP.AddLogs(adminModel.User_id, "Section", "Delete trash section", "Delete", "Success");
                        SuperAdminLayout? mainPage = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
                        if (mainPage != null)
                        {
                            mainPage.SAdminContent.Navigate(new ViewAllSections(adminModel.User_id));
                        }
                    }
                }
            }
        }
    }
}
