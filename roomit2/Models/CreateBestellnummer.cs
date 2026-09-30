using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace roomit2.Models
{
    public static class CreateBestellnummer
    {
        public static String CreateBestellNumber ()
        {
            List<char> bestellnummer = InitBestellung();
            string bestellnummerMix = MixNumber(bestellnummer.ToArray());
            return bestellnummerMix;

        }
        public static string CreateTempPassword()
        {
            List<char> bestellnummer = InitPassword();
            string bestellnummerMix = MixNumber(bestellnummer.ToArray());
            return bestellnummerMix;

        }

        private static List<char> InitBestellung()
        {
            int n = 1;
            Random rnd = new Random();
            string nummer = string.Empty;
            List<char> zeichen = new List<char>();
            int kleinbuchstaben = 4;
            int ziffern = 11;
            int passLong = kleinbuchstaben + ziffern ;


            // ADD Ziffern
            for (int i = 0; i < ziffern; i++)
            {
                zeichen.Add((char)rnd.Next(48, 57));
            }

            //// ADD Kleinbuchstaben
            //for (int i = 0; i < kleinbuchstaben; i++)
            //{
            //    zeichen.Add((char)rnd.Next(97, 122));
            //}

            return zeichen;
        }

        private static List<char> InitPassword()
        {
            int n = 1;
            Random rnd = new Random();
            string password = string.Empty;
            List<char> zeichen = new List<char>();
            int kleinbuchstaben = 6;
            int grossbuchstaben = 2;
            int ziffern = 2;
            int sonderZeichen = 2;
            int passLong = kleinbuchstaben + grossbuchstaben + ziffern + sonderZeichen;


            // ADD Ziffern
            for (int i = 0; i < ziffern; i++)
            {
                zeichen.Add((char)rnd.Next(48, 57));
            }
            // ADD Grossbuchstaben
            for (int i = 0; i < grossbuchstaben; i++)
            {
                zeichen.Add((char)rnd.Next(65, 90));
            }
            // ADD SonderZeichen
            for (int i = 0; i < sonderZeichen; i++)
            {
                int x = rnd.Next(1, 3);

                if (x == 1)
                {
                    zeichen.Add((char)rnd.Next(33, 47));
                }
                else if (x == 2)
                {
                    zeichen.Add((char)rnd.Next(58, 64));
                }
                else if (x == 3)
                {
                    zeichen.Add((char)rnd.Next(123, 126));
                }

            }

            // ADD Kleinbuchstaben
            for (int i = 0; i < kleinbuchstaben; i++)
            {
                zeichen.Add((char)rnd.Next(97, 122));
            }

            return zeichen;
        }

        private static string MixNumber(char[] chars)
        {
            Random rnd = new Random();
            int index1;
            int index2;
            int n = 0;
            char c;

            do
            {
                index1 = rnd.Next(chars.Length);
                index2 = rnd.Next(chars.Length);

                if (index1 != index2 && index1 < chars.Length && index2 < chars.Length)
                {
                    c = chars[index1];
                    chars[index1] = chars[index2];
                    chars[index2] = c;
                }

                n++;

            } while (n <= 100);

            return new string(chars);
        }
    }
}
