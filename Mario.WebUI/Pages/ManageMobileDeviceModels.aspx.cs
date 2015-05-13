using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Ext.Net;
using Mario.Business;
using Mario.DataAccess;

namespace Mario.WebUI.Pages
{
    public partial class ManageMobileDeviceModels : BasePage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!X.IsAjaxRequest)
            {
                bindGridMobileDeviceModelsList();
            }
        }

        #region Grid相关事件

        private void bindGridMobileDeviceModelsList()
        {
            MobileDeviceModelsManager manager = new MobileDeviceModelsManager();
            this.GridMobileDeviceModels.GetStore().DataSource = manager.SelectAllEntities();
            this.GridMobileDeviceModels.GetStore().DataBind();
        }

        /// <summary>
        /// 点击选择某一个项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grid_RowSelected(object sender, DirectEventArgs e)
        {
            RowSelectionModel sm = this.GridMobileDeviceModels.GetSelectionModel() as RowSelectionModel;
            if (sm.SelectedRows.Count == 0)
            {
                this.btnUpdate.Disable();
                this.btnDelete.Disable();
            }
            else
            {
                this.btnUpdate.Enable();
                this.btnDelete.Enable();
            }
        }

        /// <summary>
        /// 绑定实体到子窗体
        /// </summary>
        /// <param name="id"></param>
        private void bindMobileDeviceModels(int id)
        {
            this.txtID.Text = id.ToString();
            if (id == 0)
            {
                this.txtBrand.Text = string.Empty;
                this.txtDevice.Text = string.Empty;
                this.txtWidth.Text = "600";
                this.txtHeight.Text = "800";
                this.txtOSVersion.Text = string.Empty;
                this.txtWeight.Text = "1";
                subWindow.Title = "新增";
            }
            else
            {
                MobileDeviceModelsManager manager = new MobileDeviceModelsManager();
                MobileDeviceModels model = manager.SelectSingleEntity(id);
                this.txtBrand.Text = model.Brand;
                this.txtDevice.Text = model.Device;
                this.txtWidth.Text = model.Width.ToString();
                this.txtHeight.Text = model.Height.ToString();
                this.txtOSVersion.Text = model.OSVersion;
                this.txtWeight.Text = model.Weight.ToString();
                subWindow.Title = "修改";
            }
            subWindow.Center();
            subWindow.Show();
        }

        protected void btnAdd_Click(object sender, DirectEventArgs e)
        {
            this.bindMobileDeviceModels(0);
        }

        protected void btnUpdate_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.GridMobileDeviceModels.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int id = int.Parse(rsm.SelectedRow.RecordID);
                this.bindMobileDeviceModels(id);
            }
        }

        protected void btnDelete_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.GridMobileDeviceModels.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int id = int.Parse(rsm.SelectedRow.RecordID);
                MobileDeviceModelsManager manager = new MobileDeviceModelsManager();
                int result = manager.Delete(id);
                if (result == 0)
                {
                    X.Msg.Alert("系统提示", "删除失败!").Show();
                }
                else
                {
                    X.Msg.Info("系统提示", "删除成功!", UI.Success).Show();
                }
            }
            else
            {
                X.Msg.Alert("系统提示", "没有选中要操作的项").Show();
            }
            this.bindGridMobileDeviceModelsList();
        }
        protected void btnBuildWeightRandom_Click(object sender, DirectEventArgs e)
        {
            MobileDeviceModelsManager manager = new MobileDeviceModelsManager();
            manager.ComputeWeight();
            X.Msg.Info("系统提示", "计算完成!", UI.Success).Show();
        }

        #endregion

        #region 子Windows事件

        protected void btnWinSave_Click(object sender, DirectEventArgs e)
        {
            int id = int.Parse(this.txtID.Text);

            MobileDeviceModels model = null;
            MobileDeviceModelsManager manager = new MobileDeviceModelsManager();
            if (id == 0)
            {
                model = new MobileDeviceModels();
                model.ID = id;
            }
            else
            {
                model = manager.SelectSingleEntity(id);
            }

            model.Brand = this.txtBrand.Text;
            model.Device = this.txtDevice.Text;
            model.Width = int.Parse(this.txtWidth.Text);
            model.Height = int.Parse(this.txtHeight.Text);
            model.OSVersion = this.txtOSVersion.Text;
            model.Weight = int.Parse(this.txtWeight.Text);

            int result = manager.AddOrUpdate(model);

            if (result == 0)
            {
                X.Msg.Alert("系统提示", "保存失败!当前已存在同一型号的移动设备.").Show();
            }
            else
            {
                X.Msg.Info("系统提示", "保存成功!", UI.Success).Show();
            }

            subWindow.Hide();
            this.bindGridMobileDeviceModelsList();
        }

        #endregion
    }
}