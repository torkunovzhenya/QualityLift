using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;

namespace MainApp.Services
{
    public class DownloadEventArgs : EventArgs
    {
        public bool FileSaved = false;
        public DownloadEventArgs(bool fileSaved)
        {
            FileSaved = fileSaved;
        }
    }

    public interface IDownloader
    {
        void DownloadFile(string url, string folder, string name);
        event EventHandler<DownloadEventArgs> OnFileDownloaded;
    }
}
