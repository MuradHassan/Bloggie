namespace Bloggie.Web.Models.Domain
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string RoleName { get; set; }
    }
}
