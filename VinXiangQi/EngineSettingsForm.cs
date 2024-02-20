using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VinXiangQi
{
    public partial class EngineSettingsForm : Form
    {
        public EngineSettings CurrentEngineSettings;
        public EngineHelper Engine;

        public EngineSettingsForm(EngineHelper engine, EngineSettings currentEngine)
        {
            Engine = engine;
            CurrentEngineSettings = currentEngine;
            InitializeComponent();
        }

        private void EngineSettingsForm_Load(object sender, EventArgs e)
        {
            flowLayoutPanel.Controls.Clear();
            foreach (string line in Engine.OptionList)
            {
                string[] args = line.Split(' ');
                if (args[0] == "option")
                {
                    int name_end_index;
                    string name, type;
                    string[] params1;
                    if (Engine.EngineType == "ucci")
                    {
                        name_end_index = 1;
                        for (int i = 1; i < args.Length; i++)
                        {
                            if (args[i] == "type")
                            {
                                name_end_index = i - 1;
                            }
                        }
                        name = string.Join(" ", args.Skip(2).Take(name_end_index - 1));
                        type = args[name_end_index + 2];
                        params1 = args.Skip(name_end_index + 3).ToArray();
                    }
                    else
                    {
                        name_end_index = 2;
                        for (int i = 2; i < args.Length; i++)
                        {
                            if (args[i] == "type")
                            {
                                name_end_index = i - 1;
                            }
                        }
                        
                        name = string.Join(" ", args.Skip(2).Take(name_end_index - 1));
                        type = args[name_end_index + 2];
                        params1 = args.Skip(name_end_index + 3).ToArray();
                    }
                    if (type == "check")
                    {
                        GroupBox groupBox = new GroupBox();
                        groupBox.Width = 200;
                        groupBox.Height = 55;
                        groupBox.Text = "";
                        string def = params1[1];
                        if (CurrentEngineSettings.Configs.ContainsKey(name))
                        {
                            def = CurrentEngineSettings.Configs[name];
                        }
                        CheckBox checkBox = new CheckBox();
                        checkBox.Top = 20;
                        checkBox.Left = 15;
                        checkBox.Width = 180;
                        checkBox.Text = name;
                        checkBox.Name = "checkBox_" + name;
                        checkBox.Checked = def == "true";
                        checkBox.CheckedChanged += new EventHandler(((object sender1, EventArgs e1) =>
                        {
                            CurrentEngineSettings.Configs[name] = checkBox.Checked ? "true" : "false";
                            Engine.SetOption(name, checkBox.Checked ? "true" : "false");
                            Mainform.SaveSettings();
                        }));
                        groupBox.Controls.Add(checkBox);
                        flowLayoutPanel.Controls.Add(groupBox);
                    }
                    else if (type == "spin")
                    {
                        GroupBox groupBox = new GroupBox();
                        groupBox.Width = 200;
                        groupBox.Height = 55;
                        groupBox.Text = name;
                        string def = params1[1];
                        if (CurrentEngineSettings.Configs.ContainsKey(name))
                        {
                            def = CurrentEngineSettings.Configs[name];
                        }                        
                        string min = params1[3];
                        string max = params1[5];
                        NumericUpDown numericUpDown = new NumericUpDown();
                        numericUpDown.Top = 20;
                        numericUpDown.Left = 15;
                        numericUpDown.Minimum = int.Parse(min);
                        numericUpDown.Maximum = int.Parse(max);
                        numericUpDown.Value = (decimal)double.Parse(def);
                        numericUpDown.Name = "numericUpDown_" + name;
                        numericUpDown.ValueChanged += new EventHandler(((object sender1, EventArgs e1) =>
                        {
                            CurrentEngineSettings.Configs[name] = numericUpDown.Value.ToString();
                            Engine.SetOption(name, numericUpDown.Value.ToString());
                            Mainform.SaveSettings();
                        }));
                        groupBox.Controls.Add(numericUpDown);
                        this.flowLayoutPanel.Controls.Add(groupBox);
                    }
                    else if (type == "combo")
                    {
                        GroupBox groupBox = new GroupBox();
                        groupBox.Width = 200;
                        groupBox.Height = 55;
                        groupBox.Text = name;
                        string def = params1[1];
                        if (CurrentEngineSettings.Configs.ContainsKey(name))
                        {
                            def = CurrentEngineSettings.Configs[name];
                        }
                        string[] options = params1.Skip(2).ToArray();
                        List<string> valieOptions = new List<string>();
                        for (int i = 1; i < options.Length; i += 2)
                        {
                            valieOptions.Add(options[i]);
                        }
                        ComboBox comboBox = new ComboBox();
                        comboBox.Top = 20;
                        comboBox.Left = 15;
                        comboBox.Name = "comboBox_" + name;
                        comboBox.Items.AddRange(valieOptions.ToArray());
                        comboBox.SelectedIndex = Array.IndexOf(valieOptions.ToArray(), def);
                        comboBox.SelectedIndexChanged += new EventHandler(((object sender1, EventArgs e1) =>
                        {
                            CurrentEngineSettings.Configs[name] = comboBox.SelectedItem.ToString();
                            Engine.SetOption(name, comboBox.SelectedItem.ToString());
                            Mainform.SaveSettings();
                        }));
                        groupBox.Controls.Add(comboBox);
                        this.flowLayoutPanel.Controls.Add(groupBox);
                    }
                    else if (type == "button")
                    {
                        GroupBox groupBox = new GroupBox();
                        groupBox.Width = 200;
                        groupBox.Height = 55;
                        groupBox.Text = "";
                        Button button = new Button();
                        button.Top = 20;
                        button.Left = 15;
                        button.Text = name;
                        button.Name = "button_" + name;
                        button.Width = 120;
                        button.Click += new EventHandler(((object sender1, EventArgs e1) =>
                        {
                            CurrentEngineSettings.Configs[name] = "";
                            Engine.SetOption(name, "");
                            Mainform.SaveSettings();
                        }));
                        groupBox.Controls.Add(button);
                        this.flowLayoutPanel.Controls.Add(groupBox);
                    }
                }
            }
        }

        private void EngineSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
