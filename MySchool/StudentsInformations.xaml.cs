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
    /// Interaction logic for StudentsInformations.xaml
    /// </summary>
    public partial class StudentsInformations : UserControl
    {
        Students student;

        public void SetStutent(object s)
        {
            this.student = (Students)s;

            lbFirstName.Content = student.FirstName;
            lbLastName.Content = student.LastName;
            lbBirthdate.Content = student.Birthdate.ToString("yyyy-MM-dd");
            lbAge.Content = DateTime.Now.Year - student.Birthdate.Year;
            lbSerialNumber.Content = student.SerialNumber;
            lbEmailAddress.Content = student.EmailAddress;
        }

        public StudentsInformations()
        {
            InitializeComponent();
        }
    }
}
