using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
    public class Inventory
    {
        public int InventoryId { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public int MinimumStock { get; set; }
        public int CurrentStock { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } // Eliminación lógica
    }

    public class InventoryDAL
    {
        private readonly string _connectionString;

        public InventoryDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ──────────────────────────────────────────────
        // CREAR
        // ──────────────────────────────────────────────
        public int Create(Inventory inventory)
        {
            const string query = @"
                INSERT INTO Inventory (PurchasePrice, SalePrice, MinimumStock, CurrentStock, ProductId, IsActive)
                VALUES (@PurchasePrice, @SalePrice, @MinimumStock, @CurrentStock, @ProductId, 1);
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@PurchasePrice", inventory.PurchasePrice);
                cmd.Parameters.AddWithValue("@SalePrice", inventory.SalePrice);
                cmd.Parameters.AddWithValue("@MinimumStock", inventory.MinimumStock);
                cmd.Parameters.AddWithValue("@CurrentStock", inventory.CurrentStock);
                cmd.Parameters.AddWithValue("@ProductId", inventory.ProductId);

                conn.Open();
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        // ──────────────────────────────────────────────
        // OBTENER POR ID
        // ──────────────────────────────────────────────
        public Inventory? GetById(int inventoryId)
        {
            const string query = @"
                SELECT InventoryId, PurchasePrice, SalePrice, MinimumStock, CurrentStock, ProductId, CreatedAt, IsActive
                FROM Inventory
                WHERE InventoryId = @InventoryId
                  AND IsActive = 1;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@InventoryId", inventoryId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapInventory(reader);
                }
            }

            return null;
        }

        // ──────────────────────────────────────────────
        // OBTENER TODOS (activos)
        // ──────────────────────────────────────────────
        public List<Inventory> GetAll()
        {
            const string query = @"
                SELECT InventoryId, PurchasePrice, SalePrice, MinimumStock, CurrentStock, ProductId, CreatedAt, IsActive
                FROM Inventory
                WHERE IsActive = 1;";

            var list = new List<Inventory>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(MapInventory(reader));
                }
            }

            return list;
        }

        // ──────────────────────────────────────────────
        // MODIFICAR
        // ──────────────────────────────────────────────
        public bool Update(Inventory inventory)
        {
            const string query = @"
                UPDATE Inventory
                SET PurchasePrice = @PurchasePrice,
                    SalePrice     = @SalePrice,
                    MinimumStock  = @MinimumStock,
                    CurrentStock  = @CurrentStock,
                    ProductId     = @ProductId
                WHERE InventoryId = @InventoryId
                  AND IsActive    = 1;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@InventoryId", inventory.InventoryId);
                cmd.Parameters.AddWithValue("@PurchasePrice", inventory.PurchasePrice);
                cmd.Parameters.AddWithValue("@SalePrice", inventory.SalePrice);
                cmd.Parameters.AddWithValue("@MinimumStock", inventory.MinimumStock);
                cmd.Parameters.AddWithValue("@CurrentStock", inventory.CurrentStock);
                cmd.Parameters.AddWithValue("@ProductId", inventory.ProductId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // ──────────────────────────────────────────────
        // ELIMINAR (lógico)
        // ──────────────────────────────────────────────
        public bool Delete(int inventoryId)
        {
            const string query = @"
                UPDATE Inventory
                SET IsActive = 0
                WHERE InventoryId = @InventoryId
                  AND IsActive    = 1;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@InventoryId", inventoryId);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // ──────────────────────────────────────────────
        // HELPER — mapear fila a objeto
        // ──────────────────────────────────────────────
        private Inventory MapInventory(SqlDataReader reader)
        {
            return new Inventory
            {
                InventoryId = Convert.ToInt32(reader["InventoryId"]),
                PurchasePrice = Convert.ToDecimal(reader["PurchasePrice"]),
                SalePrice = Convert.ToDecimal(reader["SalePrice"]),
                MinimumStock = Convert.ToInt32(reader["MinimumStock"]),
                CurrentStock = Convert.ToInt32(reader["CurrentStock"]),
                ProductId = Convert.ToInt32(reader["ProductId"]),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
        }
    }
}