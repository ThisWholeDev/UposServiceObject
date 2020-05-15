using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Microsoft.Win32;
using System.Collections.Generic;

namespace Upos.ServiceObject
{
    public class Logger
    {
        private static bool _isSet;

        private const string _loggerEnabledKey = "LOGGERENABLED";
        private const string _logLocation = "LOGLOCATION";

        private PatternLayout _layout = new PatternLayout();
        private const string LOG_PATTERN = "%date [%thread] %level %logger - %message%newline";

        public string DefaultPattern => LOG_PATTERN;

        public Logger()
        {
            _layout.ConversionPattern = DefaultPattern;
            _layout.ActivateOptions();
        }

        public PatternLayout DefaultLayout => _layout;

        public void AddAppender(IAppender appender)
        {
            Hierarchy hierarchy =
                (Hierarchy) LogManager.GetRepository();

            hierarchy.Root.AddAppender(appender);
        }

        static Logger()
        {
            const string regBaseName = "SOFTWARE\\OLEforRetail\\ServiceOPOS\\CashDrawer\\PosDrawer";
            var reg = Registry.LocalMachine.OpenSubKey(regBaseName);

            var registryKeys = GetKeys(reg);

            var logLevel = Level.Info;
            if (!registryKeys.TryGetValue(_loggerEnabledKey, out var isEnabled)
                || !bool.Parse((string)isEnabled))
            {
                logLevel = Level.Off;
            }

            var hierarchy = (Hierarchy) LogManager.GetRepository();
            var tracer = new TraceAppender();
            var patternLayout = new PatternLayout {ConversionPattern = LOG_PATTERN};

            patternLayout.ActivateOptions();

            tracer.Layout = patternLayout;
            tracer.ActivateOptions();
            hierarchy.Root.AddAppender(tracer);

            RollingFileAppender roller = new RollingFileAppender
                                         {
                                             Layout = patternLayout,
                                             AppendToFile = true,
                                             RollingStyle = RollingFileAppender.RollingMode.Size,
                                             MaxSizeRollBackups = 4,
                                             MaximumFileSize = "100KB",
                                             StaticLogFileName = true,
                                             File = "MsCashDrawerLog.txt"
                                         };
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            hierarchy.Root.Level = logLevel;
            hierarchy.Configured = true;
        }

        public static ILog Create(string typeName)
        {
            return LogManager.GetLogger(typeName);
        }

        private static Dictionary<string, object> GetKeys(RegistryKey reg)
        {
            var dict = new Dictionary<string, object>();

            foreach (var valueName in reg.GetValueNames())
            {
                var key = valueName.ToUpper();
                var value = reg.GetValue(valueName);
                dict.Add(key, value);
            }

            return dict;
        }
    }
}
