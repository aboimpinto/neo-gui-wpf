﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using Autofac;
using Neo.UI.Base.MVVM;

namespace Neo.UI.MarkupExtensions
{
    public class DataContextBindingExtension : MarkupExtension
    {
        #region Public Properties 
        [ConstructorArgument("viewModel")]
        public Type ViewModel { get; set; }
        #endregion

        #region Constructor 
        public DataContextBindingExtension()
        {
            // NOP
        }

        public DataContextBindingExtension(Type viewModel)
        {
            this.ViewModel = viewModel;
        }
        #endregion

        #region MarkupExtension implementation 
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            var target = provideValueTarget?.TargetObject as FrameworkElement;

            if (target == null || DesignerProperties.GetIsInDesignMode(target)) return null;

            var viewModelInstance = ApplicationContext.Instance.ContainerLifetimeScope.Resolve(this.ViewModel);

            if (viewModelInstance is ILoadable loadableViewModel)
            {
                loadableViewModel.OnLoad();
            }

            return viewModelInstance;
        }
        #endregion
    }
}