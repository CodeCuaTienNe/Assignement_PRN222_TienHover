using NMS_BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NMS_DAOs
{
    public class CategoryDAO
    {
        private FunewsManagementContext _context;
        private static CategoryDAO _instance;

        public CategoryDAO()
        {
            _context = new FunewsManagementContext();
        }

        public static CategoryDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CategoryDAO();
                }
                return _instance;
            }
        }

        public List<Category> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        public Category GetCategoryById(int id)
        {
            return _context.Categories.FirstOrDefault(c => c.CategoryId == id);
        }

        public void AddCategory(Category category)
        {
            try
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding category: " + ex.Message);
            }
        }

        public void UpdateCategory(Category category)
        {
            try
            {
                var existingCategory = _context.Categories.Find(category.CategoryId);
                if (existingCategory != null)
                {
                    _context.Entry(existingCategory).CurrentValues.SetValues(category);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating category: " + ex.Message);
            }
        }

        public void DeleteCategory(int id)
        {
            try
            {
                // Check if category is used in any news article
                bool isUsed = _context.NewsArticles.Any(n => n.CategoryId == id);
                if (isUsed)
                {
                    throw new Exception("Cannot delete category because it is used in news articles");
                }

                var category = _context.Categories.Find(id);
                if (category != null)
                {
                    _context.Categories.Remove(category);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting category: " + ex.Message);
            }
        }
    }
}
