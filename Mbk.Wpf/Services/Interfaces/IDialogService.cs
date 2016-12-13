using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mbk.Wpf.Services.Interfaces
{
    public interface IDialogService
    {
        bool BrowseFolder(out string selectedPath);
        void ShowMessage(string message, string caption);
        bool ShowConfirmationMessage(string message, string caption);
    }
}
