using System.Collections.Generic;

namespace MainView.Mock
{
    public interface IModel
    {
        Model.DataUpdateEventHandler OnDataUpdate { get; set; }

        void CheckWebsite(object obj);
        void CloseTimers();
        IEnumerable<IWebsite> GetWebsites();
        int GetWebsitesCount();
        void LoadFromXML(string xmlPath = "./websites.xml");
        void SaveToXML(string xmlPath = "./websites.xml");
        ValidationResult ValidateAdd(IWebsite website);
        ValidationResult ValidateEdit(int index, IWebsite website);
        ValidationResult ValidateRemove(int index);
    }
}