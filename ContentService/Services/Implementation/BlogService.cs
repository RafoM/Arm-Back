using ContentService.Data.Entity;
using ContentService.Data;
using ContentService.Services.Interface;
using ContentService.Models.ResponseModels;
using Microsoft.EntityFrameworkCore;
using ContentService.Models.RequestModels;
using ContentService.Common.Enums;
using ContentService.Common.Constants;
using System.Linq;

namespace ContentService.Services.Implementation
{
    public class BlogService : IBlogService
    {
        private readonly ContentDbContext _dbContext;
        private readonly IContentTranslationService _translationService;
        private readonly IFileStorageService _fileStorageService;

        public BlogService(
            ContentDbContext dbContext,
            IContentTranslationService translationService,
            IFileStorageService fileStorageService)
        {
            _dbContext = dbContext;
            _translationService = translationService;
            _fileStorageService = fileStorageService;
        }

        public async Task<BlogResponseModel> CreateAsync(BlogRequestModel request)
        {
            var blog = new Blog
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            if (request.TagIds != null && request.TagIds.Count > 0)
            {
                var tags = await _dbContext.BlogTags
                    .Where(t => request.TagIds.Contains(t.Id))
                    .ToListAsync();
                foreach (var tag in tags)
                {
                    blog.BlogTags.Add(tag);
                }
            }

            _dbContext.Blogs.Add(blog);
            await _dbContext.SaveChangesAsync();

            var contentId = blog.Id;

            await _translationService.SetTranslationAsync(contentId, ContentTranslationKeys.Title, request.Title, request.LanguageId, ContentTypeEnum.Blog);
            await _translationService.SetTranslationAsync(contentId, ContentTranslationKeys.Subtitle, request.Subtitle, request.LanguageId, ContentTypeEnum.Blog);
            await _translationService.SetTranslationAsync(contentId, ContentTranslationKeys.Content, request.Content, request.LanguageId, ContentTypeEnum.Blog);
            await _translationService.SetTranslationAsync(contentId, ContentTranslationKeys.MainImage, request.MainImage, request.LanguageId, ContentTypeEnum.Blog);


            return await GetByIdAsync(contentId, request.LanguageId);
        }

        public async Task<BlogResponseModel> UpdateAsync(BlogUpdateModel request)
        {
            var blog = await _dbContext.Blogs
                .Include(b => b.BlogTags)
                .FirstOrDefaultAsync(b => b.Id == request.BlogId);

            if (blog == null)
                throw new KeyNotFoundException($"Blog with ID {request.BlogId} not found.");

            blog.UpdatedAt = DateTime.UtcNow;

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

            await _translationService.SetTranslationAsync(request.BlogId, ContentTranslationKeys.Title, request.Title, request.LanguageId, ContentTypeEnum.Blog);
            await _translationService.SetTranslationAsync(request.BlogId, ContentTranslationKeys.Subtitle, request.Subtitle, request.LanguageId, ContentTypeEnum.Blog);
            await _translationService.SetTranslationAsync(request.BlogId, ContentTranslationKeys.Content, request.Content, request.LanguageId, ContentTypeEnum.Blog);
            await _translationService.SetTranslationAsync(request.BlogId, ContentTranslationKeys.MainImage, request.MainImage, request.LanguageId, ContentTypeEnum.Blog);

            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(blog.Id, request.LanguageId);
        }

        public async Task<BlogResponseModel> GetByIdAsync(Guid blogId, int languageId)
        {
            var blog = await _dbContext.Blogs
                .Include(b => b.BlogTags)
                .FirstOrDefaultAsync(b => b.Id == blogId);

            if (blog == null)
                throw new KeyNotFoundException($"Blog with ID {blogId} not found.");

            return await ToBlogResponse(blog, languageId);
        }

        public async Task<IEnumerable<BlogResponseModel>> GetAllAsync(int languageId)
        {
            var blogs = await _dbContext.Blogs
                .Include(b => b.BlogTags)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            var responseList = new List<BlogResponseModel>();
            foreach (var blog in blogs)
            {
                responseList.Add(await ToBlogResponse(blog, languageId));
            }

            return responseList;
        }

        public async Task<IEnumerable<BlogResponseModel>> GetByTagIdsAsync(List<Guid> tagIds, int languageId)
        {
            var blogs = await _dbContext.Blogs
                .Include(b => b.BlogTags)
                .Where(b => b.BlogTags.Any(tag => tagIds.Contains(tag.Id)))
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            var responseList = new List<BlogResponseModel>();
            foreach (var blog in blogs)
            {
                responseList.Add(await ToBlogResponse(blog, languageId));
            }

            return responseList;
        }

        public async Task DeleteAsync(Guid blogId)
        {
            var blog = await _dbContext.Blogs.FindAsync(blogId);
            if (blog == null)
                throw new KeyNotFoundException($"Blog with ID {blogId} not found.");

            _dbContext.Blogs.Remove(blog);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<BlogResponseModel> ToBlogResponse(Blog blog, int languageId)
        {
            var tagResponses = new List<BlogTagResponseModel>();

            foreach (var tag in blog.BlogTags)
            {
                var translatedTag = await _translationService.GetTranslationAsync(tag.Id,  ContentTranslationKeys.Tag, languageId, ContentTypeEnum.BlogTag);
                tagResponses.Add(new BlogTagResponseModel
                {
                    TagId = tag.Id,
                    Tag = translatedTag
                });
            }

            return new BlogResponseModel
            {
                BlogId = blog.Id,
                Title = await _translationService.GetTranslationAsync(blog.Id, ContentTranslationKeys.Title, languageId, ContentTypeEnum.Blog) ?? string.Empty,
                Subtitle = await _translationService.GetTranslationAsync(blog.Id, ContentTranslationKeys.Subtitle, languageId, ContentTypeEnum.Blog),
                MainImage = await _translationService.GetTranslationAsync(blog.Id, ContentTranslationKeys.MainImage, languageId, ContentTypeEnum.Blog),
                Content = await _translationService.GetTranslationAsync(blog.Id, ContentTranslationKeys.Content, languageId, ContentTypeEnum.Blog) ?? string.Empty,
                CreatedAt = blog.CreatedAt,
                UpdatedAt = blog.UpdatedAt,
                LanguageId = languageId,
                Tags = tagResponses
            };
        }

        public async Task<string> UploadBlogMediaAsync(IFormFile mediaFile)
        {
            if (mediaFile == null || mediaFile.Length == 0)
                throw new Exception("Invalid media file.");

            var fileUrl = await _fileStorageService.UploadFileAsync(mediaFile, "blog-media");
            return fileUrl;
        }
    }
}
