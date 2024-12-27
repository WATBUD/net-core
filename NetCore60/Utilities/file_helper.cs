using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;

public static class FileHelper
{
    public static async Task<List<string>> UploadImagesAsync(List<IFormFile> images, string rootPath)
    {
        var uploadResults = new List<string>();
        var imagesDirectory = Path.Combine(rootPath, "images");

        if (!Directory.Exists(imagesDirectory))
        {
            Directory.CreateDirectory(imagesDirectory);
        }

        foreach (var image in images)
        {
            if (image.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(imagesDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                uploadResults.Add($"/images/{fileName}");
            }
        }

        return uploadResults;
    }

    public static void DeleteImages(List<string> oldImagePaths, string rootPath)
    {
        foreach (var oldImagePath in oldImagePaths)
        {
            var fullPath = Path.Combine(rootPath, oldImagePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}
