using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _MYS1_API_P13
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_generar_Click(object sender, EventArgs e)
        {
            generador_objetos gn = new generador_objetos();
            gn.crearModelo();
            MessageBox.Show("Modelo creado con éxito.");
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}
