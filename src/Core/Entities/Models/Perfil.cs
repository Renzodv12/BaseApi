using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entities.Enum;

namespace Core.Entities.Models
{
    public class Perfil : Entity
    {
        public string? Name { get; set; }
        public string? Descripcion { get; set; }
        public string? ImageSrc { get; set; }
    }
}