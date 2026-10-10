using Azure;
using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Bloggie.Web.Repositories
{
    public class BlogRepository : IBlogRepository
    {
        private readonly BloggieDbContext _bloggieDbContext;

        public BlogRepository(BloggieDbContext bloggieDbContext)
        {
            _bloggieDbContext = bloggieDbContext;
        }

        public async Task<IEnumerable<Blog>> GetAllAsync
            (
            string? searchQuery,
            string? tagName,
            string? sort,
            int? pageNumber,
            int? pageSize
            )
        {
            var query = _bloggieDbContext.BlogPosts.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(x => x.Heading.Contains(searchQuery));
            }

            // Sort
            if (sort != null)
            {
                query = sort switch
                {
                    "Desc" => query.OrderByDescending(x => x.Heading),
                    _ => query.OrderBy(x => x.Heading)
                };
            }

            // Filter by tag
            if (!string.IsNullOrWhiteSpace(tagName))
            {
                query = query.Where(x => x.Tags.Any(t => t.Name == tagName));
            }

            // Pagination

            if (pageNumber != null && pageSize != null)
            {
                var blogsList = query.Skip((int)((pageNumber - 1) * pageSize)).Take((int)pageSize);
                return await blogsList.Include(x => x.Tags).Include(x => x.Likes).Include(x => x.Comments).ToListAsync();
            }
            else
            {
                return await query.Include(x => x.Tags).Include(x => x.Likes).Include(x => x.Comments).ToListAsync();
            }
        }


        public async Task<Blog> AddAsync(Blog blogPost)
        {
            await _bloggieDbContext.BlogPosts.AddAsync(blogPost);
            await _bloggieDbContext.SaveChangesAsync();
            return blogPost;
        }

        public async Task<Blog?> DeleteAsync(Guid id)
        {
            var existingBlogPost = await _bloggieDbContext.BlogPosts.FindAsync(id);
            if (existingBlogPost != null)
            {
                _bloggieDbContext.BlogPosts.Remove(existingBlogPost);
                await _bloggieDbContext.SaveChangesAsync();
                return existingBlogPost;
            }
            return null;
        }

        public async Task<Blog?> GetAsync(Guid id)
        {
            return await _bloggieDbContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Blog?> GetByUrlHandelAsync(string urlHandel)
        {
            return await _bloggieDbContext.BlogPosts.Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.UrlHandle == urlHandel);
        }

        public async Task<Blog?> UpdateAsync(Blog blogPost)
        {
            var existingBlogPost = await _bloggieDbContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == blogPost.Id);
            if (existingBlogPost != null)
            {
                existingBlogPost.Id = blogPost.Id;
                existingBlogPost.Heading = blogPost.Heading;
                existingBlogPost.PageTitle = blogPost.PageTitle;
                existingBlogPost.Content = blogPost.Content;
                existingBlogPost.ShortDescription = blogPost.ShortDescription;
                existingBlogPost.FeaturedImageUrl = blogPost.FeaturedImageUrl;
                existingBlogPost.UrlHandle = blogPost.UrlHandle;
                existingBlogPost.PublishedDate = blogPost.PublishedDate;
                existingBlogPost.Author = blogPost.Author;
                existingBlogPost.Visible = blogPost.Visible;
                existingBlogPost.Tags = blogPost.Tags;
                await _bloggieDbContext.SaveChangesAsync();
                return existingBlogPost;
            }
            return null;
        }

        public async Task<int> CountAsync()
        {
            return await _bloggieDbContext.BlogPosts.CountAsync();
        }


    }
}
