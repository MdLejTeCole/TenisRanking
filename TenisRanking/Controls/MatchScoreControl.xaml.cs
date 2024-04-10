using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using TenisRankingDatabase;
using System.ComponentModel;
using TenisRankingDatabase.Tables;
using Microsoft.EntityFrameworkCore;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GameTools.Controls
{
    public sealed partial class MatchScoreControl : UserControl, INotifyPropertyChanged
    {
        private TenisRankingDbContext _dbContext;

        private Match _match;

        public Match Match
        {
            get { return _match; }
            set
            {
                if (_match != value)
                {
                    _match = value;
                    OnPropertyChanged(nameof(Match));
                }
            }
        }

        public bool Set2Enabled { get; set; }
        public bool Set3Enabled { get; set; }

        public long MatchId { get; set; }
        protected MatchScoreControl()
        {
            this.InitializeComponent();
        }

        public MatchScoreControl(TenisRankingDbContext dbContext, long matchId) : this()
        {
            _dbContext = dbContext;
            Match = _dbContext.Matches
                .Include(x => x.Tournament)
                .Include(x => x.PlayerMatches)
                    .ThenInclude(x => x.Player)
                .First(x => x.Id == matchId);
            if (Match.PlayerMatches.Count == 1)
            {
                Match.PlayerMatches.Add(new PlayerMatch());
            }
            Set2Enabled = Match.Tournament.NumberOfSets >= 2;
            Set3Enabled = Match.Tournament.NumberOfSets >= 3;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
