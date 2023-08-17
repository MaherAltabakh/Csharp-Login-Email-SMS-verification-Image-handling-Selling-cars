using System.ComponentModel.DataAnnotations;

namespace BestPrice.Models
{
    public class Car : IComparable<Car>
    {
       
        private int year;

        public int CarID { get; set; }
        public int Odometer { get; set; }
        public int Year
        {
            get { return year; }
            set
            {
                if (value < DateTime.Now.Year)
                    year = value;
                else
                {
                    year = DateTime.Now.Year;
                }
            }
        }
        public int Price { get; set; }

        public int BetweenIntSearch { get; set; }
        public int ImageNumbersSort { get; set; }


        public List<IFormFile> ImageFile { get; set; }
        public List<string> ImageName { get; set; }


        public string CarModel { get; set; }
        public string CarMaker { get; set; }
        public string CarVinNumber { get; set; }
        public string CarColor { get; set; }
        public int CompareTo(Car car)
        {
            return this.ImageNumbersSort.CompareTo(car.ImageNumbersSort);
        }

    }
}






