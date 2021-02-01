using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Xml.Linq;
using Timer = System.Threading.Timer;

namespace MainView.Mock
{
    public class Model : IModel
    {
        private List<IWebsite> websites = new List<IWebsite>();
        private List<Timer> timers = new List<Timer>();

        public delegate void DataUpdateEventHandler();
        public DataUpdateEventHandler OnDataUpdate { get; set; }

        private TimerCallback timerCallback = null;

        public Model()
        {
            timerCallback = new TimerCallback(CheckWebsite);
            LoadFromXML();
        }

        private void AddWebsite(IWebsite website)
        {
            timers.Add(new Timer(timerCallback, website, 0, website.CheckInterval * 1000));
            websites.Add(website);
        }

        private void EditWebsite(int index, IWebsite website)
        {
            timers[index].Change(0, website.CheckInterval * 300);
            websites[index] = website;
        }

        private void RemoveWebsite(int index)
        {
            websites.RemoveAt(index);
            timers[index].Dispose();
            timers.RemoveAt(index);
        }

        public void CheckWebsite(object obj)
        {
            IWebsite website = (IWebsite)obj;
            website.IsOnline = WebsiteChecker.CheckURL(website.URL);
            website.ChecksCount++;
            OnDataUpdate?.Invoke();
        }

        public IEnumerable<IWebsite> GetWebsites()
        {
            return websites;
        }

        public int GetWebsitesCount()
        {
            return websites.Count;
        }

        private ValidationResult ValidateValues(IWebsite website)
        {
            ValidationResult vr = new ValidationResult();
            if (String.IsNullOrWhiteSpace(website.Name) || website.Name.Length < 3)
            {
                vr.IsSuccessfull = false;
                vr.Message = "Name is too short!!!";
                return vr;
            }

            
            Uri uriResult;
            bool result = Uri.TryCreate(website.URL, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if(!result)
            {
                vr.IsSuccessfull = false;
                vr.Message = @"URL format: protocol://name.domain! Protocols are - (http|https).";
                return vr;
            }

            /*string regexURL = @"^((http|ftp|https)://)?([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?$";
            if (String.IsNullOrWhiteSpace(website.URL) || !Regex.IsMatch(website.URL, regexURL))
            {
                vr.IsSuccessfull = false;
                vr.Message = @"URL format: protocol://name.domain! Protocols are - (http|ftp|https).";
                return vr;
            }*/

            if (website.CheckInterval < 10)
            {
                vr.IsSuccessfull = false;
                vr.Message = @"Interval should be greater than 10 (seconds)!";
                return vr;
            }

            vr.IsSuccessfull = true;
            return vr;
        }


        public ValidationResult ValidateAdd(IWebsite website)
        {
            ValidationResult vr = ValidateValues(website);
            if (!vr.IsSuccessfull) return vr;

            AddWebsite(website);

            SaveToXML();

            return vr;
        }

        public ValidationResult ValidateEdit(int index, IWebsite website)
        {
            ValidationResult vr = ValidateValues(website);
            if (!vr.IsSuccessfull) return vr;

            EditWebsite(index, website);

            SaveToXML();

            return vr;
        }

        public ValidationResult ValidateRemove(int index)
        {
            ValidationResult vr = new ValidationResult() { IsSuccessfull = true };
            if (index >= websites.Count || index < 0)
            {
                vr.IsSuccessfull = false;
                vr.Message = "Index is out of range";
            }
            RemoveWebsite(index);
            SaveToXML();
            return vr;
        }

        public void SaveToXML(string xmlPath = "./websites.xml")
        {
            XDocument xdoc = new XDocument();
            XElement xroot = new XElement("Websites");
            for (int ix = 0; ix < websites.Count; ix++)
            {
                XElement xwebsite = new XElement("Website",
                    new XAttribute("Name", websites[ix].Name),
                    new XAttribute("URL", websites[ix].URL),
                    new XAttribute("CheckInterval", websites[ix].CheckInterval));
                xroot.Add(xwebsite);
            }
            xdoc.Add(xroot);
            xdoc.Save(xmlPath);
        }

        public void LoadFromXML(string xmlPath = "./websites.xml")
        {
            websites.Clear();
            XDocument xdoc = XDocument.Load(xmlPath);
            foreach (XElement xwebsite in xdoc.Root.Elements("Website"))
            {
                string name = xwebsite.Attribute("Name").Value;
                string url = xwebsite.Attribute("URL").Value;
                uint checkInterval = Convert.ToUInt32(xwebsite.Attribute("CheckInterval").Value, CultureInfo.InvariantCulture);
                IWebsite website = new Website() { Name = name, URL = url, CheckInterval = checkInterval };
                AddWebsite(website);
            }
        }

        public void CloseTimers()
        {
            foreach (Timer timer in timers)
            {
                timer.Dispose();
            }
            timers.Clear();
        }
    }
}
