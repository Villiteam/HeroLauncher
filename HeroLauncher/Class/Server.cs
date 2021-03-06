﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroLauncher
{
    public class Server
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public String Version { get; set; } 
        public String Img { get; set; }
        public String IpV4 { get; set; }
        public string Port { get; set; }
        public String Active { get; set; }

        public Server(int id, String name, String version, string ipv4, string port, string img)
        {
            this.Id = id;
            this.Name = name;
            this.Version = version;
            this.IpV4 = ipv4;
            this.Port = port;
            this.Img = img;
        }
    }
}
