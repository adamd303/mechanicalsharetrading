using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Trading.Mechanical.ViewModel
{
    public interface IFilePathProvider
    {
        string GetLoadPath();

        string GetSavePath();
    }
}
