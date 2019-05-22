using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MainApp.AppResources;

namespace MainApp.Services
{
    public static class FormUpload
    {
        private static readonly Encoding encoding = Encoding.UTF8;
        public static HttpWebResponse MultipartFormDataPost(Dictionary<string, object> postParameters)
        {
            string DataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
            string ContentType = "multipart/form-data; boundary=" + DataBoundary;
            string PostUrl = "https://waifu2x.booru.pics/Home/upload";
            string UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.99 YaBrowser/19.1.2.258 Yowser/2.5 Safari/537.36";

            byte[] formData = GetMultipartFormData(postParameters, DataBoundary);

            return PostForm(PostUrl, UserAgent, ContentType, formData);
        }
        private static HttpWebResponse PostForm(string postUrl, string userAgent, string contentType, byte[] formData)
        {
            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            #region RequestProperties
            request.Method = "POST";
            request.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Referer = "https://waifu2x.booru.pics/";
            request.Headers.Add("AcceptLanguage", "ru,en;q=0.9,ba;q=0.8");
            request.ContentType = contentType;
            request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();
            request.Headers.Add("AcceptEncoding", "gzip, deflate, br");
            request.ContentLength = formData.Length;
            #endregion

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
            }

            return request.GetResponse() as HttpWebResponse;
        }

        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new MemoryStream();
            bool needsCLRF = false;

            foreach (var parameter in postParameters)
            {
                if (needsCLRF)
                    formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                if (parameter.Value is FileParameter)
                {
                    FileParameter fileToUpload = (FileParameter)parameter.Value;
                    
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        parameter.Key,
                        fileToUpload.FileName,
                        fileToUpload.ContentType);

                    formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                        boundary,
                        parameter.Key,
                        parameter.Value);

                    formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                }
            }
            
            string end = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(end), 0, encoding.GetByteCount(end));
            
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }
    }

    public class FileParameter
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public FileParameter(byte[] file, string filename, string contenttype)
        {
            File = file;
            FileName = filename;
            ContentType = contenttype;
        }
    }

    public static class ImageExchange
    {
        public static string GetImageUrl(string s)
        {
            string hash = "";
            string find = "hash=\" + \"";

            int index = s.IndexOf(find);

            if (index == -1)
            {
                find = "/outfiles/";
                index = s.IndexOf(find);
                if (index == -1)
                    throw new FormatException("Image maximum size - 8M, 3840x3840");
            }

            index += find.Length;
            char c;

            do{
                c = s[index];
                hash += c;
                index++;
            } while (c != '_');

            return hash;
        }

        public static string Post(int denoise, int scale, string format, Stream filestream)
        {
            byte[] data;
            
            data = new byte[filestream.Length];
            filestream.Read(data, 0, data.Length);
            filestream.Dispose();

            #region Generation of Post Objects
            Dictionary<string, object> Parameters = new Dictionary<string, object>();
            Parameters.Add("img", new FileParameter(data, "preimage." + format, "image/" + format));
            Parameters.Add("denoise", denoise.ToString());
            Parameters.Add("scale", scale.ToString());
            Parameters.Add("submit", "");
            #endregion

            // Create request and receive response
            string ans;
            using (HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(Parameters))
            {
                StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
                string fullResponse = responseReader.ReadToEnd();
                ans = GetImageUrl(fullResponse);
            }

            format = format == "jpeg" ? "jpg" : "png";
            return "https://waifu2x.booru.pics/outfiles/" + ans + $"s{scale}_n{denoise}.{format}";
        }
    }
}
