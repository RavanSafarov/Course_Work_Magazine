using Amazon;
using Amazon.S3;
using Course_Work_Magazine.BLL.Services;

namespace Course_Work_Magazine.API.Extensions;

public static class AwsExtensions
{
    public static IServiceCollection AddAwsServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IAmazonS3>(_ =>
        {
            var awsSection = config.GetSection("AWS");
            var settings = awsSection.Get<AwsSettings>();

            var region = RegionEndpoint.GetBySystemName(settings.Region);

            return new AmazonS3Client(
                settings.AccessKey,
                settings.SecretKey,
                region
            );
        });

        return services;
    }
}
