using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Threading;

namespace EasyLauncher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class LauncherGroup
        {
            public String Name { get; set; }
            public string Process { get; set; }
            public List<LauncherEntry> Entries { get; set; }
            
        }

        public class LauncherEntry
		{
            public string Name { get; set; }
            public string Process { get; set; }
            public string Arguments { get; set; }

        }

        private List<LauncherGroup> _launcherGroups;

        private void EnumerateWork(DirectoryInfo di)
		{
            var dirFiles = di.GetFiles("*.sln");

            var solutionGroup = _launcherGroups.Where(g => g.Name == "VS Solutions").First();

            foreach (var dirFile in dirFiles)
            {
                solutionGroup.Entries.Add(new LauncherEntry
                {
                    Name = dirFile.Name,
                    Arguments = dirFile.FullName
                });
            }
            
            foreach(var dir in di.GetDirectories())
			{
                if (dir.Name.StartsWith("irs-"))
                    continue;

                EnumerateWork(dir);
			}
		}

        private void SaveConfig()
		{
            using (var fs = new FileStream("config.json", FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            using(var sw = new StreamWriter(fs))
			{
                var launcherGroups = Newtonsoft.Json.JsonConvert.SerializeObject(_launcherGroups, Newtonsoft.Json.Formatting.Indented);
                sw.Write(launcherGroups);
                sw.Flush();
			}
		}

        private void LoadConfig()
		{
            using (var fs = new FileStream("config.json", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs))
            {
                var jsonString = sr.ReadToEnd();
                _launcherGroups = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LauncherGroup>>(jsonString);
            }
        }

        private void BuildConfig()
		{
            LoadConfig();
            foreach(var dir in new DirectoryInfo("c:\\work").GetDirectories("irs-*"))
                EnumerateWork(dir);

            SaveConfig();
		}

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            contextMenuStrip1.Items.Clear();
            LoadConfig();

            foreach (var group in _launcherGroups)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = group.Name;
                item.Tag = group;

                foreach (var entry in group.Entries)
                {
                    ToolStripMenuItem menuItem = new ToolStripMenuItem();
                    menuItem.Text = entry.Name;
                    menuItem.Tag = entry;
					menuItem.Click += MenuItem_Click;

                    item.DropDownItems.Add(menuItem);
                }

                contextMenuStrip1.Items.Add(item);
            }

            ToolStripMenuItem closeItem = new ToolStripMenuItem();
            closeItem.Text = "Exit";
            closeItem.Click += new EventHandler(delegate (object sndr, EventArgs eva) { this.Close(); });
            contextMenuStrip1.Items.Add(closeItem);
        }

		private void MenuItem_Click(object sender, EventArgs e)
		{
            try
            {
                var menu = ((ToolStripMenuItem)sender);
                var entry = ((LauncherEntry)menu.Tag);
                var group = ((LauncherGroup)menu.OwnerItem.Tag);

                var processName = group.Process;
                if (!String.IsNullOrWhiteSpace(entry.Process))
                {
                    processName = entry.Process;
                }

                if(String.IsNullOrWhiteSpace(processName))
				{
                    return;
				}

                var startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = processName;
                startInfo.Arguments = entry.Arguments;
                System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception ex) 
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

    }
}
