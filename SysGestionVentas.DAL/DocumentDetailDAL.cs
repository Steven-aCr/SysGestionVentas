using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysGestionVentas.DAL
{
    public class DocumentDetailDAL
    {
        public static async Task<int> GuardarAsync(DocumentDetail detail, DbContexto dbContexto)
        {
            dbContexto.Add(detail);
            return await dbContexto.SaveChangesAsync();
        }

        // Obtener todos los detalles de un documento específico
        public static async Task<List<DocumentDetail>> ObtenerPorDocumentoAsync(int idDocumento, DbContexto dbContexto)
        {
            return await dbContexto.DocumentDetail
                .Where(d => d.IdDocument == idDocumento)
                .ToListAsync();
        }
    }
}