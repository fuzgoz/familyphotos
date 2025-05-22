using FamilyPhotos.Entities;

namespace FamilyPhotos.Models
{
    public class FolderViewModel
    {

        public List<Photo> Photos { get; set; } = new List<Photo>();

        public Folder Folder { get; set; } = new();

        public List<Person> People { get; set; } = new();

    }
}
