using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
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
using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;

namespace Student_Attendance_Management_System.SuperAdmin.Components
{
    /// <summary>
    /// Interaction logic for ClassStatusLineGraph.xaml
    /// </summary>
    public partial class ClassStatusLineGraph : UserControl
    {
        public SeriesCollection SeriesCollection { get; set; }
        public List<Axis> LabelAxes { get; set; }
        public List<Axis> YAxis { get; set; }
        public ClassStatusLineGraph()
        {
            InitializeComponent();

            AttendanceOperations attendOp = new();
            List<AttendanceModel> performanceList = attendOp.GetGenderPerformanceOverall();
            // Get distinct months in use
            var usedMonths = performanceList
                .Select(p => p.Two_month)
                .Distinct()
                .OrderBy(m => m)
                .ToList();

            var monthLabels = usedMonths
                .Select(m => System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(m))
                .ToArray();

            var maleData = usedMonths
                .Select(m => performanceList.FirstOrDefault(p => p.Two_gender == "Male" && p.Two_month == m)?.Performance_percentage ?? 0)
                .ToList();

            var femaleData = usedMonths
                .Select(m => performanceList.FirstOrDefault(p => p.Two_gender == "Female" && p.Two_month == m)?.Performance_percentage ?? 0)
                .ToList();

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Male",
                    Values = new ChartValues<double>(maleData),
                    Stroke = (Brush)new BrushConverter().ConvertFromString("#ba36db"),
                    Fill = Brushes.Transparent
                },
                new LineSeries
                {
                    Title = "Female",
                    Values = new ChartValues<double>(femaleData),
                    Stroke = (Brush)new BrushConverter().ConvertFromString("#404040"),
                    Fill = (Brush)new BrushConverter().ConvertFromString("#40404033")
                }
            };

            // Properly aligned X-axis
            lineChart.AxisX.Clear();
            lineChart.AxisX.Add(new Axis
            {
                Title = "Month",
                Labels = monthLabels
            });

            lineChart.Series = SeriesCollection;
        }
    }
}
