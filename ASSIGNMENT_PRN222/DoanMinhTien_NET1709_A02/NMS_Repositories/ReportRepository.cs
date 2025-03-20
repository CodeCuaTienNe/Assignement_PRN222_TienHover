using NMS_BusinessObjects;
using NMS_DAOs;
using System;
using System.Collections.Generic;

namespace NMS_Repositories
{
    public class ReportRepository : IReportRepository
    {
        public List<NewsArticle> GetArticlesByDateRange(DateTime startDate, DateTime endDate) => 
            ReportDAO.Instance.GetArticlesByDateRange(startDate, endDate);

        public Dictionary<string, int> GetArticleCountByCategory(DateTime startDate, DateTime endDate) => 
            ReportDAO.Instance.GetArticleCountByCategory(startDate, endDate);

        public Dictionary<string, int> GetArticleCountByAuthor(DateTime startDate, DateTime endDate) => 
            ReportDAO.Instance.GetArticleCountByAuthor(startDate, endDate);

        public Dictionary<string, int> GetArticleCountByStatus(DateTime startDate, DateTime endDate) => 
            ReportDAO.Instance.GetArticleCountByStatus(startDate, endDate);

        public Dictionary<string, int> GetArticleCountByDay(DateTime startDate, DateTime endDate) => 
            ReportDAO.Instance.GetArticleCountByDay(startDate, endDate);
    }
}
