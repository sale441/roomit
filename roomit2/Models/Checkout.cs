using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace roomit2.Models
{
    public class Checkout
    {

        public string? KundenType { get; set; }
        public string? Anrede { get; set; }

        [Required(ErrorMessage = "Vorname ist ein  Pflichtfeld ")]
        [DataType(DataType.Text,ErrorMessage = "Text Format wird nicht erkannt")]
        public string? Vorname { get; set; }

        [Required(ErrorMessage = "Nachname ist ein  Pflichtfeld ")]
        [DataType(DataType.Text,ErrorMessage = "Text Format wird nicht erkannt")]
        public string? Nachname { get; set; }

        [CheckBoxRequired(ErrorMessage = "akzeptieren Sie bitte die Geschäftsbedingungen")]
        public bool GeschäftsbedingungenAkzeptiert { get; set; }

        [Required(ErrorMessage = "Telefon ist ein  Pflichtfeld ")]
        [Phone(ErrorMessage = "Telefon Format wird nicht erkannt")]
        public string? Telefon { get; set; }

        [Required(ErrorMessage = "E-Mail ist ein  Pflichtfeld ")]
        [EmailAddress(ErrorMessage = "Email Format wird nicht erkannt")]
        public string? Email { get; set; }


        //[Required(ErrorMessage = "Passwort ist ein  Pflichtfeld")] // **** wegen Als Gast bestellung Required wird dynamisch in Jquery hinzugefügt
        [StringLength(100, ErrorMessage = "Die {0} muss mindestens {2} und maximal {1} ​​Zeichen lang sein.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Passwort")]
        public string? Password { get; set; }

        //[Required(ErrorMessage = "Passwort ist ein  Pflichtfeld")]   // **** wegen Als Gast bestellung Required wird dynamisch in Jquery hinzugefügt
        [DataType(DataType.Password)]
        [Display(Name = "Bestätigen Sie das Passwort")]
        [Compare("Password", ErrorMessage = "Das Passwort und das Bestätigungspasswort stimmen nicht überein.")]
        public string? ConfirmPassword { get; set; }

        [DisplayName("Nummer der Bestellung")]
        public string? Bestellnummer { get; set; }
        public bool HasTowAdresse { get; set; }


        [DisplayName("Lieferdatum:")]
        public DateTime LieferDatum { get; set; }

        public Adresse? RechnungAdresse { get; set; }

        [DefaultValue(value:null)]
        [ValidateNever]
        public Adresse? LieferAdresse { get; set; }

        [NotMapped]
        [TempData]
        public bool GastKunde { get; set; }

        //Friendlycaptcha
        [Required(ErrorMessage = "Anti-Roboter-Verifizierung")]
        public string? FriendlycaptchaToken { get; set; }


    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class CheckBoxRequiredAttribute : RequiredAttribute   // die Vererbung von ValidationAttribute funktioniert nicht aber mit RequiredAttribute funktioniert
    {
        public override bool IsValid(object? value)
        {
            return value != null && value is bool && (bool)value;

        }

    }

    //public class CheckBoxRequired : ValidationAttribute
    //{
    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        //get the entered value
    //        var checkout = (Checkout)validationContext.ObjectInstance;
    //        //Check whether the IsAccepted is selected or not.
    //        if (checkout.GeschäftsbedingungenAkzeptiert == false)
    //        {
    //            //if not checked the checkbox, return the error message.
    //            return new ValidationResult(ErrorMessage == null ? "Please checked the checkbox" : ErrorMessage);

    //        }
    //        else { 
    //            return ValidationResult.Success;
    //        }

    //    }
    //}
}
