using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Runtime.Intrinsics.X86;
using Core.Entities.Enum;

namespace Core.Entities.Models
{

    public class Projects : Entity
    {

        public string? Name { get; set; }
        public string? Descripcion { get; set; }
        public string[]? ProgramsLanguages { get; set; }
        public string? ViewUrl { get; set; }
        public string? CodeUrl { get; set; }

    }
}