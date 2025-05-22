using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace FamilyPhotos.Entities;

public class ApplicationUser : IdentityUser
{
    public ICollection<Folder> Folders { get; set; } = new List<Folder>();

    public ICollection<Photo> Photos { get; set; } = new List<Photo>();
}
