using SysGestionVentas.EN;
using Microsoft.EntityFrameworkCore;
using SysGestionVentas.DAL;

namespace BDGestionVentas.DAL
{
    public class DocumentDAL
    {
        public static async Task<int> GuardarAsync(Document pDocument)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    dbContexto.Add(pDocument);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<int> ModificarAsync(Document pDocument)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var document = await dbContexto.Document.FirstOrDefaultAsync(
                        d => d.DocumentId == pDocument.DocumentId);

                    document.DocTypeId = pDocument.DocTypeId;
                    document.DocNumber = pDocument.DocNumber;
                    document.IssueDate = pDocument.IssueDate;
                    document.PersonId = pDocument.PersonId;

                    dbContexto.Update(document);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<int> EliminarAsync(Document pDocument)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var document = await dbContexto.Document.FirstOrDefaultAsync(
                        d => d.DocumentId == pDocument.DocumentId);

                    dbContexto.Remove(document);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<Document> ObtenerPorIdAsync(Document pDocument)
        {
            var result = new Document();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Document
                        .Include(d => d.DocumentType)
                        .Include(d => d.Person)
                        .FirstOrDefaultAsync(d => d.DocumentId == pDocument.DocumentId);
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<List<Document>> ObtenerTodosAsync(Document pDocument)
        {
            var result = new List<Document>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Document
                        .Include(d => d.DocumentType)
                        .Include(d => d.Person)
                        .Where(d =>
                            (pDocument.DocNumber == null || d.DocNumber.Contains(pDocument.DocNumber)) &&
                            (pDocument.DocTypeId == 0 || d.DocTypeId == pDocument.DocTypeId) &&
                            (pDocument.PersonId == 0 || d.PersonId == pDocument.PersonId)
                        )
                        .OrderBy(d => d.IssueDate)
                        .ToListAsync();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }
    }
}