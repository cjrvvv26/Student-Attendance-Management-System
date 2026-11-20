using CommunityToolkit.Mvvm.ComponentModel;
using MySql.Data.MySqlClient;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.Student.Pages;
using Student_Attendance_Management_System.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Student_Attendance_Management_System.Data
{
    public class ClassesOperations
    {
        //Get all classes   
        public ObservableCollection<ClassModel> GetAllClass()
        {
            string query = "SELECT * FROM class c JOIN sections sec ON c.section_id = sec.section_id JOIN teachers t ON c.teacher_id = t.teacher_id JOIN subjects sub ON c.subject_id = sub.subject_id";

            try
            {
                using (MySqlConnection conn = DataHelper.connectDB())
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            ObservableCollection<ClassModel> classes = new ObservableCollection<ClassModel>();
                            while (reader.Read())
                            {
                                ClassModel classModel = new ClassModel
                                {
                                    Class_id = reader.GetInt32("class_id"),
                                    Subject_id = reader.GetInt32("subject_id"),
                                    Teacher_id = reader.GetInt32("teacher_id"),
                                    Section_id = reader.GetInt32("section_id"),
                                    Class_semester = reader.GetString("class_semester"),
                                    Schedule_day = reader.GetString("schedule_day"),
                                    Schedule_time = reader.GetString("schedule_time"),
                                    Subject_name = reader.GetString("subject_name"),
                                    Class_term = reader.GetString("class_term"),
                                    Teacher_no = reader.GetString("employee_number"),
                                    Teacher_name = reader.GetString("first_name") + " " + reader.GetString("last_name"),
                                    Gender = reader.GetString("gender"),
                                    Age = reader.GetInt32("age"),
                                    Department = reader.GetString("department"),
                                    Course = reader.GetString("course"),
                                    Year_section = reader.GetInt32("year_level").ToString() + reader.GetString("section"),
                                    Subject_code = reader.GetString("subject_code"),
                                    Subject_unit = reader.GetInt32("units"),


                                };
                                classes.Add(classModel);
                            }
                            return classes;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("An error occurred while fetching classes: ", ex.Message);
                return null;
            }
        }
        //Get class by its ID   
        public ClassModel GetClassById(int classId)
        {
            string query = "SELECT * FROM class c " +
                "JOIN sections sec ON c.section_id = sec.section_id " +
                "JOIN teachers t ON c.teacher_id = t.teacher_id " +
                "JOIN subjects sub ON c.subject_id = sub.subject_id " +
                "WHERE c.class_id = @classId";
            try
            {
                using (MySqlConnection conn = DataHelper.connectDB())
                {
                    using MySqlCommand cmd = new(query, conn);
                    cmd.Parameters.AddWithValue("@classId", classId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ClassModel classModel = new ClassModel
                            {
                                Class_id = reader.GetInt32("class_id"),
                                Subject_id = reader.GetInt32("subject_id"),
                                Teacher_id = reader.GetInt32("teacher_id"),
                                Section_id = reader.GetInt32("section_id"),
                                Class_semester = reader.GetString("class_semester"),
                                Schedule_day = reader.GetString("schedule_day"),
                                Schedule_time = reader.GetString("schedule_time"),
                                Class_term = reader.GetString("class_term"),
                                Subject_name = reader.GetString("subject_name"),
                                Teacher_no = reader.GetString("employee_number"),
                                Teacher_name = reader.GetString("first_name") + " " + reader.GetString("last_name"),
                                Gender = reader.GetString("gender"),
                                Age = reader.GetInt32("age"),
                                Department = reader.GetString("department"),
                                Course = reader.GetString("course"),
                                Year_section = reader.GetInt32("year_level").ToString() + reader.GetString("section"),
                                Subject_code = reader.GetString("subject_code"),
                                Subject_unit = reader.GetInt32("units"),
                            };
                            return classModel;
                        }
                        return null;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"An error occurred while fetching class by ID: {ex.Message}");
                return null;
            }
        }

        //Get all classes by course
        public ObservableCollection<ClassModel> GetClassByCourse(string course)
        {
            ObservableCollection<ClassModel> classes = new();
            string query = "SELECT * FROM class c JOIN sections sec ON c.section_id = sec.section_id JOIN teachers t ON c.teacher_id = t.teacher_id JOIN subjects sub ON c.subject_id = sub.subject_id WHERE sec.course = @course";
            try
            {
                using (MySqlConnection conn = DataHelper.connectDB())
                {
                    using MySqlCommand cmd = new(query, conn);
                    cmd.Parameters.AddWithValue("@course", course);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClassModel classModel = new ClassModel
                            {
                                Class_id = reader.GetInt32("class_id"),
                                Subject_id = reader.GetInt32("subject_id"),
                                Teacher_id = reader.GetInt32("teacher_id"),
                                Section_id = reader.GetInt32("section_id"),
                                Class_term = reader.GetString("class_term"),
                                Class_semester = reader.GetString("class_semester"),
                                Schedule_day = reader.GetString("schedule_day"),
                                Schedule_time = reader.GetString("schedule_time"),
                                Subject_name = reader.GetString("subject_name"),
                                Teacher_no = reader.GetString("employee_number"),
                                Teacher_name = reader.GetString("first_name") + " " + reader.GetString("last_name"),
                                Gender = reader.GetString("gender"),
                                Age = reader.GetInt32("age"),
                                Department = reader.GetString("department"),
                                Course = reader.GetString("course"),
                                Year_section = reader.GetInt32("year_level").ToString() + reader.GetString("section"),
                                Subject_code = reader.GetString("subject_code"),
                                Subject_unit = reader.GetInt32("units"),


                            };
                            classes.Add(classModel);
                        }
                        return classes;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"An error occurred while fetching class by course: {ex.Message}");
                return null;
            }
        }

        //Get all classes by course & year
        public ObservableCollection<ClassModel> GetClassByCourseYear(string course, int? year)
        {
            ObservableCollection<ClassModel> classes = new();
            string query = "SELECT * FROM class c JOIN sections sec ON c.section_id = sec.section_id JOIN teachers t ON c.teacher_id = t.teacher_id JOIN subjects sub ON c.subject_id = sub.subject_id WHERE sec.course = @course AND sec.year_level = @yearLevel";
            try
            {
                using (MySqlConnection conn = DataHelper.connectDB())
                {
                    using MySqlCommand cmd = new(query, conn);
                    cmd.Parameters.AddWithValue("@course", course);
                    cmd.Parameters.AddWithValue("@yearLevel", year);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClassModel classModel = new ClassModel
                            {
                                Class_id = reader.GetInt32("class_id"),
                                Subject_id = reader.GetInt32("subject_id"),
                                Teacher_id = reader.GetInt32("teacher_id"),
                                Class_term = reader.GetString("class_term"),
                                Section_id = reader.GetInt32("section_id"),
                                Class_semester = reader.GetString("class_semester"),
                                Schedule_day = reader.GetString("schedule_day"),
                                Schedule_time = reader.GetString("schedule_time"),
                                Subject_name = reader.GetString("subject_name"),
                                Teacher_no = reader.GetString("employee_number"),
                                Teacher_name = reader.GetString("first_name") + " " + reader.GetString("last_name"),
                                Gender = reader.GetString("gender"),
                                Age = reader.GetInt32("age"),
                                Department = reader.GetString("department"),
                                Course = reader.GetString("course"),
                                Year_section = reader.GetInt32("year_level").ToString() + reader.GetString("section"),
                                Subject_code = reader.GetString("subject_code"),
                                Subject_unit = reader.GetInt32("units"),


                            };
                            classes.Add(classModel);
                        }
                        return classes;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"An error occurred while fetching class by course: {ex.Message}");
                return null;
            }
        }

        //Get all classes by course & year & sem
        public ObservableCollection<ClassModel> GetClassByCourseYearSem(string course, int? year, string semester)
        {
            ObservableCollection<ClassModel> classes = new();
            string query = "SELECT * FROM class c JOIN sections sec ON c.section_id = sec.section_id JOIN teachers t ON c.teacher_id = t.teacher_id JOIN subjects sub ON c.subject_id = sub.subject_id WHERE sec.course = @course AND sec.year_level = @yearLevel AND c.class_semester = @classSem";
            try
            {
                using (MySqlConnection conn = DataHelper.connectDB())
                {
                    using MySqlCommand cmd = new(query, conn);
                    cmd.Parameters.AddWithValue("@course", course);
                    cmd.Parameters.AddWithValue("@yearLevel", year);
                    cmd.Parameters.AddWithValue("@classSem", semester);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ClassModel classModel = new ClassModel
                            {
                                Class_id = reader.GetInt32("class_id"),
                                Subject_id = reader.GetInt32("subject_id"),
                                Teacher_id = reader.GetInt32("teacher_id"),
                                Class_term = reader.GetString("class_term"),
                                Section_id = reader.GetInt32("section_id"),
                                Class_semester = reader.GetString("class_semester"),
                                Schedule_day = reader.GetString("schedule_day"),
                                Schedule_time = reader.GetString("schedule_time"),
                                Subject_name = reader.GetString("subject_name"),
                                Teacher_no = reader.GetString("employee_number"),
                                Teacher_name = reader.GetString("first_name") + " " + reader.GetString("last_name"),
                                Gender = reader.GetString("gender"),
                                Age = reader.GetInt32("age"),
                                Department = reader.GetString("department"),
                                Course = reader.GetString("course"),
                                Year_section = reader.GetInt32("year_level").ToString() + reader.GetString("section"),
                                Subject_code = reader.GetString("subject_code"),
                                Subject_unit = reader.GetInt32("units"),


                            };
                            classes.Add(classModel);
                        }
                        return classes;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"An error occurred while fetching class by course: {ex.Message}");
                return null;
            }
        }

        //Get all classes by course & sem
        public ObservableCollection<ClassModel> GetClassByCourseSem(string course, string semester)
        {
            ObservableCollection<ClassModel> classes = new();
            string query = "SELECT * FROM class c JOIN sections sec ON c.section_id = sec.section_id JOIN teachers t ON c.teacher_id = t.teacher_id JOIN subjects sub ON c.subject_id = sub.subject_id WHERE sec.course = @course AND c.class_semester = @classSem";
            try
            {
                using (MySqlConnection conn = DataHelper.connectDB())
                {
                    using MySqlCommand cmd = new(query, conn);
                    cmd.Parameters.AddWithValue("@course", course);
                    cmd.Parameters.AddWithValue("@classSem", semester);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ClassModel classModel = new ClassModel
                            {
                                Class_id = reader.GetInt32("class_id"),
                                Subject_id = reader.GetInt32("subject_id"),
                                Teacher_id = reader.GetInt32("teacher_id"),
                                Section_id = reader.GetInt32("section_id"),
                                Class_semester = reader.GetString("class_semester"),
                                Class_term = reader.GetString("class_term"),
                                Schedule_day = reader.GetString("schedule_day"),
                                Schedule_time = reader.GetString("schedule_time"),
                                Subject_name = reader.GetString("subject_name"),
                                Teacher_no = reader.GetString("employee_number"),
                                Teacher_name = reader.GetString("first_name") + " " + reader.GetString("last_name"),
                                Gender = reader.GetString("gender"),
                                Age = reader.GetInt32("age"),
                                Department = reader.GetString("department"),
                                Course = reader.GetString("course"),
                                Year_section = reader.GetInt32("year_level").ToString() + reader.GetString("section"),
                                Subject_code = reader.GetString("subject_code"),
                                Subject_unit = reader.GetInt32("units"),


                            };
                            classes.Add(classModel);
                        }
                        return classes;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"An error occurred while fetching class by course: {ex.Message}");
                return null;
            }
        }

        //Get all classes by course & sem
        public ObservableCollection<ClassModel> GetClassBySem(string semester)
        {
            ObservableCollection<ClassModel> classes = new();
            string query = "SELECT * FROM class c JOIN sections sec ON c.section_id = sec.section_id JOIN teachers t ON c.teacher_id = t.teacher_id JOIN subjects sub ON c.subject_id = sub.subject_id WHERE c.class_semester = @classSem";
            try
            {
                using (MySqlConnection conn = DataHelper.connectDB())
                {
                    using MySqlCommand cmd = new(query, conn);
                    cmd.Parameters.AddWithValue("@classSem", semester);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClassModel classModel = new ClassModel
                            {
                                Class_id = reader.GetInt32("class_id"),
                                Subject_id = reader.GetInt32("subject_id"),
                                Teacher_id = reader.GetInt32("teacher_id"),
                                Section_id = reader.GetInt32("section_id"),
                                Class_semester = reader.GetString("class_semester"),
                                Class_term = reader.GetString("class_term"),
                                Schedule_day = reader.GetString("schedule_day"),
                                Schedule_time = reader.GetString("schedule_time"),
                                Subject_name = reader.GetString("subject_name"),
                                Teacher_no = reader.GetString("employee_number"),
                                Teacher_name = reader.GetString("first_name") + " " + reader.GetString("last_name"),
                                Gender = reader.GetString("gender"),
                                Age = reader.GetInt32("age"),
                                Department = reader.GetString("department"),
                                Course = reader.GetString("course"),
                                Year_section = reader.GetInt32("year_level").ToString() + reader.GetString("section"),
                                Subject_code = reader.GetString("subject_code"),
                                Subject_unit = reader.GetInt32("units"),


                            };
                            classes.Add(classModel);
                        }
                        return classes;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"An error occurred while fetching class by course: {ex.Message}");
                return null;
            }
        }

        //Check if class course & section have existing same schedule
        public List<string> CheckClassSchedule( int sectionId, string scheduleDay)
        {
            List<string> dayTime = new();
            string query = "SELECT  schedule_time FROM class WHERE section_id = @sectionID AND schedule_day = @scheduleDay";
            try
            {
                using (MySqlConnection conn = DataHelper.connectDB())
                {
                    using MySqlCommand cmd = new(query, conn);
                    cmd.Parameters.AddWithValue("@sectionId", sectionId);
                    cmd.Parameters.AddWithValue("@scheduleDay", scheduleDay);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dayTime.Add(reader.GetString("schedule_time"));
                        }
                        return dayTime;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"An error occurred while checking class schedule: {ex.Message}");
                return null;
            }
        }

        //Get class by its teacher id and schedule day
        public ObservableCollection<ClassModel> GetClassesByTeacherAndDay(int teacherID)
        {
            ObservableCollection<ClassModel> classes = new();
            string query = "SELECT * FROM class c JOIN sections sec ON c.section_id = sec.section_id " +
                "JOIN teachers t ON c.teacher_id = t.teacher_id " +
                "JOIN subjects sub ON c.subject_id = sub.subject_id " +
                "WHERE t.teacher_id = @teacherID AND c.schedule_day = DAYNAME(CURDATE())";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@teacherID", teacherID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ClassModel classModel = new ClassModel
                        {
                            Class_id = reader.GetInt32("class_id"),
                            Subject_id = reader.GetInt32("subject_id"),
                            Teacher_id = reader.GetInt32("teacher_id"),
                            Section_id = reader.GetInt32("section_id"),
                            Class_semester = reader.GetString("class_semester"),
                            Class_term = reader.GetString("class_term"),
                            Schedule_day = reader.GetString("schedule_day"),
                            Schedule_time = reader.GetString("schedule_time"),
                            Subject_name = reader.GetString("subject_name"),
                            Teacher_no = reader.GetString("employee_number"),
                            Teacher_name = reader.GetString("first_name") + " " + reader.GetString("last_name"),
                            Gender = reader.GetString("gender"),
                            Age = reader.GetInt32("age"),
                            Department = reader.GetString("department"),
                            Course = reader.GetString("course"),
                            Year_section = reader.GetInt32("year_level").ToString() + reader.GetString("section"),
                            Subject_code = reader.GetString("subject_code"),
                            Subject_unit = reader.GetInt32("units"),
                        };
                        classes.Add(classModel);
                    }
                    return classes;
                }
            }
        }

        //Get class by its student id and schedule day
        public ObservableCollection<ClassModel> GetClassesByStudentAndDay(int studentID)
        {
            ObservableCollection<ClassModel> classes = new();
            string query = @"SELECT *
                            FROM class c
                            JOIN sections sec ON c.section_id = sec.section_id
                            JOIN teachers t ON c.teacher_id = t.teacher_id
                            JOIN subjects sub ON c.subject_id = sub.subject_id
                            JOIN students s ON s.section_id = c.section_id
                            WHERE s.student_id = @studentID
                              AND c.schedule_day = DAYNAME(CURDATE())
                              AND CURTIME() BETWEEN 
                                  STR_TO_DATE(SUBSTRING_INDEX(c.schedule_time, ' - ', 1), '%l:%i %p') AND 
                                  STR_TO_DATE(SUBSTRING_INDEX(c.schedule_time, ' - ', -1), '%l:%i %p');
                            ";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@studentID", studentID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ClassModel classModel = new ClassModel
                        {
                            Class_id = reader.GetInt32("class_id"),
                            Subject_id = reader.GetInt32("subject_id"),
                            Teacher_id = reader.GetInt32("teacher_id"),
                            Section_id = reader.GetInt32("section_id"),
                            Class_semester = reader.GetString("class_semester"),
                            Class_term = reader.GetString("class_term"),
                            Schedule_day = reader.GetString("schedule_day"),
                            Schedule_time = reader.GetString("schedule_time"),
                            Subject_name = reader.GetString("subject_name"),
                            Teacher_no = reader.GetString("employee_number"),
                            Teacher_name = reader.GetString("first_name") + " " + reader.GetString("last_name"),
                            Gender = reader.GetString("gender"),
                            Age = reader.GetInt32("age"),
                            Department = reader.GetString("department"),
                            Course = reader.GetString("course"),
                            Year_section = reader.GetInt32("year_level").ToString() + reader.GetString("section"),
                            Subject_code = reader.GetString("subject_code"),
                            Subject_unit = reader.GetInt32("units"),
                        };
                        classes.Add(classModel);
                    }
                    return classes;
                }
            }
        }

        //Get all class by its teacher id
        public ObservableCollection<ClassModel> GetClassesByTeacher(int teacherID)
        {
            ObservableCollection<ClassModel> classes = new();
            string query = "SELECT * FROM class c JOIN sections sec ON c.section_id = sec.section_id " +
                "JOIN teachers t ON c.teacher_id = t.teacher_id " +
                "JOIN subjects sub ON c.subject_id = sub.subject_id " +
                "WHERE t.teacher_id = @teacherID";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@teacherID", teacherID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ClassModel classModel = new ClassModel
                        {
                            Class_id = reader.GetInt32("class_id"),
                            Subject_id = reader.GetInt32("subject_id"),
                            Teacher_id = reader.GetInt32("teacher_id"),
                            Section_id = reader.GetInt32("section_id"),
                            Class_semester = reader.GetString("class_semester"),
                            Class_term = reader.GetString("class_term"),
                            Schedule_day = reader.GetString("schedule_day"),
                            Schedule_time = reader.GetString("schedule_time"),
                            Subject_name = reader.GetString("subject_name"),
                            Teacher_no = reader.GetString("employee_number"),
                            Teacher_name = reader.GetString("first_name") + " " + reader.GetString("last_name"),
                            Gender = reader.GetString("gender"),
                            Age = reader.GetInt32("age"),
                            Department = reader.GetString("department"),
                            Course = reader.GetString("course"),
                            Year_section = reader.GetInt32("year_level").ToString() + reader.GetString("section"),
                            Subject_code = reader.GetString("subject_code"),
                            Subject_unit = reader.GetInt32("units"),
                        };
                        classes.Add(classModel);
                    }
                    return classes;
                }
            }
        }

        //Get all class by its student id
        public ObservableCollection<ClassModel> GetClassesByStudent(int studentID)
        {
            ObservableCollection<ClassModel> classes = new();
            string query = @"SELECT *
                            FROM class c
                            JOIN sections sec ON c.section_id = sec.section_id
                            JOIN teachers t ON c.teacher_id = t.teacher_id
                            JOIN subjects sub ON c.subject_id = sub.subject_id
                            JOIN students s ON s.section_id = c.section_id
                            WHERE s.student_id = @studentID;";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@studentID", studentID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ClassModel classModel = new ClassModel
                        {
                            Class_id = reader.GetInt32("class_id"),
                            Subject_id = reader.GetInt32("subject_id"),
                            Teacher_id = reader.GetInt32("teacher_id"),
                            Section_id = reader.GetInt32("section_id"),
                            Class_semester = reader.GetString("class_semester"),
                            Class_term = reader.GetString("class_term"),
                            Schedule_day = reader.GetString("schedule_day"),
                            Schedule_time = reader.GetString("schedule_time"),
                            Subject_name = reader.GetString("subject_name"),
                            Teacher_no = reader.GetString("employee_number"),
                            Teacher_name = reader.GetString("first_name") + " " + reader.GetString("last_name"),
                            Gender = reader.GetString("gender"),
                            Age = reader.GetInt32("age"),
                            Department = reader.GetString("department"),
                            Course = reader.GetString("course"),
                            Year_section = reader.GetInt32("year_level").ToString() + reader.GetString("section"),
                            Subject_code = reader.GetString("subject_code"),
                            Subject_unit = reader.GetInt32("units"),
                        };
                        classes.Add(classModel);
                    }
                    return classes;
                }
            }
        }
        //Count classes by teacher_ID
        public int CountClassByTeacher(int teacherID)
        {
            string query = @"SELECT COUNT(teacher_id) AS total_class FROM class WHERE teacher_id = @teacherID";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                int classes = 0;
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@teacherID", teacherID);
                cmd.ExecuteNonQuery();
                using MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    classes = reader.GetInt32("total_class");
                }
                return classes;
            }
        }

        //Get the on going class by its scheduled day & time
        public ObservableCollection<ClassModel> GetOnGoingClasses() 
        {
            ObservableCollection<ClassModel> classes = new();
            
            string query = "SELECT * FROM class c " +
                "JOIN sections sec ON c.section_id = sec.section_id " +
                "JOIN teachers t ON c.teacher_id = t.teacher_id " +
                "JOIN subjects sub ON c.subject_id = sub.subject_id " +
                "WHERE schedule_day = DAYNAME(CURDATE())  AND CURTIME() BETWEEN " +
                "STR_TO_DATE(SUBSTRING_INDEX(schedule_time, ' - ', 1), '%h:%i %p') AND" +
                " STR_TO_DATE(SUBSTRING_INDEX(schedule_time, ' - ', -1), '%h:%i %p');";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        ClassModel classModel = new ClassModel
                        {
                            Class_id = reader.GetInt32("class_id"),
                            Subject_id = reader.GetInt32("subject_id"),
                            Teacher_id = reader.GetInt32("teacher_id"),
                            Section_id = reader.GetInt32("section_id"),
                            Class_semester = reader.GetString("class_semester"),
                            Class_term = reader.GetString("class_term"),
                            Schedule_day = reader.GetString("schedule_day"),
                            Schedule_time = reader.GetString("schedule_time"),
                            Subject_name = reader.GetString("subject_name"),
                            Teacher_no = reader.GetString("employee_number"),
                            Teacher_name = reader.GetString("first_name") + " " + reader.GetString("last_name"),
                            Gender = reader.GetString("gender"),
                            Age = reader.GetInt32("age"),
                            Department = reader.GetString("department"),
                            Course = reader.GetString("course"),
                            Year_section = reader.GetInt32("year_level").ToString() + reader.GetString("section"),
                            Subject_code = reader.GetString("subject_code"),
                            Subject_unit = reader.GetInt32("units"),
                        };
                        classes.Add(classModel);
                    }
                    return classes;
                }
            }
        }

        public bool VerifyIfOnGoing(int classID)
        {
            string query = "SELECT * FROM class c " +
                "JOIN sections sec ON c.section_id = sec.section_id " +
                "JOIN teachers t ON c.teacher_id = t.teacher_id " +
                "JOIN subjects sub ON c.subject_id = sub.subject_id " +
                "WHERE class_id = @classID AND schedule_day = DAYNAME(CURDATE())  AND CURTIME() BETWEEN " +
                "STR_TO_DATE(SUBSTRING_INDEX(schedule_time, ' - ', 1), '%h:%i %p') AND" +
                " STR_TO_DATE(SUBSTRING_INDEX(schedule_time, ' - ', -1), '%h:%i %p');";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@classID", classID);
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    return true;
                }
                return false;
            }
        }

        //Add class
        public bool AddClass(ClassModel classModel, int adminUserID)
        {
            string query = "INSERT INTO class (subject_id, teacher_id, section_id, class_term, class_semester, schedule_day, schedule_time, created_at, updated_at) " +
                           "VALUES (@subjectId, @teacherId, @sectionId, @term, @classSemester, @scheduleDay, @scheduleTime, @createdAt, @updatedAt)";
            try
            {
                using (MySqlConnection conn = DataHelper.connectDB())
                {
                    using MySqlCommand cmd = new(query, conn);
                    cmd.Parameters.AddWithValue("@subjectId", classModel.Subject_id);
                    cmd.Parameters.AddWithValue("@teacherId", classModel.Teacher_id);
                    cmd.Parameters.AddWithValue("@sectionId", classModel.Section_id);
                    cmd.Parameters.AddWithValue("@term", classModel.Class_term);
                    cmd.Parameters.AddWithValue("@classSemester", classModel.Class_semester);
                    cmd.Parameters.AddWithValue("@scheduleDay", classModel.Schedule_day);
                    cmd.Parameters.AddWithValue("@scheduleTime", classModel.Schedule_time);
                    cmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("A class added successfully", "Add Class Successfully", MessageBoxButton.OK, MessageBoxImage.Information);
                        NotificationOperation notificationOperation = new();
                        notificationOperation.AdminNonUserNotification(adminUserID, "Class", "", "Create class", "Super Admin");
                        return true;
                    }
                    return false;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"An error occurred while adding a class: {ex.Message}");
                return false;
            }
        }

        //Restore class
        public bool RestoreClass(ClassModel classModel, int adminUserID)
        {
            string query = "INSERT INTO class (subject_id, teacher_id, section_id, class_semester, class_term, schedule_day, schedule_time, created_at, updated_at) " +
                           "VALUES (@subjectId, @teacherId, @sectionId, @classSemester, @term, @scheduleDay, @scheduleTime, @createdAt, @updatedAt)";

            string subjectQuery = "SELECT * FROM subjects WHERE subject_id = @subID";

            //Once restored class it deletes in archive
            string archiveQuery = "DELETE FROM archive WHERE original_id = @origID";
            try
            {
                using (MySqlConnection conn = DataHelper.connectDB())
                {
                    using MySqlCommand subjCmd = new(subjectQuery, conn);
                    subjCmd.Parameters.AddWithValue("@subID", classModel.Subject_id);
                    string subjectName = "";
                    using (MySqlDataReader reader = subjCmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            subjectName = reader.GetString("subject_name");
                        }
                    }

                    using MySqlCommand cmd = new(query, conn);
                    cmd.Parameters.AddWithValue("@subjectId", classModel.Subject_id);
                    cmd.Parameters.AddWithValue("@teacherId", classModel.Teacher_id);
                    cmd.Parameters.AddWithValue("@sectionId", classModel.Section_id);
                    cmd.Parameters.AddWithValue("@classSemester", classModel.Class_semester);
                    cmd.Parameters.AddWithValue("@scheduleDay", classModel.Schedule_day);
                    cmd.Parameters.AddWithValue("@term", classModel.Class_term);
                    cmd.Parameters.AddWithValue("@scheduleTime", classModel.Schedule_time);
                    cmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);
                    cmd.ExecuteNonQuery();

                    using MySqlCommand trashCmd = new(archiveQuery, conn);
                    trashCmd.Parameters.AddWithValue("@origID", classModel.Class_id);
                    int rowsAffected = trashCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("A class restore successfully", "Restore Class Successfully", MessageBoxButton.OK, MessageBoxImage.Information);
                        NotificationOperation notificationOperation = new();
                        notificationOperation.AdminNonUserNotification(adminUserID, "Class", subjectName, "Restore class", "Super Admin");
                        return true;
                    }
                    return false;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"An error occurred while adding a class: {ex.Message}");
                return false;
            }
        }

        //Update class
        public bool UpdateClass(ClassModel classModel, int adminUserID)
        {
            try
            {
                string query = "UPDATE class SET subject_id = @subjectId, class_term = @term, teacher_id = @teacherId, section_id = @sectionId, class_semester = @classSemester, schedule_day = @scheduleDay, schedule_time = @scheduleTime, updated_at = @updatedAt WHERE class_id = @classId";
                using (MySqlConnection conn = DataHelper.connectDB())
                {
                    using MySqlCommand cmd = new(query, conn);
                    cmd.Parameters.AddWithValue("@classId", classModel.Class_id);
                    cmd.Parameters.AddWithValue("@subjectId", classModel.Subject_id);
                    cmd.Parameters.AddWithValue("@teacherId", classModel.Teacher_id);
                    cmd.Parameters.AddWithValue("@term", classModel.Class_term);
                    cmd.Parameters.AddWithValue("@sectionId", classModel.Section_id);
                    cmd.Parameters.AddWithValue("@classSemester", classModel.Class_semester);
                    cmd.Parameters.AddWithValue("@scheduleDay", classModel.Schedule_day);
                    cmd.Parameters.AddWithValue("@scheduleTime", classModel.Schedule_time);
                    cmd.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("A class updated successfully", "Update Class Successfully", MessageBoxButton.OK, MessageBoxImage.Information);
                        NotificationOperation notificationOperation = new();
                        notificationOperation.AdminNonUserNotification( adminUserID,"Class", "", "Update class", "Super Admin");
                        return true;
                    }
                    return false;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"An error occurred while updating a class: {ex.Message}");
                return false;
            }
        }

        //Delete class
        public bool DeleteClass(int classID , int adminUserID)
        {
            string query = "DELETE FROM class WHERE class_id = @class_id";
            string subjectQuery = "SELECT * FROM subjects WHERE subject_id = @subID";
            using (MySqlConnection conn = DataHelper.connectDB())
            {

                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@class_id", classID);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    NotificationOperation notificationOperation = new();
                    notificationOperation.AdminNonUserNotification( adminUserID,"Class", "", "Delete class", "Super Admin");
                    return true;
                }
                return false;
            }
        }

        //Delete All class
        public bool DeleteAllClass(int adminUserID)
        {
            string query = "DELETE FROM class";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                try
                {
                    using MySqlCommand cmd = new(query, conn);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        NotificationOperation notificationOperation = new();
                        notificationOperation.AdminNonUserNotification(adminUserID, "Class", "", "Delete class", "Super Admin");
                    }
                    return true;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("An error occurred while deleting class: ", ex.Message);
                    return false;
                }
            }
        }

        //Count all students
        public int CountAllClasses()
        {
            string query = @"SELECT COUNT(*) AS total_class FROM class;";
            using MySqlConnection conn = DataHelper.connectDB();
            using MySqlCommand cmd = new(query, conn);
            using MySqlDataReader reader = cmd.ExecuteReader();
            int count = 0;
            if (reader.Read())
            {
                count = reader.GetInt32("total_class");
            }
            return count;
        }
    }
}
