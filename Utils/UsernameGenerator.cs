
namespace ProjectAPI.Utils
{
    public class UsernameChecker{
        public string UsernameCheckerGenerator(string? Username, string email)
        {
            
        if (Username == null)
            {
            string uniqueSuffix = Guid.NewGuid().ToString("N")[..8];
            string[] emailstr = email.Split('@');
            string NewUsername = emailstr[0]+uniqueSuffix;
            return NewUsername;
        }
        return Username;
    }
    }
    
}