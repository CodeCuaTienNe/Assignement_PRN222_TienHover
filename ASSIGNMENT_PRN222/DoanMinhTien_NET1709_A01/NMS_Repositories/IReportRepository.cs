using NMS_BusinessObjects;
using System;
using System.Collections.Generic;

namespace NMS_Repositories
{
    public interface IReportRepository
    {
        List<NewsArticle> GetArticlesByDateRange(DateTime startDate, DateTime endDate);
        Dictionary<string, int> GetArticleCountByCategory(DateTime startDate, DateTime endDate);
        Dictionary<string, int> GetArticleCountByAuthor(DateTime startDate, DateTime endDate);
        Dictionary<string, int> GetArticleCountByStatus(DateTime startDate, DateTime endDate);
        Dictionary<string, int> GetArticleCountByDay(DateTime startDate, DateTime endDate);
    }
}
