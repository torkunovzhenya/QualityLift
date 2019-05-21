using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MainApp.Droid;
using MainApp.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidDownloader))]
namespace MainApp.Droid
{
    public class AndroidDownloader : IDownloader
    {
        public event EventHandler<DownloadEventArgs> OnFileDownloaded;

        public void DownloadFile(string url, string data, string name)
        {
            string pathToNewFolder = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "QualityLift Images");
            Directory.CreateDirectory(pathToNewFolder);

            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                string pathToNewFile = Path.Combine(pathToNewFolder, name);

                if (data != "No data")
                {
                    using (FileStream file = new FileStream(pathToNewFile, FileMode.Create, FileAccess.Write))
                    {
                        byte[] d = Convert.FromBase64String(data);
                        file.Write(d, 0, d.Length);
                        OnFileDownloaded?.Invoke(this, new DownloadEventArgs(true));
                    }
                }
                else
                    webClient.DownloadFileAsync(new Uri(url), pathToNewFile);
            }
            catch (Exception)
            {
                OnFileDownloaded?.Invoke(this, new DownloadEventArgs(false));
            }
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            OnFileDownloaded?.Invoke(this, new DownloadEventArgs(e.Error == null));
        }
    }
}