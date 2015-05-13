<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewAppProjectsSummaryReport.aspx.cs" Inherits="Mario.WebUI.Pages.ViewAppProjectsSummaryReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>项目汇总一览表</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <ext:ResourceManager runat="server" />
        <ext:Viewport runat="server" Layout="FitLayout">
            <Items>
                <ext:GridPanel ID="GridPanelSummaryReport" runat="server" Region="Center" >
                    <Store>
                        <ext:Store ID="Store1" runat="server" PageSize="10" RemotePaging="false" OnReadData="GridPanelSummaryReport_Refresh">
                            <Model>
                                <ext:Model runat="server" IDProperty="ID">
                                    <Fields>
                                        <ext:ModelField Name="ID" />
                                        <ext:ModelField Name="ChineseName" />
                                        <ext:ModelField Name="YesterdayNewAdd" />
                                        <ext:ModelField Name="YesterdayRetention" />
                                        <ext:ModelField Name="AddLimit" />
                                        <ext:ModelField Name="MobileDeviceCount" />
                                    </Fields>
                                </ext:Model>
                            </Model>
                        </ext:Store>
                    </Store>
                    <ColumnModel runat="server">
                        <Columns>
                            <ext:Column runat="server" Text="项目编号" DataIndex="ID" >
                                <Filter>
                                    <ext:NumberFilter />
                                </Filter>
                            </ext:Column>
                            <ext:Column runat="server" Text="中文名" DataIndex="ChineseName" Width="220" >
                                <Filter>
                                    <ext:StringFilter />
                                </Filter>
                            </ext:Column>
                            <ext:Column runat="server" Text="昨日新增数" DataIndex="YesterdayNewAdd">
                                <Filter>
                                    <ext:NumberFilter />
                                </Filter>
                            </ext:Column>
                            <ext:Column runat="server" Text="昨日留存数" DataIndex="YesterdayRetention">
                                <Filter>
                                    <ext:NumberFilter />
                                </Filter>
                            </ext:Column>
                            <ext:Column runat="server" Text="新增上限数" DataIndex="AddLimit" >
                                <Filter>
                                    <ext:NumberFilter />
                                </Filter>
                            </ext:Column>
                            <ext:Column runat="server" Text="安装移动设备数" DataIndex="MobileDeviceCount" Flex="1">
                                <Filter>
                                    <ext:NumberFilter />
                                </Filter>
                            </ext:Column>
                        </Columns>
                    </ColumnModel>
                    <SelectionModel>
                        <ext:RowSelectionModel runat="server" Mode="Single" />
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
                                        <ext:ListItem Text="10" />
                                        <ext:ListItem Text="20" />
                                        <ext:ListItem Text="50" />
                                        <ext:ListItem Text="100" />
                                    </Items>
                                    <SelectedItems>
                                        <ext:ListItem Value="10" />
                                    </SelectedItems>
                                    <Listeners>
                                        <Select Handler="#{GridPanelSummaryReport}.store.pageSize = parseInt(this.getValue(), 10); #{GridPanelSummaryReport}.store.reload();" />
                                    </Listeners>
                                </ext:ComboBox>
                            </Items>
                            <Plugins>
                                <ext:ProgressBarPager runat="server" />
                            </Plugins>
                        </ext:PagingToolbar>
                    </BottomBar>
                </ext:GridPanel>
            </Items>
        </ext:Viewport>
    </div>
    </form>
</body>
</html>
