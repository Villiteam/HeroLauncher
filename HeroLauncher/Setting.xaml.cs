using HeroLauncher.Class;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HeroLauncher
{
    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class Setting : Window
    {
        public Setting()
        {
            InitializeComponent();
            using (StreamReader r = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\Setting.json"))
            {
                string json = r.ReadToEnd();
                Config config = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(json);
                Width.Text = config.Width.ToString();
                Height.Text = config.Height.ToString();
                if (config.FullScreen)
                {
                    FullScreen.IsChecked = true;
                }
                else
                {
                    FullScreen.IsChecked = false;
                }

                if (config.ShowSnapshots)
                {
                    Snapshots.IsChecked = true;
                }
                else
                {
                    Snapshots.IsChecked = false;
                }

                if (config.ShowBeta)
                {
                    Beta.IsChecked = true;
                }
                else
                {
                    Beta.IsChecked = false;
                }

                if (config.ShowAlpha)
                {
                    Alpha.IsChecked = true;
                }
                else
                {
                    Alpha.IsChecked = false;
                }
                JVM.Text = config.JVMArguments;
                MinecraftArg.Text = config.MinecraftArguments;
                JavaPath.Text = config.JavaPath;
                Memorys.Value = config.Memory;
                MemoryTextBox.Text = config.Memory.ToString();
                r.Close();
            }
        }

        private void Memorys_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MemoryTextBox.Text = Memorys.Value.ToString();
        }

        private void MemoryTextBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Memorys.Value = Double.Parse(MemoryTextBox.Text);
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var tw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.HeroLauncher\Setting.json", false))
            {
                Config config = new Config();
                config.Account = "";
                if (FullScreen.IsChecked == true)
                {
                    config.FullScreen = true;
                }
                else
                {
                    config.FullScreen = false;
                }
                config.Height = int.Parse(Height.Text);
                config.Width = int.Parse(Width.Text);
                config.Memory = int.Parse(MemoryTextBox.Text);

                if (Snapshots.IsChecked == true)
                {
                    config.ShowSnapshots = true;
                }
                else
                {
                    config.ShowSnapshots = false;
                }
                if (Beta.IsChecked == true)
                {
                    config.ShowBeta = true;
                }
                else
                {
                    config.ShowBeta = false;
                }
                if (Alpha.IsChecked == true)
                {
                    config.ShowAlpha = true;
                }
                else
                {
                    config.ShowAlpha = false;
                }
                config.Version = "";
                config.JVMArguments = JVM.Text;
                config.MinecraftArguments = MinecraftArg.Text;
                config.JavaPath = JavaPath.Text;
                tw.WriteLine(JsonConvert.SerializeObject(config).ToString());
                tw.Close();
            }
            this.Close();
        }
    }
}
