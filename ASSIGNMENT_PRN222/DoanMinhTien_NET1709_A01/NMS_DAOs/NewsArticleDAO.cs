﻿using NMS_BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
            return _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .ToList();
        }

        //Get by ID
        public NewsArticle GetNewsArticleById(string id)
        {
            return _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .SingleOrDefault(m => m.NewsArticleId.Equals(id));
        }

        public NewsArticle GetNewsArticleByTitle(string title)
        {
            return _context.NewsArticles.SingleOrDefault(m=>m.NewsTitle.Equals(title));
        }

        public void AddNewsArticle(NewsArticle newsArticle)
        {
            try
            {
                // Kiểm tra ID đã tồn tại chưa
                var existingArticleById = _context.NewsArticles.FirstOrDefault(a => a.NewsArticleId == newsArticle.NewsArticleId);
                if (existingArticleById != null)
                {
                    throw new Exception($"News article with ID '{newsArticle.NewsArticleId}' is already existed");
                }

                // Kiểm tra tiêu đề đã tồn tại chưa
                var existingArticleByTitle = GetNewsArticleByTitle(newsArticle.NewsTitle);
                if (existingArticleByTitle != null)
                {
                    throw new Exception($"News article with title '{newsArticle.NewsTitle}' is already existed");
                }

                // Thêm bài viết mới
                _context.NewsArticles.Add(newsArticle);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error when create news: " + ex.Message);
            }
        }

        public List<NewsArticle> SearchNewsArticles(string searchTerm)
        {
            try
            {
                // Convert the search term to lowercase for case-insensitive comparison
                string searchTermLower = searchTerm.ToLower();
                
                return _context.NewsArticles
                    .Include(n => n.Category)
                    .Include(n => n.CreatedBy)
                    .Where(n => 
                        (n.NewsTitle != null && n.NewsTitle.ToLower().Contains(searchTermLower)) || 
                        (n.Headline != null && n.Headline.ToLower().Contains(searchTermLower)) ||
                        (n.NewsContent != null && n.NewsContent.ToLower().Contains(searchTermLower)))
                    .OrderByDescending(n => n.CreatedDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching news articles: {ex.Message}", ex);
            }
        }

        public List<NewsArticle> GetNewsArticlesByAuthor(short authorId)
        {
            try
            {
                return _context.NewsArticles
                    .Include(n => n.Category)
                    .Include(n => n.CreatedBy)
                    .Where(n => n.CreatedById == authorId)
                    .OrderByDescending(n => n.CreatedDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving articles by author: {ex.Message}", ex);
            }
        }

        public void UpdateNewsArticle(NewsArticle newsArticle)
        {
            try
            {
                var existingArticle = _context.NewsArticles.Find(newsArticle.NewsArticleId);
                if (existingArticle != null)
                {
                    _context.Entry(existingArticle).CurrentValues.SetValues(newsArticle);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating news article: " + ex.Message);
            }
        }

        public void DeleteNewsArticle(string id)
        {
            try
            {
                var newsArticle = GetNewsArticleById(id);
                if (newsArticle != null)
                {
                    _context.NewsArticles.Remove(newsArticle);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("News article not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting news article: " + ex.Message);
            }
        }

        public void AddTagsToArticle(string newsArticleId, List<int> tagIds)
        {
            try
            {
                var newsArticle = _context.NewsArticles
                    .Include(n => n.Tags)
                    .FirstOrDefault(n => n.NewsArticleId == newsArticleId);
                    
                if (newsArticle == null)
                {
                    throw new Exception("News article not found");
                }
                
                // Clear existing tags
                newsArticle.Tags.Clear();
                
                // Add selected tags
                foreach (var tagId in tagIds)
                {
                    var tag = _context.Tags.Find(tagId);
                    if (tag != null)
                    {
                        newsArticle.Tags.Add(tag);
                    }
                }
                
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding tags to news article: " + ex.Message, ex);
            }
        }

        public List<Tag> GetTagsForArticle(string newsArticleId)
        {
            try
            {
                var newsArticle = _context.NewsArticles
                    .Include(n => n.Tags)
                    .FirstOrDefault(n => n.NewsArticleId == newsArticleId);
                    
                if (newsArticle == null)
                {
                    throw new Exception("News article not found");
                }
                
                return newsArticle.Tags.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving tags for news article: " + ex.Message, ex);
            }
        }
    }
}
