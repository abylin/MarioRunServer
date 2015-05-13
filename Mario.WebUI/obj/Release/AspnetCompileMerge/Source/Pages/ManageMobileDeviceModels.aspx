<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageMobileDeviceModels.aspx.cs" Inherits="Mario.WebUI.Pages.ManageMobileDeviceModels" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>管理移动设备型号</title>
</head>
<body>
    <form id="form1" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Window runat="server" ID="subWindow" Modal="true" Layout="FitLayout" Width="400" Height="430" ToFrontOnShow="true" Hidden="true" CloseAction="Hide">
            <Items>
                <ext:FormPanel runat="server" Layout="FormLayout" ID="submitPanel" Border="false">
                    <Items>
                        <ext:TextField ID="txtID" FieldLabel="编号" runat="server" ReadOnly="true" />
                        <ext:TextField ID="txtBrand" FieldLabel="品牌" runat="server" AllowBlank="false" BlankText="品牌不能为空" MaxLength="250" />
                        <ext:TextField ID="txtDevice" FieldLabel="型号" runat="server" AllowBlank="false" BlankText="型号不能为空" MaxLength="250" />
                        <ext:NumberField ID="txtWidth" FieldLabel="分辨率宽度" runat="server" AllowBlank="false" BlankText="分辨率宽度不能为空" MinValue="100" MaxValue="9999"/>
                        <ext:NumberField ID="txtHeight" FieldLabel="分辨率高度" runat="server" AllowBlank="false" BlankText="分辨率高度不能为空" MinValue="100" MaxValue="9999"/>
                        <ext:NumberField ID="txtWeight" FieldLabel="权重" runat="server" AllowBlank="false" BlankText="权重不能为空" MinValue="1" MaxValue="999"/>
                        <ext:TextField ID="txtOSVersion" FieldLabel="系统版本" runat="server" MaxLength="250" />
                    </Items>
                </ext:FormPanel>
            </Items>
            <Buttons>
                <ext:Button runat="server" Text="提交" Icon="DatabaseSave" ID="ctl1075">
                    <DirectEvents>
                        <Click OnEvent="btnWinSave_Click" Before="return #{submitPanel}.getForm().isValid();">
                            <EventMask ShowMask="true" Msg="正在处理数据，请稍候..."></EventMask>
                        </Click>
                    </DirectEvents>
                </ext:Button>
                <ext:Button runat="server" Text="取消" Icon="Cancel" ID="ctl1077">
                    <Listeners>
                        <Click Handler="#{subWindow}.hide();" />
                    </Listeners>
                </ext:Button>
            </Buttons>
        </ext:Window>
        <ext:Viewport ID="Viewport1" runat="server" Margins="0 0 0 0" Layout="BorderLayout">
            <Items>
                <ext:GridPanel ID="GridMobileDeviceModels" runat="server" Region="Center">
                    <TopBar>
                        <ext:Toolbar ID="Toolbar1" runat="server" AutoScroll="true">
                            <Items>
                                <ext:Button ID="btnAdd"
                                    runat="server"
                                    Text="添加"
                                    Icon="Add">
                                    <DirectEvents>
                                        <Click OnEvent="btnAdd_Click"></Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnUpdate"
                                    runat="server"
                                    Text="更新"
                                    Disabled="true"
                                    Icon="DatabaseSave">
                                    <DirectEvents>
                                        <Click OnEvent="btnUpdate_Click">
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnDelete"
                                    runat="server"
                                    Text="删除"
                                    Disabled="true"
                                    Icon="Delete">
                                    <DirectEvents>
                                        <Click OnEvent="btnDelete_Click">
                                            <Confirmation ConfirmRequest="true" Title="系统提示" Message="您确定要删除选中的项吗？" />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                                <ext:Button ID="btnBuildWeightRandom"
                                    runat="server"
                                    Text="计算权重分配概率"
                                    Icon="ComputerEdit">
                                    <DirectEvents>
                                        <Click OnEvent="btnBuildWeightRandom_Click">
                                            <Confirmation ConfirmRequest="true" Title="系统提示" Message="修改某一个型号的权重后，需要重新计算权重分配概率才能生效。每天凌晨会自动计算，现在是否手动重新计算？" />
                                            <EventMask ShowMask="true" Msg="正在计算，请稍候..."></EventMask>
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Store>
                        <ext:Store ID="StoreMobileDeviceModels"
                            runat="server"
                            PageSize="20"
                            RemotePaging="false">
                            <Model>
                                <ext:Model runat="server" IDProperty="ID">
                                    <Fields>
                                        <ext:ModelField Name="ID" Type="Int" />
                                        <ext:ModelField Name="Brand" Type="String" />
                                        <ext:ModelField Name="Device" Type="String" />
                                        <ext:ModelField Name="Width" Type="Int" />
                                        <ext:ModelField Name="Height" Type="Int" />
                                        <ext:ModelField Name="OSVersion" Type="String" />
                                        <ext:ModelField Name="Weight" Type="Int" />
                                    </Fields>
                                </ext:Model>
                            </Model>
                        </ext:Store>
                    </Store>
                    <ColumnModel runat="server">
                        <Columns>
                            <ext:Column runat="server" Text="编号" DataIndex="ID" />
                            <ext:Column runat="server" Text="品牌" DataIndex="Brand" />
                            <ext:Column runat="server" Text="型号" DataIndex="Device" />
                            <ext:Column runat="server" Text="分辨率宽度" DataIndex="Width" />
                            <ext:Column runat="server" Text="分辨率高度" DataIndex="Height" />
                            <ext:Column runat="server" Text="权重" DataIndex="Weight" />
                            <ext:Column runat="server" Text="系统版本" DataIndex="OSVersion" Flex="1" />
                        </Columns>
                    </ColumnModel>
                    <SelectionModel>
                        <ext:RowSelectionModel runat="server" Mode="Single">
                            <DirectEvents>
                                <Select OnEvent="grid_RowSelected" />
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
