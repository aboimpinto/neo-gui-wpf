﻿using System.Linq;
using Moq;
using Neo.Gui.Base.Services.Interfaces;
using Neo.Gui.TestHelpers;
using Neo.Gui.ViewModels.Accounts;
using Xunit;

namespace Neo.Gui.ViewModels.Tests.Accounts
{
    public class CreateMultiSigContractViewModelTests : TestBase
    {
        [Fact]
        public void Ctr_CreateValidCreateMultiSigContractViewModel()
        {
            // Arrange
            // Act
            var viewModel = this.AutoMockContainer.Create<CreateMultiSigContractViewModel>();

            //Assert
            Assert.IsType<CreateMultiSigContractViewModel>(viewModel);
        }

        [Fact]
        public void AddPublicKeyCommand_AddPublicKeyIsEnableAndKeyIsNotYetAdded_KeyAddedToPublicKeysList()
        {
            // Arrance
            var newPublicKey = "newPublicKey";

            var viewModel = this.AutoMockContainer.Create<CreateMultiSigContractViewModel>();
            viewModel.NewPublicKey = newPublicKey;

            // Act
            viewModel.AddPublicKeyCommand.Execute(null);

            //Assert
            Assert.Single(viewModel.PublicKeys);
            Assert.Equal(newPublicKey, viewModel.PublicKeys.Single());
            Assert.Empty(viewModel.NewPublicKey);
            Assert.Equal(1, viewModel.MinimumSignatureNumberMaxValue);
        }

        [Fact]
        public void AddPublicKeyCommand_AddPublicKeyIsEnableAndKeyIsAlreadyAdded_NotificationIsCalledAndJustOneKeyIsAdded()
        {
            // Arrance
            var newPublicKey = "newPublicKey";

            var notificationServiceMock = this.AutoMockContainer.GetMock<INotificationService>();

            var viewModel = this.AutoMockContainer.Create<CreateMultiSigContractViewModel>();
            viewModel.NewPublicKey = newPublicKey;

            // Act
            viewModel.AddPublicKeyCommand.Execute(null);
            viewModel.NewPublicKey = newPublicKey;
            viewModel.AddPublicKeyCommand.Execute(null);

            //Assert
            notificationServiceMock.Verify(x => x.ShowInformationNotification(It.IsAny<string>()));     // TODO - Issue #130: When the shown string is localized, the test should reflect it.
            Assert.Single(viewModel.PublicKeys);
        }

        [Fact]
        public void AddPublicKeyCommand_AddPublicKeyIsNotEnable_NoKeyIsAdded()
        {
            // Arrance
            var notificationServiceMock = this.AutoMockContainer.GetMock<INotificationService>();

            var viewModel = this.AutoMockContainer.Create<CreateMultiSigContractViewModel>();

            // Act
            viewModel.AddPublicKeyCommand.Execute(null);

            //Assert
            Assert.Empty(viewModel.PublicKeys);
        }

        [Fact]
        public void RemovePublicKeyCommand_RemovePublicKeyIsEnabled_PublicKeyListIsEmpty()
        {
            // Arrance
            var newPublicKey = "newPublicKey";

            var viewModel = this.AutoMockContainer.Create<CreateMultiSigContractViewModel>();
            viewModel.NewPublicKey = newPublicKey;
            viewModel.SelectedPublicKey = newPublicKey;

            // Act
            viewModel.AddPublicKeyCommand.Execute(null);
            viewModel.RemovePublicKeyCommand.Execute(null);

            //Assert
            Assert.Empty(viewModel.PublicKeys);
            Assert.Equal(0, viewModel.MinimumSignatureNumberMaxValue);
        }
    }
}
