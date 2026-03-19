using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;

namespace DataAccessLayer
{
    public class CategoryDAL
    {
        #region "Validaciones privadas"

        private static async Task<bool> ExisteNombre(Category pCategory,
            DbContexto pDBContexto)
        {
            bool result = false;
            var categoryExiste = await pDBContexto.Category.FirstOrDefaultAsync(
                c => c.Name == pCategory.Name && c.CategoryId != pCategory.CategoryId);

            if (categoryExiste != null && categoryExiste.CategoryId > 0)
                result = true;

            return result;
        }

        #endregion

        #region "CRUD"

        public static async Task<int> GuardarAsync(Category pCategory)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    bool existeNombre = await ExisteNombre(pCategory, dbContexto);
                    if (existeNombre == false)
                    {
                        pCategory.CreatedAt = DateTime.Now;
                        dbContexto.Add(pCategory);
                        result = await dbContexto.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("El nombre de la categoría ya existe.");
                    }
                }
            }
            catch (Exception ex)
            {
                result = 0;
                throw new Exception(ex.Message);
            }
            return result;
        }

        public static async Task<int> ModificarAsync(Category pCategory)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    bool existeNombre = await ExisteNombre(pCategory, dbContexto);
                    if (existeNombre == false)
                    {
                        var category = await dbContexto.Category.FirstOrDefaultAsync(
                            c => c.CategoryId == pCategory.CategoryId);

                        category.Name = pCategory.Name;
                        category.Description = pCategory.Description;
                        category.StatusId = pCategory.StatusId;
                        category.CreatedByUser = pCategory.CreatedByUser;

                        dbContexto.Update(category);
                        result = await dbContexto.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("El nombre de la categoría ya existe.");
                    }
                }
            }
            catch (Exception ex)
            {
                result = 0;
                throw new Exception(ex.Message);
            }
            return result;
        }

        public static async Task<int> EliminarAsync(Category pCategory)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var category = await dbContexto.Category.FirstOrDefaultAsync(
                        c => c.CategoryId == pCategory.CategoryId);

                    if (category != null)
                    {
                        dbContexto.Remove(category);
                        result = await dbContexto.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception("La categoría no fue encontrada.");
                    }
                }
            }
            catch (Exception ex)
            {
                result = 0;
                throw new Exception(ex.Message);
            }
            return result;
        }

        public static async Task<Category> ObtenerPorIdAsync(Category pCategory)
        {
            var result = new Category();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Category
                        .Include(c => c.Status)       // StatusId → navegación
                        .FirstOrDefaultAsync(c => c.CategoryId == pCategory.CategoryId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public static async Task<List<Category>> ObtenerTodosAsync(Category pCategory)
        {
            var result = new List<Category>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    // Filtros opcionales según lo que venga en pCategory
                    result = await dbContexto.Category
                        .Include(c => c.Status)
                        .Where(c =>
                            (pCategory.Name == null || c.Name.Contains(pCategory.Name)) &&
                            (pCategory.StatusId == 0 || c.StatusId == pCategory.StatusId)
                        )
                        .OrderBy(c => c.Name)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        #endregion
    }

}