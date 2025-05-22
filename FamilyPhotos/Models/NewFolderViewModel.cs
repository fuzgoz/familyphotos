namespace FamilyPhotos.Models
{
    public class NewFolderViewModel
    {
        public int? Id { get; set; }
        
        public string Name { get; set; }
        public DateOnly? Date { get; set; }
        public string? Location { get; set; }
        public string? Color { get; set; }

        public bool IsFavorite { get; set; } = false;

        public List<string> PersonNames { get; set; } = new List<string>();
    }

}
