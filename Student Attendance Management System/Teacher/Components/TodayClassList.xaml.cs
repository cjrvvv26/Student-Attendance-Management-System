using CommunityToolkit.Mvvm.ComponentModel;
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

namespace Student_Attendance_Management_System.Teacher.Components
{
    /// <summary>
    /// Interaction logic for TodayClassList.xaml
    /// </summary>
    /// 

    public partial class TodayClassList : UserControl
    {
        public ObservableCollection<ClassModel> todayClassList { get; set; } = new();
        public TodayClassList()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void LoadClasses(TeacherModel teacherData)
        {
            ClassesOperations classOps = new();
            todayClassList = classOps.GetClassesByTeacherAndDay(teacherData.Teacher_id);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (todayClassList.Count <= 0)
                {
                    noClassText.Visibility = Visibility.Visible;
                    dataGridClassList.Visibility = Visibility.Collapsed;
                    return;
                }
                noClassText.Visibility = Visibility.Collapsed;
                dataGridClassList.Visibility = Visibility.Visible;
            }), System.Windows.Threading.DispatcherPriority.Background);

            DataContext = this;
        }

        //View class info
        private void ViewClassInfo_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
