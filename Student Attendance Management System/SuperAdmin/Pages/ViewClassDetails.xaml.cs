using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Student_Attendance_Management_System.SuperAdmin.Pages
{
    /// <summary>
    /// Interaction logic for ViewClassDetails.xaml
    /// </summary>
    public partial class ViewClassDetails : Page
    {
        public ClassModel _selectedClass { get; set; }
        public AdminModel adminModel;
        public ViewClassDetails(ClassModel selectedClass, int adminUserID)
        {
            InitializeComponent();
            AdminVM adminVM = new(adminUserID);
            adminModel = adminVM.AdminData;
            DataContext = selectedClass;
            _selectedClass = selectedClass;
            FetchCourses();
            GenerateSemesterSelection();
            sectionCombo.IsEnabled = false;
            semesterCombo.IsEnabled = false;
            scheduleDayCombo.IsEnabled = false;
            scheduleTimeSelect.IsEnabled = false;
            termCombo.IsEnabled = false;
            subjectCombo.IsEnabled = false;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                DisplayErrorText();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        //Method that will be called when the course combo box is changed
        private void courseCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FetchCourses();
            SelectionOfSubjects();
            semesterCombo.SelectedIndex = 0;
            GenerateScheduleTimeDaySelection();
            AvailableTeachersVM teachersVM = new(courseCombo.SelectedItem.ToString());
            DataContext = teachersVM;

            GetAvailableTime();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                DisplayErrorText();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        //Method that will be called when the course combo box is changed
        private void sectionCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionOfSubjects();
            GenerateSemesterSelection();
            subjectCombo.SelectedIndex = 0;
            subjectCombo.IsEnabled = true;
        }

        //Method that will be called when the semester combo box is changed
        private void semesterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GenerateSemesterSelection();
            SelectionOfSubjects();
            GenerateScheduleTimeDaySelection();
            GetAvailableTime();
            if (subjectCombo.Items.Count > 0)
            {
                subjectCombo.SelectedIndex = 0;
            }
        }

        // Generate Semester Selection
        private void GenerateSemesterSelection()
        {
            List<string> semesters = new() { "1st Semester", "2nd Semester" };
            semesterCombo.ItemsSource = semesters;
        }

        //Fetch all courses
        private void FetchCourses()
        {
            SectionOperations sectionOP = new();
            List<string> courses = sectionOP.GetAllCourse();
            courseCombo.ItemsSource = courses;
            FetchYearSectionsByCourse();
            SelectionOfSubjects();
            GenerateClassTerm();
            termCombo.IsEnabled = true;
            termCombo.SelectedIndex = 0;    
        }

        //Fetch sections by course
        private void FetchYearSectionsByCourse()
        {
            var selectedSection = sectionCombo.SelectedItem as string;

            if (courseCombo.SelectedItem == null)
                return;

            SectionOperations sectionOP = new();
            List<string> sections = sectionOP.GetListSections(courseCombo.SelectedItem.ToString());
            List<int> yearLevels = sectionOP.GetAllYearLevelByCourse(courseCombo.SelectedItem.ToString());

            List<string> yearSectionList = new();
            for (int i = 0; i < yearLevels.Count; i++)
            {
                yearSectionList.Add($"{yearLevels[i]}{sections[i]}");
            }

            sectionCombo.ItemsSource = yearSectionList;

            if (selectedSection != null && yearSectionList.Contains(selectedSection))
                sectionCombo.SelectedItem = selectedSection;
            else
                sectionCombo.SelectedIndex = 0;
        }

        //Selection of subjects
        private void subjectCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (subjectCombo.SelectedItem == null)
            {
                return; // Exit if no subject is selected
            }
            
            string selectedSubject = subjectCombo.SelectedItem.ToString();
            SubjectOperations subjectOP = new();
            SubjectModel subject = subjectOP.GetSubjectByName(selectedSubject);
            //Store the id of subject for class db
            if (subject != null)
            {
                subjectCodeText.Text = subject.Subject_code;
                subjectUnitText.Text = subject.Units.ToString();
            }
        }

        //Fetch subjects by its course, section, semester availability
        private void SelectionOfSubjects()
        {
            if (courseCombo.SelectedItem == null ||
                sectionCombo.SelectedItem == null ||
                semesterCombo.SelectedItem == null)
            {
                return; // Exit if any ComboBox has no selection
            }

            string course = courseCombo.SelectedItem.ToString();
            string section = sectionCombo.SelectedItem.ToString();
            string semester = semesterCombo.SelectedItem.ToString();

            if (section.Length < 2)
            {
                return;
            }

            string actualSection = section.Remove(1, 1); // Remove the first character (ex: '1A' ➝ 'A')

            SubjectOperations subjectOP = new();
            ObservableCollection<SubjectModel> subjects = subjectOP.GetAllSubjectsByCourse(course, actualSection, semester);
            List<string> allSubjects = new();

            foreach (var subject in subjects)
            {
                allSubjects.Add(subject.Subject_name);
            }

            string sectionText = sectionCombo.SelectedItem.ToString();
            int yearLevel = int.Parse(sectionText.Substring(0, 1));

            List<string> usedSubjects = subjectOP.GetAllSubjectIDByCourseYear(course, yearLevel);
            List<string> availableSubjects = allSubjects.Except(usedSubjects).ToList();
            availableSubjects.Add(_selectedClass.Subject_name);
            subjectCombo.ItemsSource = null;
            if (availableSubjects.Count <= 0)
            {
                subjectCombo.IsEnabled = false;
                scheduleDayCombo.IsEnabled = false;
                scheduleTimeSelect.IsEnabled = false;
                subjectCodeText.Text = "";
                subjectUnitText.Text = "";
                scheduleDayCombo.SelectedItem = "";
                scheduleTimeSelect.SelectedItem = "";
                return;
            }
            subjectCombo.IsEnabled = true;
            scheduleDayCombo.IsEnabled = true;
            scheduleTimeSelect.IsEnabled = true;
            subjectCombo.ItemsSource = availableSubjects;
        }

        //Proper UI handling in teacher's data
        private void DisplayErrorText()
        {
            if (courseCombo.SelectedItem == null &&
                semesterCombo.SelectedItem == null &&
                sectionCombo.SelectedItem == null)
            {
                noCourseSectionText.Visibility = Visibility.Visible;
                dataGridTeachers.Visibility = Visibility.Collapsed;
                noTeacherAvailableText.Visibility = Visibility.Collapsed;
            }
            else if (dataGridTeachers.Items.Count <= 0)
            {
                noCourseSectionText.Visibility = Visibility.Collapsed;
                dataGridTeachers.Visibility = Visibility.Collapsed;
                noTeacherAvailableText.Visibility = Visibility.Visible;
            }
            else
            {
                noTeacherAvailableText.Visibility = Visibility.Collapsed;
                noCourseSectionText.Visibility = Visibility.Collapsed;
                dataGridTeachers.Visibility = Visibility.Visible;
            }
        }

        //Mouse event for selected row

        private void dataGridTeachers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            var selectedRow = dataGrid.SelectedItem as AvailableTeachers;
            if (selectedRow != null)
            {
                selectedRow.IsSelected = true;
                UpdateClassBtn.IsEnabled = true;

                foreach (var teacher in dataGridTeachers.ItemsSource as ObservableCollection<AvailableTeachers>)
                {
                    if (teacher != selectedRow)
                    {
                        teacher.IsSelected = false;
                    }
                }

                dataGridTeachers.Items.Refresh();
                UpdateClassBtn.IsEnabled = true;
            }
        }

        //Set schedule day selections
        private void scheduleDayCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetAvailableTime();
        }

        //Generate schedule day selection
        private void GenerateScheduleTimeDaySelection()
        {
            if (subjectCombo.Items.Count <= 0)
            {
                scheduleDayCombo.IsEnabled = false;
                scheduleTimeSelect.IsEnabled = false;
                return;
            }

            scheduleDayCombo.IsEnabled = true;
            scheduleDayCombo.ItemsSource = new List<string>
            {
                "Monday",
                "Tuesday",
                "Wednesday",
                "Thursday",
                "Friday",
                "Saturday"
            };
            scheduleDayCombo.SelectedIndex = 0;

            scheduleTimeSelect.IsEnabled = true;

            scheduleDayCombo.SelectedIndex = 0;
        }

        //Get available time by course_id and day
        private void GetAvailableTime()
        {
            if (subjectCombo.Items.Count <= 0)
            {
                scheduleDayCombo.IsEnabled = false;
                scheduleTimeSelect.IsEnabled = false;
                return;
            }
            SectionOperations sectionOP = new();
            //Find the id of course & section by selected course & year
            if(sectionCombo.SelectedItem != null)
            {
                string sectionText = sectionCombo.SelectedItem.ToString();
                int yearLevel = int.Parse(sectionText.Substring(0, 1));

                SectionModel classID = sectionOP.GetAllSectionByCourseAndYearLevel(courseCombo.SelectedItem.ToString(), yearLevel);
                ClassesOperations classesOP = new();
                List<string> allTimes = new()
            {
                "7:00 AM - 8:30 AM",
                "8:45 AM - 10:00 AM",
                "10:15 AM - 11:30 AM",
                "12:00 PM - 1:30 PM",
                "1:45 PM - 3:00 PM",
                "3:15 PM - 4:30 PM",
                "4:45 PM - 6:00 PM"
            };

                List<string> usedTimes = classesOP.CheckClassSchedule(classID.Section_id, scheduleDayCombo.SelectedItem.ToString());

                List<string> availableTimes = allTimes.Except(usedTimes).ToList();
                availableTimes.Add(_selectedClass.Schedule_time);
                scheduleTimeSelect.ItemsSource = availableTimes;
                scheduleTimeSelect.SelectedIndex = 0;
                return;
            }
            return;
        }

        //Generate the selection value of class term
        private void GenerateClassTerm()
        {
            termCombo.ItemsSource = new List<string>()
            {
                "Prelim", "Midterm", "Finals"
            };
        }

        //View all classes
        private void ViewAllClass_Click(object sender, RoutedEventArgs e)
        {
            SuperAdminLayout? SAMainPage = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();

            if (SAMainPage != null)
            {
                SAMainPage.SAdminContent.Navigate(new ViewAllClasses(adminModel.User_id));
            }
        }

        //Click Update Pencil Icon
        private void UpdateIcon_Click(object sender, RoutedEventArgs e)
        {
            UpdateUI();
        }

        
        private void UpdateUI()
        {
            //Hide Textblocks
            courseText.Visibility = Visibility.Collapsed;
            yearSectionText.Visibility = Visibility.Collapsed;
            semesterText.Visibility = Visibility.Collapsed;
            subjectNameText.Visibility = Visibility.Collapsed;
            schedTimeText.Visibility = Visibility.Collapsed;
            schedDayText.Visibility = Visibility.Collapsed;
            UpdateIconBtn.Visibility = Visibility.Collapsed;
            termCombo.Visibility = Visibility.Collapsed;
            teacherInfoViewBox.Visibility = Visibility.Collapsed;
            //Show components for updating
            courseCombo.Visibility = Visibility.Visible;
            sectionCombo.Visibility = Visibility.Visible;
            semesterCombo.Visibility = Visibility.Visible;
            subjectCombo.Visibility = Visibility.Visible;
            scheduleDayCombo.Visibility = Visibility.Visible;
            termCombo.Visibility = Visibility.Visible;
            scheduleTimeSelect.Visibility = Visibility.Visible;
            UpdateClassBtn.Visibility = Visibility.Visible;

            //Disable the course,section, and semester
            courseCombo.IsEnabled = false;
            sectionCombo.IsEnabled = false;
            semesterCombo.IsEnabled = false;

            // Set the saved values in the ComboBoxes
            FetchCourses();
            GenerateSemesterSelection();

            courseCombo.SelectedItem = _selectedClass.Course;
            sectionCombo.SelectedItem = _selectedClass.Year_section;
            semesterCombo.SelectedItem = _selectedClass.Class_semester;
            subjectCombo.SelectedItem = _selectedClass.Subject_name;
            scheduleDayCombo.SelectedItem = _selectedClass.Schedule_day;
            scheduleTimeSelect.SelectedItem = _selectedClass.Schedule_time;
            subjectCodeText.Text = _selectedClass.Subject_code;
            termCombo.SelectedItem = _selectedClass.Class_term;
            subjectUnitText.Text = _selectedClass.Subject_unit.ToString();

            //Re-reselect teacher
            teacherDataGridBox.Visibility = Visibility.Visible;

            //Enable 
            UpdateClassBtn.Visibility = Visibility.Visible;
            UpdateClassBtn.IsEnabled = false;
        }

        //Proceed Update Class
        private void UpdateClassBtn_Click(object sender, RoutedEventArgs e)
        {
            string course = courseCombo.SelectedItem.ToString();
            string section = sectionCombo.SelectedItem.ToString();
            string semester = semesterCombo.SelectedItem.ToString();
            string subjectName = subjectCombo.SelectedItem.ToString();
            string day = scheduleDayCombo.SelectedItem.ToString();
            string time = scheduleTimeSelect.SelectedItem.ToString();
            string term = termCombo.SelectedItem.ToString();

            if (string.IsNullOrEmpty(course) ||
                string.IsNullOrEmpty(section) ||
                string.IsNullOrEmpty(semester) ||
                string.IsNullOrEmpty(subjectName) ||
                string.IsNullOrEmpty(day) ||
                string.IsNullOrEmpty(term) ||
                string.IsNullOrEmpty(time) ||
                string.IsNullOrEmpty(((AvailableTeachers)dataGridTeachers.SelectedItem).Teacher_id.ToString()))
            {
                MessageBox.Show("All fields must filled", "Fill the Fields", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var updatedClass = new ClassModel
            {
                Class_id = _selectedClass.Class_id,
                Section_id = _selectedClass.Section_id,
                Class_semester = semester,
                Schedule_day = day,
                Schedule_time = time,
                Class_term = term,
                Subject_id = _selectedClass.Subject_id,
                Teacher_id = ((AvailableTeachers)dataGridTeachers.SelectedItem).Teacher_id
            };
            //update class
            ClassesOperations classesOP = new();

            if (classesOP.UpdateClass(updatedClass, adminModel.User_id))
            {
                SuperAdminLayout? SAMainPage = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
                if (SAMainPage != null)
                {
                    SAMainPage.SAdminContent.Navigate(new ViewAllClasses(adminModel.User_id));
                }
            }
        }
    }
}
