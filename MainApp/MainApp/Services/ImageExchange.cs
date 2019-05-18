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
            string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;
            string postUrl = "https://waifu2x.booru.pics/Home/upload";
            string userAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.99 YaBrowser/19.1.2.258 Yowser/2.5 Safari/537.36";

            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);

            return PostForm(postUrl, userAgent, contentType, formData);
        }
        private static HttpWebResponse PostForm(string postUrl, string userAgent, string contentType, byte[] formData)
        {
            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            //if (request == null)
            //{
            //    throw new NullReferenceException("request is not a http request");
            //}

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
                requestStream.Close();
            }

            return request.GetResponse() as HttpWebResponse;
        }

        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {
                if (needsCLRF)
                    formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                if (param.Value is FileParameter)
                {
                    FileParameter fileToUpload = (FileParameter)param.Value;

                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                    // Write the file data directly to the Stream, rather than serializing it to a string.
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                        boundary,
                        param.Key,
                        param.Value);
                    formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                }
            }

            // Add the end of the request.  Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }

        public class FileParameter
        {
            public byte[] File { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
            public FileParameter(byte[] file) : this(file, null) { }
            public FileParameter(byte[] file, string filename) : this(file, filename, null) { }
            public FileParameter(byte[] file, string filename, string contenttype)
            {
                File = file;
                FileName = filename;
                ContentType = contenttype;
            }
        }
    }


    public static class ImageExchange
    {
        public static string GetImageUrl(string s)
        {
            string ur = "";
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

            do
            {
                c = s[index];
                ur += c;
                index++;
            } while (c != '_');

            return ur;
        }

        public static string Post(int denoise, int scale, string format, Stream filestream)
        {
            byte[] data;

            using (filestream)
            {
                data = new byte[filestream.Length];
                filestream.Read(data, 0, data.Length);
            }

            // Generate post objects
            Dictionary<string, object> Parameters = new Dictionary<string, object>();
            Parameters.Add("img", new FormUpload.FileParameter(data, "preimage." + format, "image/" + format));
            Parameters.Add("denoise", denoise.ToString());
            Parameters.Add("scale", scale.ToString());
            Parameters.Add("submit", "");

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
