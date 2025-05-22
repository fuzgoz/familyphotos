using FamilyPhotos.Entities;
using FamilyPhotos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;

namespace FamilyPhotos.Controllers
{



    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly FamilyPhotoContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(FamilyPhotoContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return RedirectToAction("ManageUsers", "Admin");
        }

        public async Task<IActionResult> List()
        {
            var folders = await _context.Folders
                .ToListAsync();


            // ViewModel létrehozása
            AdminViewModel viewModel = new()
            {
                Folders = folders
            };

            return View(viewModel);
        }

        public async Task<IActionResult> ManageUsers()
        {
            var users = _userManager.Users.ToList();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                userViewModels.Add(new UserViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    IsAdmin = isAdmin
                });
            }

            return View(userViewModels);
        }

        public async Task<IActionResult> MakeAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            await _userManager.AddToRoleAsync(user, "Admin");
            return Redirect("ManageUsers");
        }


        //teszt szempont
        public async Task<IActionResult> ToggleAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            return Redirect("ManageUsers");
        }

        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return RedirectToAction("ManageUsers");
        }


        public async Task<IActionResult> Statistics()
        {
            var folders = await _context.Folders
            .Include(f => f.FolderPhotos) // Mappák fotóinak betöltése
            .ThenInclude(fp => fp.Photo) // Fotók betöltése
            .OrderBy(f => f.Date)
            .ToListAsync();

            var cumulativeData = new List<object>();
            int cumulativeSum = 0;
            int noDateCount = 0; // A 'Date' nélküli mappák száma

            foreach (var folder in folders)
            {
                if (folder.FolderPhotos != null)
                {
                    if (folder.Date.HasValue)
                    {
                        // Ha van dátum, akkor a megszokott módon adunk hozzá adatokat
                        cumulativeSum += folder.FolderPhotos.Count();
                        cumulativeData.Add(new
                        {
                            Date = folder.Date?.ToString("yyyy-MM-dd"),
                            ImageCount = folder.FolderPhotos.Count()
                        });
                    }
                    else
                    {
                        // Ha nincs dátum, összesítjük a képeket, és hozzáadjuk az adatokhoz
                        noDateCount += folder.FolderPhotos.Count();
                    }
                }
            }

            // Ha van dátum nélküli mappa, adjuk hozzá egy külön sorban
            if (noDateCount > 0)
            {
                cumulativeData.Add(new
                {
                    Date = "Nincs Dátum",
                    ImageCount = noDateCount
                });
            }

            // Adatok átadása a View-ba
            return View(cumulativeData);
        }

        [HttpPost("/Admin/DownloadXML/{folderId}")]
        public async Task<IActionResult> DownloadXML(int folderId)
        {
            var folder = await _context.Folders
                .Where(f => f.Id == folderId)
                .FirstOrDefaultAsync();

            if (folder == null)
            {
                return NotFound();
            }

            var xmlSerializer = new XmlSerializer(typeof(Folder));

            using (var memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, folder);

                var xmlData = memoryStream.ToArray();

                return File(xmlData, "application/xml", "folder.xml");
            }
        }


        public IActionResult Settings()
        {
            return View();
        }
    }
}
