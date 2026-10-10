using Azure.Core;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ICommentRepository _blogPostCommentRepository;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public CommentsController(
            ICommentRepository blogPostCommentRepository,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager
            )
        {
            _blogPostCommentRepository = blogPostCommentRepository;
            _signInManager = signInManager;
            _userManager = userManager;
        }




        [HttpPost]
        public async Task<IActionResult> Add(CommentViewModel commentViewModel)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var comment = new Comment
                {
                    UserId = Guid.Parse(_userManager.GetUserId(User)),
                    BlogPostId = commentViewModel.BlogPostId,
                    Description = commentViewModel.Description,
                    DateAdded = DateTime.Now
                };
                await _blogPostCommentRepository.AddAsync(comment);
                return RedirectToAction("Index", "BlogDetails", new { urlHandel = commentViewModel.UrlHandle });
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(CommentViewModel commentViewModel)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var currentComment = await _blogPostCommentRepository.GetByIdAsync(commentViewModel.Id);
                if (currentComment != null)
                {
                    var currentUserId = Guid.Parse(_userManager.GetUserId(User));
                    if (currentComment.UserId == currentUserId)
                    {
                        if (!string.IsNullOrEmpty(commentViewModel.Description))
                        {
                            var comment = new Comment
                            {
                                Id = commentViewModel.Id,
                                UserId = Guid.Parse(_userManager.GetUserId(User)),
                                BlogPostId = commentViewModel.BlogPostId,
                                Description = commentViewModel.Description,
                                DateAdded = commentViewModel.DateAdded
                            };
                            await _blogPostCommentRepository.UpdateAsync(comment);
                            return RedirectToAction("Index", "BlogDetails", new { urlHandel = commentViewModel.UrlHandle });
                        }
                    }
                }

            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CommentViewModel commentViewModel)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var currentComment = await _blogPostCommentRepository.GetByIdAsync(commentViewModel.Id);
                if (currentComment != null)
                {
                    var currentUserId = Guid.Parse(_userManager.GetUserId(User));
                    if (currentComment.UserId == currentUserId)
                    {
                        await _blogPostCommentRepository.DeleteAsync(commentViewModel.Id);
                        return RedirectToAction("Index", "BlogDetails", new { urlHandel = commentViewModel.UrlHandle });
                    }
                }
            }
            return View();
        }


    }
}
