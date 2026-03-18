using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int StatusId { get; set; }
        public int CreatedByUser { get; set; }
        public bool IsActive { get; set; } // Eliminación lógica
    }

    public class CategoryDAL
    {
        private readonly string _connectionString;

        public CategoryDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ──────────────────────────────────────────────
        // CREAR
        // ──────────────────────────────────────────────
        public int Create(Category category)
        {
            const string query = @"
                INSERT INTO Category (Name, Description, StatusId, CreatedByUser, IsActive)
                VALUES (@Name, @Description, @StatusId, @CreatedByUser, 1);
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Name", category.Name);
                cmd.Parameters.AddWithValue("@Description", (object?)category.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StatusId", category.StatusId);
                cmd.Parameters.AddWithValue("@CreatedByUser", category.CreatedByUser);

                conn.Open();
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        // ──────────────────────────────────────────────
        // OBTENER POR ID
        // ──────────────────────────────────────────────
        public Category? GetById(int categoryId)
        {
            const string query = @"
                SELECT CategoryId, Name, Description, CreatedAt, StatusId, CreatedByUser, IsActive
                FROM Category
                WHERE CategoryId = @CategoryId
                  AND IsActive = 1;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@CategoryId", categoryId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapCategory(reader);
                }
            }

            return null;
        }

        // ──────────────────────────────────────────────
        // OBTENER TODOS (activos)
        // ──────────────────────────────────────────────
        public List<Category> GetAll()
        {
            const string query = @"
                SELECT CategoryId, Name, Description, CreatedAt, StatusId, CreatedByUser, IsActive
                FROM Category
                WHERE IsActive = 1;";

            var list = new List<Category>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(MapCategory(reader));
                }
            }

            return list;
        }

        // ──────────────────────────────────────────────
        // MODIFICAR
        // ──────────────────────────────────────────────
        public bool Update(Category category)
        {
            const string query = @"
                UPDATE Category
                SET Name          = @Name,
                    Description   = @Description,
                    StatusId      = @StatusId,
                    CreatedByUser = @CreatedByUser
                WHERE CategoryId = @CategoryId
                  AND IsActive   = 1;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@CategoryId", category.CategoryId);
                cmd.Parameters.AddWithValue("@Name", category.Name);
                cmd.Parameters.AddWithValue("@Description", (object?)category.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StatusId", category.StatusId);
                cmd.Parameters.AddWithValue("@CreatedByUser", category.CreatedByUser);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // ──────────────────────────────────────────────
        // ELIMINAR (lógico)
        // ──────────────────────────────────────────────
        public bool Delete(int categoryId)
        {
            const string query = @"
                UPDATE Category
                SET IsActive = 0
                WHERE CategoryId = @CategoryId
                  AND IsActive   = 1;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@CategoryId", categoryId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // ──────────────────────────────────────────────
        // HELPER — mapear fila a objeto
        // ──────────────────────────────────────────────
        private Category MapCategory(SqlDataReader reader)
        {
            return new Category
            {
                CategoryId = Convert.ToInt32(reader["CategoryId"]),
                Name = reader["Name"]?.ToString() ?? string.Empty,
                Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                StatusId = Convert.ToInt32(reader["StatusId"]),
                CreatedByUser = Convert.ToInt32(reader["CreatedByUser"]),
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
        }
    }
}
