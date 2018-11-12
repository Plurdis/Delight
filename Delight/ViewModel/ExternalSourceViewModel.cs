using Delight.Core.Sources;
using Delight.Core.Template;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace Delight.ViewModel
{
    public class ExternalSourceViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ExternalSourceViewModel()
        {
            //OpenFileDialog ofd = new OpenFileDialog();
            //if (ofd.ShowDialog().Value)
            //{
            //    LoadTemplates(ofd.FileName);
            //}

            Sources = new ObservableCollection<BaseSource>();

            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=0pXYp72dwl0&list=PLMTfiLScYpP4a8MSHoyMcRmNdSwV7ghK9&ab_channel=MatthiasM.de"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=nM0xDI5R50E&ab_channel=1theK%28%EC%9B%90%EB%8D%94%EC%BC%80%EC%9D%B4%29"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=a0yaOFwKKJ8&list=PLMTfiLScYpP4a8MSHoyMcRmNdSwV7ghK9&index=4&ab_channel=floopyFX"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=Yq_fVXatChk&index=20&list=PLMTfiLScYpP4a8MSHoyMcRmNdSwV7ghK9&ab_channel=AlexRideout"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=PopWkg8-hsI&list=PLMTfiLScYpP4a8MSHoyMcRmNdSwV7ghK9&index=65&ab_channel=LuenWarneke"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=layW4vGC2uM&ab_channel=piano77man77"));
            Sources.Add(YoutubeDownloader.GetYoutubeSource_Offical("https://www.youtube.com/watch?v=RzE62hW-w0s&index=4&list=PL11S5Ezd1D92ZLY5i1m4ly8OkAKYNJh6O&ab_channel=footageisland"));
            //AddFromYoutubeSource(ys);
        }

        public void LoadTemplates(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ExternalSources));
            
            var reader = new StreamReader(filePath);
            var sources = serializer.Deserialize(reader) as ExternalSources;

            sources.Sources.ForEach(i => Sources.Add(i));
        }

        public ObservableCollection<BaseSource> Sources { get; private set; }

    }
}
