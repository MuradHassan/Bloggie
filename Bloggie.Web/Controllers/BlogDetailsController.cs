using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    public class BlogDetailsController : Controller
    {
        private readonly IBlogRepository _blogPostRepository;
        private readonly ILikeRepository _blogPostLikeRepository;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICommentRepository _blogPostCommentRepository;

        public BlogDetailsController
            (
            IBlogRepository blogPostRepository,
            ILikeRepository blogPostLikeRepository,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ICommentRepository blogPostCommentRepository
            )
        {
            _blogPostRepository = blogPostRepository;
            _blogPostLikeRepository = blogPostLikeRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _blogPostCommentRepository = blogPostCommentRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string urlHandel)
        {
            var blogPost = await _blogPostRepository.GetByUrlHandelAsync(urlHandel);
            var blogDetailsViewModel = new BlogDetailsViewModel();
            var liked = false;

            if (blogPost != null)
            {
                var totalLikes = await _blogPostLikeRepository.GetTotalLikes(blogPost.Id);

                if (_signInManager.IsSignedIn(User))
                {
                    var userId = _userManager.GetUserId(User);

                    if (userId != null)
                    {
                        var likeFromUser = await _blogPostLikeRepository.GetLike(blogPost.Id, Guid.Parse(userId));

                        liked = likeFromUser != null;
                    }
                }

                // Get comments for blog post
                var blogCommentsDomainModel = await _blogPostCommentRepository.GetCommentsByBlogIdAsync(blogPost.Id);
                var blogCommentsForView = new List<CommentViewModel>();
                foreach (var blogComment in blogCommentsDomainModel)
                {
                    blogCommentsForView.Add(new CommentViewModel
                    {
                        Id = blogComment.Id,
                        UserId = blogComment.UserId,
                        UserName = (await _userManager.FindByIdAsync(blogComment.UserId.ToString())).UserName,
                        Description = blogComment.Description,
                        DateAdded = blogComment.DateAdded,
                    });
                }

                blogDetailsViewModel = new BlogDetailsViewModel
                {
                    Id = blogPost.Id,
                    Content = blogPost.Content,
                    PageTitle = blogPost.PageTitle,
                    Author = blogPost.Author,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    Heading = blogPost.Heading,
                    PublishedDate = blogPost.PublishedDate,
                    UrlHandle = blogPost.UrlHandle,
                    Visible = blogPost.Visible,
                    Tags = blogPost.Tags,
                    TotalLikes = totalLikes,
                    Liked = liked,
                    Comments = blogCommentsForView
                };
            }
            return View(blogDetailsViewModel);
        }
    }
}

