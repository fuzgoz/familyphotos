using System;
using System.Collections.Generic;

namespace FamilyPhotos.Entities;

public partial class FolderPhoto
{
    public int FolderId { get; set; }

    public int PhotoId { get; set; }

    public virtual Folder Folder { get; set; } = null!;

    public virtual Photo Photo { get; set; } = null!;
}
