using HeroLauncher.Class;
using HeroLauncher.Properties;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HeroLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static String urlJsonList = "https://drive.google.com/uc?export=view&id=1e9SRxGXajxIqOQFXmaxSFNS5g7q9LzTe";
        private static JObject versionDownload = new JObject();
        private static JObject version = new JObject();
        private static JObject versionMinecraft = new JObject();
        private static JObject versionOptiFine = new JObject();
        private static JObject versionForge = new JObject();
        private static Dictionary<String, Version> verList = new Dictionary<String, Version>();
        private static List<String> verListString = new List<String>();
        private static Config config;
        private string startserver = "";
        private string startport = "";

        public MainWindow()
        {
            InitializeComponent();
            System.Net.ServicePointManager.DefaultConnectionLimit = 1000;

            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\Setting.json"))
            {
                using (StreamReader r = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\Setting.json"))
                {
                    string json = r.ReadToEnd();
                    config = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(json);
                }
            }
            else
            {
                using (var tw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\Setting.json", false))
                {
                    config = new Config();
                    config.Account = "";
                    config.FullScreen = false;
                    config.Height = 530;
                    config.Width = 925;
                    config.Memory = 512;
                    config.ShowSnapshots = false;
                    config.ShowBeta = false;
                    config.ShowAlpha = false;
                    config.Version = "";
                    config.JVMArguments = "";
                    config.MinecraftArguments = "";
                    config.JavaPath = "";
                    tw.WriteLine(JsonConvert.SerializeObject(config).ToString());
                    tw.Close();
                }
            }
            CreateDirectoryAppdata();
            LoadListVersion();
            foreach (var obj in versionMinecraft["versions"])
            {
                if(obj["type"].ToString() == "snapshot")
                {
                    if (config.ShowSnapshots)
                    {
                        verList.Add("Snapshot " + obj["id"].ToString(), new Version(obj["id"].ToString(), obj["type"].ToString(), obj["url"].ToString(), obj["time"].ToString(), obj["releaseTime"].ToString()));
                        verListString.Add("Snapshot " + obj["id"].ToString());
                    }
                } else
                if (obj["type"].ToString() == "old_beta")
                {
                    if (config.ShowBeta)
                    {
                        verList.Add("Beta " + obj["id"].ToString(), new Version(obj["id"].ToString(), obj["type"].ToString(), obj["url"].ToString(), obj["time"].ToString(), obj["releaseTime"].ToString()));
                        verListString.Add("Beta " + obj["id"].ToString());
                    }
                } else
                if (obj["type"].ToString() == "old_alpha")
                {
                    if (config.ShowAlpha)
                    {
                        verList.Add("Alpha " + obj["id"].ToString(), new Version(obj["id"].ToString(), obj["type"].ToString(), obj["url"].ToString(), obj["time"].ToString(), obj["releaseTime"].ToString()));
                        verListString.Add("Alpha " + obj["id"].ToString());
                    }
                } else
                {
                    verList.Add("Release " + obj["id"].ToString(), new Version(obj["id"].ToString(), obj["type"].ToString(), obj["url"].ToString(), obj["time"].ToString(), obj["releaseTime"].ToString()));
                    verListString.Add("Release " + obj["id"].ToString());
                }
            }
            VersionCombobox.ItemsSource = verListString;
            List<Server> servers = new List<Server>();
            using (var context = new HeroLauncherEntities())
            {
                var query = context.ServerList.OrderByDescending(t => t.Id).Take(10);
                foreach(var serv in query)
                {
                    servers.Add(new Server(serv.Id, serv.Name, serv.Version, serv.IpV4, serv.Port.ToString(), serv.Img));
                }
                ServerListView.ItemsSource = servers;
            }

            List<Blog> blogs = new List<Blog>();
            using (var context = new HeroLauncherEntities())
            {
                var query = context.Blogs.OrderByDescending(t => t.Id).Take(10);
                foreach (var serv in query)
                {
                    blogs.Add(new Blog(serv.Id, serv.Title, serv.Subtitle, serv.Content, serv.Img, serv.Tag, serv.Author));
                }
                BlogListView.ItemsSource = blogs;
            }
            
        }

        private static void CreateDirectoryAppdata()
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\assets\indexes");
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\assets\objects");
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\assets\virtual");
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries");
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\versions");
        }

        private static void LoadListVersion()
        {
            LoadJsonVersion();
        }

        private static void LoadJsonVersion()
        {
            try
            {
                version = GetJsonFromServer(urlJsonList);
                versionMinecraft = GetJsonFromServer(version.GetValue("Minecraft").ToString());
                versionOptiFine = GetJsonFromServer(version.GetValue("OptiFine").ToString());
                versionForge = GetJsonFromServer(version.GetValue("Forge").ToString());
            }
            catch { }            
        }

        private static JObject GetJsonFromServer(string url)
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                try
                {
                    json_data = w.DownloadString(url);
                }
                catch (Exception) { }
                return JObject.Parse(json_data);
            }
        }

        private void PlayButtom_Click(object sender, RoutedEventArgs e)
        {
            if (UserNameTextBox.Text.Length == 0)
            {
                MessageBox.Show("Vui Lòng Nhập Tên User");
            } else
            {
                if(VersionCombobox.SelectedItem == null)
                {
                    MessageBox.Show("Vui Lòng Chọn Phiên Bản Minecraft");
                } else
                {
                    RunMinecraft(UserNameTextBox.Text, VersionCombobox.SelectedItem.ToString());
                }
            }
        }

        private void RunMinecraft(String User, String VersionString)
        {
            progressBar.Value = 0;
            BackgroundWorker bgWorkerExport = new BackgroundWorker();
            bgWorkerExport.WorkerReportsProgress = true;
            bgWorkerExport.DoWork += Export_DoWork;
            List<String> arguments = new List<String>();
            arguments.Add(VersionString);
            arguments.Add(User);
            bgWorkerExport.RunWorkerAsync(arguments);
        }

        private void Export_DoWork(object sender, DoWorkEventArgs e)
        {
            List<String> genericlist = e.Argument as List<String>;
            DownloadVersion(genericlist[0], genericlist[1]);
        }

        private bool DownloadVersion(String VersionDownload, String User)
        {
            Version result;
            String AssetsPathRun = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\assets";
            String ClassPathRun = "";
            String NativesPath = "";
            String FullCmd;
            ShowHideProgress(true);
            if (verList.TryGetValue(VersionDownload, out result))
            {
                versionDownload = GetJsonFromServer(result.url);
            } else
            {
                return false;
            }
            //Assets
            runFunction(5);
            runFunction1("Run Download");
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\assets\indexes\" + versionDownload["assetIndex"]["id"].ToString() + @".json"))
            {
                if (new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\assets\indexes\" + versionDownload["assetIndex"]["id"].ToString() + @".json").Length != long.Parse(versionDownload["assetIndex"]["size"].ToString()))
                {
                    new WebClient().DownloadFile(versionDownload["assetIndex"]["url"].ToString(), Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\assets\indexes\" + versionDownload["assetIndex"]["id"].ToString() + @".json");
                }
            } else
            {
                new WebClient().DownloadFile(versionDownload["assetIndex"]["url"].ToString(), Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\assets\indexes\" + versionDownload["assetIndex"]["id"].ToString() + @".json");
            }
            JObject JsonAssetsFile;
            using (StreamReader r = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\assets\indexes\" + versionDownload["assetIndex"]["id"].ToString() + @".json"))
            {
                string JsonAssetsFileString = r.ReadToEnd();
                JsonAssetsFile = JObject.Parse(JsonAssetsFileString);
            }

            runFunction(10);
            runFunction1("Download Assets");
            int countAsset = 0;
            List<String> ready = new List<string>();
            List<Task> tasksAsset = new List<Task>();
            foreach (var obj in JsonAssetsFile["objects"])
            {
                String hashAssets = obj.First["hash"].ToString();
                if (!ready.Contains(hashAssets))
                {
                    ready.Add(hashAssets);
                    long sizeAssets = long.Parse(obj.First["size"].ToString());
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\assets\objects\" + hashAssets[0] + hashAssets[1]);
                    if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\assets\objects\" + hashAssets[0] + hashAssets[1] + @"\" + hashAssets))
                    {
                        if (new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\assets\objects\" + hashAssets[0] + hashAssets[1] + @"\" + hashAssets).Length != sizeAssets)
                        {
                            tasksAsset.Add(Task.Factory.StartNew(() => new WebClient().DownloadFile("http://resources.download.minecraft.net/" + hashAssets[0] + hashAssets[1] + @"/" + hashAssets, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\assets\objects\" + hashAssets[0] + hashAssets[1] + @"\" + hashAssets)));
                        }
                    }
                    else
                    {
                        tasksAsset.Add(Task.Factory.StartNew(() => new WebClient().DownloadFile("http://resources.download.minecraft.net/" + hashAssets[0] + hashAssets[1] + @"/" + hashAssets, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\assets\objects\" + hashAssets[0] + hashAssets[1] + @"\" + hashAssets)));
                    }
                    countAsset++;
                    runFunction(10 + countAsset * (30 / JsonAssetsFile["objects"].Count()));
                    runFunction1("Assets " + countAsset + "/" + JsonAssetsFile["objects"].Count());
                } else
                {
                    countAsset++;
                }
            }
            Task.WaitAll(tasksAsset.ToArray());
            ready.Clear();
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\versions\" + versionDownload["id"].ToString() + @"\natives");
            //Library
            WebClient webClient = new WebClient();
            int countClassPath = 0;
            foreach (var obj in versionDownload["libraries"])
            {
                if (obj["rules"] == null)
                {
                    if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + obj["downloads"]["artifact"]["path"]))
                    {
                        if (new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + obj["downloads"]["artifact"]["path"]).Length != long.Parse(obj["downloads"]["artifact"]["size"].ToString()))
                        {
                            Directory.CreateDirectory(new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + obj["downloads"]["artifact"]["path"]).DirectoryName);
                            Task.Factory.StartNew(() => new WebClient().DownloadFile(obj["downloads"]["artifact"]["url"].ToString(), Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + obj["downloads"]["artifact"]["path"])).Wait();
                        }
                        if (countClassPath == 0)
                        {
                            ClassPathRun = ClassPathRun + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + @obj["downloads"]["artifact"]["path"].ToString().Replace("/", @"\");
                        }
                        else
                        {
                            ClassPathRun = ClassPathRun + @";" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + @obj["downloads"]["artifact"]["path"].ToString().Replace("/", @"\");
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + obj["downloads"]["artifact"]["path"]).DirectoryName);
                        Task.Factory.StartNew(() => new WebClient().DownloadFile(obj["downloads"]["artifact"]["url"].ToString(), Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + obj["downloads"]["artifact"]["path"])).Wait();
                        if (countClassPath == 0)
                        {
                            ClassPathRun = ClassPathRun + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + @obj["downloads"]["artifact"]["path"].ToString().Replace("/", @"\");
                        }
                        else
                        {
                            ClassPathRun = ClassPathRun + @";" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + @obj["downloads"]["artifact"]["path"].ToString().Replace("/", @"\");
                        }
                    }
                }
                else
                {
                    bool allow = true;
                    foreach (var objrules in obj["rules"])
                    {
                        if (objrules["os"] == null)
                        {
                            if (objrules["action"].ToString() == "allow")
                            {
                                allow = true;
                            }
                            else
                            {
                                allow = false;
                            }
                        }
                        else
                        {
                            if (objrules["action"].ToString() == "allow")
                            {
                                if (objrules["os"]["name"].ToString() == "osx")
                                {
                                    allow = false;
                                }
                            }
                            else
                            {
                                if (objrules["os"]["name"].ToString() == "osx")
                                {
                                    allow = true;
                                }
                            }
                        }
                    }

                    if (allow == true)
                    {
                        if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + obj["downloads"]["artifact"]["path"]))
                        {
                            if (new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + obj["downloads"]["artifact"]["path"]).Length != long.Parse(obj["downloads"]["artifact"]["size"].ToString()))
                            {
                                Directory.CreateDirectory(new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + obj["downloads"]["artifact"]["path"]).DirectoryName);
                                Task.Factory.StartNew(() => new WebClient().DownloadFile(obj["downloads"]["artifact"]["url"].ToString(), Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + obj["downloads"]["artifact"]["path"])).Wait();
                            }
                            if (countClassPath == 0)
                            {
                                ClassPathRun = ClassPathRun + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + @obj["downloads"]["artifact"]["path"].ToString().Replace("/", @"\");
                            }
                            else
                            {
                                ClassPathRun = ClassPathRun + @";" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + @obj["downloads"]["artifact"]["path"].ToString().Replace("/", @"\");
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + obj["downloads"]["artifact"]["path"]).DirectoryName);
                            Task.Factory.StartNew(() => new WebClient().DownloadFile(obj["downloads"]["artifact"]["url"].ToString(), Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + obj["downloads"]["artifact"]["path"])).Wait();
                            if (countClassPath == 0)
                            {
                                ClassPathRun = ClassPathRun + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + @obj["downloads"]["artifact"]["path"].ToString().Replace("/", @"\");
                            }
                            else
                            {
                                ClassPathRun = ClassPathRun + @";" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + @obj["downloads"]["artifact"]["path"].ToString().Replace("/", @"\");
                            }
                        }
                    }
                }
                //Natives
                if (obj["natives"] != null)
                {
                    if (obj["natives"]["windows"] != null)
                    {
                        if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + obj["downloads"]["classifiers"][obj["natives"]["windows"].ToString()]["path"]))
                        {
                            if (new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + obj["downloads"]["classifiers"][obj["natives"]["windows"].ToString()]["path"]).Length != long.Parse(obj["downloads"]["classifiers"][obj["natives"]["windows"].ToString()]["size"].ToString()))
                            {
                                Directory.CreateDirectory(new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + obj["downloads"]["classifiers"][obj["natives"]["windows"].ToString()]["path"]).DirectoryName);
                                Task.Factory.StartNew(() => new WebClient().DownloadFile(obj["downloads"]["classifiers"][obj["natives"]["windows"].ToString()]["url"].ToString(), Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + obj["downloads"]["classifiers"][obj["natives"]["windows"].ToString()]["path"])).Wait();
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + obj["downloads"]["classifiers"][obj["natives"]["windows"].ToString()]["path"]).DirectoryName);
                            Task.Factory.StartNew(() => new WebClient().DownloadFile(obj["downloads"]["classifiers"][obj["natives"]["windows"].ToString()]["url"].ToString(), Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + obj["downloads"]["classifiers"][obj["natives"]["windows"].ToString()]["path"])).Wait();
                        }

                        FastZip fz = new FastZip();
                        fz.ExtractZip(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\libraries\" + obj["downloads"]["classifiers"][obj["natives"]["windows"].ToString()]["path"], Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\versions\" + versionDownload["id"].ToString() + @"\natives", null);
                    }
                }
                countClassPath++;
                runFunction(40 + countClassPath * (30 / versionDownload["libraries"].Count()));
                runFunction1("Library " + countClassPath + "/" + versionDownload["libraries"].Count());

            }
            NativesPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\versions\" + versionDownload["id"].ToString() + @"\natives";
            //Version
            runFunction1("Start Download Version");
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\versions\" + versionDownload["id"].ToString() + @"\" + versionDownload["id"].ToString() + @".jar"))
            {
                if (new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\versions\" + versionDownload["id"].ToString() + @"\" + versionDownload["id"].ToString() + @".jar").Length != long.Parse(versionDownload["downloads"]["client"]["size"].ToString()))
                {
                    Directory.CreateDirectory(new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\versions\" + versionDownload["id"].ToString() + @"\" + versionDownload["id"].ToString() + @".jar").DirectoryName);
                    Task.Factory.StartNew(() => new WebClient().DownloadFile(versionDownload["downloads"]["client"]["url"].ToString(), Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\versions\" + versionDownload["id"].ToString() + @"\" + versionDownload["id"].ToString() + @".jar")).Wait();
                }
            }
            else
            {
                Directory.CreateDirectory(new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\versions\" + versionDownload["id"].ToString() + @"\" + versionDownload["id"].ToString() + @".jar").DirectoryName);
                Task.Factory.StartNew(() => new WebClient().DownloadFile(versionDownload["downloads"]["client"]["url"].ToString(), Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\versions\" + versionDownload["id"].ToString() + @"\" + versionDownload["id"].ToString() + @".jar")).Wait();
            }

            runFunction(90);
            runFunction1("Download Comple");
            ClassPathRun = ClassPathRun + @";" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\versions\" + versionDownload["id"].ToString() + @"\" + versionDownload["id"].ToString() + @".jar";
            FullCmd = "java";
            foreach (var objArg in versionDownload["arguments"]["jvm"])
            {
                if (objArg.Count() > 1)
                {
                    bool allowarg = true;
                    foreach (var objArgRules in objArg["rules"])
                    {
                        if (objArgRules["action"].ToString() == "allow")
                        {
                            if (objArgRules["os"]["name"] != null)
                            {
                                if (objArgRules["os"]["name"].ToString() == "windows")
                                {
                                    allowarg = true;
                                }
                                else
                                {
                                    allowarg = false;
                                }
                            }
                            if (objArgRules["os"]["version"] != null)
                            {
                                if (IsWindows10() && @objArgRules["os"]["version"].ToString() == @"^10\.")
                                {
                                    allowarg = true;
                                }
                                else
                                {
                                    allowarg = false;
                                }
                            }
                            if (objArgRules["os"]["arch"] != null)
                            {
                                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432")) && objArg["rules"]["os"]["arch"].ToString() == "x86")
                                {
                                    allowarg = true;
                                }
                                else
                                {
                                    allowarg = false;
                                }
                            }
                        }
                    }

                    if (allowarg)
                    {
                        if (objArg["value"].Count() > 1)
                        {
                            foreach (var objArgVar in objArg["value"])
                            {
                                if (objArgVar.ToString().Contains(" "))
                                {
                                    FullCmd = FullCmd + " \"" + objArgVar.ToString() + "\"";
                                } else
                                {
                                    FullCmd = FullCmd + " " + objArgVar.ToString();
                                }
                            }
                        } else
                        {
                            if (objArg["value"].ToString().Contains(" "))
                            {
                                FullCmd = FullCmd + " \"" + objArg["value"].ToString() + "\"";
                            }
                            else
                            {
                                FullCmd = FullCmd + " " + objArg["value"].ToString();
                            }
                        }
                    }
                } else
                {
                    if (objArg.ToString().Contains("${natives_directory}"))
                    {
                        FullCmd = FullCmd + " " + objArg.ToString().Replace("${natives_directory}", "\"" + NativesPath + "\"");
                    }
                    else if (objArg.ToString().Contains("${launcher_name}"))
                    {
                        FullCmd = FullCmd + " " + objArg.ToString().Replace("${launcher_name}", "minecraft-launcher");
                    }
                    else if (objArg.ToString().Contains("${launcher_version}"))
                    {
                        FullCmd = FullCmd + " " + objArg.ToString().Replace("${launcher_version}", "2.0.1003");
                    }
                    else if (objArg.ToString().Contains("${classpath}"))
                    {
                        FullCmd = FullCmd + " " + objArg.ToString().Replace("${classpath}", "\"" + ClassPathRun + "\"");
                    } else
                    {
                        FullCmd = FullCmd + " " + objArg.ToString();
                    }
                }
            }

            FullCmd = FullCmd + " " + versionDownload["mainClass"];

            foreach (var objArg in versionDownload["arguments"]["game"])
            {
                if (objArg.Count() > 1)
                {
                    FullCmd = FullCmd + " --width " + config.Width + " --height " + config.Height;
                }
                else
                {
                    if (objArg.ToString().Contains("${auth_player_name}"))
                    {
                        FullCmd = FullCmd + " " + objArg.ToString().Replace("${auth_player_name}", User);
                    }
                    else if (objArg.ToString().Contains("${version_name}"))
                    {
                        FullCmd = FullCmd + " " + objArg.ToString().Replace("${version_name}", versionDownload["id"].ToString());
                    }
                    else if (objArg.ToString().Contains("${game_directory}"))
                    {
                        FullCmd = FullCmd + " " + objArg.ToString().Replace("${game_directory}", "\"" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher") + "\"";
                    }
                    else if (objArg.ToString().Contains("${assets_root}"))
                    {
                        FullCmd = FullCmd + " " + objArg.ToString().Replace("${assets_root}", "\"" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\assets") + "\"";
                    }
                    else if (objArg.ToString().Contains("${assets_index_name}"))
                    {
                        FullCmd = FullCmd + " " + objArg.ToString().Replace("${assets_index_name}", versionDownload["assets"].ToString());
                    }
                    else if (objArg.ToString().Contains("${auth_uuid}"))
                    {
                        FullCmd = FullCmd + " " + objArg.ToString().Replace("${auth_uuid}", "00000000-0000-0000-0000-000000000000");
                    }
                    else if (objArg.ToString().Contains("${auth_access_token}"))
                    {
                        FullCmd = FullCmd + " " + objArg.ToString().Replace("${auth_access_token}", "null");
                    }
                    else if (objArg.ToString().Contains("${user_type}"))
                    {
                        FullCmd = FullCmd + " " + objArg.ToString().Replace("${user_type}", "legacy");
                    }
                    else if (objArg.ToString().Contains("${version_type}"))
                    {
                        FullCmd = FullCmd + " " + objArg.ToString().Replace("${version_type}", "release");
                    }
                    else
                    {
                        FullCmd = FullCmd + " " + objArg.ToString();
                    }
                }
            }
            if (startserver != null)
            {
                if (startserver.Length >= 1)
                {
                    FullCmd = FullCmd + " --server=\"" + startserver + "\" " + "--port " + startport + " ";
                }
            }

            FullCmd = FullCmd + " -Xmn128M -Xmx" + config.Memory + "M";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/c" + FullCmd;
            process.StartInfo = startInfo;
            process.Start();
            runFunction(100);
            runFunction1("Start");
            MessageBox.Show("Minecraft is starting. Please wating ...");
            ShowHideProgress(false);
            return true;
        }

        static bool IsWindows10()
        {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

            string productName = (string)reg.GetValue("ProductName");

            return productName.StartsWith("Windows 10");
        }

        private async void ShowHideProgress(bool value)
        {
            await Task.Run(() =>
            {
                if (value)
                {
                    progressBar.Dispatcher.Invoke(() => progressBar.Visibility = Visibility.Visible, System.Windows.Threading.DispatcherPriority.Background);
                    LogProgressBar.Dispatcher.Invoke(() => LogProgressBar.Visibility = Visibility.Visible, System.Windows.Threading.DispatcherPriority.Background);
                } else
                {
                    progressBar.Dispatcher.Invoke(() => progressBar.Visibility = Visibility.Collapsed, System.Windows.Threading.DispatcherPriority.Background);
                    LogProgressBar.Dispatcher.Invoke(() => LogProgressBar.Visibility = Visibility.Collapsed, System.Windows.Threading.DispatcherPriority.Background);
                }
            });
        }

        private async void runFunction(float value)
        {
            await Task.Run(() =>
            {
                progressBar.Dispatcher.Invoke(() => progressBar.Value = value, System.Windows.Threading.DispatcherPriority.Background);
            });
        }

        private async void runFunction1(String msg)
        {
            await Task.Run(() =>
            {
                LogProgressBar.Dispatcher.Invoke(() => LogProgressBar.Text = msg, System.Windows.Threading.DispatcherPriority.Background);
            });
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\Setting.json"))
            {
                using (StreamReader r = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\Setting.json"))
                {
                    string json = r.ReadToEnd();
                    config = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(json);
                }
            }
            else
            {
                using (var tw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\Setting.json", false))
                {
                    config = new Config();
                    config.Account = "";
                    config.FullScreen = false;
                    config.Height = 530;
                    config.Width = 925;
                    config.Memory = 512;
                    config.ShowSnapshots = false;
                    config.ShowBeta = false;
                    config.ShowAlpha = false;
                    config.Version = "";
                    config.JVMArguments = "";
                    config.MinecraftArguments = "";
                    config.JavaPath = "";
                    tw.WriteLine(JsonConvert.SerializeObject(config).ToString());
                    tw.Close();
                }
            }
            CreateDirectoryAppdata();
            LoadListVersion();
            verList.Clear();
            foreach (var obj in versionMinecraft["versions"])
            {
                if (obj["type"].ToString() == "snapshot")
                {
                    if (config.ShowSnapshots)
                    {
                        verList.Add("Snapshot " + obj["id"].ToString(), new Version(obj["id"].ToString(), obj["type"].ToString(), obj["url"].ToString(), obj["time"].ToString(), obj["releaseTime"].ToString()));
                        verListString.Add("Snapshot " + obj["id"].ToString());
                    }
                }
                else
                if (obj["type"].ToString() == "old_beta")
                {
                    if (config.ShowBeta)
                    {
                        verList.Add("Beta " + obj["id"].ToString(), new Version(obj["id"].ToString(), obj["type"].ToString(), obj["url"].ToString(), obj["time"].ToString(), obj["releaseTime"].ToString()));
                        verListString.Add("Beta " + obj["id"].ToString());
                    }
                }
                else
                if (obj["type"].ToString() == "old_alpha")
                {
                    if (config.ShowAlpha)
                    {
                        verList.Add("Alpha " + obj["id"].ToString(), new Version(obj["id"].ToString(), obj["type"].ToString(), obj["url"].ToString(), obj["time"].ToString(), obj["releaseTime"].ToString()));
                        verListString.Add("Alpha " + obj["id"].ToString());
                    }
                }
                else
                {
                    verList.Add("Release " + obj["id"].ToString(), new Version(obj["id"].ToString(), obj["type"].ToString(), obj["url"].ToString(), obj["time"].ToString(), obj["releaseTime"].ToString()));
                    verListString.Add("Release " + obj["id"].ToString());
                }
            }
            VersionCombobox.ItemsSource = verListString;
        }

        private void FileFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher");
        }

        private void ServerListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                Server server = (Server)(item.Content);
                ServerChoice.Text = server.Name;
                startserver = server.IpV4;
                startport = server.Port;
            }
        }

        private void BlogListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                Blog blog = (Blog)(item.Content);
                BlogInfo blogInfo = new BlogInfo();
                blogInfo.setAuthor(blog.Author);
                blogInfo.setId(blog.Id);
                blogInfo.setContent(blog.Content);
                blogInfo.setImg(blog.Img);
                blogInfo.setSubTitle(blog.SubTitle);
                blogInfo.setTag(blog.Tag);
                blogInfo.setTitle(blog.Title);
                BlogInfoStack.DataContext = blogInfo;
                BlogInfo.Visibility = Visibility.Visible;
            }
        }

        private void setting_Click(object sender, RoutedEventArgs e)
        {
            Setting setting = new Setting();
            setting.ShowDialog();
        }

        private void CloseBlogButtom_Click(object sender, RoutedEventArgs e)
        {
            BlogInfo.Visibility = Visibility.Collapsed;
        }

        
    }
}
