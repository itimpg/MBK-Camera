using AutoMapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Mbk.Business.Interfaces;
using Mbk.Model;
using Mbk.Wpf.Messages;
using Mbk.Wpf.Models;
using Mbk.Wpf.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mbk.Wpf.ViewModel
{
    public class CameraSettingViewModel : ViewModelBase
    {
        #region Commands
        private ICommand _onLoadCommand;
        public ICommand OnLoadCommand
        {
            get
            {
                return _onLoadCommand ?? (_onLoadCommand = new RelayCommand(async () =>
                {
                    try
                    {
                        IsLoading = true;

                        int cameraId = AppSessionModel.Instance().CameraId;
                        Camera camera = null;
                        if (cameraId > 0
                            && (camera = await _cameraManager.GetCameraAsync(cameraId)) != null)
                        {
                            Title = "Edit camera";
                            CameraObject = Mapper.Map<CameraViewModel>(camera);
                        }
                        else
                        {
                            Title = "Add new camera";
                            CameraObject = new CameraViewModel();
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
                }));
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

                        var camera = Mapper.Map<Camera>(CameraObject);
                        await _cameraManager.SaveCameraAsync(camera);

                        Messenger.Default.Send(new DataChangedNotificationMessage("Camera Changed", DataChangedType.Camera));
                        Messenger.Default.Send(new CloseWindowNotificationMessage("Closed", WindowType.Camera));
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
                    Messenger.Default.Send(new CloseWindowNotificationMessage("Closed", WindowType.Camera));
                }));
            }
        }
        #endregion

        #region Properties
        private IDialogService _dialogService;
        private ICameraManager _cameraManager;

        private CameraViewModel _camera;
        public CameraViewModel CameraObject
        {
            get { return _camera; }
            set { Set(() => CameraObject, ref _camera, value); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { Set(() => Title, ref _title, value); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(() => IsLoading, ref _isLoading, value); }
        }
        #endregion

        #region Constructor
        public CameraSettingViewModel(
            IDialogService dialogService,
            ICameraManager cameraManager)
        {
            _dialogService = dialogService;
            _cameraManager = cameraManager;

            CameraObject = new CameraViewModel();
        }
        #endregion
    }

    public class CameraViewModel : ObservableObject
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { Set(() => Id, ref _id, value); }
        }

        private string _ipAddress;
        public string IpAddress
        {
            get { return _ipAddress; }
            set { Set(() => IpAddress, ref _ipAddress, value); }
        }

        private string _floor;
        public string Floor
        {
            get { return _floor; }
            set { Set(() => Floor, ref _floor, value); }
        }

        private string _cameraName;
        public string CameraName
        {
            get { return _cameraName; }
            set { Set(() => CameraName, ref _cameraName, value); }
        }

        private decimal _height;
        public decimal Height
        {
            get { return _height; }
            set { Set(() => Height, ref _height, value); }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set { Set(() => Status, ref _status, value); }
        }

    }
}
