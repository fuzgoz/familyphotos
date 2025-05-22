using FamilyPhotos.Entities;

namespace FamilyPhotos.Models
{
    public class FilterViewModel
    {
        public List<Folder>? Folders { get; set; }
        public DateOnly? SearchDate { get; set; }
        public string? SearchPerson { get; set; }
        public string? SearchLocation { get; set; }
    }
}
