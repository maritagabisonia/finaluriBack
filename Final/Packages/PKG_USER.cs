using Final.Models;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;

namespace Final.Packages
{
    public interface IPKG_USERS
    {
        public void register_user(RegisterUser user);
        public LogInResponse Login_user(LogIn user);
        public GetUser get_user_by_id(int id);
        public  List<FullNameUser>  get_all_user();

        public List<UserAnswer> get_answers_by_id(int id);

        public void add_question(addQuestion newQuestion);
        public void update_questions(getQuestion updatedQuestion);
        public void update_question(int id, List<questionJSONmodel> question);

        public void parse_json_answers(List<userForm> userForm);
        public List<questionDTO> parse_json_question(List<questionJSONmodel> question);

        public List<getQuestion> get_questions();
        public List<questionDTO> read_questions(int subjectID);

        public List<getQuestion> add_question_ret_list(addQuestion newQuestion);




    }
    public class PKG_USER: PKG_BASE, IPKG_USERS
    {

        IConfiguration configuration;
        public PKG_USER(IConfiguration configuration) : base(configuration)
        {

        }
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

        public void register_user(RegisterUser user)
        {
            CreatePasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);

    


            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = Connstr;

            conn.Open();

            OracleCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;
            cmd.CommandText = "olerning.marita_user_f.registre_user";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_firstName", OracleDbType.Varchar2).Value = user.FirstName;
            cmd.Parameters.Add("p_USERNAME", OracleDbType.Varchar2).Value = user.UserName;
            cmd.Parameters.Add("p_roleId", OracleDbType.Int32).Value = user.Roleid;
            cmd.Parameters.Add("p_passwordHash", OracleDbType.Varchar2).Value = Convert.ToBase64String(passwordHash);
            cmd.Parameters.Add("p_passwordSalt", OracleDbType.Varchar2).Value = Convert.ToBase64String(passwordSalt);

            cmd.ExecuteNonQuery();

            conn.Close();
        }
        public LogInResponse Login_user(LogIn user)
        {
            OracleConnection conn = new OracleConnection(Connstr);

            conn.Open();
            OracleCommand cmd = conn.CreateCommand();

            cmd.CommandText = "olerning.marita_user_f.login_user";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = user.UserName;
            cmd.Parameters.Add("user_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            OracleDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                LogInResponse logInResponse = new LogInResponse();
                logInResponse.Id = int.Parse(reader["id"].ToString());
                logInResponse.Role = reader["role"].ToString();

                Password password = new Password();

                password.PasswordHash = Convert.FromBase64String(reader["passwordhash"].ToString());
                password.PasswordSalt = Convert.FromBase64String(reader["passwordsalt"].ToString());

                Console.WriteLine($"Retrieved PasswordHash: {reader["passwordhash"]}");
                Console.WriteLine($"Retrieved PasswordSalt: {reader["passwordsalt"]}");

                if (VerifyPasswordHash(user.Password, password.PasswordHash, password.PasswordSalt))
                {
                    return logInResponse;
                }
               
            }
            return null;
        }
        public GetUser get_user_by_id(int id)
        {
            OracleConnection conn = new OracleConnection(Connstr);

            conn.Open();
            OracleCommand cmd = conn.CreateCommand();

            cmd.CommandText = "olerning.marita_user_f.get_user_by_id";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_id", OracleDbType.Varchar2).Value = id;
            cmd.Parameters.Add("user_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            OracleDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                GetUser getUser = new GetUser();
                getUser.Id = int.Parse(reader["id"].ToString());
                getUser.FirstName = reader["firstname"].ToString();
                getUser.UserName = reader["username"].ToString();

                return getUser;
                

            }
            return null;

        }

        public List<FullNameUser> get_all_user()
        {
            OracleConnection conn = new OracleConnection(Connstr);
            List<FullNameUser> listOfUsers = new List<FullNameUser>(); 

            conn.Open();
            OracleCommand cmd = conn.CreateCommand();

            cmd.CommandText = "olerning.pkg_marita_answers.get_all_user";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("user_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                FullNameUser user = new FullNameUser();

                user.Id = int.Parse(reader["id"].ToString());
                user.FullName = reader["fullname"].ToString();
                
                listOfUsers.Add(user);

            }
            return listOfUsers;
        }
        public List<UserAnswer> get_answers_by_id(int id)
        {

            OracleConnection conn = new OracleConnection(Connstr);
            List<UserAnswer> listOfUserAnswers = new List<UserAnswer>();

            conn.Open();
            OracleCommand cmd = conn.CreateCommand();

            cmd.CommandText = "olerning.pkg_marita_answers.get_answers_by_user";
            cmd.CommandType = CommandType.StoredProcedure;


            cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = id;

            cmd.Parameters.Add("answer_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                UserAnswer userAnswer = new UserAnswer();

                userAnswer.Answer = reader["answer"].ToString();
                userAnswer.Question = reader["question"].ToString();

                listOfUserAnswers.Add(userAnswer);




            }
            return listOfUserAnswers;
        }

        public void add_question(addQuestion newQuestion)
        {


            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = Connstr;

            conn.Open();

            OracleCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;
            cmd.CommandText = "olerning.pkg_marita_qustions.add_question";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add( "p_answer", OracleDbType.Varchar2).Value = newQuestion.Answer;
            cmd.Parameters.Add("p_question", OracleDbType.Varchar2).Value = newQuestion.Question;
            cmd.Parameters.Add("p_important", OracleDbType.Int32).Value = newQuestion.Important;


            cmd.ExecuteNonQuery();

            conn.Close();

        }
        public void update_questions(getQuestion updatedQuestion)
        {
            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = Connstr;

            conn.Open();

            OracleCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;
            cmd.CommandText = "olerning.pkg_marita_qustions.update_questions";
            cmd.CommandType = CommandType.StoredProcedure;


            cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = updatedQuestion.Id;
            cmd.Parameters.Add("p_question", OracleDbType.Varchar2).Value = updatedQuestion.Question;
            cmd.Parameters.Add("p_answer", OracleDbType.Varchar2).Value = updatedQuestion.Answer;
            cmd.Parameters.Add("p_important", OracleDbType.Int32).Value = updatedQuestion.Important;


            cmd.ExecuteNonQuery();

            conn.Close();

        }

        public void parse_json_answers(List<userForm> userForm )
        {
          
                string json = JsonConvert.SerializeObject(userForm);
                OracleConnection conn = new OracleConnection();
                conn.ConnectionString = Connstr;

                conn.Open();

                OracleCommand cmd = conn.CreateCommand();
                cmd.Connection = conn;
                cmd.CommandText = "olerning.pkg_marita_answers.parse_json_answers";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_json_data", OracleDbType.Clob).Value = json;

                cmd.ExecuteNonQuery();

                conn.Close();
            
        }
        public List<getQuestion> get_questions()
        {
            OracleConnection conn = new OracleConnection(Connstr);
            List<getQuestion> listOfQuestions = new List<getQuestion>();

            conn.Open();
            OracleCommand cmd = conn.CreateCommand();

            cmd.CommandText = "olerning. pkg_marita_qustions.get_questions";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("user_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                getQuestion question = new getQuestion();

                question.Id = int.Parse(reader["id"].ToString());
                question.Question = reader["question"].ToString();
                question.Answer = reader["answer"].ToString();
                question.Important = reader["important"].ToString() == "1" ;

                listOfQuestions.Add(question);

            }
            return listOfQuestions;
        }
        public List<getQuestion> add_question_ret_list(addQuestion newQuestion)
        {
            OracleConnection conn = new OracleConnection();
            List<getQuestion> listOfQuestions = new List<getQuestion>();

            conn.ConnectionString = Connstr;

            conn.Open();

            OracleCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;
            cmd.CommandText = "olerning.pkg_marita_qustions.add_question_ret_list";
            cmd.CommandType = CommandType.StoredProcedure;


            cmd.Parameters.Add("p_question", OracleDbType.Varchar2).Value = newQuestion.Question;
            cmd.Parameters.Add("p_answer", OracleDbType.Varchar2).Value = newQuestion.Answer;
            cmd.Parameters.Add("p_important", OracleDbType.Int32).Value = newQuestion.Important;

            cmd.Parameters.Add("question_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                getQuestion question = new getQuestion();
                question.Id = int.Parse(reader["id"].ToString());
                question.Question = reader["question"].ToString();
                question.Answer = reader["answer"].ToString();
                question.Important = reader["important"].ToString() == "1";


                listOfQuestions.Add(question);

            }
            return listOfQuestions;
        }






        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
