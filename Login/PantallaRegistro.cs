using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using BCrypt.Net;

namespace Login
{
    public partial class PantallaRegistro : Form
    {
        public PantallaRegistro()
        {
            InitializeComponent();
            // Configurar el ComboBox para que solo permita seleccionar opciones predefinidas
            rolComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            sexoComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            // Rellenar los ComboBox con las opciones
            rolComboBox.Items.Add("Estudiante");
            rolComboBox.Items.Add("Profesor");
            rolComboBox.Items.Add("Administrador");

            sexoComboBox.Items.Add("Masculino");
            sexoComboBox.Items.Add("Femenino");
        }

        private void rolComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Verificar si hay una selección en el ComboBox
            if (rolComboBox.SelectedItem == null)
            {
                // Manejar el caso en que no hay selección
                return;
            }

            if (rolComboBox.SelectedItem.ToString() == "Estudiante")
            {
                codigoUnicoTextBox.Enabled = true;
                codigoUnicoTextBox.Text = string.Empty;
            }
            else
            {
                codigoUnicoTextBox.Enabled = false;
                try
                {
                    int newCode = GenerateUniqueCode();
                    codigoUnicoTextBox.Text = newCode.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al generar el código único: " + ex.Message);
                }
            }
        }

        private int GenerateUniqueCode()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SCAEConnectionString"].ConnectionString;
            Random random = new Random();
            int newCode;
            bool codeExists;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                do
                {
                    // Generar un número aleatorio de hasta 8 dígitos
                    newCode = random.Next(10000000, 100000000); // Genera un número entre 10,000,000 y 99,999,999
                    codeExists = false;

                    // Verificar si el código ya existe
                    string query = "SELECT COUNT(*) FROM Usuarios WHERE id = @NewCode";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NewCode", newCode);
                        int count = (int)cmd.ExecuteScalar();
                        if (count > 0)
                        {
                            codeExists = true;
                        }
                    }
                } while (codeExists); // Repetir si el código ya existe

                return newCode;
            }
        }

        private void LimpiarCampos()
        {
            emailTextBox.Text = "";
            passwordTextBox.Text = "";
            cedulaTextBox.Text = "";
            nombresTextBox.Text = "";
            apellidosTextBox.Text = "";
            sexoComboBox.SelectedIndex = -1;
            direccionTextBox.Text = "";
            telefonoTextBox.Text = "";
            rolComboBox.SelectedIndex = -1;
            codigoUnicoTextBox.Text = "";
        }


        private void codigoUnicoTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void emailTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void passwordTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void cedulaTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void nombresTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void apellidosTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void sexoComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void direccionTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void telefonoTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void registrarUsuarioButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(emailTextBox.Text) ||
                string.IsNullOrEmpty(passwordTextBox.Text) ||
                string.IsNullOrEmpty(cedulaTextBox.Text) ||
                string.IsNullOrEmpty(nombresTextBox.Text) ||
                string.IsNullOrEmpty(apellidosTextBox.Text) ||
                sexoComboBox.SelectedItem == null ||
                string.IsNullOrEmpty(direccionTextBox.Text) ||
                string.IsNullOrEmpty(telefonoTextBox.Text) ||
                rolComboBox.SelectedItem == null)
            {
                MessageBox.Show("Por favor, completa todos los campos.");
                return;
            }

            // Validar el campo de código único si está habilitado
            if (codigoUnicoTextBox.Enabled)
            {
                if (codigoUnicoTextBox.Text.Length != 9 || !long.TryParse(codigoUnicoTextBox.Text, out _))
                {
                    MessageBox.Show("El código único debe tener exactamente 9 dígitos numéricos.");
                    return;
                }
            }

            if (!emailTextBox.Text.EndsWith("@epn.edu.ec"))
            {
                MessageBox.Show("El correo debe terminar con @epn.edu.ec.");
                return;
            }

            if (cedulaTextBox.Text.Length != 10 || !long.TryParse(cedulaTextBox.Text, out _))
            {
                MessageBox.Show("La cédula debe tener 10 dígitos numéricos.");
                return;
            }

            if (telefonoTextBox.Text.Length != 10 || !long.TryParse(telefonoTextBox.Text, out _))
            {
                MessageBox.Show("El teléfono debe tener 10 dígitos numéricos.");
                return;
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(passwordTextBox.Text);

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SCAEConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Usuarios (id, email, password, ci, nombres, apellidos, sexo, direccion, telefono, rol, bloqueado) " +
                               "VALUES (@id, @Email, @Password, @CI, @Nombres, @Apellidos, @Sexo, @Direccion, @Telefono, @Rol, 0)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", codigoUnicoTextBox.Text);
                    cmd.Parameters.AddWithValue("@Email", emailTextBox.Text);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    cmd.Parameters.AddWithValue("@CI", cedulaTextBox.Text);
                    cmd.Parameters.AddWithValue("@Nombres", nombresTextBox.Text);
                    cmd.Parameters.AddWithValue("@Apellidos", apellidosTextBox.Text);
                    cmd.Parameters.AddWithValue("@Sexo", sexoComboBox.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@Direccion", direccionTextBox.Text);
                    cmd.Parameters.AddWithValue("@Telefono", telefonoTextBox.Text);
                    cmd.Parameters.AddWithValue("@Rol", rolComboBox.SelectedItem.ToString());

                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Usuario registrado con éxito.");
                        LimpiarCampos();
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Error al registrar el usuario: " + ex.Message);
                    }
                }
            }
        }

        private void regresarButton_Click(object sender, EventArgs e)
        {
            //regresar a login

            Form pantallaLogin = new PantallaLogin();
            pantallaLogin.Show();
            this.Hide();
            //this.Close();
        }
    }
}