using NMS_BusinessObjects;
using NMS_DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMS_Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public List<Category> GetAllCategories()
            => CategoryDAO.Instance.GetAllCategories();
        
        public Category GetCategoryById(short id)
            => CategoryDAO.Instance.GetCategoryById(id);

        public void AddCategory(Category category)
           => CategoryDAO.Instance.AddCategory(category);

        public void UpdateCategory(Category category)
           => CategoryDAO.Instance.UpdateCategory(category);
        
        public void DeleteCategory(short id)
           => CategoryDAO.Instance.DeleteCategory(id);
           
        public bool IsCategoryInUse(short id)
            => CategoryDAO.Instance.IsCategoryInUse(id);
            
        public List<Category> SearchCategories(string searchTerm)
            => CategoryDAO.Instance.SearchCategories(searchTerm);
            
        public List<Category> GetChildCategories(short parentId)
            => CategoryDAO.Instance.GetChildCategories(parentId);
            
        public int GetArticleCountForCategory(short categoryId)
            => CategoryDAO.Instance.GetArticleCountForCategory(categoryId);
            
        public List<NewsArticle> GetArticlesByCategory(short categoryId)
            => CategoryDAO.Instance.GetArticlesByCategory(categoryId);
    }
}
