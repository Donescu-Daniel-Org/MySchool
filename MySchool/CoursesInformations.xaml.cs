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

namespace MySchool
{
    /// <summary>
    /// Interaction logic for CoursesInformations.xaml
    /// </summary>
    public partial class CoursesInformations : UserControl
    {
        Courses course;

        public void SetCourse(object c)
        {
            course = (Courses)c;

            lbNaming.Content = course.Naming;
            lbDomain.Content = course.Domain;
            lbTeacher.Content = course.Teacher;
            lbDuration.Content = course.Duration.ToString() + " minutes";
        }

        public CoursesInformations()
        {
            InitializeComponent();
        }
    }
}
