using AutoMapper;
using log4net;
using Mbk.Business;
using Mbk.Business.Interfaces;
using Mbk.Dal;
using Mbk.Dal.Repositories;
using Mbk.Dal.Repositories.Interfaces;
using Mbk.Model;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace Mbk.Service
{
    public partial class MbkCameraService : ServiceBase
    {
        #region Fields
        private ILog _logger;
        private IDataManager _dataManager;
        private IReportManager _reportManager;
        private ICameraManager _cameraManager;
        private IConfigManager _configManager;

        private Timer _dataTimer;
        private Timer _reportTimer;
        private ConfigModel _config;
        #endregion

        #region Constructor
        public MbkCameraService()
        {
            InitializeComponent();
        }

        #endregion

        #region Event handlers
        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("Mbk service is starting...", EventLogEntryType.Information);

            Init();
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
            try
            {
                if (_config.DataConfig.IsEnabled)
                {
                    int hour = _config.DataConfig.Hour;
                    int minute = _config.DataConfig.Minute;

                    _dataTimer = new Timer(DataCollectingTimerCallback, _config, TilNextTime(hour, minute), TimeSpan.FromDays(1));
                    _logger.Info($"Auto collect data will start on {hour}:{minute.ToString("00")} of everyday.");
                }
                else
                {
                    _logger.Info("Auto collect data function is disabled.");
                }

                if (_config.ExportConfig.IsEnabled)
                {
                    int hour = _config.ExportConfig.Hour;
                    int minute = _config.ExportConfig.Minute;

                    _reportTimer = new Timer(ReportTimerCallback, _config.ExportConfig, TilNextTime(hour, minute), TimeSpan.FromDays(1));
                    _logger.Info($"Auto export will start on {hour}:{minute.ToString("00")} of everyday");
                }
                else
                {
                    _logger.Info("Auto export function is disabled.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        private void DataCollectingTimerCallback(object obj)
        {
            try
            {
                _configManager.CheckConfig(_config);

                ConfigModel config = (ConfigModel)obj;
                var cameras = _cameraManager.GetCameraListAsync().Result;
                foreach (var cam in cameras)
                {
                    try
                    {
                        _dataManager.CollectDataAsync(config, cam).Wait();
                        _logger.Info($"Get data from IP {cam.IpAddress} is successful.");
                    }
                    catch (AggregateException ex)
                    {
                        var errorMessage = ex.InnerException.Message;
                        _logger.Error($"Cannot get data from IP {cam.IpAddress}: {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        private void ReportTimerCallback(object obj)
        {
            try
            {
                _configManager.CheckConfig(_config);

                ScheduleConfigModel config = (ScheduleConfigModel)obj;

                var reportDate = DateTime.Now.AddHours(-7).Date;
                int totalCamera = _reportManager.GenerateDataReportAsync(config.Location, reportDate, config.Period).Result;
                if (totalCamera > 0)
                {
                    _logger.Info($"Report for {reportDate.ToString("dd/MM/yyyy")} was created successful for {totalCamera} camera(s)");
                }
                else
                {
                    _logger.Info("No data for create report");
                }
            }
            catch (AggregateException ex)
            {
                var errorMessage = ex.InnerException.Message;
                _logger.Error($"Cannot crete report: {errorMessage}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
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
                executeTime = executeTime.AddDays(1);
            }
            return executeTime.Subtract(DateTime.Now);
        }

        private void Init()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<ModelProfile>();
            });

            _configManager = new ConfigManager();
            _config = _configManager.GetConfig();

            string connectionString = _config.DatabaseSource;

            IHeatMapRepository heatMapRepository = new HeatMapRepository(connectionString);
            ICountingRepository countingRepository = new CountingRepository(connectionString);
            _dataManager = new DataManager(heatMapRepository, countingRepository);

            IReportRepository reportRepository = new ReportRepository(connectionString);
            _reportManager = new ReportManager(reportRepository);

            ICameraRepository cameraRepository = new CameraRepository(connectionString);
            _cameraManager = new CameraManager(cameraRepository);
        }

        #endregion
    }
}
