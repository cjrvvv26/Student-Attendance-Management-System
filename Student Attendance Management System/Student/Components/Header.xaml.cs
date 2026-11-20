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

namespace Student_Attendance_Management_System.Student.Components
{
    /// <summary>
    /// Interaction logic for Header.xaml
    /// </summary>
    public partial class Header : UserControl
    {
        private StudentModel _student;
        public Header()
        {
            InitializeComponent();
        }

        public void SetStudentData(StudentModel student)
        {
            _student = student;
            full_name.Text = _student.First_name + " " + _student.Last_name;
            role.Text = _student.Role;
        }
    }
}
