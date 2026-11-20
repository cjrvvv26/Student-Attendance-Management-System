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

namespace Student_Attendance_Management_System.SuperAdmin.Components
{
    /// <summary>
    /// Interaction logic for LogsDashBoard.xaml
    /// </summary>
    public partial class LogsDashBoard : UserControl
    {
        public LogsDashBoard()
        {
            InitializeComponent();
            DataContext = new AuditLogsVM();
        }
    }
}
