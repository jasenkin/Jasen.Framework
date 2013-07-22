using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Jasen.Framework.SchemaProvider;
using System.IO;

namespace Jasen.Framework.CodeGenerator
{
    public delegate void SaveCompletedEventHanlder(IDatabaseProvider config);
 
    public partial class ConnectionForm : Form
    {
        public event SaveCompletedEventHanlder SaveCompleted;

        public ConnectionForm()
        {
            InitializeComponent();
            this.btnCancel.Click += this.OnCancelClick;
            this.btnSave.Click += this.OnSaveClick;
            this.Load += this.OnFormLoad;
            this.cmbDataSources.SelectedIndexChanged += this.OnDataSourceSelectedIndexChanged;
        }

        private void OnDataSourceSelectedIndexChanged(object sender, EventArgs e)
        {
            var directory = this.cmbDataSources.SelectedItem as DataSource;

            if (directory == null)
            {
                return;
            }

            this.txtConnection .Text = directory.ConnectionString; 
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            LoadProviderPlugins();
            BindSources();
        }

        private void LoadProviderPlugins()
        {
            var providers = PluginManager.GetSchemaProviderInfos(SystemConfig.PluginPath);
            this.cmbProviders.DisplayMember = "Name";

            foreach (var provider in providers)
            {
                this.cmbProviders.Items.Add(provider);
            }

            if (this.cmbProviders.Items.Count > 0)
            {
                this.cmbProviders.SelectedIndex = 0;
            }
        }

        private void BindSources()
        {
            DataSourceSetting setting = DataSourceSetting.Deserialize(SystemConfig.SourceFilePath);

            if (setting == null || setting.DataSources == null || setting.DataSources.Count == 0)
            {
                return;
            }

            foreach (var dataSource in setting.DataSources)
            {
                 int index = this.cmbDataSources.Items.Add(dataSource);

                if (string.Equals(dataSource.Name, setting.SelectedDataSource))
                {
                    this.cmbDataSources.SelectedIndex = index;
                } 

            }

            this.cmbDataSources.DisplayMember = "Name";
            this.cmbDataSources.ValueMember = "ConnectionString";

            if (this.cmbDataSources.Items.Count > 0&&this.cmbDataSources.SelectedIndex<0)
            {
                this.cmbDataSources.SelectedIndex = 0;
            }
        }

        private void OnSaveClick(object sender, EventArgs e)
        {
            if (this.cmbProviders.Items.Count == 0 || this.cmbProviders.SelectedItem == null)
            {
                MessageBox.Show("请选择相关提供者插件。");
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txtConnection.Text))
            {
                MessageBox.Show("请输入连接字符串。");
                return;
            }

            var providerInfo = this.cmbProviders.SelectedItem as ProviderInfo;
            bool connectSuccess = CheckConnection(ref providerInfo);

            if (!connectSuccess)
            {
                return;
            }
            this.Close();

            if (this.SaveCompleted != null)
            {
                this.SaveCompleted(providerInfo.Provider);

            }

            SaveSetting();
        }

        private bool CheckConnection(ref ProviderInfo providerInfo)
        {
            bool connectSuccess = false;

            try
            {
                providerInfo.Provider.Database.ConnectionString = this.txtConnection.Text;
                connectSuccess = providerInfo.Provider.Database.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接不成功：" + ex.Message);
                providerInfo = null;
                connectSuccess = false;
            }
            finally
            {
                if (providerInfo != null && connectSuccess)
                {
                    providerInfo.Provider.Database.Close();
                }
            }

            return connectSuccess;
        }

        private void SaveSetting()
        {
            DataSourceSetting setting = DataSourceSetting.Deserialize(SystemConfig.SourceFilePath) ?? new DataSourceSetting();

            for (int i = 0; i < setting.DataSources.Count; i++)
            {
                if (string.Equals(setting.DataSources[i].Name, this.cmbDataSources.Text))
                {
                    setting.SelectedDataSource = this.cmbDataSources.Text;
                    setting.DataSources[i].ConnectionString = this.txtConnection.Text;
                    DataSourceSetting.Serialize(SystemConfig.SourceFilePath, setting);
                    return;
                }
            }

            DataSource dataSource = new DataSource();
            dataSource.Name = this.cmbDataSources.Text;
            dataSource.ConnectionString = this.txtConnection.Text;
            setting.SelectedDataSource = dataSource.Name;
            setting.DataSources.Add(dataSource);

            DataSourceSetting.Serialize(SystemConfig.SourceFilePath, setting);
        }
 
        private void OnCancelClick(object sender, EventArgs e)
        {
            this.Close();
        }  
         
    }
}
