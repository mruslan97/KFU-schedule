using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Minio;
using Schedule.Models;

namespace Schedule.Services.Impl
{
    public class ObjectStorageService : IObjectStorageService
    {
        private StorageOptions _options;

        private MinioClient _minioClient;

        public ObjectStorageService(IOptions<StorageOptions> options)
        {
            _options = options.Value;
            _minioClient = new MinioClient(_options.Host, _options.AccessKey, _options.SecretKey);
        }

        public async Task<IEnumerable<byte>> GetDay(string group, int day)
        {
            using (var ms = new MemoryStream())
            {
                await _minioClient.GetObjectAsync(_options.Bucket, $"{group}_day{day}.png", 
                    (stream) =>
                    {
                        stream.CopyTo(ms);
                    });
                
                return ms.ToArray();
            }
        }
    }
}