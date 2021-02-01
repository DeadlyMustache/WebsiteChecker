
namespace MainView.Mock
{
    public class Presenter
    {
        
        private IModel model = null;
        private IView view = null;
        

        public Presenter(IModel model, IView view)
        {
            this.model = model;
            this.view = view;

            ModelDataToViewData();

            view.ContextMenuEvent += View_ContextMenuEvent;
            model.OnDataUpdate = WhenModelUpdatesData;
        }

        public void WhenModelUpdatesData()
        {
            view.DataUpdate();
        }

        private void ModelDataToViewData()
        {
            view.AddMany(model.GetWebsites());
        }

        private ValidationResult View_ContextMenuEvent(EventType eventType, ContextMenuEventArgs eventArgs)
        {
            switch (eventType)
            {
                case EventType.BeginAdding: return BeginAdding();
                case EventType.RowValidating: return RowValidating(eventArgs);
                case EventType.Remove: return Remove(eventArgs);
                case EventType.Exit: model.CloseTimers(); return new ValidationResult();
                default: return new ValidationResult() { IsSuccessfull = false };
            }
        }

        private ValidationResult BeginAdding()
        {
            view.Add(new Website());
            return new ValidationResult() { IsSuccessfull = true };
        }

        private ValidationResult RowValidating(ContextMenuEventArgs eventArgs)
        {
            if (eventArgs.RowIndex == model.GetWebsitesCount())
            {
                return model.ValidateAdd(eventArgs.Website);
            }
            else
            {
                return model.ValidateEdit(eventArgs.RowIndex, eventArgs.Website);
            }
        }

        private ValidationResult Remove(ContextMenuEventArgs eventArgs)
        {
            ValidationResult vr = model.ValidateRemove(eventArgs.RowIndex);
            if (!vr.IsSuccessfull) return vr;
            view.Remove(eventArgs.RowIndex);
            return vr;
        }
    }
}
