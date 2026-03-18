using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
    public class Permission
    {
        public int PermissionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsActive { get; set; } // Eliminación lógica
    }

    public class PermissionDAL
    {
        private readonly string _connectionString;

        public PermissionDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ──────────────────────────────────────────────
        // CREAR
        // ──────────────────────────────────────────────
        public int Create(Permission permission)
        {
            const string query = @"
                INSERT INTO Permission (Name, Description, IsActive)
                VALUES (@Name, @Description, 1);
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Name", permission.Name);
                cmd.Parameters.AddWithValue("@Description", (object?)permission.Description ?? DBNull.Value);

                conn.Open();
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        // ──────────────────────────────────────────────
        // OBTENER POR ID
        // ──────────────────────────────────────────────
        public Permission? GetById(int permissionId)
        {
            const string query = @"
                SELECT PermissionId, Name, Description, CreateAt, IsActive
                FROM Permission
                WHERE PermissionId = @PermissionId
                  AND IsActive = 1;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@PermissionId", permissionId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapPermission(reader);
                }
            }

            return null;
        }

        // ──────────────────────────────────────────────
        // OBTENER TODOS (activos)
        // ──────────────────────────────────────────────
        public List<Permission> GetAll()
        {
            const string query = @"
                SELECT PermissionId, Name, Description, CreateAt, IsActive
                FROM Permission
                WHERE IsActive = 1;";

            var list = new List<Permission>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(MapPermission(reader));
                }
            }

            return list;
        }

        // ──────────────────────────────────────────────
        // MODIFICAR
        // ──────────────────────────────────────────────
        public bool Update(Permission permission)
        {
            const string query = @"
                UPDATE Permission
                SET Name        = @Name,
                    Description = @Description
                WHERE PermissionId = @PermissionId
                  AND IsActive     = 1;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@PermissionId", permission.PermissionId);
                cmd.Parameters.AddWithValue("@Name", permission.Name);
                cmd.Parameters.AddWithValue("@Description", (object?)permission.Description ?? DBNull.Value);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // ──────────────────────────────────────────────
        // ELIMINAR (lógico)
        // ──────────────────────────────────────────────
        public bool Delete(int permissionId)
        {
            const string query = @"
                UPDATE Permission
                SET IsActive = 0
                WHERE PermissionId = @PermissionId
                  AND IsActive     = 1;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@PermissionId", permissionId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // ──────────────────────────────────────────────
        // HELPER — mapear fila a objeto
        // ──────────────────────────────────────────────
        private Permission MapPermission(SqlDataReader reader)
        {
            return new Permission
            {
                PermissionId = Convert.ToInt32(reader["PermissionId"]),
                Name = reader["Name"]?.ToString() ?? string.Empty,
                Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                CreateAt = Convert.ToDateTime(reader["CreateAt"]),
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
        }
    }
}