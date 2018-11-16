using Delight.Component.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Delight.Component.Controls
{
    public class StarGridViewColumn : GridViewColumn
    {
        public static readonly DependencyProperty StarWidthProperty =
            DependencyHelper.Register();

        public double StarWidth
        {
            get { return this.GetValue<double>(StarWidthProperty); }
            set { SetValue(StarWidthProperty, value); }
        }

        public StarGridViewColumn()
        {
            StarWidthProperty.AddValueChanged(this, StarWidthChanged);
        }

        private void StarWidthChanged(object sender, EventArgs e)
        {
        }
    }
}
