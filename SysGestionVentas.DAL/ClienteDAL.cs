using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
    public class Client
    {
        public int ClientId { get; set; }
        public int PersonId { get; set; }
        public bool IsActive { get; set; } // Eliminación lógica
    }

    public class ClientDAL
    {
        private readonly string _connectionString;

        public ClientDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ──────────────────────────────────────────────
        // CREAR
        // ──────────────────────────────────────────────
        public int Create(Client client)
        {
            const string query = @"
                INSERT INTO Client (PersonId, IsActive)
                VALUES (@PersonId, 1);
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@PersonId", client.PersonId);

                conn.Open();
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        // ──────────────────────────────────────────────
        // OBTENER POR ID
        // ──────────────────────────────────────────────
        public Client? GetById(int clientId)
        {
            const string query = @"
                SELECT ClientId, PersonId, IsActive
                FROM Client
                WHERE ClientId = @ClientId
                  AND IsActive = 1;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ClientId", clientId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapClient(reader);
                }
            }

            return null;
        }

        // ──────────────────────────────────────────────
        // OBTENER TODOS (activos)
        // ──────────────────────────────────────────────
        public List<Client> GetAll()
        {
            const string query = @"
                SELECT ClientId, PersonId, IsActive
                FROM Client
                WHERE IsActive = 1;";

            var list = new List<Client>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(MapClient(reader));
                }
            }

            return list;
        }

        // ──────────────────────────────────────────────
        // MODIFICAR
        // ──────────────────────────────────────────────
        public bool Update(Client client)
        {
            const string query = @"
                UPDATE Client
                SET PersonId = @PersonId
                WHERE ClientId = @ClientId
                  AND IsActive = 1;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ClientId", client.ClientId);
                cmd.Parameters.AddWithValue("@PersonId", client.PersonId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // ──────────────────────────────────────────────
        // ELIMINAR (lógico)
        // ──────────────────────────────────────────────
        public bool Delete(int clientId)
        {
            const string query = @"
                UPDATE Client
                SET IsActive = 0
                WHERE ClientId = @ClientId
                  AND IsActive = 1;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ClientId", clientId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // ──────────────────────────────────────────────
        // HELPER — mapear fila a objeto
        // ──────────────────────────────────────────────
        private Client MapClient(SqlDataReader reader)
        {
            return new Client
            {
                ClientId = Convert.ToInt32(reader["ClientId"]),
                PersonId = Convert.ToInt32(reader["PersonId"]),
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
        }
    }
}