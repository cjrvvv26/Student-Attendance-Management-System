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
using LiveCharts.Wpf;   
using LiveCharts;

namespace Student_Attendance_Management_System.SuperAdmin.Components
{
    /// <summary>
    /// Interaction logic for ClassesStatus.xaml
    /// </summary>
    public partial class ClassesStatus : UserControl
    {
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        public ClassesStatus()
        {
            InitializeComponent();

            // Create a new SeriesCollection
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Male",
                    Values = new ChartValues<double> { 50, 60, 75, 88 },
                    Fill = (SolidColorBrush)FindResource("DefaultPrimary")
                }
            };

            SeriesCollection.Add(new ColumnSeries
            {
                Title = "Female",
                Values = new ChartValues<double> { 20, 30, 25, 12 },
                Fill = (SolidColorBrush)FindResource("DefaultDarkGray")
            });

            Labels = new[] { "BSIT", "BEed", "BT-AUTO", "BTLEd" };

            DataContext = this;
        }
    }
}
