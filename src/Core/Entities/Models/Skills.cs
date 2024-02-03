namespace Core.Entities.Models
{
    public class Skills : Entity
    {
        public Skills()
        {
            SkillsDetails = new List<SkillsDetails>();
        }
        public string? Descripcion { get; set; }
        public string? LogoUrl { get; set; }
        public bool ShowLogo { get; set; }
        public List<SkillsDetails> SkillsDetails { get; set; }
    }
    public class SkillsDetails : Entity
    {
        public string? Name { get; set; }
        public string? IconName { get; set; }
    }

}