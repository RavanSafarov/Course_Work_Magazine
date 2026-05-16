using Amazon.S3;
using Amazon.S3.Model;
using Course_Work_Magazine.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Work_Magazine.BLL.Services;

public class S3StorageService : IStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly IConfiguration _configuration;
    public S3StorageService(IAmazonS3 s3Client, IConfiguration configuration)
    {
        _s3Client = s3Client;
        _configuration = configuration;
    }

    public async Task DeleteFileByUrlAsync(string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            return;
        }
        var bucketName = _configuration["AWS:BucketName"];
        var region = _configuration["AWS:Region"];
        if (string.IsNullOrWhiteSpace(bucketName))
        {
            return;
        }
        var market = $"{bucketName}.s3.{region}.amazonaws.com";
        var index = fileUrl.IndexOf(market, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            return;
        }
        var key = fileUrl[(index +  market.Length)..];
        if (string.IsNullOrWhiteSpace(key))
        {
            return ;
        }
        var request = new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = key
        };
        await _s3Client.DeleteObjectAsync(request);
    }

    public async Task<string> UploadFileAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is empty");
        }
        var bucketName = _configuration["AWS:BucketName"];
        var region = _configuration["AWS:Region"];
        if (string.IsNullOrWhiteSpace(bucketName)) 
        {
            throw new InvalidOperationException("AWS S3 bucket name is not configured");
        }
        var key = $"products/{Guid.NewGuid()}_{file.FileName}";
        await using var stream = file.OpenReadStream();
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = stream,
            ContentType = file.ContentType
        };
        await _s3Client.PutObjectAsync(request);
        return $"https://{bucketName}.s3.{region}.amazonaws.com/{key}";
    }
}
