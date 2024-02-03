namespace Core.Entities.Models
{
    public class Experence : Entity
    {
        public Experence()
        {
            ExperenceDetails = new List<ExperenceDetails>();
        }
        public string? Name { get; set; }
        public string? Descripcion { get; set; }
        public List<ExperenceDetails> ExperenceDetails { get; set; }
    }

    public class ExperenceDetails : Entity
    {
        public string? Name { get; set; }
        public string? Descripcion { get; set; }
        public string? Desde { get; set; }
        public string? Hasta { get; set; }
        public int Orden { get; set; }
    }
}