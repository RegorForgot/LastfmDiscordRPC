using System.Collections.Generic;

namespace LastfmDiscordRPC2.ViewModels.Update;

public class ViewModelSetter : IViewModelSetter
{
    private readonly IEnumerable<ISettableViewModel> _viewModels;

    public ViewModelSetter(IEnumerable<ISettableViewModel> viewModels)
    {
        _viewModels = viewModels;
    }
    
    public void SetAllViewModels()
    {
        foreach (ISettableViewModel viewModel in _viewModels)
        {
            viewModel.SetProperties();
        }
    }
}