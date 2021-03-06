﻿using System;
using System.Diagnostics;
using System.Windows;
using Autofac;
using Neo.Gui.Dialogs;
using Neo.Gui.Dialogs.Interfaces;
using Neo.Gui.Wpf.Controls;

namespace Neo.Gui.Wpf.Native.Services
{
    public class DialogManager : IDialogManager
    {
        #region Private Fields 
        private static ILifetimeScope containerLifetimeScope;
        #endregion

        #region IDialogManager implementation
        public IDialog<TLoadParameters> CreateDialog<TLoadParameters>(TLoadParameters parameters)
        {
            var view = ResolveDialogInstance<TLoadParameters>();

            return view;
        }

        public void ShowDialog<TLoadParameters>(TLoadParameters parameters)
        {
            var isDialogClosed = false;

            var view = this.CreateDialog(parameters);

            if (view.DataContext is IDialogViewModel<TLoadParameters> viewModel)
            {
                viewModel.Close += (sender, args) =>
                {
                    isDialogClosed = true;
                    (view as Window).Close();
                };

                viewModel.OnDialogLoad(parameters);
            }

            if (!isDialogClosed)
            {
                view.ShowDialog();
            }
        }

        public TDialogResult ShowDialog<TLoadParameters, TDialogResult>(TLoadParameters parameters)
        {
            var isDialogClosed = false;
            var dialogResult = default(TDialogResult);

            var view = this.CreateDialog<TLoadParameters, TDialogResult>(parameters, result => { dialogResult = result; });

            if (view.DataContext is IDialogViewModel<TLoadParameters> viewModel)
            {
                viewModel.Close += (sender, args) =>
                {
                    isDialogClosed = true;
                    (view as Window).Close();
                };

                viewModel.OnDialogLoad(parameters);
            }

            if (!isDialogClosed)
            {
                view.ShowDialog();
            }

            return dialogResult;
        }

        public string ShowInputDialog(string title, string message, string input = "")
        {
            var isOk = InputBox.Show(out var result, message, title, input);

            return isOk ? result : null;
        }

        public void ShowInformationDialog(string title, string message, string text)
        {
            InformationBox.Show(text, message, title);
        }

        public MessageDialogResult ShowMessageDialog(string title, string message, MessageDialogType type = MessageDialogType.Ok, MessageDialogResult defaultResult = MessageDialogResult.Ok)
        {
            switch (type)
            {
                case MessageDialogType.Ok:
                    if (string.IsNullOrEmpty(title))
                    {
                        MessageBox.Show(message);
                    }
                    else
                    {
                        MessageBox.Show(message, title);
                    }
                    return MessageDialogResult.Ok;

                case MessageDialogType.OkCancel:
                    var result = MessageBox.Show(message, title, MessageBoxButton.OKCancel);

                    switch (result)
                    {
                        case MessageBoxResult.OK:
                            return MessageDialogResult.Ok;

                        case MessageBoxResult.Cancel:
                        default:
                            return MessageDialogResult.Cancel;
                    }

                case MessageDialogType.YesNo:
                    result = MessageBox.Show(message, title, MessageBoxButton.YesNo);

                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            return MessageDialogResult.Yes;

                        case MessageBoxResult.No:
                        default:
                            return MessageDialogResult.No;
                    }
            }

            return defaultResult;
        }

        #endregion

        #region Public static methods
        public static void SetLifetimeScope(ILifetimeScope lifetimeScope)
        {
            containerLifetimeScope = lifetimeScope;
        }
        #endregion

        #region Private Methods
        private IDialog<TLoadParameters> CreateDialog<TLoadParameters, TDialogResult>(TLoadParameters parameters, Action<TDialogResult> resultSetter)
        {
            var view = this.CreateDialog(parameters);

            if (view.DataContext is IResultDialogViewModel<TLoadParameters, TDialogResult> viewModel)
            {
                var viewWindow = view as Window;

                viewModel.SetDialogResultAndClose += (sender, e) =>
                {
                    resultSetter?.Invoke(e);

                    viewWindow?.Close();
                };
            }

            return view;
        }

        private static IDialog<TDialogResult> ResolveDialogInstance<TDialogResult>()
        {
            var view = containerLifetimeScope?.Resolve<IDialog<TDialogResult>>();

            Debug.Assert(view != null);
            Debug.Assert(view is Window);

            return view;
        }
        
        #endregion
    }
}
