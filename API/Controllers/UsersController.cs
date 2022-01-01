using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return Ok(await _userRepository.GetMembersAsync());

        }

        [HttpGet]
        [Route("{username}")]
        public async Task<ActionResult<AppUser>> GetUserByName(string username)
        {
            return Ok(await _userRepository.GetMemberByUsernameAsync(username));
        }

        [HttpPut]
        public async Task<ActionResult> UpdateMember(MemberUpdateDto memberUpdateDto)
        {
            var currentUserName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUsernameAsync(currentUserName);

            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            if (await _userRepository.SaveALlAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }
    }
}