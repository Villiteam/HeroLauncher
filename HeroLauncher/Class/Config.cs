using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroLauncher.Class
{
    class Config
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public bool FullScreen { get; set; }
        public bool ShowSnapshots { get; set; }
        public bool ShowBeta { get; set; }
        public bool ShowAlpha { get; set; }
        public int Memory { get; set; }
        public string Account { get; set; }
        public string Version { get; set; }
        public string JVMArguments { get; set; }
        public string MinecraftArguments { get; set; }
        public string JavaPath { get; set; }
    }
}
