using NMS_BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NMS_DAOs
{
    public class NewsArticleDAO
    {
        private FunewsManagementContext _context;
        private static NewsArticleDAO _instance;

        public NewsArticleDAO()
        {
            _context = new FunewsManagementContext();
        }
        public static NewsArticleDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NewsArticleDAO();
                }
                return _instance;
            }
        }

        //get all
        public List<NewsArticle> GetNewsArticles()
        {
            return _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .ToList();
        }

        //Get by ID
        public NewsArticle GetNewsArticleById(string id)
        {
            return _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .SingleOrDefault(m => m.NewsArticleId.Equals(id));
        }

        public NewsArticle GetNewsArticleByTitle(string title)
        {
            return _context.NewsArticles.SingleOrDefault(m => m.NewsTitle.Equals(title));
        }

        public void AddNewsArticle(NewsArticle newsArticle)
        {
            try
            {
                // Auto-generate news article ID
                string newId = GenerateNewsArticleId();
                newsArticle.NewsArticleId = newId;

                // Kiểm tra tiêu đề đã tồn tại chưa
                var existingArticleByTitle = GetNewsArticleByTitle(newsArticle.NewsTitle);
                if (existingArticleByTitle != null)
                {
                    throw new Exception($"News article with title '{newsArticle.NewsTitle}' is already existed");
                }

                // Thêm bài viết mới
                _context.NewsArticles.Add(newsArticle);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error when create news: " + ex.Message);
            }
        }

        // New method to generate news article ID
        private string GenerateNewsArticleId()
        {
            // Get the highest current ID
            var existingIds = _context.NewsArticles
                .Select(a => a.NewsArticleId)
                .ToList();

            // If no articles exist, start with "1"
            if (!existingIds.Any())
            {
                return "1";
            }

            // Try to find numeric values in existing IDs
            var numericIds = new List<int>();
            foreach (var id in existingIds)
            {
                if (int.TryParse(id, out int numericId))
                {
                    numericIds.Add(numericId);
                }
            }

            // Get the max numeric ID and add 1, or start with 1 if no valid numeric IDs found
            int newNumericId = (numericIds.Any() ? numericIds.Max() : 0) + 1;

            // Return the new ID as a simple string number
            return newNumericId.ToString();
        }

        public List<NewsArticle> SearchNewsArticles(string searchTerm)
        {
            try
            {
                // Convert the search term to lowercase for case-insensitive comparison
                string searchTermLower = searchTerm.ToLower();

                return _context.NewsArticles
                    .Include(n => n.Category)
                    .Include(n => n.CreatedBy)
                    .Where(n =>
                        (n.NewsTitle != null && n.NewsTitle.ToLower().Contains(searchTermLower)) ||
                        (n.Headline != null && n.Headline.ToLower().Contains(searchTermLower)) ||
                        (n.NewsContent != null && n.NewsContent.ToLower().Contains(searchTermLower)))
                    .OrderByDescending(n => n.CreatedDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching news articles: {ex.Message}", ex);
            }
        }

        public List<NewsArticle> GetNewsArticlesByAuthor(short authorId)
        {
            try
            {
                return _context.NewsArticles
                    .Include(n => n.Category)
                    .Include(n => n.CreatedBy)
                    .Where(n => n.CreatedById == authorId)
                    .OrderByDescending(n => n.CreatedDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving articles by author: {ex.Message}", ex);
            }
        }

        public void UpdateNewsArticle(NewsArticle newsArticle)
        {
            try
            {
                var existingArticle = _context.NewsArticles.Find(newsArticle.NewsArticleId);
                if (existingArticle != null)
                {
                    _context.Entry(existingArticle).CurrentValues.SetValues(newsArticle);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating news article: " + ex.Message);
            }
        }

        public void DeleteNewsArticle(string id)
        {
            try
            {
                var newsArticle = _context.NewsArticles
                    .Include(n => n.Tags)
                    .FirstOrDefault(n => n.NewsArticleId == id);

                if (newsArticle == null)
                {
                    throw new Exception($"News article with ID '{id}' not found");
                }

                // Clear all tags associated with this article
                newsArticle.Tags.Clear();

                // Delete the NewsArticle
                _context.NewsArticles.Remove(newsArticle);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting news article: " + ex.Message);
            }
        }

        public void AddTagsToArticle(string newsArticleId, List<int> tagIds)
        {
            try
            {
                var newsArticle = _context.NewsArticles
                    .Include(n => n.Tags)
                    .FirstOrDefault(n => n.NewsArticleId == newsArticleId);

                if (newsArticle == null)
                {
                    throw new Exception("News article not found");
                }

                // Clear existing tags
                newsArticle.Tags.Clear();

                // Add selected tags
                foreach (var tagId in tagIds)
                {
                    var tag = _context.Tags.Find(tagId);
                    if (tag != null)
                    {
                        newsArticle.Tags.Add(tag);
                    }
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding tags to news article: " + ex.Message, ex);
            }
        }

        public List<Tag> GetTagsForArticle(string newsArticleId)
        {
            try
            {
                var newsArticle = _context.NewsArticles
                    .Include(n => n.Tags)
                    .FirstOrDefault(n => n.NewsArticleId == newsArticleId);

                if (newsArticle == null)
                {
                    throw new Exception("News article not found");
                }

                return newsArticle.Tags.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving tags for news article: " + ex.Message, ex);
            }
        }
    }
}
