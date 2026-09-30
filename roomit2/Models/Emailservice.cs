using MimeKit;
using MimeKit.Utils;
using Org.BouncyCastle.Asn1.Misc;
using System.Globalization;
using System.Net.Mail;

namespace roomit2.Models
{
    public static class Emailservice
    {
               
        public static async Task<bool> SendAsync(IConfiguration configuration, BodyBuilder builder, string emailTo, string subject)
        {

            try
            {
              
                string from = configuration["EmailConfig:From"];
                string smtpServer = configuration["EmailConfig:SmtpServer"];
                string rhenusUsername = configuration["EmailConfig:RhenusUsername"];

                int port = Convert.ToInt32(configuration["EmailConfig:Port"]);

                SmtpClient smtp = new SmtpClient(smtpServer);

                smtp.UseDefaultCredentials = false;
                smtp.Port = port;

                MailAddress mailFrom = new MailAddress(from);
                MailAddress mailTo = new MailAddress(emailTo);

                MailMessage mailMessage = new MailMessage()
                {
                    Subject = subject,
                    SubjectEncoding = System.Text.Encoding.UTF8,
                    Body = builder.HtmlBody,
                    BodyEncoding = System.Text.Encoding.UTF8,
                    IsBodyHtml = true,
                    From = mailFrom,
                    Sender = mailFrom,
                    Priority = MailPriority.High,
                };

                mailMessage.To.Add(mailTo);

                //smtp.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

                await smtp.SendMailAsync(mailMessage);
                smtp.Dispose();

                //if (builder.HtmlBody != "")
                //{
                //    await SendAsync(to, subject, username, builder, configuration);
                //}

                return true;
            }
            catch (Exception e)
            {              
                //throw new Exception($"Email has not sent: {e.Message}");
                return false;
            }

        }

        // Kontaktformular Rhenus Temp
        public static async Task<BodyBuilder> TemplateRhneusCreateAsync(IWebHostEnvironment environment, Kontaktformular kontaktKunde)
        {
            string _temp2Pos = string.Empty;
            string htmlPositionen = string.Empty;
            var templatePath = "wwwroot/EmailTemplates/EmailKontaktFormularRhenus.html";
            string logoPath = "assets/images/room-it-logo.png";
            string imagePathInTestServer = "http://dehze01-wsv828:82/assets/images/"; // diese Adresse soll ersetzt werden wo Seite gehostet werden soll und so kann man auf die Bilder für Email zugreifen

            // Bilder Produkte im Outlook Cloud Speicher für das Einbetten der Bilder fürs Email Versand
            var imagesInOutlookCloud = new Dictionary<string, string>()
            {             
                { "room-it-logo.png","https://onedrive.live.com/embed?resid=5C7DBA71D835A974%21341&authkey=%21AGj-Yx8Y9uHXwgE&width=153&height=76"},

                { "html-IT-logo.png","https://onedrive.live.com/embed?resid=5C7DBA71D835A974%21340&authkey=%21ANs6AcldPMvdxE4&width=58&height=57"}

            };

            var builder = new BodyBuilder();

            templatePath = Path.Combine(environment.ContentRootPath, templatePath); // für templatePath soll ContentRootPath

            if (File.Exists(templatePath))
            {
                using (StreamReader htmlBody = File.OpenText(templatePath))
                {
                    builder.HtmlBody = await htmlBody.ReadToEndAsync();
                }

                builder.HtmlBody = builder.HtmlBody
                    //.Replace("$logo$", string.Format($"cid:{emailLogo.ContentId}"))
                    .Replace("$logo$", string.Format($"{imagesInOutlookCloud["room-it-logo.png"]}"))
                    //.Replace("$Datum$", string.Format($"{kontaktKunde.ErstellungssDatum}"))
                    .Replace("$Anrede$", string.Format($"{kontaktKunde.Anrede}"))
                    .Replace("$Vorname$", string.Format($"{kontaktKunde.Vorname}"))
                    .Replace("$Nachname$", string.Format($"{kontaktKunde.Nachname}"))
                    .Replace("$E-Mail$", string.Format($"{kontaktKunde.Email}"))
                    .Replace("$Telefon$", string.Format($"{kontaktKunde.Telefon}"))
                    .Replace("$Betreff$", string.Format($"{kontaktKunde.Betreff}"))
                    .Replace("$Komentar$", string.Format($"{kontaktKunde.Nachricht}"));

            }
            else
            {
                builder.HtmlBody = "<div><h1>ERRRRRRRROOOORRR</h1></div>";
            }



            return builder;
        }

        // Kontaktformular Kunde Temp
        public static async Task<BodyBuilder> TemplateKundeCreateAsync(IWebHostEnvironment environment, Kontaktformular kontaktKunde)
        {
            string _temp2Pos = string.Empty;
            string htmlPositionen = string.Empty;
            var templatePath = "wwwroot/EmailTemplates/EmailKontaktFormularKunde.html";
            string logoPath = "assets/images/room-it-logo.png";
            string imagePathInTestServer = "http://dehze01-wsv828:82/assets/images/"; // diese Adresse soll ersetzt werden wo Seite gehostet werden soll und so kann man auf die Bilder für Email zugreifen

            // Bilder Produkte im Outlook Cloud Speicher für das Einbetten der Bilder fürs Email Versand
            var imagesInOutlookCloud = new Dictionary<string, string>()
            {
                { "room-it-logo.png","https://onedrive.live.com/embed?resid=5C7DBA71D835A974%21341&authkey=%21AGj-Yx8Y9uHXwgE&width=153&height=76"},

                { "html-IT-logo.png","https://onedrive.live.com/embed?resid=5C7DBA71D835A974%21340&authkey=%21ANs6AcldPMvdxE4&width=58&height=57"}

            };

            var builder = new BodyBuilder();

            templatePath = Path.Combine(environment.ContentRootPath, templatePath); // für templatePath soll ContentRootPath

            if (File.Exists(templatePath))
            {
                using (StreamReader htmlBody = File.OpenText(templatePath))
                {
                    builder.HtmlBody = await htmlBody.ReadToEndAsync();
                }

                builder.HtmlBody = builder.HtmlBody
                    //.Replace("$logo$", string.Format($"cid:{emailLogo.ContentId}"))
                    .Replace("$logo$", string.Format($"{imagesInOutlookCloud["room-it-logo.png"]}"))
                    //.Replace("$Datum$", string.Format($"{kontaktKunde.ErstellungssDatum}"))
                    .Replace("$Vorname$", string.Format($"{kontaktKunde.Vorname}"))
                    .Replace("$Nachname$", string.Format($"{kontaktKunde.Nachname}"));
            }
            else
            {
                builder.HtmlBody = "<div><h1>ERRRRRRRROOOORRR</h1></div>";
            }



            return builder;
        }

        // Bestellung Rhenus Temp
        public static async Task<BodyBuilder> Template2RhneusCreateAsync(IWebHostEnvironment environment, GastKunde gastKunde, Bestellung bestellung)
        {
            string _temp2Pos = string.Empty;
            string htmlPositionen = string.Empty;
            var templatePath = "wwwroot/EmailTemplates/Temp2Rhenus.html";            
            string logoPath = "assets/images/room-it-logo.png";
            string imagePathInTestServer = "http://dehze01-wsv828:82/assets/images/"; // diese Adresse soll ersetzt werden wo Seite gehostet werden soll und so kann man auf die Bilder für Email zugreifen

            // Bilder Produkte im Outlook Cloud Speicher für das Einbetten der Bilder fürs Email Versand
            var imagesInOutlookCloud = new Dictionary<string, string>() 
            {
                { "Produkte_Karton3.png","https://1drv.ms/i/c/5c7dba71d835a974/IQNvbURnPWdeS7ZnkQOv8rcoATcqjWyJ7aHrqOXU-2o2Ox0?width=660" },
                { "Produkte_Container.png","https://1drv.ms/i/c/5c7dba71d835a974/IQNCvf_YlmY1QaIaCoRj-2S6AT2GunZU_95nkwiiQA-KTbQ?width=256"},
                { "Produkte_Einstuecke.png","https://1drv.ms/i/c/5c7dba71d835a974/IQOolTnnNWFPQrRqI1jLUChAAeFrQAx_IpKlakhncKtHlWQ?width=256" },

                { "room-it-logo.png","https://onedrive.live.com/embed?resid=5C7DBA71D835A974%21341&authkey=%21AGj-Yx8Y9uHXwgE&width=153&height=76"},
                
                { "html-IT-logo.png","https://onedrive.live.com/embed?resid=5C7DBA71D835A974%21340&authkey=%21ANs6AcldPMvdxE4&width=58&height=57"}
                          
            };

            //@Region Zwei Varabile die nicht verwendet sind
            string divLogo = "<div class=\"col num6\" style=\"min-width: 320px; max-width: 325px; display: table-cell; vertical-align: top; width: 325px;\">\r\n<div style=\"width:100% !important;\">\r\n<!--[if (!mso)&(!IE)]><!-->\r\n" +
                "<div style=\"border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:25px; padding-bottom:25px; padding-right: 0px; padding-left: 25px;\">\r\n<!--<![endif]-->\r\n <div align=\"left\" class=\"img-container left fullwidthOnMobile fixedwidth\" style=\"padding-right: 0px;padding-left: 0px;\">\r\n" +
                "<div style=\"font-size:1px;line-height:5px\"></div>\r\n<img alt=\"Logo\" id=\"logo\" border=\"0\" class=\"left fullwidthOnMobile fixedwidth\" src=\"$logo$\" style=\"text-decoration: none; -ms-interpolation-mode: bicubic; border: 0; height: auto; width: 100%; max-width: 195px; display: block;\" title=\"Image\" width=\"195\" />\r\n" +
                " </div>\r\n</div>\r\n</div>\r\n" +
                "</div>";


            string temp2RchLiefAdr = "<tr>\r\n " +
                "<td>\r\n " +
                "<table width=\"560\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"devicewidthinner\" style=\"border-bottom: 1px solid #bbbbbb;\">\r\n" +
                " <tbody>\r\n <tr>\r\n" +
                "<td style=\"font-size: 14px; line-height: 18px; color: #666666; padding-bottom: 25px;\"><strong>Anfragenummer:</strong> $Anfragenummer$ <strong>Anfragedatum:</strong> $Anfragedatum$ </td>\r\n" +
                " </tr>\r\n<tr>\r\n<td style=\"width: 55%; font-size: 16px; font-weight: bold; color: #666666; padding-bottom: 5px;\">\r\nRECHNUNGSADRESSE\r\n" +
                "</td>\r\n <td style=\"width: 45%; font-size: 16px; font-weight: bold; color: #666666; padding-bottom: 5px;\">\r\nLIEFERADRESSE\r\n" +
                "</td>\r\n</tr>\r\n<tr>\r\n<td style=\"width: 55%; font-size: 14px; line-height: 18px; color: #666666;\">\r\n     " +
                "§RVorName$\r\n </td>\r\n<td style=\"width: 45%; font-size: 14px; line-height: 18px; color: #666666;\">\r\n " +
                " $LVorName$\r\n</td>\r\n </tr>\r\n <tr>\r\n" +
                "<td style=\"width: 55%; font-size: 14px; line-height: 18px; color: #666666;\">\r\n$RStrasse$\r\n</td>\r\n" +
                "<td style=\"width: 45%; font-size: 14px; line-height: 18px; color: #666666;\">\r\n$LStrasse$\r\n</td>\r\n </tr>\r\n" +
                "<tr>\r\n<td style=\"width: 55%; font-size: 14px; line-height: 18px; color: #666666; padding-bottom: 10px;\">\r\n $ROrt$  &nbsp;&nbsp;&nbsp;  $RPLZ$\r\n  " +
                "</td>\r\n<td style=\"width: 45%; font-size: 14px; line-height: 18px; color: #666666; padding-bottom: 10px;\">\r\n" +
                " $LOrt$  &nbsp;&nbsp;&nbsp;  $LPLZ$\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n" +
                "  </td>\r\n</tr>";

            string temp2Pos = "\r\n<tr>\r\n<td style=\"padding-top: 0;\">\r\n<table width=\"700\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"devicewidthinner\" style=\"border-bottom: 1px solid #eeeeee;\">\r\n" +
                "<tbody>\r\n <tr>\r\n<td rowspan=\"4\" style=\"padding-right: 10px; padding-bottom: 10px; width: 280px;\">\r\n<img style=\"height: 100px;width: 200px;\" src=\"$image$\" alt=\"Product Image\" />\r\n " +
                "</td>\r\n<td style=\"font-size: 15px; font-weight: bold; color: #666666; padding-bottom: 5px; text-align: left;\">$ArtikelName$</td>\r\n</tr>\r\n<tr>\r\n" +
                " <td colspan=\"2\" style=\"font-size: 14px; line-height: 18px; color: gray; text-align: left;\">Preis:</td>\r\n<td colspan=\"2\" style=\" font-size: 14px; line-height: 18px; color: #757575; text-align: right; width: 25%; padding-right: 5%;\">$Pries$ </td>\r\n" +
                " </tr>\r\n<tr>\r\n<td colspan=\"2\" style=\"font-size: 14px; line-height: 18px; color: gray; text-align: left;\"> Menge: </td>\r\n<td colspan=\"2\" style=\" font-size: 14px; line-height: 18px; color: #757575; text-align: right; width: 25%; padding-right: 5%;\"> $Menge$ </td>\r\n" +
                " </tr>\r\n <tr>\r\n<td colspan=\"2\" style=\"font-size: 14px; line-height: 18px; color: gray; text-align: left;\"> Gesamt: </td>\r\n<td colspan=\"2\" style=\" font-size: 14px; line-height: 18px; color: #757575; text-align: right; width: 25%; padding-right: 5%;\">\r\n" +
                " <b style=\"color: #666666;\">$Total$</b>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>";

            //@endregion
     

            var builder = new BodyBuilder();

            templatePath = Path.Combine(environment.ContentRootPath, templatePath); // für templatePath soll ContentRootPath

            if (File.Exists(templatePath))
            {
                using (StreamReader htmlBody = File.OpenText(templatePath))
                {
                    builder.HtmlBody = await htmlBody.ReadToEndAsync();
                }

                string liefStrasse = gastKunde.LiefStraße != null ? gastKunde.LiefStraße : gastKunde.RechStraße;
                string liefOrt = gastKunde.LiefOrt != null ? gastKunde.LiefOrt : gastKunde.RechOrt;
                string? liefPLZ = gastKunde.LiefPLZ != null ? gastKunde.LiefPLZ : gastKunde.RechPLZ;

                // Logo Bild in das Template einbetten
                string absolutePathLogo = Path.Combine(environment.WebRootPath,logoPath);

                //ImageLinedResource imageLined = new ImageLinedResource()
                //{
                //    ContentType = "image/png",
                //    FileName = absolutePathLogo,
                //    ContentData = File.ReadAllBytes(absolutePathLogo),                 
                //};

                //var emailLogo = builder.LinkedResources.Add(imageLined.FileName);              
                //emailLogo.ContentId = MimeUtils.GenerateMessageId();
                      
                builder.HtmlBody = builder.HtmlBody
                    //.Replace("$logo$", string.Format($"cid:{emailLogo.ContentId}"))
                    .Replace("$logo$", string.Format($"{imagesInOutlookCloud["room-it-logo.png"]}"))
                    .Replace("$Auftragsnummer$", string.Format($"{bestellung.Bestellnummer}"))
                    //.Replace("$Anfragedatum$", string.Format($"{bestellung.BestellungssDatum}"))
                    .Replace("$Anrede$", string.Format($"{gastKunde.Anrede}")) 

                    .Replace("$RVorname$", string.Format($"{gastKunde.Vorname}"))
                    .Replace("$RNachname$", string.Format($"{gastKunde.Nachname}"))
                    .Replace("$RStrasse$", string.Format($"{gastKunde.RechStraße}"))
                    .Replace("$RPLZ$", string.Format($"{gastKunde.RechPLZ}"))
                    .Replace("$ROrt$", string.Format($"{gastKunde.RechOrt}"))

                    .Replace("$LVorname$", string.Format($"{gastKunde.Vorname}"))
                    .Replace("$LNachname$", string.Format($"{gastKunde.Nachname}"))
                    .Replace("$LStrasse$", string.Format($"{liefStrasse}"))
                    .Replace("$LPLZ$", string.Format($"{liefPLZ}"))
                    .Replace("$LOrt$", string.Format($"{liefOrt}"))
                    .Replace("$TEL$", string.Format($"{gastKunde.Telefon}"))
                    .Replace("$Mail$", string.Format($"{gastKunde.Email}"));

                // Bilder Dynamisch in der Email Template einsetzen
                // Schritt NR.1 Absolute Pfad zusammen setzen
                // Schritt NR.2 Name des Bildes spliten, die im HTML Template in src gesetzt war => <img src="BildName" />
                // Schritt NR.3 Absolute Pfad an builder.LinkedResources hängen
                // Schritt NR.4 ContentId genrieren
                // Schritt NR.5 der cid:BildName durch genrierten ContentId ersetzen


                for (int i = 0; i < bestellung.Positionen.Count; i++)
                {                    
                    var item = bestellung.Positionen[i]; // /assets/images/container-Box3.png
                    //var tt = item.ProductImage.TrimStart('/'); // assets/images/container-Box3.png   // Ohne /
                    //var tt2 = item.ProductImage.Remove(0,1);  // assets/images/container-Box3.png   // Ohne /
                    //var itemBild = Path.Combine(environment.WebRootPath,tt); 

                    //ImageLinedResource imageLined2 = new ImageLinedResource()
                    //{
                    //    ContentType = "image/png",
                    //    FileName = itemBild,
                    //    ContentData = File.ReadAllBytes(itemBild),
                    //};

                    // Byte[] itemBildByts = File.ReadAllBytes(itemBild);
                    //var produktBild = builder.LinkedResources.Add(itemBild);
                    //produktBild.ContentId = MimeUtils.GenerateMessageId();

                    //// GiltNUR den Test server
                    //string imageFileName = Path.GetFileName(itemBild);
                    //string absolutePathImTestServer = Path.Combine(imagePathInTestServer,imageFileName);

                    string imageFileName = Path.GetFileName(item?.ProductImage!);
                    string einheit = string.Empty;

                    switch (item?.ArtikelName) 
                    {
                        case "Karton-Box": einheit = "Kartons"; break;
                        case "Container-Box-Metall":
                        case "Container-Box-Holz": einheit = "Container"; break;
                        case "Einzelstücke":
                            if (item.Anzah > 1)
                            {
                                einheit = "Einzelstücke";
                            }
                            else
                            {
                                einheit = "Einzelstück";
                            }
                            break; 
                           
                    }

                    _temp2Pos = temp2Pos
                                //.Replace("$image$", string.Format($"cid:{produktBild.ContentId}"))
                                .Replace("$image$", string.Format($"{imagesInOutlookCloud[imageFileName]}"))
                                .Replace("$ArtikelName$", string.Format(item?.ArtikelName!))
                                .Replace("$Pries$", string.Format($"{item!.Stückpreis.ToString("C2",CultureInfo.GetCultureInfo("de-DE"))}"))
                                .Replace("$Menge$", string.Format(item.Anzah.ToString() + " " + einheit))
                                .Replace("$Total$", string.Format($"{item.Summe.ToString("C2", CultureInfo.GetCultureInfo("de-DE"))}"));


                    htmlPositionen += _temp2Pos;

                }

                builder.HtmlBody = builder.HtmlBody
                   .Replace("$Positionen$", string.Format($"{htmlPositionen}"))
                   .Replace("$TotalOhneMwSt$", string.Format($"{(bestellung.GesamtBestellungsBetrag * (1 - 0.19)).ToString("C2", CultureInfo.GetCultureInfo("de-DE"))}"))
                   .Replace("$MwSt$", string.Format($"{19}"))
                   .Replace("$TotalMitMwSt$", string.Format($"{bestellung.GesamtBestellungsBetrag.ToString("C2", CultureInfo.GetCultureInfo("de-DE"))}"));

            }
            else
            {
                builder.HtmlBody = "<div><h1>ERRRRRRRROOOORRR</h1></div>";
            }



            return builder;
        }

        // Bestellbestätigung Kunde Temp
        public static async Task<BodyBuilder> Template2KundeCreateAsync(IWebHostEnvironment environment, GastKunde gastKunde, Bestellung bestellung)
        {
            string _temp2Pos = string.Empty;
            string htmlPositionen = string.Empty;
            var templatePath = "wwwroot/EmailTemplates/Temp2Kunde.html";
            string logoPath = "assets/images/room-it-logo.png";
            string imagePathInTestServer = "http://dehze01-wsv828:82/assets/images/"; // diese Adresse soll ersetzt werden wo Seite gehostet werden soll und so kann man auf die Bilder für Email zugreifen

            // Bilder Produkte im Outlook Cloud Speicher für das Einbetten der Bilder fürs Email Versand
            var imagesInOutlookCloud = new Dictionary<string, string>()
            {
                { "Produkte_Karton3.png","https://1drv.ms/i/c/5c7dba71d835a974/IQNvbURnPWdeS7ZnkQOv8rcoATcqjWyJ7aHrqOXU-2o2Ox0?width=660" },
                { "Produkte_Container.png","https://1drv.ms/i/c/5c7dba71d835a974/IQNCvf_YlmY1QaIaCoRj-2S6AT2GunZU_95nkwiiQA-KTbQ?width=256"},
                { "Produkte_Einstuecke.png","https://1drv.ms/i/c/5c7dba71d835a974/IQOolTnnNWFPQrRqI1jLUChAAeFrQAx_IpKlakhncKtHlWQ?width=256" },

                { "room-it-logo.png","https://onedrive.live.com/embed?resid=5C7DBA71D835A974%21341&authkey=%21AGj-Yx8Y9uHXwgE&width=153&height=76"},

                { "html-IT-logo.png","https://onedrive.live.com/embed?resid=5C7DBA71D835A974%21340&authkey=%21ANs6AcldPMvdxE4&width=58&height=57"}

            };

            //@Region Zwei Varabile die nicht verwendet sind
            string divLogo = "<div class=\"col num6\" style=\"min-width: 320px; max-width: 325px; display: table-cell; vertical-align: top; width: 325px;\">\r\n<div style=\"width:100% !important;\">\r\n<!--[if (!mso)&(!IE)]><!-->\r\n" +
                "<div style=\"border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:25px; padding-bottom:25px; padding-right: 0px; padding-left: 25px;\">\r\n<!--<![endif]-->\r\n <div align=\"left\" class=\"img-container left fullwidthOnMobile fixedwidth\" style=\"padding-right: 0px;padding-left: 0px;\">\r\n" +
                "<div style=\"font-size:1px;line-height:5px\"></div>\r\n<img alt=\"Logo\" id=\"logo\" border=\"0\" class=\"left fullwidthOnMobile fixedwidth\" src=\"$logo$\" style=\"text-decoration: none; -ms-interpolation-mode: bicubic; border: 0; height: auto; width: 100%; max-width: 195px; display: block;\" title=\"Image\" width=\"195\" />\r\n" +
                " </div>\r\n</div>\r\n</div>\r\n" +
                "</div>";


            string temp2RchLiefAdr = "<tr>\r\n " +
                "<td>\r\n " +
                "<table width=\"560\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"devicewidthinner\" style=\"border-bottom: 1px solid #bbbbbb;\">\r\n" +
                " <tbody>\r\n <tr>\r\n" +
                "<td style=\"font-size: 14px; line-height: 18px; color: #666666; padding-bottom: 25px;\"><strong>Anfragenummer:</strong> $Anfragenummer$ <strong>Anfragedatum:</strong> $Anfragedatum$ </td>\r\n" +
                " </tr>\r\n<tr>\r\n<td style=\"width: 55%; font-size: 16px; font-weight: bold; color: #666666; padding-bottom: 5px;\">\r\nRECHNUNGSADRESSE\r\n" +
                "</td>\r\n <td style=\"width: 45%; font-size: 16px; font-weight: bold; color: #666666; padding-bottom: 5px;\">\r\nLIEFERADRESSE\r\n" +
                "</td>\r\n</tr>\r\n<tr>\r\n<td style=\"width: 55%; font-size: 14px; line-height: 18px; color: #666666;\">\r\n     " +
                "§RVorName$\r\n </td>\r\n<td style=\"width: 45%; font-size: 14px; line-height: 18px; color: #666666;\">\r\n " +
                " $LVorName$\r\n</td>\r\n </tr>\r\n <tr>\r\n" +
                "<td style=\"width: 55%; font-size: 14px; line-height: 18px; color: #666666;\">\r\n$RStrasse$\r\n</td>\r\n" +
                "<td style=\"width: 45%; font-size: 14px; line-height: 18px; color: #666666;\">\r\n$LStrasse$\r\n</td>\r\n </tr>\r\n" +
                "<tr>\r\n<td style=\"width: 55%; font-size: 14px; line-height: 18px; color: #666666; padding-bottom: 10px;\">\r\n $ROrt$  &nbsp;&nbsp;&nbsp;  $RPLZ$\r\n  " +
                "</td>\r\n<td style=\"width: 45%; font-size: 14px; line-height: 18px; color: #666666; padding-bottom: 10px;\">\r\n" +
                " $LOrt$  &nbsp;&nbsp;&nbsp;  $LPLZ$\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n" +
                "  </td>\r\n</tr>";

            // div für Postionen und Imges
            string temp2Pos = "<table width=\"700\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"devicewidthinner\" style=\"border-bottom: 1px solid #eeeeee;\">\r\n" +
                "<tbody>\r\n <tr>\r\n<td rowspan=\"4\" style=\"padding-right: 10px; padding-bottom: 10px; width: 280px;\">\r\n<img style=\"height: 100px;width:200px;\" src=\"$image$\" alt=\"Product Image\" />\r\n " +
                "</td>\r\n<td style=\"font-size: 15px; font-weight: bold; color: #666666; padding-bottom: 5px; text-align: left;\">$ArtikelName$</td>\r\n</tr>\r\n<tr>\r\n" +
                " <td colspan=\"2\" style=\"font-size: 14px; line-height: 18px; color: gray; text-align: left;\">Preis:</td>\r\n<td colspan=\"2\" style=\" font-size: 14px; line-height: 18px; color: #757575; text-align: right; width: 25%; padding-right: 5%;\">$Pries$ </td>\r\n" +
                " </tr>\r\n<tr>\r\n<td colspan=\"2\" style=\"font-size: 14px; line-height: 18px; color: gray; text-align: left;\"> Menge: </td>\r\n<td colspan=\"2\" style=\" font-size: 14px; line-height: 18px; color: #757575; text-align: right; width: 25%; padding-right: 5%;\"> $Menge$ </td>\r\n" +
                " </tr>\r\n <tr>\r\n<td colspan=\"2\" style=\"font-size: 14px; line-height: 18px; color: gray; text-align: left;\"> Gesamt: </td>\r\n<td colspan=\"2\" style=\" font-size: 14px; line-height: 18px; color: #757575; text-align: right; width: 25%; padding-right: 5%;\">\r\n" +
                " <b style=\"color: #666666;\">$Total$</b>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>";

            //@endregion


            var builder = new BodyBuilder();

            templatePath = Path.Combine(environment.ContentRootPath, templatePath); // für templatePath soll ContentRootPath

            if (File.Exists(templatePath))
            {
                using (StreamReader htmlBody = File.OpenText(templatePath))
                {
                    builder.HtmlBody = await htmlBody.ReadToEndAsync();
                }

                string liefStrasse = gastKunde.LiefStraße != null ? gastKunde.LiefStraße : gastKunde.RechStraße;
                string liefOrt = gastKunde.LiefOrt != null ? gastKunde.LiefOrt : gastKunde.RechOrt;
                string? liefPLZ = gastKunde.LiefPLZ != null ? gastKunde.LiefPLZ : gastKunde.RechPLZ;

                // Logo Bild in das Template einbetten
                string absolutePathLogo = Path.Combine(environment.WebRootPath, logoPath);

                //ImageLinedResource imageLined = new ImageLinedResource()
                //{
                //    ContentType = "image/png",
                //    FileName = absolutePathLogo,
                //    ContentData = File.ReadAllBytes(absolutePathLogo),                 
                //};

                //var emailLogo = builder.LinkedResources.Add(imageLined.FileName);              
                //emailLogo.ContentId = MimeUtils.GenerateMessageId();

                builder.HtmlBody = builder.HtmlBody
                    //.Replace("$logo$", string.Format($"cid:{emailLogo.ContentId}"))
                    .Replace("$logo$", string.Format($"{imagesInOutlookCloud["room-it-logo.png"]}"))
                    .Replace("$Auftragsnummer$", string.Format($"{bestellung.Bestellnummer}"))
                    //.Replace("$Anfragedatum$", string.Format($"{bestellung.BestellungssDatum}"))
                    .Replace("$Anrede$", string.Format($"{gastKunde.Anrede}"))

                    .Replace("$RVorname$", string.Format($"{gastKunde.Vorname}"))
                    .Replace("$RNachname$", string.Format($"{gastKunde.Nachname}"))
                    .Replace("$RStrasse$", string.Format($"{gastKunde.RechStraße}"))
                    .Replace("$RPLZ$", string.Format($"{gastKunde.RechPLZ}"))
                    .Replace("$ROrt$", string.Format($"{gastKunde.RechOrt}"))

                    .Replace("$LVorname$", string.Format($"{gastKunde.Vorname}"))
                    .Replace("$LNachname$", string.Format($"{gastKunde.Nachname}"))
                    .Replace("$LStrasse$", string.Format($"{liefStrasse}"))
                    .Replace("$LPLZ$", string.Format($"{liefPLZ}"))
                    .Replace("$LOrt$", string.Format($"{liefOrt}"));

                // Bilder Dynamisch in der Email Template einsetzen
                // Schritt NR.1 Absolute Pfad zusammen setzen
                // Schritt NR.2 Name des Bildes spliten, die im HTML Template in src gesetzt war => <img src="BildName" />
                // Schritt NR.3 Absolute Pfad an builder.LinkedResources hängen
                // Schritt NR.4 ContentId genrieren
                // Schritt NR.5 der cid:BildName durch genrierten ContentId ersetzen


                for (int i = 0; i < bestellung.Positionen.Count; i++)
                {
                    var item = bestellung.Positionen[i]; // /assets/images/container-Box3.png
                    //var tt = item.ProductImage.TrimStart('/'); // assets/images/container-Box3.png   // Ohne /
                    //var tt2 = item.ProductImage.Remove(0,1);  // assets/images/container-Box3.png   // Ohne /
                    //var itemBild = Path.Combine(environment.WebRootPath,tt); 

                    //ImageLinedResource imageLined2 = new ImageLinedResource()
                    //{
                    //    ContentType = "image/png",
                    //    FileName = itemBild,
                    //    ContentData = File.ReadAllBytes(itemBild),
                    //};

                    // Byte[] itemBildByts = File.ReadAllBytes(itemBild);
                    //var produktBild = builder.LinkedResources.Add(itemBild);
                    //produktBild.ContentId = MimeUtils.GenerateMessageId();

                    //// GiltNUR den Test server
                    //string imageFileName = Path.GetFileName(itemBild);
                    //string absolutePathImTestServer = Path.Combine(imagePathInTestServer,imageFileName);

                    string imageFileName = Path.GetFileName(item?.ProductImage!);
                    string einheit = string.Empty;

                    switch (item?.ArtikelName)
                    {
                        case "Karton-Box": einheit = "Kartons"; break;
                        case "Container-Box-Metall":
                        case "Container-Box-Holz": einheit = "Container"; break;
                        case "Einzelstücke":
                            if (item.Anzah > 1)
                            {
                                einheit = "Einzelstücke";
                            }
                            else
                            {
                                einheit = "Einzelstück";
                            }
                            break;

                    }

                    _temp2Pos = temp2Pos
                                //.Replace("$image$", string.Format($"cid:{produktBild.ContentId}"))
                                .Replace("$image$", string.Format($"{imagesInOutlookCloud[imageFileName]}"))
                                .Replace("$ArtikelName$", string.Format(item?.ArtikelName!))
                                .Replace("$Pries$", string.Format($"{item!.Stückpreis.ToString("C2", CultureInfo.GetCultureInfo("de-DE"))}"))
                                .Replace("$Menge$", string.Format(item.Anzah.ToString() + " " + einheit))
                                .Replace("$Total$", string.Format($"{item.Summe.ToString("C2", CultureInfo.GetCultureInfo("de-DE"))}"));


                    htmlPositionen += _temp2Pos;

                }

                builder.HtmlBody = builder.HtmlBody
                   .Replace("$Positionen$", string.Format($"{htmlPositionen}"))
                   .Replace("$TotalOhneMwSt$", string.Format($"{(bestellung.GesamtBestellungsBetrag * (1 - 0.19)).ToString("C2", CultureInfo.GetCultureInfo("de-DE"))}"))
                   .Replace("$MwSt$", string.Format($"{19}"))
                   .Replace("$TotalMitMwSt$", string.Format($"{bestellung.GesamtBestellungsBetrag.ToString("C2", CultureInfo.GetCultureInfo("de-DE"))}"));

            }
            else
            {
                builder.HtmlBody = "<div><h1>ERRRRRRRROOOORRR</h1></div>";
            }



            return builder;
        }

        public static async Task<BodyBuilder> TemplateKundeCreateAsync(IWebHostEnvironment environment, GastKunde gastKunde, Bestellung bestellung)
        {
            string _temp1Pos = string.Empty;
            string htmlPositionen = string.Empty;
            var templatePath = "wwwroot/EmailTemplates/Temp1.html";
            string logoPath = "assets/images/room-it-logo.png";
            string imagePathInTestServer = "http://dehze01-wsv828:82/assets/images/"; // diese Adresse soll ersetzt werden wo Seite gehostet werden soll und so kann man auf die Bilder für Email zugreifen

            // Bilder Produkte im Outlook Cloud Speicher für das Einbetten der Bilder fürs Email Versand
            var imagesInOutlookCloud = new Dictionary<string, string>()
            {
                { "Produkte_Karton3.png","https://1drv.ms/i/c/5c7dba71d835a974/IQNvbURnPWdeS7ZnkQOv8rcoATcqjWyJ7aHrqOXU-2o2Ox0?width=660" },
                { "Produkte_Container.png","https://1drv.ms/i/c/5c7dba71d835a974/IQNCvf_YlmY1QaIaCoRj-2S6AT2GunZU_95nkwiiQA-KTbQ?width=256"},
                { "Produkte_Einstuecke.png","https://1drv.ms/i/c/5c7dba71d835a974/IQOolTnnNWFPQrRqI1jLUChAAeFrQAx_IpKlakhncKtHlWQ?width=256" },

                { "html-IT-logo.png","https://onedrive.live.com/embed?resid=5C7DBA71D835A974%21340&authkey=%21ANs6AcldPMvdxE4&width=58&height=57"},

                { "room-it-logo.png","https://onedrive.live.com/embed?resid=5C7DBA71D835A974%21341&authkey=%21AGj-Yx8Y9uHXwgE&width=153&height=76"},

            };

            // string Temp1
            string temp1Pos = "<div style=\"border-collapse: collapse;display: table;width: 100%;background-color:#FFFFFF;\">\r\n<div class=\"col num3\" style=\"max-width: 320px; min-width: 162px; display: table-cell; vertical-align: top; width: 162px;\">\r\n <div style=\"width:100% !important;\">\r\n" +
                "<!--[if (!mso)&(!IE)]><!-->\r\n<div style=\"border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;\">\r\n<!--<![endif]-->\r\n" +
                "<div align=\"center\" class=\"img-container center fixedwidth\" style=\"padding-right: 0px;padding-left: 0px;\">\r\n<img align=\"center\" alt=\"Image\" border=\"0\" class=\"center fixedwidth\" src=\"$image$\" style=\"text-decoration: none; -ms-interpolation-mode: bicubic; border: 0; height: auto; width: 100%; max-width: 130px; display: block;\" title=\"Image\" width=\"130\" />\r\n" +
                "</div>\r\n</div>\r\n</div>\r\n</div>\r\n<div class=\"col num3\" style=\"max-width: 320px; min-width: 162px; display: table-cell; vertical-align: top; width: 161px;\">\r\n<div style=\"width:100% !important;\">\r\n" +
                "<!--[if (!mso)&(!IE)]><!-->\r\n<div style=\"border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:1px dotted #E8E8E8; padding-top:30px; padding-bottom:35px; padding-right: 0px; padding-left: 0px;\">\r\n" +
                "<div style=\"color:#555555;font-family:'Lato', Tahoma, Verdana, Segoe, sans-serif;line-height:120%;padding-top:10px;padding-right:10px;padding-bottom:5px;padding-left:0px;\">\r\n<div style=\"line-height: 14px; font-family: 'Lato', Tahoma, Verdana, Segoe, sans-serif; font-size: 12px; color: #555555;\">\r\n" +
                "<p style=\"line-height: 14px; text-align: left; font-size: 12px; margin: 0;\"><span style=\"color: #2190e3;\"><span style=\"font-size: 16px; line-height: 19px;\"><strong>$ArtikelName$</strong></span></span></p>\r\n</div>\r\n</div>\r\n" +
                "<div style=\"color:#555555;font-family:'Lato', Tahoma, Verdana, Segoe, sans-serif;line-height:120%;padding-top:0px;padding-right:10px;padding-bottom:10px;padding-left:0px;\">\r\n<p style=\"font-size: 12px; line-height: 14px; color: #555555; font-family: 'Lato', Tahoma, Verdana, Segoe, sans-serif; margin: 0;\"> $Pries$ </p>\r\n" +
                "</div>\r\n</div>\r\n<!--<![endif]-->\r\n</div>\r\n</div>\r\n<div class=\"col num3\" style=\"max-width: 320px; min-width: 162px; display: table-cell; vertical-align: top; width: 161px;\">\r\n" +
                "<div style=\"width:100% !important;\">\r\n<!--[if (!mso)&(!IE)]><!-->\r\n<div style=\"border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:1px dotted #E8E8E8; padding-top:55px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;\">\r\n" +
                "<div style=\"color:#555555;font-family:'Lato', Tahoma, Verdana, Segoe, sans-serif;line-height:120%;padding-top:0px;padding-right:10px;padding-bottom:10px;padding-left:10px;\">\r\n<div style=\"font-size: 12px; line-height: 14px; font-family: 'Lato', Tahoma, Verdana, Segoe, sans-serif; color: #555555;\">\r\n" +
                "<p style=\"font-size: 14px; line-height: 24px; text-align: center; margin: 0;\"><span style=\"font-size: 16px;\"><strong>$Menge$</strong></span></p>\r\n</div>\r\n </div>\r\n <!--[if mso]></td></tr></table><![endif]-->\r\n" +
                "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"divider\" role=\"presentation\" style=\"table-layout: fixed; vertical-align: top; border-spacing: 0; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt; min-width: 100%; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;\" valign=\"top\" width=\"100%\">\r\n" +
                "<tbody>\r\n<tr style=\"vertical-align: top;\" valign=\"top\">\r\n<td class=\"divider_inner\" style=\"word-break: break-word; vertical-align: top; min-width: 100%; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%; padding-top: 10px; padding-right: 10px; padding-bottom: 10px; padding-left: 10px;\" valign=\"top\">\r\n   " +
                "<table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"divider_content\" height=\"30\" role=\"presentation\" style=\"table-layout: fixed; vertical-align: top; border-spacing: 0; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%; border-top: 0px solid transparent; height: 30px;\" valign=\"top\" width=\"100%\">\r\n     " +
                "<tbody>\r\n<tr style=\"vertical-align: top;\" valign=\"top\">\r\n<td height=\"30\" style=\"word-break: break-word; vertical-align: top; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;\" valign=\"top\"><span></span></td>\r\n" +
                "</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n <!--[if (!mso)&(!IE)]><!-->\r\n" +
                "</div>\r\n <!--<![endif]-->\r\n</div>\r\n</div>\r\n<div class=\"col num3\" style=\"max-width: 320px; min-width: 162px; display: table-cell; vertical-align: top; width: 162px;\">\r\n<div style=\"width:100% !important;\">\r\n<!--[if (!mso)&(!IE)]><!-->\r\n" +
                "<div style=\"border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:55px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;\">\r\n<div style=\"color:#555555;font-family:'Lato', Tahoma, Verdana, Segoe, sans-serif;line-height:120%;padding-top:0px;padding-right:15px;padding-bottom:0px;padding-left:0px;\">\r\n                                                    <div style=\"line-height: 14px; font-size: 12px; font-family: 'Lato', Tahoma, Verdana, Segoe, sans-serif; color: #555555;\">\r\n                                                        " +
                "<p style=\"line-height: 24px; text-align: center; font-size: 12px; margin: 0;\"><span style=\"font-size: 20px;\"><span style=\"line-height: 24px; font-size: 20px;\"><strong>$Total$</strong></span></span></p>\r\n</div>\r\n</div>\r\n<!--[if mso]></td></tr></table><![endif]-->\r\n<!--[if (!mso)&(!IE)]><!-->\r\n</div>\r\n<!--<![endif]-->\r\n</div>\r\n</div>\r\n  " +
                "<!--[if (mso)|(IE)]></td></tr></table><![endif]-->\r\n<!--[if (mso)|(IE)]></td></tr></table></td></tr></table><![endif]-->\r\n</div>";

            string bestellungDaten = "<div style=\"color:#787878;font-family:'Lato', Tahoma, Verdana, Segoe, sans-serif;line-height:150%;padding-top:0px;padding-right:10px;padding-bottom:30px;padding-left:10px;\">\r\n<div style=\"font-size: 12px; line-height: 18px; font-family: 'Lato', Tahoma, Verdana, Segoe, sans-serif; color: #787878;\">\r\n" +
                "<h3>Anfragenummer:&nbsp;&nbsp;<span style=\"color:gray;\">$Anfragenummer$ </span></h3>\r\n<h3>Anfragedatum:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=\"color:gray;\">$Anfragedatum$ </span></h3>\r\n<p style=\"margin-top:0;margin-bottom:0;\"><span style=\"line-height: 40px; font-size: 20px;\">RECHNUNGSADRESSE</span></p>\r\n " +
                "<p style=\"margin-top:0;margin-bottom:0;\"><span style=\" font-size: 14px;\">$Anrede$</span></p>\r\n<p style=\"margin-top:0;margin-bottom:0;\"><span style=\" font-size: 14px;\">$RVorname$ </span> &nbsp;&nbsp;  <span>$RNachname$</span></p>\r\n<p style=\"margin-top:0;margin-bottom:0;\"><span style=\" font-size: 14px;\">$RStrasse$</span></p>\r\n" +
                "<p style=\"margin-top:0;margin-bottom:0;\"><span style=\" font-size: 14px;\">$ROrt$</span> &nbsp;&nbsp;  <span>$RPLZ$</span></p>\r\n</div>\r\n<br />\r\n<div style=\"font-size: 12px; line-height: 18px; font-family: 'Lato', Tahoma, Verdana, Segoe, sans-serif; color: #787878;\">\r\n" +
                "<p style=\"margin-top:0;margin-bottom:0;\"><span style=\"line-height: 40px; font-size: 20px;\">LIEFERADRESSE </span></p>\r\n<p style=\"margin-top:0;margin-bottom:0;\"><span style=\" font-size: 14px;\">$Anrede$</span></p>\r\n<p style=\"margin-top:0;margin-bottom:0;\"><span style=\" font-size: 14px;\">$LVorname$ </span> &nbsp;&nbsp;  <span>$LNachname$</span></p>\r\n" +
                "<p style=\"margin-top:0;margin-bottom:0;\"><span style=\" font-size: 14px;\">$LStrasse$</span></p>\r\n<p style=\"margin-top:0;margin-bottom:0;\"><span style=\" font-size: 14px;\">$LOrt$</span> &nbsp;&nbsp;<span>$LPLZ$</span></p>\r\n</div>\r\n</div>";

            var builder = new BodyBuilder();

            templatePath = Path.Combine(environment.ContentRootPath, templatePath); // für templatePath soll ContentRootPath

            if (File.Exists(templatePath))
            {
                using (StreamReader htmlBody = File.OpenText(templatePath))
                {
                    builder.HtmlBody = await htmlBody.ReadToEndAsync();
                }

                string liefStrasse = gastKunde.LiefStraße != null ? gastKunde.LiefStraße : gastKunde.RechStraße;
                string liefOrt = gastKunde.LiefOrt != null ? gastKunde.LiefOrt : gastKunde.RechOrt;
                string? liefPLZ = gastKunde.LiefPLZ != null ? gastKunde.LiefPLZ : gastKunde.RechPLZ;

                // Logo Bild in das Template einbetten
                string absolutePathLogo = Path.Combine(environment.WebRootPath, logoPath);

                //ImageLinedResource imageLined = new ImageLinedResource()
                //{
                //    ContentType = "image/png",
                //    FileName = absolutePathLogo,
                //    ContentData = File.ReadAllBytes(absolutePathLogo),                 
                //};

                //var emailLogo = builder.LinkedResources.Add(imageLined.FileName);              
                //emailLogo.ContentId = MimeUtils.GenerateMessageId();

                bestellungDaten = bestellungDaten
                    .Replace("$Anfragenummer$", string.Format($"{bestellung.Bestellnummer}"))
                    .Replace("$Anfragedatum$", string.Format($"{bestellung.BestellungssDatum}"))
                    .Replace("$Anrede$", string.Format($"{gastKunde.Anrede}"))

                    .Replace("$RVorname$", string.Format($"{gastKunde.Vorname}"))
                    .Replace("$RNachname$", string.Format($"{gastKunde.Nachname}"))
                    .Replace("$RStrasse$", string.Format($"{gastKunde.RechStraße}"))
                    .Replace("$RPLZ$", string.Format($"{gastKunde.RechPLZ}"))
                    .Replace("$ROrt$", string.Format($"{gastKunde.RechOrt}"))

                    .Replace("$LVorname$", string.Format($"{gastKunde.Vorname}"))
                    .Replace("$LNachname$", string.Format($"{gastKunde.Nachname}"))
                    .Replace("$LStrasse$", string.Format($"{liefStrasse}"))
                    .Replace("$LPLZ$", string.Format($"{liefPLZ}"))
                    .Replace("$LOrt$", string.Format($"{liefOrt}"));

                // Bilder Dynamisch in der Email Template einsetzen
                // Schritt NR.1 Absolute Pfad zusammen setzen
                // Schritt NR.2 Name des Bildes spliten, die im HTML Template in src gesetzt war => <img src="BildName" />
                // Schritt NR.3 Absolute Pfad an builder.LinkedResources hängen
                // Schritt NR.4 ContentId genrieren
                // Schritt NR.5 der cid:BildName durch genrierten ContentId ersetzen


                for (int i = 0; i < bestellung.Positionen.Count; i++)
                {
                    var item = bestellung.Positionen[i]; // /assets/images/container-Box3.png
                    //var tt = item.ProductImage.TrimStart('/'); // assets/images/container-Box3.png   // Ohne /
                    //var tt2 = item.ProductImage.Remove(0,1);  // assets/images/container-Box3.png   // Ohne /
                    //var itemBild = Path.Combine(environment.WebRootPath,tt); 

                    //ImageLinedResource imageLined2 = new ImageLinedResource()
                    //{
                    //    ContentType = "image/png",
                    //    FileName = itemBild,
                    //    ContentData = File.ReadAllBytes(itemBild),
                    //};

                    // Byte[] itemBildByts = File.ReadAllBytes(itemBild);
                    //var produktBild = builder.LinkedResources.Add(itemBild);
                    //produktBild.ContentId = MimeUtils.GenerateMessageId();

                    //// GiltNUR den Test server
                    //string imageFileName = Path.GetFileName(itemBild);
                    //string absolutePathImTestServer = Path.Combine(imagePathInTestServer,imageFileName);

                    string imageFileName = Path.GetFileName(item?.ProductImage!);
                    string einheit = string.Empty;

                    switch (item?.ArtikelName)
                    {
                        case "Karton-Box": einheit = "Karton-Box"; break;
                        case "Container-Box-Metall":
                        case "Container-Box-Holz": einheit = "Container"; break;
                        case "Einzelstücke":
                            if (item.Anzah > 1)
                            {
                                einheit = "Einzelstücke";
                            }
                            else
                            {
                                einheit = "Einzelstück";
                            }
                            break;

                        case "Umzugskarton": einheit = "Umzugskarton"; break;
                        case "Ladehilfe": einheit = "Monat"; break;
                    }

                    _temp1Pos = temp1Pos
                                //.Replace("$image$", string.Format($"cid:{produktBild.ContentId}"))
                                .Replace("$image$", string.Format($"{imagesInOutlookCloud[imageFileName]}"))
                                .Replace("$ArtikelName$", string.Format(item?.ArtikelName!))
                                .Replace("$Pries$", string.Format($"{item!.Stückpreis.ToString("C2", CultureInfo.GetCultureInfo("de-DE"))}"))
                                .Replace("$Menge$", string.Format(item.Anzah.ToString() + " " + einheit))
                                .Replace("$Total$", string.Format($"{item.Summe.ToString("C2", CultureInfo.GetCultureInfo("de-DE"))}"));


                    htmlPositionen += _temp1Pos;

                }

                builder.HtmlBody = builder.HtmlBody
                   .Replace("$logo$", string.Format($"{imagesInOutlookCloud["room-it-logo.png"]}"))
                   .Replace("$BestellungDaten$", string.Format($"{bestellungDaten}"))
                   .Replace("$Positionen$", string.Format($"{htmlPositionen}"))
                   //.Replace("$TotalOhneMwSt$", string.Format($"{(bestellung.GesamtBestellungsBetrag * (1 - 0.19)).ToString("C2", CultureInfo.GetCultureInfo("de-DE"))}"))
                  // .Replace("$MwSt$", string.Format($"{19}"))
                   .Replace("$TotalMitMwSt$", string.Format($"{bestellung.GesamtBestellungsBetrag.ToString("C2", CultureInfo.GetCultureInfo("de-DE"))}"));

            }
            else
            {
                builder.HtmlBody = "<div><h1>ERRRRRRRROOOORRR</h1></div>";
            }



            return builder;
        }

        public static async Task<BodyBuilder> TemplateKundeCreateAsyncBilderWithCid(IWebHostEnvironment environment, GastKunde gastKunde, Bestellung bestellung)
        {
            string _temp1Pos = string.Empty;
            string htmlPositionen = string.Empty;
            var templatePath = "wwwroot/EmailTemplates/Temp1.html";
            string logoPath = "assets/images/room-it-logo.png";
            string imagePathInTestServer = "http://dehze01-wsv828:82/assets/images/"; // diese Adresse soll ersetzt werden wo Seite gehostet werden soll und so kann man auf die Bilder für Email zugreifen

           

            // string Temp1
            string temp1Pos = "<div style=\"border-collapse: collapse;display: table;width: 100%;background-color:#FFFFFF;\">\r\n<div class=\"col num3\" style=\"max-width: 320px; min-width: 162px; display: table-cell; vertical-align: top; width: 162px;\">\r\n <div style=\"width:100% !important;\">\r\n" +
                "<!--[if (!mso)&(!IE)]><!-->\r\n<div style=\"border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:5px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;\">\r\n<!--<![endif]-->\r\n" +
                "<div align=\"center\" class=\"img-container center fixedwidth\" style=\"padding-right: 0px;padding-left: 0px;\">\r\n<img align=\"center\" alt=\"Image\" border=\"0\" class=\"center fixedwidth\" src=\"$image$\" style=\"text-decoration: none; -ms-interpolation-mode: bicubic; border: 0; height: auto; width: 100%; max-width: 130px; display: block;\" title=\"Image\" width=\"130\" />\r\n" +
                "</div>\r\n</div>\r\n</div>\r\n</div>\r\n<div class=\"col num3\" style=\"max-width: 320px; min-width: 162px; display: table-cell; vertical-align: top; width: 161px;\">\r\n<div style=\"width:100% !important;\">\r\n" +
                "<!--[if (!mso)&(!IE)]><!-->\r\n<div style=\"border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:1px dotted #E8E8E8; padding-top:30px; padding-bottom:35px; padding-right: 0px; padding-left: 0px;\">\r\n" +
                "<div style=\"color:#555555;font-family:'Lato', Tahoma, Verdana, Segoe, sans-serif;line-height:120%;padding-top:10px;padding-right:10px;padding-bottom:5px;padding-left:0px;\">\r\n<div style=\"line-height: 14px; font-family: 'Lato', Tahoma, Verdana, Segoe, sans-serif; font-size: 12px; color: #555555;\">\r\n" +
                "<p style=\"line-height: 14px; text-align: left; font-size: 12px; margin: 0;\"><span style=\"color: #2190e3;\"><span style=\"font-size: 16px; line-height: 19px;\"><strong>$ArtikelName$</strong></span></span></p>\r\n</div>\r\n</div>\r\n" +
                "<div style=\"color:#555555;font-family:'Lato', Tahoma, Verdana, Segoe, sans-serif;line-height:120%;padding-top:0px;padding-right:10px;padding-bottom:10px;padding-left:0px;\">\r\n<p style=\"font-size: 12px; line-height: 14px; color: #555555; font-family: 'Lato', Tahoma, Verdana, Segoe, sans-serif; margin: 0;\"> $Pries$ </p>\r\n" +
                "</div>\r\n</div>\r\n<!--<![endif]-->\r\n</div>\r\n</div>\r\n<div class=\"col num3\" style=\"max-width: 320px; min-width: 162px; display: table-cell; vertical-align: top; width: 161px;\">\r\n" +
                "<div style=\"width:100% !important;\">\r\n<!--[if (!mso)&(!IE)]><!-->\r\n<div style=\"border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:1px dotted #E8E8E8; padding-top:55px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;\">\r\n" +
                "<div style=\"color:#555555;font-family:'Lato', Tahoma, Verdana, Segoe, sans-serif;line-height:120%;padding-top:0px;padding-right:10px;padding-bottom:10px;padding-left:10px;\">\r\n<div style=\"font-size: 12px; line-height: 14px; font-family: 'Lato', Tahoma, Verdana, Segoe, sans-serif; color: #555555;\">\r\n" +
                "<p style=\"font-size: 14px; line-height: 24px; text-align: center; margin: 0;\"><span style=\"font-size: 20px;\"><strong>$Menge$</strong></span></p>\r\n</div>\r\n </div>\r\n <!--[if mso]></td></tr></table><![endif]-->\r\n" +
                "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"divider\" role=\"presentation\" style=\"table-layout: fixed; vertical-align: top; border-spacing: 0; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt; min-width: 100%; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;\" valign=\"top\" width=\"100%\">\r\n" +
                "<tbody>\r\n<tr style=\"vertical-align: top;\" valign=\"top\">\r\n<td class=\"divider_inner\" style=\"word-break: break-word; vertical-align: top; min-width: 100%; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%; padding-top: 10px; padding-right: 10px; padding-bottom: 10px; padding-left: 10px;\" valign=\"top\">\r\n   " +
                "<table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"divider_content\" height=\"30\" role=\"presentation\" style=\"table-layout: fixed; vertical-align: top; border-spacing: 0; border-collapse: collapse; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%; border-top: 0px solid transparent; height: 30px;\" valign=\"top\" width=\"100%\">\r\n     " +
                "<tbody>\r\n<tr style=\"vertical-align: top;\" valign=\"top\">\r\n<td height=\"30\" style=\"word-break: break-word; vertical-align: top; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;\" valign=\"top\"><span></span></td>\r\n" +
                "</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n <!--[if (!mso)&(!IE)]><!-->\r\n" +
                "</div>\r\n <!--<![endif]-->\r\n</div>\r\n</div>\r\n<div class=\"col num3\" style=\"max-width: 320px; min-width: 162px; display: table-cell; vertical-align: top; width: 162px;\">\r\n<div style=\"width:100% !important;\">\r\n<!--[if (!mso)&(!IE)]><!-->\r\n" +
                "<div style=\"border-top:0px solid transparent; border-left:0px solid transparent; border-bottom:0px solid transparent; border-right:0px solid transparent; padding-top:55px; padding-bottom:5px; padding-right: 0px; padding-left: 0px;\">\r\n<div style=\"color:#555555;font-family:'Lato', Tahoma, Verdana, Segoe, sans-serif;line-height:120%;padding-top:0px;padding-right:15px;padding-bottom:0px;padding-left:0px;\">\r\n                                                    <div style=\"line-height: 14px; font-size: 12px; font-family: 'Lato', Tahoma, Verdana, Segoe, sans-serif; color: #555555;\">\r\n                                                        " +
                "<p style=\"line-height: 24px; text-align: center; font-size: 12px; margin: 0;\"><span style=\"font-size: 20px;\"><span style=\"line-height: 24px; font-size: 20px;\"><strong>$Total$</strong></span></span></p>\r\n</div>\r\n</div>\r\n<!--[if mso]></td></tr></table><![endif]-->\r\n<!--[if (!mso)&(!IE)]><!-->\r\n</div>\r\n<!--<![endif]-->\r\n</div>\r\n</div>\r\n  " +
                "<!--[if (mso)|(IE)]></td></tr></table><![endif]-->\r\n<!--[if (mso)|(IE)]></td></tr></table></td></tr></table><![endif]-->\r\n</div>";

            string bestellungDaten = "<div style=\"color:#787878;font-family:'Lato', Tahoma, Verdana, Segoe, sans-serif;line-height:150%;padding-top:0px;padding-right:10px;padding-bottom:30px;padding-left:10px;\">\r\n<div style=\"font-size: 12px; line-height: 18px; font-family: 'Lato', Tahoma, Verdana, Segoe, sans-serif; color: #787878;\">\r\n" +
                "<h3>Anfragenummer:&nbsp;&nbsp;<span style=\"color:gray;\">$Anfragenummer$ </span></h3>\r\n<h3>Anfragedatum:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=\"color:gray;\">$Anfragedatum$ </span></h3>\r\n<p style=\"margin-top:0;margin-bottom:0;\"><span style=\"line-height: 40px; font-size: 20px;\">RECHNUNGSADRESSE</span></p>\r\n " +
                "<p style=\"margin-top:0;margin-bottom:0;\"><span style=\" font-size: 14px;\">$Anrede$</span></p>\r\n<p style=\"margin-top:0;margin-bottom:0;\"><span style=\" font-size: 14px;\">$RVorname$ </span> &nbsp;&nbsp;  <span>$RNachname$</span></p>\r\n<p style=\"margin-top:0;margin-bottom:0;\"><span style=\" font-size: 14px;\">$RStrasse$</span></p>\r\n" +
                "<p style=\"margin-top:0;margin-bottom:0;\"><span style=\" font-size: 14px;\">$ROrt$</span> &nbsp;&nbsp;  <span>$RPLZ$</span></p>\r\n</div>\r\n<br />\r\n<div style=\"font-size: 12px; line-height: 18px; font-family: 'Lato', Tahoma, Verdana, Segoe, sans-serif; color: #787878;\">\r\n" +
                "<p style=\"margin-top:0;margin-bottom:0;\"><span style=\"line-height: 40px; font-size: 20px;\">LIEFERADRESSE </span></p>\r\n<p style=\"margin-top:0;margin-bottom:0;\"><span style=\" font-size: 14px;\">$Anrede$</span></p>\r\n<p style=\"margin-top:0;margin-bottom:0;\"><span style=\" font-size: 14px;\">$LVorname$ </span> &nbsp;&nbsp;  <span>$LNachname$</span></p>\r\n" +
                "<p style=\"margin-top:0;margin-bottom:0;\"><span style=\" font-size: 14px;\">$LStrasse$</span></p>\r\n<p style=\"margin-top:0;margin-bottom:0;\"><span style=\" font-size: 14px;\">$LOrt$</span> &nbsp;&nbsp;<span>$LPLZ$</span></p>\r\n</div>\r\n</div>";

            var builder = new BodyBuilder();

            templatePath = Path.Combine(environment.ContentRootPath, templatePath); // für templatePath soll ContentRootPath
            

            if (File.Exists(templatePath))
            {
                using (StreamReader htmlBody = File.OpenText(templatePath))
                {
                    builder.HtmlBody = await htmlBody.ReadToEndAsync();
                }

                string liefStrasse = gastKunde.LiefStraße != null ? gastKunde.LiefStraße : gastKunde.RechStraße;
                string liefOrt = gastKunde.LiefOrt != null ? gastKunde.LiefOrt : gastKunde.RechOrt;
                string? liefPLZ = gastKunde.LiefPLZ != null ? gastKunde.LiefPLZ : gastKunde.RechPLZ;
                

                bestellungDaten = bestellungDaten
                    .Replace("$Anfragenummer$", string.Format($"{bestellung.Bestellnummer}"))
                    .Replace("$Anfragedatum$", string.Format($"{bestellung.BestellungssDatum}"))
                    .Replace("$Anrede$", string.Format($"{gastKunde.Anrede}"))

                    .Replace("$RVorname$", string.Format($"{gastKunde.Vorname}"))
                    .Replace("$RNachname$", string.Format($"{gastKunde.Nachname}"))
                    .Replace("$RStrasse$", string.Format($"{gastKunde.RechStraße}"))
                    .Replace("$RPLZ$", string.Format($"{gastKunde.RechPLZ}"))
                    .Replace("$ROrt$", string.Format($"{gastKunde.RechOrt}"))

                    .Replace("$LVorname$", string.Format($"{gastKunde.Vorname}"))
                    .Replace("$LNachname$", string.Format($"{gastKunde.Nachname}"))
                    .Replace("$LStrasse$", string.Format($"{liefStrasse}"))
                    .Replace("$LPLZ$", string.Format($"{liefPLZ}"))
                    .Replace("$LOrt$", string.Format($"{liefOrt}"));

                // Bilder Dynamisch in der Email Template einsetzen
                // Schritt NR.1 Absolute Pfad zusammen setzen
                // Schritt NR.2 Name des Bildes spliten, die im HTML Template in src gesetzt war => <img src="BildName" />
                // Schritt NR.3 Absolute Pfad an builder.LinkedResources hängen
                // Schritt NR.4 ContentId genrieren
                // Schritt NR.5 der cid:BildName durch genrierten ContentId ersetzen


                for (int i = 0; i < bestellung.Positionen.Count; i++)
                {
                    var item = bestellung.Positionen[i];                              // /assets/images/container-Box3.png                    
                    //var tt2 = item.ProductImage.Remove(0,1);                       // assets/images/container-Box3.png   // Ohne /
                   
                    var imageOhne = item.ProductImage.TrimStart('/');                // assets/images/container-Box3.png   // Ohne /               
                    
                    var itemBild = Path.Combine(environment.WebRootPath,imageOhne);
                    

               

                    ImageLinedResource imageLined1 = new ImageLinedResource()
                    {
                        ContentType = "image/png",
                        FileName = itemBild,
                       // ContentData = File.ReadAllBytes(itemBild),
                    };

                    var produktBild = builder.LinkedResources.Add(imageLined1.FileName);
                    produktBild.ContentId = MimeUtils.GenerateMessageId();

                    //// GiltNUR den Test server
                    //string imageFileName = Path.GetFileName(itemBild);
                    //string absolutePathImTestServer = Path.Combine(imagePathInTestServer,imageFileName);

                    
                    string einheit = string.Empty;

                    switch (item.ArtikelName)
                    {
                        case "Karton-Box": einheit = "Karton-Box"; break;
                        case "Container-Box": einheit = "Container"; break;
                        case "Einzelstücke":
                            if (item.Anzah > 1)
                            {
                                einheit = "Einzelstücke";
                            }
                            else
                            {
                                einheit = "Einzelstück";
                            }
                            break;

                        case "Umzugskarton": einheit = "Umzugskarton"; break;
                        case "Ladehilfe": einheit = "Monat"; break;
                    }

                    _temp1Pos = temp1Pos
                                .Replace("$image$", string.Format($"cid:{produktBild.ContentId}"))
                                .Replace("$ArtikelName$", string.Format(item?.ArtikelName!))
                                .Replace("$Pries$", string.Format($"{item!.Stückpreis.ToString("C2", CultureInfo.GetCultureInfo("de-DE"))}"))
                                .Replace("$Menge$", string.Format(item.Anzah.ToString() + " " + einheit))
                                .Replace("$Total$", string.Format($"{item.Summe.ToString("C2", CultureInfo.GetCultureInfo("de-DE"))}"));


                    htmlPositionen += _temp1Pos;

                }

                // Logo Bild in das Template einbetten
  
                string absolutePathLogo = Path.Combine(environment.WebRootPath, logoPath);

                ImageLinedResource imageLined = new ImageLinedResource()
                {
                    ContentType = "image/png",
                    FileName = absolutePathLogo,
                   // ContentData = File.ReadAllBytes(absolutePathLogo),
                };

                var emailLogo = builder.LinkedResources.Add(imageLined.FileName);
                emailLogo.ContentId = MimeUtils.GenerateMessageId();

                builder.HtmlBody = builder.HtmlBody
                   .Replace("$logo$", string.Format($"cid:{emailLogo.ContentId}"))
                   .Replace("$BestellungDaten$", string.Format($"{bestellungDaten}"))
                   .Replace("$Positionen$", string.Format($"{htmlPositionen}"))
                   //.Replace("$TotalOhneMwSt$", string.Format($"{(bestellung.GesamtBestellungsBetrag * (1 - 0.19)).ToString("C2", CultureInfo.GetCultureInfo("de-DE"))}"))
                   // .Replace("$MwSt$", string.Format($"{19}"))
                   .Replace("$TotalMitMwSt$", string.Format($"{bestellung.GesamtBestellungsBetrag.ToString("C2", CultureInfo.GetCultureInfo("de-DE"))}"));

            }
            else
            {
                builder.HtmlBody = "<div><h1>ERRRRRRRROOOORRR</h1></div>";
            }



            return builder;
        }

    }
}
