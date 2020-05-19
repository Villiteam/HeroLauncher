using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroLauncher
{
    public class Blog
    {
        public int Id { get; set; }
        public String Title { get; set; }
        public String SubTitle { get; set; }
        public String Content { get; set; }
        public String Img { get; set; }
        public String Tag { get; set; }
        public String Author { get; set; }

        public Blog(int id, String title, String subtitle, String content, String img, String tag, String author)
        {
            this.Id = id;
            this.Title = title;
            this.SubTitle = subtitle;
            this.Content = content;
            this.Img = img;
            this.Tag = tag;
            this.Author = author;
        }
    }
}
