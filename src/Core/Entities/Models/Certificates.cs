namespace Core.Entities.Models
{

    public class Certificates : Entity
    {
        public string? Name { get; set; }
        public string? Descripcion { get; set; }
        public string? ImageSrc { get; set; }

        public string[]? ProgramsLanguages { get; set; }
        public string? ViewUrl { get; set; }
    }
}