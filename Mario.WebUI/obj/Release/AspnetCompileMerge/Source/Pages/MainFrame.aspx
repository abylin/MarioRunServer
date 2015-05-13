<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MainFrame.aspx.cs" Inherits="Mario.WebUI.Pages.MainFrame" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Mario服务管理平台V2.0</title>
    <ext:XScript runat="server">
        <script>
            var addTab = function (tabPanel, id, title, url, menuItem) {
                var tab = tabPanel.getComponent(id);
                if (!tab) {
                    tab = tabPanel.add({ 
                        id       : id, 
                        title    : title, 
                        closable : true,
                        menuItem : menuItem,
                        loader   : {
                            url      : url,
                            renderer : "frame",
                            loadMask : {
                                showMask : true,
                                msg      : "页面载入中..."
                            }
                        }
                    });

                    tab.on("activate", function (tab) {
                        #{MenuPanel1}.setSelection(tab.menuItem);
                    });
                }
                tabPanel.setActiveTab(tab);
            }
        </script>
    </ext:XScript>
</head>
<body>
    <form runat="server">
        <ext:ResourceManager runat="server"  />
        <ext:Viewport Layout="VBoxLayout" runat="server">
            <Defaults>
                <ext:Parameter Name="margins" Value="0 0 0 0" Mode="Value" />
            </Defaults>
            <LayoutConfig>
                <ext:VBoxLayoutConfig Align="Stretch" />
            </LayoutConfig>
            <Content>
            <ext:Panel Flex="1" runat="server" Title="Mario服务管理平台V2.0" Icon="Controller" Layout="BorderLayout" ID="panelMain">
                <Items>
                    <ext:MenuPanel  ID="MenuPanel1" runat="server" Width="220" Region="West" AutoScroll="true" Collapsible="true" Title="可折叠功能菜单" >
                        <Menu id="Menu1" runat="server" >
                            <Items>
                                <ext:MenuItem ID="MenuItemSummaryReport" runat="server" Text="查看APP项目汇总报表" Icon="ReportMagnify">
                                    <Listeners>
                                        <Click Handler="addTab(#{TabPanel1}, 'tabPageSummaryReport','查看APP项目汇总报表', 'ViewAppProjectsSummaryReport.aspx', this);" />
                                    </Listeners>
                                </ext:MenuItem> 
                                <ext:MenuItem ID="MenuItemSingleReport" runat="server" Text="查看单个APP项目报表" Icon="ChartLine">
                                    <Listeners>
                                        <Click Handler="addTab(#{TabPanel1}, 'tabPageSingleReport','查看APP项目报表', 'ViewDayReport.aspx', this);" />
                                    </Listeners>
                                </ext:MenuItem>                                           
                                <ext:MenuItem ID="MenuItem2" runat="server" Text="修改个人密码" Icon="Key">
                                    <Listeners>
                                        <Click Handler="addTab(#{TabPanel1}, 'tabPage2', '修改个人密码','ChangePassword.aspx', this);" />
                                    </Listeners>
                                </ext:MenuItem>

                                <ext:MenuSeparator ID="speartor_1" />

                                <ext:MenuItem runat="server" ID="MenuItem3"  Text="管理APP项目及操作方案" Icon="ChartOrganisation">
                                    <Listeners>
                                        <Click Handler="addTab(#{TabPanel1}, 'tabPage3', '管理APP项目及操作方案','ManageAppProjects.aspx', this);" />
                                    </Listeners>
                                </ext:MenuItem>
                                <ext:MenuItem runat="server" ID="MenuItem4"  Text="查看APP项目今天的任务" Icon="TimelineMarker">
                                    <Listeners>
                                        <Click Handler="addTab(#{TabPanel1}, 'tabPage4', '查看APP项目今天的任务','ViewTodayTasks.aspx', this);" />
                                    </Listeners>
                                </ext:MenuItem>
                                <ext:MenuItem runat="server" ID="MenuItem5"  Text="管理移动设备及承载应用" Icon="PhoneAdd">
                                    <Listeners>
                                        <Click Handler="addTab(#{TabPanel1}, 'tabPage5', '管理移动设备及承载应用','ManageMobileDevices.aspx', this);" />
                                    </Listeners>
                                </ext:MenuItem>
                                <ext:MenuItem runat="server" ID="MenuItem6" Text="管理虚拟移动设备型号" Icon="Phone" >
                                    <Listeners>
                                        <Click Handler="addTab(#{TabPanel1}, 'tabPage6', '管理虚拟移动设备型号','ManageMobileDeviceModels.aspx', this);" />
                                    </Listeners>
                                </ext:MenuItem>
                                <ext:MenuItem runat="server" ID="MenuItem7" Text="更新说明" Icon="Date">
                                    <Listeners>
                                        <Click Handler="addTab(#{TabPanel1}, 'tabPage7', '算法说明','History.html', this);" />
                                    </Listeners>
                                </ext:MenuItem>
                                <ext:MenuItem runat="server" ID="MenuItem8" Text="脚本命令说明" Icon="ScriptCode">
                                    <Listeners>
                                        <Click Handler="addTab(#{TabPanel1}, 'tabPage8', '脚本命令说明','CommandGuide.html', this);" />
                                    </Listeners>
                                </ext:MenuItem>
                                <ext:MenuItem runat="server" ID="MenuItem9" Text="留存算法说明" Icon="ChartCurve">
                                    <Listeners>
                                        <Click Handler="addTab(#{TabPanel1}, 'tabPage9', '留存算法说明','ServerHelp.html', this);" />
                                    </Listeners>
                                </ext:MenuItem>

                                <ext:MenuSeparator ID="speartor_2" />

                                <ext:MenuItem runat="server" ID="MenuItem10" Text="用户管理" Icon="UserKey" >
                                    <Listeners>
                                        <Click Handler="addTab(#{TabPanel1}, 'tabPage10', '用户管理','ManageUserInfos.aspx', this);" />
                                    </Listeners>
                                </ext:MenuItem>
                                <ext:MenuItem runat="server" ID="MenuItem11" Text="角色权限管理" Icon="GroupKey">
                                    <Listeners>
                                        <Click Handler="addTab(#{TabPanel1}, 'tabPage11', '角色权限管理','ManageRoles.aspx', this);" />
                                    </Listeners>
                                </ext:MenuItem>


                                <ext:MenuItem runat="server" ID="MenuItem12" Text="Mario客户端版本管理" Icon="ApplicationHome">
                                    <Listeners>
                                        <Click Handler="addTab(#{TabPanel1}, 'tabPage12', 'Mario移动客户端版本管理','ManageMarioPackages.aspx', this);" />
                                    </Listeners>
                                </ext:MenuItem>

                                <ext:MenuItem runat="server" ID="MenuItem13" Text="查看系统日志" Icon="PageMagnify" >
                                    <Listeners>
                                        <Click Handler="addTab(#{TabPanel1}, 'tabPage13', '查看系统日志','ViewSystemLogs.aspx', this);" />
                                    </Listeners>
                                </ext:MenuItem>

                                <ext:MenuItem runat="server" ID="MenuItem14" Text="查看移动端日志" Icon="PhoneError">
                                    <Listeners>
                                        <Click Handler="addTab(#{TabPanel1}, 'tabPage14', '查看移动端日志','ViewMobileDevicesLogs.aspx', this);" />
                                    </Listeners>
                                </ext:MenuItem>

                                <ext:MenuItem runat="server" ID="MenuItem15" Text="注销" Icon="DoorOut">
                                    <DirectEvents>
                                        <Click OnEvent="logout_Click">
                                            <Confirmation ConfirmRequest="true" Title="系统提示" Message="确定注销吗？" />
                                        </Click>
                                    </DirectEvents>
                                </ext:MenuItem>
                                
                                <ext:MenuSeparator />
                            </Items>
                        </Menu>
                    </ext:MenuPanel>
                    <ext:TabPanel Border="false" ID="TabPanel1" runat="server" Region="Center">
                        <Plugins>
                            <ext:TabCloseMenu CloseTabText="关闭" CloseAllTabsText="关闭所有页" CloseOthersTabsText="除此之外其他全部关闭"></ext:TabCloseMenu>
                        </Plugins>
                    </ext:TabPanel>
                </Items>
            </ext:Panel>
            </Content>
        </ext:Viewport>
    </form>
</body>
</html>
