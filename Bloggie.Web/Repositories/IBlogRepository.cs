using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Repositories
{
    public interface IBlogRepository
    {
        Task<IEnumerable<Blog>> GetAllAsync
            (
            string? searchQuery,
            string? tagName,
            string? sort,
            int? pageNumber,
            int? pageSize
            );

        Task<Blog?> GetAsync(Guid id);
        Task<Blog> AddAsync(Blog blogPost);
        Task<Blog?> UpdateAsync(Blog blogPost);
        Task<Blog?> DeleteAsync(Guid id);
        Task<Blog?> GetByUrlHandelAsync(string urlHandel);
        Task<int> CountAsync();
    }
}
