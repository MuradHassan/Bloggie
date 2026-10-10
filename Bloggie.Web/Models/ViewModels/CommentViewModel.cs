namespace Bloggie.Web.Models.ViewModels
{
    public class CommentViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid BlogPostId { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string UrlHandle { get; set; }

    }
}
