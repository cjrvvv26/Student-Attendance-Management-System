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
    /// Interaction logic for ViewSectionAndSubjects.xaml
    /// </summary>
    public partial class ViewSectionAndSubjects : Page
    {
        public ViewSectionAndSubjects()
        {
            InitializeComponent();
        }

        private void AddSection_Click(object sender, RoutedEventArgs e)
        {
            //SectionSubjectContent.Navigate(new AddYearSection());
        }

        private void ViewSubjects_Click(object sender, RoutedEventArgs e)
        {
            SuperAdminLayout? mainPageAdmin = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
            if (mainPageAdmin != null)
            {
                //mainPageAdmin.SAdminContent.Navigate(new ViewSubjects());
            }
        }
    }
}
