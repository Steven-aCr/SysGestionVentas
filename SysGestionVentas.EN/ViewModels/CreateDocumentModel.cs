using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.EN.ViewModels
{
    /// <summary>
    /// ViewModel utilizado para capturar en un único formulario los datos necesarios
    /// para crear un nuevo <see cref="Client"/> (con su <see cref="Person"/> asociada),
    /// un <see cref="Document"/> con todas sus <see cref="DocumentDetailLineModel"/>
    /// en una sola transacción atómica.
    /// El campo <c>PersonId</c> no se captura desde el formulario; se asigna
    /// internamente en la capa BL tras persistir la <see cref="Person"/>.
    /// </summary>
    public class CreateDocumentModel
    {
        // ── Encabezado del documento ──────────────────────────────────────────

        /// <summary>
        /// Identificador del tipo de documento (FK a <c>DocumentType</c>).
        /// </summary>
        [Required(ErrorMessage = "El tipo de documento es obligatorio.")]
        [Display(Name = "Tipo de documento")]
        public int DocTypeId { get; set; }

        /// <summary>
        /// Número único del documento (ej. FAC-0001).
        /// </summary>
        [Required(ErrorMessage = "El número de documento es obligatorio.")]
        [StringLength(50, MinimumLength = 3,
            ErrorMessage = "El número de documento debe tener entre 3 y 50 caracteres.")]
        [Display(Name = "Número de documento")]
        public string DocNumber { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de emisión del documento.
        /// </summary>
        [Required(ErrorMessage = "La fecha de emisión es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de emisión")]
        public DateTime IssueDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Identificador del estado inicial del documento.
        /// </summary>
        [Required(ErrorMessage = "El estado es obligatorio.")]
        [Display(Name = "Estado")]
        public int StatusId { get; set; }

        /// <summary>
        /// Identificador del usuario autenticado que crea el documento.
        /// Se asigna automáticamente desde el claim de sesión en el controlador;
        /// no debe ser enviado desde el formulario.
        /// </summary>
        public int CreatedByUser { get; set; }

        /// <summary>
        /// Identificador de la <see cref="Person"/> creada en la transacción.
        /// Se asigna internamente en la capa BL; no se captura desde el formulario.
        /// </summary>
        public int PersonId { get; set; }

        // ── Datos del nuevo cliente (Person + Client) ─────────────────────────

        /// <summary>
        /// Nombre del contacto del cliente. Requerido para registrar la <see cref="Person"/>.
        /// </summary>
        [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
        [Display(Name = "Nombre del cliente")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Apellido del contacto del cliente. Requerido para registrar la <see cref="Person"/>.
        /// </summary>
        [Required(ErrorMessage = "El apellido del cliente es obligatorio.")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres.")]
        [Display(Name = "Apellido del cliente")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Dirección del cliente. Requerida para registrar la <see cref="Person"/>.
        /// </summary>
        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(255)]
        [Display(Name = "Dirección")]
        public string Adress { get; set; } = string.Empty;

        /// <summary>
        /// Número de teléfono del cliente. Requerido para registrar la <see cref="Person"/>.
        /// Formato esperado: 1234-5678.
        /// </summary>
        [Required(ErrorMessage = "El teléfono del cliente es obligatorio.")]
        [Phone(ErrorMessage = "Formato de número de teléfono inválido.")]
        [StringLength(20)]
        [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "Formato: 1234-5678")]
        [Display(Name = "Teléfono")]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// DUI del cliente (opcional). Si se proporciona, debe ser único en el sistema.
        /// Formato esperado: 12345678-9.
        /// </summary>
        [RegularExpression(@"^\d{8}-\d$", ErrorMessage = "Formato: 12345678-9")]
        [StringLength(10)]
        [Display(Name = "DUI")]
        public string? Dui { get; set; }

        /// <summary>
        /// Estado de la <see cref="Person"/> del cliente al momento de su creación.
        /// Por convención se inicializa en 1 (Activo) y no se expone en el formulario.
        /// </summary>
        public int PersonStatusId { get; set; } = 1;

        // ── Líneas de detalle ─────────────────────────────────────────────────

        /// <summary>
        /// Colección de líneas de detalle capturadas dinámicamente desde el formulario.
        /// Debe contener al menos un elemento para que el documento sea válido.
        /// </summary>
        public List<CreateDocumentDetailModel> Detalles { get; set; } = new();
    }
}