using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;
using Microsoft.Net.Http.Headers;

namespace ProjectAPI.DTOs
{
   public class UploadImageDTO
    {
        public string? name { get; set; }
        public IFormFile? image{ get; set; }
    } 
}