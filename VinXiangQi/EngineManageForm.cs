using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using VinXiangQi.Properties;

namespace VinXiangQi
{
    public partial class EngineManageForm : Form
    {
        public EngineManageForm()
        {
            InitializeComponent();
        }

        void InitUI()
        {
            listView_engine_list.Items.Clear();
            foreach (var engine in Mainform.Settings.EngineList)
            {
                listView_engine_list.Items.Add(new ListViewItem(new string[] { engine.Key, engine.Value.Author, engine.Value.ExePath }));
            }
        }
        private void EngineManageForm_Load(object sender, EventArgs e)
        {
            InitUI();
        }

        private void button_add_engine_Click(object sender, EventArgs e)
        {
            if (openFileDialog_engine.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog_engine.FileName;
                path = path.Replace(Environment.CurrentDirectory, ".");
                if (!File.Exists(path))
                {
                    MessageBox.Show($"{path} 引擎文件不存在！");
                    return;
                }
                EngineSettings settings = new EngineSettings();
                settings.ExePath = path;
                EngineHelper engine = new EngineHelper(settings.ExePath, settings.Configs);
                engine.Init();
                string[] fileNameParts = settings.ExePath.Split('\\').Last().Split('.');
                if (fileNameParts[fileNameParts.Length - 1] != "exe")
                {
                    MessageBox.Show("不是合法的引擎文件！");
                }
                string fileName = string.Join(".", fileNameParts.Take(fileNameParts.Length - 1));
                string key = fileName;
                if (Mainform.Settings.EngineList.ContainsKey(fileName))
                {
                    for (int i = 1; i <= 100; i++)
                    {
                        if (!Mainform.Settings.EngineList.ContainsKey(fileName + "_" + i.ToString()))
                        {
                            key = fileName + "_" + i.ToString();
                            break;
                        }
                    }
                }
                settings.Name = key;
                settings.Author = engine.EngineAuthor;
                Mainform.Settings.EngineList.Add(key, settings);
                Mainform.SaveSettings();
                InitUI();
            }
        }

        private void button_delete_engine_Click(object sender, EventArgs e)
        {
            if (listView_engine_list.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in listView_engine_list.SelectedItems)
                {
                    string key = item.Text;
                    if (Mainform.Settings.SelectedEngine == key)
                    {
                        if (Mainform.Engine != null)
                        {
                            Mainform.Engine.Stop();
                        }
                        if (Mainform.Settings.EngineList.Count > 0)
                            Mainform.Settings.SelectedEngine = Mainform.Settings.EngineList.Keys.First();
                        else
                            Mainform.Settings.SelectedEngine = "";
                    }
                    Mainform.Settings.EngineList.Remove(key);
                }
                Mainform.SaveSettings();
                InitUI();
            }
        }

        private void button_engine_settings_Click(object sender, EventArgs e)
        {
            if (listView_engine_list.SelectedItems.Count == 1)
            {
                string key = ((ListViewItem)listView_engine_list.SelectedItems[0]).Text;
                EngineSettings settings = Mainform.Settings.EngineList[key];
                EngineHelper engine = new EngineHelper(settings.ExePath, settings.Configs);
                engine.Init();
                if (!engine.Initialized) return;
                EngineSettingsForm engineSettingsForm = new EngineSettingsForm(engine, settings);
                engineSettingsForm.StartPosition = FormStartPosition.CenterParent;
                engineSettingsForm.Text = settings.ExePath + " 引擎设置";
                engineSettingsForm.ShowDialog();
            }
            else
            {
                MessageBox.Show(this, "请先选中一个引擎");
            }
        }

        private void listView_engine_list_DoubleClick(object sender, EventArgs e)
        {
            if (listView_engine_list.SelectedItems.Count == 1)
            {
                string key = ((ListViewItem)listView_engine_list.SelectedItems[0]).Text;
                EngineSettings settings = Mainform.Settings.EngineList[key];
                EngineHelper engine = new EngineHelper(settings.ExePath, settings.Configs);
                engine.Init();
                if (!engine.Initialized) return;
                EngineInfoEditForm engineInfoEditForm = new EngineInfoEditForm(key, settings.Author, settings.ExePath);
                engineInfoEditForm.ShowDialog();
                if (engineInfoEditForm.Saved)
                {
                    settings.Name = engineInfoEditForm.EngineName;
                    settings.Author = engineInfoEditForm.EngineAuthor;
                    if (Mainform.Settings.EngineList.ContainsKey(settings.Name) && Mainform.Settings.EngineList[settings.Name].ExePath != settings.ExePath)
                    {
                        for (int i = 1; i <= 100; i++)
                        {
                            if (!Mainform.Settings.EngineList.ContainsKey(settings.Name + "_" + i.ToString()))
                            {
                                settings.Name += "_" + i.ToString();
                                break;
                            }
                        }
                    }
                    Dictionary<string, EngineSettings> engineList = new Dictionary<string, EngineSettings>();
                    foreach (var item in Mainform.Settings.EngineList)
                    {
                        if (item.Key != key)
                        {
                            engineList.Add(item.Key, item.Value);
                        }
                        else
                        {
                            engineList.Add(settings.Name, item.Value);
                        }
                    }
                    Mainform.Settings.EngineList = engineList;
                    Mainform.SaveSettings();
                    InitUI();
                }
            }
            else
            {
                MessageBox.Show(this, "请先选中一个引擎");
            }
        }
    }
}
