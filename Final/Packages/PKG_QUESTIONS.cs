using Final.Models;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Final.Packages
{
  
    public interface IPKG_QUESTIONS
    {
        public void update_question(int id, List<questionJSONmodel> question);
        public List<questionDTO> parse_json_question(List<questionJSONmodel> question);
        public List<questionDTO> read_questions(int subjectID);

    }



    public class PKG_QUESTIONS : PKG_BASE, IPKG_QUESTIONS
    {
        IConfiguration configuration;
        public PKG_QUESTIONS(IConfiguration configuration) : base(configuration){}
        public void update_question(int id, List<questionJSONmodel> question)
        {
            OracleConnection conn = new OracleConnection();
            string json = JsonConvert.SerializeObject(question);

            conn.ConnectionString = Connstr;

            conn.Open();

            OracleCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;
            cmd.CommandText = "olerning.pkg_marita_q_modified.update_question";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_old_id", OracleDbType.Int32).Value = id;

            cmd.Parameters.Add("p_json_data", OracleDbType.Clob).Value = json;



            cmd.ExecuteNonQuery();

            conn.Close();



        }

        public List<questionDTO> read_questions(int subjectID)
        {
            OracleConnection conn = new OracleConnection();
            List<questionDTO> quiz = new List<questionDTO>();

            conn.ConnectionString = Connstr;

            conn.Open();

            OracleCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;
            cmd.CommandText = "olerning.pkg_marita_q_modified.get_questions";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_subject_id", OracleDbType.Int32).Value = subjectID;
            cmd.Parameters.Add("question_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            OracleDataReader reader = cmd.ExecuteReader();

            Dictionary<int, questionDTO> questionDict = new Dictionary<int, questionDTO>();

            while (reader.Read())
            {
                int questionId = reader.GetInt32(reader.GetOrdinal("question_id"));

                if (!questionDict.ContainsKey(questionId))
                {
                    questionDTO question = new questionDTO
                    {
                        ID = reader.IsDBNull(reader.GetOrdinal("question_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("question_id")),
                        Question = reader.IsDBNull(reader.GetOrdinal("question")) ? null : reader.GetString(reader.GetOrdinal("question")),
                        Important = reader.IsDBNull(reader.GetOrdinal("important")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("important")),
                        NumberOfAnswers = reader.IsDBNull(reader.GetOrdinal("number_of_answers")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("number_of_answers")),
                        Answers = new List<answerDTO>()
                    };
                    questionDict[questionId] = question;
                }

                answerDTO ans = new answerDTO
                {
                    ID = reader.IsDBNull(reader.GetOrdinal("id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("id")),
                    Answer = reader.IsDBNull(reader.GetOrdinal("answer")) ? null : reader.GetString(reader.GetOrdinal("answer")),
                    Correct = reader.IsDBNull(reader.GetOrdinal("correct")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("correct"))
                };

                questionDict[questionId].Answers.Add(ans);
            }

            quiz = new List<questionDTO>(questionDict.Values);


            return quiz;
        }

        public List<questionDTO> parse_json_question(List<questionJSONmodel> question)
        {

            string json = JsonConvert.SerializeObject(question);
            List<questionDTO> quiz = new List<questionDTO>();
            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = Connstr;

            conn.Open();

            OracleCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;
            cmd.CommandText = "olerning.pkg_marita_q_modified.add_question";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_json_data", OracleDbType.Clob).Value = json;
            cmd.Parameters.Add("question_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;



            OracleDataReader reader = cmd.ExecuteReader();

            Dictionary<int, questionDTO> questionDict = new Dictionary<int, questionDTO>();
            while (reader.Read())
            {
                int questionId = reader.GetInt32(reader.GetOrdinal("question_id"));

                if (!questionDict.ContainsKey(questionId))
                {
                    questionDTO modifiedQuestion = new questionDTO
                    {
                        ID = reader.IsDBNull(reader.GetOrdinal("question_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("question_id")),
                        Question = reader.IsDBNull(reader.GetOrdinal("question")) ? null : reader.GetString(reader.GetOrdinal("question")),
                        Important = reader.IsDBNull(reader.GetOrdinal("important")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("important")),
                        NumberOfAnswers = reader.IsDBNull(reader.GetOrdinal("number_of_answers")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("number_of_answers")),
                        Answers = new List<answerDTO>(),
                    };
                    questionDict[questionId] = modifiedQuestion;
                }

                answerDTO ans = new answerDTO
                {
                    ID = reader.IsDBNull(reader.GetOrdinal("id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("id")),
                    Answer = reader.IsDBNull(reader.GetOrdinal("answer")) ? null : reader.GetString(reader.GetOrdinal("answer")),
                    Correct = reader.IsDBNull(reader.GetOrdinal("correct")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("correct"))
                };
                questionDict[questionId].Answers.Add(ans);
            }
            quiz = new List<questionDTO>(questionDict.Values);
            conn.Close();

            return quiz;
        }



    }
}
