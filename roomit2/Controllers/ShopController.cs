using ExcelDataReader; // 1
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using roomit2.Models;
using System.Data; // 2
using System.Data.OleDb;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.IO; // 3
using System.Reflection.Emit;
using System.Text.Encodings.Web;
using System.Text;
using System.Text.Json;
using SessionExtensions = roomit2.Models.SessionExtensions;
using MimeKit;
using System.Net;
using Microsoft.Net.Http.Headers;
using Microsoft.CodeAnalysis;
using Org.BouncyCastle.Tsp;




namespace roomit2.Controllers
{
    public class ShopController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        //private readonly IUserStore<ApplicationUser> _userStore;
        //private readonly IUserEmailStore<ApplicationUser> _emailStore;

        //private readonly IEmailSender _emailSender;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ILogger<ShopController> _logger;
        private readonly IMemoryCache _dbCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly RoomitContext _context;
        public string sqlMessage;
        bool update = false;
        Warenkorb? warenkorbImSession;
        List<Warenkorb>? Warenkorbitems;
        List<Product> ProduktListe;

        public ShopController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<ShopController> logger,
            IMemoryCache dbCache,
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration,
            RoomitContext context,
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory)
        {
            _signInManager = signInManager;
            _userManager = userManager;

            _logger = logger;
            _dbCache = dbCache;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _context = context;
            _context.Database.EnsureCreated();
            ProduktListe = new List<Product>();
            //Warenkorbitems = new List<Warenkorb>();
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory= httpClientFactory;

        }

        public async Task<IActionResult> Index()
        {
            string excelPath = "Excel/PLZ-Matrix Köln2.xlsx";
            var dataSet = ExcelToDataTable(excelPath);
            var isDone = ImportDataTableToSqlServer(dataSet);

            bool isProduktListeExist = _dbCache.TryGetValue<List<Product>>("ProduktListe", out ProduktListe);

            if (!isProduktListeExist)
            {
                var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromHours(23),
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(23),
                };
                //leistungsortePostsListe = new List<LeistungsOrtePost>();

                ProduktListe = _context.Produkte.ToList();

                _dbCache.Set<IEnumerable<Product>>("ProduktListe", ProduktListe, memoryCacheEntryOptions);
            }
            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            ISession? session = _httpContextAccessor?.HttpContext?.Session;
            warenkorbImSession = SessionExtensions.Get<Warenkorb>(session, ip);

            if (warenkorbImSession != null)
            {
                TempData["Quantity"] = warenkorbImSession?.WarenkorbItemsListe.Count;
                TempData["TotalAmount"] = warenkorbImSession?.WarenkorbItemsListe.Sum(x => x.Total);
            }

            return View();
        }

        //public DataTable ExcelToDataTable(string path)
        //{
        //    try
        //    {
        //        DataTable datatable = new DataTable();
        //        var constr = _configuration.GetConnectionString("ExcelConnection");


        //        constr = string.Format(constr, path);

        //        using (OleDbConnection connExcel = new OleDbConnection(constr))
        //        {
        //            using (OleDbCommand cmdExcel = new OleDbCommand())
        //            {
        //                using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
        //                {

        //                    cmdExcel.Connection = connExcel;

        //                    connExcel.Open();

        //                    DataTable excelschema;

        //                    excelschema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        //                    var sheetname = excelschema.Rows[0]["Table_Name"].ToString();
        //                    connExcel.Close();

        //                    //Read Data from First Sheet.
        //                    connExcel.Open();
        //                    cmdExcel.CommandText = "SELECT * From [" + sheetname + "]";
        //                    odaExcel.SelectCommand = cmdExcel;
        //                    odaExcel.Fill(datatable);
        //                    connExcel.Close();

        //                }
        //            }

        //        }

        //        return datatable;
        //    }
        //    catch (Exception e)
        //    {
        //        return null;
        //        throw;
        //    }

        //}


        public DataTable ExcelToDataTable(string path)
        {
            try
            {
                // Erlaubt das Lesen von Excel-Dateien in neueren .NET-Versionen
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                //var excelConnection = _configuration.GetConnectionString("ExcelConnection");
                //string path2 = string.Format(excelConnection, path);
                string path3 = Path.Combine(_webHostEnvironment.WebRootPath, path);
                //string path4 = string.Format(_webHostEnvironment.ContentRootPath, path);

                using (var stream = System.IO.File.Open(path3, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // Konfiguriert den Reader so, dass die erste Zeile als Spaltenname genutzt wird
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });

                        // Gibt das erste Tabellenblatt (Sheet) als DataTable zurück
                        if (result.Tables.Count > 0)
                        {
                            return result.Tables[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Hier kannst du den Fehler loggen, falls beim Laden etwas schiefgeht
                System.Diagnostics.Debug.WriteLine($"Excel-Fehler: {ex.Message}");
                throw;
            }

            return null;
        }


        //public bool ImportDataTableToSqlServer(DataTable myDataSet)
        //{
        //    try
        //    {
        //        var sqlconn = _configuration.GetConnectionString("SqlDataConnection");
        //        using (SqlConnection connection = new SqlConnection(sqlconn))
        //        {
        //            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection))
        //            {
        //                sqlBulkCopy.DestinationTableName = "dbo.Produkte";

        //                sqlBulkCopy.ColumnMappings.Add("Name", "Name");
        //                sqlBulkCopy.ColumnMappings.Add("Description", "Description");
        //                sqlBulkCopy.ColumnMappings.Add("Price", "Price");
        //                sqlBulkCopy.ColumnMappings.Add("ProductImage", "ProductImage");
        //                sqlBulkCopy.ColumnMappings.Add("LocationNr", "LocationNr");
        //                sqlBulkCopy.ColumnMappings.Add("Stadtname", "Stadtname");
        //                sqlBulkCopy.ColumnMappings.Add("Postleitzahl", "Postleitzahl");
        //                sqlBulkCopy.ColumnMappings.Add("Artikelnummer", "Artikelnummer");
        //                sqlBulkCopy.ColumnMappings.Add("NL", "NL");

        //                connection.Open();
        //                sqlBulkCopy.WriteToServer(myDataSet);
        //                connection.Close();
        //            }

        //            connection.Open();

        //        }


        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //        throw;
        //    }

        //}

        public bool ImportDataTableToSqlServer(DataTable myDataSet)
        {
            try
            {
                var sqlconn = _configuration.GetConnectionString("SqlDataConnection");
                using (SqlConnection connection = new SqlConnection(sqlconn))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection))
                    {
                        sqlBulkCopy.DestinationTableName = "dbo.Produkte";

                        sqlBulkCopy.ColumnMappings.Add("Name", "Name");
                        sqlBulkCopy.ColumnMappings.Add("Description", "Description");
                        sqlBulkCopy.ColumnMappings.Add("Price", "Price");
                        sqlBulkCopy.ColumnMappings.Add("ProductImage", "ProductImage");
                        sqlBulkCopy.ColumnMappings.Add("LocationNr", "LocationNr");
                        sqlBulkCopy.ColumnMappings.Add("Stadtname", "Stadtname");
                        sqlBulkCopy.ColumnMappings.Add("Postleitzahl", "Postleitzahl");
                        sqlBulkCopy.ColumnMappings.Add("Artikelnummer", "Artikelnummer");
                        sqlBulkCopy.ColumnMappings.Add("NL", "NL");

                        connection.Open();
                        sqlBulkCopy.WriteToServer(myDataSet);
                        connection.Close();
                    }

                    connection.Open();

                }


                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }

        }

        public IActionResult Postleitzahl()
        {
            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            ISession? session = _httpContextAccessor?.HttpContext?.Session;
            warenkorbImSession = SessionExtensions.Get<Warenkorb>(session, ip);

            if (warenkorbImSession?.WarenkorbItemsListe.Count > 0)
            {
                ViewBag.Area = warenkorbImSession?.Stadtname;
                ViewBag.Zipcode = warenkorbImSession?.Postleitzahl;

                return View("Shop", warenkorbImSession);
            }
            return View();
        }

        public async Task<IActionResult> Shop(int zipcode)
        {
            Product? produkt = null;
            ProduktListe = new List<Product>();

            if (zipcode > 0)
            {
                bool isProduktListeExist = _dbCache.TryGetValue<List<Product>>("ProduktListe", out ProduktListe);

                if (!isProduktListeExist)
                {
                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        SlidingExpiration = TimeSpan.FromHours(23),
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(23),
                    };
                    //leistungsortePostsListe = new List<LeistungsOrtePost>();

                    ProduktListe = await _context.Produkte.ToListAsync();

                    _dbCache.Set<IEnumerable<Product>>("ProduktListe", ProduktListe, memoryCacheEntryOptions);
                }

                produkt = ProduktListe.ToList().FirstOrDefault(x => x.Postleitzahl == zipcode);

                if (produkt == null)
                {
                    return View(new Warenkorb());
                }

                ViewBag.Area = produkt?.Stadtname;
                ViewBag.Zipcode = zipcode;

                string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                ISession? session = _httpContextAccessor?.HttpContext?.Session;

                warenkorbImSession = SessionExtensions.Get<Warenkorb>(session, ip);

                if (warenkorbImSession != null && warenkorbImSession.WarenkorbItemsListe.Count > 0)
                {
                    if (zipcode == warenkorbImSession.Postleitzahl)
                    {
                        TempData["Quantity"] = warenkorbImSession.WarenkorbItemsListe.Count;
                        TempData["TotalAmount"] = warenkorbImSession.WarenkorbItemsListe.Sum(x => x.Total);

                    }
                    else
                    {
                        session?.Remove(ip!);
                        warenkorbImSession = new Warenkorb()
                        {
                            IPAdresse = ip!,
                            Stadtname = produkt?.Stadtname,
                            NL = produkt?.NL,
                            Postleitzahl = zipcode,
                            WarenkorbItemsListe = new List<WarenkorbItems>(),
                            Quantity = 0,
                            TotalAmount = 0
                        };

                        SessionExtensions.Set<Warenkorb>(session, ip!, warenkorbImSession);
                    }

                    return View(warenkorbImSession);
                }
                else
                {
                    warenkorbImSession = new Warenkorb()
                    {
                        IPAdresse = ip!,
                        Stadtname = produkt?.Stadtname,
                        NL = produkt?.NL,
                        Postleitzahl = zipcode,
                        WarenkorbItemsListe = new List<WarenkorbItems>(),
                        Quantity = 0,
                        TotalAmount = 0
                    };

                    SessionExtensions.Set<Warenkorb>(session, ip!, warenkorbImSession);

                    return View(warenkorbImSession);
                }
            }
            else
            {

                return View(nameof(Postleitzahl));

            }

        }

        //public async Task<IActionResult> Shop(int zipcode)
        //{
        //    Product? area = null;
        //    ProduktListe = new List<Product>();

        //    if (zipcode > 0)
        //    {
        //        bool isProduktListeExist = _dbCache.TryGetValue<List<Product>>("ProduktListe", out ProduktListe);

        //        if (!isProduktListeExist)
        //        {
        //            var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
        //            {
        //                SlidingExpiration = TimeSpan.FromHours(23),
        //                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(23),
        //            };
        //            //leistungsortePostsListe = new List<LeistungsOrtePost>();

        //            ProduktListe = await _context.Produkte.ToListAsync();

        //            _dbCache.Set<IEnumerable<Product>>("ProduktListe", ProduktListe, memoryCacheEntryOptions);
        //        }

        //        area = ProduktListe.ToList().FirstOrDefault(x => x.Postleitzahl == zipcode);
        //    }

        //    ViewBag.Area = area?.Stadtname;
        //    ViewBag.Zipcode = zipcode;

        //    string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        //    ISession? session = _httpContextAccessor?.HttpContext?.Session;

        //    warenkorbImSession = SessionExtensions.Get<Warenkorb>(session, ip);

        //    if (warenkorbImSession != null && warenkorbImSession.WarenkorbItemsListe.Count > 0)
        //    {
        //        if (zipcode == warenkorbImSession.Postleitzahl)
        //        {
        //            TempData["Quantity"] = warenkorbImSession.WarenkorbItemsListe.Count;
        //            TempData["TotalAmount"] = warenkorbImSession.WarenkorbItemsListe.Sum(x => x.Total);
        //            return View(warenkorbImSession);
        //        }
        //        else
        //        {
        //            session?.Remove(ip);
        //        }

        //    }

        //    return View(new Warenkorb());


        //    //// Altes Code
        //    //var warenkorbSessionItems = SessionExtensions.Get<List<Warenkorb>>(session, ip);

        //    //if (warenkorbSessionItems != null)
        //    //{
        //    //    TempData["Quantity"] = warenkorbSessionItems.Count;
        //    //    TempData["TotalAmount"] = warenkorbSessionItems.Sum(x => x.TotalAmount);
        //    //    return View(warenkorbSessionItems);
        //    //}

        //    //return View(new List<Warenkorb>());
        //}

        private void AddItemToShoppingCart(string productName, int quantity, string _articlePrice, string _articlePriceAmount, int postleitzahl, string stadtname)
        {
            string picture = null;
            double articlePrice = double.Parse(_articlePrice, CultureInfo.InvariantCulture);
            double articlePriceAmount = double.Parse(_articlePriceAmount, CultureInfo.InvariantCulture);

          
            try
            {
                if (productName != null && quantity > 0 && articlePriceAmount > 0)
                {
                    bool isProduktListeExist = _dbCache.TryGetValue<List<Product>>("ProduktListe", out ProduktListe);
                    string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                    var currentTime = DateTime.Now;
                    // das muß noch behandelt werden Falls der Product nicht vorhanden
                    Product? product = isProduktListeExist == true ? ProduktListe.FirstOrDefault(x => x.Postleitzahl == postleitzahl) : null;

                    ISession? session = _httpContextAccessor?.HttpContext?.Session;

                    warenkorbImSession = SessionExtensions.Get<Warenkorb>(session, ip);

                    if (productName != null)
                    {
                        if (productName == "Karton-Box")
                        {
                            picture = "/assets/images/Produkte_Karton3.png";
                        }
                        else if (productName == "Container-Box-Holz")
                        { 
                            picture = "/assets/images/Produkte_Container.png";
  
                        }
                        else if (productName == "Container-Box-Metall")
                        {
                            picture = "/assets/images/Produkte_Container.png";
                        }
                        else if (productName == "Einzelstücke")
                        {
                            picture = "/assets/images/Produkte_Einstuecke.png";
                        }
                        //else if (productName == "Umzugskarton")
                        //{
                        //    picture = "/assets/images/umzugkarton_600x600.png";
                        //}
                        //else if (productName == "Ladehilfe")
                        //{
                        //    picture = "/assets/images/Ladehilfe_600x600_Tranz.jpg";
                        //}

                    }

                    if (warenkorbImSession == null)
                    {
                        var newList = new List<WarenkorbItems>();

                        newList.Add(new WarenkorbItems()
                        {
                            ProductImage = picture,
                            ProduktName = productName,
                            Quantity = quantity,
                            Total = articlePriceAmount,
                            UnitPrice = articlePrice
                        });

                        warenkorbImSession = new Warenkorb()
                        {
                            IPAdresse = ip,
                            Stadtname = stadtname,
                            NL = product?.NL,
                            Postleitzahl = postleitzahl,
                            WarenkorbItemsListe = newList,
                            Quantity = newList.Count,

                        };



                    }
                    else
                    {
                        var warenkorbItem = warenkorbImSession?.WarenkorbItemsListe?.FirstOrDefault(item => item.ProduktName == productName);

                        if (warenkorbItem == null)
                        {
                            warenkorbImSession?.WarenkorbItemsListe?
                                .Add(new WarenkorbItems()
                                {
                                    ProductImage = picture,
                                    ProduktName = productName,
                                    Quantity = quantity,
                                    Total = articlePriceAmount,
                                    UnitPrice = articlePrice
                                });

                        }
                        else
                        {
                            warenkorbItem.Quantity = quantity;
                            warenkorbItem.Total = articlePriceAmount;
                        }

                    }

                    //*************************** BITTE AUFpassen nicht zwei TempData mit Json Object passen klappt nicht wirft immer einen Fehler **********************************
                    //TempData["Quantity"] = warenkorbImSession.WarenkorbItemsListe.Count;
                    //TempData["TotalAmount"] = warenkorbImSession.WarenkorbItemsListe.Sum(x => x.Total);

                    warenkorbImSession!.TotalAmount = warenkorbImSession.WarenkorbItemsListe.Sum(x => x.Total);
                    warenkorbImSession.Quantity = warenkorbImSession.WarenkorbItemsListe.Count;

                    SessionExtensions.Set<Warenkorb>(session, ip, warenkorbImSession);

                }
            }
            catch (Exception)
            {

                throw;
            }


            //return new JsonResult(new { Quantity = Warenkorbitems.Count, TotalAmount = totalAmount })
            //{
            //    StatusCode = StatusCodes.Status200OK,
            //    ContentType= "application/json",               
            //};

            //var tt = Json( new { Quantity = Warenkorbitems.Count, TotalAmount = totalAmount });

            //var warenkorbImSessionInJson = JsonSerializer.Serialize(warenkorbImSession);         
            //return Json(warenkorbImSessionInJson);
        }

        private void RemoveItemFromShoppingCart(string productName, int quantity, string _articlePrice, string _articlePriceAmount, int postleitzahl, string stadtname)
        {
            WarenkorbItems? warenkorbItem = null;
            Product? product = null;
            double articlePrice = double.Parse(_articlePrice, CultureInfo.InvariantCulture);
            double articlePriceAmount = double.Parse(_articlePriceAmount, CultureInfo.InvariantCulture);

            try
            {
                if (productName != null)
                {
                    bool isProduktListeExist = _dbCache.TryGetValue<List<Product>>("ProduktListe", out ProduktListe);

                    if (!isProduktListeExist)
                    {
                        var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                        {
                            SlidingExpiration = TimeSpan.FromHours(23),
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(23),
                        };
                        //leistungsortePostsListe = new List<LeistungsOrtePost>();

                        var ProduktListe = _context.Produkte.ToList();

                        _dbCache.Set<IEnumerable<Product>>("ProduktListe", ProduktListe, memoryCacheEntryOptions);
                        _dbCache.TryGetValue<List<Product>>("ProduktListe", out ProduktListe);
                    }

                    string? ip = HttpContext.Connection.RemoteIpAddress?.ToString(); // IP finden
                    var currentTime = DateTime.Now;
                    // das muß noch behandelt werden Falls der Product nicht vorhanden
                    product = isProduktListeExist == true ? ProduktListe.FirstOrDefault(x => x.Name == productName) : null;

                    ISession? session = _httpContextAccessor?.HttpContext?.Session;

                    warenkorbImSession = SessionExtensions.Get<Warenkorb>(session, ip);

                    // Erstmal den Warenkorb aus der Liste finden *** 

                    if (warenkorbImSession != null)
                    {
                        if (warenkorbImSession.WarenkorbItemsListe.Count > 0)
                        {
                            warenkorbItem = warenkorbImSession?.WarenkorbItemsListe?.FirstOrDefault(item => item.ProduktName == productName);

                            if (warenkorbItem != null)
                            {
                                if (quantity >= 1)
                                {
                                    warenkorbItem.Quantity = quantity;
                                    warenkorbItem.Total = articlePriceAmount;
                                }
                                else
                                {
                                    warenkorbImSession?.WarenkorbItemsListe.Remove(warenkorbItem);

                                }
                                warenkorbImSession.Quantity = warenkorbImSession.WarenkorbItemsListe.Count;
                                warenkorbImSession.TotalAmount = warenkorbImSession.WarenkorbItemsListe.Sum(x => x.Total);
                            }
                        }
                    }


                    if (warenkorbImSession?.WarenkorbItemsListe.Count > 0)
                    {
                        SessionExtensions.Set<Warenkorb>(session, ip!, warenkorbImSession);
                    }
                    else
                    {
                        session?.Clear();
                    }



                    //*************************** BITTE AUFpassen nicht zwei TempData mit Json Object passen klappt nicht wirft immer einen Fehler **********************************
                    //TempData["Quantity"] = warenkorbImSession.WarenkorbItemsListe.Count;
                    //TempData["TotalAmount"] = Warenkorbitems.Sum(x => x.totalAmount);

                }
            }
            catch (Exception)
            {

                throw;
            }


            //double? _totalAmount = warenkorbImSession?.WarenkorbItemsListe.Count > 0 ? warenkorbImSession?.WarenkorbItemsListe.Sum(x => x.Total) : 0;
            //int? _quantity = warenkorbImSession?.WarenkorbItemsListe.Count > 0 ? warenkorbImSession?.WarenkorbItemsListe.Count : 0;

            //return new JsonResult(new { Quantity = Warenkorbitems.Count, TotalAmount = totalAmount })
            //{
            //    StatusCode = StatusCodes.Status200OK,
            //    ContentType= "application/json",               
            //};

            //var tt = Json( new { Quantity = Warenkorbitems.Count, TotalAmount = totalAmount });
            //var warenkorbImSessionInJson = JsonSerializer.Serialize(warenkorbImSession);
            //return Json(warenkorbImSessionInJson);
            //return Json(new { Quantity = _quantity, TotalAmount = _totalAmount });
        }

        //#region alte Methoden

        //public JsonResult AddItemToShoppingCart(string productName, int quantity, double articlePrice, double articlePriceAmount)
        //{
        //    Warenkorb? warenkorbItem = null;
        //    List<Warenkorb>? warenkorbSessionItems = null;
        //    string picture = null;

        //    if (productName != null)
        //    {
        //        if (productName == "Karton-Box")
        //        {
        //            picture = "/assets/images/container-Box3.png";
        //        }
        //        else if (productName == "Container-Box")
        //        {
        //            picture = "/assets/images/container-Box1_Tranz.png";
        //        }
        //        else if (productName == "Einzelstücke")
        //        {
        //            picture = "/assets/images/container-Box2.png";
        //        }
        //        else if (productName == "Umzugskarton")
        //        {
        //            picture = "/assets/images/umzugkarton_600x600.png";
        //        }
        //        else if (productName == "Ladehilfe")
        //        {
        //            picture = "/assets/images/Ladehilfe_600x600_Tranz.jpg";
        //        }

        //    }

        //    try
        //    {
        //        if (productName != null && quantity > 0 && articlePriceAmount > 0)
        //        {
        //            bool isProduktListeExist = _dbCache.TryGetValue<List<Product>>("ProduktListe", out ProduktListe);
        //            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        //            var currentTime = DateTime.Now;
        //            // das muß noch behandelt werden Falls der Product nicht vorhanden
        //            Product? product = isProduktListeExist == true ? ProduktListe.FirstOrDefault(x => x.Name == productName) : null;

        //            ISession? session = _httpContextAccessor?.HttpContext?.Session;

        //            warenkorbSessionItems = SessionExtensions.Get<List<Warenkorb>>(session, ip);

        //            // Erstmal den Warenkorb aus der Liste finden *** 

        //            //if (warenkorbSessionItems != null)
        //            //{
        //            //    warenkorbItem = warenkorbSessionItems?.FirstOrDefault(item => item.ProduktName == productName);
        //            //}

        //            if (warenkorbSessionItems == null)
        //            {
        //                warenkorbSessionItems = new List<Warenkorb>();

        //                warenkorbSessionItems?.Add(new Warenkorb()
        //                {
        //                    ProduktName = productName,
        //                    IPAdresse = ip,
        //                    Stadtname = product?.Stadtname,
        //                    NL = product?.NL,
        //                    Postleitzahl = product?.Postleitzahl,
        //                    Quantity = quantity,
        //                    UnitPrice = articlePrice,
        //                    TotalAmount = articlePriceAmount,
        //                    Total = quantity * articlePrice,
        //                    ProductImage = picture
        //                });
        //            }
        //            else
        //            {
        //                warenkorbItem = warenkorbSessionItems?.FirstOrDefault(item => item.ProduktName == productName);

        //                if (warenkorbItem == null)
        //                {
        //                    warenkorbSessionItems?.Add(new Warenkorb()
        //                    {
        //                        ProduktName = productName,
        //                        IPAdresse = ip,
        //                        Stadtname = product?.Stadtname,
        //                        NL = product?.NL,
        //                        Postleitzahl = product?.Postleitzahl,
        //                        Quantity = quantity,
        //                        UnitPrice = articlePrice,
        //                        TotalAmount = articlePriceAmount,
        //                        Total = quantity * articlePrice,
        //                        ProductImage = picture
        //                    });
        //                }
        //                else
        //                {
        //                    warenkorbItem.Quantity = quantity;
        //                    warenkorbItem.TotalAmount = articlePriceAmount;
        //                    warenkorbItem.Total = quantity * articlePrice;
        //                }

        //            }

        //            SessionExtensions.Set<List<Warenkorb>>(session, ip, warenkorbSessionItems);

        //            //*************************** BITTE AUFpassen nicht zwei TempData mit Json Object passen klappt nicht wirft immer einen Fehler **********************************
        //            TempData["Quantity"] = warenkorbSessionItems.Count;
        //            //TempData["TotalAmount"] = Warenkorbitems.Sum(x => x.totalAmount);

        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }


        //    double _totalAmount = warenkorbSessionItems.Sum(x => x.TotalAmount);
        //    int _quantity = warenkorbSessionItems.Count;

        //    //return new JsonResult(new { Quantity = Warenkorbitems.Count, TotalAmount = totalAmount })
        //    //{
        //    //    StatusCode = StatusCodes.Status200OK,
        //    //    ContentType= "application/json",               
        //    //};

        //    //var tt = Json( new { Quantity = Warenkorbitems.Count, TotalAmount = totalAmount });

        //    return Json(new { Quantity = _quantity, TotalAmount = _totalAmount });
        //}

        //public JsonResult RemoveItemFromShoppingCart(string productName, int quantity, double articlePrice, double articlePriceAmount)
        //{
        //    Warenkorb? warenkorbItem = null;
        //    List<Warenkorb>? warenkorbSessionItems = new List<Warenkorb>();

        //    try
        //    {
        //        if (productName != null)
        //        {
        //            bool isProduktListeExist = _dbCache.TryGetValue<List<Product>>("ProduktListe", out ProduktListe);
        //            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString(); // IP finden
        //            var currentTime = DateTime.Now;
        //            // das muß noch behandelt werden Falls der Product nicht vorhanden
        //            Product? product = isProduktListeExist == true ? ProduktListe.FirstOrDefault(x => x.Name == productName) : null;

        //            ISession? session = _httpContextAccessor?.HttpContext?.Session;

        //            warenkorbSessionItems = SessionExtensions.Get<List<Warenkorb>>(session, ip);

        //            // Erstmal den Warenkorb aus der Liste finden *** 

        //            //if (warenkorbSessionItems != null)
        //            //{
        //            //    warenkorbItem = warenkorbSessionItems?.FirstOrDefault(item => item.ProduktName == productName);
        //            //}

        //            if (warenkorbSessionItems?.Count > 0)
        //            {
        //                warenkorbItem = warenkorbSessionItems?.FirstOrDefault(item => item.ProduktName == productName);

        //                if (warenkorbItem != null)
        //                {
        //                    if (warenkorbItem.Quantity > 1)
        //                    {
        //                        warenkorbItem.Quantity = quantity;
        //                        warenkorbItem.TotalAmount = articlePriceAmount;
        //                        warenkorbItem.Total = quantity * articlePrice;
        //                        if (warenkorbItem.ProduktName == "Karton-Box" && warenkorbItem.Quantity == 0)
        //                        {
        //                            warenkorbSessionItems?.Remove(warenkorbItem);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //warenkorbItem.Quantity = 0;
        //                        //warenkorbItem.totalAmount = 0;
        //                        //warenkorbItem.Total = 0;
        //                        warenkorbSessionItems?.Remove(warenkorbItem);

        //                    }


        //                }


        //            }

        //            SessionExtensions.Set<List<Warenkorb>>(session, ip, warenkorbSessionItems);

        //            //*************************** BITTE AUFpassen nicht zwei TempData mit Json Object passen klappt nicht wirft immer einen Fehler **********************************
        //            TempData["Quantity"] = warenkorbSessionItems.Count;
        //            //TempData["TotalAmount"] = Warenkorbitems.Sum(x => x.totalAmount);

        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }


        //    double _totalAmount = warenkorbSessionItems.Count > 0 ? warenkorbSessionItems.Sum(x => x.TotalAmount) : 0;
        //    int _quantity = warenkorbSessionItems.Count > 0 ? warenkorbSessionItems.Count : 0;

        //    //return new JsonResult(new { Quantity = Warenkorbitems.Count, TotalAmount = totalAmount })
        //    //{
        //    //    StatusCode = StatusCodes.Status200OK,
        //    //    ContentType= "application/json",               
        //    //};

        //    //var tt = Json( new { Quantity = Warenkorbitems.Count, TotalAmount = totalAmount });

        //    return Json(new { Quantity = _quantity, TotalAmount = _totalAmount });
        //}

        // #endregion

        public JsonResult GetShoppingCart()
        {

            //TempData["WarenkorbImSession"] = warenkorbImSession;

            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            ISession? session = _httpContextAccessor?.HttpContext?.Session;
            warenkorbImSession = SessionExtensions.Get<Warenkorb>(session, ip);

            var warenkorbImSessionInJson = warenkorbImSession?.WarenkorbItemsListe.Count() > 0 ? JsonSerializer.Serialize(warenkorbImSession) : null;

            return Json(warenkorbImSessionInJson);
        }

        public JsonResult _ShoppingCart(string productName, int quantity, string _articlePrice, string _articlePriceAmount, int postleitzahl, string stadtname, int flag)
        {

            if (flag == 1)
            {
                AddItemToShoppingCart(productName, quantity, _articlePrice, _articlePriceAmount, postleitzahl, stadtname);
            }
            else
            {
                RemoveItemFromShoppingCart(productName, quantity, _articlePrice, _articlePriceAmount, postleitzahl, stadtname);
            }

            //TempData["WarenkorbImSession"] = warenkorbImSession;

            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            ISession? session = _httpContextAccessor?.HttpContext?.Session;
            warenkorbImSession = SessionExtensions.Get<Warenkorb>(session, ip);

            var warenkorbImSessionInJson = JsonSerializer.Serialize(warenkorbImSession);

            return Json(warenkorbImSessionInJson);
        }

        public IActionResult Warenkorb()
        {

            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            ISession? session = _httpContextAccessor?.HttpContext?.Session;
            warenkorbImSession = SessionExtensions.Get<Warenkorb>(session, ip);

            if (warenkorbImSession != null)
            {
                //var tt = warenkorbSessionItems?.GroupBy(x => x.Postleitzahl); // Grupieren nach PLZ falls der Kunde für verschieden PLZ bestellt hat

                TempData["Quantity"] = warenkorbImSession.WarenkorbItemsListe.Count;
                TempData["TotalAmount"] = warenkorbImSession.WarenkorbItemsListe.Sum(x => x.Total);

                //TempData["WarenkorbImSession"] = warenkorbImSession;
                return View(warenkorbImSession);
            }
            else
            {
                return View(new Warenkorb());
            }
        }

        public IActionResult shippingPayment()
        {
            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            ISession? session = _httpContextAccessor?.HttpContext?.Session;
            Warenkorb? warenkorbSessionItems = SessionExtensions.Get<Warenkorb>(session, ip);

            if (warenkorbSessionItems != null)
            {
                var tt = warenkorbSessionItems?.WarenkorbItemsListe.GroupBy(x => x.ProduktName); // Grupieren nach PLZ falls der Kunde für verschieden PLZ bestellt hat

                TempData["Quantity"] = warenkorbSessionItems?.WarenkorbItemsListe.Count;
                TempData["TotalAmount"] = warenkorbSessionItems?.WarenkorbItemsListe.Sum(x => x.Total);
                return View(warenkorbSessionItems);
            }
            else
            {
                return View(new Warenkorb());
            }
        }

        public IActionResult Checkout()
        {

            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            ISession? session = _httpContextAccessor?.HttpContext?.Session;
            warenkorbImSession = SessionExtensions.Get<Warenkorb>(session, ip);
            Checkout checkout = new Checkout();
         
            if (warenkorbImSession != null)
            {

                ViewBag.WarenkorbItemsCount = warenkorbImSession.WarenkorbItemsListe.Count;
                ViewBag.TotalAmount = warenkorbImSession.WarenkorbItemsListe.Sum(x => x.Total);
                return View(checkout); // checkout ist Not NULL
            }
            else
            {
                ViewBag.WarenkorbItemsCount = 0;
                ViewBag.TotalAmount = 0;
                return View(checkout); // checkout IS NULL
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bestelluebersicht([Bind("KundenType,Anrede,Vorname,Nachname,GeschäftsbedingungenAkzeptiert,Telefon,Email,Password,ConfirmPassword,RechnungAdresse,GastKunde,HasTowAdresse,LieferAdresse,FriendlycaptchaToken")] Checkout newKunde)
        {
            LieferAdresse Lieferadresse = null;
            RechnungAdresse rechnungadresse = null;

            FriendlyCaptchaResponse captchaResponse = new FriendlyCaptchaResponse();

            #region Friendlycaptcha Abschnit Code

            var api_Key = _configuration["Friendlycaptcha:APIKey"];
            var sitekey = _configuration["Friendlycaptcha:SiteKey"];

            //// neue Code HttpClient
            //try
            //{
            //    //var httpRequestMessage = new HttpRequestMessage()
            //    //{
            //    //    Method = HttpMethod.Post,
            //    //    RequestUri = new Uri("https://global.frcapi.com/api/v2/captcha/siteverify"),
            //    //};

            //    var httpClient = _httpClientFactory.CreateClient();

            //    //httpClient.Timeout = TimeSpan.FromSeconds(40);
            //    //httpClient.DefaultRequestVersion = HttpVersion.Version20;
            //    //httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;

            //    httpClient.DefaultRequestHeaders.Add("X-API-Key", api_Key);

            //    var param = new Dictionary<string, string> { { "response", newKunde?.FriendlycaptchaToken! }, { "sitekey", sitekey } };
            //    var parameters = new FormUrlEncodedContent(param);

            //    var response = await httpClient.PostAsync("https://global.frcapi.com/api/v2/captcha/siteverify", parameters);
            //    var responseAsString = await response.Content.ReadAsStringAsync();
            //    captchaResponse = JsonSerializer.Deserialize<FriendlyCaptchaResponse>(responseAsString);
            //}
            //catch (HttpRequestException e)
            //{
            //    //ViewBag.ErrorMessage = e.Message;

            //    return View("Error", e.Message);
            //}

            #endregion


            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            ISession? session = _httpContextAccessor?.HttpContext?.Session;
            warenkorbImSession = SessionExtensions.Get<Warenkorb>(session, ip);

            ViewBag.WarenkorbItemsCount = warenkorbImSession?.WarenkorbItemsListe.Count;
            ViewBag.TotalAmount = warenkorbImSession?.WarenkorbItemsListe.Sum(x => x.Total);

            string returnUrl = Url.Content("~/");


            if (ModelState.IsValid /*&& captchaResponse!.success*/)
            {
                GastKunde gastKunde = new GastKunde();

                if (newKunde!.HasTowAdresse)
                {
                    if (newKunde?.LieferAdresse?.Straße != null || newKunde?.LieferAdresse?.Ort != null || newKunde?.LieferAdresse?.PLZ != null)
                    {
                        gastKunde.LiefStraße = newKunde?.LieferAdresse?.Straße!;
                        gastKunde.LiefOrt = newKunde?.RechnungAdresse?.Ort!;
                        gastKunde.LiefPLZ = newKunde?.RechnungAdresse?.PLZ!;

                        //Lieferadresse = new LieferAdresse()
                        //{
                        //    AdresseTyp = "LieferAdresse",
                        //    Straße = ,
                        //    Ort = newKunde.LieferAdresse.Ort,
                        //    PLZ = newKunde.LieferAdresse.PLZ,
                        //    ApplicationUserID = null,
                        //};
                    }
                    else
                    {
                        ModelState.AddModelError("", "Rechnungs- und Lieferadresse weichen voneinander ab, deshalb ist die Lieferadresse auch ein Pflichtfeld");
                        return View("Checkout");
                    }

                }

                if (newKunde?.RechnungAdresse?.Straße != null)
                {
                    gastKunde.RechStraße = newKunde?.RechnungAdresse?.Straße!;
                    gastKunde.RechOrt = newKunde?.RechnungAdresse?.Ort!;
                    gastKunde.RechPLZ = newKunde?.RechnungAdresse?.PLZ!;

                    //rechnungadresse = new RechnungAdresse()
                    //{
                    //    AdresseTyp = "RechnungAdresse",
                    //    Straße = newKunde.RechnungAdresse.Straße,
                    //    Ort = newKunde.RechnungAdresse.Ort,
                    //    PLZ = newKunde.RechnungAdresse.PLZ,
                    //    ApplicationUserID = null,
                    //};

                }
                gastKunde.Anrede = newKunde?.Anrede;
                gastKunde.KundenType = newKunde?.KundenType;
                gastKunde.Vorname = newKunde?.Vorname;
                gastKunde.Nachname = newKunde?.Nachname;
                gastKunde.GeschäftsbedingungenAkzeptiert = newKunde!.GeschäftsbedingungenAkzeptiert;
                gastKunde.Telefon = newKunde.Telefon;
                gastKunde.Email = newKunde.Email;
                gastKunde.HasTowAdresse = newKunde.HasTowAdresse;
                gastKunde.KundenType = "Gast";
                warenkorbImSession!.ApplicationUser = null;
                warenkorbImSession!.LieferDatum = newKunde.LieferDatum;

                warenkorbImSession!.GastKunde = gastKunde;

                SessionExtensions.Set<Warenkorb>(session, ip, warenkorbImSession);
                return View(warenkorbImSession);

                #region Region Altes Code wo GastKunde mit User zum Anmelden einfach das Auskomentieren wieder frei schlaten und das Code Oben ersetzen

                //if (newKunde.GastKunde)
                //{
                //    GastKunde gastKunde = new GastKunde();

                //    if (newKunde.HasTowAdresse)
                //    {
                //        if (newKunde?.LieferAdresse?.Straße != null || newKunde?.LieferAdresse?.Ort != null || newKunde?.LieferAdresse?.PLZ != null)
                //        {
                //            gastKunde.LiefStraße = newKunde?.LieferAdresse?.Straße!;
                //            gastKunde.LiefOrt = newKunde?.RechnungAdresse?.Ort!;
                //            gastKunde.LiefPLZ = newKunde?.RechnungAdresse?.PLZ!;

                //            //Lieferadresse = new LieferAdresse()
                //            //{
                //            //    AdresseTyp = "LieferAdresse",
                //            //    Straße = ,
                //            //    Ort = newKunde.LieferAdresse.Ort,
                //            //    PLZ = newKunde.LieferAdresse.PLZ,
                //            //    ApplicationUserID = null,
                //            //};
                //        }
                //        else
                //        {
                //            ModelState.AddModelError("", "Rechnungs- und Lieferadresse weichen voneinander ab, deshalb ist die Lieferadresse auch ein Pflichtfeld");
                //            return View("Checkout");
                //        }

                //    }

                //    if (newKunde?.RechnungAdresse?.Straße != null)
                //    {
                //        gastKunde.RechStraße = newKunde?.RechnungAdresse?.Straße!;
                //        gastKunde.RechOrt = newKunde?.RechnungAdresse?.Ort!;
                //        gastKunde.RechPLZ = newKunde?.RechnungAdresse?.PLZ!;

                //        //rechnungadresse = new RechnungAdresse()
                //        //{
                //        //    AdresseTyp = "RechnungAdresse",
                //        //    Straße = newKunde.RechnungAdresse.Straße,
                //        //    Ort = newKunde.RechnungAdresse.Ort,
                //        //    PLZ = newKunde.RechnungAdresse.PLZ,
                //        //    ApplicationUserID = null,
                //        //};

                //    }
                //    gastKunde.Anrede = newKunde?.Anrede;
                //    gastKunde.KundenType = newKunde?.KundenType;
                //    gastKunde.Vorname = newKunde?.Vorname;
                //    gastKunde.Nachname = newKunde?.Nachname;
                //    gastKunde.GeschäftsbedingungenAkzeptiert = newKunde!.GeschäftsbedingungenAkzeptiert;
                //    gastKunde.Telefon = newKunde.Telefon;
                //    gastKunde.Email = newKunde.Email;
                //    gastKunde.HasTowAdresse = newKunde.HasTowAdresse;
                //    warenkorbImSession!.ApplicationUser = null;
                //    warenkorbImSession!.LieferDatum = newKunde.LieferDatum;

                //    warenkorbImSession!.GastKunde = gastKunde;

                //    SessionExtensions.Set<Warenkorb>(session, ip, warenkorbImSession);
                //}
                //else
                //{
                //    ApplicationUser? getKunde = await _userManager.FindByEmailAsync(newKunde.Email);

                //    if (getKunde != null)
                //    {
                //        ModelState.AddModelError("", "Sie sind schon als Kund bei uns im System registriert. melden Sie sich bitte mit Ihrem Kundendaten an.Danke!");
                //        return View("Checkout");
                //    }
                //    var user = new ApplicationUser();

                //    if (newKunde.HasTowAdresse)
                //    {
                //        if (newKunde?.LieferAdresse?.Straße != null || newKunde?.LieferAdresse?.Ort != null || newKunde?.LieferAdresse?.PLZ != null)
                //        {
                //            Lieferadresse = new LieferAdresse()
                //            {
                //                AdresseTyp = "LieferAdresse",
                //                Straße = newKunde.LieferAdresse.Straße,
                //                Ort = newKunde.LieferAdresse.Ort,
                //                PLZ = newKunde.LieferAdresse.PLZ,
                //                ApplicationUserID = user.Id,
                //                //GastKunde = null
                //            };
                //        }
                //        else
                //        {
                //            ModelState.AddModelError("", "Rechnungs- und Lieferadresse weichen voneinander ab, deshalb ist die Lieferadresse auch ein Pflichtfeld");
                //            return View("Checkout");
                //        }

                //    }

                //    if (newKunde?.RechnungAdresse?.Straße != null)
                //    {
                //        rechnungadresse = new RechnungAdresse()
                //        {
                //            AdresseTyp = "RechnungAdresse",
                //            Straße = newKunde.RechnungAdresse.Straße,
                //            Ort = newKunde.RechnungAdresse.Ort,
                //            PLZ = newKunde.RechnungAdresse.PLZ,
                //            ApplicationUserID = user.Id,
                //            //GastKunde = null
                //        };

                //    }

                //    user.Anrede = newKunde?.Anrede;
                //    user.KundenType = newKunde?.KundenType;
                //    user.Vorname = newKunde?.Vorname;
                //    user.Nachname = newKunde?.Nachname;
                //    user.GeschäftsbedingungenAkzeptiert = newKunde!.GeschäftsbedingungenAkzeptiert;
                //    user.PhoneNumber = newKunde.Telefon;
                //    user.Email = newKunde.Email;
                //    user.UserName = newKunde.Email;
                //    user.hasTowAdresse = newKunde.HasTowAdresse;
                //    user.RechnungAdresse = rechnungadresse;
                //    user.LieferAdresse = Lieferadresse;
                //    user.Password = newKunde?.Password!;
                //    warenkorbImSession!.ApplicationUser = user;
                //    warenkorbImSession!.GastKunde = null;
                //    warenkorbImSession!.LieferDatum = newKunde!.LieferDatum;
                //    SessionExtensions.Set<Warenkorb>(session, ip, warenkorbImSession);
                //}
                #endregion
            }
            else
            {
                return View("checkout");
            }


        }

        //container
        public async Task<IActionResult> Container(int zipcode)
        {
            Product? produkt = null;
            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            ISession? session = _httpContextAccessor?.HttpContext?.Session;
            warenkorbImSession = SessionExtensions.Get<Warenkorb>(session, ip);
            
            if (zipcode == 0 && warenkorbImSession != null && warenkorbImSession?.WarenkorbItemsListe.Count > 0)
            {
                ViewBag.Area = warenkorbImSession.Stadtname;
                ViewBag.Zipcode = warenkorbImSession.Postleitzahl;
                TempData["Quantity"] = warenkorbImSession.WarenkorbItemsListe.Count;
                TempData["TotalAmount"] = warenkorbImSession.WarenkorbItemsListe.Sum(x => x.Total);
                return View(warenkorbImSession);
                
            }
            else if (zipcode == 0 && warenkorbImSession == null || zipcode == 0 && warenkorbImSession?.WarenkorbItemsListe.Count == 0 )
            {
                return View(nameof(Postleitzahl));
            }
            else
            {
                bool isProduktListeExist = _dbCache.TryGetValue<List<Product>>("ProduktListe", out ProduktListe);

                if (!isProduktListeExist)
                {
                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        SlidingExpiration = TimeSpan.FromHours(23),
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(23),
                    };
                    //leistungsortePostsListe = new List<LeistungsOrtePost>();

                    ProduktListe = await _context.Produkte.ToListAsync();

                    _dbCache.Set<IEnumerable<Product>>("ProduktListe", ProduktListe, memoryCacheEntryOptions);
                }

                produkt = ProduktListe.ToList().FirstOrDefault(x => x.Postleitzahl == zipcode);

                if (warenkorbImSession != null && warenkorbImSession?.WarenkorbItemsListe.Count > 0)
                {
                    if (zipcode == warenkorbImSession.Postleitzahl)
                    {
                        ViewBag.Area = warenkorbImSession.Stadtname;
                        ViewBag.Zipcode = warenkorbImSession.Postleitzahl;
                        TempData["Quantity"] = warenkorbImSession.WarenkorbItemsListe.Count;
                        TempData["TotalAmount"] = warenkorbImSession.WarenkorbItemsListe.Sum(x => x.Total);
                        return View(warenkorbImSession);
                    }
                    else
                    {
                        session?.Remove(ip!);                       
                    }
                }

                warenkorbImSession = new Warenkorb()
                {
                    IPAdresse = ip,
                    Stadtname = produkt?.Stadtname,
                    NL = produkt?.NL,
                    Postleitzahl = zipcode,
                    WarenkorbItemsListe = new List<WarenkorbItems>(),
                    Quantity = 0,
                    TotalAmount = 0
                };

                SessionExtensions.Set<Warenkorb>(session, ip!, warenkorbImSession);
                ViewBag.Area = warenkorbImSession.Stadtname;
                ViewBag.Zipcode = warenkorbImSession.Postleitzahl;

                TempData["Quantity"] = warenkorbImSession.Quantity;
                TempData["TotalAmount"] = warenkorbImSession.TotalAmount;
                return View(warenkorbImSession);
            }

        }
        //Einzelstueck
        public async Task<IActionResult> Einzelstueck(int zipcode)
        {
            Product? produkt = null;
            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            ISession? session = _httpContextAccessor?.HttpContext?.Session;
            warenkorbImSession = SessionExtensions.Get<Warenkorb>(session, ip);

            if (zipcode == 0 && warenkorbImSession != null && warenkorbImSession?.WarenkorbItemsListe.Count > 0)
            {
                ViewBag.Area = warenkorbImSession.Stadtname;
                ViewBag.Zipcode = warenkorbImSession.Postleitzahl;
                TempData["Quantity"] = warenkorbImSession.WarenkorbItemsListe.Count;
                TempData["TotalAmount"] = warenkorbImSession.WarenkorbItemsListe.Sum(x => x.Total);
                return View(warenkorbImSession);

            }
            else if (zipcode == 0 && warenkorbImSession == null || zipcode == 0 && warenkorbImSession?.WarenkorbItemsListe.Count == 0)
            {
                return View(nameof(Postleitzahl));
            }
            else
            {
                bool isProduktListeExist = _dbCache.TryGetValue<List<Product>>("ProduktListe", out ProduktListe);

                if (!isProduktListeExist)
                {
                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        SlidingExpiration = TimeSpan.FromHours(23),
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(23),
                    };
                    //leistungsortePostsListe = new List<LeistungsOrtePost>();

                    ProduktListe = await _context.Produkte.ToListAsync();

                    _dbCache.Set<IEnumerable<Product>>("ProduktListe", ProduktListe, memoryCacheEntryOptions);
                }

                produkt = ProduktListe.ToList().FirstOrDefault(x => x.Postleitzahl == zipcode);

                if (warenkorbImSession != null && warenkorbImSession?.WarenkorbItemsListe.Count > 0)
                {
                    if (zipcode == warenkorbImSession.Postleitzahl)
                    {
                        ViewBag.Area = warenkorbImSession.Stadtname;
                        ViewBag.Zipcode = warenkorbImSession.Postleitzahl;
                        TempData["Quantity"] = warenkorbImSession.WarenkorbItemsListe.Count;
                        TempData["TotalAmount"] = warenkorbImSession.WarenkorbItemsListe.Sum(x => x.Total);
                        return View(warenkorbImSession);
                    }
                    else
                    {
                        session?.Remove(ip!);
                    }
                }

                warenkorbImSession = new Warenkorb()
                {
                    IPAdresse = ip,
                    Stadtname = produkt?.Stadtname,
                    NL = produkt?.NL,
                    Postleitzahl = zipcode,
                    WarenkorbItemsListe = new List<WarenkorbItems>(),
                    Quantity = 0,
                    TotalAmount = 0
                };

                SessionExtensions.Set<Warenkorb>(session, ip!, warenkorbImSession);
                ViewBag.Area = warenkorbImSession.Stadtname;
                ViewBag.Zipcode = warenkorbImSession.Postleitzahl;

                TempData["Quantity"] = warenkorbImSession.Quantity;
                TempData["TotalAmount"] = warenkorbImSession.TotalAmount;
                return View(warenkorbImSession);
            }
        }

        ////Karton
        //[HttpGet]
        //[Route("Shop/Karton/{Zipcode?}/{Stadtname?}")]
        public async Task<IActionResult> Karton(int zipcode,string stadtname)
        {
            Product? produkt = null;
            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            ISession? session = _httpContextAccessor?.HttpContext?.Session;
            warenkorbImSession = SessionExtensions.Get<Warenkorb>(session, ip);

            if (zipcode == 0 && warenkorbImSession != null && warenkorbImSession?.WarenkorbItemsListe.Count > 0)
            {
                ViewBag.Area = warenkorbImSession.Stadtname;
                ViewBag.Zipcode = warenkorbImSession.Postleitzahl;
                TempData["Quantity"] = warenkorbImSession.WarenkorbItemsListe.Count;
                TempData["TotalAmount"] = warenkorbImSession.WarenkorbItemsListe.Sum(x => x.Total);
                return View(warenkorbImSession);

            }
            else if (zipcode == 0 && warenkorbImSession == null || zipcode == 0 && warenkorbImSession?.WarenkorbItemsListe.Count == 0)
            {
                return View(nameof(Postleitzahl));
            }
            else
            {
                bool isProduktListeExist = _dbCache.TryGetValue<List<Product>>("ProduktListe", out ProduktListe);

                if (!isProduktListeExist)
                {
                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        SlidingExpiration = TimeSpan.FromHours(23),
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(23),
                    };
                    //leistungsortePostsListe = new List<LeistungsOrtePost>();

                    ProduktListe = await _context.Produkte.ToListAsync();

                    _dbCache.Set<IEnumerable<Product>>("ProduktListe", ProduktListe, memoryCacheEntryOptions);
                }

                produkt = ProduktListe.ToList().FirstOrDefault(x => x.Postleitzahl == zipcode);

                if (warenkorbImSession != null && warenkorbImSession?.WarenkorbItemsListe.Count > 0)
                {
                    if (zipcode == warenkorbImSession.Postleitzahl)
                    {
                        ViewBag.Area = warenkorbImSession.Stadtname;
                        ViewBag.Zipcode = warenkorbImSession.Postleitzahl;
                        TempData["Quantity"] = warenkorbImSession.WarenkorbItemsListe.Count;
                        TempData["TotalAmount"] = warenkorbImSession.WarenkorbItemsListe.Sum(x => x.Total);
                        return View(warenkorbImSession);
                    }
                    else
                    {
                        session?.Remove(ip!);
                    }
                }

                warenkorbImSession = new Warenkorb()
                {
                    IPAdresse = ip,
                    Stadtname = produkt?.Stadtname,
                    NL = produkt?.NL,
                    Postleitzahl = zipcode,
                    WarenkorbItemsListe = new List<WarenkorbItems>(),
                    Quantity = 0,
                    TotalAmount = 0
                };

                SessionExtensions.Set<Warenkorb>(session, ip!, warenkorbImSession);
                ViewBag.Area = warenkorbImSession.Stadtname;
                ViewBag.Zipcode = warenkorbImSession.Postleitzahl;

                TempData["Quantity"] = warenkorbImSession.Quantity;
                TempData["TotalAmount"] = warenkorbImSession.TotalAmount;
                return View(warenkorbImSession);
            }
        }

        //Umzugskarton
        public IActionResult Umzugskarton()
        {
            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            ISession? session = _httpContextAccessor?.HttpContext?.Session;

            warenkorbImSession = SessionExtensions.Get<Warenkorb>(session, ip);

            if (warenkorbImSession?.Postleitzahl != null )
            {

                TempData["Quantity"] = warenkorbImSession.WarenkorbItemsListe.Count;
                TempData["TotalAmount"] = warenkorbImSession.WarenkorbItemsListe.Sum(x => x.Total);
                return View(warenkorbImSession);
            }

            return View(new Warenkorb());
        }

        //Ladehilfe
        public IActionResult Ladehilfe()
        {
            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            ISession? session = _httpContextAccessor?.HttpContext?.Session;

            warenkorbImSession = SessionExtensions.Get<Warenkorb>(session, ip);

            if (warenkorbImSession?.Postleitzahl != null)
            {

                TempData["Quantity"] = warenkorbImSession.WarenkorbItemsListe.Count;
                TempData["TotalAmount"] = warenkorbImSession.WarenkorbItemsListe.Sum(x => x.Total);
                return View(warenkorbImSession);
            }

            return View(new Warenkorb());
        }


        ////container
        //public IActionResult Container()
        //{
        //    return View();
        //}

        [HttpPost]
        public async Task<IActionResult> Danke(string _ip)
        {
            // ToDo Liste
            // 1. Create New Bestellung
            // 2. Create Fortlaufende Nummer
            // 3. Gast Zugang
            // 4. Email versand
            // 5. neue Seite für die bestätigung der erfolgreichen Bestellvorgang

            //IdentityResult result = new IdentityResult();

            Bestellung bestellung = new Bestellung();
            Warenkorb _warenkorb = new Warenkorb();

            if (ModelState.IsValid)
            {
                try
                {
                    //string? ip = HttpContext.Connection.RemoteIpAddress?.ToString(); // 
                    ISession? session = _httpContextAccessor?.HttpContext?.Session;
                    warenkorbImSession = SessionExtensions.Get<Warenkorb>(session, _ip);

                    ViewBag.WarenkorbItemsCount = warenkorbImSession?.WarenkorbItemsListe.Count;
                    ViewBag.TotalAmount = warenkorbImSession?.WarenkorbItemsListe.Sum(x => x.Total);

                    string returnUrl = Url.Content("~/");

                    if (warenkorbImSession?.WarenkorbItemsListe?.Count > 0)
                    {
                        string bestellungsnummer = CreateBestellnummer.CreateBestellNumber();

                        // wenn neue User mit Passord erstellet werden soll
                        if (/*warenkorbImSession?.ApplicationUser == null && */warenkorbImSession?.GastKunde != null)
                        {
                            GastKunde gastKunde = warenkorbImSession.GastKunde;
                            DateTime lief = DateTime.Now.AddMonths(4);//// das LieferDatum ist irrelevant, das Lieferdatum habe ich nach 4 Monaten gesetzt, nur die E_Commers haben sich fürs Text (id="tsCheckoutOrderEstDeliveryDate") in Danke View entschieden 

                            bestellung = new Bestellung()
                            {
                                BestellStatus = "in Bearbeitung",
                                ApplicationUserID = null,
                                //GastKunde = warenkorbImSession?.GastKunde,
                                Bestellnummer = warenkorbImSession?.NL == "Köln" ? "K" + bestellungsnummer : "M" + bestellungsnummer,
                                FortlaufendeNummer = bestellungsnummer,
                                hasTowAdresse = warenkorbImSession!.GastKunde!.HasTowAdresse,
                                IPAdresse = warenkorbImSession?.IPAdresse!,
                                GesamtAnzahlArtikeln = warenkorbImSession!.Quantity,
                                Stadtname = warenkorbImSession.Stadtname,
                                GesamtBestellungsBetrag = warenkorbImSession.TotalAmount,
                                LieferDatum = lief,
                                Niederlassung = warenkorbImSession.NL,
                                Postleitzahl = warenkorbImSession.Postleitzahl,
                                KundenType = "Gast"
                            };

                            // Bestellpositionen in die Bestellung hinzufügen
                            foreach (var item in warenkorbImSession?.WarenkorbItemsListe!)
                            {
                                bestellung.Positionen.Add(new Bestellpositionen()
                                {
                                    Anzah = item.Quantity,
                                    ArtikelName = item.ProduktName,
                                    ProductImage = item.ProductImage,
                                    Stückpreis = item.UnitPrice,
                                    Summe = item.Total,
                                });
                            }

                            //// **** Zum Testen Ab Hier Auskommentieren

                            using (var transaction = _context.Database.BeginTransaction())
                            {

                                ////bestellung.GastKunde = gastKunde;  
                                ////bestellung.ApplicationUserID = gastKunde.ID.ToString();

                                await _context!.TblBestellung.AddAsync(bestellung);


                                gastKunde.Bestellung = bestellung;
                                gastKunde.Bestellnummer = bestellung.Bestellnummer;

                                await _context!.TblGastkunden.AddAsync(gastKunde!);
                                await _context.SaveChangesAsync();


                                bestellung.GastKundeID = (await _context.TblGastkunden.FirstOrDefaultAsync(x => x.Bestellung == bestellung))?.ID;


                                await _context.SaveChangesAsync();
                                transaction.Commit();
                            }

                            _warenkorb = warenkorbImSession; //// bevor der Session gelöscht wird, einmal koppieren in einem Objekt und zur View senden, um die Bestätigung der Bestellung zu gewährleisten

                            session?.Clear();

                            string rehnusEmail = "info@roomit-selfstorage.de";

                            BodyBuilder bodyBuilderRhenus = await Emailservice.Template2RhneusCreateAsync(_webHostEnvironment, gastKunde, bestellung);

                            if (bodyBuilderRhenus.HtmlBody != null)
                            {
                                var isMailSent = await Emailservice.SendAsync(_configuration, bodyBuilderRhenus, rehnusEmail, "Neue Bestellung");
                            }

                            ////BodyBuilder bodyBuilderKunde = await Emailservice.TemplateKundeCreateAsyncBilderWithCid(_webHostEnvironment, gastKunde, bestellung);

                            BodyBuilder bodyBuilderKunde = await Emailservice.Template2KundeCreateAsync(_webHostEnvironment, gastKunde, bestellung);

                            if (bodyBuilderKunde.HtmlBody != null)
                            {
                                var isMailSent = await Emailservice.SendAsync(_configuration, bodyBuilderKunde, gastKunde?.Email, "Bestellbestätigung");
                            }

                            //// **** Bis hier
                        }

                        #region Region ApplicationUser 

                        //else if (warenkorbImSession?.ApplicationUser != null && warenkorbImSession?.GastKunde == null)
                        //{

                        //    ApplicationUser? user = warenkorbImSession?.ApplicationUser;

                        //    bestellung = new Bestellung()
                        //    {
                        //        BestellStatus = "in Bearbeitung",
                        //        ApplicationUserID = warenkorbImSession?.ApplicationUser?.Id,
                        //        //GastKunde = warenkorbImSession?.GastKunde,
                        //        Bestellnummer = warenkorbImSession?.NL == "Köln" ? "K" + bestellungsnummer : "M" + bestellungsnummer,
                        //        FortlaufendeNummer = bestellungsnummer,
                        //        hasTowAdresse = warenkorbImSession!.ApplicationUser!.hasTowAdresse,
                        //        IPAdresse = warenkorbImSession?.IPAdresse!,
                        //        GesamtAnzahlArtikeln = warenkorbImSession!.Quantity,
                        //        Stadtname = warenkorbImSession.Stadtname,
                        //        GesamtBestellungsBetrag = warenkorbImSession.TotalAmount,
                        //        LieferDatum = warenkorbImSession.LieferDatum,
                        //        Niederlassung = warenkorbImSession.NL,
                        //        Postleitzahl = warenkorbImSession.Postleitzahl,
                        //        //LieferAdresse = warenkorbImSession?.ApplicationUser?.LieferAdresse!,
                        //    };

                        //    foreach (var item in warenkorbImSession?.WarenkorbItemsListe!)
                        //    {
                        //        bestellung.Positionen.Add(new Bestellpositionen()
                        //        {
                        //            Anzah = item.Quantity,
                        //            ArtikelName = item.ProduktName,
                        //            ProductImage = item.ProductImage,
                        //            Stückpreis = item.UnitPrice,
                        //            Summe = item.Total,
                        //        });
                        //    }

                        //    // Alle in einem Transaction hinzufügen und alle aufeinmal in die DB speichern oder nicht 
                        //    using (var transaction = _context.Database.BeginTransaction())
                        //    {
                        //        await _context.TblRechnungAdressen.AddAsync(user?.RechnungAdresse!);
                        //        await _context.SaveChangesAsync();

                        //        if (user?.LieferAdresse != null)
                        //        {
                        //            await _context.TblLieferAdresse.AddAsync(user?.LieferAdresse!);
                        //            await _context.SaveChangesAsync();

                        //            //bestellung.LieferAdresse = await _context.TblLieferAdresse.FirstOrDefaultAsync(r => r.ApplicationUserID == user!.Id);
                        //        }

                        //        await _context!.TblBestellung.AddAsync(bestellung);
                        //        await _context.SaveChangesAsync();

                        //        user!.RechnungAdresseID = _context?.TblRechnungAdressen.Count() > 0 ? (await _context.TblRechnungAdressen.FirstOrDefaultAsync(r => r.ApplicationUserID == user.Id))!.ID : 0;
                        //        user!.LieferAdresseID = _context?.TblLieferAdresse.Count() > 0 ? (await _context.TblLieferAdresse.FirstOrDefaultAsync(r => r.ApplicationUserID == user.Id))!.ID : 0;

                        //        result = await _userManager.CreateAsync(user!, user?.Password);

                        //        transaction.Commit();
                        //    }

                        //    if (result.Succeeded)
                        //    {


                        //        _logger.LogInformation("User created a new account with password.");

                        //        var userId = await _userManager.GetUserIdAsync(user!);
                        //        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user!);
                        //        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        //        var callbackUrl = Url.Page(
                        //            "/Account/ConfirmEmail",
                        //            pageHandler: null,
                        //            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        //            protocol: Request.Scheme);

                        //        //await _emailSender.SendEmailAsync(newKunde.Email, "Confirm your email",
                        //        //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        //        _warenkorb = warenkorbImSession;
                        //        session?.Clear();

                        //        // if bestellung dann userer anlegen if error dann als Gast anlegen und wenn nicht registrieren

                        //        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        //        {
                        //            //return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });

                        //            // Hier Return To Onther Site z.B. Kaufbestätigung
                        //            //return RedirectToAction("Index", "Shop");
                        //            return View(_warenkorb);
                        //        }
                        //        else
                        //        {
                        //            //await _signInManager.SignInAsync(user!, isPersistent: false);
                        //            return View(_warenkorb);
                        //            //return LocalRedirect(returnUrl);
                        //        }
                        //    }
                        //    // Username 'Sale441@hotmail.de' is already taken has removed.
                        //    //Email 'Sale441@hotmail.de' is already taken
                        //    foreach (var error in result.Errors)
                        //    {
                        //        if (error.Description.Contains("is already taken."))
                        //        {
                        //            error.Description = error.Description.Replace("is already taken.", "ist schon vergeben.");
                        //        }
                        //        if (!error.Description.Contains("Username"))
                        //        {
                        //            ModelState.AddModelError(string.Empty, error.Description);
                        //        }

                        //    }

                        //}

                        #endregion ApplicationUser 

                    }

                    //return View(_warenkorb);
                    return View(bestellung);
                }
                catch (SqlException e)
                {

                    //ApplicationUser getUser = await _userManager.FindByEmailAsync(warenkorbImSession?.ApplicationUser?.Email);

                    //if (getUser != null)
                    //{
                    //    await _userManager.DeleteAsync(getUser);
                    //}

                    ModelState.AddModelError("", "Es könnte keine Bestellung aufgenmone werde.");
                    return View("Checkout");
                    // Hier NOCH ADD return View Error Seite das er nochmal versuchen soll
                }
            }
            else 
            {
                ModelState.AddModelError("", "Es könnte keine Bestellung aufgenmone werde.");
                return View("Checkout");
            }
             
        }

        public IActionResult Kontakt()
        {
            return View(new Kontaktformular());
        }

        ////FriendlyCaptchaResponse
        //[HttpPost]
        //public async Task<IActionResult> Kontakt(Kontaktformular kontaktKunde)
        //{
        //    string rehnusEmail = "info@roomit-selfstorage.de";
        //    //string rehnusEmail = "saleh.khalil@rhenus.com";

        //    FriendlyCaptchaResponse captchaResponse = new FriendlyCaptchaResponse();

        //    #region Friendlycaptcha Abschnit Code

        //    var api_Key = _configuration["Friendlycaptcha:APIKey"];
        //    var sitekey = _configuration["Friendlycaptcha:SiteKey"];
        //    var httpClient = _httpClientFactory.CreateClient();

        //    var token = kontaktKunde?.FriendlycaptchaToken;  // V1
        //    var url = "https://api.friendlycaptcha.com/api/v1/siteverify"; // V1

        //    //var token = kontaktKunde?.FriendlycaptchaToken!; // V2
        //    //var url = "https://global.frcapi.com/api/v2/captcha/siteverify"; // V2



        //    HTTPRequest request = new HTTPRequest();

        //    captchaResponse = await request.HTTPRequestAsync(api_Key, sitekey, token, url, httpClient);

        //    if (!captchaResponse.success)
        //    {
        //        return View("Error", captchaResponse?.data?.challenge?.origin);
        //    }

        //    #endregion


        //    if (ModelState.IsValid && captchaResponse!.success)
        //    {
        //        try
        //        {

        //            BodyBuilder bodyBuilderRhenus = await Emailservice.TemplateRhneusCreateAsync(_webHostEnvironment, kontaktKunde);
        //            BodyBuilder bodyBuilderKunde = await Emailservice.TemplateKundeCreateAsync(_webHostEnvironment, kontaktKunde);

        //            if (bodyBuilderRhenus.HtmlBody != null)
        //            {
        //                var isMailSent = await Emailservice.SendAsync(_configuration, bodyBuilderRhenus, rehnusEmail, "Anfrage Kontaktformular");
        //            }


        //            //BodyBuilder bodyBuilderKunde = await Emailservice.TemplateKundeCreateAsync(_webHostEnvironment, kontaktKunde);

        //            if (bodyBuilderKunde.HtmlBody != null)
        //            {
        //                var isMailSent = await Emailservice.SendAsync(_configuration, bodyBuilderKunde, kontaktKunde.Email, "Bestätigung der Kontaktanfrage");
        //            }

        //            return View("Kontankbestätigung");
        //        }
        //        catch (HttpRequestException e)
        //        {
        //            //ViewBag.ErrorMessage = e.Message;

        //            return View("Error", e.Message);
        //        }
        //    }
        //    else
        //    {
        //        return View("Error", $"ModelState.IsValid: {ModelState.IsValid}, captchaResponse!.success: {captchaResponse!.success}");
        //    }

        //    //return View();   // zum testen

        //}

        //GoogleReCaptcha

        [HttpPost]
        public async Task<IActionResult> Kontakt(Kontaktformular kontaktKunde)
        {
            string rehnusEmail = "info@roomit-selfstorage.de";
            string secretKey = _configuration["GoogleReCaptcha:SecretKey"];

            var httpClient = _httpClientFactory.CreateClient();

            if (String.IsNullOrEmpty(kontaktKunde.GoogleReCaptchaToken))
            {
                ModelState.AddModelError("Contact.GoogleReCaptcha", "Bitte bestätige das reCAPTCHA.");
            }
            else
            {
                try
                {
                    var response = await httpClient.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={kontaktKunde.GoogleReCaptchaToken}", null);
                    if (!response.IsSuccessStatusCode)
                    {
                        ModelState.AddModelError("Contact.GoogleReCaptcha", "Fehler bei der Überprüfung des reCAPTCHA.");
                    }
                    else
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var reCaptchaResult = System.Text.Json.JsonSerializer.Deserialize<ReCaptchaResult>(jsonResponse);
                        if (reCaptchaResult == null || !reCaptchaResult.Success)
                        {
                            ModelState.AddModelError("Contact.GoogleReCaptcha", "reCAPTCHA-Überprüfung fehlgeschlagen.");
                        }
                    }
                }
                catch (Exception ex)
                {

                    _logger.LogError(ex, "Fehler bei der Überprüfung des reCAPTCHA.");
                    ModelState.AddModelError("Contact.GoogleReCaptcha", "reCAPTCHA-Überprüfung fehlgeschlagen.");
                    throw;
                }
            }

            //googleReCaptchaSuccess = false;

            if (ModelState.IsValid)
            {
                BodyBuilder bodyBuilderRhenus = await Emailservice.TemplateRhneusCreateAsync(_webHostEnvironment, kontaktKunde);
                BodyBuilder bodyBuilderKunde = await Emailservice.TemplateKundeCreateAsync(_webHostEnvironment, kontaktKunde);

                if (bodyBuilderRhenus.HtmlBody != null)
                {
                    var isMailSent = await Emailservice.SendAsync(_configuration, bodyBuilderRhenus, kontaktKunde.Email, "Anfrage Kontaktformular");
                }

                if (bodyBuilderKunde.HtmlBody != null)
                {
                    var isMailSent = await Emailservice.SendAsync(_configuration, bodyBuilderKunde, kontaktKunde.Email, "Bestätigung der Kontaktanfrage");
                }

                ViewBag.Message = "OK!";
                return View("Kontankbestätigung");
            }
            else
            {
                ViewBag.Message = "verifizieren Sie sich noch einmal mit dem Anti-Roboter-Button!";
                return View();
            }

            
        }

        public IActionResult Ablauf()
        {
            return View();
        }

        public IActionResult Packanleitung()
        {
            return View();
        }

        public IActionResult Vorteile()
        {
            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }

        public IActionResult Impressum()
        {
            return View();
        }

        public IActionResult Datenschutz()
        {
            return View();
        }

        public IActionResult AGB()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}