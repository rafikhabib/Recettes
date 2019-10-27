using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Agil_Recettes
{
    public partial class Form1 : Form
    {
        static SqlConnection cn=new SqlConnection("Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename=|DataDirectory|\\Agil_Recettes_BD.mdf;Integrated Security = True");
        static SqlCommand cmd;
        SqlDataAdapter adap;
        SqlCommandBuilder cb;
        DataSet dtst;
        public DataTable tab;

        public static long ttmachine;
        public static int totaleLitresGasoil=0;
        public static int totaleLitresSansPlomb=0;
        public static int totaleLitres50=0;


        List<long> lsgz = new List<long>();
        List<long> lspayment = new List<long>();
        List<long> lscredits = new List<long>();
        List<long> lsbons = new List<long>();
        List<long> lsespeces = new List<long>();

        internal static List<Machine> Lsmachines { get; set; } = new List<Machine>();
        internal static List<string> Lssgaz { get; set; } = new List<string>();
        internal static List<string> Lsspayment { get; set; } = new List<string>();
        internal static List<string> Lsscredits { get; set; } = new List<string>();
        internal static List<string> Lssbons { get; set; } = new List<string>();
        internal static List<string> Lssespeces { get; set; } = new List<string>();

        public Form1()
        {
            InitializeComponent();
            ttmachine = 0;
            SetBounds(-5, 10, 816, 641);
        }

        public static long VenteMachine(long NouveauIndex , string id)
        {
            long result= -1;
            try
            {
                cn.Open();
                cmd = cn.CreateCommand();
                cmd.CommandText = "select [index] from Machine where id= @id";
                cmd.Parameters.Add("@id", SqlDbType.VarChar, 50);
                cmd.Prepare();
                cmd.Parameters["@id"].Value = id;
                object AncientIndexO=cmd.ExecuteScalar();
                long AncientIndex = long.Parse(AncientIndexO.ToString());
                if(NouveauIndex >= AncientIndex)
                {
                    cmd.CommandText = "select pu from Machine where id= @id1";
                    cmd.Parameters.Add("@id1", SqlDbType.VarChar, 50);
                    cmd.Prepare();
                    cmd.Parameters["@id1"].Value = id;
                    object puO = cmd.ExecuteScalar();
                    long pu = long.Parse(puO.ToString());
                    result =(NouveauIndex - AncientIndex) * pu;
                    
                    //recuperation de type de machine(gasoil,50,sans plomb)
                    cmd.CommandText = "select type from Machine where id= @id2";
                    cmd.Parameters.Add("@id2", SqlDbType.VarChar, 50);
                    cmd.Prepare();
                    cmd.Parameters["@id2"].Value = id;
                    object type1 = cmd.ExecuteScalar();
                    string type = type1.ToString();
                    //ajout de litrage pour chaque type
                    switch (type) {
                        case "Gasoil": totaleLitresGasoil += int.Parse((NouveauIndex - AncientIndex).ToString());  
                        break;
                        case "50": totaleLitres50 += int.Parse((NouveauIndex - AncientIndex).ToString());
                        break;
                        case "Sans Plomb": totaleLitresSansPlomb += int.Parse((NouveauIndex - AncientIndex).ToString());
                        break;
                    }

                    //test
                    //MessageBox.Show(totaleLitres50.ToString() + totaleLitresGasoil.ToString() + totaleLitresSansPlomb.ToString());
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " vente machine ");
            }
            finally
            {
                cn.Close();
            }
            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> lsid= new List<string>();
            try
            {
                cn.Open();
                cmd = cn.CreateCommand();
                cmd.CommandText = "select [id] from Machine";
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lsid.Add(reader.GetString(0));
                }
                cn.Close();
                foreach(string id in lsid)
                {
                    Form2 form2 =new Form2(id);
                    form2.ShowDialog();
                }
                textBox1.Text = ttmachine.ToString();
                button1.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " button 1 click ");
            }
            finally
            {
                cn.Close();
            }
        }
        public long SommeListe(List<long> lsgz)
        {
            long s=0;           
            foreach (long i in lsgz)
            {
                s += i;
            }
            return s;
        }       
        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {       
            if (e.KeyCode.Equals(Keys.Enter))
            {               
                if (long.TryParse(textBox3.Text, out long result)&& uint.TryParse(textBox7.Text, out uint qt))
                {
                    string resEnString = textBox7.Text + " * " + textBox3.Text;
                    Lssgaz.Add(resEnString);
                    lsgz.Add(result*qt);
                    textBox2.Text = SommeListe(lsgz).ToString();
                    textBox3.Clear();
                    totalvente();
                }
            }
        }

        public void totalvente()
        {
            long ttvente = 0;
            if(textBox1.Text!="" && textBox2.Text!="" && textBox5.Text != "")
            {
                if (long.TryParse(textBox1.Text, out long sm)&& long.TryParse(textBox2.Text, out long sg)&& long.TryParse(textBox5.Text, out long sp))
                {
                    ttvente += sm + sg + sp;
                    textBox6.Text = ttvente.ToString();
                }
            }
        }
        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                if (long.TryParse(textBox4.Text, out long result))
                {
                    Lsspayment.Add(textBox4.Text);
                    lspayment.Add(result);
                    textBox5.Text = SommeListe(lspayment).ToString();
                    textBox4.Clear();
                    totalvente();                   
                }
            }

        }
        public void totalfourni()
        {
            long ttfourni = 0;
            if (textBox10.Text != "" && textBox13.Text != "" && textBox14.Text != "")
            {
                if (long.TryParse(textBox14.Text, out long sm) && long.TryParse(textBox13.Text, out long sg) && long.TryParse(textBox10.Text, out long sp))
                {
                    ttfourni += sm + sg + sp;
                    textBox8.Text = ttfourni.ToString();
                }
            }
        }
        
        private void textBox15_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                if (long.TryParse(textBox15.Text, out long result))
                {
                    Lsscredits.Add(textBox15.Text);
                    lscredits.Add(result);
                    textBox14.Text = SommeListe(lscredits).ToString();
                    textBox15.Clear();
                    totalfourni();
                }
            }

        }
        private void textBox12_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                if (long.TryParse(textBox12.Text, out long result)&& uint.TryParse(textBox11.Text, out uint qt))
                {
                    string resEnString = textBox12.Text + " * " + textBox11.Text;
                    Lssbons.Add(resEnString);
                    lsbons.Add(result * qt);
                    textBox13.Text = SommeListe(lsbons).ToString();
                    textBox12.Clear();
                    totalfourni();
                }
            }
        }
        private void textBox9_KeyDown(object sender, KeyEventArgs e)
        {          
            if (e.KeyCode.Equals(Keys.Enter))
            {
                if (long.TryParse(textBox9.Text, out long result))
                {
                    Lssespeces.Add(textBox9.Text);
                    lsespeces.Add(result);
                    textBox10.Text = SommeListe(lsespeces).ToString();
                    textBox9.Clear();
                    totalfourni();
                }
            }
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox17_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                if (long.Parse(textBox8.Text) - long.Parse(textBox6.Text) > 0)
                {
                    textBox16.Text = "+" + (long.Parse(textBox8.Text) - long.Parse(textBox6.Text)).ToString();
                }
                else
                {
                    textBox16.Text = (long.Parse(textBox8.Text) - long.Parse(textBox6.Text)).ToString();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "PDF file|*.pdf", ValidateNames = true })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.A3.Rotate());
                    try
                    {
                        PdfWriter Writer = PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                        doc.Open();
                        PdfPTable tabledestring = new PdfPTable(1);
                        //remplissage de tableau des indexes
                        PdfPTable table = ListTopdfTable(Lsmachines);
                        doc.Add(table);

                        //remplissage des autres tableaux
                        tabledestring = ListEnStringTopdfTable(Lssgaz,"Liste de gaz",textBox2.Text);
                        //doc.Add(tabledestring);
                        tabledestring.WriteSelectedRows(0, -1, doc.Left + 315+215, doc.Top, Writer.DirectContent);

                        tabledestring = ListEnStringTopdfTable(Lsspayment, "Liste des payments", textBox5.Text);
                        //doc.Add(tabledestring);
                        tabledestring.WriteSelectedRows(0, -1, doc.Left+315, doc.Top, Writer.DirectContent);

                        tabledestring = ListEnStringTopdfTable(Lsscredits, "Liste des credits", textBox14.Text);
                        tabledestring.TotalWidth = 300f;
                        //doc.Add(tabledestring);
                        tabledestring.WriteSelectedRows(0, -1, doc.Left, doc.Top-400, Writer.DirectContent);

                         tabledestring = ListEnStringTopdfTable(Lssbons, "Liste des bons", textBox13.Text);
                        //doc.Add(tabledestring);
                        tabledestring.WriteSelectedRows(0, -1, doc.Left+315, doc.Top-400, Writer.DirectContent);

                        tabledestring = ListEnStringTopdfTable(Lssespeces, "Liste des especes", textBox10.Text);
                        //doc.Add(tabledestring);
                        tabledestring.WriteSelectedRows(0, -1, doc.Left + 315+215, doc.Top-400, Writer.DirectContent);

                        //test
                        PdfPTable pTable = new PdfPTable(1);
                        pTable.TotalWidth = 200f;
                        Phrase phrase = new Phrase(); 
                        PdfPCell cell = new PdfPCell();
                        cell.DisableBorderSide(1); cell.DisableBorderSide(3); cell.HorizontalAlignment = 1;

                        phrase.Add("Identifiant De l'employé = " + textBox17.Text);
                        cell.AddElement(phrase); cell.HorizontalAlignment = 1;
                        pTable.AddCell(cell);
                        pTable.WriteSelectedRows(0, -1, doc.Left + 760, doc.Top, Writer.DirectContent);
                        pTable.DeleteLastRow();
                        cell = new PdfPCell();
                        cell.DisableBorderSide(1); cell.DisableBorderSide(3); cell.HorizontalAlignment = 1;

                        Phrase phrase1=new Phrase("somme de vente = " + textBox6.Text);
                        cell.AddElement(phrase1); cell.HorizontalAlignment = 1;
                        pTable.AddCell(cell);
                        pTable.WriteSelectedRows(0, -1, doc.Left + 760, doc.Top-120, Writer.DirectContent);
                        pTable.DeleteLastRow();
                        cell = new PdfPCell();
                        cell.DisableBorderSide(1); cell.DisableBorderSide(3); cell.HorizontalAlignment = 1;

                        Phrase phrase2 = new Phrase("somme fourni = " +textBox8.Text );
                        cell.AddElement(phrase2); cell.HorizontalAlignment = 1;
                        pTable.AddCell(cell);
                        pTable.WriteSelectedRows(0, -1, doc.Left + 760, doc.Top-240, Writer.DirectContent);
                        pTable.DeleteLastRow();
                        cell = new PdfPCell();
                        cell.DisableBorderSide(1); cell.DisableBorderSide(3); cell.HorizontalAlignment = 1;

                        Phrase phrase3 = new Phrase("recette = " +textBox16.Text);
                        phrase3.Font.Size=18;
                        cell.AddElement(phrase3); cell.HorizontalAlignment = 1;
                        pTable.AddCell(cell);
                        pTable.WriteSelectedRows(0, -1, doc.Left + 760, doc.Top-360, Writer.DirectContent);
                        pTable.DeleteLastRow();


                        //ecriture des somme des litres
                        cell = new PdfPCell();
                        cell.DisableBorderSide(-1); cell.HorizontalAlignment = 1;
                        Phrase phrase4 = new Phrase("Litrage gasoil = " + totaleLitresGasoil.ToString());
                        phrase4.Font.Size = 11;
                        cell.AddElement(phrase4); cell.HorizontalAlignment = 1;
                        pTable.AddCell(cell);
                        pTable.WriteSelectedRows(0, -1, doc.Left + 985, doc.Top, Writer.DirectContent);
                        pTable.DeleteLastRow();

                        cell = new PdfPCell();
                        cell.DisableBorderSide(-1); cell.HorizontalAlignment = 1;
                        Phrase phrase5 = new Phrase("Litrage sans plomb = " + totaleLitresSansPlomb.ToString());
                        phrase5.Font.Size = 11;
                        cell.AddElement(phrase5); cell.HorizontalAlignment = 1;
                        pTable.AddCell(cell);
                        pTable.WriteSelectedRows(0, -1, doc.Left + 985, doc.Top-30, Writer.DirectContent);
                        pTable.DeleteLastRow();

                        cell = new PdfPCell();
                        cell.DisableBorderSide(-1); cell.HorizontalAlignment = 1;
                        Phrase phrase6 = new Phrase("Litrage 50 = " + totaleLitres50.ToString());
                        phrase6.Font.Size = 11;
                        cell.AddElement(phrase6); cell.HorizontalAlignment = 1;
                        pTable.AddCell(cell);
                        pTable.WriteSelectedRows(0, -1, doc.Left + 985, doc.Top-60, Writer.DirectContent);
                        pTable.DeleteLastRow();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                    finally
                    {
                        doc.Close();
                    }
                }
            }
        }

        private void textBox7_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                textBox3.Focus();
            }
        }

        private void textBox11_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
                textBox12.Focus();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private PdfPTable ListTopdfTable(List<Machine> Lsmachines)
        {
            PdfPTable table = new PdfPTable(3);
            PdfPCell cell = new PdfPCell(new Phrase("Tableau Des Indexes"));
            cell.Colspan = 3;
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);
            table.AddCell("Id");
            table.AddCell("Ancient Index");
            table.AddCell("Nouveau Index");
            foreach (Machine m in Lsmachines)
            {
                table.AddCell(m.Id);
                table.AddCell(m.AncIndx);
                table.AddCell(m.NvIndx);
            }
            PdfPCell cell1 = new PdfPCell(new Phrase("Somme"));
            cell1.Colspan = 2;
            cell1.HorizontalAlignment = 1;
            table.AddCell(cell1);
            table.AddCell(textBox1.Text);

            table.TotalWidth = 300f;
            table.LockedWidth = true;

            //relative col widths in proportions - 1/3 and 2/3

            float[] widths = new float[] { 0.75f,1.5f,1.5f };

            table.SetWidths(widths);
            table.HorizontalAlignment = 0;

            //leave a gap before and after the table

            table.SpacingBefore = 5f;
            table.SpacingAfter = 5f;
            return table;
        }

        private PdfPTable ListEnStringTopdfTable(List<string> vs,string titre,string somme)
        {
            PdfPTable table = new PdfPTable(1);
            PdfPCell cell = new PdfPCell(new Phrase(titre));
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);
            foreach (string m in vs)
            {
                table.AddCell(m);
            }
            PdfPCell cell1 = new PdfPCell(new Phrase("Somme"));
            cell1.HorizontalAlignment = 1;
            table.AddCell(cell1);
            table.AddCell(somme);

            table.TotalWidth = 200f;
            table.LockedWidth = true;

            //relative col widths in proportions - 1/3 and 2/3

            float[] widths = new float[] { 1f };

            table.SetWidths(widths);

            table.HorizontalAlignment = 0;

            //leave a gap before and after the table

            table.SpacingBefore = 5f;
            table.SpacingAfter = 5f;
            return table;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3("index");
            form3.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3("prix");
            form3.ShowDialog();
        }
    }
}
