using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StagePainter.Common;
using StagePainter.Core.Common;
using StagePainter.Projects;
using StagePainter.Windows;

using LocalCommandManager = StagePainter.Common.CommandManager;
using wf = System.Windows.Forms;

namespace StagePainter
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            LocalCommandManager.Init();
            InitializeComponent();
            MouseManager.Init();

            this.Closing += (s, e) => Environment.Exit(0);
#if DEBUG
            //img.Source = ImageCreator.GetWireFrame(200, 300, Brushes.Red);
#endif
            CommandBindings.Add(new CommandBinding(MenuCommands.ExitCommand, (s, e) => Environment.Exit(0)));
            CommandBindings.Add(new CommandBinding(MenuCommands.ExportCommand, (s, e) => MessageBox.Show("[내보내기]는 완성되지 않은 기능입니다.")));
            CommandBindings.Add(new CommandBinding(MenuCommands.NewProjectCommand, (s, e) => MessageBox.Show("[새로운 프로젝트]는 완성되지 않은 기능입니다.")));
            CommandBindings.Add(new CommandBinding(MenuCommands.OpenFileCommand, (s, e) =>
            {

                wf.OpenFileDialog ofd = new wf.OpenFileDialog();
                // wma, aac, mp4, aiff
                
                ofd.Filter = "오디오 파일 (*.mp3;*.m4a;*.wav;*.flac)|*.mp3;*.m4a;*.wav;*.flac|미디어 파일 (*.mp4)|*.mp4";

                ofd.ShowDialog();
            }));
            CommandBindings.Add(new CommandBinding(MenuCommands.OpenProjectCommand, (s, e) => MessageBox.Show("[프로젝트 열기]는 완성되지 않은 기능입니다.")));
            CommandBindings.Add(new CommandBinding(MenuCommands.SaveAsCommand, (s, e) => MessageBox.Show("[다른 이름으로 저장]은 완성되지 않은 기능입니다.")));
            CommandBindings.Add(new CommandBinding(MenuCommands.SaveCommand, (s, e) => MessageBox.Show("[저장]은 완성되지 않은 기능입니다.")));
            
            MenuCommands.ExitCommand.InputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Control));

            SetProject(new ProjectInfo()
            {
                ProjectName = "ConsoleApp1"
            });
        }

        #region [  Global Variable  ]

        ProjectInfo ProjectInfo { get; set; }

        #endregion

        public void SetProject(ProjectInfo projectInfo)
        {
            ProjectInfo = projectInfo;
            projectInfo.PropertyChanged += ProjectInfo_PropertyChanged;

            this.Title = "SPainter - " + projectInfo.ProjectName;
        }

        private void ProjectInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is ProjectInfo pi)
            {
                switch (e.PropertyName)
                {
                    case "ProjectName":
                        this.Title = "SPainter - " + pi.ProjectName;
                        break;
                }
            }
            
        }

        #region [  File I/O Task  ]

        public void OpenProject()
        {

        }

        public void SaveAs()
        {

        }

        public void Save()
        {

        }
        
        #endregion

        public void AddCommandBinding(KeyGesture gesture, ExecutedRoutedEventHandler eventHandler)
        {
            RoutedCommand comm = new RoutedCommand();

            comm.InputGestures.Add(gesture);

            this.CommandBindings.Add(new CommandBinding(comm, eventHandler));
        }

        private void Menu_MouseMove(object sender, MouseEventArgs e)
        {
            string id = ((UIElement)sender).Uid;

            this.Title = id;
        }
    }
}
