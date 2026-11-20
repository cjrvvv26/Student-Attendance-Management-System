using MySql.Data.MySqlClient;
using Student_Attendance_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Student_Attendance_Management_System.Data
{
    public class AttendanceOperations
    {
        //Get all students in that class by section ID
        public ObservableCollection<StudentModel> GetAllEnrolledStudentsByClassID(int classID)
        {
            string query = "SELECT s.student_id, s.student_number, s.first_name, s.last_name,      s.age, s.gender, s.section_id FROM students s WHERE s.section_id = (SELECT section_id FROM class WHERE class_id = @classID)";

            ObservableCollection<StudentModel> students = new();

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@classID", classID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        StudentModel studentModel = new()
                        {
                            Student_id = reader.GetInt32("student_id"),
                            Student_number = reader.GetString("student_number"),
                            First_name = reader.GetString("first_name"),
                            Last_name = reader.GetString("last_name"),
                            Full_name = $"{reader.GetString("first_name")} {reader.GetString("last_name")}",
                            Section_id = reader.GetInt32("section_id"),
                            Age = reader.GetInt32("age"),
                            Gender = reader.GetString("gender")
                        };
                        students.Add(studentModel);
                    }
                    return students;
                }
            }
        }

        public ObservableCollection<StudentModel> GetExistingStudentsByClassID(int classID, DateTime attendanceDate)
        {
            // SQL query to join students with their attendance records for the specific class and date
            string query = @"
                            SELECT 
                                s.student_id, 
                                s.student_number, 
                                s.first_name, 
                                s.last_name, 
                                s.age, 
                                s.gender, 
                                s.section_id,
                                a.status AS student_status
                            FROM students s
                            LEFT JOIN attendance a 
                                ON s.student_id = a.student_id 
                                AND DATE(a.recorded_at) = @attendanceDate
                            WHERE s.section_id = (
                                SELECT section_id 
                                FROM class 
                                WHERE class_id = @classID
                            )"; // Ensure the attendance date is considered in the query

            ObservableCollection<StudentModel> students = new();

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@classID", classID);
                cmd.Parameters.AddWithValue("@attendanceDate", attendanceDate); // Pass the attendance date as a parameter
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        StudentModel studentModel = new()
                        {
                            Student_id = reader.GetInt32("student_id"),
                            Student_number = reader.GetString("student_number"),
                            First_name = reader.GetString("first_name"),
                            Last_name = reader.GetString("last_name"),
                            Full_name = $"{reader.GetString("first_name")} {reader.GetString("last_name")}",
                            Section_id = reader.GetInt32("section_id"),
                            Age = reader.GetInt32("age"),
                            Gender = reader.GetString("gender"),
                            // Ensure that the status is correctly retrieved from the attendance table
                            Student_status = reader.IsDBNull(reader.GetOrdinal("student_status")) ? "Absent" : reader.GetString("student_status")
                        };
                        students.Add(studentModel);
                    }
                    return students;
                }
            }
        }

        //Insert attendance record
        public void AddAttendanceRecord(AttendanceModel record)
        {
            string query = "INSERT INTO attendance (student_id, class_id, attendance_term, date, status, recorded_at) VALUES (@studentID, @classID, @term, @date, @status, @recordedAt)";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@studentID", record.Student_id);
                cmd.Parameters.AddWithValue("@classID", record.Class_id);
                cmd.Parameters.AddWithValue("@term", record.Class_term);
                cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@status", record.Student_status);
                cmd.Parameters.AddWithValue("@recordedAt", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
        }

        //Update the student status if late and absent
        public void UpdateStudentStatus(AttendanceModel record)
        {
            string query = "UPDATE attendance SET status = @status WHERE student_id = @studentID";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);

                cmd.Parameters.AddWithValue("@studentID", record.Student_id);
                cmd.Parameters.AddWithValue("@status", record.Student_status);
                cmd.ExecuteNonQuery();
            }
        }

        //Auto update the student status
        public void AutoUpdateAttendanceStatus()
        {
            string updateAbsentQuery = @"
            UPDATE attendance a
            JOIN class c ON a.class_id = c.class_id
            SET a.status = 'Absent'
            WHERE a.status = 'Pending'
            AND a.recorded_at IS NULL
            AND c.schedule_day = DAYNAME(CURDATE())
            AND TIME(NOW()) >= STR_TO_DATE(SUBSTRING_INDEX(c.schedule_time, ' -', -1), '%h:%i %p');";

            string updateLateQuery = @"
                    UPDATE attendance a
                    JOIN class c ON a.class_id = c.class_id
                    SET a.status = 'Absent'
                    WHERE a.status = 'Late'
                  AND DATE(a.recorded_at) = CURDATE()
                  AND TIME(NOW()) >= STR_TO_DATE(SUBSTRING_INDEX(c.schedule_time, ' -', -1), '%h:%i %p');";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                try
                {
                    MySqlCommand cmdLate = new MySqlCommand(updateLateQuery, conn);
                    cmdLate.ExecuteNonQuery();

                    MySqlCommand cmdAbsent = new MySqlCommand(updateAbsentQuery, conn);
                    cmdAbsent.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating attendance status: " + ex.Message);
                }
            }
        }

        //Get saved record all students
        public ObservableCollection<AttendanceModel> GetAllRecordByClassIDTerm(int classID, string term)
        {
            string query = "SELECT DATE(a.recorded_at) AS attendance_date, " +
                "a.recorded_at AS recorded_at_time, s.student_id, s.gender, " +
                "CONCAT(s.first_name, \" \", s.last_name) AS full_name, " +
                "a.status FROM attendance a " +
                "JOIN students s ON a.student_id = s.student_id " +
                "WHERE a.class_id = @classID AND a.attendance_term = @term " +
                "ORDER BY a.recorded_at DESC, s.first_name";
            ObservableCollection<AttendanceModel> savedRecord = new();
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@classID", classID);
                cmd.Parameters.AddWithValue("@term", term);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AttendanceModel model = new()
                        {
                            Student_id = reader.GetInt32("student_id"),
                            Student_name = reader.GetString("full_name"),
                            Student_status = reader.GetString("status"),
                            Student_gender = reader.GetString("gender"),
                            Date = reader.GetDateTime("attendance_date").ToString(),
                            Date_time = reader.GetDateTime("recorded_at_time").ToString()
                        };
                        savedRecord.Add(model);
                    }
                    return savedRecord;
                }
            }
        }

        //Has been created an attendance already this day
        public bool HasAttendanceBeenCreatedToday(int classID)
        {
            string query = @"SELECT COUNT(*) 
                     FROM attendance 
                     WHERE class_id = @classID 
                     AND DATE(recorded_at) = CURDATE()";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@classID", classID);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        //Get the record summary for display and update by term
        public ObservableCollection<AttendanceModel> GetAllRecordSummary(int classID, string term)
        {
            string query = "SELECT DATE(a.recorded_at) AS attendance_date, " +
                "c.class_term, " +
                "COUNT(*) AS total_students, " +
                "SUM(CASE WHEN a.status = 'Present' THEN 1 ELSE 0 END) AS total_present," +
                "SUM(CASE WHEN a.status = 'Late' THEN 1 ELSE 0 END) AS total_late, " +
                "SUM(CASE WHEN a.status = 'Absent' THEN 1 ELSE 0 END) AS total_absent FROM     attendance a " +
                "JOIN class c ON a.class_id = c.class_id " +
                "WHERE a.class_id = @classID AND a.attendance_term = @term " +
                "GROUP BY DATE(a.recorded_at) " +
                "ORDER BY attendance_date DESC;";
            ObservableCollection<AttendanceModel> record = new();
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@classID", classID);
                cmd.Parameters.AddWithValue("@term", term);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AttendanceModel model = new AttendanceModel()
                        {
                            Date = reader.GetDateTime("attendance_date").ToString(),
                            Total_students = reader.GetInt32("total_students"),
                            Total_absent = reader.GetInt32("total_absent"),
                            Total_late = reader.GetInt32("total_late"),
                            Class_term = reader.GetString("class_term"),
                            Total_present = reader.GetInt32("total_present"),
                        };
                        record.Add(model);
                    }
                    return record;
                }
            }
        }

        public ObservableCollection<AttendanceModel> GetAllRecordSummaryClassID(int classID)
        {
            string query = "SELECT DATE(a.recorded_at) AS attendance_date, " +
                "c.class_term, " +
                "COUNT(*) AS total_students, " +
                "SUM(CASE WHEN a.status = 'Present' THEN 1 ELSE 0 END) AS total_present," +
                "SUM(CASE WHEN a.status = 'Late' THEN 1 ELSE 0 END) AS total_late, " +
                "SUM(CASE WHEN a.status = 'Absent' THEN 1 ELSE 0 END) AS total_absent FROM     attendance a " +
                "JOIN class c ON a.class_id = c.class_id " +
                "WHERE a.class_id = @classID " +
                "GROUP BY DATE(a.recorded_at) " +
                "ORDER BY attendance_date DESC;";
            ObservableCollection<AttendanceModel> record = new();
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@classID", classID);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AttendanceModel model = new AttendanceModel()
                        {
                            Date = reader.GetDateTime("attendance_date").ToString(),
                            Total_students = reader.GetInt32("total_students"),
                            Total_absent = reader.GetInt32("total_absent"),
                            Total_late = reader.GetInt32("total_late"),
                            Class_term = reader.GetString("class_term"),
                            Total_present = reader.GetInt32("total_present"),
                        };
                        record.Add(model);
                    }
                    return record;
                }
            }
        }

        //Get all Student record from existing record for modifications
        public ObservableCollection<AttendanceModel> GetAttendanceByClassAndDate(int classID, DateTime recordedAt)
        {
            string query = "SELECT s.student_id, s.student_name, a.status, a.recorded_at " +
                           "FROM attendance a " +
                           "JOIN student s ON a.student_id = s.student_id " +
                           "WHERE a.class_id = @classID AND DATE(a.recorded_at) = @recordedAt";

            ObservableCollection<AttendanceModel> attendanceList = new();

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@classID", classID);
                cmd.Parameters.AddWithValue("@recordedAt", recordedAt.Date); // Date only

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        attendanceList.Add(new AttendanceModel
                        {
                            Student_id = reader.GetInt32("student_id"),
                            Student_name = reader.GetString("student_name"),
                            Student_status = reader.GetString("status"),
                            Date = reader.GetDateTime("recorded_at").ToString("yyyy-MM-dd")
                        });
                    }
                }
            }
            return attendanceList;
        }

        //Update student status
        public bool UpdateAttendanceStatus(int studentID, int classID, string newStatus, DateTime attendanceDate)
        {
            string query = "UPDATE attendance SET status = @newStatus " +
                           "WHERE student_id = @studentID AND class_id = @classID AND DATE(recorded_at) = @attendanceDate";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using (MySqlCommand cmd = new(query, conn))
                {
                    cmd.Parameters.AddWithValue("@newStatus", newStatus);
                    cmd.Parameters.AddWithValue("@studentID", studentID);
                    cmd.Parameters.AddWithValue("@classID", classID);
                    cmd.Parameters.AddWithValue("@attendanceDate", attendanceDate.ToString("yyyy-MM-dd"));

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        //Check if record can still update
        public bool CanUpdateAttendance(int classId)
        {
            string query = @"SELECT COUNT(*) 
                            FROM class c 
                            WHERE c.class_id = @classId
                            AND c.schedule_day = DAYNAME(CURDATE())
                            AND TIME(NOW()) BETWEEN 
                            STR_TO_DATE(SUBSTRING_INDEX(c.schedule_time, '-', 1), '%h:%i %p') AND 
                            STR_TO_DATE(SUBSTRING_INDEX(c.schedule_time, '-', -1), '%h:%i %p');";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@classId", classId);

                    var result = cmd.ExecuteScalar();

                    return Convert.ToInt32(result) > 0;
                }
            }
        }

        //View Overrall of class record
        public ObservableCollection<AttendanceModel> GetOverallOfClass(int classID)
        {
            ObservableCollection<AttendanceModel> records = new();
            string query = @"SELECT 
                            s.student_id,
                            s.student_number,
                            s.gender,
                            CONCAT(s.first_name, ' ', s.last_name) AS full_name,
                            a.class_id,
                            a.attendance_term,
    
                            SUM(CASE WHEN a.status = 'Present' THEN 1 ELSE 0 END) AS total_present,
                            SUM(CASE WHEN a.status = 'Absent' THEN 1 ELSE 0 END) AS total_absent,
                            SUM(CASE WHEN a.status = 'Late' THEN 1 ELSE 0 END) AS total_late,
    
                            (SUM(CASE WHEN a.status = 'Absent' THEN 1 ELSE 0 END) + FLOOR(SUM(CASE WHEN a.status = 'Late' THEN 1 ELSE 0 END) / 3)) AS adjusted_absent,

                            CASE 
                                WHEN (SUM(CASE WHEN a.status = 'Absent' THEN 1 ELSE 0 END) + FLOOR(SUM(CASE WHEN a.status = 'Late' THEN 1 ELSE 0 END) / 3)) >= 6 
                                THEN 'Dropped Out'
                                ELSE 'Enrolled'
                            END AS final_status

                            FROM attendance a
                            JOIN students s ON s.student_id = a.student_id
                            WHERE a.class_id = @classID
                            GROUP BY s.student_id, a.class_id, a.attendance_term;";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@classID", classID);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AttendanceModel model = new()
                        {
                            Student_id = reader.GetInt32("student_id"),
                            Student_number = reader.GetString("student_number"),
                            Student_name = reader.GetString("full_name"),
                            Student_gender = reader.GetString("gender"),
                            Class_id = reader.GetInt32("class_id"),
                            Class_term = reader.GetString("attendance_term"),
                            Total_absent = reader.GetInt32("total_absent"),
                            Total_present = reader.GetInt32("total_present"),
                            Abjusted_absent = reader.GetInt32("adjusted_absent"),
                            Final_status = reader.GetString("final_status")
                        };
                        records.Add(model);
                    }
                    return records;
                }
            }
        }

        //View Overrall of class record by term
        public ObservableCollection<AttendanceModel> GetOverallClassByTerm(int classID, string term)
        {
            ObservableCollection<AttendanceModel> records = new();
            string query = @"SELECT 
                            s.student_id,
                            s.student_number,
                            s.gender,
                            CONCAT(s.first_name, ' ', s.last_name) AS full_name,
                            a.class_id,
                            a.attendance_term,
    
                            SUM(CASE WHEN a.status = 'Present' THEN 1 ELSE 0 END) AS total_present,
                            SUM(CASE WHEN a.status = 'Absent' THEN 1 ELSE 0 END) AS total_absent,
                            SUM(CASE WHEN a.status = 'Late' THEN 1 ELSE 0 END) AS total_late,
    
                            (SUM(CASE WHEN a.status = 'Absent' THEN 1 ELSE 0 END) + FLOOR(SUM(CASE WHEN a.status = 'Late' THEN 1 ELSE 0 END) / 3)) AS adjusted_absent,

                            CASE 
                                WHEN (SUM(CASE WHEN a.status = 'Absent' THEN 1 ELSE 0 END) + FLOOR(SUM(CASE WHEN a.status = 'Late' THEN 1 ELSE 0 END) / 3)) >= 6 
                                THEN 'Dropped Out'
                                ELSE 'Enrolled'
                            END AS final_status

                            FROM attendance a
                            JOIN students s ON s.student_id = a.student_id
                            WHERE a.class_id = @classID
                             AND a.attendance_term = @term 
                            GROUP BY s.student_id, a.class_id, a.attendance_term;";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@classID", classID);
                cmd.Parameters.AddWithValue("@term", term);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AttendanceModel model = new()
                        {
                            Student_id = reader.GetInt32("student_id"),
                            Student_number = reader.GetString("student_number"),
                            Student_gender = reader.GetString("gender"),
                            Student_name = reader.GetString("full_name"),
                            Class_id = reader.GetInt32("class_id"),
                            Class_term = reader.GetString("attendance_term"),
                            Total_absent = reader.GetInt32("total_absent"),
                            Total_present = reader.GetInt32("total_present"),
                            Abjusted_absent = reader.GetInt32("adjusted_absent"),
                            Final_status = reader.GetString("final_status")
                        };
                        records.Add(model);
                    }
                    return records;
                }
            }
        }

        //Get the total of dropouts by teacher_id
        public int GetOverallDropOuts(int teacherID)
        {
            string query = @"SELECT 
                            COUNT(*) AS total_dropouts
                            FROM (
                            SELECT 
                                s.student_id,
                                a.class_id,
                                a.attendance_term,
                                (
                                    SUM(CASE WHEN a.status = 'Absent' THEN 1 ELSE 0 END) + 
                                    FLOOR(SUM(CASE WHEN a.status = 'Late' THEN 1 ELSE 0 END) / 3)
                                ) AS adjusted_absent
                            FROM attendance a
                            JOIN students s ON s.student_id = a.student_id
                            JOIN class c ON c.class_id = a.class_id
                            WHERE c.teacher_id = @teacherID
                            GROUP BY s.student_id, a.class_id, a.attendance_term
                            HAVING (
                                SUM(CASE WHEN a.status = 'Absent' THEN 1 ELSE 0 END) + 
                                FLOOR(SUM(CASE WHEN a.status = 'Late' THEN 1 ELSE 0 END) / 3)
                            ) >= 3
                        ) AS dropouts;";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                int total = 0;
                cmd.Parameters.AddWithValue("@teacherID", teacherID);
                using MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    total = reader.GetInt32("total_dropouts");
                }
                return total;
            }
        }

        //Count all dropouts
        public int GetAllDrop()
        {
            string query = @"SELECT 
                            COUNT(*) AS total_dropouts
                            FROM (
                                SELECT 
                                    s.student_id
                                FROM attendance a
                                JOIN students s ON s.student_id = a.student_id
                                GROUP BY s.student_id
                                HAVING (
                                    SUM(CASE WHEN a.status = 'Absent' THEN 1 ELSE 0 END) + 
                                    FLOOR(SUM(CASE WHEN a.status = 'Late' THEN 1 ELSE 0 END) / 3)
                                ) >= 3
                            ) AS dropouts;
                            ";
            using MySqlConnection conn = DataHelper.connectDB();
            using MySqlCommand cmd = new(query,conn);
            using MySqlDataReader reader = cmd.ExecuteReader();
            int total = 0;
            if (reader.Read())
            {
                total = reader.GetInt32("total_dropouts");
            }
            return total;
        }

        //Count overall students from diff classes handled by specified teacher
        public int GetOverallStudents(int teacherID)
        {
            string query = @"SELECT 
                            COUNT(DISTINCT a.student_id) AS total_unique_students
                            FROM attendance a
                            JOIN class c ON c.class_id = a.class_id
                            WHERE c.teacher_id = @teacherID;
                            ";
            int total = 0;
            using MySqlConnection conn = DataHelper.connectDB();
            using MySqlCommand cmd = new(query,conn);
            cmd.Parameters.AddWithValue("@teacherID", teacherID);
            using MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                total = reader.GetInt32("total_unique_students");
            }
            return total;
        }

        //Get the percentage of student performance from multiple classes that a specified teacher handled
        public double GetStudentPerformance(int teacherID)
        {
            string query = @"SELECT 
                            ROUND(
                                (SUM(CASE WHEN a.status = 'Present' THEN 1 ELSE 0 END) * 100.0) / COUNT(*),
                                2
                            ) AS overall_performance_percentage

                        FROM attendance a
                        JOIN class c ON c.class_id = a.class_id
                        WHERE c.teacher_id = @teacherID;  
                        ";
            double total = 0;
            using MySqlConnection conn = DataHelper.connectDB();
            using MySqlCommand cmd = new(query,conn);
            cmd.Parameters.AddWithValue("@teacherID", teacherID);
            using MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                total = reader.GetDouble("overall_performance_percentage");
            }
            return total;
        }

        //Get the student gender based performance handled by teacher
        public List<AttendanceModel> GetGenderPerformance(int teacherId)
        {
            List<AttendanceModel> performanceList = new();

            using (var conn = DataHelper.connectDB())
            {
                string query = @"
                            SELECT 
                                s.gender AS two_gender,
                                MONTH(a.recorded_at) AS two_month,
                                COUNT(*) AS total_session,
                                SUM(CASE WHEN a.status = 'Present' THEN 1 ELSE 0 END) AS present_count,
                                ROUND((SUM(CASE WHEN a.status = 'Present' THEN 1 ELSE 0 END) / COUNT(*)) * 100, 2) AS performance_percentage
                            FROM attendance a
                            JOIN students s ON s.student_id = a.student_id
                            JOIN class c ON c.class_id = a.class_id
                            WHERE c.teacher_id = @teacherID
                            GROUP BY s.gender, MONTH(a.recorded_at)
                            ORDER BY s.gender, MIN(a.recorded_at);
                        ";

                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@teacherID", teacherId);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var model = new AttendanceModel
                    {
                        Two_gender = reader["two_gender"].ToString(),
                        Two_month = Convert.ToInt32(reader["two_month"]),
                        Total_session = Convert.ToInt32(reader["total_session"]),
                        Present_count = Convert.ToInt32(reader["present_count"]),
                        Performance_percentage = Convert.ToDouble(reader["performance_percentage"])
                    };

                    performanceList.Add(model);
                }
            }
            return performanceList;
        }

        //Get the student gender based performance handled by teacher
        public List<AttendanceModel> GetGenderPerformanceOverall()
        {
            List<AttendanceModel> performanceList = new();

            using (var conn = DataHelper.connectDB())
            {
                string query = @"
                            SELECT 
                                s.gender AS two_gender,
                                MONTH(a.recorded_at) AS two_month,
                                COUNT(*) AS total_session,
                                SUM(CASE WHEN a.status = 'Present' THEN 1 ELSE 0 END) AS present_count,
                                ROUND((SUM(CASE WHEN a.status = 'Present' THEN 1 ELSE 0 END) / COUNT(*)) * 100, 2) AS performance_percentage
                            FROM attendance a
                            JOIN students s ON s.student_id = a.student_id
                            JOIN class c ON c.class_id = a.class_id
                            GROUP BY s.gender, MONTH(a.recorded_at)
                            ORDER BY s.gender, MIN(a.recorded_at);
                        ";

                using var cmd = new MySqlCommand(query, conn);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var model = new AttendanceModel
                    {
                        Two_gender = reader["two_gender"].ToString(),
                        Two_month = Convert.ToInt32(reader["two_month"]),
                        Total_session = Convert.ToInt32(reader["total_session"]),
                        Present_count = Convert.ToInt32(reader["present_count"]),
                        Performance_percentage = Convert.ToDouble(reader["performance_percentage"])
                    };

                    performanceList.Add(model);
                }
            }
            return performanceList;
        }

        //Get the total present of specified student
        public int GetTotalPresent(int studentID)
        {
            string query = @"SELECT COUNT(a.status) AS total_present FROM attendance a
                            JOIN students s ON s.student_id = a.student_id
                            WHERE s.student_id = @studentID AND a.status = 'Present'";
            int total = 0;  
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query,conn);
                cmd.Parameters.AddWithValue("@studentID", studentID);
                using MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    total = reader.GetInt32("total_present");
                }
                return total;
            }
        }

        //Get the total absent of specified student
        public int GetTotalAbsent(int studentID)
        {
            string query = @"SELECT COUNT(a.status) AS total_absent FROM attendance a
                            JOIN students s ON s.student_id = a.student_id
                            WHERE s.student_id = @studentID AND a.status = 'Absent'";
            int total = 0;
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@studentID", studentID);
                using MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    total = reader.GetInt32("total_absent");
                }
                return total;
            }
        }

        //Check if the teacher already created attendance today
        public int IfTeacherAlreadyCreateRecordToday(int classID, int studentID)
        {
            string query = @"SELECT DISTINCT c.class_id, c.subject_id, s.student_id, c.schedule_day, c.schedule_time, a.attendance_id
                        FROM class c
                        JOIN attendance a ON c.class_id = a.class_id
                        JOIN students s ON s.student_id = a.student_id
                        WHERE c.class_id = @classID AND s.student_id = @studentID
                          AND c.schedule_day = DAYNAME(CURDATE())
                          AND a.status = 'Pending'
                          AND a.date = CURDATE();";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using (MySqlCommand cmd = new(query, conn))
                {
                    cmd.Parameters.AddWithValue("@classID", classID);
                    cmd.Parameters.AddWithValue("@studentID", studentID);

                    object result = cmd.ExecuteScalar();
                    return result != null ? 1 : 0;
                }
            }
        }

        //Student can modify his/her status
        public void StudentModifyStats(int classID, int studentID)
        {
            string query = @"UPDATE attendance
                            SET status = IF(TIMESTAMPDIFF(MINUTE, recorded_at, NOW()) > 15, 'Late', 'Present'),
                                recorded_at = NOW()
                            WHERE student_id = @studentID
                              AND class_id = @classID
                              AND date = CURDATE();";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query,conn);
                cmd.Parameters.AddWithValue("@classID", classID);
                cmd.Parameters.AddWithValue("@studentID", studentID);
                cmd.Parameters.AddWithValue("@newUpdate", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
        }

        //Get student performance
        public double GetStudentOverallPerformance(int studentID)
        {
            string query = @"SELECT 
                            ROUND((SUM(CASE WHEN status = 'Present' THEN 1 ELSE 0 END) / COUNT(*)) * 100, 2) AS present_percent 
                            FROM attendance
                            WHERE student_id = @studentID;";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                double result = 0;  
                using MySqlCommand cmd = new(query,conn);
                cmd.Parameters.AddWithValue("@studentID", studentID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result = reader.GetDouble("present_percent");
                    }
                }
                return result;
            }
        }
    }
}