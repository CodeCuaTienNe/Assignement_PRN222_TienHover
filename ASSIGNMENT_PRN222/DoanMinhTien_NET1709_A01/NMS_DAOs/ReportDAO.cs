using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NMS_BusinessObjects;

namespace NMS_DAOs
{
    public class ReportDAO
    {
        private FunewsManagementContext _context;
        private static ReportDAO _instance;

        public ReportDAO()
        {
            _context = new FunewsManagementContext();
        }

        public static ReportDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ReportDAO();
                }
                return _instance;
            }
        }

        public List<NewsArticle> GetArticlesByDateRange(DateTime startDate, DateTime endDate)
        {
            // Ensure end date includes the entire day
            endDate = endDate.Date.AddDays(1).AddSeconds(-1);

            return _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                .OrderByDescending(n => n.CreatedDate)
                .ToList();
        }

        public Dictionary<string, int> GetArticleCountByCategory(DateTime startDate, DateTime endDate)
        {
            // Ensure end date includes the entire day
            endDate = endDate.Date.AddDays(1).AddSeconds(-1);

            var query = _context.NewsArticles
                .Include(n => n.Category)
                .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate && n.Category != null)
                .GroupBy(n => n.Category.CategoryName)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .ToList();

            var result = new Dictionary<string, int>();
            foreach (var item in query)
            {
                result.Add(item.CategoryName ?? "Uncategorized", item.Count);
            }

            return result;
        }

        public Dictionary<string, int> GetArticleCountByAuthor(DateTime startDate, DateTime endDate)
        {
            // Ensure end date includes the entire day
            endDate = endDate.Date.AddDays(1).AddSeconds(-1);

            var query = _context.NewsArticles
                .Include(n => n.CreatedBy)
                .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate && n.CreatedBy != null)
                .GroupBy(n => n.CreatedBy.AccountName)
                .Select(g => new
                {
                    AuthorName = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .ToList();

            var result = new Dictionary<string, int>();
            foreach (var item in query)
            {
                result.Add(item.AuthorName ?? "Unknown", item.Count);
            }

            return result;
        }

        public Dictionary<string, int> GetArticleCountByStatus(DateTime startDate, DateTime endDate)
        {
            // Ensure end date includes the entire day
            endDate = endDate.Date.AddDays(1).AddSeconds(-1);

            var articles = _context.NewsArticles
                .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                .ToList();

            var result = new Dictionary<string, int>
            {
                { "Active", articles.Count(a => a.NewsStatus == true) },
                { "Inactive", articles.Count(a => a.NewsStatus == false) }
            };

            return result;
        }

        public Dictionary<string, int> GetArticleCountByDay(DateTime startDate, DateTime endDate)
        {
            // Ensure end date includes the entire day
            endDate = endDate.Date.AddDays(1).AddSeconds(-1);

            var articles = _context.NewsArticles
                .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                .ToList();

            // Create a dictionary with all dates in the range
            var result = new Dictionary<string, int>();
            
            // Iterate through all days in the range
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                string formattedDate = date.ToString("yyyy-MM-dd");
                int count = articles.Count(a => a.CreatedDate?.Date == date.Date);
                result.Add(formattedDate, count);
            }

            return result;
        }
    }
}
