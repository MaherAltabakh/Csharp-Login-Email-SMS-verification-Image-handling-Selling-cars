using Dapper;
using System.Data.SqlClient;


namespace BestPrice.Models

{
    public class UserDB
    {
        private string connectionString = ("server = Administrator; database=BestPrice; user id= reader; password= pass123;");

        //compare with the info in the DataBase
        public User signIn(string email, string password)
        {
            User user = new User();
            string sql = "Select * from [User] where (Email = @Email) and (Password = @Password)";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    user = conn.QueryFirst<User>(sql, new { email, password });
                }
                catch
                {
                    user.UserID = 0;
                }
            }
            return user;
        }

        public User LookForUserByEmail(string email)
        {
            User user = new User();
            string sql = "Select * from [User] where (Email = @Email)";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    user = conn.QueryFirst<User>(sql, new { email });
                }
                catch
                {
                    user.UserID = 0;
                }
            }
            return user;
        }

        public void UpdatePassword(string NewPassword, string email)
        {
            User user = LookForUserByEmail(email);
            user.Password = NewPassword;
            string sql = "UPDATE [User] SET " +
                            " Password = @Password" +
                            " WHERE UserID = @UserID;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Execute(sql, user);
            }
        }


        public void UserEditSave(User user)
        {
            string sql;
            if (user.UserID == 0)
            {
                user.CreatedBy = 0;
                user.DateCreated = DateTime.Now;
                user.Active = 0;
                sql = "INSERT INTO [User] " +
                        "(FirstName, LastName, AddressLine1," +
                        " AddressLine2, City, [State]," +
                        " ZipCode, Phone, Email, " +
                        " [Password], Active, CreatedBy, DateCreated)" +
                        "VALUES" +
                        "(@FirstName, @LastName, @AddressLine1," +
                        " @AddressLine2, @City, @State," +
                        " @ZipCode, @Phone, @Email," +
                        " @Password, @Active, @CreatedBy, @DateCreated)";
            }
            else
            {
                user.UpdatedBy = user.UserID;
                user.DateLastUpdated = DateTime.Now;
                user.Active = 0;
                sql = "UPDATE [User] SET " +
                        "FirstName = @FirstName, LastName = @LastName, AddressLine1 = @AddressLine1," +
                        " AddressLine2 = @AddressLine2, City = @City, [State] = @State," +
                        " ZipCode = @ZipCode, Phone = @Phone, Email = @Email, " +
                        " [Password] = @Password, Active = @Active, UpdatedBy = @UpdatedBy, DateLastUpdated = @DateLastUpdated" +
                        " WHERE UserID = @UserID;";
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Execute(sql, user);
            }
        }
        public void DeletAccount(string userId)
        {
            string sql = " Delete from [User] " +
                         " WHERE [UserID] = @UserID;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Execute(sql, new { userId });
            }
        }






        //public List<User> GetAllUsers()
        //{
        //    List<User> users = new List<User>();
        //    string sql = "select * from [User]";
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        users = conn.Query<User>(sql).ToList();
        //    }
        //    return users;
        //}

        //public User GetUserById(int UserID)
        //{
        //    var user = new User();
        //    string sql = " select * from [User] WHERE UserID =@UserID";

        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        user = conn.QueryFirst<User>(sql, new { UserID });
        //    }

        //    return user;
        //}

        //public void DeleteUser(int UserID)
        //{
        //    string sql = "DELETE FROM [User] WHERE [UserID] = @UserID";
        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Execute(sql, new { UserID });
        //    }

        //}

    }
}
