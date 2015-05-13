<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageUserInfos.aspx.cs" Inherits="Mario.WebUI.Pages.ManageUserInfos" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>用户管理</title>
</head>
<body>
    <form id="form1" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Window runat="server" ID="subWindowUserInfo" Modal="true" Layout="FitLayout" Width="350" Height="330" ToFrontOnShow="true" Hidden="true" CloseAction="Hide">
            <Items>
                <ext:FormPanel runat="server" Layout="FormLayout" ID="submitPanel" Border="false">
                    <Items>
                        <ext:TextField ID="txtUserID" FieldLabel="用户编号" runat="server" ReadOnly="true" />
                        <ext:TextField ID="txtUserName" FieldLabel="用户名" runat="server" AllowBlank="false" BlankText="用户名不能为空" MaxLength="250" />
                        <ext:TextField ID="txtPassword" FieldLabel="初始密码" runat="server" Text="1234(请在用户登录后修改)" ReadOnly="true" />
                        <ext:TextField ID="txtChineseName" FieldLabel="用户中文名" runat="server" AllowBlank="false" BlankText="中文姓名不能为空" MaxLength="50" />
                        <ext:TextField ID="txtWeiXinOpenID" FieldLabel="OpenID" runat="server" MaxLength="32" />
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
                        <Click Handler="#{subWindowUserInfo}.hide();" />
                    </Listeners>
                </ext:Button>
            </Buttons>
        </ext:Window>
        <ext:Viewport ID="Viewport1" runat="server" Margins="0 0 0 0" Layout="BorderLayout">
            <Items>
                <ext:GridPanel ID="GridUserInfos" runat="server" Region="Center">
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
                                            <Confirmation ConfirmRequest="true" Title="系统提示" Message="您确定要删除选中的用户吗？" />
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </TopBar>
                    <Store>
                        <ext:Store ID="StoreUserInfos"
                            runat="server"
                            PageSize="20"
                            RemotePaging="false">
                            <Model>
                                <ext:Model runat="server" IDProperty="ID">
                                    <Fields>
                                        <ext:ModelField Name="ID" Type="Int" />
                                        <ext:ModelField Name="UserName" Type="String" />
                                        <ext:ModelField Name="ChineseName" Type="String" />
                                        <ext:ModelField Name="WeiXinOpenID" Type="String" />
                                    </Fields>
                                </ext:Model>
                            </Model>
                        </ext:Store>
                    </Store>
                    <ColumnModel runat="server">
                        <Columns>
                            <ext:Column runat="server" Text="用户编号" DataIndex="ID" />
                            <ext:Column runat="server" Text="用户名" DataIndex="UserName" />
                            <ext:Column runat="server" Text="中文名" DataIndex="ChineseName" Flex="1" />
                        </Columns>
                    </ColumnModel>
                    <SelectionModel>
                        <ext:RowSelectionModel runat="server" Mode="Single">
                            <DirectEvents>
                                <Select OnEvent="gridUserinfos_RowSelected" />
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
