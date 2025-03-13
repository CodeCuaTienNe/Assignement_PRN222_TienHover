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
        

        public Category GetCategoryById(int id)
            => CategoryDAO.Instance.GetCategoryById(id);

        public void AddCategory(Category category)
           => CategoryDAO.Instance.AddCategory(category);

        public void UpdateCategory(Category category)
           => CategoryDAO.Instance.UpdateCategory(category);
        

        public void DeleteCategory(int id)
           => CategoryDAO.Instance.DeleteCategory(id);
    }
}
