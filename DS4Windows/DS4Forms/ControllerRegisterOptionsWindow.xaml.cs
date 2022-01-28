using DS4Windows;
using DS4WinWPF.DS4Forms.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DS4WinWPF.DS4Forms
{
    /// <summary>
    /// Interaction logic for ControllerRegisterOptions.xaml
    /// </summary>
    public partial class ControllerRegisterOptionsWindow : Window
    {
        private readonly ControllerRegDeviceOptsViewModel deviceOptsVM;

        public ControllerRegisterOptionsWindow(ControlServiceDeviceOptions deviceOptions, ControlService service)
        {
            InitializeComponent();

            deviceOptsVM = new ControllerRegDeviceOptsViewModel(deviceOptions, service);

            devOptionsDockPanel.DataContext = deviceOptsVM;
            deviceOptsVM.ControllerSelectedIndexChanged += ChangeActiveDeviceTab;
        }

        private void ChangeActiveDeviceTab(object sender, EventArgs e)
        {
            if (deviceSettingsTabControl.SelectedItem is TabItem currentTab)
            {
                currentTab.DataContext = null;
            }

            int tabIdx = deviceOptsVM.FindTabOptionsIndex();
            if (tabIdx >= 0)
            {
                TabItem pendingTab = deviceSettingsTabControl.Items[tabIdx] as TabItem;
                deviceOptsVM.FindFittingDataContext();
                pendingTab.DataContext = deviceOptsVM.DataContextObject;
            }

            deviceOptsVM.CurrentTabSelectedIndex = tabIdx;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            deviceOptsVM.SaveControllerConfigs();
        }
    }
}
