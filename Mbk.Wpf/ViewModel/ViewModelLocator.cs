/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Mbk.Wpf"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using AutoMapper;
using GalaSoft.MvvmLight.Ioc;
using Mbk.Business;
using Mbk.Business.Interfaces;
using Mbk.Dal;
using Mbk.Dal.Repositories;
using Mbk.Dal.Repositories.Interfaces;
using Mbk.Model;
using Mbk.Wpf.Services;
using Mbk.Wpf.Services.Interfaces;
using Microsoft.Practices.ServiceLocation;

namespace Mbk.Wpf.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CameraViewModel, CameraModel>().ReverseMap();
                cfg.AddProfile<ModelProfile>();
            });

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<IDialogService, DialogService>();

            SimpleIoc.Default.Register<ICameraRepository, CameraRepository>();
            SimpleIoc.Default.Register<IHeatMapRepository, HeatMapRepository>();
            SimpleIoc.Default.Register<ICountingRepository, CountingRepository>();

            SimpleIoc.Default.Register<ICameraManager, CameraManager>();
            SimpleIoc.Default.Register<IReportManager, ReportManager>();
            SimpleIoc.Default.Register<IConfigManager, ConfigManager>();
            SimpleIoc.Default.Register<IDataManager, DataManager>();


            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ConfigViewModel>();
            SimpleIoc.Default.Register<CameraSettingViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public ConfigViewModel Config
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ConfigViewModel>();
            }
        }

        public CameraSettingViewModel Camera
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CameraSettingViewModel>();
            }
        }

        public static void Cleanup()
        {
        }
    }
}