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

namespace Agil_Recettes
{
    public partial class Form2 : Form
    {
        string id;
        SqlConnection con = new SqlConnection("Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename=|DataDirectory|\\Agil_Recettes_BD.mdf;Integrated Security = True");
        SqlCommand cmd;
        SqlDataAdapter adap;
        SqlCommandBuilder cb;
        DataSet dtst;
        public DataTable tab;

        public Form2(string id)
        {
            InitializeComponent();
            this.id = id;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                cmd = con.CreateCommand();
                cmd.CommandText = "select [index] from Machine where id= @id";
                cmd.Parameters.Add("@id", SqlDbType.VarChar, 50);
                cmd.Prepare();
                cmd.Parameters["@id"].Value = id;
                object AncientIndexO = cmd.ExecuteScalar();
                textBox1.Text = AncientIndexO.ToString();
                textBox2.Text = AncientIndexO.ToString();
                label1.Text = id.Remove(2,1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message+" form 2 load");
            }
            finally
            {
                con.Close();
            }
        }

        private void textBox2_Validated(object sender, EventArgs e)
        {

        }

        private void RemplirLsMachines()
        {
            Machine machine = new Machine(label1.Text,textBox1.Text,textBox2.Text);
            Form1.Lsmachines.Add(machine);
        }


        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                long NouveauIndex = long.Parse(textBox2.Text);
                if (NouveauIndex<long.Parse(textBox1.Text))
                {                   
                    MessageBox.Show("nouveau index doit etre supérieur a ancient index", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    try
                    {                        
                        con.Open();
                        Form1.ttmachine += Form1.VenteMachine(NouveauIndex, id);
                        cmd.CommandText = "update machine set [index]=@NouveauIndex where [id] =@id1";
                        cmd.Parameters.Add("@NouveauIndex", SqlDbType.BigInt);
                        cmd.Parameters.Add("@id1", SqlDbType.VarChar, 50);
                        cmd.Prepare();
                        cmd.Parameters["@NouveauIndex"].Value = NouveauIndex;
                        cmd.Parameters["@id1"].Value = id;
                        cmd.ExecuteNonQuery();
                        RemplirLsMachines();
                        Close();
                    }catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message+ " text box2 key down enter ");
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }
    }
}
