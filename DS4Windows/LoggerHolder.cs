﻿using NLog;
using NLog.Targets.Wrappers;

namespace DS4WinWPF
{
    public class LoggerHolder
    {
        private readonly Logger logger;// = LogManager.GetCurrentClassLogger();
        public Logger Logger { get => logger; }

        public LoggerHolder(DS4Windows.ControlService service)
        {
            var configuration = LogManager.Configuration;
            var wrapTarget = configuration.FindTargetByName<WrapperTargetBase>("logfile");
            var fileTarget = wrapTarget.WrappedTarget as NLog.Targets.FileTarget;
            fileTarget.FileName = $@"{DS4Windows.Global.appdatapath}\Logs\ds4windows_log.txt";
            fileTarget.ArchiveFileName = $@"{DS4Windows.Global.appdatapath}\Logs\ds4windows_log_{{#}}.txt";
            LogManager.Configuration = configuration;
            LogManager.ReconfigExistingLoggers();

            logger = LogManager.GetCurrentClassLogger();

            service.Debug += WriteToLog;
            DS4Windows.AppLogger.GuiLog += WriteToLog;
        }

        private void WriteToLog(object sender, DS4Windows.DebugEventArgs e)
        {
            if (e.Temporary)
            {
                return;
            }

            if (!e.Warning)
            {
                logger.Info(e.Data);
            }
            else
            {
                logger.Warn(e.Data);
            }
        }
    }
}
