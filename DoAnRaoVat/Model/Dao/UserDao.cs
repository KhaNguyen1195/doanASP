﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.EF;
using PagedList;
using Common;

namespace Model.Dao
{
    public class UserDao
    {
        DoAnASPDBContext db = null;
        public UserDao()
        {
            db = new DoAnASPDBContext();
        }

        public long Insert(User entity)
        {
            db.Users.Add(entity);
            db.SaveChanges();
            return entity.ID;
        }

        public long InsertForFacebook(User entity)
        {
            var user = db.Users.SingleOrDefault(x => x.Username == entity.Username);
            if (user==null)
            {
                db.Users.Add(entity);
                db.SaveChanges();
                return entity.ID;
            }
            else
            {
                return user.ID;
            }
            
        }

        public bool Update(User entity)
        {
            try
            {
                var user = db.Users.Find(entity.ID);
                if (!string.IsNullOrEmpty(entity.Password))
                {
                    user.Password = entity.Password;
                }
                user.Username = entity.Username;
                user.Name = entity.Name;
                user.UserGroupID = entity.UserGroupID;
                user.Address = entity.Address;
                user.Email = entity.Email;
                user.Phone = entity.Phone;
                user.Status = entity.Status;
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public IEnumerable<User> LissAllPaging(string searchString, int page, int pageSize)
        {
            IQueryable<User> model = db.Users;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Username.Contains(searchString) || x.Phone.Contains(searchString));
            }
            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }

        //lấy username theo ID cho login
        public User GetByID(string UserName)
        {
            return db.Users.SingleOrDefault(x => x.Username == UserName);
        }

        public User ViewDetail(int id)
        {
            return db.Users.Find(id);
        }

        //public List<string> GetListCredential(string userName)
        //{
        //    var user = db.Users.Single(x => x.Username == userName);
        //    var data = (from cre in db.Credentials
        //               join gr in db.UserGroups on cre.UserGroupID equals gr.ID
        //               join rol in db.Roles on cre.RoleID equals rol.ID
        //               where gr.ID == user.UserGroupID
        //               select new 
        //               {
        //                   RoleID= cre.RoleID,
        //                   UserGroupID=cre.UserGroupID
        //               }).AsEnumerable().Select(x=>new Credential() {
        //                   RoleID=x.RoleID,
        //                   UserGroupID=x.UserGroupID
        //                });
        //    return data.Select(x=>x.RoleID).ToList();      
        //}

        public int Login(string UserName, string PassWord, bool IsLoginAdmin = false)
        {
            var result = db.Users.SingleOrDefault(x => x.Username == UserName);
            if (result == null)
            {
                return 0;
            }
            else
            {
                if (IsLoginAdmin == true)
                {
                    if (result.UserGroupID == CommonConstants.ADMIN_GROUP)
                    {
                        if (result.Status == false)
                        {
                            return -1;
                        }
                        else
                        {
                            if (result.Password == PassWord)
                            {
                                return 1;
                            }
                            else
                            {
                                return -2;
                            }
                        }
                    }
                    else
                    {
                        return -3; 

                    }
                }
                else
                {
                    if (result.Status == false)
                    {
                        return -1;
                    }
                    else
                    {
                        if (result.Password == PassWord)
                        {
                            return 1;
                        }
                        else
                        {
                            return -2;
                        }
                    }

                }
                
            }
        }

        public bool Delete(int id)
        {
            var user = db.Users.SingleOrDefault(x => x.ID == id);
            if (user == null)
            {
                return false;
            }
            db.Users.Remove(user);
            db.SaveChanges();
            return true;
        }

        //Kiểm tra Đăng ký
        public bool CheckUserName(string userName)
        {
            return db.Users.Count(u => u.Username == userName) > 0;
        }
        public bool CheckPhone(string phone)
        {
            return db.Users.Count(u => u.Phone == phone) > 0;
        }

        /*-------------- Thay đổi trạng thái --------------------*/
        public bool ChangeStatus(long id)
        {
            var user = db.Users.Find(id);
            user.Status = !user.Status;
            db.SaveChanges();
            return user.Status;
        }
    }
}
