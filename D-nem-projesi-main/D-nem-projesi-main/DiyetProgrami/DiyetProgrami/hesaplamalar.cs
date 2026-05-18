using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DiyetProgrami
{
    public class hesaplamalar
    {


        public static double aktiviteKatsayisiHesapla(string aktiviteSeviyesi)
        {
            if (string.IsNullOrEmpty(aktiviteSeviyesi)) return 1.2;

            // Metinsel uyuşmazlıkları (boşluk, büyük/küçük harf) tamamen sıfırlıyoruz
            string temizAktiflik = aktiviteSeviyesi.Trim().ToLower();

            switch (temizAktiflik)
            {
                case "hareketsiz":
                    kullancicisession.AktiviteKatsayisi = 1.2;
                    break;
                case "hafif aktif":
                    kullancicisession.AktiviteKatsayisi = 1.375;
                    break;
                case "orta aktif":
                    kullancicisession.AktiviteKatsayisi = 1.55;
                    break;
                case "çok aktif":
                    kullancicisession.AktiviteKatsayisi  = 1.725;
                    break;
                default:
                    kullancicisession.AktiviteKatsayisi = 1.2; // Eşleşme olmazsa varsayılan güvenli değer
                    break;
            }
            return kullancicisession.AktiviteKatsayisi;
        }

        public static double vkiHesapla(double boy, double kilo)
        {
            if (boy <= 0) return 0;

            double boyMetre = boy / 100.0;
            double vki = kilo / (boyMetre * boyMetre);

            return Math.Round(vki, 2);
        }

        public static double bmh(bool cinsiyet, double kilo, double boy, int yas)
        {
            if (cinsiyet == true) // Erkekler
            {
                return Math.Round(66.5 + (13.75 * kilo) + (5.003 * boy) - (6.75 * yas), 2);
            }
            else // Kadınlar
            {
                return Math.Round(655.1 + (9.563 * kilo) + (1.85 * boy) - (4.676 * yas), 2);
            }
        }

        public static double gunlukkalori(bool cinsiyet, double kilo, double boy, int yas, double katsayi, string amac)
        {
            double anaBmh = bmh(cinsiyet, kilo, boy, yas);
            double netIhtiyac = anaBmh * katsayi;
            double finalKalori = netIhtiyac;

            if (!string.IsNullOrEmpty(amac))
            {
                string temizAmac = amac.Trim().ToLower();

                // "vermek" yerine "verm" veya "verme", "almak" yerine "alm" veya "alma" kontrolü yapıyoruz
                if (temizAmac.Contains("verm") || temizAmac.Contains("yakmak") || temizAmac.Contains("zayıf"))
                {
                    finalKalori = netIhtiyac - 500;
                }
                else if (temizAmac.Contains("alm") || temizAmac.Contains("kas"))
                {
                    finalKalori = netIhtiyac + 400;
                }
                else if (temizAmac.Contains("özel") || temizAmac.Contains("hızlı"))
                {
                    finalKalori = netIhtiyac - 700;
                }
            }

            if (finalKalori < 1200) finalKalori = 1200;

            return Math.Round(finalKalori, 2);


        }

        public static double BugunAlinanToplamKalori()
        {
            double toplamKalori = 0;

            string query = @"
                SELECT SUM(bd.MiktarGram * b.Kalori / 100.0) 
                FROM Ogunler o
                JOIN OgunDetaylari bd ON o.OgunID = bd.OgunID
                JOIN Besinler b ON bd.BesinID = b.BesinID
                WHERE o.KullaniciID = @KullaniciID 
                  AND CAST(o.Tarih AS DATE) = CAST(GETDATE() AS DATE)";

            using (SqlConnection c = new SqlConnection(connection.connectionstring))
            {
                using (SqlCommand cmd = new SqlCommand(query, c))
                {
                    cmd.Parameters.AddWithValue("@KullaniciID", kullancicisession.KullaniciID);

                    try
                    {
                        c.Open();
                        object result = cmd.ExecuteScalar();

                        if (result != DBNull.Value && result != null)
                        {
                            toplamKalori = Convert.ToDouble(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Kalori hesaplanırken hata oluştu: " + ex.Message);
                    }
                }
            }
            return toplamKalori;
        }

        public static void ProgressBarGuncelle(ProgressBar pbKalori, Label lblKaloriOzet)
        {
            double hedefKalori = kullancicisession.GunlukKaloriHedefi;

            // Sıfıra bölünme veya negatif hedef kalori hatalarına karşı koruma
            if (hedefKalori <= 0) hedefKalori = 2000;

            pbKalori.Maximum = (int)hedefKalori;
            pbKalori.ForeColor = System.Drawing.Color.Yellow;

            double alinanKalori = BugunAlinanToplamKalori();

            if (alinanKalori >= hedefKalori)
            {
                pbKalori.Value = pbKalori.Maximum;
                pbKalori.ForeColor = System.Drawing.Color.Red;
            }
            else if (alinanKalori <= 0)
            {
                pbKalori.Value = 0;
            }
            else
            {
                pbKalori.Value = (int)alinanKalori;
            }

            lblKaloriOzet.Text = $" Bugün aldığınız kalori: Alınan: {alinanKalori:N0} / Hedef: {hedefKalori:N0} kcal";
        }

        public static void GrafigiGuncelle(Chart chartGelisim)
        {
            chartGelisim.Series["Kilo"].Points.Clear();

            string query = @"SELECT KayitTarihi, Kilo 
                             FROM KullaniciGelisimi 
                             WHERE KullaniciID = @uid 
                             ORDER BY KayitTarihi ASC";

            using (SqlConnection con = new SqlConnection(connection.connectionstring))
            {
                // SqlCommand nesnesini using bloğuna alarak güvenli hale getirdik
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@uid", kullancicisession.KullaniciID);

                    try
                    {
                        con.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                DateTime tarih = Convert.ToDateTime(dr["KayitTarihi"]);
                                double kilo = Convert.ToDouble(dr["Kilo"]);

                                chartGelisim.Series["Kilo"].Points.AddXY(tarih.ToShortDateString(), kilo);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Grafik yüklenirken hata: " + ex.Message);
                    }
                }
            }
        }
    }
}