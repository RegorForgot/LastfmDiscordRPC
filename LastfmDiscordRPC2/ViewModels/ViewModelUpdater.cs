using System.Collections.Generic;
using LastfmDiscordRPC2.ViewModels.Panes;

namespace LastfmDiscordRPC2.ViewModels;

public class ViewModelUpdater : IViewModelUpdater
{
    private readonly IEnumerable<IUpdatableViewModel> _viewModels;

    public ViewModelUpdater(IEnumerable<IUpdatableViewModel> viewModels)
    {
        _viewModels = viewModels;
    }
    
    public void UpdateAllViewModels()
    {
        foreach (IUpdatableViewModel viewModel in _viewModels)
        {
            viewModel.UpdateProperties();
        }
    }
}