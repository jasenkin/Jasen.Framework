using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms; 
using Jasen.Framework.MetaData;
using Jasen.Framework.SchemaProvider; 

namespace Jasen.Framework.CodeGenerator
{
    public partial class MainForm : Form
    {
        private IDatabaseProvider _provider;

        public MainForm()
        {
            InitializeComponent(); 
            this.tvwMetaData.NodeMouseDoubleClick += OnTreeViewNodeMouseDoubleClick;
            this.tsBtnConnect.Click += OnConnectClick; 
            this.tsBtnSystemSeeting.Click += OnSystemSeetingClick;
        }

        private void OnSystemSeetingClick(object sender, EventArgs e)
        {
            var connectionForm = new SystemSettingForm();
            connectionForm.ShowDialog();
        } 

        private void OnConnectionFormSaveCompleted(IDatabaseProvider provider)
        {
            this._provider = provider;

            if (this._provider == null)
            {
                return;
            }

            this.tvwMetaData.BeginInvoke(new Action(InitData)); 
        }

        private void InitData()
        {
            this._provider.Init();

            this.tvwMetaData.Nodes.Clear();
            AddTreeNode("Tables", OperationType.Table, this._provider.TableNames);
            AddTreeNode("Views", OperationType.View, this._provider.ViewNames);
            AddTreeNode("Procedures", OperationType.Procedure, this._provider.ProcedureNames);
            this.tvwMetaData.ExpandAll();
        }

        private void AddTreeNode(string parentNodeName, OperationType operationType ,IEnumerable<string> nodeNames)
        {
            var parentTreeNode = new TreeNode(parentNodeName);
            TreeNode treeNode;

            if (nodeNames != null && nodeNames.Count() > 0)
            {
                foreach (string name in nodeNames)
                {
                    treeNode = new TreeNode(name);
                    treeNode.Tag = new OperationNode(name, operationType);
                    parentTreeNode.Nodes.Add(treeNode);
                }
            }

            this.tvwMetaData.Nodes.Add(parentTreeNode);
        }

        private void OnConnectClick(object sender, EventArgs e)
        { 
            var connectionForm = new ConnectionForm();
            connectionForm.SaveCompleted += this.OnConnectionFormSaveCompleted;
            connectionForm.ShowDialog();
        }

        private void OnTreeViewNodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var operationNode =  e.Node.Tag as OperationNode;

            if (operationNode == null)
            {
                return;
            }

            GenerateFileContent(operationNode.Name, operationNode.OperationType);
        }

        private void GenerateFileContent(string name ,OperationType operationType,bool generateFile = false,bool openFile =false)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            string templateFilePath;
            SystemSetting setting;
            string error;
            if (!CheckSystemSetting(out setting, out templateFilePath, out error))
            {
                MessageBox.Show(error, "系统配置...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            GenerateFile(name, operationType, setting, templateFilePath, generateFile, openFile);
        }

        private void GenerateFile(string name, OperationType operationType, SystemSetting setting,
            string templateFilePath, bool generateFile,bool openFile= false)
        {
            string content = this._provider.Build(name.Trim(), operationType,
                                                  setting.DefaultNameSpace,setting.AddAttribute,templateFilePath);
            this.rtxtCode.Text = content;

            if(generateFile)
            {
                string outputDir = setting.OutputDir;

                if(!Directory.Exists(setting.OutputDir))
                {
                    Directory.CreateDirectory(SystemConfig.OutputFilePath);
                    outputDir = SystemConfig.OutputFilePath;
                    SystemSetting.Serialize(SystemConfig.SettingFilePath, setting);
                }

                string outputFileName = outputDir + "\\" + name.Trim() + ".cs";
                File.WriteAllText(outputFileName, content.Replace("\n","\r\n"),Encoding.GetEncoding("gb2312"));
                Process.Start(outputFileName);
            }
        }

        private bool CheckSystemSetting(out SystemSetting setting , out string templateFilePath ,out string error)
        {
            setting = SystemSetting.Deserialize(SystemConfig.SettingFilePath);
            templateFilePath = null;
            error = string.Empty;

            if(setting==null)
            {
                error += "不存在相关系统设置，请设置系统配置!";
                return false;
            }

            templateFilePath = SystemConfig.TemplateFilePath + setting.SelectedTemplate;

            if (!File.Exists(templateFilePath))
            {
                error += "不存在有效的模板文件，请设置系统配置模板文件!";
                return false;
               
            }

            return true;
        }

        private void tsBtnLookUp_Click(object sender, EventArgs e)
        {
            if (this.tvwMetaData.SelectedNode == null)
            {
                return;
            }

            var operationNode = this.tvwMetaData.SelectedNode.Tag as OperationNode;
            if (operationNode == null)
            {
                return;
            }

            InfoForm infoForm = new InfoForm();

            if (operationNode.OperationType == OperationType.Procedure)
            {
                var info = this._provider.GetProcedureParameters(operationNode.Name);
                infoForm.InitProcedureInfo(info);
            }

            else
            {
                var info = this._provider.GetTableOrViewInfo(operationNode.Name);
                infoForm.InitTableOrViewInfo(info);
            } 

            infoForm.ShowDialog();
        }

        private void tsBtnGenerateFile_Click(object sender, EventArgs e)
        {
            if (this._provider == null)
            {
                return;
            }

            var operationNode = this.tvwMetaData.SelectedNode.Tag as OperationNode;
            if (operationNode == null)
            {
                return;
            }

            GenerateFileContent(operationNode.Name, operationNode.OperationType, true, true);
        }

        private void tsBtnGenerateAllFile_Click(object sender, EventArgs e)
        {
            if (this._provider == null)
            {
                return;
            }

            string templateFilePath;
            SystemSetting setting;
            string error;
            if (!CheckSystemSetting(out setting, out templateFilePath, out error))
            {
                MessageBox.Show(error, "系统配置...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            GenerateFile(this._provider.GetTableNames(), OperationType.Table, setting, templateFilePath);
            GenerateFile(this._provider.ViewNames, OperationType.View, setting, templateFilePath);
            GenerateFile(this._provider.ProcedureNames, OperationType.Procedure, setting, templateFilePath);
        }

        private void GenerateFile(IList<string> names, OperationType operationType, SystemSetting setting, string templateFilePath)
        {
            if(names==null||names.Count==0)
            {
                return;
            }

            foreach(var name in names)
            {
                GenerateFile(name, operationType, setting, templateFilePath, true);
            }
        }

    }

     
}
