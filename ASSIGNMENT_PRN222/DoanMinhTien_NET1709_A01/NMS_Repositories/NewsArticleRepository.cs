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
        public List<NewsArticle> GetAllNewsArticles() => NewsArticleDAO.Instance.GetNewsArticles();

        public NewsArticle GetNewsArticleById(string id) => NewsArticleDAO.Instance.GetNewsArticleById(id);

        public void AddNewsArticle(NewsArticle newsArticle) => NewsArticleDAO.Instance.AddNewsArticle(newsArticle);

        public void UpdateNewsArticle(NewsArticle newsArticle) => NewsArticleDAO.Instance.UpdateNewsArticle(newsArticle);

        public void DeleteNewsArticle(string id) => NewsArticleDAO.Instance.DeleteNewsArticle(id);

        public List<NewsArticle> SearchNewsArticles(string searchTerm) => NewsArticleDAO.Instance.SearchNewsArticles(searchTerm);

        public List<NewsArticle> GetNewsArticlesByAuthor(short authorId) => NewsArticleDAO.Instance.GetNewsArticlesByAuthor(authorId);

        public void AddTagsToArticle(string newsArticleId, List<int> tagIds) => NewsArticleDAO.Instance.AddTagsToArticle(newsArticleId, tagIds);

        public List<Tag> GetTagsForArticle(string newsArticleId) => NewsArticleDAO.Instance.GetTagsForArticle(newsArticleId);
    }
}
