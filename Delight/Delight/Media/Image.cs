using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Delight.Media
{
    public class Image : IMedia
    {
        public string Identity { get; set; }

        public ImageSource ImageSource { get; set; }
    }
}
