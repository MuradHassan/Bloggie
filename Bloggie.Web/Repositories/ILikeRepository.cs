using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Repositories
{
    public interface ILikeRepository
    {
        Task<int> GetTotalLikes(Guid blogPostId);
        Task<Like> GetLike(Guid blogPostId, Guid userId);
        Task DeleteLike(Like like);
        Task<Like> AddLikeForBlog(Like blogPostLike);
        Task<bool> ToggleLike(Guid blogPostId, Guid userId);
        Task<IEnumerable<Like>> GetLikesByUserIdAsync(Guid id);
        Task<bool> DeleteRangeAsync(IEnumerable<Like> blogPostLike);
    }
}
