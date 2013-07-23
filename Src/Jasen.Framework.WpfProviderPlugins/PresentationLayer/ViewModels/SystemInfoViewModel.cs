using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using Common.Applications.Applications; 

namespace Jasen.Framework.WpfProviderPlugins.PresentationLayer.ViewModels
{
    public class SystemInfoViewModel: ViewModelBase
    {
        public SystemInfoViewModel()
        {
            this.ApplicationInfo = new ApplicationInfo();
        }

        public ApplicationInfo ApplicationInfo
        {
            get;
            private set;
        }
    }
}
