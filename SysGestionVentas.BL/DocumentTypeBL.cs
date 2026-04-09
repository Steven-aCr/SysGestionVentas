using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BDGestionVentas.BL
{
    /// <summary>
    /// Capa de lógica de negocio para la entidad <see cref="DocumentType"/>.
    /// Actúa como intermediario entre la capa de presentación y la capa DAL,
    /// delegando las operaciones CRUD a <see cref="DocumentTypeDAL"/>.
    /// Esta entidad define los tipos de documentos del sistema
    /// (facturas, notas de crédito, cotizaciones, etc.) y requiere
    /// que el campo <c>Name</c> sea único.
    /// </summary>
    public class DocumentTypeBL
    {
        #region "CRUD"

        /// <summary>
        /// Guarda un nuevo tipo de documento en la base de datos de forma asíncrona,
        /// invocando la lógica de validación definida en <see cref="DocumentTypeDAL.GuardarAsync"/>.
        /// </summary>
        /// <param name="pDocType">
        /// Objeto <see cref="DocumentType"/> con los datos del nuevo tipo de documento.
        /// El campo <c>Name</c> es requerido y debe ser único.
        /// El campo <c>Description</c> es opcional.
        /// </param>
        /// <returns>
        /// Número de filas afectadas en la base de datos.
        /// Retorna <c>1</c> si el tipo de documento fue guardado correctamente,
        /// <c>0</c> si ocurrió algún error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el <c>Name</c> del tipo de documento ya existe, o si ocurre
        /// cualquier error durante la operación.
        /// </exception>
        public async Task<int> GuardarAsync(DocumentType pDocType)
        {
            return await DocumentTypeDAL.GuardarAsync(pDocType);
        }

        /// <summary>
        /// Modifica los datos de un tipo de documento existente de forma asíncrona,
        /// invocando la lógica de actualización definida en <see cref="DocumentTypeDAL.ModificarAsync"/>.
        /// </summary>
        /// <param name="pDocType">
        /// Objeto <see cref="DocumentType"/> con los datos actualizados.
        /// El campo <c>DocTypeId</c> es requerido para identificar el registro a modificar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas en la base de datos.
        /// Retorna <c>1</c> si la modificación fue exitosa, <c>0</c> si ocurrió algún error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el <c>Name</c> ya está en uso por otro tipo de documento, o si
        /// ocurre cualquier error durante la operación.
        /// </exception>
        public async Task<int> ModificarAsync(DocumentType pDocType)
        {
            return await DocumentTypeDAL.ModificarAsync(pDocType);
        }

        /// <summary>
        /// Elimina un tipo de documento de la base de datos de forma asíncrona,
        /// invocando la lógica de eliminación definida en <see cref="DocumentTypeDAL.EliminarAsync"/>.
        /// </summary>
        /// <param name="pDocType">
        /// Objeto <see cref="DocumentType"/> que debe contener el <c>DocTypeId</c>
        /// del tipo de documento a eliminar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas en la base de datos.
        /// Retorna <c>1</c> si la eliminación fue exitosa, <c>0</c> si ocurrió algún error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre cualquier error durante la operación de eliminación.
        /// Tener en cuenta que si el tipo de documento tiene registros relacionados
        /// en la tabla <c>Document</c>, la base de datos rechazará la eliminación
        /// por integridad referencial.
        /// </exception>
        public async Task<int> EliminarAsync(DocumentType pDocType)
        {
            return await DocumentTypeDAL.EliminarAsync(pDocType);
        }

        /// <summary>
        /// Obtiene la lista completa de tipos de documento registrados en la base de datos
        /// de forma asíncrona. Esta entidad no tiene relaciones de navegación,
        /// por lo que no se aplica ningún <c>Include</c>.
        /// </summary>
        /// <param name="pDocType">
        /// Objeto <see cref="DocumentType"/> utilizado como parámetro de entrada.
        /// En esta versión no se aplican filtros; se retornan todos los registros.
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="DocumentType"/>.
        /// Retorna una lista vacía si no hay registros o si ocurre un error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre cualquier error durante la consulta.
        /// </exception>
        public async Task<List<DocumentType>> ObtenerTodosAsync(DocumentType pDocType)
        {
            return await DocumentTypeDAL.ObtenerTodosAsync(pDocType);
        }

        /// <summary>
        /// Obtiene un tipo de documento específico de la base de datos de forma asíncrona,
        /// buscándolo por su <c>DocTypeId</c>.
        /// </summary>
        /// <param name="pDocType">
        /// Objeto <see cref="DocumentType"/> que debe contener el <c>DocTypeId</c>
        /// del tipo de documento a buscar.
        /// </param>
        /// <returns>
        /// Objeto <see cref="DocumentType"/> si fue encontrado;
        /// un objeto vacío si no existe el registro.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre cualquier error durante la consulta.
        /// </exception>
        public async Task<DocumentType> ObtenerPorIdAsync(DocumentType pDocType)
        {
            return await DocumentTypeDAL.ObtenerPorIdAsync(pDocType);
        }

        public async Task<IEnumerable> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task EliminarAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> ObtenerTodosAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<string?> ObtenerPorIdAsync(int value)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
