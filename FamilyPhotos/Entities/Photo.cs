using System;
using System.Collections.Generic;

namespace FamilyPhotos.Entities;

public partial class Photo
{
    public int Id { get; set; }

    public string FilePath { get; set; } = null!;

    public DateTime TimeStamp { get; set; }

    public string UserId { get; set; } = null!; // Idegen kulcs az Identity User-hez

    public ApplicationUser User { get; set; } = null!; // Navigációs tulajdonság

    public ICollection<FolderPhoto> FolderPhotos { get; set; }
}
