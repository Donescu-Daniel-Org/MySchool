using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySchool
{
    class Courses
    {
        public int Id { get; set; }
        public string Naming { get; set; }
        public string Domain { get; set; }
        public string Teacher { get; set; }
        public int Duration { get; set; }

        const string CONN_STRING_NAME = "MySchoolDB";

        public static readonly string COURSE_ID = "id";
        public static readonly string COURSE_NAMING = "naming";
        public static readonly string COURSE_DOMAIN = "domain";
        public static readonly string COURSE_TEACHER = "teacher";
        public static readonly string COURSE_DURATION = "duration";

        public Courses(int id, string naming, string domain, string teacher, int duration)
        {
            this.Id       = id;
            this.Naming   = naming;
            this.Domain   = domain;
            this.Teacher  = teacher;
            this.Duration = duration;
        }

        public Courses() { }

        public static List<Courses> RetrieveAllCourses()
        {
            DataTable dbTable = RetrieveAllCoursesDT();

            List<Courses> courses = new List<Courses>();
            foreach(DataRow dr in dbTable.Rows)
            {
                courses.Add(DataRowToCourses(dr));
            }

            return courses;
        }

        public static DataTable RetrieveAllCoursesDT()
        {
            DBManager dbManager = new DBManager(CONN_STRING_NAME);

            return dbManager.ExecuteProcedure("RetrieveAllCourses");
        }

        public static List<string> RetrieveAllCoursesNames()
        {
            DBManager dbManager = new DBManager(CONN_STRING_NAME);

            DataTable dbTable = dbManager.ExecuteProcedure("RetrieveAllCoursesNames");

            List<string> coursesNames = new List<string>();
            foreach(DataRow dr in dbTable.Rows)
            {
                coursesNames.Add((string)dr["naming"]);
            }

            return coursesNames;
        }

        public static Courses RetrieveCourseById(int id)
        {
            DBManager dbManager = new DBManager(CONN_STRING_NAME);

            Tuple<string, object, SqlDbType> procParam = new Tuple<string, object, SqlDbType>("@id", id, SqlDbType.Int);

            DataTable dbTable = dbManager.ExecuteProcedure("RetrieveCourseById", procParam);


            if(dbTable.Rows.Count > 1)
            {
                Debug.Assert(false, "There are more than one record in database for this course name!");
                return DataRowToCourses(dbTable.Rows[0]);
            }
            else if(dbTable.Rows.Count == 0)
            {
                throw new Exception("This course name doesn't exists or an unexpected error has occured!");
            }
            else
                return DataRowToCourses(dbTable.Rows[0]);
                
        }
        public static bool ValidateDuration(int duration)
        {
            return duration > 0;
        }

        public static List<string> RetrieveRemainingCoursesForStudent(int id)
        {
            DBManager dbManager = new DBManager(CONN_STRING_NAME);

            Tuple<string, object, SqlDbType> procParam = new Tuple<string, object, SqlDbType>("@id", id, SqlDbType.Int);

            DataTable dbTable = dbManager.ExecuteProcedure("RetrieveRemainingCoursesForStudent", procParam);

            List<string> coursesNames = new List<string>();
            foreach(DataRow dr in dbTable.Rows)
            {
                coursesNames.Add((string)dr["naming"]);
            }

            return coursesNames;
        }

        public static void UpdateAllData(DataTable dt)
        {
            DBManager dbManager = new DBManager(CONN_STRING_NAME);

            foreach (DataRow dr in dt.Rows)
            {
                Tuple<string, object, SqlDbType> param1 = new Tuple<string, object, SqlDbType>("@id", dr[COURSE_ID], SqlDbType.Int);
                Tuple<string, object, SqlDbType> param2 = new Tuple<string, object, SqlDbType>("@naming", dr[COURSE_NAMING], SqlDbType.NVarChar);
                Tuple<string, object, SqlDbType> param3 = new Tuple<string, object, SqlDbType>("@domain", dr[COURSE_DOMAIN], SqlDbType.NVarChar);
                Tuple<string, object, SqlDbType> param4 = new Tuple<string, object, SqlDbType>("@teacher", dr[COURSE_TEACHER], SqlDbType.NVarChar);
                Tuple<string, object, SqlDbType> param5 = new Tuple<string, object, SqlDbType>("@duration", dr[COURSE_DURATION], SqlDbType.Int);

                dbManager.ExecuteNonProcedure("UpdateCourseAtId", param1, param2, param3, param4, param5);
            }
        }

        public static void DeleteCourses(List<int> ids)
        {
            DBManager dbManager = new DBManager(CONN_STRING_NAME);

            foreach (int id in ids)
            {
                Tuple<string, object, SqlDbType> param1 = new Tuple<string, object, SqlDbType>("@id", id, SqlDbType.Int);
                dbManager.ExecuteNonProcedure("DeleteCourseAtId", param1);
            }
        }

        private static Courses DataRowToCourses(DataRow dr)
        {
            int id         = (int)   dr[COURSE_ID];
            string naming  = (string)dr[COURSE_NAMING];
            string domain  = (string)dr[COURSE_DOMAIN];
            string teacher = (string)dr[COURSE_TEACHER];
            int duration   = (int)   dr[COURSE_DURATION];

            return new Courses(id, naming, domain, teacher, duration);
        }
    }
}
