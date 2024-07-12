using Final.Models;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Security.Cryptography;

namespace Final.Packages
{
    public interface IPKG_USERS
    {
        public void register_user(RegisterUser user);
        public LogInResponse Login_user(LogIn user);
        public GetUser get_user_by_id(int id);
        public  List<string>  get_all_user();

        public List<UserAnswer> get_answers_by_full_name(int id);

        public void add_question(addQuestion newQuestion);
        public void update_questions(int id,addQuestion newQuestion);

        public void parse_json_answers(List<userForm> userForm);
        public List<addQuestion> get_questions();


    }
    public class PKG_USER: PKG_BASE, IPKG_USERS
    {

        IConfiguration configuration;
        public PKG_USER(IConfiguration configuration) : base(configuration)
        {

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

        public List<string> get_all_user()
        {
            OracleConnection conn = new OracleConnection(Connstr);
            List<string> listOfUsers = new List<string>(); 

            conn.Open();
            OracleCommand cmd = conn.CreateCommand();

            cmd.CommandText = "olerning.pkg_marita_answers.get_all_user";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("user_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string userName;
                userName = reader["fullname"].ToString();
                listOfUsers.Add(userName);

              


            }
            return listOfUsers;
        }
        public List<UserAnswer> get_answers_by_full_name(int id)
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
        public void update_questions(int id, addQuestion newQuestion)
        {
            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = Connstr;

            conn.Open();

            OracleCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;
            cmd.CommandText = "olerning.pkg_marita_qustions.update_questions";
            cmd.CommandType = CommandType.StoredProcedure;


            cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = id;

            cmd.Parameters.Add("p_answer", OracleDbType.Varchar2).Value = newQuestion.Answer;
            cmd.Parameters.Add("p_question", OracleDbType.Varchar2).Value = newQuestion.Question;
            cmd.Parameters.Add("p_important", OracleDbType.Int32).Value = newQuestion.Important;


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
        public List<addQuestion> get_questions()
        {
            OracleConnection conn = new OracleConnection(Connstr);
            List<addQuestion> listOfQuestions = new List<addQuestion>();

            conn.Open();
            OracleCommand cmd = conn.CreateCommand();

            cmd.CommandText = "olerning. pkg_marita_qustions.get_questions";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("user_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                addQuestion question = new addQuestion();
                question.Question = reader["question"].ToString();
                question.Answer = reader["answer"].ToString();

                question.Important = reader["important"].ToString() == "1" ;


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
