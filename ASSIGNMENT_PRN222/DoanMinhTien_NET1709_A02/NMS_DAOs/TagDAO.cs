using NMS_BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMS_DAOs
{
    public class TagDAO
    {
        private FunewsManagementContext _context;
        private static TagDAO _instance;

        public TagDAO()
        {
            _context = new FunewsManagementContext();
        }

        public static TagDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TagDAO();
                }
                return _instance;
            }
        }

        public List<Tag> GetAllTags()
        {
            return _context.Tags.ToList();
        }

        public Tag GetTagById(int id)
        {
            return _context.Tags.FirstOrDefault(t => t.TagId == id);
        }

        public Tag GetTagByName(string name)
        {
            // Modified to use a database-compatible approach for case-insensitive comparison
            if (string.IsNullOrEmpty(name))
                return null;
                
            // Convert both sides to lowercase for case-insensitive comparison
            string nameLower = name.ToLower();
            return _context.Tags.FirstOrDefault(t => t.TagName.ToLower() == nameLower);
        }

        public void AddTag(Tag tag)
        {
            try
            {
                // Check if tag already exists
                var existingTag = GetTagByName(tag.TagName);
                if (existingTag != null)
                {
                    throw new Exception($"Tag with name '{tag.TagName}' already exists");
                }

                // Auto-generate ID by finding the max ID and adding 1
                int maxId = 0;
                if (_context.Tags.Any())
                {
                    maxId = _context.Tags.Max(t => t.TagId);
                }
                tag.TagId = maxId + 1;

                _context.Tags.Add(tag);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding tag: " + ex.Message);
            }
        }

        public void UpdateTag(Tag tag)
        {
            try
            {
                // Check if name is duplicated with another tag
                // Convert both sides to lowercase for case-insensitive comparison
                if (!string.IsNullOrEmpty(tag.TagName))
                {
                    string nameLower = tag.TagName.ToLower();
                    var existingTag = _context.Tags
                        .Where(t => t.TagId != tag.TagId && t.TagName.ToLower() == nameLower)
                        .FirstOrDefault();
                    
                    if (existingTag != null)
                    {
                        throw new Exception($"Tag with name '{tag.TagName}' already exists");
                    }
                }

                var tagToUpdate = _context.Tags.Find(tag.TagId);
                if (tagToUpdate != null)
                {
                    _context.Entry(tagToUpdate).CurrentValues.SetValues(tag);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("Tag not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating tag: " + ex.Message);
            }
        }

        public bool IsTagInUse(int id)
        {
            // Check if tag is used by any news articles
            // Load the tag with its news articles
            var tag = _context.Tags
                .Include(t => t.NewsArticles)
                .FirstOrDefault(t => t.TagId == id);
                
            return tag != null && tag.NewsArticles.Count > 0;
        }

        public void DeleteTag(int id)
        {
            try
            {
                // Check if the tag is in use
                if (IsTagInUse(id))
                {
                    throw new Exception("Cannot delete this tag because it is used by one or more news articles");
                }

                var tag = _context.Tags.Find(id);
                if (tag != null)
                {
                    _context.Tags.Remove(tag);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("Tag not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting tag: " + ex.Message);
            }
        }

        public List<Tag> SearchTags(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllTags();
                
            // Convert search term to lowercase for case-insensitive comparison
            string searchTermLower = searchTerm.ToLower();
            
            return _context.Tags
                .Where(t => t.TagName.ToLower().Contains(searchTermLower) || 
                          (t.Note != null && t.Note.ToLower().Contains(searchTermLower)))
                .ToList();
        }

        public List<NewsArticle> GetArticlesByTag(int tagId)
        {
            var tag = _context.Tags
                .Include(t => t.NewsArticles)
                    .ThenInclude(a => a.CreatedBy)
                .Include(t => t.NewsArticles)
                    .ThenInclude(a => a.Category)
                .FirstOrDefault(t => t.TagId == tagId);
            
            if (tag == null)
            {
                return new List<NewsArticle>();
            }
            
            return tag.NewsArticles.OrderByDescending(a => a.CreatedDate).ToList();
        }
    }
}
