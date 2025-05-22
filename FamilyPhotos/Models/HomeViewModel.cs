using FamilyPhotos.Entities;

namespace FamilyPhotos.Models
{
    public class HomeViewModel
    {
        public List<Photo> Photos { get; set; } = new List<Photo>();
        
        public List<Folder> Folders { get; set; } = new List<Folder>();

        public int? ActiveFolder { get; set; } = null;

        public List<Person>? People { get; set; } = null;

        public DateOnly? SearchDate { get; set; }
        public string? SearchPerson { get; set; } = null;
        public string? SearchLocation { get; set; } = null;

    }
}
