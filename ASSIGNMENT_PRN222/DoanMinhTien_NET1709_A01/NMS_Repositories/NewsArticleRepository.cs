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
        public List<NewsArticle> GetNewsArticles()
            => NewsArticleDAO.Instance.GetNewsArticles();
    }
}
