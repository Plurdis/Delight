using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Delight.Windows
{
    /// <summary>
    /// LoadingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public LoadingWindow()
        {
            InitializeComponent();
            this.Loaded += LoadingWindow_Loaded;
        }

        private void LoadingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Thread thr = new Thread(() =>
            {
                bool b = true;
                while (b)
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (pg.Maximum != pg.Value)
                            pg.Value += 1;
                        else
                            this.Close();
                    });
                    Thread.Sleep(1000);
                }
            });

            thr.Start();
        }
    }
}
