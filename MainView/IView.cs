using System.Collections.Generic;
using MainView.Mock;

namespace MainView
{
    public interface IView
    {
        
        event ContextMenuEventHandler ContextMenuEvent;

        void Add(IWebsite newWebsite);
        void AddMany(IEnumerable<IWebsite> newWebsites);
        void Remove(int index);
        void DataUpdate();
        void RefreshDgv();
    }

    public delegate ValidationResult ContextMenuEventHandler(EventType eventType, ContextMenuEventArgs eventArgs);
}