using Microsoft.AspNetCore.Http;

namespace Course_Work_Magazine.BLL.Services.Interfaces;

public interface IStorageService
{
    Task<string> UploadFileAsync(IFormFile? file); 
    Task DeleteFileByUrlAsync(string fileUrl);
}
