using DiyetProgrami;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DiyetProgrami
{
    public partial class mainmenu : Form
    {
        private List<string> motivasyonSozleri = new List<string>()
        {
            "Bugün içtiğin su, yarınki enerjindir! 💧",
            "Hedefine ulaşmak için harika bir gün, vazgeçme! 💪",
            "Küçük adımlar, büyük sonuçlar doğurur. 🎯",
            "Kendine yaptığın yatırım, en karlı yatırımdır. 🌱",
            "Dün bitti. Bugün yeni bir başlangıç yapma şansın var! 🌅",
            "Sağlıklı beslenmek bir ceza değil, bedenine verdiğin bir ödüldür. 🍎",
            "Disiplin, isteklerinizle hedefleriniz arasındaki köprüdür. 🌉"
        };
        private void MotivasyonSozuGetir()
        {
            Random rnd = new Random();
            int rastgeleIndeks = rnd.Next(motivasyonSozleri.Count);
            label4.Text = motivasyonSozleri[rastgeleIndeks];
        }
        public mainmenu()
        {
            InitializeComponent();
        }

        private void hızlıÖğünBesinEkleButonuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ogunekle f = new ogunekle();
            f.Owner = this;
            f.Show();
        }

        // ListBox'taki yazıyı dinamik ve güvenli bir şekilde güncelleyen yardımcı metot
        private void ListBoxSatirGuncelle(string baslangicMetni, string yeniDeger)
        {
            int bulunanIndeks = -1;
            for (int i = 0; i < Bilgileriniz.Items.Count; i++)
            {
                if (Bilgileriniz.Items[i].ToString().StartsWith(baslangicMetni))
                {
                    bulunanIndeks = i;
                    break;
                }
            }

            if (bulunanIndeks != -1)
            {
                Bilgileriniz.Items[bulunanIndeks] = baslangicMetni + yeniDeger;
            }
            else
            {
                Bilgileriniz.Items.Add(baslangicMetni + yeniDeger);
            }
        }

        private void mainmenu_Load(object sender, EventArgs e)
        {
            MotivasyonSozuGetir();
            progressBar1.Value = kullancicisession.SuBardak;
            label2.Text = $"Ne kadar su içtiniz: Bugün {progressBar1.Value} bardak su içildi;";

            chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            chart1.Series["Kilo"].MarkerStyle = MarkerStyle.Circle;
            chart1.Series["Kilo"].MarkerSize = 8;

            Bilgileriniz.Items.Clear(); // Mükerrer kayıtları önlemek için önce temizleyelim
            Bilgileriniz.Items.Add("1. Kullanıcı ID'niz: " + kullancicisession.KullaniciID);
            Bilgileriniz.Items.Add("----------");
            Bilgileriniz.Items.Add("2. Adınız: " + kullancicisession.Ad);
            Bilgileriniz.Items.Add("----------");
            Bilgileriniz.Items.Add("3. Soyadınız: " + kullancicisession.Soyad);
            Bilgileriniz.Items.Add("----------");
            Bilgileriniz.Items.Add("4. Yaşınız: " + kullancicisession.Yas);
            Bilgileriniz.Items.Add("----------");
            Bilgileriniz.Items.Add("5. Doğum Tarihiniz: " + kullancicisession.DogumTarihi);
            Bilgileriniz.Items.Add("----------");
            Bilgileriniz.Items.Add("6. Cinsiyetiniz: " + (kullancicisession.Cinsiyet ? "Erkek" : "Kadın"));
            Bilgileriniz.Items.Add("----------");
            Bilgileriniz.Items.Add("7. Boyunuz: " + kullancicisession.Boy + " cm");
            Bilgileriniz.Items.Add("----------");
            Bilgileriniz.Items.Add("8. Güncel Kilonuz: " + kullancicisession.GuncelKilo + " kg");
            Bilgileriniz.Items.Add("----------");
            Bilgileriniz.Items.Add("9. Hedef Kilonuz: " + kullancicisession.HedefKilo + " kg");
            Bilgileriniz.Items.Add("----------");
            double vkiDegeri = kullancicisession.VKI;
            string vkiDurumu = "";
            Color durumRengi = Color.Black;

            if (vkiDegeri < 18.5)
            {
                vkiDurumu = "Zayıf";
                durumRengi = Color.Orange;
            }
            else if (vkiDegeri >= 18.5 && vkiDegeri < 24.9)
            {
                vkiDurumu = "Normal (İdeal)";
                durumRengi = Color.LightGreen;
            }
            else if (vkiDegeri >= 25 && vkiDegeri < 29.9)
            {
                vkiDurumu = "Fazla Kilolu";
                durumRengi = Color.DarkOrange;
            }
            else
            {
                vkiDurumu = "Obez";
                durumRengi = Color.Red;
            }

            // ListBox'a hem skoru hem de metinsel durumu şıkça yazıyoruz:
            Bilgileriniz.Items.Add($"10. Vücut Kitle İndeksiniz: {vkiDegeri} ({vkiDurumu})");

            // Eğer ekranda ayrı bir lblVkiDurum label'ı oluşturduysanız rengini de dinamik yapalım:
            label8.Text = $"VKI Durumu: {vkiDurumu}";
            label8.ForeColor = durumRengi;
            Bilgileriniz.Items.Add("----------");
            Bilgileriniz.Items.Add("11. Bazal Metabolizma Hızınız: " + kullancicisession.BMH);
            Bilgileriniz.Items.Add("----------");
            Bilgileriniz.Items.Add("12. Aktivite Seviyeniz: " + kullancicisession.AktiviteSeviyesi);
            Bilgileriniz.Items.Add("----------");
            Bilgileriniz.Items.Add("13. Amacınız: " + kullancicisession.amac);

            hesaplamalar.BugunAlinanToplamKalori();
            hesaplamalar.ProgressBarGuncelle(pbKalori, label1);
            hesaplamalar.GrafigiGuncelle(chart1);
        }

        private void kiloGüncelleBugünTartıldımButonuToolStripMenuItem_Click(object sender, EventArgs e) { }

        // SU EKLEME BUTONU
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (progressBar1.Value < progressBar1.Maximum)
                {
                    kullancicisession.SuBardak += 1;
                    progressBar1.Value = kullancicisession.SuBardak;
                    label2.Text = $"Ne kadar su içtiniz: Bugün {progressBar1.Value} bardak su içildi.";

                    using (SqlConnection con = new SqlConnection(connection.connectionstring))
                    {
                        string query = "UPDATE Kullanicilar SET SuBardak = @BugunIcilenSuMiktari, SuSonGuncellemeTarihi = CAST(GETDATE() AS DATE) WHERE KullaniciID = @KullaniciID";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@BugunIcilenSuMiktari", kullancicisession.SuBardak);
                            cmd.Parameters.AddWithValue("@KullaniciID", kullancicisession.KullaniciID);

                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Tebrikler! Günlük su hedefine ulaştın.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Su miktarı güncellenirken bir hata oluştu: " + ex.Message);
            }
        }

        private void progressBar1_Click(object sender, EventArgs e) { }

        private void s_MouseEnter(object sender, EventArgs e)
        {
            Button dinamikButon = (Button)sender;
            if (dinamikButon.Name == "btnKaydet") dinamikButon.BackColor = Color.DarkGreen;
            else if (dinamikButon.Name == "btnGuncelle") dinamikButon.BackColor = Color.SteelBlue;
            else if (dinamikButon.Name == "btnSil") dinamikButon.BackColor = Color.Firebrick;
        }

        private void s_MouseLeave(object sender, EventArgs e)
        {
            Button dinamikButon = (Button)sender;
            if (dinamikButon.Name == "btnKaydet") dinamikButon.BackColor = Color.FromArgb(128, 255, 255, 255);
            else if (dinamikButon.Name == "btnGuncelle") dinamikButon.BackColor = Color.DodgerBlue;
            else if (dinamikButon.Name == "btnSil") dinamikButon.BackColor = Color.IndianRed;
        }

        // AKTİVİTE SEVİYESİ GÜNCELLEME BUTONU
        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir aktivite seviyesi seçin.");
                return;
            }

            try
            {
                string secilenAktivite = comboBox1.SelectedItem.ToString();
                double aktiviteKatsayisi = hesaplamalar.aktiviteKatsayisiHesapla(secilenAktivite);
                kullancicisession.AktiviteKatsayisi = aktiviteKatsayisi;
                MessageBox.Show("Seçilen aktivite seviyesine göre katsayı: " + aktiviteKatsayisi);
                // Hesaplama esnasında session'daki en güncel amaç bilgisini gönderiyoruz
                kullancicisession.GunlukKaloriHedefi = hesaplamalar.gunlukkalori(
                    kullancicisession.Cinsiyet,
                    kullancicisession.GuncelKilo,
                    kullancicisession.Boy,
                    kullancicisession.Yas,
                    aktiviteKatsayisi,
                    kullancicisession.amac
                );

                MessageBox.Show("Aktivite seviyeniz güncelleniyor, lütfen bekleyiniz...\nGünlük kalori ihtiyacınız: " + kullancicisession.GunlukKaloriHedefi);

                using (SqlConnection con = new SqlConnection(connection.connectionstring))
                {
                    string query = "UPDATE Kullanicilar SET AktiviteSeviyesi = @AktiviteSeviyesi, GunlukKaloriIhtiyaci = @GunlukKaloriIhtiyaci, AktiviteKatsayisi = @aktivitekatsayi WHERE KullaniciID = @KullaniciID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@AktiviteSeviyesi", secilenAktivite);
                        cmd.Parameters.AddWithValue("@GunlukKaloriIhtiyaci", kullancicisession.GunlukKaloriHedefi);
                        cmd.Parameters.AddWithValue("@aktivitekatsayi", aktiviteKatsayisi);
                        cmd.Parameters.AddWithValue("@KullaniciID", kullancicisession.KullaniciID);

                        con.Open();
                        int etkilenenSatir = cmd.ExecuteNonQuery();

                        if (etkilenenSatir > 0)
                        {
                            MessageBox.Show("Aktivite seviyeniz başarıyla güncellendi!");

                            // İŞTE BURASI CRITICAL: Session'ları eksiksiz dolduruyoruz
                            kullancicisession.AktiviteKatsayisi = aktiviteKatsayisi;
                            kullancicisession.AktiviteSeviyesi = secilenAktivite;

                            ListBoxSatirGuncelle("12. Aktivite Seviyeniz: ", secilenAktivite);
                            hesaplamalar.ProgressBarGuncelle(pbKalori, label1);
                        }
                        else
                        {
                            MessageBox.Show("Güncelleme başarısız. Kullanıcı ID bulunamadı.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }

        // KİLO GÜNCELLEME BUTONU
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Lütfen geçerli bir kilo değeri girin.");
                return;
            }

            try
            {
                double yeniKilo = Convert.ToDouble(textBox1.Text);
                double guncelvki = hesaplamalar.vkiHesapla(kullancicisession.Boy, yeniKilo);
                double bmh = hesaplamalar.bmh(kullancicisession.Cinsiyet, yeniKilo, kullancicisession.Boy, kullancicisession.Yas);

                // Kilo güncellenirken mevcut güncel aktivite katsayısını kullanıyoruz
                double gunlukkalori = hesaplamalar.gunlukkalori(
                    kullancicisession.Cinsiyet,
                    yeniKilo,
                    kullancicisession.Boy,
                    kullancicisession.Yas,
                    kullancicisession.AktiviteKatsayisi,
                    kullancicisession.amac
                );

                string query = "UPDATE Kullanicilar SET GuncelKilo = @GuncelKilo, VKI = @VKI, BMH = @BMH, GunlukKaloriIhtiyaci = @GunlukKaloriIhtiyaci WHERE KullaniciID = @KullaniciID";
                string query2 = "INSERT INTO KullaniciGelisimi(KullaniciID, KayitTarihi, Kilo, VKI, Boy) VALUES (@KullaniciID, GETDATE(), @Kilo, @VKI, @Boy)";

                using (SqlConnection con = new SqlConnection(connection.connectionstring))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@GuncelKilo", yeniKilo);
                        cmd.Parameters.AddWithValue("@VKI", guncelvki);
                        cmd.Parameters.AddWithValue("@BMH", bmh);
                        cmd.Parameters.AddWithValue("@GunlukKaloriIhtiyaci", gunlukkalori);
                        cmd.Parameters.AddWithValue("@KullaniciID", kullancicisession.KullaniciID);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd2 = new SqlCommand(query2, con))
                    {
                        cmd2.Parameters.AddWithValue("@KullaniciID", kullancicisession.KullaniciID);
                        cmd2.Parameters.AddWithValue("@Kilo", yeniKilo);
                        cmd2.Parameters.AddWithValue("@VKI", guncelvki);
                        cmd2.Parameters.AddWithValue("@Boy", kullancicisession.Boy);
                        cmd2.ExecuteNonQuery();
                    }
                }

                // Session senkronizasyonu
                kullancicisession.GuncelKilo = yeniKilo;
                kullancicisession.VKI = guncelvki;
                kullancicisession.BMH = bmh;
                kullancicisession.GunlukKaloriHedefi = gunlukkalori;

                MessageBox.Show("Kilo ve VKI başarıyla güncellendi!");

                // Kilo güncelleme butonunun (button1_Click_1) sonlarındaki ListBox satırları:
                ListBoxSatirGuncelle("8. Güncel Kilonuz: ", yeniKilo.ToString() + " kg");

                // VKI DURUMUNU YENİDEN HESAPLAMA (Kilo butonunun içine ekleyin)
                string yeniVkiDurumu = "";
                Color yeniDurumRengi = Color.Black;

                if (guncelvki < 18.5) { yeniVkiDurumu = "Zayıf"; yeniDurumRengi = Color.Orange; }
                else if (guncelvki >= 18.5 && guncelvki < 24.9) { yeniVkiDurumu = "Normal (İdeal)"; yeniDurumRengi = Color.LightGreen; }
                else if (guncelvki >= 25 && guncelvki < 29.9) { yeniVkiDurumu = "Fazla Kilolu"; yeniDurumRengi = Color.DarkOrange; }
                else { yeniVkiDurumu = "Obez"; yeniDurumRengi = Color.Red; }

                ListBoxSatirGuncelle("10. Vücut Kitle İndeksiniz: ", $"{guncelvki.ToString("F2")} ({yeniVkiDurumu})");
                ListBoxSatirGuncelle("11. Bazal Metabolizma Hızınız: ", bmh.ToString("F2"));

                // Formdaki label'ı da anlık güncelle:
                label8.Text = $"VKI Durumu: {yeniVkiDurumu}";
                label8.ForeColor = yeniDurumRengi;

                hesaplamalar.ProgressBarGuncelle(pbKalori, label1);
                hesaplamalar.GrafigiGuncelle(chart1);

                hesaplamalar.ProgressBarGuncelle(pbKalori, label1);
                hesaplamalar.GrafigiGuncelle(chart1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }

        private void chart1_Click(object sender, EventArgs e) { }

        // AMAÇ GÜNCELLEME BUTONU
        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir hedef/amaç seçin.");
                return;
            }

            try
            {
                string secilenAmac = comboBox2.SelectedItem.ToString();
                double mevcutAktiviteKatsayisi = kullancicisession.AktiviteKatsayisi;

                if (mevcutAktiviteKatsayisi <= 0)
                {
                    mevcutAktiviteKatsayisi = hesaplamalar.aktiviteKatsayisiHesapla(kullancicisession.AktiviteSeviyesi);
                }

                // 1. Yeni kaloriyi hesapla
                double hesaplananKalori = hesaplamalar.gunlukkalori(
                    kullancicisession.Cinsiyet,
                    kullancicisession.GuncelKilo,
                    kullancicisession.Boy,
                    kullancicisession.Yas,
                    mevcutAktiviteKatsayisi,
                    secilenAmac
                );

                // 2. İlk önce Session nesnelerini mutlak suretle güncelle
                kullancicisession.GunlukKaloriHedefi = hesaplananKalori;
                kullancicisession.amac = secilenAmac;

                // 3. Veritabanı güncellemesi
                using (SqlConnection con = new SqlConnection(connection.connectionstring))
                {
                    string query = "UPDATE Kullanicilar SET Amac = @amac, GunlukKaloriIhtiyaci = @GunlukKaloriIhtiyaci WHERE KullaniciID = @KullaniciID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@amac", secilenAmac);
                        cmd.Parameters.AddWithValue("@GunlukKaloriIhtiyaci", hesaplananKalori);
                        cmd.Parameters.AddWithValue("@KullaniciID", kullancicisession.KullaniciID);

                        con.Open();
                        int etkilenenSatir = cmd.ExecuteNonQuery();

                        if (etkilenenSatir > 0)
                        {
                            MessageBox.Show("Amacınız başarıyla güncellendi!\nYeni Kalori Hedefiniz: " + hesaplananKalori);

                            // 4. Arayüzü en güncel session verileriyle yenile
                            ListBoxSatirGuncelle("13. Amacınız: ", secilenAmac);
                            hesaplamalar.ProgressBarGuncelle(pbKalori, label1);
                        }
                        else
                        {
                            MessageBox.Show("Güncelleme başarısız. Kullanıcı bulunamadı.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Amacınız güncellenirken bir hata oluştu: " + ex.Message);
            }
        }
    }
}