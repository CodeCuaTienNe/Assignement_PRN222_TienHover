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
        Category GetCategoryById(short id); // Changed from int to short to match the model
        void AddCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(short id); // Changed from int to short
        bool IsCategoryInUse(short id); // Add method to check if category is used by articles
    }
}
