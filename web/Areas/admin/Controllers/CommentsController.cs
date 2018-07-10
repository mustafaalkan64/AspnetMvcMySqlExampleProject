using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using DataTables.AspNet.Mvc5;
using web.Areas.admin.Models;

namespace web.Areas.admin.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly string myConnectionString = @"Server=localhost;Port=3306;Database=webdb;Uid=root;Pwd=Malkan06*-fb-;";

        // GET: admin/Article
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(int id = 0)
        {
            var comment = new Comments();
            using (var connection = new MySqlConnection(myConnectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("select c.*, a.Caption, a.ArticleContent from comments as c join article as a on c.articleId = a.ID where c.ID = @id", connection))
                {
                    cmd.Parameters.AddWithValue("?id", id);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows == true)
                        {
                            while (reader.Read())
                            {

                                comment.ID = Convert.ToInt32(reader["ID"]);
                                comment.Comment = reader["comment"].ToString();
                                comment.IpAddress = reader["IpAddress"].ToString();
                                comment.CommentByUserName = reader["commentbyusername"].ToString();
                                comment.IsAccepted = Convert.ToBoolean(reader["isaccepted"]);
                                comment.CreateDate = Convert.ToDateTime(reader["CreateDate"] == DBNull.Value ? DateTime.MinValue : reader["CreateDate"]);
                                comment.ArticleId = Convert.ToInt32(reader["articleId"]);
                                comment.Article.Caption = reader["Caption"].ToString();
                                comment.Article.ArticleContent = reader["ArticleContent"].ToString();
                            }
                        }
                    }

                    cmd.Dispose();
                }
                connection.Close();
            }
            return View(comment);

        }

        public ActionResult LoadData()
        {
            try
            {
                var listArticle = new List<Comments>();
                //Creating instance of DatabaseContext class  

                var sql = "select c.*, a.Caption, a.ArticleContent from comments as c join article as a on c.articleId = a.ID ";
                var countsql = "select count(*) as count from comments as c join article as a on c.articleId = a.ID ";

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
                var conditionsql = "";

                if (!string.IsNullOrEmpty(searchValue))
                {
                    sql += " WHERE c.comment LIKE '%" + searchValue +
                        "%' OR c.commentbyusername LIKE '%" + searchValue +
                        "%' OR a.Caption LIKE '%" + searchValue +
                        "%' OR a.ArticleContent LIKE '%" + searchValue +
                        "%' OR c.IpAddress LIKE '%" + searchValue +
                        "%' OR c.email LIKE '%" + searchValue + "%' ";

                    countsql += " WHERE c.comment LIKE '%" + searchValue +
                        "%' OR c.commentbyusername LIKE '%" + searchValue +
                        "%' OR a.Caption LIKE '%" + searchValue +
                        "%' OR a.ArticleContent LIKE '%" + searchValue +
                        "%' OR c.IpAddress LIKE '%" + searchValue +
                        "%' OR c.email LIKE '%" + searchValue + "%' ";

                    //_listarticle = _listarticle.Where(x => x.Caption.Contains(searchValue) || x.ArticleContent.Contains(searchValue) || x.CategoryName.Contains(searchValue));
                }

                //Sorting    
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    sql += " ORDER BY " + sortColumn + " " + sortColumnDir;                
                }

                sql += " LIMIT " + pageSize + " OFFSET " + skip;

                using (var connection = new MySqlConnection(myConnectionString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows == true)
                            {
                                while (reader.Read())
                                {
                                    var comment = new Comments();
                                    comment.ID = Convert.ToInt32(reader["ID"]);
                                    comment.Comment = reader["comment"].ToString();
                                    comment.CommentByUserName = reader["commentbyusername"].ToString();
                                    comment.IpAddress = reader["IpAddress"].ToString();
                                    comment.IsAccepted = Convert.ToBoolean(reader["isaccepted"]);
                                    comment.CreateDate = Convert.ToDateTime(reader["CreateDate"] == DBNull.Value ? DateTime.MinValue : reader["CreateDate"]);
                                    comment.ArticleId = Convert.ToInt32(reader["articleId"]);
                                    comment.Article.Caption = reader["Caption"].ToString();
                                    //comment.ArticleContent = reader["ArticleContent"].ToString();
                                    listArticle.Add(comment);
                                }
                            }
                        }
                        cmd.Dispose();
                    }

                    using (MySqlCommand cmdcount = new MySqlCommand(countsql, connection))
                    {
                        using (MySqlDataReader reader = cmdcount.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                recordsTotal = Convert.ToInt32(reader[0]);
                            }
                        }
                        cmdcount.Dispose();
                    }

                    connection.Close();
                    connection.Dispose();
                }

                var data = listArticle.AsEnumerable();
                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        // GET: admin/Article/Create
        public ActionResult Create()
        {
            var model = new Article();
            var CategoryList = new List<Category>();
            //Creating instance of DatabaseContext class  
            using (var connection = new MySqlConnection(myConnectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("select * from webdb.category order by Name asc", connection))
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
                                CategoryList.Add(category);
                            }
                        }
                    }
                    cmd.Dispose();
                }
                connection.Close();
            }
            model.Categories = CategoryList;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(Article article, HttpPostedFileBase postedFile)
        {
            using (var conn = new MySqlConnection(myConnectionString))
            {
                var _FileName = "";
                if (postedFile != null)
                {
                    if (postedFile.ContentLength > 0)
                    {
                        if (postedFile.ContentType.Contains("jpeg") || postedFile.ContentType.Contains("jpg") || postedFile.ContentType.Contains("png"))
                        {
                            _FileName = Path.GetFileName(postedFile.FileName);
                            string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _FileName);
                            postedFile.SaveAs(_path);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Yüklenen Dosya Resim Formatında Olmalıdır");
                            return View(article);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Yüklediğiniz Resim Boyutu 0 olamaz");
                        return View(article);
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Herhangi Bir Resim Yüklemediniz!");
                    return View(article);
                }

                conn.Open();
                using (var comm = conn.CreateCommand())
                {
                    comm.CommandText = "insert into webdb.article(ArticleContent, Caption, CreateDate, CategoryId) values(@content, @caption, @createdate, @categoryid)";
                    comm.Parameters.AddWithValue("?content", article.ArticleContent);
                    comm.Parameters.AddWithValue("?caption", article.Caption);
                    comm.Parameters.AddWithValue("?categoryid", article.CategoryId);
                    comm.Parameters.AddWithValue("?createdate", DateTime.Now);
                    comm.ExecuteNonQuery();

                    string stm = "select MAX(ID) from webdb.article";
                    MySqlCommand cmd = new MySqlCommand(stm, conn);
                    int articleid = Convert.ToInt32(cmd.ExecuteScalar());
                    cmd.Dispose();

                    comm.CommandText = "insert into webdb.articleımages(ArticleImageUrl, ArticleId, UploadDate) values(@image, @articleid, @uploaddate)";
                    comm.Parameters.AddWithValue("?image", _FileName);
                    comm.Parameters.AddWithValue("?articleid", articleid);
                    comm.Parameters.AddWithValue("?uploaddate", DateTime.Now);
                    comm.ExecuteNonQuery();
                    comm.Dispose();
                }
                conn.Close();
            }


            return RedirectToAction("Index");
        }

        // GET: admin/Article/Edit/5
        public ActionResult Edit(int id)
        {
            var article = new Article();
            using (MySqlConnection connection = new MySqlConnection(myConnectionString))
            {
                connection.Open();

                var CategoryList = new List<Category>();
                using (MySqlCommand cmd = new MySqlCommand("select * from webdb.category order by Name asc", connection))
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
                                CategoryList.Add(category);
                            }
                        }
                    }
                    cmd.Dispose();
                }

                article.Categories = CategoryList;

                using (MySqlCommand cmd = new MySqlCommand("select * from webdb.article where id= @id", connection))
                {
                    cmd.Parameters.AddWithValue("?id", id);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        var nameSurname = "";
                        if (reader.HasRows == true)
                        {
                            while (reader.Read())
                            {
                                article.ID = id;
                                article.CategoryId = Convert.ToInt32(reader["CategoryId"] == DBNull.Value ? 0 : reader["CategoryId"]);
                                article.ArticleContent = reader["ArticleContent"].ToString();
                                article.Caption = reader["Caption"].ToString();
                                article.CreateDate = Convert.ToDateTime(reader["CreateDate"]);
                                article.UpdateDate = Convert.ToDateTime(reader["UpdateDate"] == DBNull.Value ? DateTime.MinValue : reader["UpdateDate"]);
                            }
                        }
                    }
                }
                using (MySqlCommand cmdimage = new MySqlCommand("select * from webdb.articleımages where ArticleId = @articleid", connection))
                {
                    cmdimage.Parameters.AddWithValue("?articleid", id);
                    using (MySqlDataReader readerimage = cmdimage.ExecuteReader())
                    {
                        var nameSurname = "";
                        if (readerimage.HasRows == true)
                        {
                            while (readerimage.Read())
                            {
                                article.ArticleImage = readerimage["ArticleImageUrl"].ToString();
                            }
                        }
                    }
                    cmdimage.Dispose();
                }
                connection.Close();
                return View(article);
            }
        }

        // POST: admin/Article/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Article article, HttpPostedFileBase postedFile)
        {
            try
            {
                var _FileName = "";
                if (postedFile != null)
                {
                    if (postedFile.ContentLength > 0)
                    {
                        if (postedFile.ContentType.Contains("jpeg") || postedFile.ContentType.Contains("jpg") || postedFile.ContentType.Contains("png"))
                        {
                            _FileName = Path.GetFileName(postedFile.FileName);
                            string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _FileName);
                            postedFile.SaveAs(_path);
                            article.ArticleImage = _FileName;
                        }
                        else
                        {
                            ModelState.AddModelError("", "Yüklenen Dosya Resim Formatında Olmalıdır");
                            return View(article);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Yüklediğiniz Resim Boyutu 0 olamaz");
                        return View(article);
                    }

                }

                using (var conn = new MySqlConnection(myConnectionString))
                {
                    conn.Open();
                    using (var comm = conn.CreateCommand())
                    {
                        comm.CommandText = "update webdb.article set ArticleContent = @content, Caption = @caption, UpdateDate = @updatedate, CategoryId = @categoryId where ID = @id";
                        comm.Parameters.AddWithValue("?id", article.ID);
                        comm.Parameters.AddWithValue("?content", article.ArticleContent);
                        comm.Parameters.AddWithValue("?caption", article.Caption);
                        comm.Parameters.AddWithValue("?categoryId", article.CategoryId);
                        comm.Parameters.AddWithValue("?updatedate", DateTime.Now);
                        comm.ExecuteNonQuery();

                        if (postedFile != null)
                        {
                            using (MySqlCommand cmd = new MySqlCommand("select * from webdb.articleımages where ArticleId = @articleid", conn))
                            {
                                cmd.Parameters.AddWithValue("?articleid", article.ID);
                                using (MySqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader.HasRows == true)
                                    {
                                        reader.Close();
                                        comm.CommandText = "update webdb.articleımages set ArticleImageUrl = @imageurl, UpdateDate = @updatedate1 where ArticleId = @articleid";
                                        comm.Parameters.AddWithValue("?articleid", article.ID);
                                        comm.Parameters.AddWithValue("?imageurl", article.ArticleImage);
                                        comm.Parameters.AddWithValue("?updatedate1", DateTime.Now);
                                        comm.ExecuteNonQuery();
                                        comm.Dispose();
                                    }
                                    else
                                    {
                                        reader.Close();
                                        comm.CommandText = "insert into webdb.articleımages(ArticleImageUrl, ArticleId, UploadDate) values(@image, @articleid, @uploaddate)";
                                        comm.Parameters.AddWithValue("?image", _FileName);
                                        comm.Parameters.AddWithValue("?articleid", article.ID);
                                        comm.Parameters.AddWithValue("?uploaddate", DateTime.Now);
                                        comm.ExecuteNonQuery();
                                        comm.Dispose();
                                    }
                                }
                                cmd.Dispose();
                            }

                        }

                        var CategoryList = new List<Category>();
                        using (MySqlCommand cmd = new MySqlCommand("select * from webdb.category order by Name asc", conn))
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
                                        CategoryList.Add(category);
                                    }
                                }
                            }
                            cmd.Dispose();
                        }
                        article.Categories = CategoryList;
                    }
                    conn.Close();
                }
                return View(article);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // POST: admin/Article/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                using (var conn = new MySqlConnection(myConnectionString))
                {
                    conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "delete from webdb.articleımages where ArticleId = @id";
                        cmd.Parameters.AddWithValue("?id", id);
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "delete from webdb.comments where articleId = @id";
                        cmd.Parameters.AddWithValue("?id", id);
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                    using (var comm = conn.CreateCommand())
                    {
                        comm.CommandText = "delete from webdb.article where ID = @id";
                        comm.Parameters.AddWithValue("?id", id);
                        comm.ExecuteNonQuery();
                        comm.Dispose();
                    }
                    conn.Close();
                    return Json(new JsonResultModel
                    {
                        Success = true,
                        Message = "Makale Başarıyla Silindi"
                    });
                }
                // TODO: Add delete logic here
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Accept(int id = 0)
        {
            try
            {
                if (id == 0)
                {
                    return Json(new JsonResultModel
                    {
                        Success = false,
                        Message = "Makale Bulunamadı"
                    });
                }

                using (var conn = new MySqlConnection(myConnectionString))
                {
                    conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "update webdb.comments set isaccepted = @accepted where ID = @id";
                        cmd.Parameters.AddWithValue("?accepted", true);
                        cmd.Parameters.AddWithValue("?id", id);
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    conn.Close();
                    return Json(new JsonResultModel
                    {
                        Success = true,
                        Message = "Onaylandı!"
                    });

                }
                // TODO: Add delete logic here
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
