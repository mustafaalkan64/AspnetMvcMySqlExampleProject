using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using web.Areas.admin.Models;
using System.Configuration;

namespace web.Areas.admin.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly string myConnectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        // GET: admin/Category
        public ActionResult Index()
        {
            return View();
        }

        // GET: admin/Category/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: admin/Category/Create
        public ActionResult Create()
        {
            var Category = new Category();
            return View(Category);
        }

        [HttpPost]
        public ActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage));

                ModelState.AddModelError("", message);
                return View(category);
            }
            using (var conn = new MySqlConnection(myConnectionString))
            {

                conn.Open();
                using (var comm = conn.CreateCommand())
                {
                    comm.CommandText = "insert into webdb.category(Name, CreateDate) values(@name, @createdate)";
                    comm.Parameters.AddWithValue("?name", category.Name);
                    comm.Parameters.AddWithValue("?createdate", DateTime.Now);
                    comm.ExecuteNonQuery();
                    comm.Dispose();
                    
                }
                conn.Close();
            }


            return RedirectToAction("Index");
        }

        public ActionResult LoadData()
        {
            try
            {
                var listArticle = new List<Category>();
                //Creating instance of DatabaseContext class  
                using (var connection = new MySqlConnection(myConnectionString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand("select * from webdb.category order by ID desc", connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows == true)
                            {
                                while (reader.Read())
                                {
                                    var category = new Category();
                                    category.ID = Convert.ToInt32(reader["ID"]);
                                    category.Name = reader["Name"].ToString();
                                    category.CreateDate = Convert.ToDateTime(reader["CreateDate"] == DBNull.Value ? DateTime.MinValue : reader["CreateDate"]);
                                    category.UpdateDate = Convert.ToDateTime(reader["UpdateDate"] == DBNull.Value ? DateTime.MinValue : reader["UpdateDate"]);
                                    listArticle.Add(category);
                                }
                            }
                        }
                        cmd.Dispose();
                    }
                    connection.Close();
                }
                var _listarticle = listArticle.AsEnumerable();
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();


                //Paging Size (10,20,50,100)    
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                //Sorting    
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    //if (draw == "1")
                    //{
                    //    sortColumnDir = "desc";
                    //}
                    _listarticle = _listarticle.OrderBy(sortColumn + " " + sortColumnDir);
                }


                //else
                //{
                //    _listarticle = _listarticle.OrderByDescending(x => x.ID);
                //}
                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    _listarticle = _listarticle.Where(x => x.Name.Contains(searchValue));
                }

                //total number of rows count     
                recordsTotal = _listarticle.Count();
                //Paging     
                var data = _listarticle.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        // GET: admin/Category/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var category = new Category();
                using (MySqlConnection connection = new MySqlConnection(myConnectionString))
                {
                    connection.Open();

                    var CategoryList = new List<Category>();
                    using (MySqlCommand cmd = new MySqlCommand("select * from webdb.category where ID = @id", connection))
                    {
                        cmd.Parameters.AddWithValue("?id", id);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows == true)
                            {
                                while (reader.Read())
                                {
                                    category.ID = Convert.ToInt32(reader["ID"]);
                                    category.Name = reader["Name"].ToString();
                                    return View(category);
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Herhangi bir Kategori Bulunamadı");
                                return View(category);
                            }
                        }
                        cmd.Dispose();
                    }
                    connection.Close();
                }
                return View(category);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("ErrorMessage", ex);
                return View();
            }
            
        }

        // POST: admin/Article/Edit/5
        [HttpPost]
        public ActionResult Edit(int? id, Category category)
        {
            try
            {
                if(id == null)
                {
                    ModelState.AddModelError("", "ID Bulunamadı");
                    return View(category);
                }

                if(ModelState.IsValid)
                {
                    using (var conn = new MySqlConnection(myConnectionString))
                    {
                        conn.Open();
                        using (var comm = conn.CreateCommand())
                        {

                            comm.CommandText = "update webdb.category set Name = @name, UpdateDate = @updatedate where ID = @id";
                            comm.Parameters.AddWithValue("?id", id);
                            comm.Parameters.AddWithValue("?name", category.Name);
                            comm.Parameters.AddWithValue("?updatedate", DateTime.Now);
                            comm.ExecuteNonQuery();
                            conn.Close();
                            return RedirectToAction("Index");
                        }

                    }
                }
                else
                {
                    var message = string.Join(" | ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage));

                    ModelState.AddModelError("", message);
                    return View(category);
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: admin/Category/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: admin/Category/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
