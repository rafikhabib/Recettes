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
    public partial class Form5 : Form
    {
        SqlConnection con = new SqlConnection("Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename=|DataDirectory|\\Agil_Recettes_BD.mdf;Integrated Security = True");
        SqlCommand cmd=new SqlCommand();
        SqlCommand cmd1 = new SqlCommand();
        string type;
        public Form5()
        {
            InitializeComponent();
            
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            // TODO: cette ligne de code charge les données dans la table 'bDDataSet_PU.Prix_Unitaire'. Vous pouvez la déplacer ou la supprimer selon les besoins.
            this.prix_UnitaireTableAdapter.Fill(this.bDDataSet_PU.Prix_Unitaire);
            type = "50";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                type = comboBox1.SelectedValue.ToString();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                int pu = 0;
                if (int.TryParse(textBox1.Text, out  pu) && pu>0)
                {
                    try
                    {
                        con.Open();
                        cmd.Connection = con;
                        cmd.CommandText = "update Prix_Unitaire set [pu]=@pu1 where [type] =@type1";
                        cmd.Parameters.Add("@pu1", SqlDbType.Int);
                        cmd.Parameters.Add("@type1", SqlDbType.VarChar, 50);
                        cmd.Prepare();
                        cmd.Parameters["@pu1"].Value = pu;
                        cmd.Parameters["@type1"].Value = type;
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        con.Close();
                        con.Open();

                        cmd1.Connection = con;
                        cmd1.CommandText = "update machine set [pu]=@pu where [type] =@type";
                        cmd1.Parameters.Add("@pu", SqlDbType.Int);
                        cmd1.Parameters.Add("@type", SqlDbType.VarChar, 50);
                        cmd1.Prepare();
                        cmd1.Parameters["@pu"].Value = pu;
                        cmd1.Parameters["@type"].Value = type;
                        cmd1.ExecuteNonQuery();
                        cmd1.Parameters.Clear();

                        SqlCommand command = new SqlCommand("select * from Prix_Unitaire", con);
                        SqlDataAdapter adap  = new SqlDataAdapter(command);
                        DataSet set = new DataSet();
                        adap.Fill(set,"Prix_Unitaire");
                        dataGridView1.DataSource = set.Tables[0];
                        
                        
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show(ex.Message + " form 5 text box2 key down enter ");
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
