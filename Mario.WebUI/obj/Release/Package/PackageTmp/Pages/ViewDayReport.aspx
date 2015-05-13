<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewDayReport.aspx.cs" Inherits="Mario.WebUI.Pages.ViewDayReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>新增与留存日报表</title>
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
        <ext:ResourceManager runat="server" />
        <ext:Viewport runat="server" Layout="BorderLayout">
            <Items>
                <ext:GridPanel ID="GridPanelDayReport" runat="server" Height="300" Region="North">
                    <TopBar>
                        <ext:Toolbar runat="server" ID="Toolbar1" AutoScroll="true">
                            <Items>
                                <ext:DateField ID="DateFieldStart" runat="server" LabelWidth="25" FieldLabel="从" Vtype="daterange" EndDateField="DateFieldEnd" EnableKeyEvents="true" AllowBlank="false" Width="200">
                                    <Listeners>
                                        <KeyUp Fn="onKeyUp" />
                                    </Listeners>
                                </ext:DateField>
                                <ext:DateField ID="DateFieldEnd" runat="server" LabelWidth="25" Vtype="daterange" FieldLabel="到" StartDateField="DateFieldStart" EnableKeyEvents="true" AllowBlank="false" Width="200">
                                    <Listeners>
                                        <KeyUp Fn="onKeyUp" />
                                    </Listeners>
                                </ext:DateField>
                                <ext:SelectBox ID="selectBoxAppProject" runat="server" LabelWidth="70" FieldLabel="App项目" AllowBlank="false" Editable="false" Width="250">
                                    <SelectedItems>
                                        <ext:ListItem Index="0" />
                                    </SelectedItems>
                                </ext:SelectBox>
                                <ext:Button ID="btnQuery"
                                    runat="server"
                                    Text="查询"
                                    Icon="ChartCurveGo">
                                    <DirectEvents>
                                        <Click OnEvent="btnQuery_Click">
                                            <EventMask ShowMask="true" Msg="正在查询数据，请稍候..."></EventMask>
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button runat="server" Text="导出Excel" AutoPostBack="true" OnClick="btnToExcel_Click" Icon="PageExcel" />
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Store>
                        <ext:Store ID="Store1" runat="server" PageSize="5">
                            <Model>
                                <ext:Model runat="server">
                                    <Fields>
                                        <ext:ModelField Name="ID" />
                                        <ext:ModelField Name="CollectDate" Type="Date" />
                                        <ext:ModelField Name="AddCount" />
                                        <ext:ModelField Name="Retention" />
                                    </Fields>
                                </ext:Model>
                            </Model>
                        </ext:Store>
                    </Store>
                    <ColumnModel runat="server">
                        <Columns>
                            <ext:Column runat="server" Text="编号" DataIndex="ID" />
                            <ext:DateColumn runat="server" Text="统计日期" DataIndex="CollectDate" Format="yyyy-MM-dd" Width="180" />
                            <ext:Column runat="server" Text="新增数" DataIndex="AddCount" />
                            <ext:Column runat="server" Text="留存数" DataIndex="Retention" Flex="1" />
                        </Columns>
                    </ColumnModel>
                    <SelectionModel>
                        <ext:RowSelectionModel runat="server" Mode="Multi" />
                    </SelectionModel>
                    <View>
                        <ext:GridView runat="server" StripeRows="true" />
                    </View>
                    <BottomBar>
                        <ext:PagingToolbar runat="server" HideRefresh="true" />
                    </BottomBar>
                </ext:GridPanel>
                <ext:CartesianChart ID="ChartReport" runat="server" Animate="true" Region="Center">
                    <Store>
                        <ext:Store runat="server">
                            <Model>
                                <ext:Model runat="server">
                                    <Fields>
                                        <ext:ModelField Name="CollectDate" Type="Date" />
                                        <ext:ModelField Name="AddCount" />
                                        <ext:ModelField Name="Retention" />
                                    </Fields>
                                </ext:Model>
                            </Model>
                        </ext:Store>
                    </Store>
                    <Axes>
                        <ext:TimeAxis Title="统计日期" Fields="CollectDate" Position="Bottom" DateFormat="M月d日" />
                        <ext:NumericAxis Title="新增数" Fields="AddCount" Position="Left" Minimum="0">
                            <TitleConfig FillStyle="#94ae0a" />
                            <Label FillStyle="#94ae0a" />
                        </ext:NumericAxis>
                        <ext:NumericAxis Title="留存数" Fields="Retention" Position="Right" Minimum="0">
                            <TitleConfig FillStyle="#115fa6" />
                            <Label FillStyle="#115fa6" />
                        </ext:NumericAxis>
                    </Axes>
                    <Series>
                        <ext:LineSeries Titles="新增数" XField="CollectDate" YField="AddCount" Smooth="1">
                            <HighlightConfig>
                                <ext:CircleSprite Radius="7" />
                            </HighlightConfig>
                            <Marker>
                                <ext:CircleSprite Radius="3" LineWidth="0" />
                            </Marker>
                        </ext:LineSeries>
                        <ext:LineSeries Titles="留存数" XField="CollectDate" YField="Retention" Smooth="1">
                            <HighlightConfig>
                                <ext:CircleSprite Radius="7" />
                            </HighlightConfig>
                            <Marker>
                                <ext:CircleSprite Radius="3" LineWidth="0" />
                            </Marker>
                        </ext:LineSeries>
                    </Series>
                    <Plugins>
                        <ext:VerticalMarker runat="server">
                            <XLabelRenderer Handler="return Ext.util.Format.date(value, 'Md日');" />
                        </ext:VerticalMarker>
                    </Plugins>
                    <LegendConfig runat="server" Dock="Bottom" />
                </ext:CartesianChart>
            </Items>
        </ext:Viewport>
    </form>
</body>
</html>
