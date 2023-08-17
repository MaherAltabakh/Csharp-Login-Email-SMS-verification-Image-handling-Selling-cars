using BestPrice.Models;
using Dapper;
using System.Data.SqlClient;

namespace SellBuyCars.Models
{
    public class CarDB
    {
        private string connectionString = ("server = Administrator; database=BestPrice; user id= reader; password= pass123;");



        public List<Car> ReadFromDB(string sql)
        {
            List<Car> cars = new List<Car>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Car car = new Car();
                        car.CarID = Convert.ToInt32(reader[0].ToString());
                        car.Odometer = Convert.ToInt32(reader[1].ToString());
                        car.Year = Convert.ToInt32(reader[2].ToString());
                        car.Price = Convert.ToInt32(reader[3].ToString());
                        car.CarModel = reader[7].ToString();
                        car.CarMaker = reader[8].ToString();
                        car.CarVinNumber = reader[9].ToString();
                        car.CarColor = reader[10].ToString();


                        cars.Add(car);
                    }
                    reader.Close();
                    conn.Close();
                }
            }
            return cars;
        }

        public List<ImageModel> ReadImageNames(string sql)
        {
            List<ImageModel> imagesData = new List<ImageModel>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ImageModel image = new ImageModel();
                        image.ImageNamesID = Convert.ToInt32(reader[0].ToString());
                        image.CarID = Convert.ToInt32(reader[1].ToString());
                        image.ImageName = reader[2].ToString();

                        imagesData.Add(image);
                    }
                    reader.Close();
                    conn.Close();
                }
            }
            return imagesData;
        }

        public void UpdatecarDB(string sql)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public void UpdateImageNamesDB(List<string> newImageName, string oldImageName, int carID)
        {
            foreach (string imageName in newImageName)
            {
                string sql = $"UPDATE ImageNames SET  ImageName = REPLACE(ImageName, '{oldImageName}', '{imageName}') WHERE CarID={carID}";
                UpdatecarDB(sql);
            }
        }

        public int CreatNewCar(string sql, int LastInsertedID = 0)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
                if (LastInsertedID == 0)
                {
                    string sqlLastInsertedID = "SELECT SCOPE_IDENTITY()";
                    using (SqlCommand cmd = new SqlCommand(sqlLastInsertedID, conn))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            LastInsertedID = Convert.ToInt32(reader[0].ToString());
                        }
                        reader.Close();
                    }
                    conn.Close();
                }
            }
            return LastInsertedID;
        }
        public List<Car> GetBuyCars()
        {
            string sql = "select * from BuyCar";
            List<Car> cars = ReadFromDB(sql);

            string sqlImageNames = $"Select * from ImageNames";
            List<ImageModel> imagesData = ReadImageNames(sqlImageNames);

            foreach (Car car in cars)
            {
                car.ImageName = new List<string>();
                foreach (ImageModel image in imagesData)
                    if (image.CarID == car.CarID)
                        car.ImageName.Add(image.ImageName);
            }
            return cars;
        }

        public Car GetBuyCarsByID(int CarID)
        {
            List<Car> cars = new List<Car>();
            string sql = "select * from BuyCar where CarID = " + CarID.ToString();
            cars = ReadFromDB(sql);

            List<ImageModel> imagesData = new List<ImageModel>();
            string sqlImageNames = "Select * from ImageNames where CarID = " + CarID.ToString();
            imagesData = ReadImageNames(sqlImageNames);

            Car car = new Car();

            foreach (Car car1 in cars)
            {
                car1.ImageName = new List<string>();
                foreach (ImageModel image in imagesData)
                    if (image.CarID == car1.CarID)
                        car1.ImageName.Add(image.ImageName);
            }

            car.CarID = cars[0].CarID;
            car.CarMaker = cars[0].CarMaker;
            car.CarModel = cars[0].CarModel;
            car.Year = cars[0].Year;
            car.Odometer = cars[0].Odometer;
            car.CarVinNumber = cars[0].CarVinNumber;
            car.CarColor = cars[0].CarColor;
            car.Price = cars[0].Price;
            car.ImageName = new List<string>();
            foreach (string imageName in cars[0].ImageName)
                car.ImageName.Add(imageName);

            return car;
        }

        public void UpdateBuycarDB(Car Newcar)
        {
            string sql = "update BuyCar set " +
               "CarMaker= '" + Newcar.CarMaker + "'," +
               "CarModel= '" + Newcar.CarModel + "'," +
               "Year= " + Newcar.Year.ToString() + "," +
               "Odometer= " + Newcar.Odometer.ToString() + "," +
               "CarVinNumber= '" + Newcar.CarVinNumber + "'," +
               "CarColor= '" + Newcar.CarColor + "'," +
               "Price= " + Newcar.Price.ToString() +
               " where CarID = " + Newcar.CarID.ToString();
            UpdatecarDB(sql);
            string tableName = "ImageNames";
            AddImageNames(Newcar.ImageName, Newcar.CarID, tableName);
        }

        public void AddNewCar(Car Newcar)
        {
            string sql1 = "insert into BuyCar (CarMaker,CarModel,Year,Odometer,CarVinNumber,CarColor, Price) values ('" +
                Newcar.CarMaker + "','" + Newcar.CarModel + "'," + Newcar.Year + "," + Newcar.Odometer +
                ",'" + Newcar.CarVinNumber + "','" + Newcar.CarColor + "'," + Newcar.Price + ")";
            int InsertedID = CreatNewCar(sql1);
            string tableName = "ImageNames";
            AddImageNames(Newcar.ImageName, InsertedID, tableName);
        }

        public void AddImageNames(List<string> iamgeNames, int CarID, string tableName)
        {
            if (iamgeNames.Count != 0)
            {
                foreach (string imageName in iamgeNames)
                {
                    string sql = $"insert into {tableName} (CarID,ImageName) values ({CarID},'{imageName}')";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                }
            }
        }

        public void RemoveCarAndImageFormDB(int carID)
        {
            string sql = $"DELETE FROM BuyCar WHERE CarID = {carID}";
            UpdatecarDB(sql);
            sql = $"DELETE FROM ImageNames WHERE CarID = {carID}";
            UpdatecarDB(sql);
        }

        public void RemoveImageFromDB(string imageName)
        {
            string sql = $"DELETE FROM ImageNames WHERE ImageName = '{imageName}'";
            UpdatecarDB(sql);
        }

        public int GetCarIDForThisImageName(string imageName)
        {
            ImageModel image = new ImageModel();

            string sql = $"select * FROM ImageNames WHERE ImageName = '{imageName}'";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        image.ImageNamesID = Convert.ToInt32(reader[0].ToString());
                        image.CarID = Convert.ToInt32(reader[1].ToString());
                        image.ImageName = reader[2].ToString();
                    }
                    reader.Close();
                    conn.Close();
                }
            }
            return image.CarID;
        }

        public Car SearchCarFunction(string searchFilter, string txtsearchFilter, string numericComparator = "", string txtsearchFilterBetween = "")
        {
            Car car = new Car();


            switch (searchFilter)
            {
                case "CarID":
                    {
                        car.CarID = Convert.ToInt32(txtsearchFilter);
                        if (numericComparator == "between")
                            car.BetweenIntSearch = Convert.ToInt32(txtsearchFilterBetween);
                        break;
                    }
                case "CarMaker":
                    {
                        car.CarMaker = txtsearchFilter;
                        break;
                    }
                case "CarModel":
                    {
                        car.CarModel = txtsearchFilter;
                        break;
                    }
                case "Year":
                    {
                        car.Year = Convert.ToInt32(txtsearchFilter);
                        if (numericComparator == "between")
                            car.BetweenIntSearch = Convert.ToInt32(txtsearchFilterBetween);
                        break;
                    }
                case "Odometer":
                    {
                        car.Odometer = Convert.ToInt32(txtsearchFilter);
                        if (numericComparator == "between")
                            car.BetweenIntSearch = Convert.ToInt32(txtsearchFilterBetween);
                        break;
                    }
                case "CarVinNumber":
                    {
                        car.CarVinNumber = txtsearchFilter;
                        break;
                    }
                case "CarColor":
                    {
                        car.CarColor = txtsearchFilter;
                        break;
                    }
                case "Price":
                    {
                        car.Price = Convert.ToInt32(txtsearchFilter);
                        if (numericComparator == "between")
                            car.BetweenIntSearch = Convert.ToInt32(txtsearchFilterBetween);
                        break;
                    }
            }
            return car;
        }

        public void EmptyTemporaryDatabase()
        {
            string sqlDeleteTempSearch = "DELETE FROM TemporarySearch WHERE (carID>=1)";
            UpdatecarDB(sqlDeleteTempSearch);
            sqlDeleteTempSearch = "DELETE FROM TemporaryImageNamesSearch WHERE (carID>=1)";
            UpdatecarDB(sqlDeleteTempSearch);
        }

        public List<Car> GetSearchedCars(List<Car> cars, List<string> logicOperators, List<string> numericComparators)
        {
            List<Car> searchResults = new List<Car>();

            int filterNumbers = cars.Count;
            int operatorNumbers = logicOperators.Count;
            int orIndicator = 0;

            string sql = "", space = "";

            string[] sql1 = new string[filterNumbers];
            string[] sqlCarID = new string[filterNumbers];
            string[] sqlCarMaker = new string[filterNumbers];
            string[] sqlCarModel = new string[filterNumbers];
            string[] sqlYear = new string[filterNumbers];
            string[] sqlOdometer = new string[filterNumbers];
            string[] sqlCarVinNumber = new string[filterNumbers];
            string[] sqlCarColor = new string[filterNumbers];
            string[] sqlPrice = new string[filterNumbers];

            for (int i = 0; i < cars.Count; i++)
            {
                if (cars[i].CarID != 0)
                {
                    switch (numericComparators[i])
                    {
                        case "equalTo":
                            {
                                sqlCarID[i] = $"(CarID = {cars[i].CarID})";
                                break;
                            }
                        case "greaterThan":
                            {
                                sqlCarID[i] = $"(CarID > {cars[i].CarID})";
                                break;
                            }
                        case "lessThan":
                            {
                                sqlCarID[i] = $"(CarID < {cars[i].CarID})";
                                break;
                            }
                        case "between":
                            {
                                sqlCarID[i] = $"(CarID BETWEEN {cars[i].CarID} and {cars[i].BetweenIntSearch})";

                                break;
                            }
                    }
                }
                if (cars[i].CarMaker != null)
                    sqlCarMaker[i] = $"(CarMaker = '{cars[i].CarMaker}')";
                if (cars[i].CarModel != null)
                    sqlCarModel[i] = $"(CarModel = '{cars[i].CarModel}')";
                if (cars[i].Year != 0)
                {
                    switch (numericComparators[i])
                    {
                        case "equalTo":
                            {
                                sqlYear[i] = $"(Year = {cars[i].Year})";
                                break;
                            }
                        case "greaterThan":
                            {
                                sqlYear[i] = $"(Year > {cars[i].Year})";
                                break;
                            }
                        case "lessThan":
                            {
                                sqlYear[i] = $"(Year < {cars[i].Year})";
                                break;
                            }
                        case "between":
                            {
                                sqlYear[i] = $"(Year BETWEEN {cars[i].Year} and {cars[i].BetweenIntSearch})";
                                break;
                            }
                    }
                }
                if (cars[i].Odometer != 0)
                {
                    switch (numericComparators[i])
                    {
                        case "equalTo":
                            {
                                sqlOdometer[i] = $"(Odometer = {cars[i].Odometer})";
                                break;
                            }
                        case "greaterThan":
                            {
                                sqlOdometer[i] = $"(Odometer > {cars[i].Odometer})";
                                break;
                            }
                        case "lessThan":
                            {
                                sqlOdometer[i] = $"(Odometer < {cars[i].Odometer})";
                                break;
                            }
                        case "between":
                            {
                                sqlOdometer[i] = $"(Odometer BETWEEN {cars[i].Odometer} and {cars[i].BetweenIntSearch})";
                                break;
                            }
                    }
                }
                if (cars[i].CarVinNumber != null)
                    sqlCarVinNumber[i] = $"(CarVinNumber = '{cars[i].CarVinNumber}')";
                if (cars[i].CarColor != null)
                    sqlCarColor[i] = $"(CarColor = '{cars[i].CarColor}')";
                if (cars[i].Price != 0)
                {
                    switch (numericComparators[i])
                    {
                        case "equalTo":
                            {
                                sqlPrice[i] = $"(Price = {cars[i].Price})";
                                break;
                            }
                        case "greaterThan":
                            {
                                sqlPrice[i] = $"(Price > {cars[i].Price})";
                                break;
                            }
                        case "lessThan":
                            {
                                sqlPrice[i] = $"(Price < {cars[i].Price})";
                                break;
                            }
                        case "between":
                            {
                                sqlPrice[i] = $"(Price BETWEEN {cars[i].Price} and {cars[i].BetweenIntSearch})";
                                break;
                            }
                    }
                }
                if (i < logicOperators.Count)
                {
                    if (logicOperators[i] == "or")
                    {
                        if (orIndicator == 0)
                        {
                            sql1[i] = "( " + sqlCarID[i] + sqlCarMaker[i] + sqlCarModel[i] + sqlYear[i] + sqlOdometer[i] + sqlCarVinNumber[i] + sqlCarColor[i] + sqlPrice[i] + " " + logicOperators[i] + " ";
                            orIndicator++;
                        }
                        else
                            sql1[i] = sqlCarID[i] + sqlCarMaker[i] + sqlCarModel[i] + sqlYear[i] + sqlOdometer[i] + sqlCarVinNumber[i] + sqlCarColor[i] + sqlPrice[i] + " " + logicOperators[i] + " ";
                    }
                    else
                    {
                        if (orIndicator != 0)
                        {
                            //here after the or
                            sql1[i] = sqlCarID[i] + sqlCarMaker[i] + sqlCarModel[i] + sqlYear[i] + sqlOdometer[i] + sqlCarVinNumber[i] + sqlCarColor[i] + sqlPrice[i] + ") " + logicOperators[i] + " ";
                            orIndicator = 0;
                        }
                        else
                            sql1[i] = sqlCarID[i] + sqlCarMaker[i] + sqlCarModel[i] + sqlYear[i] + sqlOdometer[i] + sqlCarVinNumber[i] + sqlCarColor[i] + sqlPrice[i] + " " + logicOperators[i] + " ";
                    }
                }
                else
                {
                    if (orIndicator != 0)
                    {
                        //here after the or
                        sql1[i] = sqlCarID[i] + sqlCarMaker[i] + sqlCarModel[i] + sqlYear[i] + sqlOdometer[i] + sqlCarVinNumber[i] + sqlCarColor[i] + sqlPrice[i] + ") ";
                        orIndicator = 0;
                    }
                    else
                        sql1[i] = sqlCarID[i] + sqlCarMaker[i] + sqlCarModel[i] + sqlYear[i] + sqlOdometer[i] + sqlCarVinNumber[i] + sqlCarColor[i] + sqlPrice[i];
                }
            }

            sql = "select * from BuyCar where " + string.Join(space, sql1, 0, filterNumbers);

            searchResults = ReadFromDB(sql);

            List<ImageModel> imagesData = new List<ImageModel>();

            foreach (var searchResult in searchResults)
            {
                searchResult.ImageName = new List<string>();

                string sqlImageNames = "Select * from ImageNames where CarID = " + searchResult.CarID.ToString();
                imagesData = ReadImageNames(sqlImageNames);

                foreach (ImageModel image in imagesData)
                    if (image.CarID == searchResult.CarID)
                        searchResult.ImageName.Add(image.ImageName);
            }
            return searchResults;
        }

        public void AddSearchCarResult(Car Newcar)
        {

            string sql = "insert into TemporarySearch (CarID,CarMaker,CarModel,Year,Odometer,CarVinNumber,CarColor,Price) values (" +
                Newcar.CarID + ",'" + Newcar.CarMaker + "','" + Newcar.CarModel + "'," + Newcar.Year + "," + Newcar.Odometer +
                ",'" + Newcar.CarVinNumber + "','" + Newcar.CarColor + "'," + Newcar.Price + ")";
            int InsertedID = CreatNewCar(sql, Newcar.CarID);
            string tableName = "TemporaryImageNamesSearch";
            AddImageNames(Newcar.ImageName, Newcar.CarID, tableName);
        }









        public List<Car> SortedSearchResult(string sortFilter, string sortTypes)
        {
            string sql;
            List<Car> sortedcarList = new List<Car>();

            if (sortFilter != "ImageNumbers")
            {
                sql = $"select * from TemporarySearch ORDER BY {sortFilter} {sortTypes}";
            }
            else
            {
                sql = "select * from TemporarySearch";
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                sortedcarList = conn.Query<Car>(sql).ToList();
            }


            //{
            //    conn.Open();
            //    using (SqlCommand cmd = new SqlCommand(sql, conn))
            //    {
            //        SqlDataReader reader = cmd.ExecuteReader();
            //        if (reader != null)
            //        {
            //            while (reader.Read())
            //            {
            //                Car car = new Car();
            //                car.CarID = Convert.ToInt32(reader[1].ToString());
            //                car.CarMaker = reader[2].ToString();
            //                car.CarModel = reader[3].ToString();
            //                car.Year = Convert.ToInt32(reader[4].ToString());
            //                car.Odometer = Convert.ToInt32(reader[5].ToString());
            //                car.CarVinNumber = reader[6].ToString();
            //                car.CarColor = reader[7].ToString();
            //                car.Price = Convert.ToInt32(reader[8].ToString());

            //                sortedcarList.Add(car);
            //            }
            //        }
            //        reader.Close();
            //        conn.Close();
            //    }
            //}

            string sqlImageNames = $"select * from TemporaryImageNamesSearch";
            List<ImageModel> imagesData = ReadImageNames(sqlImageNames);

            foreach (Car car in sortedcarList)
            {
                car.ImageName = new List<string>();
                foreach (ImageModel image in imagesData)
                    if (image.CarID == car.CarID)
                        car.ImageName.Add(image.ImageName);
            }
            if (sortFilter == "ImageNumbers")
            {
                foreach (Car car in sortedcarList)
                    car.ImageNumbersSort = car.ImageName.Count;
                if (sortTypes == "ASC")
                {
                    sortedcarList.Sort();
                }
                if (sortTypes == "DESC")
                {
                    sortedcarList.Sort((x, y) => y.CompareTo(x));//Reverse sort
                }
            }
            return sortedcarList;
        }

        public List<Car> SortCarList(string sortFilter, string sortTypes)
        {
            string sql;
            List<Car> sortedcarList = new List<Car>();
            if (sortFilter != "ImageNumbers")
            {
                sql = $"select * from BuyCar ORDER BY {sortFilter} {sortTypes}";
            }
            else
            {
                sql = "select * from BuyCar";
            }
            sortedcarList = ReadFromDB(sql);

            string sqlImageNames;
            sqlImageNames = "Select * from ImageNames";

            List<ImageModel> imagesData = ReadImageNames(sqlImageNames);
            foreach (Car car in sortedcarList)
            {
                car.ImageName = new List<string>();
                foreach (ImageModel image in imagesData)
                    if (image.CarID == car.CarID)
                        car.ImageName.Add(image.ImageName);
            }
            if (sortFilter == "ImageNumbers")
            {
                /*SOrt by using dictionary:
                           if (sortFilter == "ImageNumbers")
                     {
                         var SortedDictionary_ = new Dictionary<Car, int>();
             var imagesNumberForEachCar = new Dictionary<Car, int>
                 ();
                 foreach (Car car in sortedcarList)
                 imagesNumberForEachCar.Add(car, car.ImageName.Count);
                 if (sortTypes == "ASC")
                 {
                 SortedDictionary_ = imagesNumberForEachCar.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
                 }
                 if (sortTypes == "DESC")
                 {
                 SortedDictionary_ = imagesNumberForEachCar.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
                 }
                 int i = 0;
                 sortedcarList = new List<Car>
                     ();
                     foreach (var car in SortedDictionary_.Keys)
                     {
                     sortedcarList.Insert(i,car);
                     i++;
                     } */

                foreach (Car car in sortedcarList)
                    car.ImageNumbersSort = car.ImageName.Count;
                if (sortTypes == "ASC")
                {
                    sortedcarList.Sort();
                }
                if (sortTypes == "DESC")
                {
                    sortedcarList.Sort((x, y) => y.CompareTo(x));//Reverse sort
                }
            }
            return sortedcarList;
        }



    }


}