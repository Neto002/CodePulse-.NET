using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;
        private const int FileSizeLimit = 10485760;

        public ImagesController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        // GET: {apibaseurl}/api/images
        [HttpGet]
        public async Task<IActionResult> GetAllImages()
        {
            // Call repository to call images
            var images = await _imageRepository.GetAll();

            // Convert model to DTO
            var response = new List<BlogImageDto>();

            images.ToList().ForEach(image => response.Add(new BlogImageDto
            {
                Id = image.Id,
                Title = image.Title,
                DateCreated = image.DateCreated,
                FileExtension = image.FileExtension,
                FileName = image.FileName,
                Url = image.Url
            }));

            return Ok(response);
        }

        // POST: {apibaseurl}/api/images
        [HttpPost]
        public async Task<IActionResult> UploadImage(
            [FromForm] IFormFile file,
            [FromForm] string fileName,
            [FromForm] string title)
        {
            ValidateFileUpload(file);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // File Upload
            var blogImage = new BlogImage
            {
                FileExtension = Path.GetExtension(file.FileName).ToLower(),
                FileName = fileName,
                Title = title,
                DateCreated = DateTime.Now
            };

            blogImage = await _imageRepository.Upload(file, blogImage);

            // Convert Model to DTO
            var response = new BlogImageDto
            {
                Id = blogImage.Id,
                Title = blogImage.Title,
                DateCreated = blogImage.DateCreated,
                FileExtension = blogImage.FileExtension,
                FileName = blogImage.FileName,
                Url = blogImage.Url
            };

            return Ok(response);
        }

        private void ValidateFileUpload(IFormFile file)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

            if (!allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                ModelState.AddModelError("file", "Unsupported file format");
            };

            if (file.Length > FileSizeLimit)
            {
                ModelState.AddModelError("file", "File size cannot be more than 10 MB");
            }
        }
    }
}
