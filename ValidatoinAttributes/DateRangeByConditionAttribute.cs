using System.ComponentModel.DataAnnotations;

namespace Doctor_Appointment.ValidatoinAttributes
{
    public class DateRangeByConditionAttribute : ValidationAttribute
    {
        DateTime _minRange, _maxRange;


        public DateRangeByConditionAttribute()
        {
            _minRange = DateTime.Now.AddDays(1);
            _maxRange = DateTime.Now.AddYears(1);
        }


        public string GetErrorMessage() =>
            $"Date should be between {_minRange.ToShortDateString()} and {_maxRange.ToShortDateString()}.";

        protected override ValidationResult? IsValid(object? value,
            ValidationContext validationContext)
        {
            var number = value as DateTime?;
            if (number is not null)
            {
                if (number.Value.Date >= _maxRange.Date || number.Value.Date < _minRange.Date)
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }



            return ValidationResult.Success;
        }
    }
}
