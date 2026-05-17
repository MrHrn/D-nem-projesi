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
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
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
            }

            if (bosAlanVarMi)
            {
                MessageBox.Show("Lütfen kırmızı alanları doldurunuz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int id = int.Parse(textBox3.Text);
                string ad = textBox1.Text;
                string soyad = textBox2.Text;
                SqlConnection c = new SqlConnection(connection.connectionstring);
                SqlCommand cmd = new SqlCommand("select count(*) from Kullanicilar where KullaniciID = @p and Ad = @p1 and Soyad = @p2", c);
                cmd.Parameters.AddWithValue("@p", id);
                cmd.Parameters.AddWithValue("@p1", ad);
                cmd.Parameters.AddWithValue("@p2", soyad);
            try
            {
                c.Open();
                int sayi = (int)cmd.ExecuteScalar();
                if (sayi > 0)
                {
                    MessageBox.Show("Giriş Başarılı");
                    string sql = "select * from Kullanicilar where KullaniciID = @p and Ad = @p1 and Soyad = @p2";
                    SqlCommand cmd2 = new SqlCommand(sql, c);
                    cmd2.Parameters.AddWithValue("@p", id);
                    cmd2.Parameters.AddWithValue("@p1", ad);
                    cmd2.Parameters.AddWithValue("@p2", soyad);
                    SqlDataReader dr = cmd2.ExecuteReader();
                    while (dr.Read())
                    {
                        kullancicisession.KullaniciID = Convert.ToInt32(dr["KullaniciID"]);
                        kullancicisession.Ad = dr["Ad"].ToString();
                        kullancicisession.Soyad = dr["Soyad"].ToString();
                        kullancicisession.Cinsiyet = Convert.ToBoolean(dr["Cinsiyet"]);

                        kullancicisession.Yas = Convert.ToInt32(dr["Yas"]);
                        kullancicisession.DogumTarihi = Convert.ToDateTime(dr["DogumTarihi"]);

                        kullancicisession.Boy = Convert.ToDouble(dr["Boy"]);
                        kullancicisession.GuncelKilo = Convert.ToDouble(dr["GuncelKilo"]);
                        kullancicisession.HedefKilo = Convert.ToDouble(dr["HedefKilo"]);

                        kullancicisession.VKI = Convert.ToDouble(dr["VKI"]);
                        kullancicisession.BMH = Convert.ToDouble(dr["BMH"]);
                        kullancicisession.AktiviteKatsayisi = Convert.ToDouble(dr["AktiviteKatsayisi"]);

                        kullancicisession.AktiviteSeviyesi = dr["AktiviteSeviyesi"].ToString();
                        kullancicisession.GunlukKaloriHedefi = Convert.ToDouble(dr["GunlukKaloriIhtiyaci"]);
                        kullancicisession.KayitTarihi = Convert.ToDateTime(dr["KayitTarihi"]);
                        kullancicisession.amac = dr["Amac"].ToString();
                        kullancicisession.SuBardak = Convert.ToInt32(dr["SuBardak"]);

                    }
                        mainmenu f = new mainmenu();
                    f.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Yanlış kullanıcı bilgileri!");
                }
            }
            catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);


                }
            }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            kayito kayito= new kayito();
            kayito.ShowDialog();
        }

        private void button1_MouseEnter(object sender, EventArgs e)
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

            // Fare butonun üzerine gelince imleç el işareti (tıklama modu) olsun
            dinamikButon.Cursor = Cursors.Hand;
        }

        private void btnKaydet_MouseLeave(object sender, EventArgs e)
        {
            Button dinamikButon = (Button)sender;

            // Fare gidince butonları orijinal tasarım renklerine geri döndürüyoruz
            if (dinamikButon.Name == "btnKaydet")
            {
                dinamikButon.BackColor = Color.White;
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

