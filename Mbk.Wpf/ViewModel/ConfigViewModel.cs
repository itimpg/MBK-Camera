using AutoMapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Mbk.Business.Interfaces;
using Mbk.Enums;
using Mbk.Model;
using Mbk.Wpf.Messages;
using Mbk.Wpf.Models;
using Mbk.Wpf.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mbk.Wpf.ViewModel
{
    public class ConfigViewModel : ViewModelBase
    {
        #region Commands
        private ICommand _onLoadCommand;
        public ICommand OnLoadCommand
        {
            get
            {
                return _onLoadCommand ??
                  (_onLoadCommand = new RelayCommand(async () =>
                  {
                      var config = _configManager.GetConfig();

                      BufferLocation = config.DataConfig.Location;
                      IsAutoBuffer = config.DataConfig.IsEnabled;
                      BufferHour = config.DataConfig.Hour;
                      BufferMinute = config.DataConfig.Minute;

                      ExportLocation = config.ExportConfig.Location;
                      IsAutoExport = config.ExportConfig.IsEnabled;
                      ExportHour = config.ExportConfig.Hour;
                      ExportMinute = config.ExportConfig.Minute;
                      ExportPeriod = config.ExportConfig.Period;

                      await LoadCameraAsync();
                  }));
            }
        }

        private ICommand _checkConnectionCommand;
        public ICommand CheckConnectionCommand
        {
            get
            {
                return _checkConnectionCommand ??
                    (_checkConnectionCommand = new RelayCommand(async () =>
                    {
                        try
                        {
                            IsLoading = true;
                            await Task.WhenAll(CameraCollection.Select(async camera =>
                            {
                                camera.Status = string.Empty;
                                camera.Status = await _cameraManager.GetCameraStatusAsync(camera.IpAddress);
                            }));
                        }
                        catch (Exception ex)
                        {
                            _dialogService.ShowMessage(ex.Message, "Error");
                        }
                        finally
                        {
                            IsLoading = false;
                        }

                    }));
            }
        }

        private ICommand _browseBufferLocationCommand;
        public ICommand BrowseBufferLocationCommnad
        {
            get
            {
                return _browseBufferLocationCommand ??
                    (_browseBufferLocationCommand = new RelayCommand(() =>
                    {
                        string path;
                        if (_dialogService.BrowseFolder(out path))
                        {
                            BufferLocation = path;
                        }
                    }));
            }
        }

        private ICommand _browseExportLocationCommand;
        public ICommand BrowseExportLocationCommand
        {
            get
            {
                return _browseExportLocationCommand ??
                  (_browseExportLocationCommand = new RelayCommand<string>((location) =>
                  {
                      string path;
                      if (_dialogService.BrowseFolder(out path))
                      {
                          ExportLocation = path;
                      }
                  }));
            }
        }

        private ICommand _editCameraCommand;
        public ICommand EditCameraCommand
        {
            get
            {
                return _editCameraCommand ??
                    (_editCameraCommand = new RelayCommand(() =>
                    {
                        int cameraId = SelectedCamera.Id;
                        var session = AppSessionModel.Instance();
                        session.CameraId = cameraId;
                        Messenger.Default.Send(new OpenWindowNotificationMessage("Open Config", WindowType.Camera));
                    }, () => SelectedCamera != null));
            }
        }

        private ICommand _addCameraCommand;
        public ICommand AddCameraCommand
        {
            get
            {
                return _addCameraCommand ??
                    (_addCameraCommand = new RelayCommand(() =>
                   {
                       var session = AppSessionModel.Instance();
                       session.CameraId = -1;
                       Messenger.Default.Send(new OpenWindowNotificationMessage("Open Config", WindowType.Camera));
                   }, () => CameraCollection.Count < 100));
            }
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new RelayCommand(async () =>
                {
                    try
                    {
                        IsLoading = true;

                        var config = _configManager.GetConfig();

                        config.DataConfig.IsEnabled = IsAutoBuffer;
                        config.DataConfig.Location = BufferLocation;
                        config.DataConfig.Hour = BufferHour;
                        config.DataConfig.Minute = BufferMinute;

                        config.ExportConfig.IsEnabled = IsAutoExport;
                        config.ExportConfig.Location = ExportLocation;
                        config.ExportConfig.Hour = ExportHour;
                        config.ExportConfig.Minute = ExportMinute;
                        config.ExportConfig.Period = ExportPeriod;

                        _configManager.SaveConfig(config);

                        await Task.Run(() =>
                        {
                            try
                            {
                                string serviceName = config.ServiceName;
                                var service = new ServiceController(serviceName);
                                bool isServiceShouldBeRunning = config.DataConfig.IsEnabled || config.ExportConfig.IsEnabled;
                                if (isServiceShouldBeRunning)
                                {
                                    if (service.Status != ServiceControllerStatus.Stopped)
                                    {
                                        service.Stop();
                                        service.WaitForStatus(ServiceControllerStatus.Stopped);
                                    }

                                    service.Start();
                                    service.WaitForStatus(ServiceControllerStatus.Running);
                                }
                                else
                                {
                                    if (service.Status != ServiceControllerStatus.Stopped)
                                    {
                                        service.Stop();
                                        service.WaitForStatus(ServiceControllerStatus.Stopped);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        });

                        Messenger.Default.Send(new DataChangedNotificationMessage("Config Changed", DataChangedType.Config));
                        Messenger.Default.Send(new CloseWindowNotificationMessage("Closed", WindowType.Config));
                    }
                    catch (Exception ex)
                    {
                        _dialogService.ShowMessage(ex.Message, "Error");
                    }
                    finally
                    {
                        IsLoading = false;
                    }
                }));
            }
        }

        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand(() =>
                {
                    Messenger.Default.Send(new CloseWindowNotificationMessage("Closed", WindowType.Config));
                }));
            }
        }
        #endregion

        #region Properties

        #region Injected Members
        private IDialogService _dialogService;
        private ICameraManager _cameraManager;
        private IConfigManager _configManager;
        #endregion

        #region Init Members
        public ObservableCollection<CameraViewModel> CameraCollection { get; set; }
        public List<SelectItem> HourList { get; set; }
        public List<SelectItem> MinuteList { get; set; }
        public List<SelectItem> TimeList { get; set; }
        #endregion

        #region Buffer Members
        private string _bufferLocation;
        public string BufferLocation
        {
            get { return _bufferLocation; }
            set { Set(() => BufferLocation, ref _bufferLocation, value); }
        }

        private bool _isAutoBuffer;
        public bool IsAutoBuffer
        {
            get { return _isAutoBuffer; }
            set { Set(() => IsAutoBuffer, ref _isAutoBuffer, value); }
        }

        private int _bufferHour;
        public int BufferHour
        {
            get { return _bufferHour; }
            set { Set(() => BufferHour, ref _bufferHour, value); }
        }

        private int _bufferMinute;
        public int BufferMinute
        {
            get { return _bufferMinute; }
            set { Set(() => BufferMinute, ref _bufferMinute, value); }
        }
        #endregion

        #region Export Members
        private string _exportLocation;
        public string ExportLocation
        {
            get { return _exportLocation; }
            set { Set(() => ExportLocation, ref _exportLocation, value); }
        }

        private bool _isAutoExport;
        public bool IsAutoExport
        {
            get { return _isAutoExport; }
            set { Set(() => IsAutoExport, ref _isAutoExport, value); }
        }

        private int _exportHour;
        public int ExportHour
        {
            get { return _exportHour; }
            set { Set(() => ExportHour, ref _exportHour, value); }
        }

        private int _exportMinute;
        public int ExportMinute
        {
            get { return _exportMinute; }
            set { Set(() => ExportMinute, ref _exportMinute, value); }
        }

        private ReportPeriodType _exportPeriod;
        public ReportPeriodType ExportPeriod
        {
            get { return _exportPeriod; }
            set { Set(() => ExportPeriod, ref _exportPeriod, value); }
        }
        #endregion

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(() => IsLoading, ref _isLoading, value); }
        }

        private CameraViewModel _selectedCamera;
        public CameraViewModel SelectedCamera
        {
            get { return _selectedCamera; }
            set { Set(() => SelectedCamera, ref _selectedCamera, value); }
        }

        #endregion

        #region Constructor
        public ConfigViewModel(
            IDialogService dialogService,
            ICameraManager cameraManager,
            IConfigManager configManager)
        {
            _dialogService = dialogService;
            _cameraManager = cameraManager;
            _configManager = configManager;

            CameraCollection = new ObservableCollection<CameraViewModel>();

            HourList = new List<SelectItem>();
            for (int i = 0; i < 24; i++)
            {
                HourList.Add(new SelectItem { Value = i, Display = $"{i} HR" });
            }

            MinuteList = new List<SelectItem>();
            for (int i = 0; i < 60; i++)
            {
                MinuteList.Add(new SelectItem { Value = i, Display = $"{i} MT" });
            }

            TimeList = new List<SelectItem>
            {
                new SelectItem { Value = ReportPeriodType.M15, Display = "15 Minute" },
                new SelectItem { Value = ReportPeriodType.M30, Display = "30 Minute" },
                new SelectItem { Value = ReportPeriodType.H1, Display = "1 Hour" },
            };

            Messenger.Default.Register<DataChangedNotificationMessage>(this, async (msg) =>
            {
                if (msg.DataChanged == DataChangedType.Camera)
                {
                    await LoadCameraAsync();
                }
            });
        }
        #endregion

        #region Private Method
        private async Task LoadCameraAsync()
        {
            try
            {
                IsLoading = true;

                CameraCollection.Clear();
                var cameras = await _cameraManager.GetCameraListAsync();
                int i = 0;
                foreach (var camera in cameras)
                {
                    var cam = Mapper.Map<CameraViewModel>(camera);
                    cam.RowNumber = ++i;
                    CameraCollection.Add(cam);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage(ex.Message, "Error");
            }
            finally
            {
                IsLoading = false;
            }
        }
        #endregion
    }
}
