namespace MainView.Mock
{
    public interface IWebsite
    {
        
        string Name { get; set; }
        string URL { get; set; }
        uint CheckInterval { get; set; }
        bool IsOnline { get; set; }
        uint ChecksCount { get; set; }
    }
}