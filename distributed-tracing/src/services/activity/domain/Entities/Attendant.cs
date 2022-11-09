using domain.Enums;

namespace domain.Entities
{
    public class Attendant
    {
        private Attendant()
        {
        }

        public int Id { get; private set; }
        public string IdentityNumber { get; private set; }
        public string FullName { get; private set; }
        public AttendentStatus AttendentStatus { get; private set; }


        public static Attendant CreateAttendant(string fullName, string identityNo)
        {
            var attendant = new Attendant();
            attendant.FullName = fullName;
            attendant.IdentityNumber = identityNo;
            attendant.AttendentStatus = domain.Enums.AttendentStatus.PaymentWaiting;
            return attendant;
        }

        public void ConfirmAttendant()
        {
            this.AttendentStatus = domain.Enums.AttendentStatus.PaymentConfirmed;
        }
    }
}
