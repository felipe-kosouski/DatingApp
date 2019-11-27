using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatingApi.Data;
using DatingApi.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using DatingApi.Models;

namespace DatingApi.Controllers
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly IDatingRepository _datingRepository;
    private readonly IMapper _mapper;

    public UsersController(IDatingRepository datingRepository, IMapper mapper)
    {
      _mapper = mapper;
      _datingRepository = datingRepository;
    }

    // GET api/users
    [HttpGet()]
    public async Task<IActionResult> GetUsers()
    {
      var users = await _datingRepository.GetUsers();

      var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
      return Ok(usersToReturn);
    }

    // GET api/users/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
      var user = await _datingRepository.GetUser(id);
      var userToReturn = _mapper.Map<UserForDetailedDto>(user);
      return Ok(userToReturn);
    }

    // POST api/users
    [HttpPost("")]
    public void Poststring(string value)
    {
    }

    // PUT api/users/5
    [HttpPut("{id}")]
    public void Putstring(int id, string value)
    {
    }

    // DELETE api/users/5
    [HttpDelete("{id}")]
    public void DeletestringById(int id)
    {
    }
  }
}