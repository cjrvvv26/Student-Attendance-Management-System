using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student_Attendance_Management_System.Models
{
    public partial class TrashModel : ObservableObject
    {
        [ObservableProperty]
        public int archive_id;

        [ObservableProperty]
        public string type;

        [ObservableProperty]
        public int original_id;

        [ObservableProperty]
        public string data;

        [ObservableProperty]
        public int? archived_by;

        [ObservableProperty]
        public DateTime archived_at;
    }
}
