using Microsoft.AspNetCore.Mvc;
using Service.IService;
using static Repository.DTO.RequestDTO;

namespace API_Services.Controllers
{
    [ApiController]
    [Route("api/groups")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService ?? throw new ArgumentNullException(nameof(groupService));
        }

        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetGroupById(int groupId)
        {
            var response = await _groupService.GetGroupByIdAsync(groupId);
            return StatusCode(response.Status, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetGroups()
        {
            var response = await _groupService.GetGroupsAsync();
            return StatusCode(response.Status, response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] GroupDTO groupDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _groupService.CreateGroupAsync(groupDto);
            return StatusCode(response.Status, response);
        }

        [HttpPut("{groupId}")]
        public async Task<IActionResult> UpdateGroup(int groupId, [FromBody] GroupDTO groupDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _groupService.UpdateGroupAsync(groupId, groupDto);
            return StatusCode(response.Status, response);
        }

        [HttpDelete("{groupId}")]
        public async Task<IActionResult> DeleteGroup(int groupId)
        {
            var response = await _groupService.DeleteGroupAsync(groupId);
            return StatusCode(response.Status, response);
        }
    }
}
