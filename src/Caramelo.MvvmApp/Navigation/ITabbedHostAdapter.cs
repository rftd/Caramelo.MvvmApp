using Caramelo.MvvmApp.ViewModel;

namespace Caramelo.MvvmApp.Navigation;

public interface ITabbedHostAdapter<TTabItem> where TTabItem : class
{
    TTabItem CreateTabItem(IMvvmViewModel viewModel, IViewFor viewInstance);

    void UpdateTabItem(TTabItem tabItem, IMvvmViewModel viewModel, IViewFor viewInstance);

    void AddTabItem(TTabItem tabItem);

    void RemoveTabItem(TTabItem tabItem);

    void ClearTabItems();

    TTabItem? GetSelectedTabItem();

    void SetSelectedTabItem(TTabItem? tabItem);

    void ShowTabs();

    void ShowDefaultContent();
}