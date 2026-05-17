using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace DiyetProgrami
{
    public class hesaplamalar
    {
        public static double aktiviteKatsayisi = kullancicisession.AktiviteKatsayisi;
        public static double aktiviteKatsayisiHesapla(string aktiviteSeviyesi)
        {
            switch (aktiviteSeviyesi)
            {
                case "Hareketsiz":
                    aktiviteKatsayisi = 1.2;
                    break;
                case "Az Hareketli":
                    aktiviteKatsayisi = 1.375;
                    break;
                case "Orta Derecede Aktif":
                    aktiviteKatsayisi = 1.55;
                    break;
                case "Çok Aktif":
                    aktiviteKatsayisi = 1.725;
                    break;
                case "Aşırı Aktif":
                    aktiviteKatsayisi = 1.9;
                    break;
                default:
                    aktiviteKatsayisi = 1.2; // Varsayılan olarak hareketsiz
                    break;
                
            }
            return aktiviteKatsayisi;
        }
        public static double vkiHesapla(double boy, double kilo)
        {
            if (boy <= 0) return 0;

            double boyMetre = boy / 100.0;
            // VKI = Kilo / (Boy * Boy)
            double vki = kilo / (boyMetre * boyMetre);

            return Math.Round(vki, 2); // Virgülden sonra 2 basamak tutması için
        }
        public static double bmh(bool cinsiyet, double kilo, double boy, int yas)
        {
            if (cinsiyet == true)
            {
                // Erkekler için Harris-Benedict formülü
                return Math.Round(66.5 + (13.75 * kilo) + (5.003 * boy) - (6.75 * yas), 2);
            }
            else
            {
                // Kadınlar için Harris-Benedict formülü
                return Math.Round(655.1 + (9.563 * kilo) + (1.85 * boy) - (4.676 * yas), 2);
            }
        }
        public static double gunlukkalori(bool cinsiyet, double kilo, double boy, int yas, double aktiviteKatsayisi, string amac)
        {
            // 1. Önce temel Bazal Metabolizma Hızını (BMH) hesaplıyoruz
            double anaBmh = bmh(cinsiyet, kilo, boy, yas);
            
            // 2. Aktivite katsayısı ile çarparak net kalori ihtiyacını buluyoruz
            double netIhtiyac = anaBmh * aktiviteKatsayisi;

            // 3. Kullanıcının amacına göre kalori bütçesini daraltıyor veya artırıyoruz
            double finalKalori = netIhtiyac;

            if (amac == "Yağ Yakmak / Kilo Vermek")
            {
                finalKalori = netIhtiyac - 500; // Güvenli kilo verimi için kaloriyi kıs
            }
            else if (amac == "Kas Yapmak / Kilo Almak")
            {
                finalKalori = netIhtiyac + 400; // Kas gelişimi için kalori fazlası oluştur
            }
            else if (amac == "Özel Gün / Hızlı Sonuç")
            {
                finalKalori = netIhtiyac - 700; // Daha agresif bir diyet planı
            }
            // "Kilo Koruma" veya eşleşmeyen bir durum olursa netIhtiyac aynen kalır.

            // 4. Sonucu virgülden sonra 2 basamak olacak şekilde yuvarlayıp dönüyoruz
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

                        // Eğer bugün hiç yemek girilmediyse SQL null döner, onu kontrol ediyoruz
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
            // 1. Kullanıcının limitini session'dan veya veri tabanından çekip Maximum değer yapıyoruz
            double hedefKalori = kullancicisession.GunlukKaloriHedefi;
            pbKalori.Maximum = (int)hedefKalori;
            pbKalori.ForeColor = System.Drawing.Color.Yellow;

            // 2. Bugün alınan kaloriyi yukarıda yazdığımız fonksiyondan çekiyoruz
            double alinanKalori = BugunAlinanToplamKalori();

            // 3. ProgressBar'ın doluluk oranını ayarlıyoruz (Hata vermemesi için sınırları koruyoruz)
            if (alinanKalori >= hedefKalori)
            {
                pbKalori.Value = pbKalori.Maximum; // Hedef aşıldıysa bar tamamen dolsun
                pbKalori.ForeColor = System.Drawing.Color.Red; // Hedef aşıldığında rengi kırmızı yap
            }
            else if (alinanKalori <= 0)
            {
                pbKalori.Value = 0; // Hiç kalori alınmadıysa boş dursun
            }
            else
            {
                pbKalori.Value = (int)alinanKalori; // Alınan kalori kadar dolsun
            }

            // Bilgilendirme label'ların varsa onları da burada güncelleyebilirsin
            lblKaloriOzet.Text = $"Alınan: {alinanKalori:N0} / Hedef: {hedefKalori:N0} kcal";
        }
        public static void GrafigiGuncelle(Chart chartGelisim)
        {
            // 1. Önce grafiği temizle (Üst üste binmemesi için)
            chartGelisim.Series["Kilo"].Points.Clear();

            string query = @"SELECT KayitTarihi, Kilo 
                     FROM KullaniciGelisimi 
                     WHERE KullaniciID = @uid 
                     ORDER BY KayitTarihi ASC"; // Eskiden yeniye doğru sırala

            using (SqlConnection con = new SqlConnection(connection.connectionstring))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@uid", kullancicisession.KullaniciID);

                try
                {
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        // X ekseni: Tarih, Y ekseni: Kilo
                        DateTime tarih = Convert.ToDateTime(dr["KayitTarihi"]);
                        double kilo = Convert.ToDouble(dr["Kilo"]);

                        // Grafiğe noktayı ekle
                        chartGelisim.Series["Kilo"].Points.AddXY(tarih.ToShortDateString(), kilo);
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
