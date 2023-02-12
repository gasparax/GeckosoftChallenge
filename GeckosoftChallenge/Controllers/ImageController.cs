using GeckosoftChallenge.Persistance;
using GeckosoftChallenge.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
namespace GeckosoftChallenge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {

        private readonly ImageRepository ImageRepository;
        public static readonly List<string> ImageExtensions = new List<string> { ".jpg", ".jpeg", ".jpe", ".bmp", ".gif", ".png" };

        public ImageController()
        {
            ImageRepository = new ImageRepository();
        }

        [HttpGet("all")]
        public IActionResult GetImages()
        {
            List<string> imagesNames = ImageRepository.GetImagesNamesList();
            if (!imagesNames.Any())
            {
                return NotFound("No images has been updated.");
            }
            string jsonListResponse = JsonSerializer.Serialize(imagesNames);
            return Ok(jsonListResponse);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || !ImageExtensions.Contains(Path.GetExtension(image.FileName)))
            {
                return BadRequest("The selected file is not an image. Load a image file.");
            }
            long id = await ImageRepository.SaveImageAsync(image);
            if (id == -1)
            {
                return StatusCode(500, "Error on uploading.");
            }
            Dictionary<string, string> response = new Dictionary<string, string>();
            response["fileName"] = image.FileName;
            response["id"] = id.ToString();
            return Ok(JsonSerializer.Serialize(response));
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteImage(long id)
        {
            if (ImageRepository.DeleteImage(id))
            {
                return Ok("Image Deleted.");
            }
            return NotFound("Image not found.");
        }

        [HttpPut("{id}")]
        public IActionResult UpdateImage(long id, UpdateImageRequest updateImageRequest)
        {
            if (updateImageRequest.Width <= 0 || updateImageRequest.Height <= 0)
            {
                return BadRequest("Width and Height must be greater then 0.");
            }
            if(ImageRepository.ResizeImage(id, updateImageRequest.Width, updateImageRequest.Height))
            {
                return Ok("Image size updated.");
            };
            return NotFound("Image not found.");
        }
    }

}