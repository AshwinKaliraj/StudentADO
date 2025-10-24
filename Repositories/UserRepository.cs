using StudentADO.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace StudentADO.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<User> GetByIdAsync(int userId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Users WHERE UserId = @UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapToUser(reader);
                    }
                }
            }
            return null;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Users WHERE Email = @Email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);

                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapToUser(reader);
                    }
                }
            }
            return null;
        }

        public async Task<List<User>> GetAllAsync()
        {
            List<User> users = new List<User>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Users ORDER BY CreatedAt DESC";
                SqlCommand cmd = new SqlCommand(query, conn);

                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(MapToUser(reader));
                    }
                }
            }
            return users;
        }

        public async Task<int> CreateAsync(User user)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO Users (Name, Email, PasswordHash, DateOfBirth, Designation, CreatedAt) 
                                 VALUES (@Name, @Email, @PasswordHash, @DateOfBirth, @Designation, @CreatedAt);
                                 SELECT CAST(SCOPE_IDENTITY() as int);";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
                cmd.Parameters.AddWithValue("@Designation", user.Designation);
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

                await conn.OpenAsync();
                int userId = (int)await cmd.ExecuteScalarAsync();
                return userId;
            }
        }

        public async Task<bool> UpdateAsync(User user)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE Users 
                                 SET Name = @Name, 
                                     Email = @Email,
                                     DateOfBirth = @DateOfBirth, 
                                     Designation = @Designation,
                                     UpdatedAt = @UpdatedAt 
                                 WHERE UserId = @UserId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", user.UserId);
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
                cmd.Parameters.AddWithValue("@Designation", user.Designation);
                cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                await conn.OpenAsync();
                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                Console.WriteLine($"[UPDATE] UserId: {user.UserId}, Rows Affected: {rowsAffected}");

                return rowsAffected > 0;
            }
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Users WHERE UserId = @UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                await conn.OpenAsync();
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);

                await conn.OpenAsync();
                int count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
        }

        private User MapToUser(SqlDataReader reader)
        {
            return new User
            {
                UserId = (int)reader["UserId"],
                Name = reader["Name"].ToString(),
                Email = reader["Email"].ToString(),
                PasswordHash = reader["PasswordHash"].ToString(),
                DateOfBirth = (DateTime)reader["DateOfBirth"],
                Designation = reader["Designation"].ToString(),
                CreatedAt = (DateTime)reader["CreatedAt"],
                UpdatedAt = reader["UpdatedAt"] != DBNull.Value ? (DateTime?)reader["UpdatedAt"] : null
            };
        }
    }
}
