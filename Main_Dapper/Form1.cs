using DapperRepository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Main_Dapper
{
    public partial class Form1 : Form
    {
        static string connection = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;
        static GenericUnitOfWork work = new GenericUnitOfWork(connection);

        static IGenericRepository<Product> repoProd = work.Repository<Product>();
        static IGenericRepository<Category> repoCat = work.Repository<Category>();

        //List<Product> products = repoProd.GetAll().ToList();
        //List<Category> categories = repoCat.GetAll().ToList();

        public Form1()
        {
            InitializeComponent();
        }

        // Select
        private void Button1_Click(object sender, EventArgs e)
        {
            SelectAll();
        }

        // Insert
        private void Button2_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            var res = form.ShowDialog();

            if (res == DialogResult.Cancel)
                return;

            var cat = repoCat.FindByName(form.textBox3.Text);

            if (cat == null)
            {
                repoCat.Add(new Category() { Name = form.textBox3.Text });
                cat = repoCat.FindByName(form.textBox3.Text);
            }

            var price = (float)Convert.ToDouble(form.textBox2.Text);
            Product prod = new Product() { Name = form.textBox1.Text, Price = price, CategoryId = cat.Id };
            repoProd.Add(prod);

            SelectAll();
        }

        // Update
        private void Button3_Click(object sender, EventArgs e)
        {

        }

        // Delete
        private void Button4_Click(object sender, EventArgs e)
        {
            int n = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ProductId"].Value);

            var prod = repoProd.FindById(n);

            repoProd.Remove(prod.Id);

            SelectAll();
        }


        // Others Methods
        private void SelectAll()
        {
            var pc = from p in repoProd.GetAll()
                     join c in repoCat.GetAll() on p.CategoryId equals c.Id
                     select new { ProductId = p.Id, ProductName = p.Name, ProductPrice = p.Price, ProductCategory = c.Name };

            pc = pc.ToList();

            dataGridView1.DataSource = pc;
        }


    }
}
