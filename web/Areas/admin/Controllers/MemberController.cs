using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using web.Areas.admin.Models;

namespace web.Areas.admin.Controllers
{
    public class MemberController : Controller
    {
        private readonly string myConnectionString = @"Server=localhost;Port=3306;Database=webdb;Uid=root;Pwd=Malkan06*-fb-;";
        // GET: admin/Member
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadData()
        {
            try
            {
                var listMember = new List<Member>();
                //Creating instance of DatabaseContext class  

                var sql = "select * from member ";
                var countsql = "select count(*) from member ";

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
                    sql += " WHERE Mobil LIKE '%" + searchValue +
                        "%' OR AdSoyad LIKE '%" + searchValue +
                        "%' OR IpAddress LIKE '%" + searchValue +
                        "%' OR MemberType LIKE '%" + searchValue + "%' ";
                        //"%' OR id LIKE %" + searchValue + "% ";

                    countsql += " WHERE Mobil LIKE '%" + searchValue +
                        "%' OR AdSoyad LIKE '%" + searchValue +
                        "%' OR IpAddress LIKE '%" + searchValue +
                        "%' OR MemberType LIKE '%" + searchValue +"%' ";
                    //"%' OR id LIKE %" + searchValue + "% ";

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
                                    var member = new Member();
                                    member.ID = Convert.ToInt32(reader["id"]);
                                    member.Mobil = reader["Mobil"].ToString();
                                    member.AdSoyad = reader["AdSoyad"].ToString();
                                    member.IpAddress = reader["IpAddress"].ToString();
                                    member.IsBlocked = Convert.ToBoolean(reader["IsBlocked"]);
                                    member.MemberType = reader["MemberType"].ToString();
                                    member.CreateDate = Convert.ToDateTime(reader["CreateDate"] == DBNull.Value ? DateTime.MinValue : reader["CreateDate"]);
                                    //comment.ArticleContent = reader["ArticleContent"].ToString();
                                    listMember.Add(member);
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

                
                var data = listMember.AsEnumerable();
                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public ActionResult Block(int id = 0)
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
                        cmd.CommandText = "update webdb.member set IsBlocked = @accepted where id = @id";
                        cmd.Parameters.AddWithValue("?accepted", true);
                        cmd.Parameters.AddWithValue("?id", id);
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    conn.Close();
                    return Json(new JsonResultModel
                    {
                        Success = true,
                        Message = "Kullanıcı Bloklandı!"
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
        public ActionResult UnBlock(int id = 0)
        {
            try
            {
                if (id == 0)
                {
                    return Json(new JsonResultModel
                    {
                        Success = false,
                        Message = "Kullanıcı Bloğu Kaldırıldı!"
                    });
                }

                using (var conn = new MySqlConnection(myConnectionString))
                {
                    conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "update webdb.member set IsBlocked = @accepted where id = @id";
                        cmd.Parameters.AddWithValue("?accepted", false);
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