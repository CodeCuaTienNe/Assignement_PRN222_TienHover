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
            return _context.Tags.FirstOrDefault(t => t.TagName.Equals(name, StringComparison.OrdinalIgnoreCase));
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
                var existingTag = _context.Tags.FirstOrDefault(t => 
                    t.TagName.Equals(tag.TagName, StringComparison.OrdinalIgnoreCase) && 
                    t.TagId != tag.TagId);
                
                if (existingTag != null)
                {
                    throw new Exception($"Tag with name '{tag.TagName}' already exists");
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
            return _context.NewsArticles.Any(a => a.Tags.Any(t => t.TagId == id));
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
    }
}
