
using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly BloggieDbContext _bloggieDbContext;

        public LikeRepository(BloggieDbContext bloggieDbContext)
        {
            _bloggieDbContext = bloggieDbContext;
        }

        public async Task<Like> AddLikeForBlog(Like blogPostLike)
        {
            await _bloggieDbContext.BlogPostLike.AddAsync(blogPostLike);
            await _bloggieDbContext.SaveChangesAsync();
            return blogPostLike;
        }

        public async Task DeleteLike(Like blogPostLike)
        {
            _bloggieDbContext.BlogPostLike.Remove(blogPostLike);
            await _bloggieDbContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteRangeAsync(IEnumerable<Like> blogPostLike)
        {
            if (blogPostLike != null)
            {
                _bloggieDbContext.BlogPostLike.RemoveRange(blogPostLike);
                await _bloggieDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<Like?> GetLike(Guid blogPostId, Guid userId)
        {
            return await _bloggieDbContext.BlogPostLike.FirstOrDefaultAsync(x => x.BlogPostId == blogPostId && x.UserId == userId);
        }

        public async Task<IEnumerable<Like>> GetLikesByUserIdAsync(Guid id)
        {
            return await _bloggieDbContext.BlogPostLike.Where(x => x.UserId == id).ToListAsync();
        }

        public async Task<int> GetTotalLikes(Guid blogPostId)
        {
            return await _bloggieDbContext.BlogPostLike.CountAsync(x => x.BlogPostId == blogPostId);
        }

        public async Task<bool> ToggleLike(Guid blogPostId, Guid userId)
        {
            var existingLike = await GetLike(blogPostId, userId);
            if (existingLike != null)
            {
                await DeleteLike(existingLike);
                return false;
            }

            var newLike = new Like
            {
                BlogPostId = blogPostId,
                UserId = userId
            };
            await AddLikeForBlog(newLike);
            return true;
        }


    }
}
