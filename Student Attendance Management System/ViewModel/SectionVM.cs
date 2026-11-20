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
    public partial class SectionVM : ObservableObject
    {
        public ObservableCollection<SectionModel> Sections { get; set; }

        [ObservableProperty]
        public ObservableCollection<SectionModel> trashSections;

        [ObservableProperty]
        public string course;

        [ObservableProperty]
        public int year_level;

        [ObservableProperty]
        public string section;

        [ObservableProperty]
        public DateTime created_at;
        public SectionVM()
        {
            GetAllSection();
            trashSections = GetTrashData();
        }

        //Get all section data
        private void GetAllSection ()
        {
            SectionOperations sectionOP = new();
            Sections = sectionOP.GetAllSection();
        }

        //Get the data from json
        private ObservableCollection<SectionModel> GetTrashData()
        {
            TrashOperations trashOps = new();
            ObservableCollection<SectionModel> sections = new();
            ObservableCollection<TrashModel> trashSections = trashOps.GetRecordedTrash("section");
            foreach (var trash in trashSections)
            {
                try
                {
                    var sectionModel = System.Text.Json.JsonSerializer.Deserialize<SectionModel>(trash.Data);

                    if (sectionModel != null)
                    {
                        sections.Add(sectionModel);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to parse archived section data.\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return sections;
        }


    }
}
