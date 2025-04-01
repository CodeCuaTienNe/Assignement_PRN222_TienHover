using NMS_BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMS_Repositories
{
    public interface ICategoryRepository
    {
        List<Category> GetAllCategories();
        Category GetCategoryById(short id);
        void AddCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(short id);
        bool IsCategoryInUse(short id);
        
        // Add these missing methods to match what's used in the Blazor components
        List<Category> SearchCategories(string searchTerm);
        List<Category> GetChildCategories(short parentId);
        int GetArticleCountForCategory(short categoryId);
        List<NewsArticle> GetArticlesByCategory(short categoryId);
    }
}
