using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Navigation;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
namespace Student_Attendance_Management_System.ViewModel
{
    public partial class StudentVM : ObservableObject
    {
        public ObservableCollection<StudentModel> Students = new();

        public ObservableCollection<StudentModel> trashStudents = new();

        [ObservableProperty]
        public ObservableCollection<StudentModel> enrolledStudents = new();

        [ObservableProperty]
        public ObservableCollection<StudentModel> existingStudentRecord = new();

        [ObservableProperty]
        private string firstName;

        [ObservableProperty]
        private string middleName;

        [ObservableProperty]
        private string lastName;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private DateTime? birth_date;

        [ObservableProperty]
        private int student_age;

        [ObservableProperty]
        private string gender;

        [ObservableProperty]
        private string course;

        [ObservableProperty]
        private string year_level;
        public StudentVM()
        {
            Students = new ObservableCollection<StudentModel>();
            trashStudents = GetTrashData();
        }

        public bool AddStudent(StudentModel newStudent)
        {
            if (string.IsNullOrWhiteSpace(newStudent.First_name) ||
                string.IsNullOrWhiteSpace(newStudent.Middle_name) ||
                string.IsNullOrWhiteSpace(newStudent.Last_name) ||
                string.IsNullOrWhiteSpace(newStudent.Email) ||
                string.IsNullOrWhiteSpace(Gender) ||
                string.IsNullOrWhiteSpace(Convert.ToString(newStudent.Section_id))||
                Birth_date == null)
            {
                MessageBox.Show($"All fields areas required.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!Regex.IsMatch(newStudent.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (Student_age < 15 || Student_age > 50)
            {
                MessageBox.Show("Student must be 15 years old above and 50 years old below.");
                return false;
            }

            StudentModel student = new StudentModel
            {
                First_name = newStudent.First_name,
                Middle_name = newStudent.Middle_name,
                Last_name = newStudent.Last_name,
                Email = newStudent.Email,
                Birth_date = Birth_date.Value,
                Age = Student_age,
                Gender = Gender,
                Password = Birth_date.Value.ToString("MMddyyyy"),
                Role = "Student",
                Section_id = Convert.ToInt32(newStudent.Section_id)
            };
            if(StudentOperations.AddStudent(student))
            {
                Students.Add(student);
                return true;
            }
            return false;
        }

        partial void OnBirth_dateChanged(DateTime? value)
        {
            calculateAge();
        }
        private void calculateAge()
        {
            if (Birth_date.HasValue)
            {
                DateTime currentDate = DateTime.Today;
                int age = currentDate.Year - Birth_date.Value.Year;
                if (Birth_date.Value > currentDate.AddYears(-age))
                {
                    age--;
                }
                Student_age = age;
                OnPropertyChanged(nameof(Student_age));
            }
        }

        //Get the data from json
        private ObservableCollection<StudentModel> GetTrashData()
        {
            TrashOperations trashOps = new();
            ObservableCollection<StudentModel> students = new();
            ObservableCollection<TrashModel> trashSections = trashOps.GetRecordedTrash("student");
            foreach (var trash in trashSections)
            {
                try
                {
                    var studentModel = System.Text.Json.JsonSerializer.Deserialize<StudentModel>(trash.Data);

                    if (studentModel != null)
                    {
                        students.Add(studentModel);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to parse archived student data.\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return students;
        }

        //Get all students data by enrolled class
        public ObservableCollection<StudentModel> GetAllEnrolledStudentsByClassID(int classID)
        {
            AttendanceOperations attendanceOps = new AttendanceOperations();
            return EnrolledStudents = attendanceOps.GetAllEnrolledStudentsByClassID(classID);
        }
        //Get all existing student record 
        public ObservableCollection<StudentModel> GetAllExistingStudentsByClassID(int classID, DateTime attendanceDate)
        {
            AttendanceOperations attendanceOps = new AttendanceOperations();
            return EnrolledStudents = attendanceOps.GetExistingStudentsByClassID(classID, attendanceDate);
        }
    }
}
