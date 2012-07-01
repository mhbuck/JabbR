using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using JabbR.Models;

namespace JabbR.WebApi
{
    public class FileUploadController : ApiController
    {
        private static string uploadLocation = HttpContext.Current.Server.MapPath("~\\Content\\upload\\");

        [HttpPost]
        public string Upload()
        {
            HttpContext postedContext = HttpContext.Current;
            HttpPostedFile file = postedContext.Request.Files[0];
            string fileName = GetUniqueFileName(file.FileName.Replace(" ", ""));

            try
            {
                byte[] binaryWriteArray = new byte[file.InputStream.Length];
                file.InputStream.Read(binaryWriteArray, 0, (int)file.InputStream.Length);
                FileStream objfilestream = new FileStream(uploadLocation + fileName, FileMode.Create, FileAccess.ReadWrite);
                objfilestream.Write(binaryWriteArray, 0, binaryWriteArray.Length);
                objfilestream.Close();
            }
            catch (IOException)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return string.Format("http://ob015.openbox.local/jabbr/content/upload/{0}", fileName);
        }

        private string GetUniqueFileName(string fileName)
        {
            var nameSplit = fileName.Split('.');
            var extension = nameSplit[nameSplit.Length - 1];

            var fileNameNoExt = fileName.Replace("." + extension, "");
            var fileNameTemp = fileName;
            var counter = 1;
            while(File.Exists(uploadLocation + fileNameTemp))
            {
                fileNameTemp = string.Format("{0}({1}).{2}", fileNameNoExt, counter, extension);
                counter++;
                
                if (counter > 100)
                {
                    break;
                }
            }

            return fileNameTemp;
        }
    }
}