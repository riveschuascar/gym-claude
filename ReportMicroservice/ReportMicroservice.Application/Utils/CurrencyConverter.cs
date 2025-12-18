using System.Text;

namespace ReportMicroservice.Application.Utils
{
    public static class CurrencyConverter
    {
        public static string Convertir(decimal amount)
        {
            // Separar parte entera y decimal
            int entero = (int)Math.Truncate(amount);
            int decimales = (int)Math.Round((amount - entero) * 100);

            string literal = NumeroALetras(entero);

            return $"SON {literal} {decimales:00}/100 BOLIVIANOS";
        }

        private static string NumeroALetras(int value)
        {
            if (value == 0) return "CERO";

            if (value < 0) return "MENOS " + NumeroALetras(Math.Abs(value));

            string words = "";

            if ((value / 1000000) > 0)
            {
                words += NumeroALetras(value / 1000000) + (value / 1000000 == 1 ? " MILLON " : " MILLONES ");
                value %= 1000000;
            }

            if ((value / 1000) > 0)
            {
                if (value / 1000 == 1)
                    words += "MIL ";
                else
                    words += NumeroALetras(value / 1000) + " MIL ";
                value %= 1000;
            }

            if ((value / 100) > 0)
            {
                if (value == 100) 
                    words += "CIEN ";
                else
                {
                    string[] centenas = { "", "CIENTO", "DOSCIENTOS", "TRESCIENTOS", "CUATROCIENTOS", "QUINIENTOS", "SEISCIENTOS", "SETECIENTOS", "OCHOCIENTOS", "NOVECIENTOS" };
                    words += centenas[value / 100] + " ";
                }
                value %= 100;
            }

            if (value > 0)
            {
                string[] unidades = { "", "UN", "DOS", "TRES", "CUATRO", "CINCO", "SEIS", "SIETE", "OCHO", "NUEVE", "DIEZ", "ONCE", "DOCE", "TRECE", "CATORCE", "QUINCE", "DIECISEIS", "DIECISIETE", "DIECIOCHO", "DIECINUEVE" };
                string[] decenas = { "", "DIEZ", "VEINTE", "TREINTA", "CUARENTA", "CINCUENTA", "SESENTA", "SETENTA", "OCHENTA", "NOVENTA" };

                if (value < 20)
                    words += unidades[value];
                else
                {
                    words += decenas[value / 10];
                    if ((value % 10) > 0)
                    {
                        if (value < 30) // Veinti...
                        {
                            words = words.TrimEnd(); // Quitar espacio
                            string[] veintes = { "VEINTE", "VEINTIUN", "VEINTIDOS", "VEINTITRES", "VEINTICUATRO", "VEINTICINCO", "VEINTISEIS", "VEINTISIETE", "VEINTIOCHO", "VEINTINUEVE" };
                            words = words.Replace("VEINTE", "") + veintes[value % 10]; 
                        }
                        else
                        {
                            words += " Y " + unidades[value % 10];
                        }
                    }
                }
            }

            return words.Trim();
        }
    }
}