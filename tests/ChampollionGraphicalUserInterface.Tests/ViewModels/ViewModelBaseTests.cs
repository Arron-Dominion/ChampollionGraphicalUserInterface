using CommunityToolkit.Mvvm.ComponentModel;
using ChampollionGraphicalUserInterface.ViewModels;

namespace ChampollionGraphicalUserInterface.Tests.ViewModels;

public sealed class ViewModelBaseTests
{
    [Fact]
    public void Provides_observable_object_behavior_to_derived_view_models()
    {
        TestViewModel viewModel = new();
        string? changedProperty = null;
        viewModel.PropertyChanged += (_, args) => changedProperty = args.PropertyName;

        viewModel.Value = 1;

        Assert.IsAssignableFrom<ObservableObject>(viewModel);
        Assert.Equal(nameof(TestViewModel.Value), changedProperty);
    }

    private sealed class TestViewModel : ViewModelBase
    {
        private int value;

        public int Value
        {
            get => value;
            set => SetProperty(ref this.value, value);
        }
    }
}