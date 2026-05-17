using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace DiyetProgrami
{
    public partial class kayito : Form
    {
        public kayito()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 1. Dolu/Boş Kontrolü
            bool bosAlanVarMi = false;
            foreach (Control item in this.Controls)
            {
                if (item is TextBox)
                {
                    if (string.IsNullOrWhiteSpace(item.Text))
                    {
                        item.BackColor = Color.Red;
                        bosAlanVarMi = true;
                    }
                    else
                    {
                        item.BackColor = Color.White;
                    }
                }
            }

            if (bosAlanVarMi)
            {
                MessageBox.Show("Lütfen kırmızı alanları doldurunuz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Form Verilerini Değişkenlere Atama
            string Ad = textBox1.Text;
            string Soyad = textBox2.Text;
            bool Cinsiyet = radioButton1.Checked; // Kısa atama
            DateTime DogumTarihi = dateTimePicker1.Value;

            double Boy = double.Parse(textBox5.Text);
            double GuncelKilo = double.Parse(textBox6.Text);
            double HedefKilo = double.Parse(textBox7.Text);

            // 3. Yeni Belirlediğimiz Kategorilere Göre Seçim Yönetimi
            string AktiviteSeviyesi = comboBox1.SelectedItem != null ? comboBox1.SelectedItem.ToString() : "Yürüyüşçü";
            string Amac = comboBox2.SelectedItem != null ? comboBox2.SelectedItem.ToString() : "Yağ Yakmak / Kilo Vermek";

            int yas = DateTime.Now.Year - DogumTarihi.Year;

            // 4. Hesaplamaları Birer Kez Yapıp Değişkenlerde Tutuyoruz (Performans İçin)
            double BMH = hesaplamalar.bmh(Cinsiyet, GuncelKilo, Boy, yas);
            double VKI = hesaplamalar.vkiHesapla(Boy, GuncelKilo);
            double aktiviteKatsayisi = hesaplamalar.aktiviteKatsayisiHesapla(AktiviteSeviyesi);

            // Bir önceki adımda düzenlediğimiz nihai metodu çağırıyoruz
            double gunlukKaloriIhtiyaci = hesaplamalar.gunlukkalori(Cinsiyet, GuncelKilo, Boy, yas, aktiviteKatsayisi, Amac);

            // 5. Veri Tabanı Kayıt İşlemi (using bloğu ile güvenli bağlantı)
            string query = @"INSERT INTO Kullanicilar 
                            (Ad, Soyad, Cinsiyet, DogumTarihi, Boy, GuncelKilo, HedefKilo, KayitTarihi, VKI, BMH, GunlukKaloriIhtiyaci, AktiviteSeviyesi, AktiviteKatsayisi, Amac) 
                            VALUES 
                            (@Ad, @Soyad, @Cinsiyet, @DogumTarihi, @Boy, @GuncelKilo, @HedefKilo, GETDATE(), @VKI, @BMH, @GunlukKalori, @AktiviteSeviyesi, @AktiviteKatsayisi, @Amac)";

            try
            {
                using (SqlConnection c = new SqlConnection(connection.connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(query, c))
                    {
                        cmd.Parameters.AddWithValue("@Ad", Ad);
                        cmd.Parameters.AddWithValue("@Soyad", Soyad);
                        cmd.Parameters.AddWithValue("@Cinsiyet", Cinsiyet);
                        cmd.Parameters.AddWithValue("@DogumTarihi", DogumTarihi);
                        cmd.Parameters.AddWithValue("@Boy", Boy);
                        cmd.Parameters.AddWithValue("@GuncelKilo", GuncelKilo);
                        cmd.Parameters.AddWithValue("@HedefKilo", HedefKilo);
                        cmd.Parameters.AddWithValue("@VKI", VKI);
                        cmd.Parameters.AddWithValue("@BMH", BMH);
                        cmd.Parameters.AddWithValue("@GunlukKalori", gunlukKaloriIhtiyaci);
                        cmd.Parameters.AddWithValue("@AktiviteSeviyesi", AktiviteSeviyesi);
                        cmd.Parameters.AddWithValue("@AktiviteKatsayisi", aktiviteKatsayisi);
                        cmd.Parameters.AddWithValue("@Amac", Amac);

                        c.Open();
                        cmd.ExecuteNonQuery();
                        
                        int etkilenenSatir = cmd.ExecuteNonQuery(); // Geriye dönen sayıyı alıyoruz
                        c.Close();

                        if (etkilenenSatir == 0)
                        {
                            // Ekrana bu mesaj geliyorsa SQL çalıştı ama değiştirecek satır bulamadı!
                            MessageBox.Show("Sorgu çalıştı ama veri tabanında HİÇBİR SATIR GÜNCELLENMEDİ! WHERE şartını kontrol et.");
                        }
                        else
                        {
                            MessageBox.Show($"{etkilenenSatir} adet satır başarıyla güncellendi.");
                        }
                    }
                }

                MessageBox.Show("Kayıt Başarılı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MessageBox.Show($"Ad: {Ad}\nSoyad: {Soyad}\nCinsiyet: {(Cinsiyet ? "Erkek" : "Kadın")}\nYaş: {yas}\nBoy: {Boy} cm\nGüncel Kilo: {GuncelKilo} kg\nHedef Kilo: {HedefKilo} kg\nAktivite Seviyesi: {AktiviteSeviyesi}\nAktivite Katsayısı: {aktiviteKatsayisi}\nBMH: {BMH}\nVKI: {VKI}\nGünlük Kalori Hedefi: {gunlukKaloriIhtiyaci} kcal");

                this.Close(); // Kayıttan sonra pencereyi kapatıyoruz
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri tabanına kaydedilirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- Efekt Metotlarında sadece sender eşleşmelerini düzelttik ---
        private void button1_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Button dinamikButon)
            {
                if (dinamikButon.Name == "button1" || dinamikButon.Name == "btnKaydet") // Butonunun ismi button1 göründüğü için iki ihtimali de ekledim
                {
                    dinamikButon.BackColor = Color.DarkGreen;
                }
                else if (dinamikButon.Name == "btnGuncelle")
                {
                    dinamikButon.BackColor = Color.SteelBlue;
                }
                else if (dinamikButon.Name == "btnSil")
                {
                    dinamikButon.BackColor = Color.Firebrick;
                }
            }
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Button dinamikButon)
            {
                if (dinamikButon.Name == "button1" || dinamikButon.Name == "btnKaydet")
                {
                    dinamikButon.BackColor = Color.FromArgb(128, 255, 128);
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
        }
    }
}