
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Mbk.Business.Interfaces;
using Mbk.Enums;
using Mbk.Model;
using Mbk.Wpf.Messages;
using Mbk.Wpf.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Mbk.Wpf.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Commands
        private ICommand _onLoadCommand;
        public ICommand OnLoadCommand
        {
            get
            {
                return _onLoadCommand ?? (_onLoadCommand = new RelayCommand(() =>
                {
                    try
                    {
                        ReportDate = DateTime.Today;
                        LoadConfig();
                    }
                    catch (Exception ex)
                    {
                        Logs.Insert(0, new SystemLogViewModel { LogDate = DateTime.Now, Description = ex.Message });
                    }
                }));
            }
        }

        private ICommand _openConfigCommand;
        public ICommand OpenConfigCommand
        {
            get
            {
                return _openConfigCommand ??
                  (_openConfigCommand = new RelayCommand(() =>
                  {
                      Messenger.Default.Send(new OpenWindowNotificationMessage("Open Config", WindowType.Config));
                  }));
            }
        }

        private ICommand _checkSystemCommand;
        public ICommand CheckSystemCommand
        {
            get
            {
                return _checkSystemCommand ??
                  (_checkSystemCommand = new RelayCommand(async () =>
                 {
                     try
                     {
                         IsLoading = true;

                         string checkResult = await _cameraManager.CheckCameraInSystemAsync();
                         Logs.Insert(0, new SystemLogViewModel { LogDate = DateTime.Now, Description = checkResult });
                     }
                     catch (Exception ex)
                     {
                         Logs.Insert(0, new SystemLogViewModel { LogDate = DateTime.Now, Description = ex.Message });
                     }
                     finally
                     {
                         IsLoading = false;
                     }
                 }));
            }
        }

        private ICommand _manualGetDataCommand;
        public ICommand ManualGetDataCommand
        {
            get
            {
                return _manualGetDataCommand ?? (_manualGetDataCommand = new RelayCommand(async () =>
               {
                   try
                   {
                       IsLoading = true;

                       var cameras = await _cameraManager.GetCameraListAsync();
                       int totalCamera = 0;

                       IsShowProgress = true;
                       ProgressValue = 0;
                       ProgressMax = cameras.Count;
                       ProgressText = string.Empty;

                       var config = _configManager.GetConfig();
                       for (int i = 0; i < cameras.Count; i++)
                       {
                           var cam = cameras[i];
                           try
                           {
                               ProgressText = $"Get data from camera: {cam.Name} ({i + 1}/{cameras.Count})";
                               await _dataManager.CollectDataAsync(config, cam);
                               totalCamera++;
                           }
                           catch (Exception)
                           {
                               continue;
                           }
                           finally
                           {
                               ProgressValue++;
                           }
                       }
                       Logs.Insert(0, new SystemLogViewModel { LogDate = DateTime.Now, Description = $"Get data finish {totalCamera} camera(s)" });
                   }
                   catch (Exception ex)
                   {
                       Logs.Insert(0, new SystemLogViewModel { LogDate = DateTime.Now, Description = ex.Message });
                   }
                   finally
                   {
                       IsShowProgress = false;
                       IsLoading = false;
                   }
               }, () => !string.IsNullOrEmpty(BufferLocation)));
            }
        }

        private ICommand _exportReportCommand;
        public ICommand ExportReportCommand
        {
            get
            {
                return _exportReportCommand ??
                  (_exportReportCommand = new RelayCommand(async () =>
                  {
                      try
                      {
                          IsLoading = true;
                          int totalCamera = await _reportManager.GenerateDataReportAsync(ReportLocation, ReportDate, ReportPeriod);
                          Logs.Insert(0, new SystemLogViewModel { LogDate = DateTime.Now, Description = $"Export data finish {totalCamera} camera(s)" });
                      }
                      catch (Exception ex)
                      {
                          Logs.Insert(0, new SystemLogViewModel { LogDate = DateTime.Now, Description = ex.Message });
                      }
                      finally
                      {
                          IsLoading = false;
                      }

                  }, () => !string.IsNullOrEmpty(ReportLocation)));
            }
        }
        #endregion

        #region Properties
        private ICameraManager _cameraManager;
        private IReportManager _reportManager;
        private IConfigManager _configManager;
        private IDataManager _dataManager;

        private string _bufferLocation;
        public string BufferLocation
        {
            get { return _bufferLocation; }
            set { Set(() => BufferLocation, ref _bufferLocation, value); }
        }

        private DateTime _reportDate;
        public DateTime ReportDate
        {
            get { return _reportDate; }
            set { Set(() => ReportDate, ref _reportDate, value); }
        }

        private ReportPeriodType _reportPeriod;
        public ReportPeriodType ReportPeriod
        {
            get { return _reportPeriod; }
            set { Set(() => ReportPeriod, ref _reportPeriod, value); }
        }

        private string _reportLocation;
        public string ReportLocation
        {
            get { return _reportLocation; }
            set { Set(() => ReportLocation, ref _reportLocation, value); }
        }

        public ObservableCollection<SystemLogViewModel> Logs { get; set; }
        public List<SelectItem> TimeList { get; set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(() => IsLoading, ref _isLoading, value); }
        }

        private bool _isShowProgress;
        public bool IsShowProgress
        {
            get { return _isShowProgress; }
            set { Set(() => IsShowProgress, ref _isShowProgress, value); }
        }

        private int _progressValue;
        public int ProgressValue
        {
            get { return _progressValue; }
            set { Set(() => ProgressValue, ref _progressValue, value); }
        }

        private int _progressMax;
        public int ProgressMax
        {
            get { return _progressMax; }
            set { Set(() => ProgressMax, ref _progressMax, value); }
        }

        private string _progressText;
        public string ProgressText
        {
            get { return _progressText; }
            set { Set(() => ProgressText, ref _progressText, value); }
        }
        #endregion

        #region Constructor
        public MainViewModel(
            ICameraManager cameraManager,
            IReportManager reportManager,
            IConfigManager configManager,
            IDataManager dataManager)
        {
            _cameraManager = cameraManager;
            _reportManager = reportManager;
            _configManager = configManager;
            _dataManager = dataManager;

            Logs = new ObservableCollection<SystemLogViewModel>();
            TimeList = new List<SelectItem>
            {
                new SelectItem { Value = ReportPeriodType.M15, Display = "15 Minute" },
                new SelectItem { Value = ReportPeriodType.M30, Display = "30 Minute" },
                new SelectItem { Value = ReportPeriodType.H1, Display = "1 Hour" },
            };

            Messenger.Default.Register<DataChangedNotificationMessage>(this, (msg) =>
            {
                if (msg.DataChanged == DataChangedType.Config)
                {
                    LoadConfig();
                }
            });
        }
        #endregion

        #region Private Method
        private void LoadConfig()
        {
            ConfigModel config = _configManager.GetConfig();
            ReportLocation = config.ExportConfig.Location;
            BufferLocation = config.DataConfig.Location;
        }
        #endregion
    }

    public class SelectItem
    {
        public object Value { get; set; }
        public string Display { get; set; }
    }

    public class SystemLogViewModel : ObservableObject
    {
        private DateTime _logDate;
        public DateTime LogDate
        {
            get { return _logDate; }
            set { Set(() => LogDate, ref _logDate, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { Set(() => Description, ref _description, value); }
        }
    }
}