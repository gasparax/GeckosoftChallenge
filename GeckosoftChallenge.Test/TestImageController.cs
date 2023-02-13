using GeckosoftChallenge.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using System.Drawing;
using System.Text.Json;

namespace GeckosoftChallenge.Test
{
    [TestClass]
    public class TestImageController
    {
        [TestMethod]
        public void Get_OnFail_Return404()
        {
            //Arrange
            var controller = new ImageController();

            //Act
            var response = (NotFoundObjectResult)controller.GetImages();
            //Assert
            Assert.AreEqual(response.Value, "No images has been updated.");
        }

        [TestMethod]
        public async Task Get_OnSuccess_Return200Async()
        {
            //Arrange
            var controller = new ImageController();
            string pathToFiles = @"..\..\..\testImages\";
            string[] fileNames = Directory.GetFiles(pathToFiles);
            foreach (var fileName in fileNames)
            {
                Console.WriteLine("Loading " + fileName);
                using var stream = File.OpenRead(fileName);
                FormFile file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(fileName));
                var uploadResponse = (OkObjectResult)await controller.UploadImage(file);
                Console.WriteLine("Image loaded " + uploadResponse.Value as string);
            }
            //Act
            var response = (OkObjectResult)controller.GetImages();
            string? jsonResponse = response.Value as string;
            Console.WriteLine(jsonResponse);
            List<string> responseList = JsonSerializer.Deserialize<List<string>>(jsonResponse);
            //Assert
            for (int i = 0; i < fileNames.Length; i++)
            {
                Assert.AreEqual(Path.GetFileName(fileNames.ElementAt(i)), responseList.ElementAt(i));
            }
            deleteUploadedFiles();
        }

        [TestMethod]
        public async Task Post_OnFail_BadFormat_Return400Async()
        {
            //Arrange
            var controller = new ImageController();
            var content = "Fake File";
            string fileName = "image0.pdf";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;
            IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
            //Act
            var response = (BadRequestObjectResult)await controller.UploadImage(file);
            Console.WriteLine(response.Value);
            //Assert
            Assert.AreEqual(response.Value, "The selected file is not an image. Load a image file.");
        }

        [TestMethod]
        public async Task Post_OnSuccess_Return200Async()
        {
            //Arrange
            var controller = new ImageController();
            string pathToFile = @"..\..\..\testImages\Image1.png";
            var fileName = "Image1.png";
            var response = new OkObjectResult("");
            //Act
            using (var stream = File.OpenRead(pathToFile))
            {
                var file = new FormFile(stream, 0, stream.Length, null, fileName);
                response = (OkObjectResult) await controller.UploadImage(file);
            }
            string? jsonResponse = response.Value as string;
            Console.WriteLine(jsonResponse);
            Dictionary<string, string> responseDict = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResponse);
            //Assert
            Assert.AreEqual(responseDict["fileName"], fileName);
            deleteUploadedFiles();
        }

        [TestMethod]
        public void Delete_OnFail_NoImage_Return404()
        {
            //Arrange
            var controller = new ImageController();

            //Act
            var response = (NotFoundObjectResult)controller.DeleteImage(-1);
            Console.WriteLine(response.Value);
            //Assert
            Assert.AreEqual(response.Value, "Image not found.");
        }

        //[TestMethod]
        public async void Delete_OnSuccess_Return200()
        {
            //Arrange
            var controller = new ImageController();
            string pathToFile = @"..\..\..\testImages\Image1.png";
            var fileName = "Image1.png";
            var uploadResponse = new OkObjectResult("");
            using (FileStream stream = File.OpenRead(pathToFile))
            {
                var file = new FormFile(stream, 0, stream.Length, null, fileName);
                uploadResponse = (OkObjectResult)await controller.UploadImage(file);
            }
            string? jsonResponse = uploadResponse.Value as string;
            Console.WriteLine(jsonResponse);
            Dictionary<string, string> responseDict = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResponse);
            //Assert
            Assert.AreEqual(responseDict["fileName"], fileName);

            //Act
            var response = (OkObjectResult) controller.DeleteImage(long.Parse(responseDict["id"]));

            //Assert
            Assert.AreEqual(response.Value, "Image Deleted.");
            deleteUploadedFiles();
        }

        [TestMethod]
        public async Task Update_OnFail_NoImage_Return404Async()
        {
            //Arrange
            var controller = new ImageController();

            //Act
            var response = (NotFoundObjectResult) await controller.UpdateImageAsync(-1, new Requests.UpdateImageRequest(100, 100));
            Console.WriteLine(response.Value);
            //Assert
            Assert.AreEqual(response.Value, "Image not found.");
        }

        [TestMethod]
        public async Task Update_OnFail_BadRequest_Return400Async()
        {
            //Arrange
            var controller = new ImageController();

            //Act
            var response = (BadRequestObjectResult) await controller.UpdateImageAsync(0, new Requests.UpdateImageRequest(-1, -1));
            Console.WriteLine(response.Value);
            //Assert
            Assert.AreEqual(response.Value, "Width and Height must be greater then 0.");
        }

        [TestMethod]
        public async Task Update_OnSuccess_Return200Async()
        {
            //Arrange
            var controller = new ImageController();
            string pathToFile = @"..\..\..\testImages\Image1.png";
            var fileName = "Image1.png";
            var uploadResponse = new OkObjectResult("");
            FormFile file;
            using (var stream = File.OpenRead(pathToFile))
            {
                file = new FormFile(stream, 0, stream.Length, null, fileName);
                uploadResponse = (OkObjectResult) await controller.UploadImage(file);
                stream.Dispose();
            }
            string? jsonResponse = uploadResponse.Value as string;
            Console.WriteLine(jsonResponse);
            Dictionary<string, string> responseDict = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResponse);
            //Act
            var response = (OkObjectResult) await controller.UpdateImageAsync(long.Parse(responseDict["id"]), new Requests.UpdateImageRequest(100, 100));
            Console.WriteLine(response.Value);
            //Assert
            Assert.AreEqual(response.Value, "Image size updated.");
            SixLabors.ImageSharp.Image img = SixLabors.ImageSharp.Image.Load(Path.Combine(Directory.GetCurrentDirectory() + @$"\uploads\{responseDict["id"]}\{fileName}"));
            Assert.AreEqual(img.Height, 100);
            Assert.AreEqual(img.Width, 100);
            deleteUploadedFiles();
        }

        private static void deleteUploadedFiles()
        {
            string uploadDir = Directory.GetCurrentDirectory() + @"/uploads";
            DirectoryInfo di = new DirectoryInfo(uploadDir);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }
    }
}