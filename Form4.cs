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
    public partial class Form4 : Form
    {
        string id;
        SqlConnection con = new SqlConnection("Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename=|DataDirectory|\\Agil_Recettes_BD.mdf;Integrated Security = True");
        SqlCommand cmd;
        public Form4(string id)
        {
            InitializeComponent();
            this.id = id;
        }

        private void Form4_Load(object sender, EventArgs e)
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
                label1.Text = id;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " form 4 load");
            }
            finally
            {
                con.Close();
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                if (long.TryParse(textBox2.Text, out long NouveauIndex))
                {
                    try
                    {
                        con.Open();
                        cmd.CommandText = "update machine set [index]=@NouveauIndex where [id] =@id1";
                        cmd.Parameters.Add("@NouveauIndex", SqlDbType.BigInt);
                        cmd.Parameters.Add("@id1", SqlDbType.VarChar, 50);
                        cmd.Prepare();
                        cmd.Parameters["@NouveauIndex"].Value = NouveauIndex;
                        cmd.Parameters["@id1"].Value = id;
                        cmd.ExecuteNonQuery();  
                        Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + " text box2 key down enter ");
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
