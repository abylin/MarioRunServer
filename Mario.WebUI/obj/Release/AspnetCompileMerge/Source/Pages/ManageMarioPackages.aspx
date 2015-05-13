<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageMarioPackages.aspx.cs" Inherits="Mario.WebUI.Pages.ManageMarioPackages" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Mario移动客户端版本管理</title>
</head>
<body>
    <form id="form1" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Window runat="server" ID="subWindow" Modal="true" Layout="FitLayout" Width="450"  ToFrontOnShow="true" Hidden="true" CloseAction="Hide">
            <Items>
                <ext:FormPanel runat="server" Layout="FormLayout" ID="submitPanel" Border="false">
                    <Items>
                        <ext:TextField ID="txtID" FieldLabel="编号" runat="server" ReadOnly="true" />
                        <ext:NumberField ID="txtVersion" FieldLabel="版本" runat="server" AllowBlank="false" BlankText="版本号不能为空" MinValue="1" MaxValue="99999" />
                        <ext:TextField ID="txtDownloadUrl" FieldLabel="下载地址" runat="server" AllowBlank="false" BlankText="下载地址不能为空" MaxLength="250" />
                        <ext:SelectBox ID="selectBoxPlatform" FieldLabel="所属平台" runat="server">
                            <Items>
                                <ext:ListItem Text="Android" Value="1" />
                                <ext:ListItem Text="iOS" Value="2" />
                                <ext:ListItem Text="Windows Phone" Value="3" />
                                <ext:ListItem Text="其它" Value="4" />
                            </Items>
                            <SelectedItems>
                                <ext:ListItem Index="0" />
                            </SelectedItems>
                        </ext:SelectBox>
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
                <ext:GridPanel ID="GridMarioPackages" runat="server" Region="Center">
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
                                    Text="修改"
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
                                <ext:FileUploadField ID="fileUploadMario" runat="server" EmptyText="请选择一个Mario客户端安装包" HideLabel="true"
                                    Height="26" Width="100" ButtonText="上传" Icon="ArrowUp" ButtonOnly="true">
                                    <DirectEvents>
                                        <Change OnEvent="fileUploadMario_Unload"
                                            Before="Ext.Msg.wait('正在上传文件...', '上传中');"
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
                        <ext:Store ID="StoreMarioPackages"
                            runat="server"
                            PageSize="20"
                            RemotePaging="false">
                            <Model>
                                <ext:Model runat="server" IDProperty="ID">
                                    <Fields>
                                        <ext:ModelField Name="ID" Type="Int" />
                                        <ext:ModelField Name="Version" Type="Int" />
                                        <ext:ModelField Name="DownloadUrl" Type="String" />
                                        <ext:ModelField Name="Platform" Type="Int" />
                                    </Fields>
                                </ext:Model>
                            </Model>
                        </ext:Store>
                    </Store>
                    <ColumnModel runat="server">
                        <Columns>
                            <ext:Column runat="server" Text="编号" DataIndex="ID" />
                            <ext:Column runat="server" Text="版本" DataIndex="Version" />
                            <ext:Column runat="server" Text="平台" DataIndex="Platform">
                                <Renderer Handler="
                                    if(value == 1)
                                        return 'Android';
                                    else if(value == 2)
                                        return 'iOS';
                                    else if(value == 3)
                                        return 'Windows Phone';
                                    else if(value == 4)
                                        return '其它';
                                    ">
                                </Renderer>
                            </ext:Column>
                            <ext:Column runat="server" Text="下载地址" Flex="1" DataIndex="DownloadUrl" />
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
