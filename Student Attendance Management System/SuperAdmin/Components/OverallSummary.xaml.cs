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

namespace Student_Attendance_Management_System.SuperAdmin.Components
{
    /// <summary>
    /// Interaction logic for OverallSummary.xaml
    /// </summary>
    public partial class OverallSummary : UserControl
    {
        public OverallSummary()
        {
            InitializeComponent();
            TeacherOperations teachOp = new();
            StudentOperations studentOps = new();
            ClassesOperations classesOps = new();
            AttendanceOperations attendanceOps = new();
            totalStudents.Text = studentOps.CountAllStudents().ToString();
            totalClasses.Text = classesOps.CountAllClasses().ToString();
            totalDrop.Text = attendanceOps.GetAllDrop().ToString();
            totalTeachers.Text = teachOp.CountAllTeachers().ToString();
        }
    }
}
