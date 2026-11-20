using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.SuperAdmin.Components;
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
    /// Interaction logic for Trash.xaml
    /// </summary>
    public partial class Trash : Page
    {
        private int adminID { get; set; }
        public Trash(int adminUserID)
        {
            InitializeComponent();
            AdminVM adminVM = new(adminUserID);
            adminID = adminVM.AdminData.User_id;
            typeArchiveFilter.ItemsSource = new List<string>()
            {
                "Classes", "Subjects", "Course & Sections"
            };
            typeArchiveFilter.SelectedIndex = 0;
            trashTable.Navigate(new ClassTrash(adminID));
        }

        private void typeArchiveFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(typeArchiveFilter.SelectedIndex)
            {
                case 0:
                    trashTable.Navigate(new ClassTrash(adminID));
                    break;
                case 1:
                    trashTable.Navigate(new SubjectTrash(adminID));
                    break;
                case 2:
                    trashTable.Navigate(new SectionTrash(adminID));
                    break;
            }
        }
    }
}
