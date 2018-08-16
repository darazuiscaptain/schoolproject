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
    public partial class ArhivaMusterijaControl : UserControl
    {

        SqlConnection konekcija = new SqlConnection(Konekcija.konString);
        public ArhivaMusterijaControl()
        {
            InitializeComponent();
            citajTabeluMusterijeFromSql();
            comboBox1_SelectedIndexChanged(new object(), new EventArgs());
        }

        public void citajTabeluMusterijeFromSql()
        {

            SqlDataAdapter sda = new SqlDataAdapter("select m.id,row_number() over (order by m.Id) as 'Br.Mušterije'," +
                "m.ImePrezime as 'Ime i Prezime',m.BrojTepiha as 'Br.Tepiha',m.BrojTelefona as 'Br. Tel.',m.Adresa, " +
                "sum(isnull(t.kvadratura,0)) as 'Kvadratura Tepiha', m.VremeDolaskaTepiha as 'Tepisi dostavljeni', m.Racun as 'Račun/Eur', m.Platio as 'Plaćeno' " +
                "from Musterijas m left join Tepisi t on t.MusterijaId = m.Id " +
                "where m.Platio = 1" +
                "" +
                "group by m.id, m.ImePrezime, m.BrojTepiha, m.BrojTelefona, m.Adresa, m.VremeDolaskaTepiha,m.Racun, m.Platio" +
                " order by m.Id asc", konekcija);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dataGridView1.DataSource = dt;

        }
        void arhivaMusterijaUOdredjenomPeriodu(int selektovaniPeriod, string selektovaniDeoDatuma)
        {
            int platio = 0;
            string queryPart = "";
            string queryPart1 = "";
            if (checkBox1.Checked)
                platio = 1;

            if (!checkBox2.Checked)
                queryPart = "where m.Platio = " + platio + " and";
            else
                queryPart1 = " where";


            SqlDataAdapter sda = new SqlDataAdapter("select m.id,row_number() over (order by m.Id) as 'Br.Mušterije'," +
           "m.ImePrezime as 'Ime i Prezime',m.BrojTepiha as 'Br.Tepiha',m.BrojTelefona as 'Br. Tel.',m.Adresa, " +
           "sum(isnull(t.kvadratura,0)) as 'Kvadratura Tepiha', m.VremeDolaskaTepiha as 'Tepisi dostavljeni', m.Racun as 'Račun/Eur', m.Platio as 'Plaćeno' " +
           "from Musterijas m left join Tepisi t on t.MusterijaId = m.Id " +
           queryPart +
          queryPart1 + " datediff(" + selektovaniDeoDatuma + ", m.VremeDolaskaTepiha, getdate()) = " + selektovaniPeriod +
           " group by m.id, m.ImePrezime, m.BrojTepiha, m.BrojTelefona, m.Adresa, m.VremeDolaskaTepiha,m.Racun, m.Platio" +
           " order by m.Id asc", konekcija);

            DataTable dt = new DataTable();
            sda.Fill(dt);
            dataGridView1.DataSource = dt;
        }
        void arhivaPotencijalneZaradeUOdredjenomPeriodu(int selektovaniPeriod, string selektovaniDeoDatuma)
        {
            label1.Text = "";
            SqlCommand komanda = new SqlCommand("select sum(isnull(Racun,0)) from Musterijas" +
                " where datediff(" + selektovaniDeoDatuma + ", VremeDOlaskaTepiha, getdate()) = " + selektovaniPeriod, konekcija);
            konekcija.Open();
            label1.Text = komanda.ExecuteScalar().ToString() + " EUR.";
            konekcija.Close();

        }
        void arhivaZaradaUOdredjenomPeriodu(int selektovaniPeriod, string selektovaniDeoDatuma)
        {
            label5.Text = "";
            SqlCommand komanda = new SqlCommand("select sum(isnull(Racun,0)) from Musterijas" +
                " where Platio = 1 and  datediff(" + selektovaniDeoDatuma + ", VremeDOlaskaTepiha, getdate()) = " + selektovaniPeriod, konekcija);
            konekcija.Open();
            label5.Text = komanda.ExecuteScalar().ToString() + " EUR.";
            konekcija.Close();

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selektovaniPeriod = 0;
            string selektovaniDeoDatuma = "";

            switch (comboBox1.Text)
            {
                case "Danas":
                    {
                        selektovaniPeriod = 0;
                        selektovaniDeoDatuma = "day";
                        break;
                    }
                case "Juče":
                    {
                        selektovaniPeriod = 1;
                        selektovaniDeoDatuma = "day";
                        break;
                    }

                case "Ove nedjelje":
                    {
                        selektovaniPeriod = 0;
                        selektovaniDeoDatuma = "week";
                        break;
                    }

                case "Prošle nedjelje":
                    {
                        selektovaniPeriod = 1;
                        selektovaniDeoDatuma = "week";
                        break;
                    }

                case "Ovog mjeseca":
                    {
                        selektovaniPeriod = 0;
                        selektovaniDeoDatuma = "month";
                        break;
                    }
                case "Prošlog mjeseca":
                    {
                        selektovaniPeriod = 1;
                        selektovaniDeoDatuma = "month";
                        break;
                    }
                case "Ove godine":
                    {
                        selektovaniPeriod = 0;
                        selektovaniDeoDatuma = "year";
                        break;
                    }
                case "Prošle godine":
                    {
                        selektovaniPeriod = 1;
                        selektovaniDeoDatuma = "year";
                        break;
                    }
                default:
                    selektovaniDeoDatuma = "";
                    selektovaniPeriod = 0;
                    break;
            }

            arhivaMusterijaUOdredjenomPeriodu(selektovaniPeriod, selektovaniDeoDatuma);
            arhivaPotencijalneZaradeUOdredjenomPeriodu(selektovaniPeriod, selektovaniDeoDatuma);
            arhivaZaradaUOdredjenomPeriodu(selektovaniPeriod, selektovaniDeoDatuma);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1.Checked = false;
            comboBox1_SelectedIndexChanged(sender, e);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            checkBox2.Checked = false;
            comboBox1_SelectedIndexChanged(sender, e);
        }
    }
}