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
    /// Interaction logic for ClassTrash.xaml
    /// </summary>
    /// 

    public partial class DisplayTrashClasses : ObservableObject
    {
        public ObservableCollection<ClassModel> TrashClasses { get; set; }
        public DisplayTrashClasses()
        {
            TrashClasses = new ObservableCollection<ClassModel>();
        }
    }

    public partial class ClassTrash : Page
    {
        private AdminModel adminData { get; set; }
        public ClassTrash(int adminID)
        {
            InitializeComponent();
            //get admin data
            AdminVM adminVM = new(adminID);
            adminData = adminVM.AdminData;

            ClassesVM classesVM = new ClassesVM();
            DisplayTrashClasses displayTrashClass = new();
            DataContext = displayTrashClass;
            TeacherOperations teacherOP = new();
            SubjectOperations subjectOp = new();
            SectionOperations sectionOp = new();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var item in classesVM.TrashClasses)
                {
                    ClassModel trashClass = new()
                    {
                        Class_id = item.Class_id,
                        Subject_id = item.Subject_id,
                        Section_id = item.Section_id,
                        Teacher_id = item.Teacher_id,
                        Class_semester = item.Class_semester,
                        Schedule_day = item.Schedule_day,
                        Schedule_time = item.Schedule_time,
                        Teacher_no = item.Teacher_no,
                        Teacher_name = item.Teacher_name,
                        Gender = item.Gender,
                        Age = item.Age,
                        Department = item.Department,
                        Course = item.Course,
                        Year_section = $"{item.Year_level}{item.Section}",
                        Subject_name = item.Subject_name,
                        Subject_code = item.Subject_code,
                        Subject_unit = item.Subject_unit,
                    };
                    displayTrashClass.TrashClasses.Add(trashClass);
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        //Restore Class
        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            ClassModel selectedClass = (ClassModel)((Button)sender).Tag;
            if (MessageBox.Show("Are you sure you want to restore this class?", "Restore Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                if (selectedClass != null)
                {
                    ClassesOperations classesOP = new();
                    if (classesOP.RestoreClass(selectedClass, adminData.User_id))
                    {
                        AuditLogsOperations auditOP = new();
                        auditOP.AddLogs(adminData.User_id, "Class", "Restore trash class", "Restore", "Success");
                        SuperAdminLayout? mainPage = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();

                        if (mainPage != null)
                        {
                            mainPage.SAdminContent.Navigate(new ViewAllClasses(adminData.User_id));
                        }
                    }
                }
            }
        }

        //Permanently delete the class
        private void DeleteClass_Click(object sender, RoutedEventArgs e)
        {
            ClassModel selectedClass = (ClassModel)((Button)sender).Tag;
            if (MessageBox.Show("Are you sure you want to delete this class?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (selectedClass != null)
                {
                    TrashOperations trashOP = new();
                    if (trashOP.DeletePermanentTrash(selectedClass.Class_id, "class"))
                    {
                        MessageBox.Show("Class deleted permanently", "Successfully Deleted", MessageBoxButton.OK, MessageBoxImage.Information);

                        AuditLogsOperations auditOP = new();
                        auditOP.AddLogs(adminData.User_id, "Class", "Delete trash class", "Delete", "Success");
                        SuperAdminLayout? mainPage = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
                        if (mainPage != null)
                        {
                            mainPage.SAdminContent.Navigate(new ViewAllClasses(adminData.User_id));
                        }
                    }
                }
            }
        }
    }
}
