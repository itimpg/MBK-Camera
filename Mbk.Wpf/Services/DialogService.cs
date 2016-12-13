using Mbk.Wpf.Services.Interfaces;
using Microsoft.Practices.ServiceLocation;
using System.Windows;
using Forms = System.Windows.Forms;

namespace Mbk.Wpf.Services
{
    public class DialogService : IDialogService
    {
        public bool BrowseFolder(out string selectedPath)
        {
            selectedPath = string.Empty;

            var dlg = new Forms.FolderBrowserDialog();
            var result = dlg.ShowDialog();
            if (result == Forms.DialogResult.OK)
            {
                selectedPath = dlg.SelectedPath;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ShowMessage(string message, string caption)
        {
            MessageBox.Show(message, caption);
        }

        public bool ShowConfirmationMessage(string message, string caption)
        {
            var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes;
        }
    }
}
