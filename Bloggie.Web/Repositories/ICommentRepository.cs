using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Repositories
{
    public interface ICommentRepository
    {
        Task<Comment> AddAsync(Comment blogPostComment);
        Task<IEnumerable<Comment>> GetCommentsByBlogIdAsync(Guid blogPostId);
        Task<Comment> GetByIdAsync(Guid id);
        Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(Guid id);
        Task<Comment> UpdateAsync(Comment blogPostComment);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteRangeAsync(IEnumerable<Comment> blogPostComment);
    }
}
