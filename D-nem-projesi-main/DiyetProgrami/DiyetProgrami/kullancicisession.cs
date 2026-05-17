using DiyetProgrami;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiyetProgrami
{
    public class kullancicisession
    {// Temel Bilgiler
        public static int KullaniciID;
        public static string Ad;
        public static string Soyad;
        public static bool Cinsiyet; // 1: Erkek, 0: Kadın
        public static DateTime DogumTarihi;
        public static int Yas;
        public static string amac;
        public static int SuBardak { get; set; }

        // Fiziksel Veriler
        public static double Boy;
        public static double GuncelKilo;
        public static double HedefKilo;

        // Hesaplanan Veriler
        public static double VKI;
        public static double BMH;
        public static double AktiviteKatsayisi;
        public static string AktiviteSeviyesi;
        public static double GunlukKaloriHedefi;

        // Sistem Bilgisi
        public static DateTime KayitTarihi;


        
            

        


    }
}
