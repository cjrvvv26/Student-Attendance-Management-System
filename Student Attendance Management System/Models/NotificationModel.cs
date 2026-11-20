using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student_Attendance_Management_System.Models
{
    public partial class NotificationModel : ObservableObject
    {
        [ObservableProperty]
        public int notification_id;

        [ObservableProperty]
        public string type;

        [ObservableProperty]
        public string title;

        [ObservableProperty] 
        public string user_id;

        [ObservableProperty]
        public string triggered_by;

        [ObservableProperty]
        public string target_role;

        [ObservableProperty]
        public string message;

        [ObservableProperty]
        public string status;

        [ObservableProperty]
        public DateTime created_at;
    }
}
