<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="Mario.WebUI.Pages.ChangePassword" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>修改个人密码</title>
</head>
<body>
    <form id="form1" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" />
        <ext:Window runat="server" ID="subWindowWeiXinQRCode" Title="微信扫描二维码绑定账号" Modal="true"  Layout="FitLayout" Width="250" Height="250" Hidden="true" CloseAction="Hide">
            <Items>
                <ext:Image runat="server" ID="QRCodeImage"></ext:Image>
            </Items>
            <DirectEvents>
                <Show OnEvent="subWindowWeiXinQRCode_Show"></Show>
                <Hide OnEvent="subWindowWeiXinQRCode_Hide"></Hide>
            </DirectEvents>
        </ext:Window>
        <ext:TaskManager runat="server" ID="QRImageTaskManager">
                <Tasks>
                    <ext:Task TaskID="WaitScanQRImageTask" AutoRun="false" Interval="500">
                        <DirectEvents>
                            <Update OnEvent="WaitScanQRImageTask_Update"></Update>
                        </DirectEvents>
                    </ext:Task>
                </Tasks>
            </ext:TaskManager>
        <ext:FormPanel ID="pwdForm" runat="server" Layout="FormLayout" Width="500">
            <Content>
                <ext:TextField ID="txtOldPassword" runat="server" FieldLabel="当前密码" InputType="Password"
                    AllowBlank="false" BlankText="当前密码不能为空" ValidateOnBlur="true">
                </ext:TextField>
                <ext:TextField ID="txtNewPassword" runat="server" FieldLabel="新密码"
                    BlankText="新密码不能为空" InputType="Password" AllowBlank="false">
                    <Listeners>
                        <ValidityChange Handler="this.next().validate();" />
                        <Blur Handler="this.next().validate();" />
                    </Listeners>
                </ext:TextField>
                <ext:TextField ID="txtNewPasswordAgain" runat="server" Vtype="password"
                    AllowBlank="false" FieldLabel="确认新密码"  BlankText="确认新密码不能为空" InputType="Password"
                    MsgTarget="Side" VtypeText="新密码和确认新密码不同，请修正">
                    <CustomConfig>
                        <ext:ConfigItem Name="initialPassField" Value="txtNewPassword" Mode="Value" />
                    </CustomConfig>
                    <Listeners>
                        <ValidityChange Handler="this.validate();" />
                        <Blur Handler="this.validate();" />
                    </Listeners>
                </ext:TextField>
            </Content>
            <Buttons>
                <ext:Button runat="server" ID="btnSubmit" Text="修改密码" OnDirectClick="btnSubmit_Click">
                    <DirectEvents>
                        <Click OnEvent="btnSubmit_Click" Before="return #{pwdForm}.getForm().isValid();"></Click>
                    </DirectEvents>
                </ext:Button>
                <ext:Button runat="server" ID="btnBindWeiXin" Text="绑定微信" OnDirectClick="btnBindWeiXin_Click" Hidden="true">
                </ext:Button>
            </Buttons>
            <Listeners>
                <ValidityChange Handler="#{btnSubmit}.setDisabled(!valid);" />
            </Listeners>
        </ext:FormPanel>
    </form>
</body>
</html>
