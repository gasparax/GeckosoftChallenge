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

        /// <summary>
        /// Returns the list of the images uploaded in alphabetical order.
        /// </summary>
        /// <returns>JSON list of image names</returns>
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

        /// <summary>
        /// Upload an image.
        /// </summary>
        /// <param name="image"></param>
        /// <returns>JSON with the filename and an integer id</returns>
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

        /// <summary>
        /// Allow the deletion of an uploaded image.
        /// </summary>
        /// <param name="id">Image id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteImage(long id)
        {
            if (ImageRepository.DeleteImage(id))
            {
                return Ok("Image Deleted.");
            }
            return NotFound("Image not found.");
        }

        /// <summary>
        /// Allow the resize of an uploaded image. 
        /// </summary>
        /// <param name="id">Image id</param>
        /// <param name="updateImageRequest">Height and Width in pixel</param>
        /// <returns>Ok</returns>
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