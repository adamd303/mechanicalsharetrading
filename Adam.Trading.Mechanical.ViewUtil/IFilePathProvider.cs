using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Trading.Mechanical.ViewUtil
{
    public interface IFilePathProvider
    {
        string GetLoadPath();

        string GetSavePath();
    }
}
