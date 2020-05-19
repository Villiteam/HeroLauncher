using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroLauncher
{
    public class BlogInfo
    {
        public int Id { get; set; }
        public String TitleBlog { get; set; }
        public String SubTitleBlog { get; set; }
        public String ContentBlog { get; set; }
        public String ImgBlog { get; set; }
        public String TagBlog { get; set; }
        public String AuthorBlog { get; set; }

        public BlogInfo()
        {

        }

        public void setId(int id)
        {
            this.Id = id;
        }

        public void setTitle(String title)
        {
            this.TitleBlog = title;
        }

        public void setSubTitle(String subtitle)
        {
            this.SubTitleBlog = subtitle;
        }

        public void setContent(String content)
        {
            this.ContentBlog = content;
        }

        public void setImg(String img)
        {
            this.ImgBlog = img;
        }

        public void setTag(String tag)
        {
            this.TagBlog = tag;
        }

        public void setAuthor(String author)
        {
            this.AuthorBlog = author;
        }
    }
}
