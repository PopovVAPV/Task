using System;
using System.Data;
using System.Data.SqlClient;

namespace Task.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=UsersDb;Integrated Security=True";

        public bool AuthenticateUser(string login, string password, out int userId, out string role, out bool needPasswordChange)
        {
            userId = 0;
            role = string.Empty;
            needPasswordChange = false;

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    // Check if user exists and not blocked
                    var command = new SqlCommand(
                        "SELECT Id, Role, PasswordHash, FailedLoginAttempts, NeedPasswordChange, IsBlocked " +
                        "FROM Users WHERE Login = @Login", connection);
                    command.Parameters.AddWithValue("@Login", login);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read()) return false;

                        userId = reader.GetInt32(0);
                        role = reader.GetString(1);
                        var storedPassword = reader.GetString(2);
                        var failedAttempts = reader.GetInt32(3);
                        needPasswordChange = reader.GetBoolean(4);
                        var isBlocked = reader.GetBoolean(5);
                        
                        reader.Close();
                        
                        if (isBlocked) return false;
                            
                        if (storedPassword == password)
                        {
                            // Update on success
                            ExecuteNonQuery("UPDATE Users SET FailedLoginAttempts = 0, LastLoginDate = GETDATE() WHERE Id = @UserId", 
                                new SqlParameter("@UserId", userId), connection);
                            return true;
                        }
                        
                        // Handle failed login
                        ExecuteNonQuery("UPDATE Users SET FailedLoginAttempts = FailedLoginAttempts + 1 WHERE Id = @UserId", 
                            new SqlParameter("@UserId", userId), connection);
                            
                        // Block user after 3 failed attempts
                        if (failedAttempts + 1 >= 3)
                        {
                            ExecuteNonQuery("UPDATE Users SET IsBlocked = 1 WHERE Id = @UserId", 
                                new SqlParameter("@UserId", userId), connection);
                        }
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void ExecuteNonQuery(string sql, SqlParameter parameter, SqlConnection connection = null)
        {
            var closeConnection = connection == null;
            try
            {
                if (connection == null)
                {
                    connection = new SqlConnection(_connectionString);
                    connection.Open();
                }
                
                var command = new SqlCommand(sql, connection);
                command.Parameters.Add(parameter);
                command.ExecuteNonQuery();
            }
            finally
            {
                if (closeConnection && connection?.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        public bool IsUserBlocked(string login)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT IsBlocked FROM Users WHERE Login = @Login", connection);
                    command.Parameters.AddWithValue("@Login", login);
                    
                    var result = command.ExecuteScalar();
                    return result != null && result != DBNull.Value && Convert.ToBoolean(result);
                }
            }
            catch
            {
                return false;
            }
        }

        public bool UpdatePassword(int userId, string currentPassword, string newPassword)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    // Verify current password
                    var command = new SqlCommand("SELECT PasswordHash FROM Users WHERE Id = @UserId", connection);
                    command.Parameters.AddWithValue("@UserId", userId);
                    
                    if ((string)command.ExecuteScalar() != currentPassword)
                        return false;
                    
                    // Update password
                    command = new SqlCommand(
                        "UPDATE Users SET PasswordHash = @NewPassword, NeedPasswordChange = 0 WHERE Id = @UserId", connection);
                    command.Parameters.AddWithValue("@NewPassword", newPassword);
                    command.Parameters.AddWithValue("@UserId", userId);
                    
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool CreateUser(string login, string password, string role)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    // Check if user exists
                    var command = new SqlCommand("SELECT COUNT(1) FROM Users WHERE Login = @Login", connection);
                    command.Parameters.AddWithValue("@Login", login);
                    
                    if ((int)command.ExecuteScalar() > 0)
                        return false;
                    
                    // Create user
                    command = new SqlCommand(
                        @"INSERT INTO Users (Login, PasswordHash, Role, IsBlocked, FailedLoginAttempts, 
                          LastLoginDate, CreationDate, NeedPasswordChange) 
                          VALUES (@Login, @Password, @Role, 0, 0, NULL, GETDATE(), 1)", connection);
                    
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Role", role);
                    
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool UnblockUser(int userId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "UPDATE Users SET IsBlocked = 0, FailedLoginAttempts = 0 WHERE Id = @UserId", connection);
                    command.Parameters.AddWithValue("@UserId", userId);
                    
                    return command.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public DataTable GetAllUsers()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "SELECT Id, Login, Role, IsBlocked, LastLoginDate, CreationDate FROM Users", connection);
                    
                    var table = new DataTable();
                    new SqlDataAdapter(command).Fill(table);
                    return table;
                }
            }
            catch
            {
                return new DataTable();
            }
        }

        public void BlockInactiveUsers()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        @"UPDATE Users 
                          SET IsBlocked = 1 
                          WHERE LastLoginDate IS NOT NULL 
                          AND DATEDIFF(day, LastLoginDate, GETDATE()) > 30", connection);
                    
                    command.ExecuteNonQuery();
                }
            }
            catch { /* Игнорируем ошибки */ }
        }
    }
} 