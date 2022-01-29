using DS4Windows;
using DS4WinWPF.DS4Forms.ViewModels.Util;
using System;
using System.Collections.Generic;
using JoinedGyroProvider = DS4Windows.JoyConDeviceOptions.JoinedGyroProvider;
using LEDBarMode = DS4Windows.DualSenseControllerOptions.LEDBarMode;
using LinkMode = DS4Windows.JoyConDeviceOptions.LinkMode;
using MuteLEDMode = DS4Windows.DualSenseControllerOptions.MuteLEDMode;

namespace DS4WinWPF.DS4Forms.ViewModels
{
    public class ControllerRegDeviceOptsViewModel
    {
        private readonly ControlServiceDeviceOptions serviceDeviceOpts;

        public bool EnableDS4 { get => serviceDeviceOpts.DS4DeviceOpts.Enabled; }

        public bool EnableDualSense { get => serviceDeviceOpts.DualSenseOpts.Enabled; }

        public bool EnableSwitchPro { get => serviceDeviceOpts.SwitchProDeviceOpts.Enabled; }

        public bool EnableJoyCon { get => serviceDeviceOpts.JoyConDeviceOpts.Enabled; }

        public DS4DeviceOptions DS4DeviceOpts { get => serviceDeviceOpts.DS4DeviceOpts; }
        public DualSenseDeviceOptions DSDeviceOpts { get => serviceDeviceOpts.DualSenseOpts; }
        public SwitchProDeviceOptions SwitchProDeviceOpts { get => serviceDeviceOpts.SwitchProDeviceOpts; }
        public JoyConDeviceOptions JoyConDeviceOpts { get => serviceDeviceOpts.JoyConDeviceOpts; }

        public bool VerboseLogMessages { get => serviceDeviceOpts.VerboseLogMessages; set => serviceDeviceOpts.VerboseLogMessages = value; }
        public List<DeviceListItem> CurrentInputDevices { get; } = new List<DeviceListItem>();

        // Serial, ControllerOptionsStore instance
        private readonly Dictionary<string, ControllerOptionsStore> inputDeviceSettings = new();
        private readonly List<ControllerOptionsStore> controllerOptionsStores = new();

        private int controllerSelectedIndex = -1;
        public int ControllerSelectedIndex
        {
            get => controllerSelectedIndex;
            set
            {
                if (controllerSelectedIndex == value)
                {
                    return;
                }

                controllerSelectedIndex = value;
                ControllerSelectedIndexChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler ControllerSelectedIndexChanged;

        public DS4ControllerOptions CurrentDS4Options
        {
            get => controllerOptionsStores[controllerSelectedIndex] as DS4ControllerOptions;
        }

        public DualSenseControllerOptions CurrentDSOptions
        {
            get => controllerOptionsStores[controllerSelectedIndex] as DualSenseControllerOptions;
        }

        public SwitchProControllerOptions CurrentSwitchProOptions
        {
            get => controllerOptionsStores[controllerSelectedIndex] as SwitchProControllerOptions;
        }

        public JoyConControllerOptions CurrentJoyConOptions
        {
            get => controllerOptionsStores[controllerSelectedIndex] as JoyConControllerOptions;
        }

        private int currentTabSelectedIndex = 0;
        public int CurrentTabSelectedIndex
        {
            get => currentTabSelectedIndex;
            set
            {
                if (currentTabSelectedIndex == value)
                {
                    return;
                }

                currentTabSelectedIndex = value;
                CurrentTabSelectedIndexChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler CurrentTabSelectedIndexChanged;

        public ControllerRegDeviceOptsViewModel(ControlServiceDeviceOptions serviceDeviceOpts,
            ControlService service)
        {
            this.serviceDeviceOpts = serviceDeviceOpts;

            int idx = 0;
            foreach (DS4Device device in service.DS4Controllers)
            {
                if (device != null)
                {
                    CurrentInputDevices.Add(new DeviceListItem(device));
                    inputDeviceSettings.Add(device.MacAddress, device.optionsStore);
                    controllerOptionsStores.Add(device.optionsStore);
                }
                idx++;
            }
        }

        private object dataContextObject = null;
        public object DataContextObject { get => dataContextObject; }

        public int FindTabOptionsIndex()
        {
            ControllerOptionsStore currentStore =
                controllerOptionsStores[controllerSelectedIndex];

            int result = 0;
            switch (currentStore.DeviceType)
            {
                case DS4Windows.InputDevices.InputDeviceType.DS4:
                    result = 1;
                    break;
                case DS4Windows.InputDevices.InputDeviceType.DualSense:
                    result = 2;
                    break;
                case DS4Windows.InputDevices.InputDeviceType.SwitchPro:
                    result = 3;
                    break;
                case DS4Windows.InputDevices.InputDeviceType.JoyConL:
                case DS4Windows.InputDevices.InputDeviceType.JoyConR:
                    result = 4;
                    break;
                default:
                    break;
            }

            return result;
        }

        public void FindFittingDataContext()
        {
            ControllerOptionsStore currentStore =
                controllerOptionsStores[controllerSelectedIndex];

            switch (currentStore.DeviceType)
            {
                case DS4Windows.InputDevices.InputDeviceType.DS4:
                    dataContextObject = new DS4ControllerOptionsWrapper(CurrentDS4Options, serviceDeviceOpts.DS4DeviceOpts);
                    break;
                case DS4Windows.InputDevices.InputDeviceType.DualSense:
                    dataContextObject = new DualSenseControllerOptionsWrapper(CurrentDSOptions, serviceDeviceOpts.DualSenseOpts);
                    break;
                case DS4Windows.InputDevices.InputDeviceType.SwitchPro:
                    dataContextObject = new SwitchProControllerOptionsWrapper(CurrentSwitchProOptions, serviceDeviceOpts.SwitchProDeviceOpts);
                    break;
                case DS4Windows.InputDevices.InputDeviceType.JoyConL:
                case DS4Windows.InputDevices.InputDeviceType.JoyConR:
                    dataContextObject = new JoyConControllerOptionsWrapper(CurrentJoyConOptions, serviceDeviceOpts.JoyConDeviceOpts);
                    break;
                default:
                    break;
            }
        }

        public void SaveControllerConfigs()
        {
            foreach (DeviceListItem item in CurrentInputDevices)
            {
                Global.SaveControllerConfigs(item.Device);
            }
        }
    }

    public class DeviceListItem
    {
        private readonly DS4Device device;
        public DS4Device Device { get => device; }

        public string IdText
        {
            get => $"{device.DisplayName} ({device.MacAddress})";
        }

        public DeviceListItem(DS4Device device)
        {
            this.device = device;
        }
    }


    public class DS4ControllerOptionsWrapper
    {
        private readonly DS4ControllerOptions options;
        public DS4ControllerOptions Options { get => options; }

        private readonly DS4DeviceOptions parentOptions;
        public bool Visible
        {
            get => parentOptions.Enabled;
        }
        public event EventHandler VisibleChanged;

        public DS4ControllerOptionsWrapper(DS4ControllerOptions options, DS4DeviceOptions parentOpts)
        {
            this.options = options;
            this.parentOptions = parentOpts;
            parentOptions.EnabledChanged += (sender, e) => { VisibleChanged?.Invoke(this, EventArgs.Empty); };
        }
    }

    public class DualSenseControllerOptionsWrapper
    {
        private readonly DualSenseControllerOptions options;
        public DualSenseControllerOptions Options { get => options; }

        private readonly DualSenseDeviceOptions parentOptions;
        public bool Visible { get => parentOptions.Enabled; }
        public event EventHandler VisibleChanged;

        public List<DSHapticsChoiceEnum> DSHapticOptions { get; } = new List<DSHapticsChoiceEnum>()
        {
            new DSHapticsChoiceEnum("Low", DS4Windows.InputDevices.DualSenseDevice.HapticIntensity.Low),
            new DSHapticsChoiceEnum("Medium", DS4Windows.InputDevices.DualSenseDevice.HapticIntensity.Medium),
            new DSHapticsChoiceEnum("High", DS4Windows.InputDevices.DualSenseDevice.HapticIntensity.High)
        };
        public List<EnumChoiceSelection<LEDBarMode>> DsLEDModes { get; } = new List<EnumChoiceSelection<LEDBarMode>>()
        {
            new EnumChoiceSelection<LEDBarMode>("Off", LEDBarMode.Off),
            new EnumChoiceSelection<LEDBarMode>("Only for multiple controllers", LEDBarMode.MultipleControllers),
            new EnumChoiceSelection<LEDBarMode>("Battery Percentage", LEDBarMode.BatteryPercentage),
            new EnumChoiceSelection<LEDBarMode>("On", LEDBarMode.On),
        };
        public List<EnumChoiceSelection<MuteLEDMode>> DsMuteLEDModes { get; } = new List<EnumChoiceSelection<MuteLEDMode>>()
        {
            new EnumChoiceSelection<MuteLEDMode>("Off", MuteLEDMode.Off),
            new EnumChoiceSelection<MuteLEDMode>("On", MuteLEDMode.On),
            new EnumChoiceSelection<MuteLEDMode>("Pulse", MuteLEDMode.Pulse),
        };

        public DualSenseControllerOptionsWrapper(DualSenseControllerOptions options,
            DualSenseDeviceOptions parentOpts)
        {
            this.options = options;
            this.parentOptions = parentOpts;
            parentOptions.EnabledChanged += (sender, e) => { VisibleChanged?.Invoke(this, EventArgs.Empty); };
        }
    }

    public class SwitchProControllerOptionsWrapper
    {
        private readonly SwitchProControllerOptions options;
        public SwitchProControllerOptions Options { get => options; }

        private readonly SwitchProDeviceOptions parentOptions;
        public bool Visible { get => parentOptions.Enabled; }
        public event EventHandler VisibleChanged;

        public SwitchProControllerOptionsWrapper(SwitchProControllerOptions options,
            SwitchProDeviceOptions parentOpts)
        {
            this.options = options;
            this.parentOptions = parentOpts;
            parentOptions.EnabledChanged += (sender, e) => { VisibleChanged?.Invoke(this, EventArgs.Empty); };
        }
    }

    public class JoyConControllerOptionsWrapper
    {
        private readonly JoyConControllerOptions options;
        public JoyConControllerOptions Options { get => options; }

        private readonly JoyConDeviceOptions parentOptions;
        public JoyConDeviceOptions ParentOptions { get => parentOptions; }

        public bool Visible { get => parentOptions.Enabled; }
        public event EventHandler VisibleChanged;

        private readonly List<EnumChoiceSelection<LinkMode>> linkModes = new()
        {
            new EnumChoiceSelection<LinkMode>("Split", LinkMode.Split),
            new EnumChoiceSelection<LinkMode>("Joined", LinkMode.Joined),
        };
        public List<EnumChoiceSelection<LinkMode>> LinkModes { get => linkModes; }
        public List<EnumChoiceSelection<JoinedGyroProvider>> JoinGyroOptions { get; } = new List<EnumChoiceSelection<JoinedGyroProvider>>()
        {
            new EnumChoiceSelection<JoinedGyroProvider>("Left", JoinedGyroProvider.JoyConL),
            new EnumChoiceSelection<JoinedGyroProvider>("Right", JoinedGyroProvider.JoyConR),
        };

        public JoyConControllerOptionsWrapper(JoyConControllerOptions options,
            JoyConDeviceOptions parentOpts)
        {
            this.options = options;
            this.parentOptions = parentOpts;
            parentOptions.EnabledChanged += (sender, e) => { VisibleChanged?.Invoke(this, EventArgs.Empty); };
        }
    }

    public class DSHapticsChoiceEnum
    {
        private readonly string displayName = string.Empty;
        public string DisplayName { get => displayName; }

        private DS4Windows.InputDevices.DualSenseDevice.HapticIntensity choiceValue;
        public DS4Windows.InputDevices.DualSenseDevice.HapticIntensity ChoiceValue
        {
            get => choiceValue;
            set => choiceValue = value;
        }

        public DSHapticsChoiceEnum(string name,
            DS4Windows.InputDevices.DualSenseDevice.HapticIntensity intensity)
        {
            displayName = name;
            choiceValue = intensity;
        }

        public override string ToString()
        {
            return displayName;
        }
    }
}
