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

        private void mainmenu_Load(object sender, EventArgs e)
        {
            progressBar1.Value = kullancicisession.SuBardak; // Session'daki su miktarını progress bar'a yansıt
            label2.Text = $"Ne kadar su içtini: Bugün {progressBar1.Value} bardak su içildi;";
            chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray; // Hafif çizgiler
            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            chart1.Series["Kilo"].MarkerStyle = MarkerStyle.Circle; // Veri noktalarına yuvarlak koy
            chart1.Series["Kilo"].MarkerSize = 8;
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
            Bilgileriniz.Items.Add("10. Vücut Kitle İndeksiniz: " + kullancicisession.VKI);
            Bilgileriniz.Items.Add("----------");
            Bilgileriniz.Items.Add("11. Bazal Metabolizma Hızınız: " + kullancicisession.BMH);
            Bilgileriniz.Items.Add("----------");
            Bilgileriniz.Items.Add("12. Aktivite Seviyeniz: " + kullancicisession.AktiviteSeviyesi);
            Bilgileriniz.Items.Add("----------");
            Bilgileriniz.Items.Add("13. Amacınız: " + kullancicisession.amac);

            hesaplamalar.BugunAlinanToplamKalori();
            hesaplamalar.ProgressBarGuncelle(pbKalori, label1);
        }

        private void kiloGüncelleBugünTartıldımButonuToolStripMenuItem_Click(object sender, EventArgs e)
        {
       
        }

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
                            int etkilenenSatir = cmd.ExecuteNonQuery();

                            // Teşhis için: Eğer veri tabanında hiçbir satır güncellenemediyse (ID bulunamadıysa) uyarır
                            if (etkilenenSatir == 0)
                            {
                                MessageBox.Show("Uyarı: Veri tabanında bu KullaniciID'ye ait kayıt bulunamadı, güncelleme yapılamadı.");
                            }
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

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void s_MouseEnter(object sender, EventArgs e)
        {

            Button dinamikButon = (Button)sender;

            // Butonun ismine göre (veya o anki rengine göre) rengini koyulaştırıyoruz
            if (dinamikButon.Name == "btnKaydet")
            {
                dinamikButon.BackColor = Color.DarkGreen; // SeaGreen'den DarkGreen'e geçiş
            }
            else if (dinamikButon.Name == "btnGuncelle")
            {
                dinamikButon.BackColor = Color.SteelBlue; // DodgerBlue'dan SteelBlue'ya geçiş
            }
            else if (dinamikButon.Name == "btnSil")
            {
                dinamikButon.BackColor = Color.Firebrick; // IndianRed'den Firebrick'e geçiş
            }
        }

        private void s_MouseLeave(object sender, EventArgs e)
        {

            Button dinamikButon = (Button)sender;

            // Fare gidince butonları orijinal tasarım renklerine geri döndürüyoruz
            if (dinamikButon.Name == "btnKaydet")
            {
                dinamikButon.BackColor = Color.FromArgb(128, 255, 255, 255);
            }
            else if (dinamikButon.Name == "btnGuncelle")
            {
                dinamikButon.BackColor = Color.DodgerBlue;
            }
            else if (dinamikButon.Name == "btnSil")
            {
                dinamikButon.BackColor = Color.IndianRed;
            }
        }

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

                // 1. Hesaplamaları veritabanı bağlantısını açmadan önce yapın (Bağlantı süresini kısa tutmak için)
                double aktiviteKatsayisi = hesaplamalar.aktiviteKatsayisiHesapla(secilenAktivite);

                kullancicisession.GunlukKaloriHedefi = hesaplamalar.gunlukkalori(
                    kullancicisession.Cinsiyet,
                    kullancicisession.GuncelKilo,
                    kullancicisession.Boy,
                    kullancicisession.Yas,
                    aktiviteKatsayisi,
                    kullancicisession.amac
                );

                MessageBox.Show("Aktivite seviyeniz güncelleniyor, lütfen bekleyiniz...\nGünlük kalori ihtiyacınız hesaplanıyor... Günlük kalori ihtiyacınız: " + kullancicisession.GunlukKaloriHedefi);

                // 2. 'using' kullanarak bağlantı yönetimini garanti altına alın (Finally bloğuna gerek kalmaz)
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
                        int etkilenenSatir = cmd.ExecuteNonQuery(); // Kaç satırın güncellendiğini döner

                        if (etkilenenSatir > 0)
                        {
                            MessageBox.Show("Aktivite seviyeniz başarıyla güncellendi!");

                            // 3. Oturum ve Arayüz Güncellemeleri
                            kullancicisession.AktiviteKatsayisi = aktiviteKatsayisi;

                            // Liste indeks kontrolü (Hata almamak için güvenli yöntem)
                            if (Bilgileriniz.Items.Count > 11)
                            {
                                Bilgileriniz.Items[22] = "12. Aktivite Seviyeniz: " + secilenAktivite;
                            }
                            else
                            {
                                // Eğer 11. indeks yoksa listeye yeni ekle veya hata yönetimini sağla
                                Bilgileriniz.Items.Add("12. Aktivite Seviyeniz: " + secilenAktivite);
                            }

                            hesaplamalar.ProgressBarGuncelle(pbKalori, label1);
                        }
                        else
                        {
                            MessageBox.Show("Güncelleme başarısız. Kullanıcı ID bulunamadı.");
                        }
                    }
                } // con.Close() işlemi otomatik olarak burada yapılır.
            }
            catch (Exception ex)
            {
                // Hatanın tam olarak nerede çıktığını anlamak için ex.Message yerine ex.ToString() kullanabilirsiniz.
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            hesaplamalar.GrafigiGuncelle(chart1);
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Lütfen geçerli bir kilo değeri girin.");
                return;
            }
            SqlConnection con = new SqlConnection(connection.connectionstring);
            string query = "update Kullanicilar set GuncelKilo = @GuncelKilo, VKI = @VKI, BMH = @BMH, GunlukKaloriIhtiyaci = @GunlukKaloriIhtiyaci where KullaniciID = @KullaniciID";
            string query2 = "insert into KullaniciGelisimi(KullaniciID, KayitTarihi, Kilo, VKI, Boy) values (@KullaniciID, GETDATE(), @Kilo, @VKI, @Boy)";

            try
            {
                
                con.Open();

                // 3. Ana Tabloyu (Kullanicilar) Güncelleme
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@GuncelKilo", textBox1.Text);
                double guncelvkı = hesaplamalar.vkiHesapla(kullancicisession.Boy, Convert.ToDouble(textBox1.Text));
                double bmh = hesaplamalar.bmh(kullancicisession.Cinsiyet, Convert.ToDouble(textBox1.Text), kullancicisession.Boy, kullancicisession.Yas);
                cmd.Parameters.AddWithValue("@VKI", guncelvkı);
                cmd.Parameters.AddWithValue("@BMH", bmh);
                double gunlukkalori = hesaplamalar.gunlukkalori(kullancicisession.Cinsiyet, Convert.ToDouble(textBox1.Text), kullancicisession.Boy, kullancicisession.Yas, kullancicisession.AktiviteKatsayisi,kullancicisession.amac);
                cmd.Parameters.AddWithValue("@GunlukKaloriIhtiyaci", gunlukkalori);
                cmd.Parameters.AddWithValue("@KullaniciID", kullancicisession.KullaniciID);
                cmd.ExecuteNonQuery();

                // 4. Gelişim Tablosuna (KullaniciGelisimi) Yeni Kayıt Ekleme
                SqlCommand cmd2 = new SqlCommand(query2, con);
                cmd2.Parameters.AddWithValue("@KullaniciID", kullancicisession.KullaniciID);
                cmd2.Parameters.AddWithValue("@Kilo", textBox1.Text);
                cmd2.Parameters.AddWithValue("@VKI", guncelvkı);
                cmd2.Parameters.AddWithValue("@Boy", kullancicisession.Boy);
                cmd2.ExecuteNonQuery();

                con.Close();

                // 5. Session'ı güncelle (Ana sayfadaki ProgressBar'ın değişmesi için şart)
                kullancicisession.GuncelKilo = Convert.ToDouble(textBox1.Text);
                kullancicisession.VKI = guncelvkı;
                kullancicisession.BMH = bmh;
                kullancicisession.GunlukKaloriHedefi = gunlukkalori;

                MessageBox.Show("Kilo ve VKI başarıyla güncellendi!");

                Bilgileriniz.Items[7] = "GüncelKilonuz: " + kullancicisession.GuncelKilo.ToString() + " kg";
                Bilgileriniz.Items[9] = "Vücut Kitle İndeksiniz: " + kullancicisession.VKI.ToString("F2");
                Bilgileriniz.Items[10] = "Bazal Metabolizma Hızınız: " + kullancicisession.BMH.ToString("F2");
                
                hesaplamalar.ProgressBarGuncelle(pbKalori, label1);
                hesaplamalar.BugunAlinanToplamKalori();
                hesaplamalar.GrafigiGuncelle(chart1);



            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir aktivite seviyesi seçin.");
                return;
            }
            try
            {
                SqlConnection con = new SqlConnection(connection.connectionstring);
                SqlCommand cmd = new SqlCommand("update Kullanicilar set Amac = amac, GunlukKaloriIhtiyaci = @GunlukKaloriIhtiyaci where KullaniciID = @KullaniciID", con);
                
                kullancicisession.amac = comboBox2.SelectedItem.ToString();
                double aktiviteKatsayisi = hesaplamalar.aktiviteKatsayisiHesapla(comboBox2.SelectedItem.ToString());
                kullancicisession.GunlukKaloriHedefi = hesaplamalar.gunlukkalori(kullancicisession.Cinsiyet, kullancicisession.GuncelKilo, kullancicisession.Boy, kullancicisession.Yas, aktiviteKatsayisi, kullancicisession.amac);
                cmd.Parameters.AddWithValue("@amac", kullancicisession.amac);
                cmd.Parameters.AddWithValue("@AktiviteSeviyesi", comboBox2.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@GunlukKaloriIhtiyaci", kullancicisession.GunlukKaloriHedefi);
                cmd.Parameters.AddWithValue("@aktivitekatsayi", aktiviteKatsayisi);
                cmd.Parameters.AddWithValue("@KullaniciID", kullancicisession.KullaniciID);
                try
                {
                    MessageBox.Show("Amacınız güncelleniyor, lütfen bekleyiniz..." + "\n Günlük kalori ihtiyacınız hesaplanıyor... Günlük kalori ihtiyacınız: " + kullancicisession.GunlukKaloriHedefi);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Amacınız başarıyla güncellendi!");
                    // Güncellenen aktivite katsayısını kullancicisession'a da atayalım
                    Bilgileriniz.Items[24] = "13. Kullanım Amacınız: " + comboBox2.SelectedItem.ToString(); // Bilgiler listesinde aktivite seviyesini güncelle
                    hesaplamalar.ProgressBarGuncelle(pbKalori, label1); // Kalori ihtiyacını güncellemek için progress bar'ı da güncelleyelim
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Amacınız güncellenirken bir hata oluştu: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Amacınız güncellenirken bir hata oluştu: " + ex.Message);
            }
        }
    }
}
