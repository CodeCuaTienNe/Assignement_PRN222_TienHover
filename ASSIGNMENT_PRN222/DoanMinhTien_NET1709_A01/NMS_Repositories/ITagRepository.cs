using NMS_BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMS_Repositories
{
    public interface ITagRepository
    {
        List<Tag> GetAllTags();
        Tag GetTagById(int id);
        void AddTag(Tag tag);
        void UpdateTag(Tag tag);
        void DeleteTag(int id);
        bool IsTagInUse(int id);
        Tag GetTagByName(string name);
        List<Tag> SearchTags(string searchTerm);
        List<NewsArticle> GetArticlesByTag(int tagId);
    }
}
