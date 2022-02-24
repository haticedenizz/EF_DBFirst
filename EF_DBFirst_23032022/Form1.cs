using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EF_DBFirst_23032022
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        KuzeyYeliEntities ctx = new KuzeyYeliEntities();
        private void Form1_Load(object sender, EventArgs e)
        {
            //dataGridView1.DataSource = k.Urunlers.ToList();
            urunlistele();

            cmbkategori.DataSource = ctx.Kategorilers.ToList();
            cmbkategori.DisplayMember = "KategoriAdi";
            cmbkategori.ValueMember = "KategoriID";

            cmbtedarikci.DataSource = ctx.Tedarikcilers.ToList();
            cmbtedarikci.DisplayMember = "SirketAdi";
            cmbtedarikci.ValueMember = "TedarikciID";


        }

        private void btnekle_Click(object sender, EventArgs e)
        {
            Urunler u = new Urunler();
            u.UrunAdi = txturunadi.Text;
            u.Fiyat = nudfiyat.Value;
            u.Stok = (short)nudstok.Value;
            u.KategoriID = (int)cmbkategori.SelectedValue;
            u.TedarikciID = (int)cmbtedarikci.SelectedValue;

            ctx.Urunlers.Add(u);
            ctx.SaveChanges();
            dataGridView1.DataSource = ctx.Urunlers.ToList();

        }
        void urunlistele()
        {
            var urunler = ctx.Urunlers.Join(ctx.Kategorilers,
               u => u.KategoriID,
               k => k.KategoriID,
              (urn,ktg) =>new {
                  urn.UrunID,
                  urn.UrunAdi,
                  urn.Fiyat,
                  urn.Stok,
                  ktg.KategoriAdi
              }).ToList();

            var urunler2 = ctx.Urunlers.Join(ctx.Kategorilers,
              u => u.KategoriID,
              k => k.KategoriID,
             (urn, ktg) => new
             {
                 urn,
                 ktg

             }).Join(ctx.Tedarikcilers,
             uk => uk.urn.TedarikciID,
             t => t.TedarikciID,
             (urun, ted) => new
             {
                 urun.urn.UrunID,
                 urun.urn.UrunAdi,
                 urun.urn.Fiyat,
                 urun.urn.Stok,
                 urun.ktg.KategoriAdi,
                 urun.ktg.KategoriID,
                 urun.urn.Sonlandi,
                 ted.SirketAdi,
                 ted.TedarikciID
             }

             );
            if (radioButton1.Checked)
                dataGridView1.DataSource = urunler2.OrderBy(x => x.Fiyat).Where(x=>x.Sonlandi==false).ToList();

            dataGridView1.Columns["UrunID"].Visible = false;
            dataGridView1.Columns["KategoriID"].Visible = false;
            dataGridView1.Columns["TedarikciID"].Visible = false;

            if (radioButton2.Checked)
                dataGridView1.DataSource = urunler2.OrderByDescending(x => x.Fiyat).ToList();



          
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            urunlistele();
        }

        private void txtara_TextChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = 
                ctx.Urunlers.Where(x => x.UrunAdi.Contains(txtara.Text) & x.Sonlandi==false).ToList();
        }

        private void btnilkonkayit_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = ctx.Urunlers.Take(10).ToList();
        }

        private void btnrapor_Click(object sender, EventArgs e)
        {
            Rapor r = new Rapor();
            r.Show();
        }
        
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dataGridView1.CurrentRow;
            txturunadi.Text = row.Cells["UrunAdi"].Value.ToString();
            nudfiyat.Value = (decimal)row.Cells["Fiyat"].Value;
            nudstok.Value = (short)row.Cells["Stok"].Value;
            cmbkategori.SelectedValue = row.Cells["KategoriID"].Value;
            cmbtedarikci.SelectedValue = row.Cells["TedarikciID"].Value;
            txturunadi.Tag = row.Cells["UrunID"].Value;



        }

        private void silToolStripMenuItem_Click(object sender, EventArgs e)
        {
           DialogResult cevap= MessageBox.Show("Seçilen kayıt silinsin mi?","Kayıt Silme",
                MessageBoxButtons.YesNo,MessageBoxIcon.Question);

            if(cevap==DialogResult.Yes)
            {
                int id = (int)dataGridView1.CurrentRow.Cells["UrunID"].Value;
                Urunler u=ctx.Urunlers.FirstOrDefault(x => x.UrunID == id);
                ctx.Urunlers.Remove(u);
                ctx.SaveChanges();

            }
        }

        private void btnguncelle_Click(object sender, EventArgs e)
        {
            
            int id = (int)txturunadi.Tag;
           Urunler u= ctx.Urunlers.FirstOrDefault(x => x.UrunID == id);
            u.UrunAdi = txturunadi.Text;
            u.Fiyat = nudfiyat.Value;
            u.Stok = (short)nudstok.Value;
            u.KategoriID = (int)cmbkategori.SelectedValue;
            u.TedarikciID = (int)cmbtedarikci.SelectedValue;
           
            ctx.SaveChanges();
            urunlistele();
        }
    }
}
