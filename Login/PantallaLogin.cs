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

using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Login
{
    public partial class PantallaLogin : Form
    {

        //Intentos de conexión
        private int loginAttempts = 0;

        public PantallaLogin()
        {
            InitializeComponent();
            passwordTextBox.UseSystemPasswordChar = true;

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SCAEConnectionString"].ConnectionString;
            string email = emailTextField.Text;
            string password = passwordTextBox.Text;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT password, rol, bloqueado FROM Usuarios WHERE email = @Email";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        bool isBlocked = reader.GetBoolean(reader.GetOrdinal("bloqueado"));
                        if (isBlocked)
                        {
                            MessageBox.Show("Su cuenta está bloqueada.");
                            return;
                        }

                        string storedPassword = reader.GetString(reader.GetOrdinal("password"));
                        string role = reader.GetString(reader.GetOrdinal("rol"));

                        bool passwordMatch = false;

                        // Primero intentamos verificar la contraseña en texto plano
                        //if (password == storedPassword)
                        //{
                        //    passwordMatch = true;
                        //}
                        //else
                        //{
                            // Luego intentamos verificar la contraseña encriptada
                            try
                            {
                                passwordMatch = BCrypt.Net.BCrypt.Verify(password, storedPassword);
                            }
                            catch
                            {
                                // Manejar el caso donde BCrypt no puede verificar la contraseña
                                MessageBox.Show("Error en la verificación de la contraseña.");
                                return;
                            }
                        //}

                        if (passwordMatch)
                        {
                            // Login exitoso
                            loginAttempts = 0;
                            Form pantalla = null;

                            switch (role)
                            {
                                case "Estudiante":
                                    pantalla = new PantallaEstudiante();
                                    break;
                                case "Profesor":
                                    pantalla = new PantallaProfesor();
                                    break;
                                case "Administrador":
                                    pantalla = new PantallaAdmin();
                                    break;
                            }

                            if (pantalla != null)
                            {
                                pantalla.Show();
                                this.Hide();
                            }
                        }
                        else
                        {
                            loginAttempts++;
                            if (loginAttempts >= 3)
                            {
                                reader.Close();
                                string updateQuery = "UPDATE Usuarios SET bloqueado = 1 WHERE email = @Email";
                                using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@Email", email);
                                    updateCmd.ExecuteNonQuery();
                                }
                                MessageBox.Show("Su cuenta ha sido bloqueada después de 3 intentos fallidos.");
                            }
                            else
                            {
                                MessageBox.Show("Contraseña incorrecta.");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Correo incorrecto.");
                    }
                }
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void emailTextField_TextChanged(object sender, EventArgs e)
        {

        }

        private void passwordTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void togglePasswordButton_Click(object sender, EventArgs e)
        {
            // Alternar entre mostrar y ocultar la contraseña
            if (passwordTextBox.UseSystemPasswordChar)
            {
                // Mostrar contraseña
                passwordTextBox.UseSystemPasswordChar = false;
                togglePasswordButton.Text = "Ocultar"; // Cambiar el texto o imagen del botón
            }
            else
            {
                // Ocultar contraseña
                passwordTextBox.UseSystemPasswordChar = true;
                togglePasswordButton.Text = "Mostrar"; // Cambiar el texto o imagen del botón
            }
        }

        private void registrarUsuarioButton_Click(object sender, EventArgs e)
        {
            Form pantallaRegistro = new PantallaRegistro();
            pantallaRegistro.Show();
            this.Hide();
        }
    }
}