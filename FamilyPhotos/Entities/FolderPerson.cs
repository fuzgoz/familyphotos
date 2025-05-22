using System;
using System.Collections.Generic;

namespace FamilyPhotos.Entities;

public partial class FolderPerson
{
    public int FolderId { get; set; }

    public int PersonId { get; set; }

    public virtual Folder Folder { get; set; } = null!;

    public virtual Person Person { get; set; } = null!;
}
