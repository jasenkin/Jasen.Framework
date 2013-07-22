using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Jasen.Framework.CodeGenerator
{
    public partial class SystemSettingForm : Form
    {
        public SystemSettingForm()
        {
            InitializeComponent();
            this.Load += OnSystemSettingFormLoad;
            this.btnEdit.Click += OnEditClick;
            this.btnSelectDir.Click += OnSelectDirClick;
        }

        private void OnSelectDirClick(object sender, EventArgs e)
        {
            var browserDialog=new FolderBrowserDialog();
            browserDialog.RootFolder = Environment.SpecialFolder.DesktopDirectory;
            if (browserDialog.ShowDialog() == DialogResult.OK)
            {
                this.txtOutputDir.Text = browserDialog.SelectedPath;   
            }
        }

        private void OnEditClick(object sender, EventArgs e)
        {
            if(this.cmbTemplates.Items.Count==0 ||this.cmbTemplates.SelectedIndex<0)
            {
                MessageBox.Show("请选择有效地模板!", "Edit Template...", MessageBoxButtons.OK, MessageBoxIcon.Error);
              
                return;
            }

            string filePath = SystemConfig.TemplateFilePath + "\\"+ this.cmbTemplates.Text;

            var settingForm =new TemplateSettingForm(filePath);
            settingForm.ShowDialog();
        }

        private void OnSystemSettingFormLoad(object sender, EventArgs e)
        {
            CreateSystemDirectory();
            LoadTemplate();
            
            SystemSetting setting = SystemSetting.Deserialize(SystemConfig.SettingFilePath);
            if(setting==null)
            {
                return;
            }

            this.textBox1.Text = setting.DefaultNameSpace;
            this.txtOutputDir.Text = setting.OutputDir;
            this.ckbAddAttribute.Checked = setting.AddAttribute;
            
            if(this.cmbTemplates.Items.Count>0)
            {
                this.cmbTemplates.SelectedText = setting.SelectedTemplate;
            }
        }

        private void CreateSystemDirectory()
        {
            if(!Directory.Exists(SystemConfig.TemplateFilePath))
            {
                Directory.CreateDirectory(SystemConfig.TemplateFilePath);
            }

            if (!Directory.Exists(SystemConfig.OutputFilePath))
            {
                Directory.CreateDirectory(SystemConfig.OutputFilePath);
            }

            if (!Directory.Exists(SystemConfig.PluginPath))
            {
                Directory.CreateDirectory(SystemConfig.PluginPath);
            }
        }

        private void LoadTemplate()
        {
            var directoryInfo =new DirectoryInfo(SystemConfig.TemplateFilePath);

            FileInfo[] filePaths = directoryInfo.GetFiles();

            foreach (var filePath in filePaths)
            {
                this.cmbTemplates.Items.Add(filePath.Name);
            }

            if (this.cmbTemplates.Items.Count > 0)
            {
                this.cmbTemplates.SelectedIndex = 0; 
            }
 
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.cmbTemplates.Text))
            {
                MessageBox.Show("请选择有效的模板!", "系统设置...", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            if(string.IsNullOrWhiteSpace(this.textBox1.Text))
            {
                MessageBox.Show("请输入命名空间!", "系统设置...", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            if (!Directory.Exists(this.txtOutputDir.Text))
            {
                MessageBox.Show("请选择有效的文件输出路径!", "系统设置...", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            var setting =new SystemSetting();
            setting.AddAttribute = this.ckbAddAttribute.Checked;
            setting.DefaultNameSpace = this.textBox1.Text.Trim();
            setting.OutputDir = this.txtOutputDir.Text;
            setting.SelectedTemplate = this.cmbTemplates.Text;

            SystemSetting.Serialize(SystemConfig.SettingFilePath, setting);

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
