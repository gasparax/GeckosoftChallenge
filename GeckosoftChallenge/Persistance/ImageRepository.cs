using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using GeckosoftChallenge.Controllers;

namespace GeckosoftChallenge.Persistance
{
    public class ImageRepository 
    {
        string pathRoot = Directory.GetCurrentDirectory() + @"\uploads\";

        private static long id = 0;
        //private readonly ILogger<ImageController> logger;
        public ImageRepository()
        {
            if (Directory.Exists(pathRoot))
            {
                string[] directories = Directory.GetDirectories(pathRoot);
                if (directories.Length > 0)
                {
                    id = directories.Length;
                }

            }
        }
        public async Task<long> SaveImageAsync(IFormFile image)
        {
            if (!Directory.Exists(pathRoot))
            {
                Directory.CreateDirectory(pathRoot);
            }
            try
            {
                id++;
                var filePath = $"{pathRoot}/{id}/{image.FileName}";
                Directory.CreateDirectory($"{pathRoot}/{id}/");
                using (var stream = File.Create(filePath))
                {
                    await image.CopyToAsync(stream);
                }
            }
            catch (IOException e)
            {
                id--;
                Console.WriteLine(e.ToString());
                return -1;
            }
            return id;
        }

        public List<string> GetImagesNamesList()
        {
            List<string> imagesNames = new List<string>();
            if (!Directory.Exists(pathRoot))
            {
                return imagesNames;
            }
            string[] fileEntries = Directory.GetDirectories(pathRoot);
            foreach (string dir in fileEntries) 
            {
                if (!Directory.GetFiles(dir).Any()) { continue; }
                imagesNames.Add(Path.GetFileName(Directory.GetFiles(dir)[0]));
            }
            imagesNames.Sort();
            return imagesNames;
        }

        public bool DeleteImage(long id)
        {
            if (!Directory.Exists(pathRoot + id)) 
            {
                return false;
            }
            try
            {
                Console.WriteLine(Directory.GetFiles(pathRoot + id)[0]);
                File.Delete(Directory.GetFiles(pathRoot + id)[0]);
                Directory.Delete(pathRoot + id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return true;
        }

        public async Task<bool> ResizeImage(long id, int width, int height)
        {
            
            if (!Directory.Exists(pathRoot + id))
            {
                return false;
            }
            string fileName = Directory.GetFiles(pathRoot + id)[0];
            try
            {
                using Image image = await Image.LoadAsync(fileName);
                image.Mutate(x => x.Resize(width, height));
                image.Save(fileName);
                image.Dispose();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }
    }
}
