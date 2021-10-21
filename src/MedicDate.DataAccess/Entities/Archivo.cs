﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicDate.DataAccess.Entities
{
    public class Archivo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string Id { get; set; } = default!;

        [StringLength(1000)]
        [Column(TypeName = "varchar(1000)")]
        [Required]
        public string RutaArchivo { get; set; } = default!;

        [StringLength(300)] public string? Descripcion { get; set; }

        public string CitaId { get; set; } = default!;
        public Cita Cita { get; set; } = default!;
    }
}