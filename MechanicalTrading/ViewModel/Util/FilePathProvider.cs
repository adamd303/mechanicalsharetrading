using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Adam.Trading.Mechanical.ViewModel
{
    public class FilePathProvider : IFilePathProvider
    {
        public string GetLoadPath()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "csv Files (*.csv)|*.csv";
            string filePath = null;
            bool? dialogResult = ofd.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                filePath = ofd.FileName;
            }
            return filePath;
        }

        public string GetSavePath()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "csv Files (*.csv)|*.csv";
            string filePath = null;
            bool? dialogResult = sfd.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                filePath = sfd.FileName;
            }
            return filePath;
        }
    }
}
