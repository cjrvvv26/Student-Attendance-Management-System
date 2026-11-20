using CommunityToolkit.Mvvm.ComponentModel;
using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.SuperAdmin.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Student_Attendance_Management_System.ViewModel
{
    public partial class TeacherVM : ObservableObject
    {
        public ObservableCollection<TeacherModel> Teacher { get; set; }

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
        private int age;

        [ObservableProperty]
        private string phone_no;

        public TeacherVM()
        {
            Teacher = new();
        }
        public bool AddTeacher(byte[]? imgSource, string department, string Gender)
        {
            if (string.IsNullOrEmpty(FirstName) ||
                string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrEmpty(MiddleName) ||
                string.IsNullOrEmpty(Email) ||
                string.IsNullOrEmpty(Convert.ToString(Age)) ||
                string.IsNullOrEmpty(Gender) ||
                string.IsNullOrEmpty(Phone_no) || 
                string.IsNullOrEmpty(department) ||
                Birth_date == null)
            {
                MessageBox.Show($"All fields must be filled!", "Error", MessageBoxButton.OK);
                return false;
            }
            //Email Validation
            string pattern = "/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$/";
            Regex emailValidation = new Regex(pattern);

            if (emailValidation.IsMatch(Email))
            {
                MessageBox.Show("Invalid email address", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            //Phone number (PH) validation
            string numberPattern = "^\\+639\\d{9}$";
            Regex phoneValidate = new Regex(numberPattern);
            if (phoneValidate.IsMatch(Convert.ToString(Phone_no)))
            {
                MessageBox.Show("Invalid Phone number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            //Age validation

            if (Age > 80 || Age <= 0)
            {
                MessageBox.Show("Age must be less than 80, Invalid Age", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            TeacherModel teacher = new()
            {
                Profile_pic = imgSource,
                First_name = FirstName,
                Middle_name = MiddleName,
                Last_name = LastName,
                Email = Email,
                Gender = Gender,
                Age = Age,
                Phone_no = Phone_no,
                Birth_date = Birth_date,
                Password = Birth_date.Value.ToString("MMddyyyy"),
                Department = department
            };
            TeacherOperations teacherOp = new();
            if (teacherOp.AddTeacher(teacher))
            {
                Teacher.Add(teacher);
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
                Age = age;
                OnPropertyChanged(nameof(Age));
            }
        }
    }
}
