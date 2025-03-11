using NMS_BusinessObjects;
using NMS_DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMS_Repositories
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        public void AddNewsArticle(NewsArticle newsArticle)
            => NewsArticleDAO.Instance.AddNewsArticle(newsArticle);

        public void DeleteNewsArticle(string id)
            => NewsArticleDAO.Instance.DeleteNewsArticle(id);

        public List<NewsArticle> GetAllNewsArticles()
            => NewsArticleDAO.Instance.GetNewsArticles();

        public NewsArticle GetNewsArticleById(string id)
            => NewsArticleDAO.Instance.GetNewsArticleById(id);

        public List<NewsArticle> GetNewsArticles()
            => NewsArticleDAO.Instance.GetNewsArticles();

        public List<NewsArticle> SearchNewsArticles(string searchTerm)
            => NewsArticleDAO.Instance.SearchNewsArticles(searchTerm);

        public void UpdateNewsArticle(NewsArticle newsArticle)
            => NewsArticleDAO.Instance.UpdateNewsArticle(newsArticle);
    }
}
