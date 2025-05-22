using FamilyPhotos.Entities;
using FamilyPhotos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Collections.Generic;
using System.IO.Compression;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace FamilyPhotos.Controllers
{
    [Authorize]
    public class FolderController : Controller
    {
        private readonly ILogger<FolderController> _logger;
        private readonly FamilyPhotoContext _context;
        private readonly string _dataPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        //int? folderId = null;

        public int? FolderId { get; set; }

        public FolderController(ILogger<FolderController> logger, FamilyPhotoContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("Folder/{Id}")]
        public async Task<IActionResult> Index(int Id)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var folder = GetFolderById(Id);

            if (folder == null)
            {
                return RedirectToAction("Index");
            }

            var images = await _context.FolderPhotos
                .Where(fp => fp.FolderId == Id)
                .Select(fp => fp.Photo)
                .OrderByDescending(p => p.TimeStamp)
                .ToListAsync();

            var people = await _context.FolderPeople
            .Where(fp => fp.FolderId == Id)
            .Select(fp => fp.Person)
            .ToListAsync();

            FolderViewModel viewModel = new FolderViewModel
            {
                Photos = images,
                Folder = folder,
                People = people
            };

            // A nézetnek átadjuk a modellt
            return View(viewModel);
        }


        [HttpGet("Folder/EditFolder/{folderId}")]
        public IActionResult EditFolderTest(int folderId)
        {
            return Ok($"EditFolder teszt működik, folderId: {folderId}");
        }

        [HttpPost("Folder/EditFolder/{folderId}")]
        public async Task<IActionResult> EditFolder(NewFolderViewModel model, int folderId)
        {
            var folder = GetFolderById(folderId);

            //if (folder == null)
            //{
            //    return NotFound();
            //}

            if (ModelState.IsValid)
            {
                folder.Name = model.Name;
                folder.Date = model.Date;
                folder.Location = model.Location;
                folder.Color = model.Color;
                folder.IsFavorite = model.IsFavorite;

                var existingPeople = _context.FolderPeople.Where(fp => fp.FolderId == folder.Id);
                _context.FolderPeople.RemoveRange(existingPeople);

                if (model.PersonNames != null && model.PersonNames.Any())
                {
                    foreach (var personName in model.PersonNames.Where(pn => !string.IsNullOrEmpty(pn)))
                    {
                        var person = await _context.People.FirstOrDefaultAsync(e => e.Name == personName);

                        if (person == null)
                        {
                            person = new Person
                            {
                                Name = personName
                            };

                            _context.People.Add(person);
                            await _context.SaveChangesAsync();
                        }

                        var folderPerson = new FolderPerson
                        {
                            FolderId = folder.Id,
                            PersonId = person.Id
                        };

                        _context.FolderPeople.Add(folderPerson);
                    }
                }

                await _context.SaveChangesAsync(); // Adatbázis mentése

                string stringFullUrl = $"~/Folder/{folderId}";

                // Átirányítás a generált URL-re
                return Redirect(stringFullUrl);

            }

            return NotFound();
        }




        [HttpPost("/Folder/Upload/{folderId}")]
        public async Task<IActionResult> UploadMultiple(IEnumerable<IFormFile> files, int folderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (files == null || !files.Any())
            {
                ViewBag.Message = "Nem választottál ki fényképet!";
                return View("Index");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                    continue;

                // Ellenőrizzük a fájl kiterjesztését
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
                {
                    ViewBag.Message = $"Nem támogatott fájltípus: {file.FileName}. Csak a következő formátumok engedélyezettek: .jpg, .jpeg, .png, .gif, .bmp";
                    continue;
                }

                try
                {
                    if (!Directory.Exists(_dataPath))
                    {
                        Directory.CreateDirectory(_dataPath);
                    }

                    // Egyedi fájlnév generálása
                    var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(_dataPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var newPhoto = new Photo
                    {
                        FilePath = $"/uploads/{fileName}",
                        TimeStamp = DateTime.Now,
                        UserId = userId
                    };

                    _context.Photos.Add(newPhoto);
                    await _context.SaveChangesAsync();

                    _context.FolderPhotos.Add(new FolderPhoto
                    {
                        FolderId = folderId,
                        PhotoId = newPhoto.Id
                    });
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"Hiba történt a fájl mentése közben: {ex.Message}";
                    continue;
                }
            }

            await _context.SaveChangesAsync();

            ViewBag.Message = "A fájlok sikeresen feltöltve!";
            return RedirectToAction("Index", new { id = folderId });
        }







        [HttpPost("/Folder/Download/{folderId}")]
        public async Task<IActionResult> Download(int folderId)
        {
            var folder = GetFolderById(folderId);

            if (folder == null)
            {
                return NotFound("Folder not found.");
            }

            var photoIds = await _context.FolderPhotos
                .Where(fp => fp.FolderId == folderId)
                .Select(fp => fp.PhotoId)
                .ToListAsync();

            var photos = await _context.Photos
                .Where(p => photoIds.Contains(p.Id))
                .ToListAsync();

            string zipFileName;
            if (folder.Date.HasValue)
            {
                zipFileName = $"{folder.Name}_{folder.Date?.ToString("yyyy-MM-dd")}.zip";
            }
            else
            {
                zipFileName = $"{folder.Name}_no-date.zip";
            }
            
            var zipFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "downloads", zipFileName);

            var downloadPath = Path.GetDirectoryName(zipFilePath);
            if (!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }


            using (var zipToCreate = new ZipArchive(new FileStream(zipFilePath, FileMode.Create), ZipArchiveMode.Update))
            {
                foreach (var photo in photos)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", photo.FilePath.TrimStart('/'));

                    if (System.IO.File.Exists(filePath))
                    {
                        var fileName = Path.GetFileName(filePath);
                        var zipEntry = zipToCreate.CreateEntry(fileName, CompressionLevel.Fastest);

                        using (var entryStream = zipEntry.Open())
                        using (var fileStream = new FileStream(filePath, FileMode.Open))
                        {
                            await fileStream.CopyToAsync(entryStream);
                        }
                    }
                }
            }

            var zipFileBytes = System.IO.File.ReadAllBytes(zipFilePath);
            return File(zipFileBytes, "application/zip", zipFileName);
        }


        #region Private Methods

        private Folder GetFolderById(int id)
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var folder = _context.Folders.FirstOrDefault(p => p.Id == id/* && p.UserId == userId*/);

            return folder;
        }

        #endregion

    }
}
