using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NMS_BusinessObjects;
using NMS_DAOs;

namespace NMS_Repositories
{
    public class TagRepository : ITagRepository
    {
        public List<Tag> GetAllTags() => TagDAO.Instance.GetAllTags();

        public Tag GetTagById(int id) => TagDAO.Instance.GetTagById(id);

        public Tag GetTagByName(string name) => TagDAO.Instance.GetTagByName(name);

        public void AddTag(Tag tag) => TagDAO.Instance.AddTag(tag);

        public void UpdateTag(Tag tag) => TagDAO.Instance.UpdateTag(tag);

        public void DeleteTag(int id) => TagDAO.Instance.DeleteTag(id);

        public bool IsTagInUse(int id) => TagDAO.Instance.IsTagInUse(id);
    }
}
