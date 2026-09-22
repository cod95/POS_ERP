using Microsoft.EntityFrameworkCore;
using POS.Domain.Models;
using POS.Persistence.Context;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace POS.ViewModels
{
    public class CurrencyRateViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _db;
        private decimal _lbpRate;
        private string _notes;
        private DateTime _effectiveDate = DateTime.Now;

        public decimal LbpRate
        {
            get => _lbpRate;
            set { if (_lbpRate != value) { _lbpRate = value; OnPropertyChanged(nameof(LbpRate)); } }
        }

        public string Notes
        {
            get => _notes;
            set { if (_notes != value) { _notes = value; OnPropertyChanged(nameof(Notes)); } }
        }

        public DateTime EffectiveDate
        {
            get => _effectiveDate;
            set { if (_effectiveDate != value) { _effectiveDate = value; OnPropertyChanged(nameof(EffectiveDate)); } }
        }

        public ObservableCollection<CurrencyRate> Rates { get; } = new();

        public ICommand SaveCommand { get; }
        public ICommand RefreshCommand { get; }

        public CurrencyRateViewModel()
        {
            _db = new AppDbContext();
            SaveCommand = new RelayCommand(_ => Save());
            RefreshCommand = new RelayCommand(_ => Load());
            Load();
        }

        private void Load()
        {
            Rates.Clear();

            var latestLbp = _db.CurrencyRates
                .AsNoTracking()
                .Where(x => x.Currency == Currency.LBP)
                .OrderByDescending(x => x.EffectiveDate)
                .ThenByDescending(x => x.Id)
                .FirstOrDefault();

            if (latestLbp != null)
            {
                LbpRate = latestLbp.Rate;
                Notes = latestLbp.Notes;
                EffectiveDate = latestLbp.EffectiveDate;
            }

            foreach (var rate in _db.CurrencyRates
                .AsNoTracking()
                .OrderByDescending(x => x.EffectiveDate)
                .ThenByDescending(x => x.Id)
                .Take(50))
            {
                Rates.Add(rate);
            }
        }

        private void Save()
        {
            if (LbpRate <= 0)
            {
                MessageBox.Show("يرجى إدخال سعر صرف صحيح لليرة اللبنانية.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _db.CurrencyRates.Add(new CurrencyRate
            {
                Currency = Currency.LBP,
                Rate = LbpRate,
                EffectiveDate = EffectiveDate,
                Notes = Notes?.Trim()
            });

            _db.SaveChanges();
            Load();

            MessageBox.Show("تم حفظ سعر الصرف بنجاح.", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
