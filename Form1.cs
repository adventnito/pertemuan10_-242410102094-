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

namespace SimpleCRUDWinform
{
    public partial class Form1 : Form
    {
        #region GlobalVar
        public static string IDKTPParam { get; set; }
        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BindTempatLahir();
            ShowDetailData(IDKTPParam);
        }

        private void ShowDetailData(string idKtp)
        {
            try
            {
                DataTable dt = new DataTable();
                if(!string.IsNullOrEmpty(idKtp))
                {
                    string sql = "select * from dbo.DataCustomer where IdKtp = @idktp";
                    using (SqlConnection conn = Tools.Connections.ConnSQlServer())
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@idktp", idKtp);
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(dt);
                        }
                    }
                }
                string nama = string.Empty;
                string panggilan = string.Empty;
                string jk = string.Empty;
                string tglLahir = string.Empty;
                int idKabKota = 0;
                string Alamat = string.Empty;
                string member = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    nama = dt.Rows[0]["NamaCustomer"].ToString();
                    panggilan = dt.Rows[0]["NamaPanggilan"].ToString();
                    jk = dt.Rows[0]["JenisKelamin"].ToString();
                    tglLahir = dt.Rows[0]["TanggalLahir"].ToString();
                    idKabKota = Convert.ToInt32(dt.Rows[0]["IdKabKotaTempatLahir"].ToString());
                    Alamat = dt.Rows[0]["Alamat"].ToString();
                    member = dt.Rows[0]["IsMember"].ToString();                   
                }

                TxtIdKtp.Text = idKtp;
                TxtNama.Text = nama;
                TxtPanggilan.Text = panggilan;
                TxtTanggalLahir.Text = tglLahir;
                CmbTempatLahir.SelectedIndex = idKabKota;
                TxtAlamat.Text = Alamat;
                if (jk == "1")
                {
                    RdoPerempuan.Checked = true;
                }
                else
                {
                    RdoLaki.Checked = true;
                }

                if (member.ToLower() == "true")
                {
                    ChkMember.Checked = true;
                }
                else
                {
                    ChkMember.Checked = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Show Detail Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindTempatLahir()
        {
            try
            {
                DataTable dt = new DataTable();
                string sql = "select IdRfKabKota,NamaKabKota from dbo.RfKabKota";
                using(SqlConnection conn = Tools.Connections.ConnSQlServer())
                {
                    conn.Open();
                    using(SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    } 
                }
                int total = dt.Rows.Count;
                DataRow dr = dt.NewRow();
                dr[0] = 0;
                dr[1]= "--Pilih Kab/Kota --";
                dt.Rows.InsertAt(dr, 0);
                CmbTempatLahir.DisplayMember = "NamaKabKota";
                CmbTempatLahir.ValueMember = "IdRfKabKota";
                CmbTempatLahir.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error Bind Tempat Lahir", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string idKtp = TxtIdKtp.Text;
            string nama = TxtNama.Text;
            string namaPanggilan = TxtPanggilan.Text;
            int jk = 0;
            if(!RdoLaki.Checked)
            {
                jk = 1;
            }
            int tempatLahir = Convert.ToInt32(CmbTempatLahir.SelectedValue);
            DateTime dateLahir = Convert.ToDateTime(TxtTanggalLahir.Text);
            string tglLahir = dateLahir.ToString("yyyy-MM-dd");
            string alamat = TxtAlamat.Text;
            bool isMember = ChkMember.Checked;
            if (string.IsNullOrEmpty(idKtp))
            {
                MessageBox.Show("ID Ktp Kosong!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            } else if(string.IsNullOrEmpty(nama))
            {
                MessageBox.Show("Nama Customer Kosong!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                string sql = string.Empty;
                using(SqlConnection conn = Tools.Connections.ConnSQlServer())
                {
                    conn.Open();
                    sql = "select 1 from DataCustomer where IdKtp = @idktp";
                    using(SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@idktp", idKtp);
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            sql = "update dbo.DataCustomer\r\nset NamaCustomer = @namaCustomer," +
                                "\r\nNamaPanggilan = @panggilan,\r\nJenisKelamin = @jk," +
                                "\r\nTanggalLahir = @tanggalLahir,\r\nIdKabKotaTempatLahir = @tempatLahir," +
                                "\r\nAlamat = @alamat,\r\nIsMember = @isMember\r\nwhere IdKtp = @idKtp";
                        }
                        else
                        {
                            sql = "insert into dbo.DataCustomer (IdKtp,NamaCustomer,NamaPanggilan," +
                                "JenisKelamin, TanggalLahir,IdKabKotaTempatLahir, Alamat, IsMember) " +
                                "\r\nvalues (@idKtp,@namaCustomer,@panggilan,@jk,@tanggalLahir," +
                                "@tempatLahir,@alamat,@isMember)";
                        }

                        using(SqlCommand cmdProcess = new SqlCommand(sql, conn))
                        {
                            cmdProcess.Parameters.AddWithValue("@idKtp", idKtp);
                            cmdProcess.Parameters.AddWithValue("@namaCustomer", nama);
                            cmdProcess.Parameters.AddWithValue("@panggilan", namaPanggilan);
                            cmdProcess.Parameters.AddWithValue("@jk", jk);
                            cmdProcess.Parameters.AddWithValue("@tanggalLahir", tglLahir);
                            cmdProcess.Parameters.AddWithValue("@tempatLahir", tempatLahir);
                            cmdProcess.Parameters.AddWithValue("@alamat", alamat);
                            cmdProcess.Parameters.AddWithValue("@isMember", isMember);
                            cmdProcess.ExecuteNonQuery();

                            MessageBox.Show("Data Telah Tersimpan", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            var listForm = new ListCustomer();
            listForm.Show();

        }
    }
}
