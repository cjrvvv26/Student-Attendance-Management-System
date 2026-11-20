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
    public partial class AttendanceVM : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<AttendanceModel> getAllSavedRecords = new();

        [ObservableProperty]
        public ObservableCollection<AttendanceModel> getOverallRecord = new();

        [ObservableProperty]
        public ClassModel classData;
        public AttendanceVM()
        {
            ClassData = new();
        }

        public ClassModel GetClassById(int classID)
        {
            ClassesOperations classOp = new();
            ClassData = classOp.GetClassById(classID);
            return ClassData;
        }

        public void LoadSavedRecords(int classID, string term)
        {
            AttendanceOperations attendanceOp = new(); 
            var records = attendanceOp.GetAllRecordSummary(classID, term);
            GetAllSavedRecords.Clear(); 
            foreach (var record in records)
            {
                GetAllSavedRecords.Add(record);
            }
        }

        public void LoadSavedRecordsAll(int classID)
        {
            AttendanceOperations attendanceOp = new();
            var records = attendanceOp.GetAllRecordSummaryClassID(classID);
            GetAllSavedRecords.Clear();
            foreach (var record in records)
            {
                GetAllSavedRecords.Add(record);
            }
        }

        public void GetOverallClassRecord(int classID)
        {
            AttendanceOperations attendanceOp = new();
            var records = attendanceOp.GetOverallOfClass(classID);
            GetOverallRecord.Clear();
            foreach (var record in records)
            {
                GetOverallRecord.Add(record);
            }
        }

        public void GetOverallClassRecordByTerm(int classID, string term)
        {
            AttendanceOperations attendanceOp = new();
            var records = attendanceOp.GetOverallClassByTerm(classID, term);
            GetOverallRecord.Clear();
            foreach (var record in records)
            {
                GetOverallRecord.Add(record);
            }
        }
    }
}
