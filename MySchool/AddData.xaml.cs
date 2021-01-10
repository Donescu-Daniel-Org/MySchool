using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace MySchool
{
    /// <summary>
    /// Interaction logic for AddData.xaml
    /// </summary>
    public partial class AddData : Window
    {
        public bool canUpdate;

        List<int> studentIDs;
        List<int> coursesIDs;

        public AddData()
        {
            InitializeComponent();

            canUpdate = false;
            UpdateData();
        }

        public void UpdateData()
        {
            DataTable dbStudents = Students.RetrieveAllStudentsDT();
            gridStudents.DataContext = dbStudents.DefaultView;

            DataTable dbCourses = Courses.RetrieveAllCoursesDT();
            gridCourses.DataContext = dbCourses.DefaultView;

            studentIDs = new List<int>();
            foreach(DataRow dr in dbStudents.Rows)
            {
                studentIDs.Add((int)dr[Students.STUDENT_ID]);
            }

            coursesIDs = new List<int>();
            foreach(DataRow dr in dbCourses.Rows)
            {
                coursesIDs.Add((int)dr[Courses.COURSE_ID]);
            }
        }

        private void OnClickOK(object sender, RoutedEventArgs e)
        {
            if (VerifyStudentsGrid() && VerifyCoursesGrid())
            {
                canUpdate = true;

                UpdateAllData();

                this.Close();
            }
            else
                MessageBox.Show("There are one or more errors on tables. Please verify!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private bool VerifyStudentsGrid()
        {
            DataTable dt = new DataTable();

            try
            {
                dt = ((DataView)gridStudents.ItemsSource).ToTable();
            }catch(Exception e)
            {
                return false;
            }

            foreach(DataRow dr in dt.Rows)
            {
                foreach(DataColumn dc in dt.Columns)
                {
                    string colName = dc.ColumnName;
                    string cellData = dr[colName].ToString();

                    if (String.IsNullOrEmpty(cellData))
                        return false;

                    if(colName == Students.STUDENT_BIRTHDATE)
                    {
                        DateTime birthdate = DateTime.Parse(cellData);
                        if (!Students.ValidateBirthdate(birthdate))
                            return false;
                    }
                    else if(colName == Students.STUDENT_SERIALNUMBER)
                    {
                        if (!Students.ValidateSerialNumber(cellData))
                            return false;
                    }
                    else if(colName == Students.STUDENT_EMAILADDRESS)
                    {
                        if (!Students.ValidateEmaiilAddress(cellData))
                            return false;
                    }
                }
            }

            return true;
        }

        private bool VerifyCoursesGrid()
        {
            DataTable dt = new DataTable();

            try
            {
                dt = ((DataView)gridCourses.ItemsSource).ToTable();
            }
            catch (Exception e)
            {
                return false;
            }

            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    string colName = dc.ColumnName;
                    string cellData = dr[colName].ToString();

                    if (String.IsNullOrEmpty(cellData))
                        return false;

                    if (colName == Courses.COURSE_DURATION)
                    {
                        int duration = int.Parse(cellData);
                        if (!Courses.ValidateDuration(duration))
                            return false;
                    }
                }
            }

            return true;
        }

        private void OnClickCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UpdateAllData()
        {
            DataTable dtStudents = new DataTable();
            DataTable dtCourses = new DataTable();

            dtStudents = ((DataView)gridStudents.ItemsSource).ToTable();
            dtCourses = ((DataView)gridCourses.ItemsSource).ToTable();

            Students.UpdateAllData(dtStudents);


            List<int> ids = new List<int>();
            foreach (DataRow dr in dtStudents.Rows)
            {
                ids.Add((int)dr[Students.STUDENT_ID]);
            }

            List<int> idsToDelete = new List<int>();

            foreach(int id in studentIDs)
            {
                bool add = true;
                foreach(int id1 in ids)
                {
                    if (id == id1)
                    {
                        add = false;
                        break;
                    }
                }

                if (add)
                    idsToDelete.Add(id);
            }

            Students.DeleteStudents(idsToDelete);

            ///////////////////// Courses
            ///

            Courses.UpdateAllData(dtCourses);
            ids.Clear();

            foreach (DataRow dr in dtCourses.Rows)
            {
                ids.Add((int)dr[Courses.COURSE_ID]);
            }

            idsToDelete.Clear();

            foreach (int id in coursesIDs)
            {
                bool add = true;
                foreach (int id1 in ids)
                {
                    if (id == id1)
                    {
                        add = false;
                        break;
                    }
                }

                if (add)
                    idsToDelete.Add(id);
            }

            Courses.DeleteCourses(idsToDelete);
        }
    }
}
