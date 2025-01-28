using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Text;

namespace MyAdvancedListbox
{
    class StudentResultData
    {
        public static List<StudentResult> getStudentResults()
        {
            try
            {
                List<StudentResult> resultList = new List<StudentResult>();
                string sql = "SELECT Id, StudentName, Result, Comment, Picture FROM Results ORDER BY Id";
                DataTable dt = Database.GetDT(sql);
                
                foreach (DataRow dr in dt.Rows)
                {
                    StudentResult sr = new StudentResult();
                    sr.Id = Convert.ToString(dr["ID"]);
                    sr.Name = Convert.ToString(dr["StudentName"]);
                    sr.Result = Convert.ToInt32(dr["Result"]);
                    sr.Comment = Convert.ToString(dr["Comment"]);
                    sr.Picture = Convert.ToString(dr["Picture"]);

                    resultList.Add(sr);
                }
                return resultList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class ResultList: ObservableCollection<StudentResult>
    {
        public ResultList()
        {
            List<StudentResult> resultList = StudentResultData.getStudentResults();
            foreach(StudentResult sr in resultList)
            {
                base.Add(sr);
            }
        }
    }
}
