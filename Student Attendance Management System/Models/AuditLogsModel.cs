using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student_Attendance_Management_System.Models
{
    public partial class AuditLogsModel : ObservableObject
    {
        [ObservableProperty]
        public int log_id;

        [ObservableProperty]
        public int user_id;

        [ObservableProperty]
        public string user_no;

        [ObservableProperty]
        public string user_name;

        [ObservableProperty]
        public string role;

        [ObservableProperty]
        public string affected_entity;

        [ObservableProperty]
        public string action;

        [ObservableProperty]
        public string action_type;

        [ObservableProperty]
        public string status;

        [ObservableProperty]
        public DateTime created_at;

        [ObservableProperty]
        public bool checkBoxChecked;
    }
}
