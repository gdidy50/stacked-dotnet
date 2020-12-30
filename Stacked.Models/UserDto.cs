namespace Stacked.Models
{
    public class UserDto : BlogDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PictureUrl { get; set; }
    }
}