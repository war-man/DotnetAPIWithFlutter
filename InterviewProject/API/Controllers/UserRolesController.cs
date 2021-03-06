﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Context;
using API.Models;
using API.Repository;
using API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Bcrypt = BCrypt.Net.BCrypt;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UsersController : ControllerBase
    {
        private readonly MyContext _context;
        public IConfiguration _configuration;
        UserRepository _repo;

        public UsersController(MyContext myContext, IConfiguration config, UserRepository repo)
        {
            _context = myContext;
            _configuration = config;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IEnumerable<GetUserVM>> GetAll() => await _repo.getAll();

        [HttpGet("{id}")]
        public GetUserVM GetID(int id) => _repo.getID(id);

        [HttpPost]
        public IActionResult Create(GetUserVM getUserVM)
        {
            var getUser = _context.Users.Where(x => x.Email == getUserVM.Email);
            if (getUser.Count() == 0)
            {
                if (ModelState.IsValid)
                {
                    var getRoleId = _context.Roles.SingleOrDefault(x => x.RoleName == getUserVM.RoleName);
                    var usr = new User
                    {
                        Name = getUserVM.Name,
                        Email = getUserVM.Email,
                        Password = Bcrypt.HashPassword(getUserVM.Password),
                        RoleId = getRoleId.RoleId,
                        VerifyCode = null,
                        CreateDate = DateTimeOffset.Now,
                        isDelete = false
                    };
                    _context.Users.Add(usr);
                    _context.SaveChanges();

                    return Ok("Successfully Created");
                }
                return BadRequest("Not Successfully");
            }
            return BadRequest("Email Already Exists ");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, GetUserVM dataVM)
        {
            if (ModelState.IsValid)
            {
                var getData = _context.Users.Include("Role").SingleOrDefault(x => x.Id == id);
                getData.Name = dataVM.Name;
                getData.Email = dataVM.Email;
                if (dataVM.Password != null)
                {
                    if (!Bcrypt.Verify(dataVM.Password, getData.Password))
                    {
                        getData.Password = Bcrypt.HashPassword(dataVM.Password);
                    }
                }
                if (dataVM.RoleName != null)
                {
                    var getRoleID = _context.Roles.SingleOrDefault(x => x.RoleName == dataVM.RoleName);
                    getData.RoleId = getRoleID.RoleId;
                }
                _context.Users.Update(getData);
                _context.SaveChanges();

                return Ok("Successfully Updated");
            }
            return BadRequest("Not Successfully");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var getData = _context.Users.SingleOrDefault(x => x.Id == id);
            if (getData == null)
            {
                return BadRequest("Not Successfully");
            }
            getData.DeleteDate = DateTimeOffset.Now;
            getData.isDelete = true;

            _context.Entry(getData).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok(new { msg = "Successfully Delete" });
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class RolesController : ControllerBase
    {
        private readonly MyContext _context;
        public IConfiguration _configuration;
        RoleRepository _repo;

        public RolesController(MyContext myContext, IConfiguration config, RoleRepository _repo)
        {
            _context = myContext;
            _configuration = config;
            this._repo = _repo;
        }

        [HttpGet]
        public async Task<IEnumerable<GetRoleVM>> GetAll() => await _repo.getAll();

        [HttpGet("{id}")]
        public GetRoleVM GetID(int id) => _repo.getID(id);

        [HttpPost]
        public IActionResult Create(GetRoleVM dataVM)
        {
            if (ModelState.IsValid)
            {
                var role = new Role
                {
                    RoleName = dataVM.Name,
                    CreateDate = DateTimeOffset.Now,
                    isDelete = false
                };
                _context.Roles.Add(role);
                _context.SaveChanges();
                return Ok("Successfully Created");
                //if (dataVM.Session == 0)
                //{
                //    return BadRequest("Session ID must be filled");
                //}
                //var getSession = _context.Users.SingleOrDefault(x => x.Id == dataVM.Session);
                //if (getSession != null)
                //{
                //    var role = new Role
                //    {
                //        RoleName = dataVM.Name,
                //        CreateDate = DateTimeOffset.Now,
                //        isDelete = false
                //    };
                //    _context.Roles.Add(role);
                //    _context.SaveChanges();
                //    return Ok("Successfully Created");
                //}
                //return BadRequest("You Don't Have Access");
            }
            return BadRequest("Not Successfully");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, GetRoleVM dataVM)
        {
            if (ModelState.IsValid)
            {
                var getData = _context.Roles.SingleOrDefault(x => x.RoleId == id);
                getData.RoleName = dataVM.Name;
                getData.UpdateDate = DateTimeOffset.Now;

                _context.Roles.Update(getData);
                _context.SaveChanges();

                return Ok("Successfully Updated");
                //if (dataVM.Session == 0)
                //{
                //    return BadRequest("Session ID must be filled");
                //}
                //var getSession = _context.Users.SingleOrDefault(x => x.Id == dataVM.Session);
                //if (getSession != null)
                //{
                //    var getData = _context.Roles.SingleOrDefault(x => x.RoleId == id);
                //    getData.RoleName = dataVM.Name;
                //    getData.UpdateDate = DateTimeOffset.Now;

                //    _context.Roles.Update(getData);
                //    _context.SaveChanges();

                //    return Ok("Successfully Updated");
                //}
                //return BadRequest("You Don't Have Access");
            }
            return BadRequest("Not Successfully");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var getData = _context.Roles.SingleOrDefault(x => x.RoleId == id);
            if (getData == null)
            {
                return BadRequest("Not Successfully");
            }
            getData.DeleteDate = DateTimeOffset.Now;
            getData.isDelete = true;

            _context.Entry(getData).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok(new { msg = "Successfully Delete" });
        }
    }
}
