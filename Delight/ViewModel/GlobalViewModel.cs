using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.ViewModel
{
    public static class GlobalViewModel
    {
        static GlobalViewModel()
        {
            ExternalSourceViewModel = new ExternalSourceViewModel();
            TemplateShopViewModel = new TemplateShopViewModel();
            MainWindowViewModel = new MainWindowViewModel();
        }

        public static ExternalSourceViewModel ExternalSourceViewModel { get; }

        public static MainWindowViewModel MainWindowViewModel { get; }

        public static TemplateShopViewModel TemplateShopViewModel { get; }
    }
}
