using NMS_BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMS_Repositories
{
    public interface INewsArticleRepository
    {
        List<NewsArticle> GetAllNewsArticles();
        NewsArticle GetNewsArticleById(string id);
        NewsArticle GetNewsArticleByTitle(string title);
        void AddNewsArticle(NewsArticle newsArticle);
        void UpdateNewsArticle(NewsArticle newsArticle);
        void DeleteNewsArticle(string id);
        List<NewsArticle> SearchNewsArticles(string searchTerm);
        List<NewsArticle> GetNewsArticlesByAuthor(short authorId);
        void AddTagsToArticle(string newsArticleId, List<int> tagIds);
        List<Tag> GetTagsForArticle(string newsArticleId);
    }
}
