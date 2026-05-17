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

namespace DiyetProgrami
{
    public partial class ogunekle : Form
    {
        public ogunekle()
        {
            InitializeComponent();
        }
        void listele()
        {
            try 
            {
                string query = @"SELECT
                k.Ad + ' ' + k.Soyad AS[Kullanıcı],
                o.OgunAdi AS[Öğün Türü],
                o.Tarih AS[Yeme Tarihi],
                b.BesinAdi AS[Yenilen Besin],
                od.MiktarGram AS[Miktar(gr)],
                b.Kalori AS[100gr Kalorisi],
                --Toplam kaloriyi hesaplıyoruz: (Gram * Kalori) / 100
                ROUND((od.MiktarGram * b.Kalori) / 100.0, 2) AS[Alınan Toplam Kalori],
                b.Protein,
                b.Karbonhidrat,
                b.Yag
            FROM Ogunler o
            --1.Öğünleri Kullanıcılarla bağlıyoruz
            JOIN Kullanicilar k ON o.KullaniciID = k.KullaniciID
            -- 2.Öğünleri detaylarıyla(hangi besinler var) bağlıyoruz
            JOIN OgunDetaylari od ON o.OgunID = od.OgunID
            -- 3.Detaylardaki BesinID'yi Besinler tablosuyla bağlıyoruz
            JOIN Besinler b ON od.BesinID = b.BesinID
            WHERE o.KullaniciID = @p
            ORDER BY o.Tarih DESC ";
                SqlConnection con = new SqlConnection(connection.connectionstring);
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@p", kullancicisession.KullaniciID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler getirilirken hata oluştu: " + ex.Message);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
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
                else if (item is ComboBox)
                {
                    if (item.Text == "")
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
            using (SqlConnection con = new SqlConnection(connection.connectionstring))
            {
                con.Open();
                // SQL Transaction kullanarak her iki tabloya kaydın "ya hep ya hiç" mantığıyla yapılmasını sağlıyoruz
                SqlTransaction tra = con.BeginTransaction();

                try
                {
                    string ogunQuery = @"INSERT INTO Ogunler (KullaniciID, OgunAdi, Tarih) 
                                 OUTPUT INSERTED.OgunID 
                                 VALUES (@uid, @adi, @tarih)";

                    SqlCommand cmdOgun = new SqlCommand(ogunQuery, con, tra);
                    cmdOgun.Parameters.AddWithValue("@uid", kullancicisession.KullaniciID);
                    cmdOgun.Parameters.AddWithValue("@adi", ogun.SelectedItem.ToString()); // Örn: Kahvaltı
                    cmdOgun.Parameters.AddWithValue("@tarih", DateTime.Now);

                    // ExecuteScalar kullanarak oluşan yeni OgunID'yi değişkene atıyoruz
                    int yeniOgunID = (int)cmdOgun.ExecuteScalar();


                    // 2. ADIM: OgunDetaylari tablosuna besin detayını ekliyoruz.
                    string detayQuery = @"INSERT INTO OgunDetaylari (OgunID, BesinID, MiktarGram) 
                                  VALUES (@oid, @bid, @gram)";

                    SqlCommand cmdDetay = new SqlCommand(detayQuery, con, tra);
                    cmdDetay.Parameters.AddWithValue("@oid", yeniOgunID); // Az önce aldığımız ID
                    cmdDetay.Parameters.AddWithValue("@bid", besin.SelectedValue); // Seçilen besinin ID'si
                    cmdDetay.Parameters.AddWithValue("@gram", Convert.ToDouble(textBox1.Text));

                    cmdDetay.ExecuteNonQuery();

                    // Her şey başarılıysa işlemleri onayla
                    tra.Commit();
                    MessageBox.Show("Öğün ve besin başarıyla kaydedildi!");
                    hesaplamalar.ProgressBarGuncelle(((mainmenu)this.Owner).pbKalori, ((mainmenu)this.Owner).lblKaloriOzet);
                    
                    listele();


                }
                catch (Exception ex)
                {
                    // Bir hata olursa yapılan tüm kayıtları geri al (Veri bütünlüğü için şart)
                    tra.Rollback();
                    MessageBox.Show("Kayıt sırasında hata oluştu: " + ex.Message);
                }
            }
        }

        private void ogunekle_Load(object sender, EventArgs e)
        {
            listele();
            string selectedBesin = "select * from Besinler";
            SqlConnection con = new SqlConnection(connection.connectionstring);
            SqlCommand cmd = new SqlCommand(selectedBesin, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            besin.DisplayMember = dt.Columns["BesinAdi"].ToString();
            besin.ValueMember = dt.Columns["BesinID"].ToString();
            besin.DataSource = dt;
        }
    }
}
