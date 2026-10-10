using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly BloggieDbContext _bloggieDbContext;

        public TagRepository(BloggieDbContext bloggieDbContext)
        {
            _bloggieDbContext = bloggieDbContext;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await _bloggieDbContext.Tags.ToListAsync();
        }

        public async Task<IEnumerable<Tag>> GetAllAsync
            (
            string? searchQuery,
            string? sort,
            int? pageNumber,
            int? pageSize
            )
        {
            var query = _bloggieDbContext.Tags.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(x => x.Name.Contains(searchQuery)
                                      || x.DisplayName.Contains(searchQuery));
            }

            // Sort
            if (sort != null)
            {
                query = sort switch
                {
                    "Desc" => query.OrderByDescending(x => x.Name),
                    _ => query.OrderBy(x => x.Name)
                };
            }

            // Pagination
            if (pageNumber != null && pageSize != null)
            {
                var tagsList = query.Skip((int)((pageNumber - 1) * pageSize)).Take((int)pageSize);
                return await tagsList.ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }

        }

        public async Task<Tag?> DeleteAsync(Guid id)
        {
            var existingTag = await _bloggieDbContext.Tags.FindAsync(id);
            if (existingTag != null)
            {
                _bloggieDbContext.Tags.Remove(existingTag);
                await _bloggieDbContext.SaveChangesAsync();
                return existingTag;
            }
            return null;
        }

        public async Task<Tag> AddAsync(Tag tag)
        {
            await _bloggieDbContext.Tags.AddAsync(tag);
            await _bloggieDbContext.SaveChangesAsync();
            return tag;
        }

        public Task<Tag?> GetAsync(Guid id)
        {
            return _bloggieDbContext.Tags.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Tag?> UpdateAsync(Tag tag)
        {
            var existingTag = await _bloggieDbContext.Tags.FindAsync(tag.Id);
            if (existingTag != null)
            {
                existingTag.Name = tag.Name;
                existingTag.DisplayName = tag.DisplayName;
                await _bloggieDbContext.SaveChangesAsync();
                return existingTag;
            }
            return null;
        }
        public async Task<int> CountAsync()
        {
            return await _bloggieDbContext.Tags.CountAsync();
        }
    }
}
