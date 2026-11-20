using CommunityToolkit.Mvvm.ComponentModel;
using MySql.Data.MySqlClient;
using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.Teacher.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Student_Attendance_Management_System.ViewModel
{
    public partial class ClassesVM : ObservableObject
    {
        public ObservableCollection<ClassModel> _classes { get; set; }

        [ObservableProperty]
        public ObservableCollection<ClassModel> trashClasses;

        [ObservableProperty]
        public ObservableCollection<ClassModel> teacherAvailableClasses;

        [ObservableProperty]
        public ObservableCollection<ClassModel> teacherAllClasses;

        [ObservableProperty]
        public ObservableCollection<ClassModel> studentAvailableClasses;

        [ObservableProperty]
        public ObservableCollection<ClassModel> studentAllClasses;

        public ClassesVM()
        {
            _classes = GetAllClass();
            trashClasses = GetAllTrashClasses();
            teacherAllClasses = new();
            teacherAvailableClasses = new();
            studentAllClasses = new();
            studentAvailableClasses = new();
        }

        public ObservableCollection<ClassModel> GetAllClass()
        {
            ClassesOperations classOp = new();
            return classOp.GetAllClass();
        }

        public ClassModel GetClassById(int id)
        {
            ClassesOperations classOp = new();
            return classOp.GetClassById(id);
        }

        public ObservableCollection<ClassModel> GetAllTrashClasses()
        {
            TrashOperations trashOps = new();
            ObservableCollection<ClassModel> classes = new();
            ObservableCollection<TrashModel> trashClasses = trashOps.GetRecordedTrash("class");
            foreach (var trash in trashClasses)
            {
                try
                {
                    var classModel = System.Text.Json.JsonSerializer.Deserialize<ClassModel>(trash.Data);

                    if(classModel != null)
                    {
                        classes.Add(classModel);
                    }
                }catch(Exception ex)
                {
                    MessageBox.Show($"Failed to parse archived class data.\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return classes;
        }

        public void LoadClassesByTeacher(int teacherID)
        {
            TeacherAllClasses.Clear();
            TeacherAvailableClasses.Clear();

            ClassesOperations classOp = new();
            
            var allClasses = classOp.GetClassesByTeacher(teacherID);
            foreach(var item in allClasses)
            {
                TeacherAllClasses.Add(item);
            }


            var availableClasses = classOp.GetClassesByTeacherAndDay(teacherID);
            foreach (var item in availableClasses)
            {
                TeacherAvailableClasses.Add(item);
            }
        }

        public void LoadClassesByStudent(int studentID)
        {
            StudentAllClasses.Clear();
            StudentAvailableClasses.Clear();

            ClassesOperations classOp = new();

            var allClasses = classOp.GetClassesByStudent(studentID);
            foreach (var item in allClasses)
            {
                StudentAllClasses.Add(item);
            }


            var availableClasses = classOp.GetClassesByStudentAndDay(studentID);
            foreach (var item in availableClasses)
            {
                StudentAvailableClasses.Add(item);
            }
        }
    }
}
