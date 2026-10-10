using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly BloggieDbContext _bloggieDbContext;

        public CommentRepository(BloggieDbContext bloggieDbContext)
        {
            _bloggieDbContext = bloggieDbContext;
        }


        public async Task<Comment> AddAsync(Comment blogPostComment)
        {
            await _bloggieDbContext.BlogPostComment.AddAsync(blogPostComment);
            await _bloggieDbContext.SaveChangesAsync();
            return blogPostComment;
        }

        public async Task<IEnumerable<Comment>> GetCommentsByBlogIdAsync(Guid blogPostId)
        {
            return await _bloggieDbContext.BlogPostComment.Where(x => x.BlogPostId == blogPostId)
                .ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(Guid id)
        {
            return await _bloggieDbContext.BlogPostComment.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Comment?> UpdateAsync(Comment blogPostComment)
        {
            var existingComment = await _bloggieDbContext.BlogPostComment.FindAsync(blogPostComment.Id);
            if (existingComment != null)
            {
                existingComment.Description = blogPostComment.Description;
                await _bloggieDbContext.SaveChangesAsync();
                return existingComment;
            }
            return null;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existingComment = await _bloggieDbContext.BlogPostComment.FindAsync(id);
            if (existingComment != null)
            {
                _bloggieDbContext.BlogPostComment.Remove(existingComment);
                await _bloggieDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }



        public async Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(Guid id)
        {
            return await _bloggieDbContext.BlogPostComment.Where(x => x.UserId == id).ToListAsync();
        }

        public async Task<bool> DeleteRangeAsync(IEnumerable<Comment> blogPostComment)
        {
            if (blogPostComment != null)
            {
                _bloggieDbContext.BlogPostComment.RemoveRange(blogPostComment);
                await _bloggieDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
