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
                // Check if category name already exists
                if (_context.Categories.Any(c => c.CategoryName.ToLower() == category.CategoryName.ToLower()))
                {
                    throw new Exception($"Category with name '{category.CategoryName}' already exists");
                }
                
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
                // Check if name is duplicated with another category
                if (!string.IsNullOrEmpty(category.CategoryName))
                {
                    string nameLower = category.CategoryName.ToLower();
                    var duplicateCategory = _context.Categories
                        .Where(c => c.CategoryId != category.CategoryId && c.CategoryName.ToLower() == nameLower)
                        .FirstOrDefault();
                    
                    if (duplicateCategory != null)
                    {
                        throw new Exception($"Category with name '{category.CategoryName}' already exists");
                    }
                }
                
                // Check for circular references in parent-child relationships
                if (category.ParentCategoryId.HasValue && category.ParentCategoryId.Value == category.CategoryId)
                {
                    throw new Exception("A category cannot be its own parent");
                }
                
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

        public bool HasChildCategories(short id)
        {
            return _context.Categories.Any(c => c.ParentCategoryId == id);
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
                    if (HasChildCategories(id))
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

        public List<Category> SearchCategories(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllCategories();
                
            // Convert search term to lowercase for case-insensitive comparison
            string searchTermLower = searchTerm.ToLower();
            
            return _context.Categories
                .Include(c => c.ParentCategory)
                .Where(c => c.CategoryName.ToLower().Contains(searchTermLower) || 
                           (c.CategoryDesciption != null && c.CategoryDesciption.ToLower().Contains(searchTermLower)))
                .ToList();
        }

        public List<Category> GetChildCategories(short parentId)
        {
            return _context.Categories
                .Where(c => c.ParentCategoryId == parentId)
                .ToList();
        }

        public int GetArticleCountForCategory(short categoryId)
        {
            return _context.NewsArticles
                .Count(a => a.CategoryId == categoryId);
        }

        public List<NewsArticle> GetArticlesByCategory(short categoryId)
        {
            return _context.NewsArticles
                .Include(a => a.CreatedBy)
                .Where(a => a.CategoryId == categoryId)
                .OrderByDescending(a => a.CreatedDate)
                .ToList();
        }
    }
}
