using Student_Attendance_Management_System.Data;
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

namespace Student_Attendance_Management_System.Student.Components
{
    /// <summary>
    /// Interaction logic for OverallPerformance.xaml
    /// </summary>
    public partial class OverallPerformance : UserControl
    {
        public OverallPerformance(int studentID)
        {
            InitializeComponent();
            AttendanceOperations attendOp = new();
            performancePercentage.Value = attendOp.GetStudentOverallPerformance(studentID);
        }
    }
}
