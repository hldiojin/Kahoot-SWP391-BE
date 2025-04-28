using Microsoft.AspNetCore.Mvc;
using Service.IService; // Make sure this namespace is correct
using Repository.DTO;    // Make sure this namespace is correct
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

namespace API_Services.Controllers // Adjust this namespace to your project's convention
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupMemberController : ControllerBase
    {
        private readonly IGroupMemberService _groupMemberService;

        public GroupMemberController(IGroupMemberService groupMemberService)
        {
            _groupMemberService = groupMemberService;
        }

        [HttpGet("{groupId}/{playerId}")]
        public async Task<IActionResult> GetGroupMemberById(int groupId, int playerId)
        {
            var response = await _groupMemberService.GetGroupMemberByIdAsync(groupId, playerId);
            return StatusCode(response.Status, response); // Use StatusCode for consistent responses
        }

        [HttpGet("group/{groupId}")]
        public async Task<IActionResult> GetGroupMembersByGroupId(int groupId)
        {
            var response = await _groupMemberService.GetGroupMembersByGroupIdAsync(groupId);
            return StatusCode(response.Status, response);
        }

        [HttpPost]
        public async Task<IActionResult> AddGroupMember([FromBody] GroupMemberDTO groupMemberDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return 400 with validation errors
            }
            var response = await _groupMemberService.AddGroupMemberAsync(groupMemberDto);
            return StatusCode(response.Status, response);
        }

        [HttpPut("{groupId}/{playerId}")]
        public async Task<IActionResult> UpdateGroupMember(int groupId, int playerId, [FromBody] GroupMemberDTO groupMemberDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _groupMemberService.UpdateGroupMemberAsync(groupId, playerId, groupMemberDto);
            return StatusCode(response.Status, response);
        }

        [HttpDelete("{groupId}/{playerId}")]
        public async Task<IActionResult> DeleteGroupMember(int groupId, int playerId)
        {
            var response = await _groupMemberService.DeleteGroupMemberAsync(groupId, playerId);
            return StatusCode(response.Status, response);
        }

        [HttpGet("group/groupid/{groupId}")]
        public async Task<IActionResult> GetGroupMemberByGroupId(int groupId)
        {
            var response = await _groupMemberService.GetGroupMemberByGroupId(groupId);
            return StatusCode(response.Status, response);
        }

        [HttpGet("player/{playerId}")]
        public async Task<IActionResult> GetGroupMemberByPlayerId(int playerId)
        {
            var response = await _groupMemberService.GetGroupMemberByPlayerId(playerId);
            return StatusCode(response.Status, response);
        }
        [HttpPost("select-group")]
        public async Task<IActionResult> SelectGroup([FromBody] CreateGroupMemberDTO dto)
        {
            var response = await _groupMemberService.SelectGroup(dto);
            return StatusCode(response.Status, response);
        }
    }
}
