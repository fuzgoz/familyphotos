using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FamilyPhotos.Entities;

[XmlRoot("Folder")]
public partial class Folder
{

    [XmlElement("Id")]
    public int Id { get; set; }

    [XmlElement("Name")]
    public string Name { get; set; } = null!;

    [XmlElement("Date")]
    public DateOnly? Date { get; set; }

    [XmlElement("Location")]
    public string? Location { get; set; }

    [XmlElement("Color")]
    public string? Color { get; set; }

    [XmlElement("IsFavorite")]
    public bool IsFavorite { get; set; }

    [XmlIgnore]
    public string UserId { get; set; } = null!; // Idegen kulcs (Identity User ID)

    [XmlIgnore]
    public DateTime? CreatedAt { get; set; }

    [XmlIgnore]
    public ApplicationUser User { get; set; } = null!; // Navigációs tulajdonság

    [XmlIgnore]
    public ICollection<FolderPhoto> FolderPhotos { get; set; } = new List<FolderPhoto>();
    
    [XmlIgnore]
    public ICollection<FolderPerson> FolderPeople { get; set; } = new List<FolderPerson>();

}