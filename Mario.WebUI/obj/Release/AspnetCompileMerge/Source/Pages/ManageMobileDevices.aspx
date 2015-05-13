<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageMobileDevices.aspx.cs" Inherits="Mario.WebUI.Pages.ManageMobileDevices" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>管理移动设备</title>
    <script>
        var twoGridSelector = {
            add: function (source, destination) {
                source = source || GridPanel1;
                destination = destination || GridPanel2;

                if (source.selModel.hasSelection()) {
                    var records = source.selModel.getSelection();
                    source.store.remove(records);
                    destination.store.add(records);
                }
            },
            addAll: function (source, destination) {
                source = source || GridPanel1;
                destination = destination || GridPanel2;
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
                <ext:Window runat="server" ID="subWindowMobileDevices" Modal="true" Layout="FitLayout" Width="400" Height="350" ToFrontOnShow="true" Hidden="true" CloseAction="Hide">
                    <Items>
                        <ext:FormPanel runat="server" Layout="FormLayout" ID="submitPanelMobileDevices" Border="false">
                            <Items>
                                <ext:TextField ID="txtMobileDevicesID" FieldLabel="编号" runat="server" ReadOnly="true" />
                                <ext:TextField ID="txtRealIMEI" FieldLabel="真实IMEI" runat="server" ReadOnly="true" >
                                    <ToolTips>
                                        <ext:ToolTip
                                            runat="server"
                                            Target="txtRealIMEI"
                                            Html="PC上运行的虚拟Android设备的IMEI为0。" />
                                    </ToolTips>
                                </ext:TextField>
                                <ext:TextField ID="txtRealModel" FieldLabel="真实型号" runat="server" ReadOnly="true" />
                                <ext:TextField ID="txtMobileDevicesMemo" FieldLabel="说明" runat="server" MaxLength="250" />
                                <ext:TextField ID="txtLastResponseTime" FieldLabel="最后更新时间" runat="server" ReadOnly="true" />
                                <ext:Checkbox ID="chkInUse" runat="server" FieldLabel="是否在使用中" />
                            </Items>
                        </ext:FormPanel>
                    </Items>
                    <Buttons>
                        <ext:Button runat="server" Text="提交" Icon="DatabaseSave" ID="btnMobileDevicesSubmit">
                            <DirectEvents>
                                <Click OnEvent="btnMobileDevicesSubmit_Click" Before="return #{submitPanelMobileDevices}.getForm().isValid();">
                                    <EventMask ShowMask="true" Msg="正在处理数据，请稍候..."></EventMask>
                                </Click>
                            </DirectEvents>
                        </ext:Button>
                        <ext:Button runat="server" Text="取消" Icon="Cancel" ID="btnMobileDevicesCancel">
                            <Listeners>
                                <Click Handler="#{subWindowMobileDevices}.hide();" />
                            </Listeners>
                        </ext:Button>
                    </Buttons>
                </ext:Window>

                <ext:GridPanel ID="gridMobileDevices" runat="server" Title="移动设备列表" MarginSpec="0 0 5 5" Icon="Layout" Region="West"
                    Width="500" AutoScroll="true">
                    <TopBar>
                        <ext:Toolbar ID="toolbar1" runat="server" AutoScroll="true">
                            <Items>
                                <ext:Button ID="btnMobileDeviceModify"
                                    runat="server"
                                    Text="编辑"
                                    Disabled="true"
                                    Icon="DatabaseEdit">
                                    <DirectEvents>
                                        <Click OnEvent="btnMobileDeviceModify_Click" />
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnMobileDeviceDelete"
                                    runat="server"
                                    Text="删除"
                                    Disabled="true"
                                    Icon="Delete">
                                    <DirectEvents>
                                        <Click OnEvent="btnMobileDeviceDelete_Click">
                                            <Confirmation ConfirmRequest="true" Title="系统提示" Message="您确定要删除选中的项吗？" />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnMobileDeviceReboot"
                                    runat="server"
                                    Text="重启"
                                    Disabled="true"
                                    Icon="PhoneStop">
                                    <DirectEvents>
                                        <Click OnEvent="btnMobileDeviceReboot_Click">
                                            <Confirmation ConfirmRequest="true" Title="系统提示" Message="此移动设备将在下次获取任务时，收到重启指令。您确定要操作吗？" />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnMobileDeviceJSONMessage"
                                    runat="server"
                                    Text="模拟报文"
                                    Disabled="true"
                                    Icon="ArrowUp">
                                    <DirectEvents>
                                        <Click OnEvent="btnMobileDeviceJSONMessage_Click" />
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Store>
                        <ext:Store ID="storeMobileDevices" runat="server" PageSize="20" RemotePaging="false">
                            <Model>
                                <ext:Model runat="server" IDProperty="ID">
                                    <Fields>
                                        <ext:ModelField Name="ID" />
                                        <ext:ModelField Name="Memo" />
                                        <ext:ModelField Name="RealIMEI" />
                                        <ext:ModelField Name="RealModel" />
                                        <ext:ModelField Name="InUse" Type="Boolean" />
                                        <ext:ModelField Name="LastResponseTime" Type="Date" />
                                    </Fields>
                                </ext:Model>
                            </Model>
                        </ext:Store>
                    </Store>
                    <ColumnModel runat="server">
                        <Columns>
                            <ext:Column runat="server" DataIndex="ID" Text="编号" Width="90" />
                            <ext:Column runat="server" DataIndex="Memo" Text="说明"  Filterable="true">
                                <Filter>
                                    <ext:StringFilter EmptyText="请输入搜索字符" />
                                </Filter>
                            </ext:Column>
                            <ext:Column runat="server" DataIndex="RealIMEI" Text="IMEI"  Filterable="true">
                                <Filter>
                                    <ext:StringFilter EmptyText="请输入搜索字符" />
                                </Filter>
                            </ext:Column>
                            <ext:Column runat="server" DataIndex="InUse" Text="使用状态">
                                <Renderer Handler="
                                    if(value == true)
                                        return '使用中';
                                    else 
                                        return '暂停使用';
                                    ">
                                </Renderer>
                            </ext:Column>
                            <ext:DateColumn runat="server" DataIndex="LastResponseTime" Text="最后响应时间" Flex="1" Format="yyyy-MM-dd HH:mm:ss" />
                        </Columns>
                    </ColumnModel>
                    <SelectionModel>
                        <ext:CheckboxSelectionModel runat="server" Mode="Simple">
                            <DirectEvents>
                                <Select OnEvent="gridMobileDevices_RowSelected">
                                    <EventMask ShowMask="true" Target="CustomTarget" CustomTarget="#{gridAppProjectsInMobileDevices}" />
                                </Select>
                            </DirectEvents>
                        </ext:CheckboxSelectionModel>
                    </SelectionModel>
                    <Plugins>
                        <ext:GridFilters runat="server" MenuFilterText="过滤" />
                    </Plugins>
                    <BottomBar>
                        <ext:PagingToolbar runat="server" HideRefresh="true" />
                    </BottomBar>
                    <DirectEvents>
                        <RowDblClick OnEvent="gridMobileDevices_RowDblClick"></RowDblClick>
                    </DirectEvents>
                </ext:GridPanel>

                <ext:Window ID="subWindowAppProjectsSelector" runat="server" Modal="true" Closable="false" Height="520" Width="700" Icon="WorldAdd"
                    Title="注意：移动设备客户端上也要有安装对应的APP。" BodyPadding="5" BodyBorder="0" Layout="HBoxLayout" Hidden="true">
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
                            <Plugins>
                                <ext:GridFilters ID="GridFilters1" runat="server" />
                            </Plugins>
                        </ext:GridPanel>
                        <ext:Panel ID="PanelCommand" runat="server" Width="42" BodyStyle="background-color: transparent;" Border="false" Layout="Anchor">
                            <Items>
                                <ext:Panel runat="server" Border="false" BodyStyle="background-color: transparent;" AnchorVertical="40%" />
                                <ext:Panel ID="Panel1" runat="server" Border="false" BodyStyle="background-color: transparent;" BodyPadding="5">
                                    <Items>
                                        <ext:Button runat="server" Icon="ResultsetNext" StyleSpec="margin-bottom:2px;">
                                            <Listeners>
                                                <Click Handler="twoGridSelector.add(#{GridPanel1},#{GridPanel2});" />
                                            </Listeners>
                                            <ToolTips>
                                                <ext:ToolTip runat="server" Title="添加" Html="配置此应用到移动设备（安装APP还是要手动的）" />
                                            </ToolTips>
                                        </ext:Button>
                                        <ext:Button runat="server" Icon="ResultsetLast" StyleSpec="margin-bottom:2px;">
                                            <Listeners>
                                                <Click Handler="twoGridSelector.addAll(#{GridPanel1},#{GridPanel2});" />
                                            </Listeners>
                                            <ToolTips>
                                                <ext:ToolTip runat="server" Title="添加全部" Html="配置全部应用到移动设备（安装APP还是要手动的）" />
                                            </ToolTips>
                                        </ext:Button>
                                        <ext:Button runat="server" Icon="ResultsetPrevious" StyleSpec="margin-bottom:2px;">
                                            <Listeners>
                                                <Click Handler="twoGridSelector.remove(#{GridPanel1},#{GridPanel2});" />
                                            </Listeners>
                                            <ToolTips>
                                                <ext:ToolTip runat="server" Title="移除" Html="从移动设备上移除此应用（如需卸载APP，要手动在设备上操作）" />
                                            </ToolTips>
                                        </ext:Button>
                                        <ext:Button runat="server" Icon="ResultsetFirst" StyleSpec="margin-bottom:2px;">
                                            <Listeners>
                                                <Click Handler="twoGridSelector.removeAll(#{GridPanel1},#{GridPanel2});" />
                                            </Listeners>
                                            <ToolTips>
                                                <ext:ToolTip runat="server" Title="移除全部" Html="从移动设备上移除全部应用（如需卸载APP，要手动在设备上操作）" />
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

                <ext:GridPanel ID="gridAppProjectsInMobileDevices" runat="server" Region="Center" Border="true"
                    Icon="ApplicationViewList" Title="移动设备中的安装的APP项目(需与移动设备中的配置一致)">
                    <TopBar>
                        <ext:Toolbar ID="toolbarAppDevicesInMobileDevices" runat="server" AutoScroll="true">
                            <Items>
                                <ext:Button ID="btnAppDevicesInMobileDevicesConfig"
                                    runat="server"
                                    Text="选择要承载的APP"
                                    Disabled="true"
                                    Icon="Add">
                                    <DirectEvents>
                                        <Click OnEvent="btnAppDevicesInMobileDevicesConfig_Click" />
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnAppDevicesInMobileDevicesDelete"
                                    runat="server"
                                    Text="移除已承载的APP"
                                    Disabled="true"
                                    Icon="Delete">
                                    <DirectEvents>
                                        <Click OnEvent="btnAppDevicesInMobileDevicesDelete_Click">
                                            <Confirmation ConfirmRequest="true" Title="系统提示" Message="您确定要删除选中的项吗？" />
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
                                <Select OnEvent="gridAppProjectsInMobileDevices_RowSelected" />
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
