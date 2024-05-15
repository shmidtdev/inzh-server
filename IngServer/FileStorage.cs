using Minio;
using Minio.DataModel.Args;

namespace IngServer;

// public class FileStorage(IMinioClient minioClient)
// {
//     public Task<bool> PostImage()
//     {
//         
//     }
//
//     public Task<bool> GetImage()
//     {
//         var smt = await minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs().WithBucket("productimages"));
//     }
// }