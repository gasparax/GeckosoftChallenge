namespace GeckosoftChallenge.Requests
{
    public class UpdateImageRequest
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public UpdateImageRequest(int height, int width)
        {
            Height = height;
            Width = width;
        }   
    }
}
