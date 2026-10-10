using System.Diagnostics;
using Bloggie.Web.Models;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogRepository _blogPostRepository;
        private readonly ITagRepository _tagRepository;

        public HomeController(
            ILogger<HomeController> logger,
            IBlogRepository blogPostRepository,
            ITagRepository tagRepository)
        {
            _logger = logger;
            _blogPostRepository = blogPostRepository;
            _tagRepository = tagRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchQuery, string? tagName)
        {
            ViewBag.SearchQuery = searchQuery;
            ViewBag.TagName = tagName;


            // get all blogs
            var blogPost = await _blogPostRepository.GetAllAsync(searchQuery, tagName, null, null, null);

            // get all tags
            var tags = await _tagRepository.GetAllAsync();
            var model = new HomeViewModel
            {
                BlogPost = blogPost,
                Tags = tags
            };
            return View(model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
