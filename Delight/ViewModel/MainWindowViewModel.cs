using Delight.Component.Common;
using Delight.Core.Extensions;
using Delight.Core.Stage;
using Delight.Core.Stage.Components;
using Delight.Core.Stage.Components.Media;
using Delight.Core.Template.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Delight.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region [  Menus  ]

        public List<MenuItem> Menus { get; set; }

        public MenuItem FileMenu { get; set; }

        public MenuItem ToolsMenu { get; set; }

        #endregion

        private int _viewingIndex = 2;
        public int ViewingIndex
        {
            get => _viewingIndex;
            set
            {
                _viewingIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ViewingIndex"));
            }
        }

        public ObservableCollection<StageComponent> MediaItems { get; set; }
        public ListCollectionView MediaItemsView { get; }

        private SourceType _filterType = SourceType.All;

        public SourceType FilterType
        {
            get => _filterType;
            set
            {
                _filterType = value;
                MediaItemsView?.Refresh();
            }
        }

        public MainWindowViewModel()
        {
            OpenFileCommand = new RoutedCommand("OpenFileCommand", typeof(MainWindowViewModel));
            TemplateShopCommand = new RoutedCommand("TemplateShopCommand", typeof(MainWindowViewModel));
            EditTimeLineCommand = new RoutedCommand("EditTimeLineCommand", typeof(MainWindowViewModel));
            GetExternalSourceCommand = new RoutedCommand("GetExternalSourceCommand", typeof(MainWindowViewModel));

            InitalizeMenu();
            InitalizeGestures();

            MediaItems = new ObservableCollection<StageComponent>();
            MediaItemsView = new ListCollectionView(MediaItems);
            MediaItemsView.Filter = new Predicate<object>((s) =>
            {
                if (s is StageComponent c)
                {
                    // c.SourceType == FilterType
                    var b = FilterType.HasFlag(c.SourceType);
                    return b;
                }
                return false;
            });
        }

        #region [  Initalize  ]

        private void InitalizeMenu()
        {
            Menus = new List<MenuItem>();
            FileMenu = GetMenuItem("파일(_F)");

            FileMenu.Items.Add(GetMenuItem("열기(_O)", "Ctrl+O", OpenFileCommand));
            FileMenu.Items.Add(GetMenuItem("외부에서 영상 소스 가져오기(_E)", "Ctrl+Shift+E", GetExternalSourceCommand));
            FileMenu.Items.Add(GetMenuItem("새 프로젝트(_P)", "Ctrl+Shift+N"));
            FileMenu.Items.Add(new Separator());
            FileMenu.Items.Add(GetMenuItem("저장(_S)", "Ctrl+S"));
            FileMenu.Items.Add(GetMenuItem("타임 라인 편집", "Ctrl+Shift+T", EditTimeLineCommand));

            ToolsMenu = GetMenuItem("도구(_T)");

            ToolsMenu.Items.Add(GetMenuItem("템플릿 샵(_T)", "Ctrl+T", TemplateShopCommand));
            ToolsMenu.Items.Add(new Separator());
            ToolsMenu.Items.Add(GetMenuItem("수동 조명 관리(_M)"));
            ToolsMenu.Items.Add(GetMenuItem("옵션(_O)"));

            Menus.Add(FileMenu);
            Menus.Add(ToolsMenu);
        }

        private void InitalizeGestures()
        {
            RegisterCommand(OpenFileCommand, new KeyGesture(Key.O, ModifierKeys.Control));
            RegisterCommand(TemplateShopCommand, new KeyGesture(Key.T, ModifierKeys.Control));
            RegisterCommand(EditTimeLineCommand, new KeyGesture(Key.T, ModifierKeys.Control | ModifierKeys.Shift));
            RegisterCommand(GetExternalSourceCommand, new KeyGesture(Key.E, ModifierKeys.Control | ModifierKeys.Shift));
        }

        #endregion


        #region [  Input Gesture Connect  ]

        Dictionary<string, RoutedCommand> _commands = new Dictionary<string, RoutedCommand>();

        public event PropertyChangedEventHandler PropertyChanged;

        public bool RegisterCommand(RoutedCommand command, KeyGesture keyGesture)
        {
            if (_commands.ContainsKey(command.Name))
            {
                return false;
            }
            _commands[command.Name] = command;
            _commands[command.Name].InputGestures.Add(keyGesture);

            return true;
        }

        #endregion


        #region [  파일 메뉴  ]

        public RoutedCommand EditTimeLineCommand { get; }

        public void EditTimeLineExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ViewingIndex = 0;
        }

        public RoutedCommand GetExternalSourceCommand { get; }

        public void GetExternalSourceExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ViewingIndex = 2;
        }

        public RoutedCommand OpenFileCommand { get; }

        public void OpenFileExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!MediaTools.GetMediaFiles(out string[] locations))
                return;

            foreach (string location in locations)
            {
                StageComponent component = null;
                var fi = new FileInfo(location);

                switch (MediaTools.GetMediaTypeFromFile(location))
                {
                    case SourceType.Unknown:
                        break;
                    case SourceType.Image:
                        component = new ImageMedia()
                        {
                            Identifier = fi.Name,
                            Path = location,
                            Time = TimeSpan.FromSeconds(20),
                            Thumbnail = new Uri(location),
                        };
                        break;
                    case SourceType.Sound:
                        var uri = new Uri("pack://application:,,,/Delight;component/Resources/defaultSoundimage.png", UriKind.Absolute);
                        component = new SoundMedia()
                        {
                            Identifier = fi.Name, //fi.Name,
                            Path = location,
                            Time = MediaTools.GetMediaDuration(location),
                            Thumbnail = uri,
                        };
                        break;
                    case SourceType.Video:
                        component = new VideoMedia()
                        {
                            Identifier = fi.Name,
                            Path = location,
                            Time = MediaTools.GetMediaDuration(location),
                            Thumbnail = ((BitmapImage)MediaTools.GetMediaThumbnail(location)).UriSource,
                        };
                        break;
                    default:
                        break;
                }

                if (component != null)
                    MediaItems.Add(component);
            }
        }

        #endregion

        #region [  도구 메뉴  ]

        public RoutedCommand TemplateShopCommand { get; }

        public void TemplateShopExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ViewingIndex = 1;
        }

        #endregion

        public static MenuItem GetMenuItem(string header, string inputGestureText = "", ICommand command = null)
        {
            return new MenuItem()
            {
                Header = header,
                InputGestureText = inputGestureText,
                Command = command,
            };
        }

    }
}
