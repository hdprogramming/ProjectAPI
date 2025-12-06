using ProjectAPI.Models;

namespace ProjectAPI.DTOs
{
    public class ProjectDto
    {
        public int id { get; set; }
        public string icon { get; set; } = "None";

        public string title { get; set; } = "";
        public string description { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
        public bool isAlive { get; set; } = true;
        public string status { get; set; } = "";
        public int? statusID { get; set; } = 1;
        public List<int> categoryIds { get; set; } = new List<int>();
        public DateOnly? date { get; set; }
         public DateTime lastdate { get; set; }
        
    }
    public class ProjectCreateDto
    {
        public int id { get; set; }
        public string icon { get; set; } = "None";

        public string title { get; set; } = "";
        public string description { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
        public bool isAlive { get; set; } = true;
        public int? statusID { get; set; } = 1;
         public List<int> categoryIds { get; set; } = new List<int>();
        public DateOnly? date { get; set; }
       
    }
    public class ProjectUpdateDto
    {
        public int id { get; set; }
        public string? icon { get; set; }

        public string? title { get; set; }
        public string? description { get; set; }
        public string? content { get; set; }
        public bool? isAlive { get; set; }
        public int? statusID { get; set; } = 1;
        public List<int> categoryIds { get; set; } = new List<int>();
        public DateOnly? date { get; set; }
        

    }
}
