using NMS_BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return _context.NewsArticles.ToList();
        }
    }
}
