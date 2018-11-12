using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Delight.Component.Controls
{
    internal class PropertyGridItemView : ListViewItem
    {
        public PropertyGridItemModel Model => (PropertyGridItemModel)DataContext;

        public string Category => Model.Category;

        public PropertyGridItemView(PropertyGridItemModel model)
        {
            this.DataContext = model;
        }
    }
}
