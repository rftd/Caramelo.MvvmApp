using Caramelo.MvvmApp.ViewModel;

namespace Caramelo.MvvmApp.Navigation;

public interface ITabbedHostAdapter<TTabItem> where TTabItem : class
{
    bool CanCloseTabs => true;

    TTabItem CreateTabItem(IMvvmViewModel viewModel, IViewFor viewInstance, bool canClose);

    void UpdateTabItem(TTabItem tabItem, IMvvmViewModel viewModel, IViewFor viewInstance, bool canClose);

    void AddTabItem(TTabItem tabItem);

    void RemoveTabItem(TTabItem tabItem);

    void ClearTabItems();

    TTabItem? GetSelectedTabItem();

    void SetSelectedTabItem(TTabItem? tabItem);

    void ShowTabs();

    void ShowDefaultContent();
}