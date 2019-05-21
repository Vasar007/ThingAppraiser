﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ThingAppraiser.Data.Models;
using ThingAppraiser.DesktopApp.Domain.Commands;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal class StartControlViewModel : ViewModelBase
    {
        private string _selectedService;

        public IReadOnlyList<string> AvailableBeautifiedServices { get; } =
            ConfigContract.AvailableBeautifiedServices;

        public string SelectedService
        {
            get => _selectedService;
            set => SetProperty(ref _selectedService, value);
        }

        public object DialogIdentifier { get; }

        public ICommand InputThingDialogCommand =>
            new RelayCommand(ExecutableDialogs.ExecuteInputThingDialog);

        public ICommand OpenFileDialogCommand =>
            new RelayCommand(ExecutableDialogs.ExecuteOpenFileDialog);

        public ICommand EnterDataDialogCommand =>
            new RelayCommand(ExecutableDialogs.ExecuteEnterDataDialog);


        public StartControlViewModel(object dialogIdentifier)
        {
            DialogIdentifier = dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));

            SelectedService = AvailableBeautifiedServices.First();
        }
    }
}