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
        List<NewsArticle> GetNewsArticles();
    }
}
