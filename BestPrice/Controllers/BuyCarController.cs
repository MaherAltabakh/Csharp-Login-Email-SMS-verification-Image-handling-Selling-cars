using BestPrice.Models;
using Microsoft.AspNetCore.Mvc;
using SellBuyCars.Models;



namespace SellBuyCars.Controllers
{
    public class BuycarController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        public BuycarController(IWebHostEnvironment hostEnvironment)
        {
            this._hostEnvironment = hostEnvironment;

        }

        public IActionResult SellCar()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCarToSell(Car car)
        {
            var carDb = new CarDB();
            List<IFormFile> imageFiles = new List<IFormFile>();
            imageFiles = car.ImageFile;
            car.ImageName = new List<string>();
            if (imageFiles != null)
            {
                for (int i = 0; i < imageFiles.Count; i++)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = (Path.GetFileNameWithoutExtension(imageFiles[i].FileName));
                    string extension = Path.GetExtension(imageFiles[i].FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfffffff") + extension;
                    car.ImageName.Add(fileName);
                    string path = Path.Combine(wwwRootPath + "/UploadedImage/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await imageFiles[i].CopyToAsync(fileStream);
                    }
                }
            }
            carDb.AddNewCar(car);

            return View();
        }

        public IActionResult BuyCarList()
        {
            CarDB carDb = new CarDB();
            List<Car> cars = carDb.GetBuyCars();

            int carsCount = cars.Count;

            int[] OrderingNumber = new int[carsCount];
            for (int i = 0; i < carsCount; i++)
                OrderingNumber[i] = i + 1;

            List<int> imageCounts = new List<int>();
            for (int i = 0; i < cars.Count; i++)
            {
                imageCounts.Add(cars[i].ImageName.Count);
            }

            ViewBag.imageCount = imageCounts;

            ViewBag.cars = cars;
            ViewBag.carsCount = carsCount;
            ViewBag.Numbers = OrderingNumber;

            return View();
        }

        public IActionResult EditCar()
        {
            int carID = Convert.ToInt32(Request.Query["CarID"]);
            var car = new Car();
            CarDB carDb = new CarDB();
            car = carDb.GetBuyCarsByID(carID);
            ViewBag.car = car;
            ViewBag.ImageNumbers = car.ImageName.Count;
            ViewBag.ImageLineNumbers = Math.Ceiling(car.ImageName.Count / 3.0);
            return View();
        }

        public IActionResult EditImage(string imageName)
        {
            CarDB carDb = new CarDB();
            int carID1 = carDb.GetCarIDForThisImageName(imageName);

            ViewBag.imageName = imageName;
            ViewBag.carID = carID1;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveUpdatedImage()
        {
            List<string> newImageNames = new List<string>();
            CarDB carDb = new CarDB();
            Car car = new Car();
            int carID1 = 0;
            string imageName = Request.Form["imageName"];
            var imageFile = Request.Form.Files[0];
            if (imageName != null)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string path = Path.Combine(wwwRootPath + "/UploadedImage/", imageName);
                System.IO.File.Delete(path);

                carID1 = carDb.GetCarIDForThisImageName(imageName);
                car = carDb.GetBuyCarsByID(carID1);
            }
            if (imageFile != null)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                string extension = Path.GetExtension(imageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfffffff") + extension;

                string path = Path.Combine(wwwRootPath + "/UploadedImage/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                newImageNames.Add(fileName);
                carDb.UpdateImageNamesDB(newImageNames, imageName, carID1);
            }
            return RedirectToAction("EditCar", new { carID = carID1 });
        }

        public IActionResult RemoveImage(string imageName)
        {
            int carID1 = 0;
            if (imageName != null)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string path = Path.Combine(wwwRootPath + "/UploadedImage/", imageName);
                System.IO.File.Delete(path);
                CarDB carDb = new CarDB();
                carID1 = carDb.GetCarIDForThisImageName(imageName);
                carDb.RemoveImageFromDB(imageName);
            }

            return RedirectToAction("EditCar", new { carID = carID1 });


        }
        [HttpPost]
        public async Task<IActionResult> SaveUpdatedCar(Car car)
        {
            var carDb = new CarDB();
            List<IFormFile> imageFiles = new List<IFormFile>();

            imageFiles = car.ImageFile;
            car.ImageName = new List<string>();
            if (imageFiles != null)
            {
                for (int i = 0; i < imageFiles.Count; i++)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(imageFiles[i].FileName);
                    string extension = Path.GetExtension(imageFiles[i].FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfffffff") + extension;
                    string path = Path.Combine(wwwRootPath + "/UploadedImage/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await imageFiles[i].CopyToAsync(fileStream);
                    }

                    car.ImageName.Add(fileName);
                }
            }

            carDb.UpdateBuycarDB(car);

            return View();
        }

        public IActionResult RemoveCarConfirmation(int carID)
        {
            Car car = new Car();
            CarDB carDb = new CarDB();
            car = carDb.GetBuyCarsByID(carID);

            int imageCounts = car.ImageName.Count;
            ViewBag.imageCount = imageCounts;

            ViewBag.car = car;
            ViewBag.ImageNumbers = car.ImageName.Count;
            ViewBag.ImageLineNumbers = Math.Ceiling(car.ImageName.Count / 5.0);
            return View();
        }

        public IActionResult RemoveCar()
        {
            int carID = Convert.ToInt32(Request.Form["RemoveThisCarID"]);
            CarDB carDb = new CarDB();
            Car car = new Car();

            car = carDb.GetBuyCarsByID(carID);

            //Delete photos form the folder:
            List<string> imageNames = car.ImageName;
            if (imageNames != null)
            {
                for (int i = 0; i < imageNames.Count; i++)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string path = Path.Combine(wwwRootPath + "/UploadedImage/", imageNames[i]);
                    System.IO.File.Delete(path);
                }
            }
            carDb.RemoveCarAndImageFormDB(carID);
            return View();
        }

        public IActionResult ImageViews(int carID)
        {
            var car = new Car();
            CarDB carDb = new CarDB();
            car = carDb.GetBuyCarsByID(carID);
            ViewBag.car = car;
            ViewBag.ImageNumbers = car.ImageName.Count();

            return View();
        }

        public IActionResult SearchCarOptions()
        {
            return View();
        }

        public IActionResult SearchCarResult()
        {
            string searchFilter, txtsearchFilter, txtsearchFilterBetween, chkAddFilter_;
            List<Car> cars = new List<Car>();
            List<string> logicOperators = new List<string>();
            List<string> numericComparators = new List<string>();
            List<Car> searchResults = new List<Car>();

            var car = new Car();
            var carDb = new CarDB();

            //searchFilter == either car id model maker...
            //txtsearchFilter => take this value and put it in the corresponding field

            searchFilter = Request.Form["searchFilter1"];
            txtsearchFilter = Request.Form["txtsearchFilter1"];
            numericComparators.Add(Request.Form["numericComparator1"]);
            txtsearchFilterBetween = Request.Form["txtsearchFilterBetween1"];

            car = carDb.SearchCarFunction(searchFilter, txtsearchFilter, numericComparators[0], txtsearchFilterBetween);

            cars.Add(car);
            car = new Car();
            //================= End of the FIRST filter ==========================================//

            //if this AddFilter1 == chkAddFilter1
            //then save the operator form searchOperator1
            //and repeat the privious actions

            chkAddFilter_ = Request.Form["chkAddFilter1"];
            if (chkAddFilter_ == "AddFilter1")
            {
                logicOperators.Add(Request.Form["searchOperator2"]);
                numericComparators.Add(Request.Form["numericComparator2"]);

                searchFilter = Request.Form["searchFilter2"];
                txtsearchFilter = Request.Form["txtsearchFilter2"];
                txtsearchFilterBetween = Request.Form["txtsearchFilterBetween2"];

                car = carDb.SearchCarFunction(searchFilter, txtsearchFilter, numericComparators[1], txtsearchFilterBetween);
                cars.Add(car);
                car = new Car();
            }
            //================= End of the SECOND filter ==========================================//

            chkAddFilter_ = Request.Form["chkAddFilter2"];
            if (chkAddFilter_ == "AddFilter2")
            {
                logicOperators.Add(Request.Form["searchOperator3"]);
                numericComparators.Add(Request.Form["numericComparator3"]);

                searchFilter = Request.Form["searchFilter3"];
                txtsearchFilter = Request.Form["txtsearchFilter3"];
                txtsearchFilterBetween = Request.Form["txtsearchFilterBetween3"];

                car = carDb.SearchCarFunction(searchFilter, txtsearchFilter, numericComparators[2], txtsearchFilterBetween);
                cars.Add(car);
                car = new Car();
            }

            //================= End of the Third filter ==========================================//

            chkAddFilter_ = Request.Form["chkAddFilter3"];
            if (chkAddFilter_ == "AddFilter3")
            {
                logicOperators.Add(Request.Form["searchOperator4"]);
                numericComparators.Add(Request.Form["numericComparator4"]);

                searchFilter = Request.Form["searchFilter4"];
                txtsearchFilter = Request.Form["txtsearchFilter4"];
                txtsearchFilterBetween = Request.Form["txtsearchFilterBetween4"];

                car = carDb.SearchCarFunction(searchFilter, txtsearchFilter, numericComparators[3], txtsearchFilterBetween);
                cars.Add(car);
                car = new Car();
            }

            //================= End of the Forth filter ==========================================//

            chkAddFilter_ = Request.Form["chkAddFilter4"];
            if (chkAddFilter_ == "AddFilter4")
            {
                logicOperators.Add(Request.Form["searchOperator5"]);
                numericComparators.Add(Request.Form["numericComparator5"]);

                searchFilter = Request.Form["searchFilter5"];
                txtsearchFilter = Request.Form["txtsearchFilter5"];
                txtsearchFilterBetween = Request.Form["txtsearchFilterBetween5"];

                car = carDb.SearchCarFunction(searchFilter, txtsearchFilter, numericComparators[4], txtsearchFilterBetween);
                cars.Add(car);
                car = new Car();
            }

            //================= End of the Fifth filter ==========================================//


            chkAddFilter_ = Request.Form["chkAddFilter5"];
            if (chkAddFilter_ == "AddFilter5")
            {
                logicOperators.Add(Request.Form["searchOperator6"]);
                numericComparators.Add(Request.Form["numericComparator6"]);

                searchFilter = Request.Form["searchFilter6"];
                txtsearchFilter = Request.Form["txtsearchFilter6"];
                txtsearchFilterBetween = Request.Form["txtsearchFilterBetween6"];

                car = carDb.SearchCarFunction(searchFilter, txtsearchFilter, numericComparators[5], txtsearchFilterBetween);
                cars.Add(car);
                car = new Car();
            }

            //================= End of the Sixth filter ==========================================//


            chkAddFilter_ = Request.Form["chkAddFilter6"];
            if (chkAddFilter_ == "AddFilter6")
            {
                logicOperators.Add(Request.Form["searchOperator7"]);
                numericComparators.Add(Request.Form["numericComparator7"]);

                searchFilter = Request.Form["searchFilter7"];
                txtsearchFilter = Request.Form["txtsearchFilter7"];
                txtsearchFilterBetween = Request.Form["txtsearchFilterBetween7"];

                car = carDb.SearchCarFunction(searchFilter, txtsearchFilter, numericComparators[6], txtsearchFilterBetween);
                cars.Add(car);
                car = new Car();
            }

            //================= End of the Seventh filter ==========================================//


            chkAddFilter_ = Request.Form["chkAddFilter7"];
            if (chkAddFilter_ == "AddFilter7")
            {
                logicOperators.Add(Request.Form["searchOperator8"]);
                numericComparators.Add(Request.Form["numericComparator8"]);

                searchFilter = Request.Form["searchFilter8"];
                txtsearchFilter = Request.Form["txtsearchFilter8"];
                txtsearchFilterBetween = Request.Form["txtsearchFilterBetween8"];

                car = carDb.SearchCarFunction(searchFilter, txtsearchFilter, numericComparators[7], txtsearchFilterBetween);
                cars.Add(car);
                car = new Car();
            }

            //================= End of the Eighth filter ==========================================//

            chkAddFilter_ = Request.Form["chkAddFilter8"];
            if (chkAddFilter_ == "AddFilter8")
            {
                logicOperators.Add(Request.Form["searchOperator9"]);
                numericComparators.Add(Request.Form["numericComparator9"]);

                searchFilter = Request.Form["searchFilter9"];
                txtsearchFilter = Request.Form["txtsearchFilter9"];
                txtsearchFilterBetween = Request.Form["txtsearchFilterBetween9"];

                car = carDb.SearchCarFunction(searchFilter, txtsearchFilter, numericComparators[8], txtsearchFilterBetween);
                cars.Add(car);
                car = new Car();
            }

            //================= End of the Ninth filter ==========================================//

            chkAddFilter_ = Request.Form["chkAddFilter9"];
            if (chkAddFilter_ == "AddFilter9")
            {
                logicOperators.Add(Request.Form["searchOperator10"]);
                numericComparators.Add(Request.Form["numericComparator10"]);

                searchFilter = Request.Form["searchFilter10"];
                txtsearchFilter = Request.Form["txtsearchFilter10"];
                txtsearchFilterBetween = Request.Form["txtsearchFilterBetween10"];

                car = carDb.SearchCarFunction(searchFilter, txtsearchFilter, numericComparators[9], txtsearchFilterBetween);
                cars.Add(car);
                car = new Car();
            }

            //================= End of the Tenth filter ==========================================//

            searchResults = carDb.GetSearchedCars(cars, logicOperators, numericComparators);

            carDb.EmptyTemporaryDatabase();

            //Adding the search results (data and Image) to a Temporary database called: TemporarySearch/TemporaryImageNamesSearch
            foreach (Car carResult in searchResults)
            {
                carDb.AddSearchCarResult(carResult);
            }

            int carsCount = searchResults.Count;

            int[] OrderingNumber = new int[carsCount];
            for (int i = 0; i < carsCount; i++)
                OrderingNumber[i] = i + 1;

            ViewBag.Numbers = OrderingNumber;
            ViewBag.carsCount = carsCount;
            ViewBag.cars = searchResults;

            List<int> imageCounts = new List<int>();
            for (int i = 0; i < carsCount; i++)
            {
                imageCounts.Add(searchResults[i].ImageName.Count);
            }

            ViewBag.imageCount = imageCounts;

            return View();
        }

        public IActionResult SortCarList()
        {
            string sortFilter = Request.Form["sortCarList"];
            string sortTypes = Request.Form["sortType"];

            CarDB carDb = new CarDB();

            List<Car> sortedCarList = carDb.SortCarList(sortFilter, sortTypes);

            int carsCount = sortedCarList.Count;
            int[] OrderingNumber = new int[carsCount];
            for (int i = 0; i < carsCount; i++)
                OrderingNumber[i] = i + 1;

            List<int> imageCounts = new List<int>();
            for (int i = 0; i < carsCount; i++)
            {
                imageCounts.Add(sortedCarList[i].ImageName.Count);
            }
            ViewBag.imageCount = imageCounts;

            ViewBag.Numbers = OrderingNumber;
            ViewBag.carsCount = carsCount;
            ViewBag.cars = sortedCarList;
            return View();
        }

        public IActionResult SortCarSearchList()
        {
            string sortFilter = Request.Form["sortCarList"];
            string sortTypes = Request.Form["sortType"];

            CarDB carDb = new CarDB();

            List<Car> SortedSearchCarList = carDb.SortedSearchResult(sortFilter, sortTypes);


            int carsCount = SortedSearchCarList.Count;

            int[] OrderingNumber = new int[carsCount];
            for (int i = 0; i < carsCount; i++)
                OrderingNumber[i] = i + 1;

            List<int> imageCounts = new List<int>();
            for (int i = 0; i < carsCount; i++)
            {
                imageCounts.Add(SortedSearchCarList[i].ImageName.Count);
            }
            ViewBag.imageCount = imageCounts;

            ViewBag.Numbers = OrderingNumber;
            ViewBag.carsCount = carsCount;
            ViewBag.cars = SortedSearchCarList;



            return View();
        }
    }
}


