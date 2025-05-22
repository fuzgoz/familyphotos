using FamilyPhotos.Entities;
using FamilyPhotos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Diagnostics;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;
using Chart.Mvc;
using System.Collections.Generic;

namespace FamilyPhotos.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FamilyPhotoContext _context;
        private readonly string _dataPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        public HomeController(ILogger<HomeController> logger, FamilyPhotoContext context)
        {
            _logger = logger;
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var folders = await _context.Folders
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.IsFavorite) //kedvencek legyenek elöl
                .ToListAsync();

            HomeViewModel viewModel = new HomeViewModel
            {
                Folders = folders,
            };

            // A nézetnek átadjuk a modellt
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Filter(DateOnly? date, string? person, string? location)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var foldersQuery = _context.Folders.AsQueryable();

            // Dátum szerinti szûrés, ha van
            if (date.HasValue)
            {
                foldersQuery = foldersQuery.Where(f => f.UserId == userId && f.Date.HasValue && f.Date.Value == date.Value);
            }

            // Személy szerinti szûrés, ha van
            if (!string.IsNullOrEmpty(person))
            {
                foldersQuery = foldersQuery.Where(f => f.UserId == userId &&
                                                        f.FolderPeople.Any(fp => fp.Person.Name == person));
            }

            // Helyszín szerinti szûrés, ha van
            if (!string.IsNullOrEmpty(location))
            {
                foldersQuery = foldersQuery.Where(f => f.UserId == userId && f.Location.Contains(location));
            }

            var folders = await foldersQuery
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.Date)
                .ToListAsync();

            // Az eredményeket átadjuk a nézetnek
            var viewModel = new FilterViewModel
            {
                Folders = folders,
                SearchDate = date,
                SearchPerson = person,
                SearchLocation = location
            };

            return View("Filter", viewModel); // Egy másik nézetet adunk vissza
        }



        public async Task<IActionResult> Favorites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
             

            var folders = await _context.Folders
                .Where(p => p.UserId == userId && p.IsFavorite)
                .ToListAsync();

            HomeViewModel viewModel = new HomeViewModel
            {
                Folders = folders,
            };

            // A nézetnek átadjuk a modellt
            return View(viewModel);
        }

        public IActionResult Statistics()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var foldersWithImageCount = _context.Folders
                .Where(u => u.UserId == userId)
                .Select(folder => new
                {
                    FolderName = folder.Name,
                    ImageCount = folder.FolderPhotos.Count
                })
                .ToList();

            return View(foldersWithImageCount);
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Logout", "Account");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (file != null && file.Length > 0)
            {
                // Ellenõrizzük, hogy létezik-e az uploads mappa, ha nem, létrehozzuk
                if (!Directory.Exists(_dataPath))
                {
                    Directory.CreateDirectory(_dataPath);
                }

                // A fájl mentése az uploads mappába
                var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(_dataPath, fileName);

                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // A fájl adatainak mentése az adatbázisba
                    var photo = new Photo
                    {
                        FilePath = $"/uploads/{fileName}", // A fájl elérési útja
                        TimeStamp = DateTime.Now, // A feltöltés dátuma
                        UserId = userId
                    };

                    _context.Photos.Add(photo);

                    // Ha sikerül a mentés, akkor az adatbázisban is tároljuk
                    await _context.SaveChangesAsync();

                    ViewBag.Message = "Kép sikeresen feltöltve!";
                    var photos = await _context.Photos.ToListAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Hiba kezelés: ha bármi hiba történik, kiírjuk
                    ViewBag.Message = $"Hiba történt a fájl mentése közben: {ex.Message}";
                }
            }
            else
            {
                ViewBag.Message = "Nem választottál ki fájlt!";
            }

            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> NewFolder(NewFolderViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                // Új mappa létrehozása
                var newFolder = new Folder
                {
                    UserId = userId,
                    Name = model.Name,
                    Date = model.Date, 
                    Location = model.Location,
                    Color = model.Color,
                    IsFavorite = model.IsFavorite,
                };

                _context.Folders.Add(newFolder);
                await _context.SaveChangesAsync(); // Mappa mentése

                // Személyek hozzáadása a mappához
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
                            await _context.SaveChangesAsync(); // Személy mentése
                        }

                        var folderPerson = new FolderPerson
                        {
                            FolderId = newFolder.Id,
                            PersonId = person.Id
                        };

                        // A FolderPerson entitás hozzáadása
                        _context.FolderPeople.Add(folderPerson);
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }

            //return View(model);
            return NotFound();
        }

        [HttpPost("Home/DeleteFolder/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var folder = await _context.Folders.FindAsync(id);
            if (folder == null)
            {
                return NotFound(new { message = "Folder not found." });
            }

            var folder_photos = await _context.FolderPhotos.Where(e => e.FolderId == id).ToListAsync();
            var photoIds = folder_photos.Select(fp => fp.PhotoId).ToList();
            var photos = await _context.Photos.Where(p => photoIds.Contains(p.Id)).ToListAsync();

            foreach (var photo in photos)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", photo.FilePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }



            var folderPeople = await _context.FolderPeople.Where(fp => fp.FolderId == id).ToListAsync();
            _context.FolderPeople.RemoveRange(folderPeople);

            _context.FolderPhotos.RemoveRange(folder_photos);
            _context.Photos.RemoveRange(photos);

            _context.Folders.Remove(folder);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { id = id });
        }


        [HttpPost]
        public async Task<IActionResult> AddPhotoToFolder(int folderId, int photoId)
        {
            var folder = await _context.Folders.FindAsync(folderId);
            if (folder == null)
                return NotFound("Folder not found.");

            var photo = await _context.Photos.FindAsync(photoId);
            if (photo == null)
                return NotFound("Photo not found. " + "folderId: " + folderId + ", photoId: " + photoId);

            _context.FolderPhotos.Add(new FolderPhoto
            {
                FolderId = folderId,
                PhotoId = photoId
            });


            await _context.SaveChangesAsync();
            return Ok("Photo(s) added to folder.");
        }


        public async Task<IActionResult> AddPersonToFolder(int personId, int folderId)
        {
            var folder = await _context.Folders.FindAsync(folderId);
            if (folder == null)
                return NotFound("Folder not found.");


            FolderPerson folderPerson = new FolderPerson
            {
                FolderId = folderId,
                PersonId = personId
            };

            _context.FolderPeople.Add(folderPerson);

            await _context.SaveChangesAsync();
            return Ok();
        }

        public async Task<IActionResult> AddPeopleToFolder(int folderId, List<int> personIds)
        {
            foreach (var personId in personIds)
            {
                var person = await _context.People.FindAsync(personId);
                if (person == null)
                {
                    return NotFound("Person not recognized.");
                }
                else
                {
                    await AddPersonToFolder(personId, folderId);
                }
            }

            await _context.SaveChangesAsync();

            return Ok("People added to folder.");
        }


    }

}
