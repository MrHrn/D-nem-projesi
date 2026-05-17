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
            chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray; // Hafif çizgiler
            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            chart1.Series["Kilo"].MarkerStyle = MarkerStyle.Circle; // Veri noktalarına yuvarlak koy
            chart1.Series["Kilo"].MarkerSize = 8;
            bilgileriniz.Items.Add("Kullanıcı ID'niz: " + kullancicisession.KullaniciID);
            bilgileriniz.Items.Add("Adınız: " + kullancicisession.Ad);
            bilgileriniz.Items.Add("Soyadınız: " + kullancicisession.Soyad);
            bilgileriniz.Items.Add("Yaşınız: " + kullancicisession.Yas);
            bilgileriniz.Items.Add("Doğum Tarihiniz: " + kullancicisession.DogumTarihi);
            bilgileriniz.Items.Add("Cinsiyetiniz: " + (kullancicisession.Cinsiyet ? "Erkek" : "Kadın"));
            bilgileriniz.Items.Add("Boyunuz: " + kullancicisession.Boy + " cm");
            bilgileriniz.Items.Add("Güncel Kilonuz: " + kullancicisession.GuncelKilo + " kg");
            bilgileriniz.Items.Add("Hedef Kilonuz: " + kullancicisession.HedefKilo + " kg");
            bilgileriniz.Items.Add("Vücut Kitle İndeksiniz: " + kullancicisession.VKI);
            bilgileriniz.Items.Add("Bazal Metabolizma Hızınız: " + kullancicisession.BMH);
            bilgileriniz.Items.Add("Aktivite Seviyeniz: " + kullancicisession.AktiviteSeviyesi);
            bilgileriniz.Items.Add("Amacınız: " + kullancicisession.amac);

            hesaplamalar.BugunAlinanToplamKalori();
            hesaplamalar.ProgressBarGuncelle(pbKalori, lblKaloriOzet);
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
                    label4.Text = $"Bugün {progressBar1.Value} bardak su içildi.";

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
            SqlConnection con = new SqlConnection(connection.connectionstring);
            SqlCommand cmd = new SqlCommand("update Kullanicilar set AktiviteSeviyesi = @AktiviteSeviyesi, AktiviteKatsayisi = @aktivitekatsayi where KullaniciID = @KullaniciID", con);
            cmd.Parameters.AddWithValue("@AktiviteSeviyesi", comboBox1.SelectedItem.ToString());
            double aktiviteKatsayisi = hesaplamalar.aktiviteKatsayisiHesapla(comboBox1.SelectedItem.ToString());
            cmd.Parameters.AddWithValue("@aktivitekatsayi", aktiviteKatsayisi);
            cmd.Parameters.AddWithValue("@KullaniciID", kullancicisession.KullaniciID);
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Aktivite seviyeniz başarıyla güncellendi!");
                // Güncellenen aktivite katsayısını kullancicisession'a da atayalım
                kullancicisession.AktiviteKatsayisi = aktiviteKatsayisi;
                bilgileriniz.Items[11] = "Aktivite Seviyeniz: " + comboBox1.SelectedItem.ToString(); // Bilgiler listesinde aktivite seviyesini güncelle
            }
            catch (Exception ex)
            {
                MessageBox.Show("Aktivite seviyeniz güncellenirken bir hata oluştu: " + ex.Message);
            }
            finally
            {
                con.Close();
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

                bilgileriniz.Items[7] = "GüncelKilonuz: " + kullancicisession.GuncelKilo.ToString() + " kg";
                bilgileriniz.Items[9] = "Vücut Kitle İndeksiniz: " + kullancicisession.VKI.ToString("F2");
                bilgileriniz.Items[10] = "Bazal Metabolizma Hızınız: " + kullancicisession.BMH.ToString("F2");
                
                hesaplamalar.ProgressBarGuncelle(pbKalori, lblKaloriOzet);
                hesaplamalar.BugunAlinanToplamKalori();
                hesaplamalar.GrafigiGuncelle(chart1);



            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }
    }
}
