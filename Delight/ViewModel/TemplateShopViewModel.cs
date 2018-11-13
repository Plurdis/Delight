using Delight.Core.Template;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.ViewModel
{
    public class TemplateShopViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public TemplateShopViewModel()
        {
            //var template = DelightTemplate.FromFile(@"C:\Users\uutak\바탕 화면\youtubeMusics.dlpack");
        }
    }
}
