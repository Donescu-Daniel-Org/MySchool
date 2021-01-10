using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int STUDENTS_SELECTION = 0;
        const int COURSES_SELECTION  = 1;

        CoursesInformations  ctrlCoursesInformations;
        StudentsInformations ctrlStudentsInformations;

        bool coursesInformationAdded;
        bool studentsInformationAdded;

        List<Students> students;
        List<Courses> courses;

        public MainWindow()
        {
            InitializeComponent();

            ctrlCoursesInformations  = new CoursesInformations();
            ctrlStudentsInformations = new StudentsInformations();

            coursesInformationAdded = false;
            studentsInformationAdded = false;

            RetrieveData();

            UpdateListView();
            UpdateEnrollCombobox();
            UpdateGeneralInformations();
        }

        public void RetrieveData()
        {
            students = Students.RetrieveAllStudents();
            courses = Courses.RetrieveAllCourses();
        }

        public void UpdateListView()
        {
            lbEntitiesView.Items.Clear();

            switch (cbData.SelectedIndex)
            {
                case STUDENTS_SELECTION:
                    {
                        List<string> studentsFullName = Students.RetrieveAllStudentsFullName();

                        foreach (string studentFullName in studentsFullName)
                        {
                            ListBoxItem lbi = new ListBoxItem();
                            lbi.Content = studentFullName;

                            lbEntitiesView.Items.Add(lbi);
                        }
                    }
                    break;
                case COURSES_SELECTION:
                    {
                        List<string> coursesNames = Courses.RetrieveAllCoursesNames();

                        foreach(string courseName in coursesNames)
                        {
                            ListBoxItem lbi = new ListBoxItem();
                            lbi.Content = courseName;

                            lbEntitiesView.Items.Add(lbi);
                        }
                    }
                    break;
                default:
                    {
                        Debug.Assert(false, "Update switch!");
                    }
                    break;
            }

            if (lbEntitiesView.Items.Count != 0)
                lbEntitiesView.SelectedIndex = 0;
            
        }

        public void UpdateGeneralInformations()
        {
            if (lbEntitiesView.Items.Count == 0)
                return;

            switch (cbData.SelectedIndex)
            {
                case STUDENTS_SELECTION:
                    {
                        wpGeneralInformations?.Children.Remove(ctrlCoursesInformations);
                        coursesInformationAdded = false;

                        string studentName = ((ListBoxItem)lbEntitiesView.SelectedValue).Content.ToString();

                        int id = int.Parse(studentName.Split('.')[0]);

                        Students student = Students.RetrieveStudentById(id);

                        ctrlStudentsInformations?.SetStutent(student);

                        if(!studentsInformationAdded)
                        {
                            wpGeneralInformations?.Children.Add(ctrlStudentsInformations);
                            studentsInformationAdded = true;
                        }

                        UpdateEnrollListBox();
                        UpdateEnrollCombobox();
                        if(btEnroll != null)
                            btEnroll.IsEnabled = true;
                    }
                    break;
                case COURSES_SELECTION:
                    {
                        wpGeneralInformations?.Children.Remove(ctrlStudentsInformations);
                        studentsInformationAdded = false;

                        string courseName = ((ListBoxItem)lbEntitiesView.SelectedValue).Content.ToString();
                        int id = int.Parse(courseName.Split('.')[0]);

                        Courses course = Courses.RetrieveCourseById(id);

                        ctrlCoursesInformations?.SetCourse(course);

                        if (!coursesInformationAdded)
                        {
                            wpGeneralInformations?.Children.Add(ctrlCoursesInformations);
                            coursesInformationAdded = true;
                        }

                        UpdateEnrollListBox();
                        UpdateEnrollCombobox();
                        if (btEnroll != null)
                            btEnroll.IsEnabled = false;
                    }
                    break;
                default:
                    {
                        Debug.Assert(false, "Update switch!");
                    }
                    break;
            }
        }

        #region Event Handlers
        private void OnComboboxDataSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateListView();
        }

        #endregion

        private void OnListBoxEntitiesViewsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGeneralInformations();
        }

        private void OnMenuAddDataClicked(object sender, RoutedEventArgs e)
        {
            AddData addData = new AddData();
            addData.ShowDialog();

            UpdateListView();
            UpdateEnrollCombobox();
        }

        private void UpdateEnrollListBox()
        {
            if (lbEnroll == null)
                return;

            lbEnroll.Items.Clear();

            switch (cbData.SelectedIndex)
            {
                case STUDENTS_SELECTION:
                    {
                        lbEnroll.IsEnabled = true;

                        string studentName = ((ListBoxItem)lbEntitiesView.SelectedValue).Content.ToString();

                        int id = int.Parse(studentName.Split('.')[0]);

                        List<string> coursesNames = Students.RetrieveEnrolledCoursesForStudent(id);

                        foreach (string courseName in coursesNames)
                        {
                            ListBoxItem lbi = new ListBoxItem();
                            lbi.Content = courseName;

                            lbEnroll.Items.Add(lbi);
                        }
                    }
                    break;
                case COURSES_SELECTION:
                    {
                        //disabled for now
                        lbEnroll.IsEnabled = false;
                    }
                    break;
                default:
                    {
                        Debug.Assert(false, "Update switch!");
                    }
                    break;
            }
        }

        private void UpdateEnrollCombobox()
        {
            if (cbEnroll == null)
                return;

            cbEnroll.Items.Clear();

            switch (cbData.SelectedIndex)
            {
                case STUDENTS_SELECTION:
                    {
                        cbEnroll.IsEnabled = true;

                        if (lbEntitiesView.SelectedValue == null)
                            break;

                        string studentName = ((ListBoxItem)lbEntitiesView.SelectedValue).Content.ToString();

                        int id = int.Parse(studentName.Split('.')[0]);

                        List<string> coursesNames = Courses.RetrieveRemainingCoursesForStudent(id);

                        foreach (string courseName in coursesNames)
                        {
                            cbEnroll.Items.Add(courseName);
                        }
                    }
                    break;
                case COURSES_SELECTION:
                    {
                        // disabled for now
                        cbEnroll.IsEnabled = false;
                    }
                    break;
                default:
                    {
                        Debug.Assert(false, "Update switch!");
                    }
                    break;
            }

            if (cbEnroll.Items.Count != 0)
            {
                cbEnroll.IsEnabled = true;
                cbEnroll.SelectedIndex = 0;
            }
            else
            {
                cbEnroll.IsEnabled = false;
            }
        }

        private void OnClickEnroll(object sender, RoutedEventArgs e)
        {
            if (cbEnroll.Items.Count == 0)
                return;

            switch (cbData.SelectedIndex)
            {
                case STUDENTS_SELECTION:
                    {
                        string studentName = ((ListBoxItem)lbEntitiesView.SelectedValue).Content.ToString();
                        int studentId = int.Parse(studentName.Split('.')[0]);

                        string courseName = cbEnroll.SelectedValue.ToString();
                        int courseId = int.Parse(courseName.Split('.')[0]);

                        Students.EnrollStudent(studentId, courseId);

                        UpdateEnrollListBox();
                        UpdateEnrollCombobox();
                    }
                    break;
                case COURSES_SELECTION:
                    {
                        //disabled for now
                    }
                    break;
                default:
                    {
                        Debug.Assert(false, "Update switch!");
                    }
                    break;
            }
        }

        private void OnListBoxEnrollKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Delete)
            {
                if (lbEnroll.SelectedValue == null)
                    return;

                string courseName = ((ListBoxItem)lbEnroll.SelectedValue).Content.ToString();
                int courseId = int.Parse(courseName.Split('.')[0]);

                string studentName = ((ListBoxItem)lbEntitiesView.SelectedValue).Content.ToString();
                int studentId = int.Parse(studentName.Split('.')[0]);

                lbEnroll.Items.Remove(lbEnroll.SelectedItem);

                Students.RemoveCourseForStudent(studentId, courseId);

                UpdateEnrollCombobox();
            }
        }

        private void OnMenuClickExit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
