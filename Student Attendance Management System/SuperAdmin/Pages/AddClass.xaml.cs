using CommunityToolkit.Mvvm.ComponentModel;
using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    /// Interaction logic for AddClass.xaml
    /// </summary>

    //Teacher Display Class Model
    public partial class AvailableTeachers : ObservableObject
    {
        [ObservableProperty]
        public bool isSelected = false;

        [ObservableProperty]
        public int teacher_id;

        [ObservableProperty]
        public string employee_number;

        [ObservableProperty]
        public string full_name;

        [ObservableProperty]
        public int age;

        [ObservableProperty]
        public string gender;

        [ObservableProperty]
        public string department;
    }
    //Available Teachers VM
    public partial class AvailableTeachersVM : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<AvailableTeachers> availableTeachers = new();

        public AvailableTeachersVM(string department)
        {
            availableTeachers = DisplayAllTeacherByDepartment(department);
        }
        private ObservableCollection<AvailableTeachers> DisplayAllTeacherByDepartment(string department)
        {
            ObservableCollection<AvailableTeachers> availableTeachersList = new();
            ObservableCollection<TeacherModel> availableTeachersModel = new();

            TeacherOperations teacherOP = new();
            availableTeachersModel = teacherOP.GetTeacherByDepart(department);


            foreach (var teacherData in availableTeachersModel)
            {
                var teacherList = new AvailableTeachers
                {
                    Teacher_id = teacherData.Teacher_id,
                    Employee_number = teacherData.Employee_number,
                    Full_name = $"{teacherData.First_name} {teacherData.Last_name}",
                    Age = teacherData.Age,
                    Gender = teacherData.Gender,
                    Department = teacherData.Department
                };
                availableTeachersList.Add(teacherList);
            }
            return availableTeachersList;
        }
    }

    public partial class AddClass : Page
    {
        public SectionModel _section;
        public SubjectModel _subject;
        public TeacherModel _teacher;
        public AdminModel adminModel;
        public AddClass(int adminUserID)
        {
            InitializeComponent();
            AdminVM adminVM = new(adminUserID);
            adminModel = adminVM.AdminData;
            FetchCourses(); 
            GenerateSemesterSelection();
            sectionCombo.IsEnabled = false;
            semesterCombo.IsEnabled = false;
            scheduleDayCombo.IsEnabled = false;
            scheduleTimeSelect.IsEnabled = false;
            subjectCombo.IsEnabled = false;
            termCombo.IsEnabled = false;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                DisplayErrorText();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        //Method that will be called when the course combo box is changed
        private void sectionCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionOfSubjects();
            subjectCombo.SelectedIndex = 0;

            sectionCombo.IsEnabled = true;
            semesterCombo.IsEnabled = true;
            subjectCombo.IsEnabled = true;
        }

        //Method that will be called when the course combo box is changed
        private void courseCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FetchCourses();
            semesterCombo.SelectedIndex = 0;
            GenerateScheduleTimeDaySelection();
            GenerateClassTerm();
            termCombo.IsEnabled= true;
            termCombo.SelectedIndex = 0;    
            AvailableTeachersVM teachersVM = new(courseCombo.SelectedItem.ToString());
            DataContext = teachersVM;

            GetAvailableTime();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                DisplayErrorText();
            }), System.Windows.Threading.DispatcherPriority.Background);
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
        }

        private void FetchYearSectionsByCourse()
        {
            if (courseCombo.SelectedItem == null || courseCombo.SelectedItem.ToString() == "Select Course")
            {
                return; // Exit early if no valid course is selected
            }
            SectionOperations sectionOP = new();
            List<string> sections = sectionOP.GetListSections(courseCombo.SelectedItem.ToString());
            List<int> yearLevels = sectionOP.GetAllYearLevelByCourse(courseCombo.SelectedItem.ToString());

            List<string> yearSectionList = new();
            for(int i = 0; yearLevels.Count > i; i++)
            {
                yearSectionList.Add($"{yearLevels[i]}{sections[i]}");
            }
            sectionCombo.ItemsSource = yearSectionList;
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

            string actualSection = section.Remove(1,1); // Remove the first character (ex: '1A' ➝ 'A')

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
            subjectCombo.ItemsSource = null;
            if(availableSubjects.Count <= 0)
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
            subjectCodeText.Text = "";
            subjectUnitText.Text = "";
            subjectCombo.ItemsSource = availableSubjects;
        }

        //Proper UI handling in teacher's data
        private void DisplayErrorText()
        {
            if(courseCombo.SelectedItem == null &&
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
            } else
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
                addClassBtn.IsEnabled = true;

                foreach(var teacher in dataGridTeachers.ItemsSource as ObservableCollection<AvailableTeachers>)
                {
                    if (teacher != selectedRow)
                    {
                        teacher.IsSelected = false;
                    }
                }

                dataGridTeachers.Items.Refresh();
            }
        }

        //Set schedule day selections
        private void scheduleDayCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetAvailableTime();
        }

        //Set schedule time selections
        private void scheduleTimeSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
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
            scheduleTimeSelect.ItemsSource = availableTimes;
            scheduleTimeSelect.SelectedIndex = 0;
        }

        //Generate the selection value of class term
        private void GenerateClassTerm()
        {
            termCombo.ItemsSource = new List<string>()
            {
                "Prelim", "Midterm", "Finals"
            };
        }

        //Proceed to add class
        private void AddClass_Click(object sender, RoutedEventArgs e)
        {
            ClassesOperations classesOP = new();

            //Find the id of course & section by selected course & year

            string course = courseCombo.SelectedItem.ToString();
            string year = sectionCombo.SelectedItem.ToString().Remove(1, 1);

            SectionOperations sectionOP = new();
            SectionModel selectedSectionData = sectionOP.GetAllSectionByCourseAndYearLevel(course, Convert.ToInt32(year));

            //Find the id of subject by selected subject name
            string subject = subjectCombo.SelectedItem.ToString();

            SubjectOperations subjectOP = new();
            SubjectModel selectedSubjectData = subjectOP.GetSubjectByName(subject);

            //Find the id of teacher by selected teacher
            var selectedTeacher = dataGridTeachers.SelectedItem as AvailableTeachers;

            ClassesOperations classOP = new();
            var newClass = new ClassModel
            {
                Subject_id = selectedSubjectData.Subject_id,
                Teacher_id = selectedTeacher.Teacher_id,
                Section_id = selectedSectionData.Section_id,
                Class_semester = semesterCombo.SelectedItem.ToString(),
                Schedule_day = scheduleDayCombo.SelectedItem.ToString(),
                Schedule_time = scheduleTimeSelect.SelectedItem.ToString(),
                Class_term = termCombo.SelectedItem.ToString()
            };

            if (classesOP.AddClass(newClass, adminModel.User_id))
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
