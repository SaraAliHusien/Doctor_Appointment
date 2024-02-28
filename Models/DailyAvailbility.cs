using Doctor_Appointment.ValidatoinAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Doctor_Appointment.Models
{
    public class DailyAvailbility
    {


        [Key]
        public int Dayid { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        [DateRangeByCondition]
        public DateTime Date { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        [Range(1, 100)]
        public int countAppointment { get; set; }
        public int BookedAppointments { get; set; } = 0;

        [ForeignKey("Doctor")]
        public int DoctorID { get; set; }
        public virtual Doctor? Doctor { get; set; }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            DateTime.Parse(Date.ToString());

            str.Append(Date.DayOfWeek.ToString()).Append(" ,").
                Append($"{Date.ToString("m")}, ")
                .Append(" from: ").Append(StartTime.ToString(@"hh\:mm")).Append(" To ").Append(EndTime.ToString(@"hh\:mm"));

            return str.ToString();
        }

    }
}
