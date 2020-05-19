using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroLauncher
{
    class Version
    {
        public String id { get; set; }
        public String type { get; set; }
        public String url { get; set; }
        public String time { get; set; }
        public String releaseTime { get; set; }

        public Version(String id, String type, String url, String time, String releaseTime)
        {
            this.id = id;
            this.type = type;
            this.url = url;
            this.time = time;
            this.releaseTime = releaseTime;
        }
    }
}
