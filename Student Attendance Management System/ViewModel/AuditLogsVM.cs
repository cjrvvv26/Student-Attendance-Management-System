using CommunityToolkit.Mvvm.ComponentModel;
using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Student_Attendance_Management_System.ViewModel
{
    public partial class AuditLogsVM : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<AuditLogsModel> logs = new();
        public AuditLogsVM()
        {
            LoadAllLogs();
        }

        public void LoadAllLogs()
        {
           AuditLogsOperations auditLogsOperations = new();
           Logs = auditLogsOperations.GetAllLogs();
        }

        public void DeleteSelectedLogs(List<int> selectedItemsID)
        {
            foreach (var log in selectedItemsID)
            {
                AuditLogsOperations auditLogsOperations = new();
                auditLogsOperations.DeleteLog(log);
            }
            // Log deletion successful
            MessageBox.Show("Log is successfully deleted", "Sucessfully Deleted!", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
    }
}
