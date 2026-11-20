using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
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

namespace Student_Attendance_Management_System.Teacher.Components
{
    /// <summary>
    /// Interaction logic for PiePerformance.xaml
    /// </summary>
    public partial class PiePerformance : UserControl
    {
        public PiePerformance(TeacherModel teacher)
        {
            InitializeComponent();
            AttendanceOperations attendOp = new();
            performancePercentage.Value = attendOp.GetStudentPerformance(teacher.Teacher_id);
        }
    }
}
