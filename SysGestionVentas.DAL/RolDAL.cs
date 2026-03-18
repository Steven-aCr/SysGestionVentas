using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
    public class Rol
    {
        public int RolId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsActive { get; set; } // Eliminación lógica
    }

    public class RolDAL
    {
        private readonly string _connectionString;

        public RolDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ──────────────────────────────────────────────
        // CREAR
        // ──────────────────────────────────────────────
        public int Create(Rol rol)
        {
            const string query = @"
                INSERT INTO Rol (Name, Description, IsActive)
                VALUES (@Name, @Description, 1);
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Name", rol.Name);
                cmd.Parameters.AddWithValue("@Description", (object?)rol.Description ?? DBNull.Value);

                conn.Open();
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        // ──────────────────────────────────────────────
        // OBTENER POR ID
        // ──────────────────────────────────────────────
        public Rol? GetById(int rolId)
        {
            const string query = @"
                SELECT RolId, Name, Description, CreateAt, IsActive
                FROM Rol
                WHERE RolId = @RolId
                  AND IsActive = 1;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@RolId", rolId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapRol(reader);
                }
            }

            return null;
        }

        // ──────────────────────────────────────────────
        // OBTENER TODOS (activos)
        // ──────────────────────────────────────────────
        public List<Rol> GetAll()
        {
            const string query = @"
                SELECT RolId, Name, Description, CreateAt, IsActive
                FROM Rol
                WHERE IsActive = 1;";

            var list = new List<Rol>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(MapRol(reader));
                }
            }

            return list;
        }

        // ──────────────────────────────────────────────
        // MODIFICAR
        // ──────────────────────────────────────────────
        public bool Update(Rol rol)
        {
            const string query = @"
                UPDATE Rol
                SET Name        = @Name,
                    Description = @Description
                WHERE RolId  = @RolId
                  AND IsActive = 1;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@RolId", rol.RolId);
                cmd.Parameters.AddWithValue("@Name", rol.Name);
                cmd.Parameters.AddWithValue("@Description", (object?)rol.Description ?? DBNull.Value);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // ──────────────────────────────────────────────
        // ELIMINAR (lógico)
        // ──────────────────────────────────────────────
        public bool Delete(int rolId)
        {
            const string query = @"
                UPDATE Rol
                SET IsActive = 0
                WHERE RolId  = @RolId
                  AND IsActive = 1;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@RolId", rolId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // ──────────────────────────────────────────────
        // HELPER — mapear fila a objeto
        // ──────────────────────────────────────────────
        private Rol MapRol(SqlDataReader reader)
        {
            return new Rol
            {
                RolId = Convert.ToInt32(reader["RolId"]),
                Name = reader["Name"]?.ToString() ?? string.Empty,
                Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                CreateAt = Convert.ToDateTime(reader["CreateAt"]),
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
        }
    }
}