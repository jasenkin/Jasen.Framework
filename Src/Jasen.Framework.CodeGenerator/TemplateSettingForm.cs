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
    public partial class TemplateSettingForm : Form
    {
        private readonly string _filePath;

        public TemplateSettingForm(string fileName)
        {
            InitializeComponent(); 
            LoadTemplate(fileName);

            this._filePath = fileName;
            this.btnSave.Click += new EventHandler(btnSave_Click);
            this.btnCancel.Click += new EventHandler(btnCancel_Click);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            File.WriteAllText(this._filePath, this.rtxtContent.Text, Encoding.GetEncoding("gb2312"));
        }

        private void LoadTemplate(string filePath)
        {
            if(!File.Exists(filePath))
            {
                File.Create(filePath);
            }

            int lastIndex = filePath.LastIndexOf(@"\");
            if (lastIndex > 0 && filePath.Length > lastIndex + 1)
            {
                this.txtTemplateName.Text = filePath.Substring(lastIndex + 1);
            }

            this.rtxtContent.Text = File.ReadAllText(filePath, Encoding.GetEncoding("gb2312"));
        }
    }
}
