using Final.Models;
using Microsoft.AspNetCore.Identity;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Final.Packages
{
    public interface IPKG_SUBJECTS
    {
        public void add_subject(string newSubject);
        public List<Subjects> get_questions();
    }



    public class PKG_SUBJECTS : PKG_BASE, IPKG_SUBJECTS
    {
        IConfiguration configuration;
        public PKG_SUBJECTS(IConfiguration configuration) : base(configuration)
        {


        }
        public void add_subject(string newSubject)
        {

            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = Connstr;

            conn.Open();

            OracleCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;
            cmd.CommandText = "olerning.pkg_marita_subjects.add_subject";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_subject_name", OracleDbType.Varchar2).Value = newSubject;


            cmd.ExecuteNonQuery();

            conn.Close();
        }
        public List<Subjects> get_questions()
        {
            OracleConnection conn = new OracleConnection(Connstr);
            List<Subjects> listOfSubjects = new List<Subjects>();

            conn.Open();
            OracleCommand cmd = conn.CreateCommand();

            cmd.CommandText = "olerning.pkg_marita_subjects.get_subject";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("subject_curs", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Subjects subject = new Subjects();

                subject.Id = int.Parse(reader["id"].ToString());
                subject.Subject = reader["subject"].ToString();

                listOfSubjects.Add(subject);

            }
            return listOfSubjects;
        }

    }
}
