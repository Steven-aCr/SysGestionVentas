using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        public int PersonId { get; set; }
        public int StatusId { get; set; }
        public bool IsActive { get; set; } // Eliminación lógica
    }

    public class EmployeeDAL
    {
        private readonly string _connectionString;

        public EmployeeDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ──────────────────────────────────────────────
        // CREAR
        // ──────────────────────────────────────────────
        public int Create(Employee employee)
        {
            const string query = @"
                INSERT INTO Employee (EmployeeCode, HireDate, Salary, PersonId, StatusId, IsActive)
                VALUES (@EmployeeCode, @HireDate, @Salary, @PersonId, @StatusId, 1);
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EmployeeCode", employee.EmployeeCode);
                cmd.Parameters.AddWithValue("@HireDate", employee.HireDate);
                cmd.Parameters.AddWithValue("@Salary", employee.Salary);
                cmd.Parameters.AddWithValue("@PersonId", employee.PersonId);
                cmd.Parameters.AddWithValue("@StatusId", employee.StatusId);

                conn.Open();
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        // ──────────────────────────────────────────────
        // OBTENER POR ID
        // ──────────────────────────────────────────────
        public Employee? GetById(int employeeId)
        {
            const string query = @"
                SELECT EmployeeId, EmployeeCode, HireDate, Salary, PersonId, StatusId, IsActive
                FROM Employee
                WHERE EmployeeId = @EmployeeId
                  AND IsActive   = 1;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapEmployee(reader);
                }
            }

            return null;
        }

        // ──────────────────────────────────────────────
        // OBTENER TODOS (activos)
        // ──────────────────────────────────────────────
        public List<Employee> GetAll()
        {
            const string query = @"
                SELECT EmployeeId, EmployeeCode, HireDate, Salary, PersonId, StatusId, IsActive
                FROM Employee
                WHERE IsActive = 1;";

            var list = new List<Employee>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(MapEmployee(reader));
                }
            }

            return list;
        }

        // ──────────────────────────────────────────────
        // MODIFICAR
        // ──────────────────────────────────────────────
        public bool Update(Employee employee)
        {
            const string query = @"
                UPDATE Employee
                SET EmployeeCode = @EmployeeCode,
                    HireDate     = @HireDate,
                    Salary       = @Salary,
                    PersonId     = @PersonId,
                    StatusId     = @StatusId
                WHERE EmployeeId = @EmployeeId
                  AND IsActive   = 1;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);
                cmd.Parameters.AddWithValue("@EmployeeCode", employee.EmployeeCode);
                cmd.Parameters.AddWithValue("@HireDate", employee.HireDate);
                cmd.Parameters.AddWithValue("@Salary", employee.Salary);
                cmd.Parameters.AddWithValue("@PersonId", employee.PersonId);
                cmd.Parameters.AddWithValue("@StatusId", employee.StatusId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // ──────────────────────────────────────────────
        // ELIMINAR (lógico)
        // ──────────────────────────────────────────────
        public bool Delete(int employeeId)
        {
            const string query = @"
                UPDATE Employee
                SET IsActive = 0
                WHERE EmployeeId = @EmployeeId
                  AND IsActive   = 1;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // ──────────────────────────────────────────────
        // HELPER — mapear fila a objeto
        // ──────────────────────────────────────────────
        private Employee MapEmployee(SqlDataReader reader)
        {
            return new Employee
            {
                EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                EmployeeCode = reader["EmployeeCode"]?.ToString() ?? string.Empty,
                HireDate = Convert.ToDateTime(reader["HireDate"]),
                Salary = Convert.ToDecimal(reader["Salary"]),
                PersonId = Convert.ToInt32(reader["PersonId"]),
                StatusId = Convert.ToInt32(reader["StatusId"]),
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
        }
    }
}