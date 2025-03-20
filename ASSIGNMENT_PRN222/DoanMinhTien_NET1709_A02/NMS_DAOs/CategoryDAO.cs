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
            return _context.Categories
                .Include(c => c.ParentCategory)
                .ToList();
        }

        public Category GetCategoryById(short id)
        {
            return _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefault(c => c.CategoryId == id);
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
                else
                {
                    throw new Exception("Category not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating category: " + ex.Message);
            }
        }

        public bool IsCategoryInUse(short id)
        {
            return _context.NewsArticles.Any(a => a.CategoryId == id);
        }

        public void DeleteCategory(short id)
        {
            try
            {
                // Check if the category is in use by any articles
                if (IsCategoryInUse(id))
                {
                    throw new Exception("Cannot delete this category because it is used by one or more news articles.");
                }

                var category = _context.Categories.Find(id);
                if (category != null)
                {
                    // Check if this category is used as a parent category
                    if (_context.Categories.Any(c => c.ParentCategoryId == id))
                    {
                        throw new Exception("Cannot delete this category because it is used as a parent category.");
                    }

                    _context.Categories.Remove(category);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("Category not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting category: " + ex.Message);
            }
        }
    }
}
