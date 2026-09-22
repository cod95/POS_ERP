using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS.Domain.Models.Payments
{
    public class PurchasePayment : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public int PurchaseId { get; set; } // Foreign key for Purchase

        [ForeignKey(nameof(PurchaseId))]
        public Purchase Purchase { get; set; } // Navigation property for Purchase

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set
            {
                if (_amount != value)
                {
                    _amount = value;
                    NotifyPropertyChanged(nameof(Amount));
                }
            }
        }

        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set
            {
                if (_date != value)
                {
                    _date = value;
                    NotifyPropertyChanged(nameof(Date));
                }
            }
        }

        private Currency _currency = Currency.USD;
        public Currency Currency
        {
            get => _currency;
            set
            {
                if (_currency != value)
                {
                    _currency = value;
                    NotifyPropertyChanged(nameof(Currency));
                }
            }
        }

        private decimal _exchangeRate = 1m;
        [Column(TypeName = "decimal(18, 6)")]
        public decimal ExchangeRate
        {
            get => _exchangeRate;
            set
            {
                if (_exchangeRate != value)
                {
                    _exchangeRate = value;
                    NotifyPropertyChanged(nameof(ExchangeRate));
                }
            }
        }

        private PaymentType _paymentType;
        public PaymentType PaymentType
        {
            get => _paymentType;
            set
            {
                if (_paymentType != value)
                {
                    _paymentType = value;
                    NotifyPropertyChanged(nameof(PaymentType));
                }
            }
        }


    }
}
