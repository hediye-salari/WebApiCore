﻿using Common;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebFramwork.Api;


namespace MyApi.Controllers
{
    [Route("api/[controller]")]

    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;

        public UserController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        [HttpGet]
        public async Task<ApiResult<List<User>>> Get(CancellationToken cancellationToken)
        {
            var users = await userRepository.TableNoTracking.ToListAsync(cancellationToken);
            return users;
            //return new ApiResult<List<User>>

            //{
            //    IsSuccess = true,
            //    StatusCode  =ApiResultStatusCode.Success,
            //    Message="عملیات با موفقیت انجام شد ",
            //    Data = users


            //};
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> Get(int id, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(cancellationToken, id);
            return user;
            return NotFound
        }
        [HttpPost]
        public async Task Create(User user, CancellationToken cancellationToken)
        {
            await userRepository.AddAsync(user, cancellationToken);
        }
        [HttpPut]
        public async Task<IActionResult> Update(int id, User user, CancellationToken cancellationToken)
        {
            var updateUser = await userRepository.GetByIdAsync(cancellationToken, id);


            if (updateUser == null)
                return NotFound();


            updateUser.UserName= user.UserName;
            updateUser.PasswordHash= user.PasswordHash;
            updateUser.FullName= user.FullName;
            updateUser.Age= user.Age;
            updateUser.Gender= user.Gender;
            updateUser.IsActive= user.IsActive;
            updateUser.LastLoginDate= user.LastLoginDate;

            await userRepository.UpdateAsync(updateUser, cancellationToken);
            return Ok();
        }
        public async Task<ActionResult> Delete(int id, User user, CancellationToken cancellationToken)
        {
            var DeleteUser = await userRepository.GetByIdAsync(cancellationToken, id);
            await userRepository.DeleteAsync(DeleteUser, cancellationToken);
            return Ok();
        }

    }
}
