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
    public partial class Form3 : Form
    {
        static SqlConnection cn = new SqlConnection("Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename=|DataDirectory|\\Agil_Recettes_BD.mdf;Integrated Security = True");
        static SqlCommand cmd;

        string ParamTyp;

        List<string> lsid = new List<string>();
        public Form3(string ParamTyp)
        {
            InitializeComponent();
            this.ParamTyp = ParamTyp;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                textBox2.Focus();
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                if (verif_user(textBox1.Text, textBox2.Text))
                {
                    if (ParamTyp == "index")
                    {

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
                            foreach (string id in lsid)
                            {
                                Form4 form4 = new Form4(id);
                                form4.ShowDialog();
                            }
                        }
                        catch (SqlException ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                    else
                    {
                        Form5 form5 = new Form5();
                        form5.ShowDialog();
                    }
                }
            }
        }
        private bool verif_user(string username, string password)
        {
            bool v = false;
            try
            {
                cn.Open();
                cmd = cn.CreateCommand();
                cmd.CommandText = "select * from Users where [username]=@usr and [password]=@pwd";
                cmd.Parameters.Add("@usr", SqlDbType.VarChar, 50);
                cmd.Parameters.Add("@pwd", SqlDbType.VarChar, 50);
                cmd.Prepare();
                cmd.Parameters["@usr"].Value = username;
                cmd.Parameters["@pwd"].Value = password;
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) { v= true; Close(); }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                cn.Close();
            }
            return v;
            
        }
    }
}
