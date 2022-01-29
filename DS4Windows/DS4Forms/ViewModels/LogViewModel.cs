using DS4Windows;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Data;

namespace DS4WinWPF.DS4Forms.ViewModels
{
    public class LogViewModel
    {
        public ObservableCollection<LogItem> LogItems { get; } = new ObservableCollection<LogItem>();

        public ReaderWriterLockSlim LogListLocker { get; } = new ReaderWriterLockSlim();

        public LogViewModel(DS4Windows.ControlService service)
        {
            string version = DS4Windows.Global.exeversion;
            LogItems.Add(new LogItem { Datetime = DateTime.Now, Message = $"DS4Windows version {version}" });
            LogItems.Add(new LogItem { Datetime = DateTime.Now, Message = $"DS4Windows Assembly Architecture: {(Environment.Is64BitProcess ? "x64" : "x86")}" });
            LogItems.Add(new LogItem { Datetime = DateTime.Now, Message = $"OS Version: {Environment.OSVersion}" });
            LogItems.Add(new LogItem { Datetime = DateTime.Now, Message = $"OS Product Name: {DS4Windows.Util.GetOSProductName()}" });
            LogItems.Add(new LogItem { Datetime = DateTime.Now, Message = $"OS Release ID: {DS4Windows.Util.GetOSReleaseId()}" });
            LogItems.Add(new LogItem { Datetime = DateTime.Now, Message = $"System Architecture: {(Environment.Is64BitOperatingSystem ? "x64" : "x32")}" });

            //logItems.Add(new LogItem { Datetime = DateTime.Now, Message = "DS4Windows version 2.0" });
            //BindingOperations.EnableCollectionSynchronization(logItems, _colLockobj);
            BindingOperations.EnableCollectionSynchronization(LogItems, LogListLocker, LogLockCallback);
            service.Debug += AddLogMessage;
            DS4Windows.AppLogger.GuiLog += AddLogMessage;
        }

        private void LogLockCallback(IEnumerable collection, object context, Action accessMethod, bool writeAccess)
        {
            if (writeAccess)
            {
                using (WriteLocker locker = new(LogListLocker))
                {
                    accessMethod?.Invoke();
                }
            }
            else
            {
                using (ReadLocker locker = new(LogListLocker))
                {
                    accessMethod?.Invoke();
                }
            }
        }

        private void AddLogMessage(object sender, DS4Windows.DebugEventArgs e)
        {
            LogItem item = new() { Datetime = e.Time, Message = e.Data, Warning = e.Warning };
            LogListLocker.EnterWriteLock();
            LogItems.Add(item);
            LogListLocker.ExitWriteLock();
            //lock (_colLockobj)
            //{
            //    logItems.Add(item);
            //}
        }
    }
}
