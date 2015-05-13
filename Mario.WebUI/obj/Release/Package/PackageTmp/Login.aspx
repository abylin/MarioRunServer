<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Mario.WebUI.Login" %>

<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Mario服务管理平台</title>
    <style type="text/css">
        .auto-style1 {
            font-size: x-large;
            text-align: center;
        }
    </style>
    <ext:XScript runat="server">
        <script>
            var testKeyEnter = function (field, e) {
                if (e.getKey() == Ext.EventObject.ENTER) {
                    return true;
                }
                else {
                    return false;
                }
            }
        </script>
    </ext:XScript>
</head>
<body>
    <ext:ResourceManager ID="resourceManager" runat="server" ShowWarningOnAjaxFailure="false">
        <Listeners>
            <WindowResize Handler="#{loginWindow}.center();"></WindowResize>
        </Listeners>
    </ext:ResourceManager>
    <form id="form1" runat="server">
        <p class="auto-style1">
            <img src="Resources/Images/Mario1.jpg" />
        </p>
        <ext:Window ID="windowsLogin" runat="server" Closable="false" Resizable="false" Icon="Lock"
            Title="Mario服务管理平台" Width="350" Modal="true" BodyPadding="0" Border="false"
            Layout="FitLayout">
            <Items>
                <ext:FormPanel runat="server" ID="submitPanel" Layout="FormLayout" Border="false">
                    <Items>
                        <ext:TextField ID="txtUsername" runat="server" FieldLabel="用户名"
                            AllowBlank="false" BlankText="请输入用户名." Icon="User" LabelWidth="60"
                            LabelAlign="Right" MsgTarget="Side">
                            <DirectEvents>
                                <SpecialKey OnEvent="txt_KeyEnter">
                                    <EventMask ShowMask="true" Msg="验证中..." />
                                </SpecialKey>
                            </DirectEvents>
                            <Listeners>
                                <SpecialKey Fn="testKeyEnter"></SpecialKey>
                            </Listeners>
                        </ext:TextField>
                        <ext:TextField
                            ID="txtPassword" runat="server" InputType="Password"
                            FieldLabel="密码" AllowBlank="false" BlankText="请输入密码."
                            Icon="Key" LabelWidth="60" LabelAlign="Right" MsgTarget="Side">
                            <DirectEvents>
                                <SpecialKey OnEvent="txt_KeyEnter">
                                    <EventMask ShowMask="true" Msg="验证中..." />
                                </SpecialKey>
                            </DirectEvents>
                            <Listeners>
                                <SpecialKey Fn="testKeyEnter" />
                            </Listeners>
                        </ext:TextField>
                    </Items>
                </ext:FormPanel>
            </Items>
            <Buttons>
                <ext:Button ID="btnLogin" runat="server" Text="登录" Icon="Accept">
                    <DirectEvents>
                        <Click OnEvent="btnLogin_Click" Before="return #{submitPanel}.getForm().isValid();">
                            <EventMask ShowMask="true" Msg="验证中..." MinDelay="300" />
                        </Click>
                    </DirectEvents>
                </ext:Button>
            </Buttons>
        </ext:Window>
        <p />
        <table style="width: 560px; text-align:center" align="center">
            <tr>
                <td><img src="Resources/Images/explorer/icon-chrome.png" alt="Chrome Logo" width="67" height="67" />
                <p>Chrome</p></td>
                <td><img  src="Resources/Images/explorer/icon-ff.png" alt="Firefox Logo" width="67" height="67" />
                <p>Firefox</p></td>
                <td><img  src="Resources/Images/explorer/icon-ie.png" alt="Internet Explorer Logo" width="67" height="67" />
                <p>IE8+</p></td>
                <td><img src="Resources/Images/explorer/icon-safari.png" alt="Safari Logo" width="67" height="67" />
                <p>Safari 6+</p></td>
                <td><img src="Resources/Images/explorer/icon-opera.png" alt="Opera Logo" width="67" height="67" />
                <p>Opera 12+</p></td>
                <td><img src="Resources/Images/explorer/icon-ios.png" alt="Apple Logo" width="67" height="67" />
                <p>Safari iOS6+</p></td>
                <td><img src="Resources/Images/explorer/icon-android.png" alt="Android Logo" width="67" height="67" />
                <p>Chrome Android 4.1+</p></td>
                <td><img src="Resources/Images/explorer/icon-win8.png" alt="Windows 8 Logo" width="67" height="67" />
                <p>IE10+ Win8</p></td>
            </tr>
        </table>
    </form>
</body>
</html>
