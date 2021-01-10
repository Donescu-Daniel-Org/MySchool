using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySchool
{
    class Students
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public string SerialNumber { get; set; }
        public string EmailAddress { get; set; }
        public bool EmailValidated { get; set; }

        const string CONN_STRING_NAME = "MySchoolDB";

        public static readonly string STUDENT_ID           = "id";
        public static readonly string STUDENT_FIRSTNAME    = "first_name";
        public static readonly string STUDENT_LASTNAME     = "last_name";
        public static readonly string STUDENT_BIRTHDATE    = "birthdate";
        public static readonly string STUDENT_SERIALNUMBER = "serial_number";
        public static readonly string STUDENT_EMAILADDRESS = "email_address";

        static readonly int SERIAL_NUMBER_LENGTH = 13;

        public Students(int id, string firstName, string lastName, DateTime birthdate, string serialNumber, string emailAddress)
        {
            this.Id           = Id;
            this.FirstName    = firstName;
            this.LastName     = lastName;
            this.Birthdate    = birthdate;
            this.SerialNumber = serialNumber;
            this.EmailAddress = emailAddress;
        }

        public Students() { }


        public static List<string> RetrieveAllStudentsFullName()
        {
            DBManager dbManager = new DBManager(CONN_STRING_NAME);

            DataTable dbTable = dbManager.ExecuteProcedure("RetrieveAllStudentsFullNames");

            List<string> fullNames = new List<string>();
            foreach(DataRow dr in dbTable.Rows)
            {
                fullNames.Add((string)dr["full_name"]);
            }

            return fullNames;
        }

        public static List<Students> RetrieveAllStudents()
        {
            DataTable dbTable = RetrieveAllStudentsDT();

            List<Students> students = new List<Students>();
            foreach(DataRow dr in dbTable.Rows)
            {
                Students student = DataRowToStudents(dr);
                students.Add(student);
            }

            return students;
        }

        public static DataTable RetrieveAllStudentsDT()
        {
            DBManager dbManager = new DBManager(CONN_STRING_NAME);

            return dbManager.ExecuteProcedure("RetrieveAllStudents");
        }

        public static Students RetrieveStudentById(int id)
        {

            DBManager dbManager = new DBManager(CONN_STRING_NAME);

            Tuple<string, object, SqlDbType> param = new Tuple<string, object, SqlDbType>("@id", id, SqlDbType.Int);

            DataTable dbTable = dbManager.ExecuteProcedure("RetrieveStudentById", param);

            if (dbTable.Rows.Count > 1)
            {
                Debug.Assert(false, "There are more than one record in database for this student name!");
                return DataRowToStudents(dbTable.Rows[0]);
            }
            else if (dbTable.Rows.Count == 0)
            {
                throw new Exception("This course name doesn't exists or an unexpected error has occured!");
            }
            else
                return DataRowToStudents(dbTable.Rows[0]);
        }

        public static bool ValidateSerialNumber(string serialNumber)
        {
            bool retVal = serialNumber.Any(x => !char.IsLetter(x));

            return (serialNumber.Length == SERIAL_NUMBER_LENGTH ? retVal : false);
        }

        public static bool ValidateBirthdate(DateTime birthdate)
        {
            return DateTime.Now.Date > birthdate.Date;
        }

        public static bool ValidateEmaiilAddress(string emailAddress)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(emailAddress);
                return addr.Address == emailAddress;
            }
            catch
            {
                return false;
            }
        }

        public static void UpdateAllData(DataTable dbTable)
        {
            DBManager dbManager = new DBManager(CONN_STRING_NAME);

            foreach (DataRow dr in dbTable.Rows)
            {
                Tuple<string, object, SqlDbType> param1 = new Tuple<string, object, SqlDbType>("@id", dr[STUDENT_ID], SqlDbType.Int);
                Tuple<string, object, SqlDbType> param2 = new Tuple<string, object, SqlDbType>("@first_name", dr[STUDENT_FIRSTNAME], SqlDbType.NVarChar);
                Tuple<string, object, SqlDbType> param3 = new Tuple<string, object, SqlDbType>("@last_name", dr[STUDENT_LASTNAME], SqlDbType.NVarChar);
                Tuple<string, object, SqlDbType> param4 = new Tuple<string, object, SqlDbType>("@birthdate", dr[STUDENT_BIRTHDATE], SqlDbType.DateTime);
                Tuple<string, object, SqlDbType> param5 = new Tuple<string, object, SqlDbType>("@serial_number", dr[STUDENT_SERIALNUMBER], SqlDbType.NVarChar);
                Tuple<string, object, SqlDbType> param6 = new Tuple<string, object, SqlDbType>("@email_address", dr[STUDENT_EMAILADDRESS], SqlDbType.NVarChar);
                
                dbManager.ExecuteNonProcedure("UpdateAtId", param1, param2, param3, param4, param5, param6);
            }
        }

        public static List<string> RetrieveEnrolledCoursesForStudent(int id)
        {
            DBManager dbManager = new DBManager(CONN_STRING_NAME);

            Tuple<string, object, SqlDbType> param1 = new Tuple<string, object, SqlDbType>("@id", id, SqlDbType.Int);

            DataTable dbTable = dbManager.ExecuteProcedure("RetrieveEnrolledCoursesForStudent", param1);

            List<string> coursesNames = new List<string>();
            foreach(DataRow dr in dbTable.Rows)
            {
                coursesNames.Add((string)dr[Courses.COURSE_NAMING]);
            }

            return coursesNames;
        }

        public static void EnrollStudent(int studentId, int courseId)
        {
            DBManager dbManager = new DBManager(CONN_STRING_NAME);

            Tuple<string, object, SqlDbType> param1 = new Tuple<string, object, SqlDbType>("@student_id", studentId, SqlDbType.Int);
            Tuple<string, object, SqlDbType> param2 = new Tuple<string, object, SqlDbType>("@course_id", courseId, SqlDbType.Int);

            dbManager.ExecuteNonProcedure("EnrollStudent", param1, param2);
        }

        public static void DeleteStudents(List<int> ids)
        {
            DBManager dbManager = new DBManager(CONN_STRING_NAME);

            foreach(int id in ids)
            {
                Tuple<string, object, SqlDbType> param1 = new Tuple<string, object, SqlDbType>("@id", id, SqlDbType.Int);
                dbManager.ExecuteNonProcedure("DeleteStudentAtId", param1);
            }
        }

        public static void RemoveCourseForStudent(int studentId, int courseId)
        {
            DBManager dbManager = new DBManager(CONN_STRING_NAME);

            Tuple<string, object, SqlDbType> param1 = new Tuple<string, object, SqlDbType>("@student_id", studentId, SqlDbType.Int);
            Tuple<string, object, SqlDbType> param2 = new Tuple<string, object, SqlDbType>("@course_id", courseId, SqlDbType.Int);

            dbManager.ExecuteNonProcedure("RemoveCourseForStudent", param1, param2);
        }

        private static Students DataRowToStudents(DataRow dr)
        {
            int id              = (int)     dr[STUDENT_ID];
            string firstName    = (string)  dr[STUDENT_FIRSTNAME];
            string lastName     = (string)  dr[STUDENT_LASTNAME];
            DateTime birthdate  = (DateTime)dr[STUDENT_BIRTHDATE];
            string serialNumber = (string)  dr[STUDENT_SERIALNUMBER];
            string emailAddress = (string)  dr[STUDENT_EMAILADDRESS];

            return new Students(id, firstName, lastName, birthdate, serialNumber, emailAddress);
        }
    }
}
