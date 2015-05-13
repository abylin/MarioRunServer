<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageAppProjects.aspx.cs" Inherits="Mario.WebUI.Pages.ManageAppProjects" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>APP项目管理</title>
    <script>
        var submitRetentionEdit = function (editor, e) {
            if (!(e.value === e.originalValue || (Ext.isDate(e.value) && Ext.Date.isEqual(e.value, e.originalValue)))) {
                Mario.gridRetention_Edit(e.record.data.ID, e.field, e.originalValue, e.value, e.record.data);
            }
        };
        var submitBlackTimeEdit = function (editor, e) {
            if (!(e.value === e.originalValue )) {
                Mario.gridBlackTime_Edit(e.record.data.ID, e.field, e.originalValue, e.value, e.record.data);
            }
        };

        var submitOperationMessagesEdit = function (editor, e) {
            if (!(e.value === e.originalValue || (Ext.isDate(e.value) && Ext.Date.isEqual(e.value, e.originalValue)))) {
                Mario.gridOperationMessages_Edit(e.record.data.ID, e.field, e.originalValue, e.value, e.record.data);
            }
        };
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <ext:ResourceManager ID="ResourceManager1" runat="server" />
            <ext:Viewport runat="server" Layout="BorderLayout">
                <Items>
                    <ext:Window runat="server" ID="subWindowAppProjects" Modal="true" Layout="FitLayout" Width="800" Height="480" ToFrontOnShow="true" Hidden="true" CloseAction="Hide">
                        <Items>
                            <ext:FormPanel runat="server" Layout="FormLayout" ID="submitPanelAppProjects" PaddingSummary="5px 5px 0" ButtonAlign="Center">
                                <Items>
                                    <ext:Container runat="server" Layout="Column">
                                        <Items>
                                            <ext:Container runat="server" Layout="FormLayout" ColumnWidth=".49" Padding="5">
                                                <Items>
                                                    <ext:TextField ID="txtAppProjectID" FieldLabel="编号" runat="server" ReadOnly="true" />
                                                    <ext:TextField ID="txtAppProjectChineseName" FieldLabel="中文名称" runat="server" AllowBlank="false" BlankText="中文名称不能为空" MaxLength="250" />
                                                    <ext:DateField ID="dateFieldAppProjectStartDate" runat="server" FieldLabel="启动日期" AllowBlank="false" BlankText="项目启动日期不能为空" />
                                                </Items>
                                            </ext:Container>
                                            <ext:Container runat="server" Layout="FormLayout" ColumnWidth=".49" Padding="5">
                                                <Items>
                                                    <ext:NumberField ID="txtAppProjectAddLimit" FieldLabel="新增上限数" runat="server" AllowBlank="false" BlankText="新增上限数不能为空" MinValue="0" />
                                                    <ext:NumberField ID="txtAppProjectRetainDelayHour" FieldLabel="留存延迟执行小时数" runat="server" AllowBlank="false" BlankText="0为无限制" MinValue="0" MaxValue="240">
                                                        <ToolTips>
                                                            <ext:ToolTip
                                                                runat="server"
                                                                Target="txtAppProjectRetainDelayHour"
                                                                Html="每天的新增做完后，需要至少等待X个小时后（通常超过24小时）才能开始执行留存操作。如果设置为0小时，则表示不受限制。" />
                                                        </ToolTips>
                                                    </ext:NumberField>
                                                    <ext:SelectBox ID="selectboxAppProjectStatus" FieldLabel="项目状态" runat="server">
                                                        <Items>
                                                            <ext:ListItem Text="使用中" Value="1" />
                                                            <ext:ListItem Text="已暂停" Value="2" />
                                                        </Items>
                                                        <SelectedItems>
                                                            <ext:ListItem Index="0" />
                                                        </SelectedItems>
                                                    </ext:SelectBox>
                                                </Items>
                                            </ext:Container>
                                        </Items>
                                    </ext:Container>
                                    <ext:Container runat="server" Layout="FitLayout" Padding="7">
                                        <Items>
                                            <ext:TextArea ID="txtAppProjectsMemo" runat="server" FieldLabel="备注" AutoScroll="true" Height="120" />
                                        </Items>
                                    </ext:Container>
                                </Items>
                            </ext:FormPanel>
                        </Items>
                        <Buttons>
                            <ext:Button runat="server" Text="提交" Icon="DatabaseSave" ID="btnAppProjectsSave">
                                <DirectEvents>
                                    <Click OnEvent="btnAppProjectsSave_Click" Before="return #{submitPanelAppProjects}.getForm().isValid();">
                                        <EventMask ShowMask="true" Msg="正在处理数据，请稍候..."></EventMask>
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button runat="server" Text="取消" Icon="Cancel" ID="btnAppProjectsCancel">
                                <Listeners>
                                    <Click Handler="#{subWindowAppProjects}.hide();" />
                                </Listeners>
                            </ext:Button>
                        </Buttons>
                    </ext:Window>

                    <ext:Window runat="server" ID="subWindowRetention" Modal="true" Title="设置留存率" Layout="FitLayout" Width="550" Height="500" ToFrontOnShow="true" Hidden="true" CloseAction="Hide">
                        <Items>
                            <ext:GridPanel ID="gridRetention" runat="server">
                                <ViewConfig LoadMask="true" LoadingText="加载数据中，请稍候......"></ViewConfig>
                                <TopBar>
                                    <ext:Toolbar ID="toolbarRetention" runat="server">
                                        <Items>
                                            <ext:Button ID="btnRetentionAdd" runat="server" Text="添加1天" AutoFocus="false" Icon="Add">
                                                <DirectEvents>
                                                    <Click OnEvent="btnRetentionAdd_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnRetentionDelete" runat="server" Text="删除最后1天" AutoFocus="false" Icon="Delete">
                                                <DirectEvents>
                                                    <Click OnEvent="btnRetentionDelete_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnRetentionExportExce" runat="server" Text="导出Excel" AutoFocus="false" Icon="PageExcel">
                                                <DirectEvents>
                                                    <Click OnEvent="btnRetentionExportExcel_Click">
                                                        <EventMask ShowMask="true" Msg="正在处理数据，请稍候..." />
                                                    </Click>
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:FileUploadField ID="btnRetentionImportExcel" runat="server" EmptyText="请选择一个Excel文件" HideLabel="true"
                                                Height="26" Width="100" ButtonText="导入Excel" Icon="PageExcel" ButtonOnly="true">
                                                <DirectEvents>
                                                    <Change OnEvent="btnRetentionImportExcel_Click"
                                                        Before="Ext.Msg.wait('导入的Excel将会清除现有方案所有报文, 正在上传文件...', '上传中');"
                                                        Failure="Ext.Msg.show({ 
                                                            title   : '系统提示', 
                                                            msg     : '上传中发生错误', 
                                                            minWidth: 200, 
                                                            modal   : true, 
                                                            icon    : Ext.Msg.ERROR, 
                                                            buttons : Ext.Msg.OK 
                                                        });">
                                                    </Change>
                                                </DirectEvents>
                                            </ext:FileUploadField>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Store>
                                    <ext:Store runat="server">
                                        <Model>
                                            <ext:Model runat="server" IDProperty="ID">
                                                <Fields>
                                                    <ext:ModelField Name="ID" />
                                                    <ext:ModelField Name="Days" />
                                                    <ext:ModelField Name="Retention" />
                                                    <ext:ModelField Name="AppProjectsID" />
                                                </Fields>
                                            </ext:Model>
                                        </Model>
                                    </ext:Store>
                                </Store>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column runat="server" Text="第X天" DataIndex="Days">
                                        </ext:Column>
                                        <ext:Column runat="server" Text="留存率(%)" DataIndex="Retention" Flex="1">
                                            <Editor>
                                                <ext:NumberField runat="server" MinValue="0" MaxValue="100" />
                                            </Editor>
                                        </ext:Column>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:CellSelectionModel runat="server" Mode="Multi" />
                                </SelectionModel>
                                <Plugins>
                                    <ext:CellEditing runat="server">
                                        <Listeners>
                                            <Edit Fn="submitRetentionEdit" />
                                        </Listeners>
                                    </ext:CellEditing>
                                </Plugins>
                            </ext:GridPanel>
                        </Items>
                    </ext:Window>

                    <ext:Window runat="server" ID="subWindowMobileDeviceList" Modal="true" Title="安装此APP项目的移动设备列表" Layout="FitLayout" Width="550" Height="500" ToFrontOnShow="true" Hidden="true" CloseAction="Hide">
                        <Items>
                            <ext:GridPanel ID="gridMobileDeviceList" runat="server">
                                <TopBar>
                                    <ext:Toolbar ID="toolbar1" runat="server">
                                        <Items>
                                            <ext:NumberField ID="txtMobileDeviceID" runat="server" FieldLabel="要添加的ID" MinValue="100000" MaxValue="999999" AllowBlank="false" Text="100000" />
                                            <ext:Button ID="btnMobileDeviceListAdd" runat="server" Text="添加" AutoFocus="false" Icon="Add">
                                                <DirectEvents>
                                                    <Click OnEvent="btnMobileDeviceListAdd_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnMobileDeviceListDelete" runat="server" Text="删除" AutoFocus="false" Icon="Delete">
                                                <DirectEvents>
                                                    <Click OnEvent="btnMobileDeviceListDelete_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Store>
                                    <ext:Store ID="storeMobileDeviceList" runat="server" PageSize="20" RemotePaging="false">
                                        <Model>
                                            <ext:Model runat="server" IDProperty="ID">
                                                <Fields>
                                                    <ext:ModelField Name="ID" Type="Int" />
                                                    <ext:ModelField Name="MobileDevicesID" Type="Int" />
                                                    <ext:ModelField Name="AppProjectsID" Type="Int" />
                                                </Fields>
                                            </ext:Model>
                                        </Model>
                                    </ext:Store>
                                </Store>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column runat="server" DataIndex="MobileDevicesID" Text="设备ID" Flex="1" />
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:RowSelectionModel runat="server" Mode="Single" />
                                </SelectionModel>
                                <BottomBar>
                                    <ext:PagingToolbar runat="server" />
                                </BottomBar>
                            </ext:GridPanel>
                        </Items>
                    </ext:Window>

                    <ext:Window runat="server" ID="subWindowBlackTime" Modal="true" Title="设置不执行时间" Layout="FitLayout" Width="400" Height="400" ToFrontOnShow="true" Hidden="true" CloseAction="Hide">
                        <Items>
                            <ext:GridPanel ID="gridBlackTime" runat="server">
                                <ViewConfig LoadMask="true" LoadingText="加载数据中，请稍候......"></ViewConfig>
                                <TopBar>
                                    <ext:Toolbar ID="toolbarBlackTime" runat="server">
                                        <Items>
                                            <ext:Button ID="btnBlackTimeAdd" runat="server" Text="添加1条" AutoFocus="false" Icon="Add">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBlackTimeAdd_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                            <ext:Button ID="btnBlackTimeDelete" runat="server" Text="删除" AutoFocus="false" Icon="Delete">
                                                <DirectEvents>
                                                    <Click OnEvent="btnBlackTimeDelete_Click" />
                                                </DirectEvents>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Store>
                                    <ext:Store runat="server">
                                        <Model>
                                            <ext:Model runat="server" IDProperty="ID">
                                                <Fields>
                                                    <ext:ModelField Name="ID" />
                                                    <ext:ModelField Name="AppProjectsID" />
                                                    <ext:ModelField Name="StartTime"  />
                                                    <ext:ModelField Name="EndTime" />
                                                </Fields>
                                            </ext:Model>
                                        </Model>
                                    </ext:Store>
                                </Store>
                                <ColumnModel runat="server">
                                    <Columns>
                                        <ext:Column runat="server" Text="开始时间" DataIndex="StartTime" Width="150">
                                            <Editor>
                                                <ext:TextField runat="server" AllowBlank="false"  />
                                            </Editor>
                                        </ext:Column>
                                        <ext:Column runat="server" Text="结束时间" DataIndex="EndTime" Flex="1" >
                                            <Editor>
                                                <ext:TextField runat="server" AllowBlank="false"  />
                                            </Editor>
                                        </ext:Column>
                                    </Columns>
                                </ColumnModel>
                                <SelectionModel>
                                    <ext:CellSelectionModel runat="server" />
                                </SelectionModel>
                                <Plugins>
                                    <ext:CellEditing runat="server">
                                        <Listeners>
                                            <Edit Fn="submitBlackTimeEdit" />
                                        </Listeners>
                                    </ext:CellEditing>
                                </Plugins>
                            </ext:GridPanel>
                        </Items>
                    </ext:Window>

                    <ext:GridPanel ID="gridAppProjects" runat="server" Region="North" Icon="ApplicationViewGallery" Title="APP项目列表" Collapsible="true">
                        <TopBar>
                            <ext:Toolbar ID="toolbarAppProjects" runat="server" AutoScroll="true">
                                <Items>
                                    <ext:Button ID="btnAppProjectsAdd" runat="server" Text="添加" Icon="Add">
                                        <DirectEvents>
                                            <Click OnEvent="btnAppProjectsAdd_Click"></Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnAppProjectsUpdate" runat="server" Text="更新基本信息" Disabled="true" Icon="DatabaseSave">
                                        <DirectEvents>
                                            <Click OnEvent="btnAppProjectsUpdate_Click">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnAppProjectsRetention" runat="server" Text="编辑留存率" Disabled="true" Icon="ChartBarEdit">
                                        <DirectEvents>
                                            <Click OnEvent="btnAppProjectsRetention_Click">
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnAppProjectsCopy" runat="server" Disabled="true" Text="复制项目" Icon="DatabaseCopy">
                                        <DirectEvents>
                                            <Click OnEvent="btnAppProjectsCopy_Click">
                                                <Confirmation ConfirmRequest="true" Title="系统提示" Message="新的项目会复制原项目的信息,并包括操作方案和报文,留存率设置等.名字为会在原中文名称加上'_副本'字样,且状态设置为已暂停.请在编辑后启用.您确定要复制选定的项目吗?" />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnAppProjectsDelete" runat="server" Text="删除" Disabled="true" Icon="Delete">
                                        <DirectEvents>
                                            <Click OnEvent="btnAppProjectsDelete_Click">
                                                <Confirmation ConfirmRequest="true" Title="系统提示" Message="您确定要删除选中的项吗？" />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnAppProjectsViewMobileDevices" runat="server" Text="已安装的移动设备" Disabled="true" Icon="PhoneLink">
                                        <DirectEvents>
                                            <Click OnEvent="btnAppProjectsViewMobileDevices_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnAppProjectsSetBlackTime" runat="server" Text="设置不执行时间" Disabled="true" Icon="ClockPause">
                                        <DirectEvents>
                                            <Click OnEvent="btnAppProjectsSetBlackTime_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Checkbox ID="chkAppProjectsDisplayPause" runat="server" BoxLabelAlign="After" BoxLabel="显示已暂停的项目"
                                        Width="150" OnDirectCheck="chkAppProjectsDisplayPause_CheckedChanged">
                                    </ext:Checkbox>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <Store>
                            <ext:Store ID="StoreAppProjects" runat="server" PageSize="10" RemotePaging="false" OnReadData="gridAppProjects_Refresh">
                                <Model>
                                    <ext:Model runat="server" IDProperty="ID">
                                        <Fields>
                                            <ext:ModelField Name="ID" Type="Int" />
                                            <ext:ModelField Name="ChineseName" Type="String" />
                                            <ext:ModelField Name="AddLimit" Type="Int" />
                                            <ext:ModelField Name="StartDate" Type="Date" />
                                            <ext:ModelField Name="Status" Type="Int" />
                                        </Fields>
                                    </ext:Model>
                                </Model>
                            </ext:Store>
                        </Store>
                        <ColumnModel runat="server">
                            <Columns>
                                <ext:Column runat="server" Text="编号" DataIndex="ID" />
                                <ext:Column runat="server" Text="中文名" DataIndex="ChineseName" Flex="1" Filterable="true">
                                    <Filter>
                                        <ext:StringFilter EmptyText="请输入搜索字符" />
                                    </Filter>
                                </ext:Column>
                                <ext:Column runat="server" Text="项目状态" DataIndex="Status">
                                    <Renderer Handler="
                                    if(value == 1)
                                        return '使用中';
                                    else if(value == 2)
                                        return '已暂停';
                                    ">
                                    </Renderer>
                                </ext:Column>
                                <ext:DateColumn runat="server" Text="启动日期" DataIndex="StartDate" Format="yyyy-MM-dd" Width="120" />
                                <ext:Column runat="server" Text="新增上限" DataIndex="AddLimit" />
                            </Columns>
                        </ColumnModel>
                        <SelectionModel>
                            <ext:RowSelectionModel runat="server" Mode="Single">
                                <DirectEvents>
                                    <Select OnEvent="gridAppProjects_RowSelected">
                                        <EventMask ShowMask="true" Target="CustomTarget" CustomTarget="#{gridOperationSchemes}" />
                                    </Select>
                                </DirectEvents>
                            </ext:RowSelectionModel>
                        </SelectionModel>
                        <Plugins>
                            <ext:GridFilters runat="server" MenuFilterText="过滤" />
                        </Plugins>
                        <BottomBar>
                            <ext:PagingToolbar runat="server">
                                <Items>
                                    <ext:Label runat="server" Text="分页数目:" />
                                    <ext:ToolbarSpacer runat="server" Width="10" />
                                    <ext:ComboBox runat="server" Width="60" Editable="false">
                                        <Items>
                                            <ext:ListItem Text="2" />
                                            <ext:ListItem Text="5" />
                                            <ext:ListItem Text="10" />
                                            <ext:ListItem Text="20" />
                                        </Items>
                                        <SelectedItems>
                                            <ext:ListItem Value="10" />
                                        </SelectedItems>
                                        <Listeners>
                                            <Select Handler="#{gridAppProjects}.store.pageSize = parseInt(this.getValue(), 10); #{gridAppProjects}.store.reload();" />
                                        </Listeners>
                                    </ext:ComboBox>
                                </Items>
                                <Plugins>
                                    <ext:ProgressBarPager runat="server" />
                                </Plugins>
                            </ext:PagingToolbar>
                        </BottomBar>
                        <DirectEvents>
                            <RowDblClick OnEvent="gridAppProjects_RowDblClick"></RowDblClick>
                        </DirectEvents>
                    </ext:GridPanel>

                    <ext:Window runat="server" ID="subWindowOperationSchemes" Modal="true" Layout="FitLayout" Width="400" Height="400" ToFrontOnShow="true" Hidden="true" CloseAction="Hide">
                        <Items>
                            <ext:FormPanel runat="server" Layout="FormLayout" ID="submitPanelOperationSchemes" Border="false">
                                <Items>
                                    <ext:TextField ID="txtOperationSchemesID" FieldLabel="方案编号" runat="server" ReadOnly="true" />
                                    <ext:TextField ID="txtOperationSchemesBelongToAppProjectID" FieldLabel="归属项目编号" runat="server" ReadOnly="true" />
                                    <ext:TextField ID="txtOperationSchemesName" FieldLabel="名称" runat="server" AllowBlank="false" BlankText="方案名称不能为空" MaxLength="250" />
                                    <ext:SelectBox ID="selectBoxSchemeType" runat="server" FieldLabel="方案类型" AllowBlank="false" Editable="false" IndicatorTip="aaa">
                                        <Items>
                                            <ext:ListItem Text="留存方案" Value="0" />
                                            <ext:ListItem Text="新增方案" Value="1" />
                                        </Items>
                                        <SelectedItems>
                                            <ext:ListItem Index="0" />
                                        </SelectedItems>
                                        <ToolTips>
                                            <ext:ToolTip
                                                runat="server"
                                                Target="selectBoxSchemeType"
                                                Html="做新增时执行新增方案，做留存时执行留存方案。当项目中没有新增方案时，做新增时将会自动从留存方案中随机取一条进行操作。" />
                                        </ToolTips>
                                    </ext:SelectBox>
                                    <ext:Checkbox ID="chkIsCopyOperationSchemes" runat="server" BoxLabelAlign="After" BoxLabel="同时复制到另一个项目" />
                                    <ext:SelectBox ID="selectBoxToAppProject" runat="server" FieldLabel="要复制到的项目" AllowBlank="false" Editable="false">
                                        <SelectedItems>
                                            <ext:ListItem Index="0" />
                                        </SelectedItems>
                                    </ext:SelectBox>
                                </Items>
                            </ext:FormPanel>
                        </Items>
                        <Buttons>
                            <ext:Button runat="server" Text="提交" Icon="DatabaseSave" ID="btnOperationSchemesSave">
                                <DirectEvents>
                                    <Click OnEvent="btnOperationSchemesSave_Click" Before="return #{submitPanelOperationSchemes}.getForm().isValid();">
                                        <EventMask ShowMask="true" Msg="正在处理数据，请稍候..."></EventMask>
                                    </Click>
                                </DirectEvents>
                            </ext:Button>
                            <ext:Button runat="server" Text="取消" Icon="Cancel" ID="btnOperationSchemesCancel">
                                <Listeners>
                                    <Click Handler="#{subWindowOperationSchemes}.hide();" />
                                </Listeners>
                            </ext:Button>
                        </Buttons>
                    </ext:Window>

                    <ext:GridPanel ID="gridOperationSchemes" runat="server" Title="项目中操作方案列表" Icon="ApplicationViewIcons" Region="West" Width="270">
                        <TopBar>
                            <ext:Toolbar ID="toolbarOperationSchemes" runat="server" AutoScroll="true">
                                <Items>
                                    <ext:Button ID="btnOperationSchemesAdd" runat="server" Text="添加" Disabled="true" Icon="Add">
                                        <DirectEvents>
                                            <Click OnEvent="btnOperationSchemesAdd_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnOperationSchemesUpdate" runat="server" Text="编辑" Disabled="true" Icon="DatabaseSave">
                                        <DirectEvents>
                                            <Click OnEvent="btnOperationSchemesUpdate_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnOperationSchemesDelete" runat="server" Text="删除" Disabled="true" Icon="Delete">
                                        <DirectEvents>
                                            <Click OnEvent="btnOperationSchemesDelete_Click">
                                                <Confirmation ConfirmRequest="true" Title="系统提示" Message="您确定要删除选中的项吗？" />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <Store>
                            <ext:Store ID="storeOperationSchemes" runat="server" PageSize="20" RemotePaging="false">
                                <Model>
                                    <ext:Model runat="server" IDProperty="ID">
                                        <Fields>
                                            <ext:ModelField Name="ID" />
                                            <ext:ModelField Name="Name" />
                                            <ext:ModelField Name="SchemeType" />
                                        </Fields>
                                    </ext:Model>
                                </Model>
                            </ext:Store>
                        </Store>
                        <ColumnModel runat="server">
                            <Columns>
                                <ext:Column runat="server" DataIndex="ID" Text="编号" Width="50" />
                                <ext:Column runat="server" DataIndex="Name" Text="方案名称" Flex="1" />
                                <ext:Column runat="server" Text="类型" DataIndex="SchemeType">
                                    <Renderer Handler="
                                    if(value == 0)
                                        return '留存方案';
                                    else if(value == 1)
                                        return '新增方案';
                                    ">
                                    </Renderer>
                                </ext:Column>
                            </Columns>
                        </ColumnModel>
                        <SelectionModel>
                            <ext:RowSelectionModel runat="server" Mode="Single">
                                <DirectEvents>
                                    <Select OnEvent="gridOperationSchemes_RowSelected">
                                        <EventMask ShowMask="true" Target="CustomTarget" CustomTarget="#{gridOperationMessages}" />
                                    </Select>
                                </DirectEvents>
                            </ext:RowSelectionModel>
                        </SelectionModel>
                        <BottomBar>
                            <ext:PagingToolbar runat="server" HideRefresh="true" />
                        </BottomBar>
                        <DirectEvents>
                            <RowDblClick OnEvent="gridAppProjects_RowDblClick"></RowDblClick>
                        </DirectEvents>
                    </ext:GridPanel>

                    <ext:Window runat="server" ID="subWindowOperationMessages" Modal="true" Layout="FitLayout" Width="760" Height="400"
                        ToFrontOnShow="true" Hidden="true" CloseAction="Hide" AutoScroll="true" MarginSpec="5 5 5 5" Icon="Pencil">
                        <Items>
                            <ext:FormPanel runat="server" ID="submitPanelOperationMessages" PaddingSummary="5px 5px 0" ButtonAlign="Center">
                                <Items>
                                    <ext:Container runat="server" Layout="Column">
                                        <Items>
                                            <ext:Container runat="server" Layout="FormLayout" ColumnWidth=".5" Padding="5">
                                                <Items>
                                                    <ext:TextField ID="txtOperationMessagesID" FieldLabel="操作编号" runat="server" ReadOnly="true" />
                                                    <ext:TextField ID="txtOperationMessagesBelongToSchemesID" FieldLabel="归属方案编号" runat="server" ReadOnly="true" />
                                                    <ext:NumberField ID="txtStep" FieldLabel="步骤序号" runat="server" AllowBlank="false" BlankText="步骤不能为空" MinValue="1" MaxValue="9999" />
                                                    <ext:NumberField ID="txtInterval" FieldLabel="操作间隔(毫秒)" runat="server" AllowBlank="false" BlankText="间隔不能为空" MinValue="0" MaxValue="600000" Step="500" />
                                                    <ext:SelectBox ID="selectboxAction" FieldLabel="动作" runat="server">
                                                        <Items>
                                                            <ext:ListItem Text="点击" Value="1" />
                                                            <ext:ListItem Text="滑动" Value="2" />
                                                            <ext:ListItem Text="物理按钮" Value="3" />
                                                            <ext:ListItem Text="执行ADB脚本" Value="4" />
                                                            <ext:ListItem Text="执行Shell脚本" Value="5" />
                                                        </Items>
                                                        <SelectedItems>
                                                            <ext:ListItem Index="0" />
                                                        </SelectedItems>
                                                    </ext:SelectBox>
                                                    <ext:TextArea ID="txtCommandScript" FieldLabel="命令脚本" runat="server" AutoScroll="true" LabelAlign="Top" />
                                                </Items>
                                            </ext:Container>
                                            <ext:Container runat="server" Layout="FormLayout" ColumnWidth=".5" Padding="5">
                                                <Items>
                                                    <ext:NumberField ID="txtXPoint" FieldLabel="起始X座标" runat="server" AllowBlank="false" BlankText="座标不能为空" MinValue="0" MaxValue="9999" />
                                                    <ext:NumberField ID="txtYPoint" FieldLabel="起始Y座标" runat="server" AllowBlank="false" BlankText="座标不能为空" MinValue="0" MaxValue="9999" />
                                                    <ext:NumberField ID="txtToXPoint" FieldLabel="目的X座标" runat="server" AllowBlank="false" BlankText="如无需要的值,则保持-1" MinValue="-1" MaxValue="9999" />
                                                    <ext:NumberField ID="txtToYPoint" FieldLabel="目的Y座标" runat="server" AllowBlank="false" BlankText="如无需要的值,则保持-1" MinValue="-1" MaxValue="9999" />
                                                    <ext:NumberField ID="txtPhysicalKey" FieldLabel="物理按钮" runat="server" AllowBlank="false" BlankText="如无需要的值,则保持-1" MinValue="-1" MaxValue="9999" />
                                                    <ext:TextArea ID="txtMemo" FieldLabel="备注" runat="server" AutoScroll="true" LabelAlign="Top" />
                                                </Items>
                                            </ext:Container>
                                        </Items>
                                    </ext:Container>
                                </Items>
                                <Buttons>
                                    <ext:Button runat="server" Text="提交" Icon="DatabaseSave" ID="btnOperationMessagesSave">
                                        <DirectEvents>
                                            <Click OnEvent="btnOperationMessagesSave_Click" Before="return #{submitPanelOperationMessages}.getForm().isValid();">
                                                <EventMask ShowMask="true" Msg="正在处理数据，请稍候..."></EventMask>
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button runat="server" Text="取消" Icon="Cancel" ID="btnOperationMessagesCancel">
                                        <Listeners>
                                            <Click Handler="#{subWindowOperationMessages}.hide();" />
                                        </Listeners>
                                    </ext:Button>
                                </Buttons>
                            </ext:FormPanel>
                        </Items>
                    </ext:Window>

                    <ext:GridPanel ID="gridOperationMessages" runat="server" Region="Center" Icon="ApplicationViewList" Title="方案中操作报文" Border="true">
                        <TopBar>
                            <ext:Toolbar ID="toolbarOperationMessages" runat="server" AutoScroll="true">
                                <Items>
                                    <ext:Button ID="btnOperationMessagesAdd" runat="server" Text="添加" Disabled="true" Icon="Add">
                                        <DirectEvents>
                                            <Click OnEvent="btnOperationMessagesAdd_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnOperationMessagesUpdate"
                                        runat="server" Text="修改" Disabled="true" Icon="DatabaseSave">
                                        <DirectEvents>
                                            <Click OnEvent="btnOperationMessagesUpdate_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnOperationMessagesDelete" runat="server" Text="删除" Disabled="true" Icon="Delete">
                                        <DirectEvents>
                                            <Click OnEvent="btnOperationMessagesDelete_Click">
                                                <Confirmation ConfirmRequest="true" Title="系统提示" Message="您确定要删除选中的项吗？" />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnOperationMessageMoveUp" runat="server" Text="上移" Disabled="true" Icon="ArrowUp">
                                        <DirectEvents>
                                            <Click OnEvent="btnOperationMessageMoveUp_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnOperationMessageMoveDown" runat="server" Text="下移" Disabled="true" Icon="ArrowDown">
                                        <DirectEvents>
                                            <Click OnEvent="btnOperationMessageMoveDown_Click" />
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnOperationMessageCopy" runat="server" Text="复制" Disabled="true" Icon="PageWhiteCopy">
                                        <DirectEvents>
                                            <Click OnEvent="btnOperationMessageCopy_Click">
                                                <Confirmation ConfirmRequest="true" Title="系统提示" Message="复制的报文将会插入到原报文的位置" />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:Button ID="btnOperationMessageExportExcel" runat="server" Text="导出Excel" Disabled="true" Icon="PageExcel">
                                        <DirectEvents>
                                            <Click OnEvent="btnOperationMessageExportExcel_Click">
                                                <EventMask ShowMask="true" Msg="正在处理数据，请稍候..." />
                                            </Click>
                                        </DirectEvents>
                                    </ext:Button>
                                    <ext:FileUploadField ID="btnOperationMessageImportExcel" runat="server" EmptyText="请选择一个Excel文件" HideLabel="true"
                                        ButtonText="导入Excel" Icon="PageExcel" ButtonOnly="true" Disabled="true">
                                        <DirectEvents>
                                            <Change OnEvent="btnOperationMessageImportExcel_Click"
                                                Before="Ext.Msg.wait('导入的Excel将会清除现有方案所有报文, 正在上传文件...', '上传中');"
                                                Failure="Ext.Msg.show({ 
                                                            title   : '系统提示', 
                                                            msg     : '上传中发生错误', 
                                                            minWidth: 200, 
                                                            modal   : true, 
                                                            icon    : Ext.Msg.ERROR, 
                                                            buttons : Ext.Msg.OK 
                                                        });">
                                            </Change>
                                        </DirectEvents>
                                    </ext:FileUploadField>
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <Store>
                            <ext:Store ID="storeOperationMessages" runat="server" PageSize="20" RemotePaging="false">
                                <Model>
                                    <ext:Model runat="server" IDProperty="ID">
                                        <Fields>
                                            <ext:ModelField Name="ID" Type="Int" />
                                            <ext:ModelField Name="Step" Type="Int" />
                                            <ext:ModelField Name="XPoint" Type="Int" />
                                            <ext:ModelField Name="YPoint" Type="Int" />
                                            <ext:ModelField Name="ToXPoint" Type="Int" />
                                            <ext:ModelField Name="ToYPoint" Type="Int" />
                                            <ext:ModelField Name="PhysicalKey" Type="Int" />
                                            <ext:ModelField Name="Interval" Type="Int" />
                                            <ext:ModelField Name="Action" Type="Int" />
                                            <ext:ModelField Name="CommandScript" Type="String" />
                                            <ext:ModelField Name="Memo" Type="String" />
                                        </Fields>
                                    </ext:Model>
                                </Model>
                            </ext:Store>
                        </Store>
                        <ColumnModel runat="server">
                            <Columns>
                                <ext:Column runat="server" Text="步骤序号" DataIndex="Step">
                                    <Editor>
                                        <ext:NumberField runat="server" MinValue="1" MaxValue="9999" />
                                    </Editor>
                                </ext:Column>
                                <ext:Column runat="server" Text="起始X座标" DataIndex="XPoint">
                                    <Editor>
                                        <ext:NumberField runat="server" MinValue="0" MaxValue="9999" />
                                    </Editor>
                                </ext:Column>
                                <ext:Column runat="server" Text="起始Y座标" DataIndex="YPoint">
                                    <Editor>
                                        <ext:NumberField runat="server" MinValue="0" MaxValue="9999" />
                                    </Editor>
                                </ext:Column>
                                <ext:Column runat="server" Text="目的X座标" DataIndex="ToXPoint">
                                    <Editor>
                                        <ext:NumberField runat="server" MinValue="-1" MaxValue="9999" />
                                    </Editor>
                                </ext:Column>
                                <ext:Column runat="server" Text="目的Y座标" DataIndex="ToYPoint">
                                    <Editor>
                                        <ext:NumberField runat="server" MinValue="-1" MaxValue="9999" />
                                    </Editor>
                                </ext:Column>
                                <ext:Column runat="server" Text="物理按键" DataIndex="PhysicalKey">
                                    <Editor>
                                        <ext:NumberField runat="server" MinValue="-1" MaxValue="9999" />
                                    </Editor>
                                </ext:Column>
                                <ext:Column runat="server" Text="操作间隔(毫秒)" DataIndex="Interval">
                                    <Editor>
                                        <ext:NumberField runat="server" MinValue="0" MaxValue="600000" />
                                    </Editor>
                                </ext:Column>
                                <ext:Column runat="server" Text="动作" DataIndex="Action">
                                    <Renderer Handler="
                                    if(value == 1)
                                        return '点击';
                                    else if(value == 2)
                                        return '滑动';
                                    else if(value == 3)
                                        return '物理按钮';
                                    else if(value == 4)
                                        return '执行ADB脚本';
                                    else if(value == 5)
                                        return '执行Shell脚本';
                                    ">
                                    </Renderer>
                                    <Editor>
                                        <ext:NumberField runat="server" MinValue="1" MaxValue="5" />
                                    </Editor>
                                </ext:Column>
                                <ext:Column runat="server" Text="命令脚本" DataIndex="CommandScript">
                                    <Editor>
                                        <ext:TextField runat="server" />
                                    </Editor>
                                </ext:Column>
                                <ext:Column runat="server" Text="备注" DataIndex="Memo" Flex="1">
                                    <Editor>
                                        <ext:TextField runat="server" />
                                    </Editor>
                                </ext:Column>
                            </Columns>
                        </ColumnModel>
                        <SelectionModel>
                            <ext:RowSelectionModel runat="server" Mode="Single">
                                <DirectEvents>
                                    <Select OnEvent="gridOperationMessages_RowSelected" />
                                </DirectEvents>
                            </ext:RowSelectionModel>
                        </SelectionModel>
                        <Plugins>
                            <ext:CellEditing runat="server">
                                <Listeners>
                                    <Edit Fn="submitOperationMessagesEdit" />
                                </Listeners>
                            </ext:CellEditing>
                        </Plugins>
                        <BottomBar>
                            <ext:PagingToolbar runat="server" HideRefresh="true" />
                        </BottomBar>
                    </ext:GridPanel>
                </Items>
            </ext:Viewport>
        </div>
    </form>
</body>
</html>
