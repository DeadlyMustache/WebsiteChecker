
namespace MainView.Mock
{
    public class Website : IWebsite
    {
        public string Name { get; set; }
        public string URL { get; set; }
        public uint CheckInterval { get; set; } = 60;
        public bool IsOnline { get; set; } = false;
        public uint ChecksCount { get; set; } = 0;


    }
}
