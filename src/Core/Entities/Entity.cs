using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entities.Enum;
using Core.Interfaces;

namespace Core.Entities
{
    public abstract class Entity : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        public Language? Language { get; set; }

        public bool Estado { get; set; }
        public string? UserId { get; set; }
        public DateTime Fcreacion { get; set; } = DateTime.Now;
        public DateTime Fmodificacion { get; set; }
    }
}

