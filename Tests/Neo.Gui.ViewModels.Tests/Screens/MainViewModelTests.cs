using Autofac;
using Neo.Gui.TestHelpers;
using Neo.Gui.ViewModels.ScreenViewModels;
using Xunit;

namespace Neo.Gui.ViewModels.Tests.Screens
{
    public class MainViewModelTests : TestBase
    {
        [Fact(Skip = "There is a problem setup Extension Methods.")]
        public void Ctor_CreateValidMainViewModel()
        {
            var loadWalletViewSimualtion = new ViewSimulation();

            var lifetimeScopeMock = this.AutoMockContainer.GetMock<ILifetimeScope>();
            //lifetimeScopeMock
            //    .Setup(x => x.ResolveKeyed<IView>(ViewNames.LoadWalletView))
            //    .Returns(loadWalletViewSimualtion);

            MainViewModel.SetLifetimeScope(lifetimeScopeMock.Object);
            var viewModel = this.AutoMockContainer.Create<MainViewModel>();

            Assert.IsType<MainViewModel>(viewModel);
        }
    }

    public class ViewSimulation : IView
    {
        public object Tag { get; set; }
        public object DataContext { get; set; }
    }
}
