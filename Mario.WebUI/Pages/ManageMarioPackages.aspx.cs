using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Ext.Net;
using Mario.Business;
using Mario.DataAccess;
using System.IO;
using System.Configuration;

namespace Mario.WebUI.Pages
{
    public partial class ManageMarioPackages : BasePage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!X.IsAjaxRequest)
            {
                bindGridMarioPackagesList();
            }
        }

        #region Grid相关事件

        private void bindGridMarioPackagesList()
        {
            MarioPackagesManager manager = new MarioPackagesManager();
            this.GridMarioPackages.GetStore().DataSource = manager.SelectAllMarioPackages();
            this.GridMarioPackages.GetStore().DataBind();
        }

        /// <summary>
        /// 点击选择某一个项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grid_RowSelected(object sender, DirectEventArgs e)
        {
            RowSelectionModel sm = this.GridMarioPackages.GetSelectionModel() as RowSelectionModel;
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
        private void bindMarioPackage(int id)
        {
            this.txtID.Text = id.ToString();
            if (id == 0)
            {
                this.txtVersion.Text = "1";
                this.txtDownloadUrl.Text = ConfigurationManager.AppSettings["ServerHostUrl"] + "/UpdatePackage/      .apk";
                this.selectBoxPlatform.Select("1");
                subWindow.Title = "新增";
            }
            else
            {
                MarioPackagesManager manager = new MarioPackagesManager();
                MarioPackages pack = manager.SelectSingleMarioPackages(id);
                this.txtVersion.Text = pack.Version.ToString();
                this.txtDownloadUrl.Text = pack.DownloadUrl;
                this.selectBoxPlatform.Select(pack.Platform.ToString());
                subWindow.Title = "修改";
            }
            subWindow.Center();
            subWindow.Show();
        }

        protected void btnAdd_Click(object sender, DirectEventArgs e)
        {
            this.bindMarioPackage(0);
        }

        protected void btnUpdate_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.GridMarioPackages.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int id = int.Parse(rsm.SelectedRow.RecordID);
                this.bindMarioPackage(id);
            }
        }

        protected void btnDelete_Click(object sender, DirectEventArgs e)
        {
            RowSelectionModel rsm = this.GridMarioPackages.GetSelectionModel() as RowSelectionModel;
            if (rsm.SelectedRows.Count == 1)
            {
                int id = int.Parse(rsm.SelectedRow.RecordID);
                MarioPackagesManager manager = new MarioPackagesManager();
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
            this.bindGridMarioPackagesList();
        }

        protected void fileUploadMario_Unload(object sender, DirectEventArgs e)
        {
            if (this.fileUploadMario.HasFile)
            {
                int fileSize = Int32.Parse(this.fileUploadMario.PostedFile.ContentLength.ToString());
                if (fileSize > 10 * 1024 * 1024) 
                {
                    X.Msg.Alert("提示信息", "上传文件过大！要上传的文件不能超过10M.").Show();
                    return;
                }
                string filename = Path.GetFileName(this.fileUploadMario.PostedFile.FileName);
                string extensionName = Path.GetExtension(filename).ToLower();//获取文件后缀
                if (extensionName != ".apk" && extensionName != ".deb" && extensionName != ".ipa" && extensionName != ".xap")
                {
                    X.Msg.Alert("提示信息", "文件格式不正确！只能为APK等移动设备安装包格式！").Show();
                    return;
                }

                string remoteFileNamePath = Server.MapPath("../UpdatePackage/" + filename);
                this.fileUploadMario.PostedFile.SaveAs(remoteFileNamePath);
                string downloadUrl = ConfigurationManager.AppSettings["ServerHostUrl"] + "/UpdatePackage/" + filename;

                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.INFO,
                    Title = "上传成功!",
                    Message = string.Format("下载地址为: {0}<br/> 文件大小: {1}KB", downloadUrl, this.fileUploadMario.PostedFile.ContentLength / 1024)
                });
            }
            else
            {
                X.Msg.Show(new MessageBoxConfig
                {
                    Buttons = MessageBox.Button.OK,
                    Icon = MessageBox.Icon.ERROR,
                    Title = "上传失败",
                    Message = "没有需要上传的文件."
                });
            }
        }
        
        #endregion

        #region 子Windows事件

        protected void btnWinSave_Click(object sender, DirectEventArgs e)
        {
            int id = int.Parse(this.txtID.Text);

            MarioPackages pack = null;
            MarioPackagesManager manager = new MarioPackagesManager();
            if (id == 0)
            {
                pack = new MarioPackages();
                pack.ID = id;
            }
            else
            {
                pack = manager.SelectSingleMarioPackages(id);
            }

            pack.Version = int.Parse(this.txtVersion.Text);
            pack.DownloadUrl = this.txtDownloadUrl.Text;
            pack.Platform = int.Parse(this.selectBoxPlatform.SelectedItem.Value);

            int result = manager.AddOrUpdate(pack);
            if (result == 0)
            {
                X.Msg.Alert("系统提示", "保存失败!").Show(); 
            }
            else
            {
                X.Msg.Info("系统提示", "保存成功!", UI.Success).Show();
                this.AddSystemLog(string.Format("新增或修改了Mario包，版本号为{0}，下载地址为：<a href='{1}'>{1}</a>", pack.Version.ToString(), pack.DownloadUrl));
            }

            subWindow.Hide();
            this.bindGridMarioPackagesList();
        }

        #endregion

        
    }
}