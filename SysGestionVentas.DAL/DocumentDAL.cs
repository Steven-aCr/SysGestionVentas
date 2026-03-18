using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient; // ✅ Paquete correcto para .NET Core/5+

namespace DataAccessLayer
{
    public class Document
    {
        public int DocumentId { get; set; }
        public int DocTypeId { get; set; }
        public string DocNumber { get; set; } = string.Empty; // ✅ Evita warning de nullable
        public DateTime IssueDate { get; set; }
        public int PersonId { get; set; }
        public bool IsActive { get; set; } // Para eliminación lógica
    }

    public class DocumentDAL
    {
        private readonly string _connectionString;

        public DocumentDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ──────────────────────────────────────────────
        // CREAR
        // ──────────────────────────────────────────────
        public int Create(Document document)
        {
            const string query = @"
                INSERT INTO Document (DocTypeId, DocNumber, IssueDate, PersonId, IsActive)
                VALUES (@DocTypeId, @DocNumber, @IssueDate, @PersonId, 1);
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@DocTypeId", document.DocTypeId);
                cmd.Parameters.AddWithValue("@DocNumber", document.DocNumber);
                cmd.Parameters.AddWithValue("@IssueDate", document.IssueDate);
                cmd.Parameters.AddWithValue("@PersonId", document.PersonId);

                conn.Open();
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        // ──────────────────────────────────────────────
        // OBTENER POR ID
        // ──────────────────────────────────────────────
        public Document? GetById(int documentId) // ✅ Nullable porque puede no encontrarse
        {
            const string query = @"
                SELECT DocumentId, DocTypeId, DocNumber, IssueDate, PersonId, IsActive
                FROM Document
                WHERE DocumentId = @DocumentId
                  AND IsActive = 1;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@DocumentId", documentId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapDocument(reader);
                }
            }

            return null;
        }

        // ──────────────────────────────────────────────
        // OBTENER TODOS (activos)
        // ──────────────────────────────────────────────
        public List<Document> GetAll()
        {
            const string query = @"
                SELECT DocumentId, DocTypeId, DocNumber, IssueDate, PersonId, IsActive
                FROM Document
                WHERE IsActive = 1;";

            var list = new List<Document>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(MapDocument(reader));
                }
            }

            return list;
        }

        // ──────────────────────────────────────────────
        // MODIFICAR
        // ──────────────────────────────────────────────
        public bool Update(Document document)
        {
            const string query = @"
                UPDATE Document
                SET DocTypeId = @DocTypeId,
                    DocNumber  = @DocNumber,
                    IssueDate  = @IssueDate,
                    PersonId   = @PersonId
                WHERE DocumentId = @DocumentId
                  AND IsActive   = 1;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@DocumentId", document.DocumentId);
                cmd.Parameters.AddWithValue("@DocTypeId", document.DocTypeId);
                cmd.Parameters.AddWithValue("@DocNumber", document.DocNumber);
                cmd.Parameters.AddWithValue("@IssueDate", document.IssueDate);
                cmd.Parameters.AddWithValue("@PersonId", document.PersonId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // ──────────────────────────────────────────────
        // ELIMINAR (lógico — solo marca IsActive = 0)
        // ──────────────────────────────────────────────
        public bool Delete(int documentId)
        {
            const string query = @"
                UPDATE Document
                SET IsActive = 0
                WHERE DocumentId = @DocumentId
                  AND IsActive   = 1;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@DocumentId", documentId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // ──────────────────────────────────────────────
        // HELPER — mapear fila a objeto
        // ──────────────────────────────────────────────
        private Document MapDocument(SqlDataReader reader)
        {
            return new Document
            {
                DocumentId = Convert.ToInt32(reader["DocumentId"]),
                DocTypeId = Convert.ToInt32(reader["DocTypeId"]),
                DocNumber = reader["DocNumber"]?.ToString() ?? string.Empty, // ✅ Evita null
                IssueDate = Convert.ToDateTime(reader["IssueDate"]),
                PersonId = Convert.ToInt32(reader["PersonId"]),
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
        }
    }
}