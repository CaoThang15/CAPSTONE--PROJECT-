namespace SMarket.Business.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadImage(Stream fileStream, string folder);
        Task<bool> DeleteImage(string imageUrl);
    }
}