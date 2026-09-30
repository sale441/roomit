// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// funktion für das Alte direkte eingabe ohne Warenkorb Button um die die Werten in Allen Produkten zu aktulliseren
function shoppingCartToSession() {

    let euro = Intl.NumberFormat('de-DE', {
        style: 'currency',
        currency: 'EUR',
    });


    var data = sessionStorage.getItem('warenkorbObj');
    var warenkorbObj = JSON.parse(data);
  
    if (warenkorbObj != null) {

        for (var key in warenkorbObj) {

            var value = warenkorbObj[key];
    
            if (key == "WarenkorbItemsListe") {
                
                // das muss noch behandelt werden => wenn der Anzahl vorher 1 oder 2 und dann macht man minus minus und dann verschwendet das produkt aus der Liste, es bleibt aber noch drin im Broweser , es soll die alle auf 0 geseztt werden 
                for (var i = 0; i < warenkorbObj["WarenkorbItemsListe"].length; i++) {

                    var itemObj = warenkorbObj["WarenkorbItemsListe"][i];

                    const domObj = $('#' + itemObj.ProduktName);
                    const domObjCart = $('#' + itemObj.ProduktName + '_Cart');     
                    
                    // Hier wurde mit JavaScript geprüft und nicht mit Jquery, da es bessere und saubare Ergebnise holt
                    /*Produkt Site*/
                    if (document.getElementById(itemObj.ProduktName) !== null) {                        
                        $(domObj).find('.product-counter').text(itemObj.Quantity);
                        $(domObj).find('.result').text(euro.format(itemObj.Total));
                        $(domObj).find('.minus').css('display', 'flex');

                    }
                    // Cart Site
                    else if (document.getElementById(itemObj.ProduktName + '_Cart') !== null) {                    
                        $(domObjCart).find('.quantity').text(itemObj.Quantity);
                        $(domObjCart).find('.unitPrice span:first').text(euro.format(itemObj.UnitPrice));
                        $(domObjCart).find('.total span:first').text(euro.format(itemObj.Total));
                        var tt = $(domObjCart).parents('#cart').find('.totalAmount')
                       
                        $(domObjCart).parents('#cart').find('.article-counter').text(warenkorbObj['Quantity']);
                        $(domObjCart).parents('#cart').find('.totalAmount').text(euro.format(itemObj.Total));
                    }

                }
               
            }
            $('#warenkorbCountity').text(warenkorbObj['Quantity']);
            $('#warenkorbAmount').text(euro.format(warenkorbObj['TotalAmount']));
          
        }
    }
    else {
        var cardItems = $('.cardItem');
        $('#warenkorbCountity').text('');
        $('#warenkorbAmount').text(euro.format(0));
        $('#cart').find('.article-counter').text('')
        $('#cart').find('.totalAmount').text('')

        for (var i = 0; i < cardItems.length; i++) {
            $(cardItems[i]).find('.product-counter').text(0);
            $(cardItems[i]).find('.result').text(0);
            $(cardItems[i]).find('.minus').css('display', 'none');
        }
        
    }
    
}

function GetShoppingCart() {

    let euro = Intl.NumberFormat('de-DE', {
        style: 'currency',
        currency: 'EUR',

    });

    $.post("/Shop/GetShoppingCart", function (data) {
  
        if (data !== null) {
            sessionStorage.setItem('warenkorbObj', data)

        }
        else {

            sessionStorage.clear();
            $('#warenkorbCountity').text(0);
            $('#warenkorbAmount').text(euro.format(0));
            //debugger
            //var tt = sessionStorage.length;
            //var t = localStorage.length;
            
            //shoppingCartToSession();
        }

    })



    var data = sessionStorage.getItem('warenkorbObj');
    var warenkorbObj = JSON.parse(data);

    if (warenkorbObj !== null) {

        $('#warenkorbCountity').text(warenkorbObj['Quantity']);
        $('#warenkorbAmount').text(euro.format(warenkorbObj['TotalAmount']));

    }
    else {

        $('#warenkorbCountity').text(0);
        $('#warenkorbAmount').text(euro.format(0));
    }

    //#region window.location.pathname
    //if (window.location.pathname == "/Shop/Produkte") {
    //    $.post("/Shop/GetShoppingCart", function (data) {
    //        if (data !== null) {
    //            sessionStorage.setItem('warenkorbObj', data)
    //            shoppingCartToSession();
    //        }
    //        else {
    //            sessionStorage.clear();
    //            shoppingCartToSession();
    //        }
    //    })

    //}
    //if (window.location.pathname == "/Shop/ShoppingCart") {

    //    $.post("/Shop/GetShoppingCart", function (data) {
   
    //        if (data !== null) {
    //            sessionStorage.setItem('warenkorbObj', data)
    //            shoppingCartToSession();
    //        }
    //        else {
    //            sessionStorage.clear();
    //            shoppingCartToSession();
    //        }
    //    })

    //}
}


function korpIdit(_this, produktname) {

    var parent = $(_this).parents('.korp');
    //var previousElement = _this.parentNode.previousElementSibling;


    /*var unitPrice = _this.parentNode.nextElementSibling.children[0].children[0].innerText;*/  /*gleich Erg mit Javascript*/
    /*var totalPrice = _this.parentNode.nextElementSibling.nextElementSibling.children[0].innerText;*/  /*gleich Erg mit Javascript*/

    //var unitPrice = $(parent).parent('.korp').find('.unitPrice span:first').text()
    //var totalPrice = $(parent).parent('.korp').find('.total span:first').text()
    //var quantity = Number.parseInt($(parent).find('.quantity').val());

 
    var unitPrice = $(parent).find('.unitPrice span').text()
    var totalPrice = $(parent).find('.total span').text()
    
    unitPrice = Number.parseInt(unitPrice);
    totalPrice = Number.parseInt(totalPrice);
    var quantity = Number.parseInt($(parent).children().find('.quantity').text());
    var postleitzahl = $('#cart .zipcodeResult span')[0].innerText;
    var stadtname = $('#cart .zipcodeResult span')[1].innerText;

   

    if (_this.className.includes('plus')) {
       
        let _flag = 1;
        
        if (produktname == 'Karton-Box') {
            if (quantity == 0) {
                quantity += 3;
            }
            else {
                quantity += 1;
            }
        }
        else {
            quantity += 1;
            
        }

        let result = quantity * unitPrice;
  

        $.post("/Shop/_ShoppingCart", { productName: produktname, quantity: quantity, _articlePrice: unitPrice, _articlePriceAmount: result, postleitzahl: postleitzahl, stadtname, stadtname, flag: _flag }, function (data) {
            if (data != null) {

                sessionStorage.setItem('warenkorbObj', data)
                warenkorbUpdate(_this);

            }
            else {
                sessionStorage.clear();
            }
        })
    }
    else if (_this.className.includes('_minus')) {
     
        let _flag = 0;

        if (produktname == 'Karton-Box') {
            if (quantity == 3) {
                quantity -= 3;
            }
            else {
                quantity -= 1;
            }
        }
        else {
            quantity -= 1;
        }

        let result = quantity * unitPrice;

     
        if (quantity == 0) {
            $(_this).remove();
            $(parent).remove();
            $('.' + produktname).remove();
        }

        $.post("/Shop/_ShoppingCart", { productName: produktname, quantity: quantity, _articlePrice: unitPrice, _articlePriceAmount: result, postleitzahl: postleitzahl, stadtname, stadtname, flag: _flag }, function (data) {
            if (data != null) { 

                sessionStorage.setItem('warenkorbObj', data)
                warenkorbUpdate(_this);

            }
            else {
                sessionStorage.clear();
                warenkorbUpdate();
            }
        })
    }
    else if (_this.className.includes('delete')) {

        $(parentNode).remove();

        let _flag = 0;
        let result = '0';
        unitPrice = '0'
        quantity = 0;

        $.post("/Shop/_ShoppingCart", { productName: produktname, quantity: quantity, _articlePrice: unitPrice, _articlePriceAmount: result, postleitzahl: postleitzahl, stadtname, stadtname, flag: _flag }, function (data) {
     
            if (data != 'null') {
                
                sessionStorage.setItem('warenkorbObj', data)

                warenkorbUpdate();

            }
            else {
                sessionStorage.clear();
                warenkorbUpdate();
            }
        })

    }
    
}

// *** es muss hier auch mit Object gearbeitet und nicht mit schleifen damit man stichartig entfernen kann sucht man das Object und entfernt man ihn ohne mit Schleife das ganze DOM durchlaufen zu müßen
function warenkorbUpdate(_this) {

    //var parent = $(_this).parents('.korp');

    //var unitPrice = $(parent).children().find('.unitPrice span:first');
    //$(unitPrice).text()

    //var totalPrice = $(parent).children().find('.total span:first').text()
    //totalPrice = Number.parseInt(totalPrice);
    //var quantity = Number.parseInt($(parent).children().find('.quantity').text());
    //var postleitzahl = $('#cart .zipcodeResult span')[0].innerText;
    //var stadtname = $('#cart .zipcodeResult span')[1].innerText;

    let euro = Intl.NumberFormat('de-DE', {
        style: 'currency',
        currency: 'EUR',
    });
    var data = sessionStorage.getItem('warenkorbObj');
    var warenkorbObj = JSON.parse(data);
    
    if (warenkorbObj != null) {

        for (var key in warenkorbObj) {

            var value = warenkorbObj[key];
            
            if (key == "WarenkorbItemsListe") {

                // das muss noch behandelt werden => wenn der Anzahl vorher 1 oder 2 und dann macht man minus minus und dann verschwendet das produkt aus der Liste, es bleibt aber noch drin im Broweser , es soll die alle auf 0 geseztt werden 
                for (var i = 0; i < warenkorbObj["WarenkorbItemsListe"].length; i++) {

                    var itemObj = warenkorbObj["WarenkorbItemsListe"][i];

                    const domObjCart = $('#' + itemObj.ProduktName + '_Cart');

                    // Hier wurde mit JavaScript geprüft und nicht mit Jquery, da es bessere und saubare Ergebnise holt                  
                    // Cart-Site
                    if (document.getElementById(itemObj.ProduktName + '_Cart') !== null) {

                        $(domObjCart).find('.quantity').text(itemObj.Quantity);
                        $(domObjCart).find('.unitPrice span:first').text(euro.format(itemObj.UnitPrice));
                        $(domObjCart).find('.total span:first').text(euro.format(itemObj.Total));
                        var tt = $(domObjCart).parents('#cart').find('.totalAmount')

                        var objInGrayZone = $('.' + itemObj.ProduktName)
                        $(objInGrayZone).children('.grayCartList-1').find('span.article-counter').text(itemObj.Quantity)
                        $(objInGrayZone).children('.grayCartList-2').text(euro.format(itemObj.Total));
                 

                        //$(domObjCart).parents('#cart').find('.article-counter').text(warenkorbObj['Quantity']);
                        $(domObjCart).parents('#cart').find('#totalAmount-x').children('.totalAmount-1').text(euro.format(warenkorbObj.TotalAmount));
                    }
                }
            }
            $('#warenkorbCountity').text(warenkorbObj['Quantity']);
            $('#warenkorbAmount').text(euro.format(warenkorbObj['TotalAmount']));
        }
    }
    else {
        var cardItems = $('.cardItem');
        $('#warenkorbCountity').text('');
        $('#warenkorbAmount').text(euro.format(0));
        $('#cart').find('.article-counter').text('')
        $('#cart').find('.totalAmount-1').text(euro.format(0))

        for (var i = 0; i < cardItems.length; i++) {
            $(cardItems[i]).find('.product-counter').text(0);
            $(cardItems[i]).find('.result').text(0);
            $(cardItems[i]).find('.minus').css('display', 'none');
        }
    }
}

function shopSiteUpdate(_this) {

    let euro = Intl.NumberFormat('de-DE', {
        style: 'currency',
        currency: 'EUR',

    });

    var cardItem = $(_this);

    var data = sessionStorage.getItem('warenkorbObj');
    var warenkorbObj = JSON.parse(data);

    if (warenkorbObj != null) {

        $('#warenkorbCountity').text(warenkorbObj['Quantity']);
        $('#warenkorbAmount').text(euro.format(warenkorbObj['TotalAmount']));

        $(cardItem).find('.product-counter').text(0);
        $(cardItem).find('.result').text(euro.format(0));
        $(cardItem).find('.minus').css('display', 'none');
        
    }
    else {

        // Alle cardItems
        var cardItems = $('.shop .cardItem')
       
     
        //$('#warenkorbCountity').text('');
        //$('#warenkorbAmount').text(euro.format(0));
        //$('#cart').find('.article-counter').text('')
        //$('#cart').find('.totalAmount').text('')

        for (var i = 0; i < cardItems.length; i++) {
            $(cardItems[i]).find('.product-counter').text(0);
            $(cardItems[i]).find('.result').text(euro.format(0));
            $(cardItems[i]).find('.minus').css('display', 'none');
        }

    }

}

function detailsSiteUpdate(_this) {

    let euro = Intl.NumberFormat('de-DE', {
        style: 'currency',
        currency: 'EUR',

    });

    var cardItem = $(_this);

    var data = sessionStorage.getItem('warenkorbObj');
    var warenkorbObj = JSON.parse(data);

    if (warenkorbObj != null) {

        $('#warenkorbCountity').text(warenkorbObj['Quantity']);
        $('#warenkorbAmount').text(euro.format(warenkorbObj['TotalAmount']));

        $(cardItem).children().find('.details-product-counter').text(0);
        $(cardItem).children().find('.details-result').text(euro.format(0));
        $(cardItem).children().find('.details-minus').css('display', 'none');        

    }
    //else {

    //    // Alle cardItems
    //    var cardItems = $('.shop .cardItem')


    //    //$('#warenkorbCountity').text('');
    //    //$('#warenkorbAmount').text(euro.format(0));
    //    //$('#cart').find('.article-counter').text('')
    //    //$('#cart').find('.totalAmount').text('')

    //    for (var i = 0; i < cardItems.length; i++) {
    //        $(cardItems[i]).find('.product-counter').text(0);
    //        $(cardItems[i]).find('.result').text(euro.format(0));
    //        $(cardItems[i]).find('.minus').css('display', 'none');
    //    }

    //}

}


(function () {

    ////******* für mein Accordion
    //$('.accordion-item').click(function () {
    //    //var items = $('.accordion-item.minus');
    //    //$(items).find('i').toggleClass('fas fa-minus fas fa-plus');
    //    //$(items).toggleClass('minus plus');
    //    //$(items).next('.accordion-item-body.noCollapse').slideUp('fast', 'linear').toggleClass('noCollapse okCollapse');


    //    $(this).find('i').toggleClass('fas fa-plus fas fa-minus');
    //    //$(this).toggleClass('plus minus');
    //    //$(this).next('.accordion-item-body').slideToggle('400','swing').toggleClass('okCollapse noCollapse');


    //});


    //********** für Bootsrap Accordion Anpassungen in JS Teil


    $('.accordion-item').click(function () {

        $('.collapse').on('show.bs.collapse', function () {
            
            //$(this).prev('.accordion-header').find('i').removeClass('fa-minus').addClass('fa-plus');
            $('.collapse.show').each(function () {
                $(this).prev('.accordion-header').find('i').removeClass('fa-minus').addClass('fa-plus');
            })
        }).on('hide.bs.collapse', function () {

            //$(this).prev('.accordion-header').find('i').removeClass('fa-minus').addClass('fa-plus');
            $('.collapse.show').each(function () {
                $(this).prev('.accordion-header').find('i').removeClass('fa-minus').addClass('fa-plus');
            })
        })

        $(this).find('i').toggleClass('fas fa-plus fas fa-minus');

        if ($(this).find('.accordion-button').hasClass('collapsed')) {          
            $(this).find('i').addClass('fa-plus')
            $(this).find('i').removeClass('fa-minus')
        }
        else {        
            $(this).find('i').removeClass('fa-plus')
            $(this).find('i').addClass('fa-minus')
                       
        } 
    });


    //const myCarouselElement = document.querySelector('#demo')
    //const carousel = new bootstrap.Carousel(myCarouselElement, {
    //    interval: 2000,
    //    wrap: true
    //})

    let euro = Intl.NumberFormat('de-DE', {
        style: 'currency',
        currency: 'EUR',
    });
    
    //#region wehn event Brwoser closed
    //window.onbeforeunload = function (event) {     
    //    var message = 'Important: Please click on \'Save\' button to leave this page.';
    //    if (typeof event == 'undefined') {
    //        event = window.event;
    //    }
    //    if (event) {
    //        event.returnValue = message;
    //    }
    //    return message;
    //};

   
    //$(function () {
    //    $(a).not('#lnkLogOut').click(function () {
    //        window.onbeforeunload = null;
    //    });
    //    $(".btn").click(function () {
    //        window.onbeforeunload = null;
    //    });
    //});

    //if (window.location.pathname == "/Shop/Produkte") {
    //    debugger

    //}
    
    //window.onbeforeunload = function (event) {

    //    //if (event.currentTarget.performance.navigation.type === PerformanceNavigation.TYPE_BACK_FORWARD) {
    //    //    var t = window.location;
    //    //    var oo = document.location;
    //    //    debugger
    //    //}


    //    if (window.location.pathname == "/Shop/Produkte") {
    //        debugger
    //        $.post("/Shop/Produkte", function (data) {

    //            //if (data !== null) {
    //            //    sessionStorage.setItem('warenkorbObj', data)

    //            //    shoppingCartToSession();
    //            //}
    //            //else {
    //            //    sessionStorage.clear();
    //            //}
    //        })

    //    }

    //};

    GetShoppingCart();

    var shoppingCartArray = [];
    
    function objectAddToSession(newKartonBox,nameOfObj) {
        // if Exist
        if (sessionStorage.getItem('shoppingCartArray') != null) {
            // convert from session JSON TO Object
            shoppingCartArray = JSON.parse(sessionStorage.getItem('shoppingCartArray'));
            // search to ProduktName == "Karton-Box"
            var kartonBox = shoppingCartArray.find(obj => obj.ProduktName == nameOfObj);
            // then if ProduktName == "Karton-Box", then Only add to Array, Else manipuliere the Object
            if (kartonBox == null) {
                shoppingCartArray.push(newKartonBox);
            }
            else {
                // with forEach Nur so auf diese Art mit forEach sonst nicht wird das Objekt manipuliert 
                shoppingCartArray.forEach((item, index, self) => {
                    if (item.ProduktName == nameOfObj) {
                        self[index] = newKartonBox
                    }
                });
                // oder so mit map aber halt wird eine neue Array kopiert.
                //shoppingCartArray =  shoppingCartArray.map((cart) => {
                //    if (cart.ProduktName == "Karton-Box") {
                //        return cart = kartonBox
                //       }
                //   });

            }
        }
        // if the Session not exist, add it in the Array
        else {
            shoppingCartArray.push(newKartonBox);
        }

        // save im Storage
        sessionStorage.setItem('shoppingCartArray', JSON.stringify(shoppingCartArray))
    }

    function objectAddToArray(newKartonBox) {
        // if Exist
        if (shoppingCartArray.length > 0) {
            // search to ProduktName == "Karton-Box"
            var kartonBox = shoppingCartArray.find(obj => obj.ProduktName == "Karton-Box");
            // then if ProduktName == "Karton-Box", then Only add to Array, Else manipuliere the Object
            if (kartonBox == null) {
                shoppingCartArray.push(newKartonBox);
            }
            else {
                // with forEach Nur so auf diese Art mit forEach sonst nicht wird das Objekt manipuliert 
                shoppingCartArray.forEach((item, index, self) => {
                    if (item.ProduktName == "Karton-Box") {
                        self[index] = newKartonBox
                    }
                });

                // oder so mit map aber halt wird eine neue Array kopiert.
                //shoppingCartArray =  shoppingCartArray.map((cart) => {
                //    if (cart.ProduktName == "Karton-Box") {
                //        return cart = kartonBox
                //       }
                //   });

            }
        }
        // if the Session not exist, add it in the Array
        else {
            shoppingCartArray.push(newKartonBox);
        }
       
    }

//#region string lieferAdresse1

    let lieferAdresse1 = '<div id="lieferAdresse1" class="adreseChild" style="margin-top:30px;">' +
        '<h4 style="color:#61b300;margin-bottom:30px;">Deine Lieferadresse</h4>' +
        '<div class="form-floating mb-3 col-auto">' +
        '<input type="text" asp-for="Input.LieferAdresse.Straße" class="form-control zipcode-field" id="floatingInput" placeholder="Straße und Nr.*" required >' +
        '<label asp-for="Input.LieferAdresse.Straße">Straße und Nr.*</label>' +
        '<span asp-validation-for="Input.LieferAdresse.Straße" class="text-danger"></span>' +
        '</div>' +
        '<div class="form-floating mb-3 col-auto">' +
        '<input type="text" asp-for="Input.LieferAdresse.Ort" class="form-control zipcode-field" placeholder="Ort*" required >' +
        '<label asp-for="Input.LieferAdresse.Ort">Ort*</label>' +
        '<span asp-validation-for="Input.LieferAdresse.Ort" class="text-danger"></span>' +
        '</div>' +
        '<div class="form-floating mb-3 col-auto">' +
        '<input type="text" asp-for="Input.LieferAdresse.PLZ" class="form-control zipcode-field" id="floatingInput" placeholder="PLZ*" required >' +
        '<label asp-for="Input.LieferAdresse.PLZ">PLZ*</label>' +
        '<span asp-validation-for="Input.LieferAdresse.PLZ" class="text-danger"></span>' +
        '</div>' +
        '<div class="form-group">' +
        '<select class="form-control">' +
        '<option class="hidden" selected disabled>Wählen Sie Ihres Land</option>' +
        '<option>Deutschland</option>' +
        '</select>' +
        '</div>' +
        '</div>';    

//#endregion

//#region string lieferAdresse2
    let lieferAdresse2 = '<div id="lieferAdresse2" class="adreseChild" style="margin-top:30px;">' +
        '<h4 style="color:#61b300;margin-bottom:30px;">Deine Lieferadresse</h4>' +
        '<div class="form-floating mb-3 col-auto">' +
        '<input type="text" asp-for="LieferAdresse.Straße" class="form-control zipcode-field" id="floatingInput" placeholder="Straße und Nr.*" required >' +
        '<label asp-for="LieferAdresse.Straße">Straße und Nr.*</label>' +
        '<span asp-validation-for="LieferAdresse.Straße" class="text-danger"></span>' +
        '</div>' +
        '<div class="form-floating mb-3 col-auto">' +
        '<input type="text" asp-for="LieferAdresse.Ort" class="form-control zipcode-field" placeholder="Ort*" required >' +
        '<label asp-for="LieferAdresse.Ort">Ort*</label>' +
        '<span asp-validation-for="LieferAdresse.Ort" class="text-danger"></span>' +
        '</div>' +
        '<div class="form-floating mb-3 col-auto">' +
        '<input type="text" asp-for="LieferAdresse.PLZ" class="form-control zipcode-field" id="floatingInput" placeholder="PLZ*" required >' +
        '<label asp-for="LieferAdresse.PLZ">PLZ*</label>' +
        '<span asp-validation-for="LieferAdresse.PLZ" class="text-danger"></span>' +
        '</div>' +
        '<div class="form-group">' +
        '<select class="form-control">' +
        '<option class="hidden" selected disabled>Wählen Sie Ihres Land</option>' +
        '<option>Deutschland</option>' +
        '</select>' +
        '</div>' +
        '</div>'; 
   //#endregion 

    $('#hasTowAdresse').on('click', function () {
        if ($(this).is(':checked')) {

            $('#adressen').append(lieferAdresse1);

          /*  $('#LieferAdresse').css('display','block')*/
        }
        else {

            $('#lieferAdresse1').remove();

           /* $('#LieferAdresse').css('display', 'none')*/

        }
    })

    $('#GastKunde').on('click', function () {

        if ($(this).is(':checked')) {

            $('#checkout .password').removeAttr("Required")
            $('#checkout .password').attr(
                { disabled: "true" }
            );
           
        }
        else {

            $('#checkout .password').attr("disabled", false);
            $('#checkout .password').attr("Required", "Required");

        }
    })

    $('body').on('click', '#hasTowAdresseInCheckout', function () {

        if ($(this).is(':checked')) {

            /*$(lieferAdresse2).appendTo('body #adressen');*/

            //var adressenBlock = $(this).parents().find('.adress-Block').children('#adressen'); // Div mit Id adressen über parents und children finden
            //debugger

            //$(adressenBlock).append(lieferAdresse2);
          
              $('#LieferAdresse2').css('display','block')
        }
        else {

            /*$('#lieferAdresse2').remove();*/

             $('#LieferAdresse2').css('display', 'none')

        }
    })


    // #region das Alte Code, wo man es klickt und direkt in den Warenkorp landet, sollte NUR wieder Aktivert werden und reagiert nur auf ohne details-Teil und minus und plus
      // #region Plus Minus (Mindestmenge: 3)

   // $('#Karton-Box .plus').on('click', function () {

   //     var value = Number.parseInt($(this).val());
   //     var productCounter = $(this).prev('.product-counter');
   //     var minusBtn = $(this).prev().prev('.minus');
   //     $(minusBtn).css('display','flex')
   //     var counterValue = Number.parseInt($(productCounter).text());

   //     if (counterValue == 0) {
   //         counterValue += 3;
   //     }
   //     else {
   //         counterValue += 1;
   //     }

   //     $(productCounter).text(counterValue);

   //     var cardItem = $(this).parents('.cardItem');
   //     var articlePriceAmount = $(cardItem).children().find('.article-price-amount');
   //     var result = value * counterValue;
   //     $(articlePriceAmount).find('.result').text(result);
   //     var postleitzahl = $('.zipcodeResult span')[0].innerText;
   //     var stadtname = $('.zipcodeResult span')[1].innerText;

   //     var productName = $(cardItem)[0].id

   //     debugger

   //     $.post("/Shop/_ShoppingCart", { productName: productName, quantity: counterValue, _articlePrice: value, _articlePriceAmount: result, postleitzahl: postleitzahl, stadtname, stadtname,flag: 1 }, function (data) {
   //         if (data != null) {

   //             sessionStorage.setItem('warenkorbObj', data)

   //             shoppingCartToSession();

   //         }
   //         else {
   //             sessionStorage.clear();
   //         }
   //     })
   // })

   // $('#Karton-Box .minus').on('click', function () {
   //     var value = Number.parseInt($(this).val());
   //     var productCounter = $(this).next('.product-counter');
   //     var counterValue = Number.parseInt($(productCounter).text());

   //     if (counterValue > 0) {

   //         if (counterValue == 3) {
   //             counterValue -= 3;
   //         }
   //         else if (counterValue > 3) {
   //             counterValue -= 1;
   //         }
   //     }

   //     if (counterValue == 0) {
   //         $(this).css('display', 'none')
   //     }

   //     $(productCounter).text(counterValue);

   //     var cardItem = $(this).parents('.cardItem');
   //     var articlePriceAmount = $(cardItem).children().find('.article-price-amount');
   //     var result = value * counterValue;
   //     $(articlePriceAmount).find('.result').text(result);

   //     var productName = $(cardItem)[0].id

   //     var postleitzahl = $('.zipcodeResult span')[0].innerText;
   //     var stadtname = $('.zipcodeResult span')[1].innerText;

   //     $.post("/Shop/_ShoppingCart", { productName: productName, quantity: counterValue, _articlePrice: value, _articlePriceAmount: result, postleitzahl: postleitzahl, stadtname, stadtname, flag: 0 }, function (data) {

   //         if (data != null) {

   //             sessionStorage.setItem('warenkorbObj', data)
   //             shoppingCartToSession();
   //         }
   //         else {
   //             sessionStorage.clear();
   //         }
   //     })
   // })

   //// #endregion

   // // #region (Mindestmenge: 1) Klasse C1

   // $('.cardGroup .C1 .plus').on('click', function () {
   //     var value = Number.parseFloat($(this).val());
   //     var productCounter = $(this).prev('.product-counter');
   //     var minusBtn = $(this).prev().prev('.minus');
   //     $(minusBtn).css('display', 'flex')
   //     var counterValue = Number.parseInt($(productCounter).text());
   //     counterValue += 1;
   //     $(productCounter).text(counterValue);
   //     var cardItem = $(this).parents('.cardItem');
   //     var articlePriceAmount = $(cardItem).children().find('.article-price-amount');
   //     var result = value * counterValue;
   //     $(articlePriceAmount).find('.result').text(result);

   //     var postleitzahl = $('.zipcodeResult span')[0].innerText;
   //     var stadtname = $('.zipcodeResult span')[1].innerText;
        
   //     var productName = $(cardItem)[0].id
        
   //     $.post("/Shop/_ShoppingCart", { productName: productName, quantity: counterValue, _articlePrice: value, _articlePriceAmount: result, postleitzahl: postleitzahl, stadtname, stadtname, flag: 1 }, function (data) {
      
   //         if (data != null) {
   //             sessionStorage.setItem('warenkorbObj', data)
   //             shoppingCartToSession();
   //         }
   //         else {
   //             sessionStorage.clear();
   //         }
   //     },"json")
   // })

   // $('.cardGroup .C1 .minus').on('click', function () {
   //     var value = Number.parseFloat($(this).val());
   //     var productCounter = $(this).next('.product-counter');
   //     var counterValue = Number.parseInt($(productCounter).text());

   //     if (counterValue > 0) {
   //         counterValue -= 1;
   //     }

   //     if (counterValue == 0) {
   //         $(this).css('display', 'none')
   //     }

   //     $(productCounter).text(counterValue);

   //     var cardItem = $(this).parents('.cardItem');
   //     var articlePriceAmount = $(cardItem).children().find('.article-price-amount');
   //     var result = value * counterValue;
   //     $(articlePriceAmount).find('.result').text(result);

   //     var postleitzahl = $('.zipcodeResult span')[0].innerText;
   //     var stadtname = $('.zipcodeResult span')[1].innerText;
 
   //     var productName = $(cardItem)[0].id

   //     $.post("/Shop/_ShoppingCart", { productName: productName, quantity: counterValue, _articlePrice: value, _articlePriceAmount: result, postleitzahl: postleitzahl, stadtname, stadtname, flag: 0 }, function (data) {
   //         if (data != null) {

   //             sessionStorage.setItem('warenkorbObj', data)
   //             shoppingCartToSession();
   //         }
   //         else {
   //             sessionStorage.clear();
   //         }
   //     })
   // })

   // //#endregion

    var kartonCounter = 0;
    var counterValue = 0;

    // #region Shop-Plus  Shop-Minus

    //(Mindestmenge: 3) diese Code unterscheidet sich von der Seite details NUR Teil der Klassenname .details- Weg löschen
    $('.shop #Karton-Box .plus2').on('click', function () {

        var data = sessionStorage.getItem('warenkorbObj');
        var warenkorbObj = JSON.parse(data);

        var value = Number.parseInt($(this).val());

        var productCounter = $(this).prev('.product-counter');
        var minusBtn = $(this).prev().prev('.minus2');
        $(minusBtn).css('display', 'flex');
        var counterValue = Number.parseInt($(productCounter).text());


        if (warenkorbObj !== null) {


            var kartonBox = warenkorbObj["WarenkorbItemsListe"].find(x => { return x.ProduktName == "Karton-Box"; })


            if (kartonBox == null || kartonBox.Quantity == 0) {

                counterValue += 3;
            }
            else {
                counterValue += 1;
            }
        }
        else {
            if (counterValue == 0) {
                counterValue += 3;
            }
            else {
                counterValue += 1;
            }

        }

        $(productCounter).text(counterValue);

        var cardItem = $(this).parents('.cardItem');
        var resultAmount = $(cardItem).children().find('.result');
        var result = value * counterValue;
        $(resultAmount).text(euro.format(result));
    })

    $('.shop #Karton-Box .minus2').on('click', function () {

        var data = sessionStorage.getItem('warenkorbObj');
        var warenkorbObj = JSON.parse(data);

        
        var cardItem = $(this).parents('.cardItem');
        //var articlePriceAmount = $(cardItem).children().find('.article-price-amount');

        var value = Number.parseInt($(this).val());
        var productCounter = $(this).next('.product-counter');

        var counterValue = Number.parseInt($(productCounter).text());


        if (warenkorbObj !== null) {


            var kartonBox = warenkorbObj["WarenkorbItemsListe"].find(x => { return x.ProduktName == "Karton-Box"; })


            if (kartonBox.Quantity >= 3) {

                if (counterValue == 3) {
                    counterValue -= 3;
                }
                else {
                    counterValue -= 1;
                }
            }
            else if (kartonBox.Quantity == 0) {
                counterValue -= 1;
            }
        }
        // warenkorb == null
        else {
            if (counterValue == 3) {
                counterValue -= 3;
            }
            else {
                counterValue -= 1;
            }

        }


        if (counterValue == 0) {
            $(this).css('display', 'none')
        }

        $(productCounter).text(counterValue);

        //var cardItem = $(this).parents('.cardItem');
        var resultAmount = $(cardItem).children().find('.result');
        var result = value * counterValue;
        $(resultAmount).text(euro.format(result));
    })

    // (Mindestmenge: 1) diese Code unterscheidet sich von der Seite details NUR Teil der Klassenname .details- Weg löschen
    $('.shop .cardGroup .C1 .plus2').on('click', function () {
        var value = Number.parseFloat($(this).val());
        var productCounter = $(this).prev('.product-counter');
        var minusBtn = $(this).prev().prev('.minus2');
        $(minusBtn).css('display', 'flex')
        var counterValue = Number.parseInt($(productCounter).text());
        counterValue += 1;
        $(productCounter).text(counterValue);

        var cardItem = $(this).parents('.cardItem');
        var resultAmount = $(cardItem).children().find('.result');
        var result = value * counterValue;
        $(resultAmount).text(euro.format(result));
    })

    $('.shop .cardGroup .C1 .minus2').on('click', function () {
        var value = Number.parseFloat($(this).val());
        var productCounter = $(this).next('.product-counter');
        var counterValue = Number.parseInt($(productCounter).text());
        debugger

        if (counterValue > 0) {
            counterValue -= 1;
        }

        if (counterValue == 0) {
            $(this).css('display', 'none')
        }

        $(productCounter).text(counterValue);

        var cardItem = $(this).parents('.cardItem');
        var resultAmount = $(cardItem).children().find('.result');
        var result = value * counterValue;
        $(resultAmount).text(euro.format(result));

    })

     // .in-warenkorb Shop - Bereich
    $('.shop #Karton-Box .in-warenkorb,.shop #Container-Box-Metall .in-warenkorb,.shop #Container-Box-Holz .in-warenkorb,.shop #Container-Box .in-warenkorb ,.shop #Einzelstücke .in-warenkorb').on('click', function () {

        var cardItem = $(this).parents('.shop .cardItem');
        var productName = $(cardItem)[0].id


        var data = sessionStorage.getItem('warenkorbObj');
        var warenkorbObj = JSON.parse(data);

        var productCounter = $(cardItem).children().find('.product-counter');
        var counterValue = Number.parseInt($(productCounter)[0].innerText); // 1.

        if (warenkorbObj != null) {
            var product = warenkorbObj["WarenkorbItemsListe"].find(x => { return x.ProduktName == productName; });

            //counterValue = product !== undefined ? counterValue + product.Quantity : counterValue;

            if (product != null || product != undefined) {
                counterValue += product.Quantity;
            }

        }

        var detailsPlus = $(cardItem).children().find('.plus2');
        var plusValue = Number.parseFloat($(detailsPlus).val());  // 2.

        //var _result = $(cardItem).children().find('.result').text();
        //var _resultSplit = _result.split('€')[0];
        //var result = Number.parseInt(_resultSplit);  // 3.

        var result = counterValue * plusValue;



        var _articlePriceAmount = $(cardItem).children().find('.article-price-amount');
        var articlePriceAmount = Number.parseInt($(_articlePriceAmount).text());

        var postleitzahl = $('.shop').children().find('.zipcodeResult span')[0].innerText;
        var stadtname = $('.shop').children().find('.zipcodeResult span')[1].innerText;
  
        if (counterValue > 0) {

            $.post("/Shop/_ShoppingCart", { productName: productName, quantity: counterValue, _articlePrice: plusValue, _articlePriceAmount: result, postleitzahl: postleitzahl, stadtname, stadtname, flag: 1 }, function (data) {
                if (data != null) {

                    sessionStorage.setItem('warenkorbObj', data)

                    shopSiteUpdate(cardItem);

                }
                else {
                    sessionStorage.clear();
                    shopSiteUpdate(cardItem);
                }
            })
        }
 

    })

    //#endregion


    // #region details-Plus  details-Minus (Mindestmenge: 3)

     //(Mindestmenge: 3) diese Code unterscheidet sich von der Seite Shop NUR Teil der Klassenname .details- Weg hinzufügen
    $('#Karton-Box .details-plus').on('click', function () {

        var data = sessionStorage.getItem('warenkorbObj');
        var warenkorbObj = JSON.parse(data);

        var value = Number.parseInt($(this).val());

        var productCounter = $(this).prev('.details-product-counter');
        var minusBtn = $(this).prev().prev('.details-minus');
        $(minusBtn).css('display', 'block');
        var counterValue = Number.parseInt($(productCounter).text());

        if (warenkorbObj !== null) {
            

            var kartonBox = warenkorbObj["WarenkorbItemsListe"].find(x => { return x.ProduktName == "Karton-Box"; })


            if (kartonBox == null || kartonBox.Quantity == 0) {

                counterValue += 3;
            }
            else {
                counterValue += 1;
            }
        }
        else {
            if (counterValue == 0) {
                counterValue += 3;
            }
            else {
                counterValue += 1;
            }
            
        }

        $(productCounter).text(counterValue);

        var cardItem = $(this).parents('.cardItem');
        var articlePriceAmount = $(cardItem).children().find('.article-price-amount');
        var result = value * counterValue;
        $(articlePriceAmount).text(euro.format(result));     
    })

    $('#Karton-Box .details-minus').on('click', function () {
   
        var data = sessionStorage.getItem('warenkorbObj');
        var warenkorbObj = JSON.parse(data);
        
        var cardItem = $(this).parents('.cardItem');
        var articlePriceAmount = $(cardItem).children().find('.article-price-amount');

        var value = Number.parseInt($(this).val());
        var productCounter = $(this).next('.details-product-counter');
        var counterValue = Number.parseInt($(productCounter).text());

        if (warenkorbObj !== null) {

            var kartonBox = warenkorbObj["WarenkorbItemsListe"].find(x => { return x.ProduktName == "Karton-Box"; })

            // if Warenkorb exist but kartonBox exist not im Warenkorb
            if (kartonBox == null || kartonBox.Quantity == 0) {

                if (counterValue == 3) {
                    counterValue -= 3;
                }
           // if Warenkorb exist, but kartonBox exist im Warenkorb
                else if (counterValue > 3 || counterValue < 3 && counterValue !== 0) {
                    counterValue -= 1;
                }           
            }
            else {
                counterValue -= 1;
            }
        }
        else {
            if (counterValue > 0) {

                if (counterValue == 3) {
                    counterValue -= 3;
                }
                else if (counterValue > 3 || counterValue < 3 && counterValue !== 0) {
                    counterValue -= 1;
                }
            }

        }
      

        if (counterValue == 0) {
            $(this).css('display', 'none')
        }

        $(productCounter).text(counterValue);
        var result = value * counterValue;
        $(articlePriceAmount).text(euro.format(result));       
    })

    //(Mindestmenge: 1) diese Code unterscheidet sich von der Seite Shop NUR Teil der Klassenname .details- Weg hinzufügen
    $('.cardItem.C1 .details-plus').on('click', function () {
        var cardItem = $(this).parents('.cardItem.C1');
        
        var value = Number.parseFloat($(this).val());

        var productCounter = $(cardItem).children().find('.details-product-counter');
        var counterValue = Number.parseInt($(productCounter).text());

        var minusBtn = $(cardItem).children().find('.details-minus');
        $(minusBtn).css('display', 'block')

        counterValue += 1;
        $(productCounter).text(counterValue);

        
        var articlePriceAmount = $(cardItem).children().find('.details-result');
        var result = value * counterValue;
        $(articlePriceAmount).text(euro.format(result));
    })

    $('.cardItem.C1 .details-minus').on('click', function () {

        var cardItem = $(this).parents('.cardItem.C1');

        var value = Number.parseFloat($(this).val());

        var productCounter = $(cardItem).children().find('.details-product-counter');
        var counterValue = Number.parseInt($(productCounter).text());

        var minusBtn = $(cardItem).children().find('.details-minus');

        if (counterValue > 0) {
            counterValue -= 1;
        }

        if (counterValue == 0) {
            $(this).css('display', 'none')
        }

        $(productCounter).text(counterValue);


        var articlePriceAmount = $(cardItem).children().find('.details-result');
        var result = value * counterValue;
        $(articlePriceAmount).text(euro.format(result));

    })

    // .in-warenkorb details- Bereich
    $('.details #Karton-Box .in-warenkorb,.details #Container-Box-Metall .in-warenkorb,.details #Container-Box-Holz .in-warenkorb,.details #Einzelstücke .in-warenkorb').on('click', function () {

        var cardItem = $(this).parents('.cardItem');
        
        var productName = $(cardItem)[0].id


        var data = sessionStorage.getItem('warenkorbObj');
        var warenkorbObj = JSON.parse(data);

        var productCounter = $(cardItem).children().find('.details-product-counter');
        var counterValue = Number.parseInt($(productCounter).text()); // 1.

        if (warenkorbObj != null) {
            var product = warenkorbObj["WarenkorbItemsListe"].find(x => { return x.ProduktName == productName; });

            //counterValue = product !== undefined ? counterValue + product.Quantity : counterValue;

            if (product != null || product != undefined) {
                counterValue += product.Quantity;
            }
            
        }

        var detailsPlus = $(cardItem).children().find('.details-plus');
        var plusValue = Number.parseFloat($(detailsPlus).val());  // 2.
      

        //var _result = $(cardItem).children().find('.result').text();
        //var _resultSplit = _result.split('€')[0];
        //var result = Number.parseInt(_resultSplit);  // 3.

        var result = counterValue * plusValue;

         
        var detailsMinus = $(cardItem).children().find('.details-minus');
        var _articlePriceAmount = $(cardItem).children().find('.details-result');

        var articlePriceAmount = Number.parseInt($(_articlePriceAmount).text());

        var postleitzahl = $(cardItem).children('.zipcodeResult2').children('span')[0].innerText;
        var stadtname = $(cardItem).children('.zipcodeResult2').children('span')[1].innerText;


        $.post("/Shop/_ShoppingCart", { productName: productName, quantity: counterValue, _articlePrice: plusValue, _articlePriceAmount: result, postleitzahl: postleitzahl, stadtname, stadtname, flag: 1 }, function (data) {

            if (data != null) {
                sessionStorage.setItem('warenkorbObj', data)
                detailsSiteUpdate(cardItem);
                //$(productCounter).text(0)
                //$(_articlePriceAmount).text(euro.format(0))
                //$(detailsMinus).css('display', 'none')
                //shoppingCartToSession();
            }
            else {
                sessionStorage.clear();
                shopSiteUpdate(cardItem);
            }
        })

    })

    // #endregion


})();


function PasswortGenerator() {
    let n = 1;
    let password = "";
    let zeichen = [];
    let kleinbuchstaben = 6;
    let grossbuchstaben = 2;
    let ziffern = 2;
    let sonderZeichen = 2;
    let passLong = kleinbuchstaben + grossbuchstaben + ziffern + sonderZeichen;
    let rnd;

    for (var i = 0; i < ziffern; i++) {
        rnd = Math.floor(Math.random() * (57 - 48) + 48);
        zeichen.push(String.fromCharCode(rnd))
    }

    for (var i = 0; i < grossbuchstaben; i++) {
        rnd = Math.floor(Math.random() * (90 - 65) + 65);
        zeichen.push(String.fromCharCode(rnd))
    }

    for (var i = 0; i < sonderZeichen; i++) {
        rnd = Math.floor(Math.random() * (3 - 1) + 1);
        if (rnd == 1) {
            rnd = Math.floor(Math.random() * (47 - 33) + 33);
            zeichen.push(String.fromCharCode(rnd))
        }
        if (rnd == 2) {
            rnd = Math.floor(Math.random() * (64 - 58) + 58);
            zeichen.push(String.fromCharCode(rnd))
        }
        if (rnd == 3) {
            rnd = Math.floor(Math.random() * (126 - 123) + 123);
            zeichen.push(String.fromCharCode(rnd))
        }

    }

    for (var i = 0; i < kleinbuchstaben; i++) {
        rnd = Math.floor(Math.random() * (122 - 97) + 97);
        zeichen.push(String.fromCharCode(rnd))
    }

    return zeichen;
}

function PasswordMix(passwort) {
    let index1 = 0;
    let index2 = 0;
    let n = 0;
    let c = '';

    do {
        index1 = Math.floor(Math.random() * passwort.length);
        index2 = Math.floor(Math.random() * passwort.length);

        if (index1 != index2 && index1 < passwort.length && index2 < passwort.length) {
            c = passwort[index1];
            passwort[index1] = passwort[index2];
            passwort[index2] = c;

        }

        n++;

    } while (n <= 100);
    return passwort;
}


//var counterValue = (function () {
//    debugger
//    let value = 0;
//    return function increment() {
//        value += 1;
//        return alert(value);

//    };

//})();