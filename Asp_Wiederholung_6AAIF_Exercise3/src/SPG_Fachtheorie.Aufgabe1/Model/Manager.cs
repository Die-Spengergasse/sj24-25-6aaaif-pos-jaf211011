using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Manager : Employee
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        protected Manager() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public  Manager(int registrationNumber, string firstName, string lastName,
            Address? address, string carType, int registationNumber) : base(registrationNumber, firstName, lastName, address)
        {
            CarType = carType;
            RegistationNumber = registationNumber;
        }

        [MaxLength(255)]
        public string CarType { get; set; }
        public int RegistationNumber { get; set; }
    }
}