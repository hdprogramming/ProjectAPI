using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;
using Microsoft.Net.Http.Headers;

namespace ProjectAPI.DTOs
{
    public class UploadImageDTO
    {
        public string? name { get; set; }
        public IFormFile? image { get; set; }
        public string? ProjectID { get; set; }
    }
    public class UploadedFilesDTO
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? url { get; set; }
    }
    public class UploadModDTO
    {
        public string? name { get; set; }
    }
}