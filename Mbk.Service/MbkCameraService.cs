using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Threading;
using Mbk.Business.Interfaces;
using Mbk.Model;
using Mbk.Business;
using Mbk.Dal.Repositories.Interfaces;
using Mbk.Dal;
using Mbk.Dal.Repositories;

namespace Mbk.Service
{
    public partial class MbkCameraService : ServiceBase
    {
        #region Fields
        private ILog _logger;
        private IConfigManager _configManager;
        private IDataManager _dataManager;
        private IReportManager _reportManager;

        private Timer _dataTimer;
        private Timer _reportTimer;
        private ConfigModel _config;
        #endregion

        #region Constructor
        public MbkCameraService()
        {
            InitializeComponent();

            _configManager = new ConfigManager();
            string connectionString = _configManager.GetConfig().DatabaseSource;

            ICameraRepository cameraRepository = new CameraRepository(connectionString);
            IHeatMapRepository heatMapRepository = new HeatMapRepository(connectionString);
            ICountingRepository countingRepository = new CountingRepository(connectionString);
            _dataManager = new DataManager(_configManager, cameraRepository, heatMapRepository, countingRepository);

            IReportRepository reportRepository = new ReportRepository(connectionString);
            _reportManager = new ReportManager(reportRepository);
        }
        #endregion

        #region Event handlers
        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("Mbk service is starting...", EventLogEntryType.Information);

            ConfigureLog4Net();
            ConfigureTimers();
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("Mbk service is stopping...", EventLogEntryType.Information);
        }
        #endregion

        #region Private Methods
        private void ConfigureTimers()
        {
            _config = _configManager.GetConfig();

            if (_config.DataConfig.IsEnabled)
            {
                int hour = _config.DataConfig.Hour;
                int minute = _config.DataConfig.Minute;

                _dataTimer = new Timer(DataCollectingTimerCallback, _config.DataConfig, TilNextTime(hour, minute), TimeSpan.FromDays(1));
                _logger.Info($"Auto collect data will start on {hour}:{minute} of everyday.");
            }
            else
            {
                _logger.Info("Auto collect data function is disabled.");
            }

            if (_config.ExportConfig.IsEnabled)
            {
                int hour = _config.ExportConfig.Hour;
                int minute = _config.ExportConfig.Minute;

                _reportTimer = new Timer(ReportTimerCallback, _config.ExportConfig, TilNextTime(18, 30), TimeSpan.FromDays(1));
                _logger.Info($"Auto export will start on {hour}:{minute} of everyday");
            }
            else
            {
                _logger.Info("Auto export function is disabled.");
            }
        }

        private void DataCollectingTimerCallback(object obj)
        {
            ScheduleConfig config = (ScheduleConfig)obj;
            _dataManager.CollectDataAsync(config.Location).Wait();
            _logger.Info($"Data was collected on {DateTime.Now}");
        }

        private void ReportTimerCallback(object obj)
        {
            ScheduleConfig config = (ScheduleConfig)obj;
            _reportManager.GenerateDataReportAsync(config.Location, DateTime.Today, config.Period).Wait();
            _logger.Info($"Report was exported on {DateTime.Now}");
        }

        private void ConfigureLog4Net()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
                _logger = LogManager.GetLogger("servicelog");
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
            }
        }

        private TimeSpan TilNextTime(int hour, int minute)
        {
            DateTime executeTime = DateTime.Today.AddHours(hour).AddMinutes(minute);
            if (executeTime < DateTime.Now)
            {
                executeTime.AddDays(1);
            }
            return (executeTime - DateTime.Now);
        }
        #endregion
    }
}
