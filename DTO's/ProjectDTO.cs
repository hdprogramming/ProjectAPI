namespace ProjectAPI.DTOs
{
    public class ProjectDto
    {
        public int id{ get; set; }
        public string IconName { get; set; } = "None";
        
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool isAlive { get; set; } = true;
        public string Status { get; set; } = string.Empty;
        public DateTime? StartingDate { get; set; }
    }
    
}
