using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using oms.DataAccessLayer;
using oms.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.Repository;

namespace oms.Controls
{
    public partial class List_Basic_UC : UserControl, IListUserControl
    {
        public List_Basic_UC()
        {
            InitializeComponent();
        }

        private DataTable dataTable;
        private GridControl gridControl;
        private GridView gridView;
        private List<BasicModel> ActiveList;
        private string tableName = string.Empty;

        private void LoadGrid()
        {
            this.dataTable = new DataTable();
            this.dataTable.Columns.Add("Id", typeof(int));
            this.dataTable.Columns.Add("Name", typeof(string));
            this.dataTable.Columns.Add("Description", typeof(string));
            this.dataTable.Columns.Add("Active", typeof(bool));
            this.dataTable.Columns.Add("Per Unit Fees", typeof(float));
            this.dataTable.Columns.Add("Flat Fees", typeof(float));


            this.gridControl = new GridControl();
            this.Controls.Add(this.gridControl);
            this.gridControl.DataSource = this.dataTable;
            this.gridControl.Dock = DockStyle.Fill;
            this.gridControl.Visible = true;

            this.gridView = new GridView();
            this.gridControl.MainView = this.gridView;
            BaseView[] views = new BaseView[] { this.gridView };
            this.gridControl.ViewCollection.AddRange(views);

            this.gridView.GridControl = this.gridControl;
            this.gridView.PopulateColumns();

            RepositoryItemTextEdit editLink0 = new RepositoryItemTextEdit();
            this.gridView.Columns["Per Unit Fees"].ColumnEdit = editLink0;
            editLink0.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            editLink0.DisplayFormat.FormatString = "c2";

            RepositoryItemTextEdit editLink1 = new RepositoryItemTextEdit();
            this.gridView.Columns["Flat Fees"].ColumnEdit = editLink1;
            editLink1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            editLink1.DisplayFormat.FormatString = "c2";

            this.gridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
            this.gridView.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
            this.gridView.Columns["Id"].Visible = false;
        }

        private void LoadData()
        {
            this.ActiveList = ListDL.Get(this.tableName);
            this.dataTable.Rows.Clear();
            foreach (BasicModel data in this.ActiveList)
            {
                dataTable.Rows.Add(data.Id, data.Name, data.Description, data.Active, data.PerUnitFees, data.FlatFees);
            }

            this.gridControl.RefreshDataSource();
            this.gridControl.Refresh();
        }

        public void LoadControl(string tablename)
        {
            this.tableName = tablename;

            this.LoadGrid();
            this.LoadData();
        }

        public void SaveControl()
        {
            foreach (DataRow row in this.dataTable.Rows)
            {
                BasicModel item = this.ActiveList.FirstOrDefault(x => x.Id.Equals(CommonFunctions.GetIntSafely(row["Id"])));
                if (item == null)
                {
                    item = new BasicModel();
                    this.ActiveList.Add(item);
                }

                item.Id = CommonFunctions.GetIntSafely(row["Id"]);
                item.Name = CommonFunctions.GetStringSafely(row["Name"]);
                item.Description = CommonFunctions.GetStringSafely(row["Description"]);
                item.Active = CommonFunctions.GetBoolSafely(row["Active"]);
                item.PerUnitFees = CommonFunctions.GetFloatSafely(row["Per Unit Fees"]);
                item.FlatFees = CommonFunctions.GetFloatSafely(row["Flat Fees"]);
            }

            int result = ListDL.AddOrUpdate(tableName, this.ActiveList);

            if (result > 0)
                CommonFunctions.ShowInfomationMessage("Update Completed !!!");

            this.LoadData();
        }

        public void RefreshControl()
        {
            throw new NotImplementedException();
        }
    }
}
