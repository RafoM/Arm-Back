using ContentService.Data.Entity;
using ContentService.Data;
using ContentService.Services.Interface;
using ContentService.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;
using ContentService.Models.RequestModels;

namespace ContentService.Services.Implementation
{
    public class BlogService : IBlogService
    {
        private readonly ContentDbContext _dbContext;

        public BlogService(ContentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BlogResponseModel> CreateAsync(BlogRequestModel request)
        {
            var newBlog = new Blog
            {
                Title = request.Title,
                Subtitle = request.Subtitle,
                MainImage = request.MainImage,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                LanguageId = request.LanguageId,
            };

            if (request.TagIds != null && request.TagIds.Count > 0)
            {
                var tags = await _dbContext.BlogTags
                    .Where(t => request.TagIds.Contains(t.Id))
                    .ToListAsync();
                foreach (var tag in tags)
                {
                    newBlog.BlogTags.Add(tag);
                }
            }

            _dbContext.Blogs.Add(newBlog);
            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(newBlog.Id);
        }

        public async Task<BlogResponseModel> UpdateAsync(BlogUpdateModel request)
        {
            var blog = await _dbContext.Blogs
                .Include(b => b.BlogTags) 
                .FirstOrDefaultAsync(b => b.Id == request.BlogId);

            if (blog == null)
                throw new KeyNotFoundException($"Blog with ID {request.BlogId} not found.");

            blog.Title = request.Title;
            blog.Subtitle = request.Subtitle;
            blog.MainImage = request.MainImage;
            blog.Content = request.Content;
            blog.UpdatedAt = DateTime.UtcNow;
            blog.LanguageId = request.LanguageId;
            if (request.TagIds != null)
            {
                blog.BlogTags.Clear();

                var newTags = await _dbContext.BlogTags
                    .Where(t => request.TagIds.Contains(t.Id))
                    .ToListAsync();

                foreach (var tag in newTags)
                {
                    blog.BlogTags.Add(tag);
                }
            }

            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(blog.Id);
        }

        public async Task<BlogResponseModel> GetByIdAsync(int blogId)
        {
            var blog = await _dbContext.Blogs
                .Include(b => b.BlogTags)
                .FirstOrDefaultAsync(b => b.Id == blogId);

            if (blog == null)
                throw new KeyNotFoundException($"Blog with ID {blogId} not found.");

            return ToBlogResponse(blog);
        }

        public async Task<IEnumerable<BlogResponseModel>> GetAllAsync()
        {
            var blogs = await _dbContext.Blogs
                .Include(b => b.BlogTags)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return blogs.Select(ToBlogResponse);
        }

        public async Task DeleteAsync(int blogId)
        {
            var blog = await _dbContext.Blogs.FindAsync(blogId);
            if (blog == null)
                throw new KeyNotFoundException($"Blog with ID {blogId} not found.");

            _dbContext.Blogs.Remove(blog);
            await _dbContext.SaveChangesAsync();
        }

        private BlogResponseModel ToBlogResponse(Blog blog)
        {
            return new BlogResponseModel
            {
                BlogId = blog.Id,
                Title = blog.Title,
                Subtitle = blog.Subtitle,
                MainImage = blog.MainImage,
                Content = blog.Content,
                CreatedAt = blog.CreatedAt,
                UpdatedAt = blog.UpdatedAt,
                LanguageId = blog.LanguageId,
                Tags = blog.BlogTags
                    .Select(t => new BlogTagResponseModel
                    {
                        TagId = t.Id,
                        Tag = t.Tag
                    })
                    .ToList()
            };
        }
    }
}
