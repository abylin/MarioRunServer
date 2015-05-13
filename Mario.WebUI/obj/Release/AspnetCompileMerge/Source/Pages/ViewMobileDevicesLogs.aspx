<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewMobileDevicesLogs.aspx.cs" Inherits="Mario.WebUI.Pages.ViewMobileDevicesLogs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>查看移动端日志</title>
    <script>
        var onKeyUp = function () {
            var me = this,
                v = me.getValue(),
                field;

            if (me.startDateField) {
                field = Ext.getCmp(me.startDateField);
                field.setMaxValue(v);
                me.dateRangeMax = v;
            } else if (me.endDateField) {
                field = Ext.getCmp(me.endDateField);
                field.setMinValue(v);
                me.dateRangeMin = v;
            }

            field.validate();
        };
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Viewport ID="Viewport1" runat="server" Layout="BorderLayout">
            <Content>
                <ext:Window runat="server" ID="subWindowDetail" Title="日志详细信息" Modal="true" Layout="FitLayout" Width="600" Height="450"
                    ToFrontOnShow="true" Hidden="true" CloseAction="Hide" AutoScroll="true" MarginSpec="5 5 5 5" Icon="ApplicationViewList">
                    <Items>
                        <ext:FormPanel runat="server" ID="panelDetail" ButtonAlign="Center">
                            <LayoutConfig>
                                <ext:VBoxLayoutConfig Align="Stretch"></ext:VBoxLayoutConfig>
                            </LayoutConfig>
                            <Items>
                                <ext:TextField ID="txtLogID" FieldLabel="日志号" runat="server" ReadOnly="true" MarginSpec="5 5 5 5" />
                                <ext:TextField ID="txtMobileDevicesID" FieldLabel="移动设备编号" runat="server" ReadOnly="true" MarginSpec="5 5 5 5" />
                                <ext:TextField ID="txtLogTime" FieldLabel="记录时间" runat="server" ReadOnly="true" MarginSpec="5 5 5 5" />
                                <ext:TextArea ID="txtMemo" FieldLabel="详细内容" runat="server" ReadOnly="true" AutoScroll="true" Height="200" MarginSpec="5 5 5 5" />
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
                <ext:Panel Border="false" Region="Center" runat="server" Layout="FitLayout">
                    <Items>
                        <ext:GridPanel ID="GridPanelLog" runat="server">
                            <Store>
                                <ext:Store ID="Store1" runat="server" OnReadData="GridPanelLog_Refresh" PageSize="10">
                                    <Model>
                                        <ext:Model runat="server" IDProperty="ID">
                                            <Fields>
                                                <ext:ModelField Name="ID" />
                                                <ext:ModelField Name="LogTime" Type="Date" />
                                                <ext:ModelField Name="MobileDevicesID" />
                                                <ext:ModelField Name="Memo" />
                                            </Fields>
                                        </ext:Model>
                                    </Model>
                                </ext:Store>
                            </Store>
                            <ColumnModel runat="server">
                                <Columns>
                                    <ext:Column runat="server" Text="日志号" DataIndex="ID" />
                                    <ext:DateColumn runat="server" Text="记录时间" DataIndex="LogTime"
                                        Format="yyyy-MM-dd HH:mm:ss" Width="180">
                                        <Filter>
                                            <ext:DateFilter OnText="在" BeforeText="之前" AfterText="之后" />
                                        </Filter>
                                    </ext:DateColumn>
                                    <ext:Column runat="server" Text="移动设备编号" DataIndex="MobileDevicesID">
                                        <Filter>
                                            <ext:NumberFilter />
                                        </Filter>
                                    </ext:Column>
                                    <ext:Column runat="server" Text="内容" DataIndex="Memo" Flex="1" />
                                </Columns>
                            </ColumnModel>
                            <SelectionModel>
                                <ext:RowSelectionModel runat="server" Mode="Single" />
                            </SelectionModel>
                            <View>
                                <ext:GridView runat="server" StripeRows="true" />
                            </View>
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
                                                <ext:ListItem Text="10" />
                                                <ext:ListItem Text="20" />
                                                <ext:ListItem Text="50" />
                                                <ext:ListItem Text="100" />
                                            </Items>
                                            <SelectedItems>
                                                <ext:ListItem Value="10" />
                                            </SelectedItems>
                                            <Listeners>
                                                <Select Handler="#{GridPanelLog}.store.pageSize = parseInt(this.getValue(), 10); #{GridPanelLog}.store.reload();" />
                                            </Listeners>
                                        </ext:ComboBox>
                                    </Items>
                                    <Plugins>
                                        <ext:ProgressBarPager runat="server" />
                                    </Plugins>
                                </ext:PagingToolbar>
                            </BottomBar>
                            <TopBar>
                                <ext:Toolbar runat="server" AutoScroll="true">
                                    <Items>
                                        <ext:DateField
                                            ID="DateField1"
                                            Width="280"
                                            runat="server"
                                            Vtype="daterange"
                                            FieldLabel=" 开始日期"
                                            EnableKeyEvents="true"
                                            AllowBlank="False">
                                            <CustomConfig>
                                                <ext:ConfigItem Name="endDateField" Value="DateField2" Mode="Value" />
                                            </CustomConfig>
                                            <Listeners>
                                                <KeyUp Fn="onKeyUp" />
                                            </Listeners>
                                        </ext:DateField>
                                        <ext:DateField
                                            ID="DateField2"
                                            Width="280"
                                            runat="server"
                                            Vtype="daterange"
                                            FieldLabel=" 结束日期"
                                            EnableKeyEvents="true"
                                            AllowBlank="False">
                                            <CustomConfig>
                                                <ext:ConfigItem Name="startDateField" Value="DateField1" Mode="Value" />
                                            </CustomConfig>
                                            <Listeners>
                                                <KeyUp Fn="onKeyUp" />
                                            </Listeners>
                                        </ext:DateField>
                                        <ext:Button ID="btnSearch" runat="server" Text="查询" Icon="PageFind">
                                            <DirectEvents>
                                                <Click OnEvent="btnSearch_Click">
                                                    <EventMask ShowMask="true" Msg="查询中..." MinDelay="100" />
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button runat="server" Text="打印" Icon="Printer" Handler="this.up('grid').print();" />
                                        <ext:Button runat="server" Text="打印当前页" Icon="Printer" Handler="this.up('grid').print({currentPageOnly : true});" />
                                        <ext:Button runat="server" Text="导出Excel" AutoPostBack="true" OnClick="btnToExcel_Click" Icon="PageExcel" />
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <DirectEvents>
                                <RowDblClick OnEvent="GridPanelLog_RowDblClick"></RowDblClick>
                            </DirectEvents>
                        </ext:GridPanel>
                    </Items>
                </ext:Panel>
            </Content>
        </ext:Viewport>
    </form>
</body>
</html>
