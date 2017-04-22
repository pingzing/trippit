using DigiTransit10.Helpers;
using DigiTransit10.Models;
using DigiTransit10.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;

namespace DigiTransit10.ViewModels.ControlViewModels
{
    public class LineSearchContentViewModel : BindableBase
    {        
        private readonly INetworkService _networkService;

        private ObservableCollection<LineSearchElementViewModel> _linesResultList = new ObservableCollection<LineSearchElementViewModel>();
        public ObservableCollection<LineSearchElementViewModel> LinesResultList
        {
            get { return _linesResultList; }
            set { Set(ref _linesResultList, value); }
        }

        private string _linesSearchBoxText;
        public string LinesSearchBoxText
        {
            get { return _linesSearchBoxText; }
            set { Set(ref _linesSearchBoxText, value); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }

        public LineSearchContentViewModel(INetworkService network, string title)
        {            
            _networkService = network;
            Title = title;
        }
        
        public async Task GetLinesAsync(string searchText, CancellationToken token)
        {
            var response = await _networkService.GetLinesAsync(searchText, token);
            if (response.IsFailure)
            {
                return;
            }
            if (token.IsCancellationRequested)
            {
                return;
            }
            LinesResultList = new ObservableCollection<LineSearchElementViewModel>(response.Result.Select(x => new LineSearchElementViewModel { BackingLine = x }));
        }
    }
}
