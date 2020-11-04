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
            Regular();
        }

        // Insert
        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                Form2 form = new Form2();
                var res = form.ShowDialog();

                if (res == DialogResult.Cancel)
                    return;

                var checkProd = repoProd.FindByName(form.textBox1.Text);
                if (checkProd != null)
                {
                    MessageBox.Show("There is Product with this name.\nTry another name.");
                    return;
                }

                var cat = repoCat.FindByName(form.textBox3.Text);

                if (cat == null)
                {
                    repoCat.Add(new Category() { Name = form.textBox3.Text });
                    cat = repoCat.FindByName(form.textBox3.Text);
                }

                var p = Convert.ToDouble(form.textBox2.Text);
                float price = (float)Math.Round(p, 2);

                Product prod = new Product() { Name = form.textBox1.Text, Price = price, CategoryId = cat.Id };
                repoProd.Add(prod);

                Regular();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Update
        private void Button3_Click(object sender, EventArgs e)
        {
            try
            {
                int n = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ProductId"].Value);
                var prod = repoProd.FindById(n);

                Form2 form = new Form2();

                var _name = dataGridView1.SelectedRows[0].Cells["ProductName"].Value;
                var _price = Convert.ToDouble(dataGridView1.SelectedRows[0].Cells["ProductPrice"].Value);
                var _cat = dataGridView1.SelectedRows[0].Cells["ProductCategory"].Value;

                _price = Math.Round(_price, 2);

                form.textBox1.Text = _name.ToString();
                form.textBox2.Text = _price.ToString();
                form.textBox3.Text = _cat.ToString();

                var res = form.ShowDialog();

                if (res == DialogResult.Cancel)
                    return;

                var checkProd = repoProd.FindByName(form.textBox1.Text);
                if (checkProd != null && form.textBox1.Text != _name.ToString())
                {
                    MessageBox.Show("There is Product with this name.\nTry another name.");
                    return;
                }

                prod.Name = form.textBox1.Text;

                var p = Convert.ToDouble(form.textBox2.Text);
                float price = (float)Math.Round(p, 2);
                prod.Price = price;

                var category = repoCat.FindByName(form.textBox3.Text);

                if (category == null)
                {
                    repoCat.Add(new Category() { Name = form.textBox3.Text });
                    category = repoCat.FindByName(form.textBox3.Text);
                }

                prod.CategoryId = category.Id;
                repoProd.Update(prod);

                Regular();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Delete
        private void Button4_Click(object sender, EventArgs e)
        {
            try
            {
                int n = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ProductId"].Value);

                var prod = repoProd.FindById(n);

                repoProd.Remove(prod.Id);

                Regular();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

        private void CheckIfCategoryUsed()
        {
            var cats = repoCat.GetAll();

            foreach (var cat in cats)
            {
                var elem = repoProd.GetAll().Where(n => n.CategoryId == cat.Id);

                if (elem.Count() == 0)
                {
                    repoCat.Remove(cat.Id);
                }
            }

        }

        private void Regular()
        {
            try
            {
                SelectAll();
                CheckIfCategoryUsed();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    }
}
