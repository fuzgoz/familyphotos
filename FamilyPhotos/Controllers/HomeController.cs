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
                .OrderByDescending(p => p.IsFavorite) //kedvencek legyenek el�l
                .ToListAsync();

            HomeViewModel viewModel = new HomeViewModel
            {
                Folders = folders,
            };

            // A n�zetnek �tadjuk a modellt
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Filter(DateOnly? date, string? person, string? location)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var foldersQuery = _context.Folders.AsQueryable();

            // D�tum szerinti sz�r�s, ha van
            if (date.HasValue)
            {
                foldersQuery = foldersQuery.Where(f => f.UserId == userId && f.Date.HasValue && f.Date.Value == date.Value);
            }

            // Szem�ly szerinti sz�r�s, ha van
            if (!string.IsNullOrEmpty(person))
            {
                foldersQuery = foldersQuery.Where(f => f.UserId == userId &&
                                                        f.FolderPeople.Any(fp => fp.Person.Name == person));
            }

            // Helysz�n szerinti sz�r�s, ha van
            if (!string.IsNullOrEmpty(location))
            {
                foldersQuery = foldersQuery.Where(f => f.UserId == userId && f.Location.Contains(location));
            }

            var folders = await foldersQuery
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.Date)
                .ToListAsync();

            // Az eredm�nyeket �tadjuk a n�zetnek
            var viewModel = new FilterViewModel
            {
                Folders = folders,
                SearchDate = date,
                SearchPerson = person,
                SearchLocation = location
            };

            return View("Filter", viewModel); // Egy m�sik n�zetet adunk vissza
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

            // A n�zetnek �tadjuk a modellt
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
                // Ellen�rizz�k, hogy l�tezik-e az uploads mappa, ha nem, l�trehozzuk
                if (!Directory.Exists(_dataPath))
                {
                    Directory.CreateDirectory(_dataPath);
                }

                // A f�jl ment�se az uploads mapp�ba
                var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(_dataPath, fileName);

                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // A f�jl adatainak ment�se az adatb�zisba
                    var photo = new Photo
                    {
                        FilePath = $"/uploads/{fileName}", // A f�jl el�r�si �tja
                        TimeStamp = DateTime.Now, // A felt�lt�s d�tuma
                        UserId = userId
                    };

                    _context.Photos.Add(photo);

                    // Ha siker�l a ment�s, akkor az adatb�zisban is t�roljuk
                    await _context.SaveChangesAsync();

                    ViewBag.Message = "K�p sikeresen felt�ltve!";
                    var photos = await _context.Photos.ToListAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Hiba kezel�s: ha b�rmi hiba t�rt�nik, ki�rjuk
                    ViewBag.Message = $"Hiba t�rt�nt a f�jl ment�se k�zben: {ex.Message}";
                }
            }
            else
            {
                ViewBag.Message = "Nem v�lasztott�l ki f�jlt!";
            }

            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> NewFolder(NewFolderViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                // �j mappa l�trehoz�sa
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
                await _context.SaveChangesAsync(); // Mappa ment�se

                // Szem�lyek hozz�ad�sa a mapp�hoz
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
                            await _context.SaveChangesAsync(); // Szem�ly ment�se
                        }

                        var folderPerson = new FolderPerson
                        {
                            FolderId = newFolder.Id,
                            PersonId = person.Id
                        };

                        // A FolderPerson entit�s hozz�ad�sa
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
