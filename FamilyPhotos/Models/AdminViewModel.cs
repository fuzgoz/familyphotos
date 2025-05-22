using FamilyPhotos.Entities;
using Microsoft.AspNetCore.Identity;


namespace FamilyPhotos.Models
{
    public class AdminViewModel
    {
        public List<Folder> Folders { get; set; }
        public IdentityUser User { get; set; }
    }
}
