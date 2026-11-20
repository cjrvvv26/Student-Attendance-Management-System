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
    public partial class SubjectVM : ObservableObject
    {
        public ObservableCollection<SubjectModel> Subjects { get; set; }

        [ObservableProperty]
        public ObservableCollection<SubjectModel> trashSubjects;

        [ObservableProperty]
        public string subject_code;

        [ObservableProperty]
        public string subject_name;

        [ObservableProperty]
        public string subject_semester;

        [ObservableProperty]
        public string course_availability;

        [ObservableProperty]
        public string year_availability;

        [ObservableProperty]
        public int units;


        public SubjectVM()
        {
            GetAllSubjectData();
            TrashSubjects = GetAllTrashSubjects();
        }

        private void GetAllSubjectData()
        {
            SubjectOperations subjectOP = new();
            Subjects = subjectOP.GetAllSubjects();
        }
        //add subject data
        public bool AddSubjectData(int adminUserID)
        {
            SubjectOperations subjectOP = new();
            SubjectModel subject = new()
            {
                Subject_code = Subject_code,
                Subject_name = Subject_name,
                Subject_semester = Subject_semester,
                Course_availability = Course_availability,
                Year_availability = Year_availability,
                Units = Units
            };
            if (subjectOP.AddSubject(subject, adminUserID))
            {
                return true;
            }
            return false;
        }

        //get archived subjects
        public ObservableCollection<SubjectModel> GetAllTrashSubjects()
        {
            TrashOperations trashOps = new();
            ObservableCollection<SubjectModel> subjects = new();
            ObservableCollection<TrashModel> trashSubjects = trashOps.GetRecordedTrash("subject");
            foreach (var trash in trashSubjects)
            {
                try
                {
                    var subjectModel = System.Text.Json.JsonSerializer.Deserialize<SubjectModel>(trash.Data);
                    if (subjectModel != null)
                    {
                        subjects.Add(subjectModel);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to parse archived class data.\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return subjects;
        }
    }
}
