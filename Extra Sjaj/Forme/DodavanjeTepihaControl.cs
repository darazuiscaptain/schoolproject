﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using ExtraSjaj.Modeli;

namespace ExtraSjaj.Forme
{
    public partial class DodavanjeTepihaControl : UserControl
    {
   
        Racun Racun = new Racun();
        public DodavanjeTepihaControl()
        {
            InitializeComponent();
        }

        SqlConnection konekcija = new SqlConnection(Konekcija.konString);
        Modeli.Musterija Musterija = new Modeli.Musterija();
        public Tepih tepih = new Tepih();
        public void ucitavanjeProfilaTepiha(Musterija musterija, int IdRacuna, bool placen)
        {
            resetujObjekte();
            label1.Text = "Mušterija: " + musterija.ImePrezime;
            label5.Text = "Racun kreiran: " + DateTime.Now.ToUniversalTime().ToString();
            if (placen)
            {
                sakrijObjekteNaKontroli();
                label6.Text = "Plaćen: Da";
            }
            else
            {
                otkrijObjekteNaKontroli();
                label6.Text = "Plaćen: Ne";
            }
                
            Racun.Id = IdRacuna;
        }



        void updateBrojTepihaURacunuNakonDodavanjaIBrisanjaTepiha()
        {
            Racun.updateBrojaTepihaNakonDodavanjaTepiha(Racun.Id,tepih.BrojTepihaURacunu(Racun.Id));
        }



        public void IscitajTabeluTepisiZaMusteriju(int IdRacuna)
        {
            try
            {
                konekcija.Open();

               /* dodavanje tepiha sa id-em iz tabele u listboxu, treba ti id kako bi mogao da radis na
                 selekt by id na listboxu nesto sa tepisima(npr.brisanje, update izabranog tepiha) */
                foreach (var tepih in tepih.popunjavanjeListeTepiha(IdRacuna))
                    listBox1.Items.Add(tepih.Value);
                



                SqlDataAdapter sda = new SqlDataAdapter("select t.id , t.Sirina as 'Širina/m' ," +
                                                        " t.Duzina as 'Dužina/m'  , t.Kvadratura as 'Kvadratura/m2'  " +
                                                        "from Tepisi t join Racuni r on r.Id = t.RacunId" +
                                                        " where r.Id = " + IdRacuna, konekcija);
                DataTable dt = new DataTable();


                sda.Fill(dt);
                dataGridView1.DataSource = dt;
                racunZaMusteriju();
               
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                konekcija.Close();
            }
            
        }
        public void IscitajTabeluTepisiZaMusteriju()
        {
            konekcija.Open();


            SqlDataAdapter sda = new SqlDataAdapter("select t.id , t.Sirina as 'Širina/m' ," +
                                                    " t.Duzina as 'Dužina/m'  , t.Kvadratura as 'Kvadratura/m2'  " +
                                                    "from Tepisi t join Racuni r on r.Id = t.RacunId" +
                                                    " where r.Id = " + DodavanjeMusterijeControl.IdRacuna.ToString(), konekcija);
            DataTable dt = new DataTable();


            sda.Fill(dt);
            dataGridView1.DataSource = dt;
            racunZaMusteriju();
            konekcija.Close();
        }

        void racunZaMusteriju()
        {
            double racun = racunMusterije();
            label2.Text = "Račun:";
            label2.Text += " " + racun.ToString() + " EUR";
            textBox3.Text = racun.ToString();
        }

        double  racunMusterije()
        {
            return Math.Round((tepih.getKvadratura(Racun.Id)) * Convert.ToDouble(comboBox1.Text), 2);
        }
            
        private void TepisiMusterije_FormClosing(object sender, FormClosingEventArgs e)
        {
            updateBrojTepihaURacunuNakonDodavanjaIBrisanjaTepiha();
        }

      
        void updateRacunNakonPlacanja()
        {
            int platio = 0;
            if (racunMusterije() == Convert.ToDouble(textBox3.Text))
                platio = 1;
            else
                platio = 0;
            Racun.updateRacunaNakonPlacanja(platio, Racun.Id);
        }

      
        private void btnDodajTepih_Click_1(object sender, EventArgs e)
        {
           
            listBox1.Items.Clear();
            tepih.DodajTepih(textBox1.Text, textBox2.Text, Musterija.Id, Racun.Id);
            IscitajTabeluTepisiZaMusteriju();
            racunZaMusteriju();
            updateBrojTepihaURacunuNakonDodavanjaIBrisanjaTepiha();
           
            foreach (var tepih in tepih.popunjavanjeListeTepiha(Racun.Id))
                listBox1.Items.Add(tepih.Value);
            Racun.updateRacunNakonDodavanjaTepiha(racunMusterije(), Racun.Id);
        }
        
        void racunNaplacen()
        {
            if (label6.Text == "Plaćeno:  Da")
            {
                btnDodajTepih.Visible = false;
                btnNaplati.Visible = false;
            }
        }
        private void btnNaplati_Click_1(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Da li si siguran da je mušterija platio?", "Poruka", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if ((dialogResult == DialogResult.Yes) && Convert.ToDouble(textBox3.Text) <= racunMusterije() && Convert.ToDouble(textBox3.Text) > 0)
            {
                updateRacunNakonPlacanja();
                MessageBox.Show("Uspešno naplaćeno.", "Poruka", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Visible = false;
            }

            else if (Convert.ToDouble(textBox3.Text) > racunMusterije() || Convert.ToDouble(textBox3.Text) < 0)
            {
                MessageBox.Show("Ispravno unesi koliko је mušterija ostavio novca.(ne mozeš uneti negativan broj, niti više od njegovog računa.");
            }
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            racunZaMusteriju();
            Racun.updateRacunNakonDodavanjaTepiha(racunMusterije(), Racun.Id);
            racunNaplacen();
        }
        
        
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
           
            if (MessageBox.Show("Da li si siguran da zelis obrisati selektovani tepih?", "Poruka", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                tepih.BrisanjeTepiha(dataGridView1.SelectedCells[0].Value.ToString());
                IscitajTabeluTepisiZaMusteriju();
                racunZaMusteriju();
                updateBrojTepihaURacunuNakonDodavanjaIBrisanjaTepiha();

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            this.Visible = false;
        }



        void sakrijObjekteNaKontroli()
        {
            textBox1.Visible = false;
            textBox2.Visible = false;
            btnDodajTepih.Visible = false;
            btnNaplati.Visible = false;
            comboBox1.Visible = false;
            label4.Visible = false;
            label3.Visible = false;
            label7.Visible = false;
            textBox3.Visible = false;
        }
        void otkrijObjekteNaKontroli()
        {
            textBox1.Visible = true;
            textBox2.Visible = true;
            btnDodajTepih.Visible = true;
            btnNaplati.Visible = true;
            comboBox1.Visible = true;
            label4.Visible = true;
            label3.Visible = true;
            label7.Visible = true;
            textBox3.Visible = true;
        }
        void resetujObjekte()
        {
            listBox1.Items.Clear();
            textBox1.Clear();
            textBox2.Clear();
        }

    }
}
