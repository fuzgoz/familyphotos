using System;
using System.Collections.Generic;

namespace FamilyPhotos.Entities;

public partial class Person
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public ICollection<FolderPerson> FolderPeople { get; set; } = new List<FolderPerson>();

}
