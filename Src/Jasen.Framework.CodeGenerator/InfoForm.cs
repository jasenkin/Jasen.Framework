using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Jasen.Framework.MetaData;

namespace Jasen.Framework.CodeGenerator
{
    public partial class InfoForm : Form
    {
        public InfoForm()
        {
            InitializeComponent();
        }

        internal void InitTableOrViewInfo(IEnumerable<TableColumn> info)
        {

            this.dgvInfo.DataSource = null;
            this.dgvInfo.Columns.Clear();
            this.dgvInfo.Rows.Clear();
            this.dgvInfo.DataSource = info;
           
        }

        internal void InitProcedureInfo(IEnumerable<ProcedureParameter> info)
        {
            var re = info.ToList();
            this.dgvInfo.DataSource = null;
            this.dgvInfo.Rows.Clear();
            this.dgvInfo.Columns.Clear();
            this.dgvInfo.DataSource = info;

        }
    }
}
