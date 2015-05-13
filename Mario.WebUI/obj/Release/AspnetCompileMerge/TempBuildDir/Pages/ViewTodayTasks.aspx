<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewTodayTasks.aspx.cs" Inherits="Mario.WebUI.Pages.ViewTodayTasks" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>查看今天的任务</title>
    <script>
        var changeTaskStatus = function (value) {
            if (value == 0)
                return '<span style="color:black;">待执行</span>';
            else if (value == 1)
                return '<span style="color:blue;">执行中</span>';
            else if (value == 2)
                return '<span style="color:green;">已完成</span>';
        };
        var updateMask = function (text) {
            myMask.down("div.x-mask-msg-text").update(text);
        };
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:TaskManager ID="TaskManager1" runat="server">
            <Tasks>
                <ext:Task TaskID="longaction" Interval="1000"  AutoRun="false"
                    OnStart="#{btnBuildTodayNewAdd}.setDisabled(true);"
                    OnStop="#{btnBuildTodayNewAdd}.setDisabled(false);
                            Ext.defer(Ext.net.Mask.hide, 2000);">
                    <DirectEvents>
                        <Update OnEvent="TaskManager1_Refresh" />
                    </DirectEvents>                   
                </ext:Task>
            </Tasks>
        </ext:TaskManager>
        <ext:Viewport ID="Viewport1" runat="server" Margins="0 0 0 0" Layout="BorderLayout">
            <Items>
                <ext:Window runat="server" ID="subWindowDetail" Title="任务详细信息" Modal="true" Layout="FitLayout" Width="800" Height="500"
                    ToFrontOnShow="true" Hidden="true" CloseAction="Hide" AutoScroll="true" MarginSpec="5 5 5 5" Icon="ApplicationViewList">
                    <Items>
                        <ext:FormPanel runat="server" Layout="TableLayout" ID="panelDetail" PaddingSummary="5px 5px 0" ButtonAlign="Center">
                            <Items>
                                <ext:Container runat="server" Layout="Column" Height="100">
                                    <Items>
                                        <ext:Container runat="server" Layout="FormLayout" ColumnWidth=".5" Padding="5">
                                            <Items>
                                                <ext:TextField ID="txtAppProjectsID" FieldLabel="所属APP项目号" runat="server" ReadOnly="true" />
                                                <ext:TextField ID="txtModel" FieldLabel="虚拟型号" runat="server" ReadOnly="true" />
                                                <ext:TextField ID="txtIMEI" FieldLabel="虚拟IMEI" runat="server" ReadOnly="true" />
                                                <ext:TextField ID="txtStartDate" FieldLabel="任务开始日期" runat="server" ReadOnly="true" />
                                                <ext:TextField ID="txtEndDate" FieldLabel="任务结束日期" runat="server" ReadOnly="true" />
                                                <ext:TextField ID="txtTaskStatus" FieldLabel="任务状态" runat="server" ReadOnly="true" />
                                                <ext:TextField ID="txtMobileDevicesID" FieldLabel="执行的真实手机号" runat="server" ReadOnly="true" />
                                                <ext:TextField ID="txtResolution" FieldLabel="分辨率" runat="server" ReadOnly="true" />
                                                <ext:TextField ID="txtAndroidID" FieldLabel="AndroidID" runat="server" ReadOnly="true" />
                                                <ext:TextField ID="txtUpdateTime" FieldLabel="执行时间" runat="server" ReadOnly="true" />
                                            </Items>
                                        </ext:Container>
                                        <ext:Container runat="server" Layout="FormLayout" ColumnWidth=".5" Padding="5">
                                            <Items>
                                                <ext:TextField ID="txtIMSI" FieldLabel="虚拟IMSI" runat="server" ReadOnly="true" />
                                                <ext:TextField ID="txtMAC" FieldLabel="虚拟MAC" runat="server" ReadOnly="true" />
                                                <ext:TextField ID="txtLine1Number" FieldLabel="虚拟电话号码" runat="server" ReadOnly="true" />
                                                <ext:TextField ID="txtPhoneNumberCity" FieldLabel="号码所在城市" runat="server" ReadOnly="true" />
                                                <ext:TextField ID="txtSimSerialNumber" FieldLabel="虚拟SIM卡号" runat="server" ReadOnly="true" />
                                                <ext:TextField ID="txtTelecomOpertaionorsName" FieldLabel="运营商名" runat="server" ReadOnly="true" />
                                                <ext:TextField ID="txtNetworkType" FieldLabel="网络类型" runat="server" ReadOnly="true" />
                                                <ext:TextField ID="txtOSVersion" FieldLabel="系统版本" runat="server" ReadOnly="true" />
                                                <ext:TextField ID="txtRetainStartTime" FieldLabel="首次留存允许执行时间" runat="server" ReadOnly="true" />                       
                                            </Items>
                                        </ext:Container>
                                    </Items>
                                </ext:Container>
                            </Items>
                            <Buttons>
                                <ext:Button runat="server" Text="关闭" Icon="Cancel" ID="btnClosed">
                                    <Listeners>
                                        <Click Handler="#{subWindowDetail}.hide();" />
                                    </Listeners>
                                </ext:Button>
                            </Buttons>
                        </ext:FormPanel>
                    </Items>
                </ext:Window>

                <ext:GridPanel ID="GridTasks" runat="server" Region="Center" >
                    <TopBar>
                        <ext:Toolbar ID="Toolbar1" runat="server" AutoScroll="true">
                            <Items>
                                <ext:SelectBox ID="selectBoxAppProject" runat="server" FieldLabel="App项目" LabelWidth="75" AllowBlank="false" Editable="false" Width="280">
                                    <SelectedItems>
                                        <ext:ListItem Index="0" />
                                    </SelectedItems>
                                </ext:SelectBox>
                                <ext:SelectBox ID="selectboxQueryType" runat="server" FieldLabel="查询类型" LabelWidth="70" Width="180" 
                                   Editable="false"  MultiSelect="false" Draggable="false" >
                                    <Items>
                                        <ext:ListItem Text="新增任务" Value="True"  />
                                        <ext:ListItem Text="留存任务" Value="False" />
                                    </Items>
                                    <SelectedItems>
                                        <ext:ListItem Index="0" />
                                    </SelectedItems>
                                </ext:SelectBox>
                                <ext:Button ID="btnQuery" runat="server" Text="查询" Icon="TableGo" >
                                    <DirectEvents>
                                        <Click OnEvent="btnQuery_Click">
                                            <EventMask ShowMask="true" Msg="正在查询数据，请稍候..."></EventMask>
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnBuildTodayNewAdd" runat="server" Text="手动生成今天新增任务" Icon="SmartphoneWrench" Handler="window.myMask = Ext.net.Mask.show({ msg: '手工生成任务初始化中...' });" >
                                    <DirectEvents>
                                        <Click OnEvent="btnBuildTodayNewAdd_Click" />
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnResetFailedTasks" runat="server" Text="恢复任务状态" Icon="TableGo" >
                                    <DirectEvents>
                                        <Click OnEvent="btnResetFailedTasks_Click">
                                            <Confirmation ConfirmRequest="true" Title="系统提示" Message="本操作将会将指定APP项目的“执行中”状态的任务全部设置为“待执行”。您确认要操作吗？" />
                                            <EventMask ShowMask="true" Msg="正在查询处理，请稍候..."></EventMask>
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Store>
                        <ext:Store ID="StoreGridTasks" runat="server" PageSize="10" RemotePaging="false" OnReadData="GridVirtualIMEI_Refresh">
                            <Model>
                                <ext:Model runat="server" IDProperty="ID">
                                    <Fields>
                                        <ext:ModelField Name="ID" Type="Int" />
                                        <ext:ModelField Name="TelecomOperatorsName" Type="String" />
                                        <ext:ModelField Name="SimSerialNumber" Type="Auto" />
                                        <ext:ModelField Name="Line1Number" Type="Auto" />
                                        <ext:ModelField Name="StartDate" Type="Date" />
                                        <ext:ModelField Name="EndDate" Type="Date" />
                                        <ext:ModelField Name="TaskStatus" Type="Int" />
                                        <ext:ModelField Name="MobileDevicesID" Type="Int" />
                                        <ext:ModelField Name="UpdateTime" Type="Date" />
                                        <ext:ModelField Name="RetainStartTime" Type="Date" />
                                    </Fields>
                                </ext:Model>
                            </Model>
                        </ext:Store>
                    </Store>
                    <ColumnModel runat="server">
                        <Columns>
                            <ext:Column runat="server" Text="编号" DataIndex="ID" />
                            <ext:Column runat="server" Text="虚拟电话号码" DataIndex="Line1Number" />
                            <ext:DateColumn runat="server" Text="任务开始日期" DataIndex="StartDate" Format="yyyy-MM-dd" />
                            <ext:DateColumn runat="server" Text="任务结束日期" DataIndex="EndDate" Format="yyyy-MM-dd" />
                            <ext:Column runat="server" Text="任务状态" DataIndex="TaskStatus" Filterable="true">
                                <Renderer Fn="changeTaskStatus" />
                                <Filter>
                                    <ext:ListFilter StoreID="storeTaskStatus"  />
                                </Filter>
                            </ext:Column>
                            <ext:Column runat="server" Text="今天执行的移动设备号" DataIndex="MobileDevicesID" />
                            <ext:DateColumn runat="server" Text="执行时间" DataIndex="UpdateTime" Format="yyyy-MM-dd HH:mm:ss" Flex="1" />
                            <ext:DateColumn runat="server" Text="首次留存允许执行时间" DataIndex="RetainStartTime" Format="yyyy-MM-dd HH:mm:ss" Flex="1" />
                        </Columns>
                    </ColumnModel> 
                    <Plugins>
                        <ext:GridFilters runat="server" MenuFilterText="过滤" />
                    </Plugins>          
                    <BottomBar>
                        <ext:PagingToolbar runat="server" AutoScroll="true">
                            <Items>
                                <ext:Label runat="server" Text="分页数目:" />
                                <ext:ToolbarSpacer runat="server" Width="10" />
                                <ext:ComboBox runat="server" Width="70" Editable="false">
                                    <Items>
                                        <ext:ListItem Text="10" />
                                        <ext:ListItem Text="20" />
                                        <ext:ListItem Text="50" />
                                        <ext:ListItem Text="100" />
                                    </Items>
                                    <SelectedItems>
                                        <ext:ListItem Value="10" />
                                    </SelectedItems>
                                    <Listeners>
                                        <Select Handler="#{GridTasks}.store.pageSize = parseInt(this.getValue(), 10); #{GridTasks}.store.reload();" />
                                    </Listeners>
                                </ext:ComboBox>
                                <ext:Label runat="server" ID="lblSummary" />
                            </Items>
                            <Plugins>
                                <ext:ProgressBarPager runat="server" />
                            </Plugins>
                        </ext:PagingToolbar>
                    </BottomBar>
                    <SelectionModel>
                        <ext:RowSelectionModel runat="server" Mode="Single" />
                    </SelectionModel>
                    <DirectEvents>
                        <RowDblClick OnEvent="GridTasks_RowDblClick"></RowDblClick>
                    </DirectEvents>
                </ext:GridPanel>
            </Items>
        </ext:Viewport>
        <ext:Store runat="server" ID="storeTaskStatus"  Data="<%# storeTaskDataArray %>" AutoDataBind="true">
            <Model>
                <ext:Model runat="server">
                    <Fields>
                        <ext:ModelField Name="id" Type="Int"></ext:ModelField>
                        <ext:ModelField Name="text" Type="String"></ext:ModelField>
                    </Fields> 
                </ext:Model>
            </Model>
        </ext:Store>
    </form>
</body>
</html>
