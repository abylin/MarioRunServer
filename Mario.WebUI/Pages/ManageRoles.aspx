<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageRoles.aspx.cs" Inherits="Mario.WebUI.Pages.ManageRoles" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>管理角色</title>
    <script>
        var twoGridSelector = {
            add: function (source, destination) {
                source = source;
                destination = destination;

                if (source.selModel.hasSelection()) {
                    var records = source.selModel.getSelection();
                    source.store.remove(records);
                    destination.store.add(records);
                }
            },
            addAll: function (source, destination) {
                source = source;
                destination = destination;
                var records = source.store.getRange();
                source.store.removeAll();
                destination.store.add(records);
            },
            remove: function (source, destination) {
                this.add(destination, source);
            },
            removeAll: function (source, destination) {
                this.addAll(destination, source);
            }
        };
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Viewport runat="server" Layout="BorderLayout">
            <Items>
                <ext:Window runat="server" ID="subWindowRoles" Modal="true" Layout="FitLayout" Width="400" Height="250" ToFrontOnShow="true" Hidden="true" CloseAction="Hide">
                    <Items>
                        <ext:FormPanel runat="server" Layout="FormLayout" ID="submitPanelRole" Border="false">
                            <Items>
                                <ext:TextField ID="txtRoleID" FieldLabel="编号" runat="server" ReadOnly="true" />
                                <ext:TextField ID="txtRoleName" FieldLabel="角色名称" runat="server" AllowBlank="false" BlankText="角色名称不能为空" />
                                <ext:TextField ID="txtRoleMemo" FieldLabel="备注" runat="server" />
                            </Items>
                        </ext:FormPanel>
                    </Items>
                    <Buttons>
                        <ext:Button runat="server" Text="提交" Icon="DatabaseSave" ID="btnRoleSubmit">
                            <DirectEvents>
                                <Click OnEvent="btnRoleSubmit_Click" Before="return #{submitPanelRole}.getForm().isValid();">
                                    <EventMask ShowMask="true" Msg="正在提交数据，请稍候..."></EventMask>
                                </Click>
                            </DirectEvents>
                        </ext:Button>
                        <ext:Button runat="server" Text="取消" Icon="Cancel" ID="btnMobileDevicesCancel">
                            <Listeners>
                                <Click Handler="#{subWindowRoles}.hide();" />
                            </Listeners>
                        </ext:Button>
                    </Buttons>
                </ext:Window>

                <ext:GridPanel ID="gridRoles" runat="server" Title="角色列表" MarginSpec="0 0 5 5" Icon="Layout" Region="West"
                    Width="380" AutoScroll="true">
                    <TopBar>
                        <ext:Toolbar ID="toolbar1" runat="server" AutoScroll="true">
                            <Items>
                                <ext:Button ID="btnRolesAdd" runat="server" Text="添加" Icon="Add">
                                    <DirectEvents>
                                        <Click OnEvent="btnRolesAdd_Click" />
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnRolesModify" runat="server" Text="修改" Disabled="true" Icon="DatabaseEdit">
                                    <DirectEvents>
                                        <Click OnEvent="btnRolesModify_Click" />
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnRolesDelete" runat="server"  Text="删除"  Disabled="true" Icon="Delete">
                                    <DirectEvents>
                                        <Click OnEvent="btnRolesDelete_Click">
                                            <Confirmation ConfirmRequest="true" Title="系统提示" Message="您确定要删除选中的项吗？" />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnRoleConfigUser" runat="server" Text="配置用户"  Disabled="true" Icon="UserKey">
                                    <DirectEvents>
                                        <Click OnEvent="btnRoleConfigUser_Click" />
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Store>
                        <ext:Store ID="storeRoles" runat="server" PageSize="10" RemotePaging="false">
                            <Model>
                                <ext:Model runat="server" IDProperty="ID">
                                    <Fields>
                                        <ext:ModelField Name="ID" />
                                        <ext:ModelField Name="Name" />
                                        <ext:ModelField Name="Memo" />
                                    </Fields>
                                </ext:Model>
                            </Model>
                        </ext:Store>
                    </Store>
                    <ColumnModel runat="server">
                        <Columns>
                            <ext:Column runat="server" DataIndex="ID" Text="编号" />
                            <ext:Column runat="server" DataIndex="Name" Text="名称" Flex="1" />
                            <ext:Column runat="server" DataIndex="Memo" Text="说明" />
                        </Columns>
                    </ColumnModel>
                    <SelectionModel>
                        <ext:RowSelectionModel runat="server" Mode="Single">
                            <DirectEvents>
                                <Select OnEvent="gridRoles_RowSelected">
                                    <EventMask ShowMask="true" Target="CustomTarget" CustomTarget="#{gridAppProjectsInRoles}" />
                                </Select>
                            </DirectEvents>
                        </ext:RowSelectionModel>
                    </SelectionModel>
                    <BottomBar>
                        <ext:PagingToolbar runat="server" HideRefresh="true" />
                    </BottomBar>
                </ext:GridPanel>

                <ext:Window ID="subWindowUserSelector" runat="server" Modal="true" Closable="false" Height="520" Width="700" Icon="WorldAdd"
                    Title="配置角色里的用户" BodyPadding="5" BodyBorder="0" Layout="HBoxLayout" Hidden="true">
                    <LayoutConfig>
                        <ext:HBoxLayoutConfig Align="Stretch" />
                    </LayoutConfig>
                    <Items>
                        <ext:GridPanel runat="server" ID="GridPanelUserLeft" EnableDragDrop="true" Title="可选的用户" Flex="1" AutoScroll="true">
                            <Store>
                                <ext:Store ID="Store3" runat="server">
                                    <Model>
                                        <ext:Model runat="server" IDProperty="ID">
                                            <Fields>
                                                <ext:ModelField Name="ID" Type="Int" />
                                                <ext:ModelField Name="UserName" Type="String" />
                                                <ext:ModelField Name="ChineseName" Type="String" />
                                            </Fields>
                                        </ext:Model>
                                    </Model>
                                </ext:Store>
                            </Store>
                            <ColumnModel runat="server">
                                <Columns>
                                    <ext:Column runat="server" Text="中文名" DataIndex="ChineseName" Flex="1" />
                                    <ext:Column runat="server" Text="用户名" DataIndex="Name" Flex="1" />
                                </Columns>
                            </ColumnModel>
                            <SelectionModel>
                                <ext:RowSelectionModel runat="server" Mode="Multi" />
                            </SelectionModel>
                        </ext:GridPanel>
                        <ext:Panel ID="PanelUserCommand" runat="server" Width="35" BodyStyle="background-color: transparent;" Border="false" Layout="Anchor">
                            <Items>
                                <ext:Panel runat="server" Border="false" BodyStyle="background-color: transparent;" AnchorVertical="40%" />
                                <ext:Panel ID="Panel3" runat="server" Border="false" BodyStyle="background-color: transparent;" BodyPadding="5">
                                    <Items>
                                        <ext:Button runat="server" Icon="ResultsetNext" StyleSpec="margin-bottom:2px;">
                                            <Listeners>
                                                <Click Handler="twoGridSelector.add(#{GridPanelUserLeft},#{GridPanelUserRight});" />
                                            </Listeners>
                                            <ToolTips>
                                                <ext:ToolTip runat="server" Title="添加" Html="添加此用户到角色" />
                                            </ToolTips>
                                        </ext:Button>
                                        <ext:Button runat="server" Icon="ResultsetLast" StyleSpec="margin-bottom:2px;">
                                            <Listeners>
                                                <Click Handler="twoGridSelector.addAll(#{GridPanelUserLeft},#{GridPanelUserRight});" />
                                            </Listeners>
                                            <ToolTips>
                                                <ext:ToolTip runat="server" Title="添加全部" Html="添加全部用户到角色" />
                                            </ToolTips>
                                        </ext:Button>
                                        <ext:Button runat="server" Icon="ResultsetPrevious" StyleSpec="margin-bottom:2px;">
                                            <Listeners>
                                                <Click Handler="twoGridSelector.remove(#{GridPanelUserLeft},#{GridPanelUserRight});" />
                                            </Listeners>
                                            <ToolTips>
                                                <ext:ToolTip runat="server" Title="移除" Html="从角色移除此用户" />
                                            </ToolTips>
                                        </ext:Button>
                                        <ext:Button runat="server" Icon="ResultsetFirst" StyleSpec="margin-bottom:2px;">
                                            <Listeners>
                                                <Click Handler="twoGridSelector.removeAll(#{GridPanelUserLeft},#{GridPanelUserRight});" />
                                            </Listeners>
                                            <ToolTips>
                                                <ext:ToolTip runat="server" Title="移除全部" Html="从角色移除全部用户" />
                                            </ToolTips>
                                        </ext:Button>
                                    </Items>
                                </ext:Panel>
                            </Items>
                        </ext:Panel>
                        <ext:GridPanel runat="server" ID="GridPanelUserRight" Title="已选的用户" EnableDragDrop="false" Flex="1" AutoScroll="true">
                            <Store>
                                <ext:Store ID="Store4" runat="server" PageSize="15" RemotePaging="false" OnSubmitData="GridPanelUserRight_SubmitData">
                                    <Model>
                                        <ext:Model runat="server" IDProperty="ID">
                                            <Fields>
                                                <ext:ModelField Name="ID" Type="Int" />
                                                <ext:ModelField Name="UserName" Type="String" />
                                                <ext:ModelField Name="ChineseName" Type="String" />
                                            </Fields>
                                        </ext:Model>
                                    </Model>
                                </ext:Store>
                            </Store>
                            <ColumnModel runat="server">
                                <Columns>
                                    <ext:Column runat="server" Text="中文名" DataIndex="ChineseName" Flex="1" />
                                    <ext:Column runat="server" Text="用户名" DataIndex="UserName" Flex="1" />
                                </Columns>
                            </ColumnModel>
                            <SelectionModel>
                                <ext:RowSelectionModel runat="server" Mode="Multi" />
                            </SelectionModel>
                        </ext:GridPanel>
                    </Items>
                    <Buttons>
                        <ext:Button ID="Button1" runat="server" Text="保存设置" Icon="Disk">
                            <Listeners>
                                <Click Handler="#{GridPanelUserRight}.submitData();" />
                            </Listeners>
                        </ext:Button>
                        <ext:Button ID="Button2" runat="server" Text="取消" Icon="Cancel">
                            <Listeners>
                                <Click Handler="#{subWindowUserSelector}.hide();" />
                            </Listeners>
                        </ext:Button>
                    </Buttons>
                </ext:Window>

                <ext:Window ID="subWindowAppProjectsSelector" runat="server" Modal="true" Closable="false" Height="520" Width="700" Icon="WorldAdd"
                    Title="配置角色所对应的App权限。" BodyPadding="5" BodyBorder="0" Layout="HBoxLayout" Hidden="true">
                    <LayoutConfig>
                        <ext:HBoxLayoutConfig Align="Stretch" />
                    </LayoutConfig>
                    <Items>
                        <ext:GridPanel runat="server" ID="GridPanel1" EnableDragDrop="true" Title="可选的APP项目" Flex="1" AutoScroll="true">
                            <Store>
                                <ext:Store ID="Store1" runat="server">
                                    <Model>
                                        <ext:Model runat="server" IDProperty="ID">
                                            <Fields>
                                                <ext:ModelField Name="ID" Type="Int" />
                                                <ext:ModelField Name="ChineseName" Type="String" />
                                            </Fields>
                                        </ext:Model>
                                    </Model>
                                </ext:Store>
                            </Store>
                            <ColumnModel runat="server">
                                <Columns>
                                    <ext:Column runat="server" Text="编号" DataIndex="ID" />
                                    <ext:Column runat="server" Text="中文名称" DataIndex="ChineseName" Flex="1" />
                                </Columns>
                            </ColumnModel>
                            <SelectionModel>
                                <ext:RowSelectionModel runat="server" Mode="Multi" />
                            </SelectionModel>
                        </ext:GridPanel>
                        <ext:Panel ID="PanelCommand" runat="server" Width="35" BodyStyle="background-color: transparent;" Border="false" Layout="Anchor">
                            <Items>
                                <ext:Panel runat="server" Border="false" BodyStyle="background-color: transparent;" AnchorVertical="40%" />
                                <ext:Panel ID="Panel1" runat="server" Border="false" BodyStyle="background-color: transparent;" BodyPadding="5">
                                    <Items>
                                        <ext:Button runat="server" Icon="ResultsetNext" StyleSpec="margin-bottom:2px;">
                                            <Listeners>
                                                <Click Handler="twoGridSelector.add(#{GridPanel1},#{GridPanel2});" />
                                            </Listeners>
                                            <ToolTips>
                                                <ext:ToolTip runat="server" Title="添加" Html="配置此App项目权限" />
                                            </ToolTips>
                                        </ext:Button>
                                        <ext:Button runat="server" Icon="ResultsetLast" StyleSpec="margin-bottom:2px;">
                                            <Listeners>
                                                <Click Handler="twoGridSelector.addAll(#{GridPanel1},#{GridPanel2});" />
                                            </Listeners>
                                            <ToolTips>
                                                <ext:ToolTip runat="server" Title="添加全部" Html="配置全部App项目权限" />
                                            </ToolTips>
                                        </ext:Button>
                                        <ext:Button runat="server" Icon="ResultsetPrevious" StyleSpec="margin-bottom:2px;">
                                            <Listeners>
                                                <Click Handler="twoGridSelector.remove(#{GridPanel1},#{GridPanel2});" />
                                            </Listeners>
                                            <ToolTips>
                                                <ext:ToolTip runat="server" Title="移除" Html="移除此App项目权限" />
                                            </ToolTips>
                                        </ext:Button>
                                        <ext:Button runat="server" Icon="ResultsetFirst" StyleSpec="margin-bottom:2px;">
                                            <Listeners>
                                                <Click Handler="twoGridSelector.removeAll(#{GridPanel1},#{GridPanel2});" />
                                            </Listeners>
                                            <ToolTips>
                                                <ext:ToolTip runat="server" Title="移除全部" Html="移除全部App项目权限" />
                                            </ToolTips>
                                        </ext:Button>
                                    </Items>
                                </ext:Panel>
                            </Items>
                        </ext:Panel>
                        <ext:GridPanel runat="server" ID="GridPanel2" Title="已选的APP项目" EnableDragDrop="false" Flex="1" AutoScroll="true">
                            <Store>
                                <ext:Store ID="Store2" runat="server" PageSize="15" RemotePaging="false" OnSubmitData="SubmitSelectedAppProjectsData">
                                    <Model>
                                        <ext:Model runat="server" IDProperty="ID">
                                            <Fields>
                                                <ext:ModelField Name="ID" Type="Int" />
                                                <ext:ModelField Name="ChineseName" Type="String" />
                                            </Fields>
                                        </ext:Model>
                                    </Model>
                                </ext:Store>
                            </Store>
                            <ColumnModel runat="server">
                                <Columns>
                                    <ext:Column runat="server" Text="编号" DataIndex="ID" />
                                    <ext:Column runat="server" Text="中文名称" DataIndex="ChineseName" Flex="1" />
                                </Columns>
                            </ColumnModel>
                            <SelectionModel>
                                <ext:RowSelectionModel runat="server" Mode="Multi" />
                            </SelectionModel>
                        </ext:GridPanel>
                    </Items>
                    <Buttons>
                        <ext:Button ID="btnSaveAppProjectsInMobileDevices" runat="server" Text="保存设置" Icon="Disk">
                            <Listeners>
                                <Click Handler="#{GridPanel2}.submitData();" />
                            </Listeners>
                        </ext:Button>
                        <ext:Button ID="btnCancelAppProjectsInMobileDevices" runat="server" Text="取消" Icon="Cancel">
                            <Listeners>
                                <Click Handler="#{subWindowAppProjectsSelector}.hide();" />
                            </Listeners>
                        </ext:Button>
                    </Buttons>
                </ext:Window>

                <ext:GridPanel ID="gridAppProjectsInRoles" runat="server" Region="Center" Border="true"
                    Icon="ApplicationViewList"  Title="此角色可以访问的App项目">
                    <TopBar>
                        <ext:Toolbar ID="toolbarAppDevicesInMobileDevices" runat="server" AutoScroll="true">
                            <Items>
                                <ext:Button ID="btnAppProjectsInRolesConfig"
                                    runat="server"
                                    Text="配置角色App权限"
                                    Disabled="true"
                                    Icon="Add">
                                    <DirectEvents>
                                        <Click OnEvent="btnAppProjectsInRolesConfig_Click" />
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnAppProjectsInRolesDelete"
                                    runat="server"
                                    Text="移除角色App权限"
                                    Disabled="true"
                                    Icon="Delete">
                                    <DirectEvents>
                                        <Click OnEvent="btnAppProjectsInRolesDelete_Click">
                                            <Confirmation ConfirmRequest="true" Title="系统提示" Message="您确定要移除选中的APP权限吗？" />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Store>
                        <ext:Store ID="StoreAppProjects"
                            runat="server"
                            PageSize="20"
                            RemotePaging="false">
                            <Model>
                                <ext:Model runat="server" IDProperty="ID">
                                    <Fields>
                                        <ext:ModelField Name="ID" Type="Int" />
                                        <ext:ModelField Name="ChineseName" Type="String" />
                                        <ext:ModelField Name="Status" Type="Int" />
                                        <ext:ModelField Name="StartDate" Type="Date" />
                                    </Fields>
                                </ext:Model>
                            </Model>
                        </ext:Store>
                    </Store>
                    <ColumnModel runat="server">
                        <Columns>
                            <ext:Column runat="server" Text="编号" DataIndex="ID" />
                            <ext:Column runat="server" Text="中文名" DataIndex="ChineseName" Flex="1" />
                            <ext:Column runat="server" Text="项目状态" DataIndex="Status">
                                <Renderer Handler="
                                    if(value == 1)
                                        return '使用中';
                                    else if(value == 2)
                                        return '已暂停';
                                    ">
                                </Renderer>
                            </ext:Column>
                            <ext:DateColumn runat="server" Text="启动日期" DataIndex="StartDate" Format="yyyy-MM-dd" />
                        </Columns>
                    </ColumnModel>
                    <SelectionModel>
                        <ext:RowSelectionModel runat="server" Mode="Single">
                            <DirectEvents>
                                <Select OnEvent="gridAppProjectsInRoles_RowSelected" />
                            </DirectEvents>
                        </ext:RowSelectionModel>
                    </SelectionModel>
                    <BottomBar>
                        <ext:PagingToolbar runat="server" HideRefresh="true" />
                    </BottomBar>
                </ext:GridPanel>
            </Items>
        </ext:Viewport>
    </form>
</body>
</html>
