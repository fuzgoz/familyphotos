using Microsoft.AspNetCore.Mvc;
using FamilyPhotos.Entities;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FamilyPhotos.Controllers
{
    [Authorize]
    [Route("Image")]
    public class ImageController : Controller
    {
        private readonly FamilyPhotoContext _context;
        private readonly string _uploadsFolder;

        public ImageController(FamilyPhotoContext context)
        {
            _context = context;
            _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }
        }

        // Listázás az index nézetre
        public async Task<IActionResult> Index()
        {
            var photos = await _context.Photos
                .OrderByDescending(p => p.TimeStamp)
                .ToListAsync();
            return View(photos); // A View modellje: List<Photo>
        }

        // Kép feltöltése drag-and-drop vagy form-al
        [HttpPost]
        [Route("Upload/{folderId}")]
        public async Task<IActionResult> Upload(int folderId, IFormFile image)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (image == null || image.Length == 0)
            {
                return BadRequest(new { message = "Nincs fájl." });
            }

            var folder = await _context.Folders.FindAsync(folderId);
            if (folder == null)
            {
                return NotFound(new { message = "Mappa nem található." });
            }

            // Generálunk egy fájlnevet és elmentjük a fájlt
            var fileName = Path.GetRandomFileName() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(_uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Mentés az adatbázisba
            var photo = new Photo
            {
                FilePath = "/uploads/" + fileName, // Webes útvonal
                TimeStamp = DateTime.Now,
                UserId =  userId
            };
            _context.Photos.Add(photo);

            await _context.SaveChangesAsync();

            var folderPhoto = new FolderPhoto
            {
                FolderId = folder.Id,
                PhotoId = photo.Id
            };
            _context.FolderPhotos.Add(folderPhoto);
            await _context.SaveChangesAsync();


            return Ok(new { message = "Fénykép sikeresen feltöltve", filePath = photo.FilePath });
        }

        // Törlés
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var photo = await _context.Photos.FindAsync(id);
            if (photo == null)
            {
                return NotFound(new { message = "Photo not found." });
            }

            // Törlés a szerverről
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", photo.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Törlés az adatbázisból
            _context.Photos.Remove(photo);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
