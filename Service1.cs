using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Management;

namespace WindowsService_tutorial
{
    public class Logger
    {
        private string logName;
        private string sourceName;

        public Logger(string logName, string sourceName)
        {
            this.logName = logName;
            this.sourceName = sourceName;

            // 檢查日誌是否存在，如果不存在則創建
            if (!EventLog.SourceExists(sourceName))
            {
                EventLog.CreateEventSource(sourceName, logName);
            }
        }

        public void WriteLogEntry(string message, EventLogEntryType eventType = EventLogEntryType.Information, int eventID = 0)
        {
            // 寫入事件日誌
            using (EventLog eventLog = new EventLog(logName))
            {
                eventLog.Source = sourceName;

                // 寫入事件
                eventLog.WriteEntry(message, eventType, eventID);
            }
        }
    }
    public partial class Service1 : ServiceBase
    {
        private FileSystemWatcher file_watcher = new FileSystemWatcher();
        private ManagementEventWatcher watcher;
        private Logger logger = new Logger("MyApplicationLog", "MyApplication");
        public Service1()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is started at " + DateTime.Now);

            string query = "SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2";
            watcher = new ManagementEventWatcher(query);

            watcher.EventArrived += (sender, e) =>
            {
                foreach (PropertyData pd in e.NewEvent.Properties)
                {
                    if (pd.Name == "DriveName")
                    {
                        DriveInfo[] drives = DriveInfo.GetDrives();
                        foreach (DriveInfo drive in drives)
                        {
                            if (drive.DriveType == DriveType.Removable && Directory.Exists(drive.RootDirectory.FullName))
                            {
                                string path = drive.RootDirectory.FullName;
                                FileWatcherStart(path);
                                WriteToFile("Root Directory: " + drive.RootDirectory.FullName);
                            }
                        }
                    }
                }
            };
            watcher.Start();
        }

        public void FileWatcherStart(string path)
        {
            file_watcher.EnableRaisingEvents = false;
            file_watcher.Deleted -= OnDeleted;
            file_watcher.Changed -= OnChanged;
            file_watcher.Renamed -= OnRenamed;

            file_watcher.Path = path;

            file_watcher.Deleted += OnDeleted;
            file_watcher.Changed += OnChanged;
            file_watcher.Renamed += OnRenamed;

            file_watcher.IncludeSubdirectories = true;
            file_watcher.EnableRaisingEvents = true;
        }
        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.Now);
        }
        string previous_message = "";
        public void WriteToFile(string Message)
        {
            if (Message == previous_message) return;
            logger.WriteLogEntry(Message, EventLogEntryType.Information, 1001);
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            previous_message = Message;
        }
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            WriteToFile($"Changed: {e.FullPath}");
        }
        private void OnDeleted(object sender, FileSystemEventArgs e) =>
            WriteToFile($"Deleted: {e.FullPath}");
        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            WriteToFile($"Renamed: Old: {e.OldFullPath} => New: {e.FullPath}");
        }
    }
}
