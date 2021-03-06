﻿using Core.Code;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Core.Helper
{
    public class UploadHelper
    {
        public static string Save(HttpPostedFileBase file,string mark)
        {
            var root = @"Upload/" + mark;
            string phicyPath = Path.Combine(HttpRuntime.AppDomainAppPath, root);
            Directory.CreateDirectory(phicyPath);
            var fileName = Guid.NewGuid().ToString("N") + file.FileName.Substring(file.FileName.LastIndexOf('.'));
            string path= Path.Combine(phicyPath, fileName);
            file.SaveAs(path);
            return string.Format("/{0}/{1}", root, fileName);
        }

        public static string Save(HttpPostedFileBase file, string mark, out string fileName)
        {
            var root = string.Empty;
            if (LoginHelper.UserIsLogin())
            {
                root = $"Upload/{LoginHelper.GetCurrentUser().Account}/{mark}";
            }
            else
            {
                root = @"Upload/" + mark;
            }
            string phicyPath = Path.Combine(HttpRuntime.AppDomainAppPath, root);
            var savefileName = Guid.NewGuid().ToString("N") + file.FileName.Substring(file.FileName.LastIndexOf('.'));
            fileName = file.FileName;
            string path = Path.Combine(phicyPath, savefileName);
            if (!Directory.Exists(phicyPath))
                Directory.CreateDirectory(phicyPath);
            file.SaveAs(path);
            return string.Format("/{0}/{1}", root, savefileName);
        }

        public static string Save(HttpPostedFile file, string mark)
        {
            var root = @"Upload/" + mark;
            string phicyPath = Path.Combine(HttpRuntime.AppDomainAppPath, root);
            Directory.CreateDirectory(phicyPath);
            var fileName = Guid.NewGuid().ToString("N") + file.FileName.Substring(file.FileName.LastIndexOf('.'));
            string path = Path.Combine(phicyPath, fileName);
            file.SaveAs(path);
            return string.Format("/{0}/{1}", root, fileName);
        }

        public static string SaveImageStream(Stream stream, string suffix)
        {
            var root = @"Upload/Image";
            string phicyPath = Path.Combine(HttpRuntime.AppDomainAppPath, root);
            Directory.CreateDirectory(phicyPath);
            var fileName = Guid.NewGuid().ToString("N") + suffix;
            string path = Path.Combine(phicyPath, fileName);

            using (Stream localFile =new FileStream(path, FileMode.OpenOrCreate))
            {
                byte[] b = new byte[5000];
                int getByteSize = stream.Read(b, 0, b.Length);
                while (getByteSize > 0)
                {
                    localFile.Write(b, 0, getByteSize);
                    getByteSize = stream.Read(b, 0, b.Length);
                }
            }

            return string.Format("/{0}/{1}", root, fileName);
        }
    }
}
