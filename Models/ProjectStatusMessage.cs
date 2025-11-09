using System.ComponentModel.DataAnnotations;

namespace ProjectAPI.Models;
public class ProjectStatusMsg
{
public int Id { get; set; }
[Required]
public string? Name { get; set; }
}